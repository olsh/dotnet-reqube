using CommandLine;

namespace ReQube.Models
{
    internal class Options
    {
        [Option('i', "input", Required = true, HelpText = "ReSharper report in XML format.")]
        public string Input { get; set; }

        [Option('o', "output", Required = true, HelpText = "SonarQube report file name.")]
        public string Output { get; set; }

        [Option('d', "directory", Required = false, HelpText = "Directory where reports will be saved. Working directory will be used if not set.")]
        public string Directory { get; set; }
    }
}
