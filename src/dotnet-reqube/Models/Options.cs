using CommandLine;

namespace ReQube.Models
{
    internal class Options
    {
        [Option('i', "input", Required = true, HelpText = "ReSharper report in XML format.")]
        public string Input { get; set; }

        [Option('o', "output", Required = true, HelpText = "Path where SonarQube report will be saved.")]
        public string Output { get; set; }
    }
}
