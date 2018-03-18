using System.Security.Cryptography;


namespace StellarAddressGenerator
{
    internal class BytesProvider
    {
        //Re-randomize byte array every 150,000th request
        private const int STEPS_TO_CHANGE = 150000;
        private int _counter;
        private readonly byte[] _byteArray = new byte[32];


        internal byte[] GetBytes()
        {
            if (_counter++ % STEPS_TO_CHANGE == 0)
            {
                using (var rngCrypto = new RNGCryptoServiceProvider())
                {
                    rngCrypto.GetBytes(_byteArray);
                }

                return _byteArray;
            }

            //"Increment" byte array
            for (int i = 31; i >= 0; i--)
            {
                if (_byteArray[i] == 255)
                {
                    _byteArray[i] = 0;
                }
                else
                {
                    _byteArray[i]++;
                    break;
                }
            }

            return _byteArray;
        }
    }
}