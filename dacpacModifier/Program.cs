using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using CommandLine;

namespace dacpacModifier
{
    class Program
    {
        static void Main(string[] args)
        {
            var ConsoleArgs = new ConsoleArgs();
            if (Parser.Default.ParseArguments(args, ConsoleArgs))
            {
                // consume Options instance properties
                if (ConsoleArgs.Verbose)
                {
                    Console.WriteLine(ConsoleArgs.InputFile);
                }
                else
                    Console.WriteLine("working ...");
            }


            //Package dacpac = Package.Open(daclocation, FileMode.Open, FileAccess.Read);
            //try
            //{
            //    Uri originUri = PackUriHelper.CreatePartUri(new Uri("/origin.xml", UriKind.Relative));
            //    Uri modelUri = PackUriHelper.CreatePartUri(new Uri("/model.xml", UriKind.Relative));

            //    XDocument dacOriginXml = XDocument.Load(XmlReader.Create(dacpac.GetPart(originUri).GetStream()));
            //    XDocument dacModelXml = XDocument.Load(XmlReader.Create(dacpac.GetPart(modelUri).GetStream()));

            //}
            //catch (Exception exception)
            //{
            //    throw;
            //}

            //if (!(dacpac == null))
            //{
            //   dacpac.Close();
            //}

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
