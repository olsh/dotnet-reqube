using Newtonsoft.Json;

namespace ReQube.Models.SonarQube.Roslyn
{
    public class Rule
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("shortDescription")]
        public string ShortDescription { get; set; }

        [JsonProperty("fullDescription")]
        public string FullDescription { get; set; }

        [JsonProperty("defaultLevel")]
        public string DefaultLevel { get; set; }

        [JsonProperty("helpUrl")]
        public string HelpUrl { get; set; }
    }
}
