using Newtonsoft.Json;

namespace ReQube.Models.SonarQube
{
    public class TextRange
    {
        [JsonProperty("endColumn")]
        public int EndColumn { get; set; }

        [JsonProperty("endLine")]
        public int EndLine { get; set; }

        [JsonProperty("startColumn")]
        public int StartColumn { get; set; }

        [JsonProperty("startLine")]
        public int StartLine { get; set; }
    }
}
