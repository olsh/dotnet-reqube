using System.Collections.Generic;

using Newtonsoft.Json;

namespace ReQube.Models.SonarQube
{
    public class SonarQubeReport
    {
        [JsonProperty("issues")]
        public ICollection<Issue> Issues { get; set; }
    }
}
