using Newtonsoft.Json;

namespace ReQube.Models.SonarQube.Roslyn
{
    public class Location
    {
        [JsonProperty("resultFile")]
        public ResultFile ResultFile { get; set; }
    }
}
