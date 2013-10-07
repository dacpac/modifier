﻿using System;
using System.Text;
using CommandLine;
using CommandLine.Text;

namespace dacpacModifier
{
    class Options
    {
        [Option('i', "input", Required = true, HelpText = "Input dacpac to read.")]
        public string InputFile { get; set; }

        [Option('e', "element", Required = true, HelpText = "The Element Type(s) to remove in the model file, use ; to split type(s)")]
        public string ElementTypes { get; set; }

        [Option('p', "property", HelpText = "Remove property type(s) by name, use ; to split type(s)")]
        public string PropertyTypes { get; set; }

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