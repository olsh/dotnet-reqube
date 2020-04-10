using CommandLine;

namespace ReQube.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Options
    {
        [Option('i', "input", Required = true, HelpText = "ReSharper report in XML format.")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Input { get; set; }

        [Option('o', "output", Required = true, HelpText = "SonarQube report file name.")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Output { get; set; }

        [Option(
            'd', 
            "directory", 
            Required = false, 
            HelpText = "Directory where reports will be saved. Working directory will be used if not set.")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Directory { get; set; }

        [Option(
            'p', 
            "project", 
            Required = false,
            HelpText = "Project to create SonarQube report for. If not set, a report is written for all projects found in the solution.")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Project { get; set; }

        [Option(
            'f', 
            "format", 
            Required = false, 
            HelpText = "SonarQube report output format.", 
            Default = SonarOutputFormat.Generic)]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public SonarOutputFormat OutputFormat { get; set; }

        [Option(
            "exclude-rules",
            Required = false,
            HelpText = 
                "Specify the ReSharper rules to be excluded from the analysis (Issue[TypeId] from ReSharper's output). " 
                + "The format is <type id>[##<message regex>]|<type id>[##<message regex>]..." 
                + "E.g. CSharpErrors##The modifier 'public'.*|UnusedMemberInSuper.Global|NotAccessedField.Global")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string ExcludedRules { get; set; }

        [Option(
            "sonar-dir",
            Required = false,
            HelpText = "The path to the .sonarqube directory of the executing sonar analysis.")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string SonarDirectory { get; set; }
    }
}
