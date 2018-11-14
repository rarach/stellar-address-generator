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


        /// <summary>
        /// Special method that 'fixes' public address that has bad checksum (last 2 bytes) but is otherwise correct. No checks
        /// are made to verify that the key doesn't actually have other problems.
        /// </summary>
        /// <param name="badPublicKey">Public stellar address with incorrect last 2 bytes</param>
        /// <returns>Corrected public key</returns>
        public static string CorrectEd25519PublicKey(string badPublicKey)
        {
            var decoded = Base32Encoding.ToBytes(badPublicKey);

            var payload = new byte[decoded.Length - 2];
            Array.Copy(decoded, 0, payload, 0, payload.Length);

            var data = new byte[payload.Length - 1];
            Array.Copy(payload, 1, data, 0, data.Length);

            var checksum = new byte[2];
            Array.Copy(decoded, decoded.Length - 2, checksum, 0, checksum.Length);

            var expectedChecksum = CalculateChecksum(payload);

            byte[] correctBytes = payload.Concat(expectedChecksum).ToArray();
            return Base32Encoding.ToString(correctBytes);
        }
    }
}