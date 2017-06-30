using Newtonsoft.Json;

namespace WEBAPP.Models
{
    public class Crisis
    {
        [JsonProperty(PropertyName ="id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}