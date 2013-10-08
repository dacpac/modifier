using System;
using System.IO;
using System.IO.Packaging;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Text;

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
            if(!Checks.IsDacPac(fi, args))
            {
                throw new FileFormatException(Constants.ErrorNotAValidDacpac);
            }
            // Write to console if verbose
            if (args.Verbose)
            {
                Console.WriteLine("Input File: {0}", args.InputFile);
                Console.WriteLine("Removing Element Type(s): {0}", args.ElementTypes);
                if (args.PropertyTypes != null)
                {
                    Console.WriteLine("Removing Property Type(s): {0}", args.PropertyTypes);
                }
            }
            RemoveElementTypes(fi, args);
        }

        /// <summary>
        /// Open's a dacpac and removes element/property types based on the console input -e/-p
        /// </summary>
        /// <param name="InputFile">The Input dacpac to open and remove elements from</param>
        /// <param name="args">The arguments to consume</param>
        public static void RemoveElementTypes(FileInfo InputFile, Options args)
        {
            Package dacpac = Package.Open(InputFile.FullName, FileMode.Open);
            try
            {
                string[] _elements = args.ElementTypes.Split(';');

                Uri originUri = PackUriHelper.CreatePartUri(new Uri(Constants.OriginXmlUri, UriKind.Relative));
                Uri modelUri = PackUriHelper.CreatePartUri(new Uri(Constants.ModelXmlUri, UriKind.Relative));

                PackagePart dacOriginPart = dacpac.GetPart(originUri);
                PackagePart dacModelPart = dacpac.GetPart(modelUri);

                XDocument dacOriginXml = XDocument.Load(XmlReader.Create(dacOriginPart.GetStream()));
                XDocument dacModelXml = XDocument.Load(XmlReader.Create(dacModelPart.GetStream()));

                XNamespace xmls = Constants.DacOriginXmlns;

                XElement checksumElement = (from cs in dacOriginXml.Descendants(xmls + "Checksum")
                                            select cs).FirstOrDefault();
                
                if (checksumElement != null)
                {
                    if (args.Verbose)
                    {
                        Console.WriteLine("Checksum exists {0}", checksumElement.Value);
                    }
                }

                foreach (string ElementType in _elements)
                {
                    dacModelXml.Root.Descendants(xmls + "Model").Elements(xmls + "Element")
                               .Where(x => x.Attribute("Type").Value == ElementType)
                               .Remove();
                }

                if (args.PropertyTypes != null)
                {
                    string[] _properties = args.PropertyTypes.Split(';');
                    foreach (string PropertyType in _properties)
                    {
                        dacModelXml.Root.Descendants(xmls + "Property")
                                   .Where(x => x.Attribute("Name").Value == PropertyType)
                                   .Remove();
                    }
                }
                
                // Stream required to reset the length as data still gets appended when saving
                Stream _stream = dacModelPart.GetStream(FileMode.Open, FileAccess.Write);
                _stream.SetLength(0);

                // Convert to string so xml can be formatted!
                string stringDacModel = dacModelXml.ToString();

                XDocument newDacModelXml = XDocument.Parse(stringDacModel);
                try
                {
                    newDacModelXml.Save(_stream, SaveOptions.None);
                    // Need to regenerate the checksum as it's been tempered with
                    byte[] byteArray = Checksum.CalculateChecksum(dacModelPart.GetStream());
                    string readableByteArray = string.Concat(byteArray.Select(s => s.ToString("X2")));
                    // Write the new checksum value
                    checksumElement.Value = readableByteArray;
                    if (args.Verbose)
                    {
                        Console.WriteLine("Calculated Checksum {0}", checksumElement.Value);
                    }

                    XmlWriterSettings _xmlWriterSettings = new XmlWriterSettings();
                    _xmlWriterSettings.Encoding = Encoding.UTF8;
                    XmlWriter _xmlWriter = XmlWriter.Create(dacOriginPart.GetStream(FileMode.Open, FileAccess.Write), _xmlWriterSettings);

                    try
                    {
                        dacOriginXml.Save(_xmlWriter);
                    }
                    finally
                    {
                        if (_xmlWriter != null)
                        {
                            _xmlWriter.Close();
                            //Console.WriteLine("Press any key to continue...");
                            //Console.ReadKey();
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            catch (Exception exception) 
            {
                throw exception;
            }
            finally 
            {
                if (!(dacpac == null))
                {
                    dacpac.Close();
                }

            }



        }
    }
}

