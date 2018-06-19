using System.Collections.Generic;

namespace ReQube.Models
{
    public class Constants
    {
        public const string EngineId = "R#";

        public const string SonarQubeCodeSmellType = "CODE_SMELL";

        public static IDictionary<string, string> ReSharperToSonarQubeSeverityMap =>
            new Dictionary<string, string>
                {
                    { "ERROR", "CRITICAL" },
                    { "WARNING", "MAJOR" },
                    { "SUGGESTION", "MINOR" },
                    { "HINT", "INFO" }
                };
    }
}
