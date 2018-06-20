using System.Collections.Generic;

using Newtonsoft.Json;

namespace ReQube.Models.SonarQube
{
    public class Issue
    {
        [JsonProperty("effortMinutes")]
        public int? EffortMinutes { get; set; }

        [JsonProperty("engineId")]
        public string EngineId { get; set; }

        [JsonProperty("primaryLocation")]
        public PrimaryLocation PrimaryLocation { get; set; }

        [JsonProperty("ruleId")]
        public string RuleId { get; set; }

        [JsonProperty("secondaryLocations")]
        public IList<SecondaryLocation> SecondaryLocations { get; set; }

        [JsonProperty("severity")]
        public string Severity { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
