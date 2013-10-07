using System;
using System.IO;
using System.Security.Cryptography;

namespace dacpacModifier
{
    class Checksum
    {
        public static byte[] CalculateChecksum(Stream _stream)
        {
            if (_stream == null)
            {
                throw new ArgumentNullException("Stream is null");
            }

            return HashAlgorithm.Create("System.Security.Cryptography.SHA256CryptoServiceProvider").ComputeHash(_stream);
        }
    }
}
