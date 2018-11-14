using StellarAddressGenerator.Stellar_DotNetCore_SDK;


namespace StellarAddressGenerator
{
    internal class BlackholeAddressGenerator
    {
        private const int PUBLIC_KEY_LENGTH = 56;

        /// <summary>
        /// Return blackhole address for given prefix and filler
        /// </summary>
        /// <param name="prefix">Stellar address prefix starting with G</param>
        /// <param name="filler">Letter or digits 2 - 7 to be used as filler between the prefix and trailing checksum</param>
        /// <returns>Correct public key (address) starting with given prefix</returns>
        internal static string GetAddress(string prefix, char filler)
        {
            string publicKey = prefix + new string(filler, PUBLIC_KEY_LENGTH - prefix.Length);
            publicKey = StrKey.CorrectEd25519PublicKey(publicKey);
            return publicKey;
        }
    }
}
