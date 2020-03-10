using Newtonsoft.Json;
using System.Collections.Generic;

namespace ReQube.Models.SonarQube.Roslyn
{
    public class Result
    {
        [JsonProperty("ruleId")]
        public string RuleId { get; set; }

        [JsonProperty("level")]
        public string Level { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("locations")]
        public ICollection<Location> Locations { get; set; } = new LinkedList<Location>();
    }
}
