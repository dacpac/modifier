using System;
using System.Text;
using CommandLine;
using CommandLine.Text;

namespace dacpacModifier
{
    class ConsoleArgs
    {
        [Option('i', "input", Required = true, HelpText = "Input dacpac to read.")]
        public string InputFile { get; set; }

        [Option('v', "verbose", HelpText = "Print details during execution.")]
        public bool Verbose { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Heading = new HeadingInfo("dacpacModifier", "v 0.1"),
                Copyright = new CopyrightInfo("Craig Ottley-Thistlethwaite", DateTime.Now.Year),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };
            help.AddPreOptionsLine("twitter:@Craig_Ottley");
            help.AddOptions(this);
            return help;
        }
    }
}
