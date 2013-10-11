using System;
using System.IO;
using System.IO.Packaging;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Text;

namespace dacpacModifier
{
    class Remove
    {
        /// <summary>
        /// Open's a dacpac and removes element/property types based on the console input -e/-p
        /// </summary>
        /// <param name="InputFile">The Input dacpac to open and remove elements from</param>
        /// <param name="args">The arguments to consume</param>
        public static void RemoveElements(FileInfo InputFile, Options args)
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

                // Remove Elements by type and name if supplied
                if (args.ElementTypeName != null)
                {
                    string[] _elementTypeNames = args.ElementTypeName.Split(';');
                    foreach (string etn in _elementTypeNames)
                    {
                        string[] _elementTypes = etn.Split('~');
                        dacModelXml.Root.Descendants(xmls + "Model").Elements(xmls + "Element")
                               .Where(x => x.Attribute("Type").Value == _elementTypes[0])
                               .Where(x => x.Attribute("Name").Value == _elementTypes[1])
                               .Remove();
                    }
                }

                // Stream required to reset the length as data still gets appended when saving
                Stream _stream = dacModelPart.GetStream(FileMode.Open, FileAccess.Write);
                _stream.SetLength(0);

                // Convert to string so xml can be formatted!
                string stringDacModel = dacModelXml.ToString();

                if (args.StringReplace != null)
                {
                    string[] arrStr = args.StringReplace.Split('~');

                    if (args.Verbose)
                    {
                        Console.WriteLine("Performing string replacement: Replacing {0} with {1}", arrStr[0], arrStr[1]);
                    }
                    if (arrStr[1] == "null")
                    {
                        stringDacModel = stringDacModel.Replace(arrStr[0], String.Empty);

                    }
                    else
                    {
                        stringDacModel = stringDacModel.Replace(arrStr[0], arrStr[1]);
                    }
                }

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

        /// <summary>
        /// Open's a dacpac and removes element/property types based on the console input -e/-p
        /// And then Import Custom Parts to the model xml file
        /// </summary>
        /// <param name="InputFile">The Input dacpac to open and remove elements from</param>
        /// <param name="args">The arguments to consume</param>
        /// <param name="CustomPartsInputFile">The Custom Parts Input file to import</param>
        public static void RemoveAddElements(FileInfo InputFile, Options args, FileInfo CustomPartsInputFile)
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

                // Remove Elements by type and name if supplied
                if (args.ElementTypeName != null)
                {
                    string[] _elementTypeNames = args.ElementTypeName.Split(';');
                    foreach (string etn in _elementTypeNames)
                    {
                        string[] _elementTypes = etn.Split('~');
                        dacModelXml.Root.Descendants(xmls + "Model").Elements(xmls + "Element")
                               .Where(x => x.Attribute("Type").Value == _elementTypes[0])
                               .Where(x => x.Attribute("Name").Value == _elementTypes[1])
                               .Remove();
                    }
                }

                // Stream required to reset the length as data still gets appended when saving
                Stream _stream = dacModelPart.GetStream(FileMode.Open, FileAccess.Write);
                _stream.SetLength(0);

                // Convert to string so xml can be formatted!
                string stringDacModel = dacModelXml.ToString();

                if (args.StringReplace != null)
                {
                    string[] arrStr = args.StringReplace.Split('~');

                    if (args.Verbose)
                    {
                        Console.WriteLine("Performing string replacement: Replacing {0} with {1}", arrStr[0], arrStr[1]);
                    }
                    if (arrStr[1] == "null")
                    {
                        stringDacModel = stringDacModel.Replace(arrStr[0], String.Empty);

                    }
                    else
                    {
                        stringDacModel = stringDacModel.Replace(arrStr[0], arrStr[1]);
                    }
                }

                XDocument newDacModelXml = XDocument.Parse(stringDacModel);
                // Add the custom parts

                XElement customPartsXml = XElement.Load(XmlReader.Create(args.CustomPartsInputFile));
                var customParts = customPartsXml.Descendants(xmls + "Model").Elements(xmls + "Element");

                var pointer = newDacModelXml.Root.Descendants(xmls + "Model").Elements(xmls + "Element").Last();

                pointer.Parent.Add(customParts);

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
