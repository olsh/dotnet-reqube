using Newtonsoft.Json;
using System.Collections.Generic;

namespace ReQube.Models.SonarQube.Roslyn
{
    public class Run
    {
        [JsonProperty("results")]
        public ICollection<Result> Results { get; set; } = new LinkedList<Result>();

        [JsonProperty("rules")]
        public IDictionary<string, Rule> Rules { get; set; } = new Dictionary<string, Rule>();
    }
}
