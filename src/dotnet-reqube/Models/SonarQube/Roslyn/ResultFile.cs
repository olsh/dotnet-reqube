using Newtonsoft.Json;

namespace ReQube.Models.SonarQube.Roslyn
{
    public class ResultFile
    {
        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("region")]
        public TextRange Region { get; set; }
    }
}
