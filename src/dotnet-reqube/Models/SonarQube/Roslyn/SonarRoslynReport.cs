using Newtonsoft.Json;
using System.Collections.Generic;

namespace ReQube.Models.SonarQube.Roslyn
{
    class SonarRoslynReport : ISonarReport
    {
        private const string DefaultSchema = "http://json.schemastore.org/sarif-1.0.0";
        private const string DefaultVersion = "1.0.0";

        [JsonProperty("runs")]
        public ICollection<Run> Runs { get; set; } = new LinkedList<Run>();

        [JsonIgnore]
        public string ProjectName { get; set; }

        [JsonProperty("$schema")]
        public string Schema { get; set; } = DefaultSchema;

        [JsonProperty("version")]
        public string Version { get; set; } = DefaultVersion;
    }
}
