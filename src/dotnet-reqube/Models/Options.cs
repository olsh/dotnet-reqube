using CommandLine;

namespace ReQube.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class Options
    {
        [Option('i', "input", Required = true, HelpText = "ReSharper report in XML format.")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Input { get; set; }

        [Option('o', "output", Required = true, HelpText = "SonarQube report file name.")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Output { get; set; }

        [Option('d', "directory", Required = false, HelpText = "Directory where reports will be saved. Working directory will be used if not set.")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Directory { get; set; }

        [Option('p', "project", Required = true,
            HelpText = "Project to create SonarQube report for. If not set, a report is written for all projects found in the solution.")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Project { get; set; }
    }
}
