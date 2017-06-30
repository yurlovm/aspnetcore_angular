using Newtonsoft.Json;

namespace WEBAPP.WebSockets
{
    public class InvocationDescriptor
    {
        [JsonProperty("methodName")]
        public string MethodName { get; set; }

        [JsonProperty("arguments")]
        public object[] Arguments { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
