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
    }
}
