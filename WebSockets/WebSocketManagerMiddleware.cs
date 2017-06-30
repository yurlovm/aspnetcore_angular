﻿using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WEBAPP.WebSockets
{
    /// <summary>
    /// WebSocket manager middleware
    /// </summary>
    public class WebSocketManagerMiddleware
    {
        private readonly RequestDelegate _next;
        private WebSocketHandler _webSocketHandler { get; set; }

        public WebSocketManagerMiddleware(RequestDelegate next, WebSocketHandler webSocketHandler)
        {
            _next = next;
            _webSocketHandler = webSocketHandler;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next.Invoke(context);
                return;
            }

            var socket = await context.WebSockets.AcceptWebSocketAsync();
            await _webSocketHandler.OnConnected(socket);

            await Receive(socket, async (result, serializedInvocationDescriptor) =>
            {
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    try
                    {
                        await _webSocketHandler.ReceiveAsync(socket, result, serializedInvocationDescriptor);
                    }
                    catch (Exception)
                    {
                        throw; //let's not swallow any exception for now
                    }
                    return;
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    try
                    {
                        await _webSocketHandler.OnDisconnected(socket);
                    }
                    catch (WebSocketException)
                    {
                        throw; //let's not swallow any exception for now
                    }

                    return;
                }

            });
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, string> handleMessage)
        {
            while (socket.State == WebSocketState.Open)
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024 * 4]);
                string serializedInvocationDescriptor = null;
                WebSocketReceiveResult result = null;
                try
                {
                    using (var ms = new MemoryStream())
                    {
                        do
                        {
                            result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                            ms.Write(buffer.Array, buffer.Offset, result.Count);
                        }
                        while (!result.EndOfMessage);

                        ms.Seek(0, SeekOrigin.Begin);

                        using (var reader = new StreamReader(ms, Encoding.UTF8))
                        {
                            serializedInvocationDescriptor = await reader.ReadToEndAsync();
                        }
                    }

                    handleMessage(result, serializedInvocationDescriptor);
                }
                catch (WebSocketException ex)
                {

                    if (ex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
                    {
                        socket.Abort();
                    }
                }

            }
        }

    }
}
