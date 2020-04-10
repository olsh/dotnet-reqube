using System.Collections.Generic;

using Newtonsoft.Json;

namespace ReQube.Models.SonarQube.Generic
{
    public class SonarGenericReport : ISonarReport
    {
        public SonarGenericReport()
        {
            Issues = new LinkedList<Issue>();
        }

        [JsonIgnore]
        public static SonarGenericReport Empty { get; } = new SonarGenericReport();

        [JsonProperty("issues")]
        public ICollection<Issue> Issues { get; set; }

        [JsonIgnore]
        public string ProjectName { get; set; }
    }
}
