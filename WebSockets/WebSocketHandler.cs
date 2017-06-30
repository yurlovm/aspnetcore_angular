using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace WEBAPP.WebSockets
{
    /// <summary>
    /// Class that handles connection and manages messages from the socket
    /// </summary>
    public abstract class WebSocketHandler
    {
        protected WebSocketConnectionManager WebSocketConnectionManager { get; set; }
        private readonly IServiceProvider _serviceProvider;
        private JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public WebSocketHandler(WebSocketConnectionManager webSocketConnectionManager, IServiceProvider serviceProvider)
        {
            WebSocketConnectionManager = webSocketConnectionManager;
            _serviceProvider = serviceProvider;
        }

        public virtual async Task OnConnected(WebSocket socket)
        {
            WebSocketConnectionManager.AddSocket(socket);

            await SendMessageAsync(socket, new Message()
            {
                MessageType = MessageType.ConnectionEvent,
                Data = WebSocketConnectionManager.GetId(socket)
            });
        }

        public virtual async Task OnDisconnected(WebSocket socket)
        {
            await WebSocketConnectionManager.RemoveSocket(WebSocketConnectionManager.GetId(socket));
        }

        public async Task SendMessageAsync(WebSocket socket, Message message)
        {
            if (socket.State != WebSocketState.Open) return;

            var serializedMessage = JsonConvert.SerializeObject(message, jsonSerializerSettings);
            var encodedMessage = Encoding.UTF8.GetBytes(serializedMessage);

            await socket.SendAsync(buffer: new ArraySegment<byte>(array: encodedMessage, offset: 0, count: encodedMessage.Length),
                                   messageType: WebSocketMessageType.Text,
                                   endOfMessage: true,
                                   cancellationToken: CancellationToken.None);
        }

        public async Task SendMessageAsync(string socketId, Message message)
        {
            await SendMessageAsync(WebSocketConnectionManager.GetSocketById(socketId), message);
        }

        public async Task SendMessageToAllAsync(Message message)
        {
            foreach (var pair in WebSocketConnectionManager.GetAll())
            {
                try
                {
                    if (pair.Value.State == WebSocketState.Open) await SendMessageAsync(pair.Value, message).ConfigureAwait(false);
                }
                catch (WebSocketException e)
                {
                    if (e.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
                    {
                        await OnDisconnected(pair.Value);
                    }
                }
            }
        }

        public async Task InvokeClientMethodAsync(string socketId, string methodName, object[] arguments)
        {
            var message = new Message()
            {
                MessageType = MessageType.ClientMethodInvocation,
                Data = JsonConvert.SerializeObject(new InvocationDescriptor()
                {
                    MethodName = methodName,
                    Arguments = arguments
                }, jsonSerializerSettings)
            };

            await SendMessageAsync(socketId, message);
        }

        public async Task InvokeClientMethodToAllAsync(string methodName, params object[] arguments)
        {
            foreach (var pair in WebSocketConnectionManager.GetAll())
            {
                try
                {
                    if (pair.Value.State == WebSocketState.Open) await InvokeClientMethodAsync(pair.Key, methodName, arguments);
                }
                catch (WebSocketException e)
                {
                    if (e.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
                    {
                        await OnDisconnected(pair.Value);
                    }
                }
            }
        }

        public virtual async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, string serializedInvocationDescriptor)
        {
            InvocationDescriptor invocationDescriptor;

            try
            {
                invocationDescriptor = JsonConvert.DeserializeObject<InvocationDescriptor>(serializedInvocationDescriptor);
            }
            catch (Exception ex)
            {
                await SendMessageAsync(socket, new Message()
                {
                    MessageType = MessageType.Text,
                    Data = $"Wrong request json format: {ex.Message}"
                });
                return;
            }


            if (string.IsNullOrWhiteSpace(invocationDescriptor.Token))
            {
                await SendMessageAsync(socket, new Message()
                {
                    MessageType = MessageType.Text,
                    Data = $"Wrong authentication token format"
                });
                return;
            }

            // Validate token here!
            SecurityToken validatedToken;
            ClaimsPrincipal user;
            try
            {
                var validationParameters = _serviceProvider.GetService<TokenValidationParameters>();
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                user = handler.ValidateToken(invocationDescriptor.Token, validationParameters, out validatedToken);
            }
            catch (Exception ex)
            {
                await SendMessageAsync(socket, new Message()
                {
                    MessageType = MessageType.Text,
                    Data = $"Authentication of token is failed"
                });
                return;
            }
            

            if(!user.Identity.IsAuthenticated)
            {
                await SendMessageAsync(socket, new Message()
                {
                    MessageType = MessageType.Text,
                    Data = $"Authentication of token is failed"
                });
                return;
            }

            var method = GetType().GetMethod(invocationDescriptor.MethodName);

            if (method == null)
            {
                await SendMessageAsync(socket, new Message()
                {
                    MessageType = MessageType.Text,
                    Data = $"Cannot find method {invocationDescriptor.MethodName}"
                });
                return;
            }

            try
            {
                object[] args = new object[invocationDescriptor.Arguments.Length + 1];
                Array.Copy(invocationDescriptor.Arguments, args, invocationDescriptor.Arguments.Length);
                args[invocationDescriptor.Arguments.Length] = socket;
                method.Invoke(this, args);
            }
            catch (TargetParameterCountException)
            {
                await SendMessageAsync(socket, new Message()
                {
                    MessageType = MessageType.Text,
                    Data = $"The {invocationDescriptor.MethodName} method does not take {invocationDescriptor.Arguments.Length} parameters!"
                });
            }

            catch (ArgumentException)
            {
                await SendMessageAsync(socket, new Message()
                {
                    MessageType = MessageType.Text,
                    Data = $"The {invocationDescriptor.MethodName} method takes different arguments!"
                });
            }
        }
    }
}
