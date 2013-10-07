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
        public static string daclocation = @"C:\code\xyz_dms_common.dacpac";

        static void Main(string[] args)
        {
            try
            {
                var options = new Options();
                               
                if (Parser.Default.ParseArguments(args, options))
                {
                    Modifier.Modify(options);
                }
            }
            catch (ArgumentException ae)
            {
                ArgumentException _arguementException = ae;
                Console.WriteLine(Constants.ErrorInvalidParameter, _arguementException.Message);
            }
            catch (FileNotFoundException fnfe)
            {
                FileNotFoundException _fileNotFound = fnfe;
                Console.WriteLine(Constants.ErrorFileNotFound, _fileNotFound.FileName);
            }
            catch (FileFormatException ffe)
            {
                FileFormatException _fileFormatException = ffe;
                Console.WriteLine(Constants.ErrorNotAValidDacpac, _fileFormatException.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
