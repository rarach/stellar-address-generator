using System;
using System.Collections.Generic;
using System.Linq;

//Taken from https://github.com/elucidsoft/dotnet-stellar-sdk/blob/master/stellar-dotnetcore-sdk/StrKey.cs
namespace StellarAddressGenerator.Stellar_DotNetCore_SDK
{
    public class StrKey
    {
        public enum VersionByte : byte
        {
            ACCOUNT_ID = 6 << 3,
            SEED = 18 << 3
        }


        public static string EncodeCheck(VersionByte versionByte, byte[] data)
        {
            var bytes = new List<byte>
            {
                (byte) versionByte
            };

            bytes.AddRange(data);
            var checksum = CalculateChecksum(bytes.ToArray());
            bytes.AddRange(checksum);
            return Base32Encoding.ToString(bytes.ToArray());
        }

        protected static byte[] CalculateChecksum(byte[] bytes)
        {
            // This code calculates CRC16-XModem checksum
            // Ported from https://github.com/alexgorbatchev/node-crc
            var crc = 0x0000;
            var count = bytes.Length;
            var i = 0;
            int code;

            while (count > 0)
            {
                code = (int)(uint)crc >> (8 & 0xFF);
                code ^= bytes[i++] & 0xFF;
                code ^= (int)(uint)code >> 4;
                crc = (crc << 8) & 0xFFFF;
                crc ^= code;
                code = (code << 5) & 0xFFFF;
                crc ^= code;
                code = (code << 7) & 0xFFFF;
                crc ^= code;
                count--;
            }

            // little-endian
            return new[]
            {
                (byte) crc,
                (byte) ((uint) crc >> 8)
            };
        }
    }
}