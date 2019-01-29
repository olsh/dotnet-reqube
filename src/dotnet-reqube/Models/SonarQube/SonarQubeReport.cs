using System.Collections.Generic;

using Newtonsoft.Json;

namespace ReQube.Models.SonarQube
{
    public class SonarQubeReport
    {
        public SonarQubeReport()
        {
            Issues = new LinkedList<Issue>();
        }

        [JsonIgnore]
        public static SonarQubeReport Empty { get; } = new SonarQubeReport();

        [JsonProperty("issues")]
        public ICollection<Issue> Issues { get; set; }

        [JsonIgnore]
        public string ProjectName { get; set; }
    }
}
