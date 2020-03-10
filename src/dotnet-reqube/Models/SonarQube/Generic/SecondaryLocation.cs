using Newtonsoft.Json;

namespace ReQube.Models.SonarQube.Generic
{
    public class SecondaryLocation
    {
        [JsonProperty("filePath")]
        public string FilePath { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("textRange")]
        public TextRange TextRange { get; set; }
    }
}
