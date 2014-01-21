using System;
using System.IO;
using System.IO.Packaging;
using System.Text;
using System.Linq;

namespace dacpacModifier
{
    class Modifier
    {
        /// <summary>
        /// First checks if file exists, then checks the file is a valid dacpac and 
        /// will then go through the dacpac and remove all elements of the specified type
        /// </summary>
        /// <param name="args">The parsed arguments</param>
        public static void Modify(Options args)
        {
            // Check to see if anything has been supplied for input file
            if (string.IsNullOrEmpty(args.InputFile))
            {
                throw new ApplicationException(Constants.ErrorNoInputFile);
            }
            // Check if the file exists
            FileInfo fi = new FileInfo(args.InputFile);
            if (!fi.Exists)
            {
                throw new FileNotFoundException(args.InputFile);
            }
            // Check the file is a valid dacpac
            if (!Checks.IsDacPac(fi, args))
            {
                throw new FileFormatException(Constants.ErrorNotAValidDacpac);
            }

            // Import custom parts to model xml
            if (args.CustomPartsInputFile != null)
            {
                string[] _partsFile = args.CustomPartsInputFile.Split(';').Distinct().ToArray();

                foreach (string part_file in _partsFile)
                {
                    FileInfo cp = new FileInfo(part_file);
                    if (!cp.Exists)
                    {
                        throw new FileNotFoundException(part_file);
                    }
                }
                // Write to console if verbose
                if (args.Verbose)
                {
                    OutputToConsole(args);
                }
                Remove.RemoveAddElements(fi, args);
            }
            else
            {
                if (args.Verbose)
                {
                    OutputToConsole(args);
                }
                Remove.RemoveElements(fi, args);
            }



        }

        public static void OutputToConsole(Options args)
        {
            Console.WriteLine("Input File: {0}", args.InputFile);
            if (args.ElementTypes != null)
            {
                Console.WriteLine("Removing Element Type(s): {0}", args.ElementTypes);
            }
            if (args.PropertyTypes != null)
            {
                Console.WriteLine("Removing Property Type(s): {0}", args.PropertyTypes);
            }
            if (args.CustomPartsInputFile != null)
            {
                Console.WriteLine("Adding Custom XML Part(s): {0}", args.CustomPartsInputFile);
            }
            if (args.RemoveElementTypeName != null)
            {
                Console.WriteLine("Removing Element Type(s) and Name(s): {0}", args.RemoveElementTypeName);
            }
            if (args.KeepElementTypeName != null)
            {
                Console.WriteLine("Keeping Element Type(s) and Name(s): {0}", args.KeepElementTypeName);
            }



        }

    }
}

