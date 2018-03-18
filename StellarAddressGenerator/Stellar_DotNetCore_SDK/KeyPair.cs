using System;
using Chaos.NaCl;


namespace StellarAddressGenerator.Stellar_DotNetCore_SDK
{
    public class KeyPair
    {
        /// <summary>
        /// Creates a new Keypair instance from secret. This can either be secret key or secret seed depending on underlying public-key signature system. Currently Keypair only supports ed25519.
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="privateKey"></param>
        /// <param name="seed"></param>
        public KeyPair(byte[] publicKey, byte[] privateKey, byte[] seed)
        {
            if (null == publicKey)
                throw new ArgumentNullException(nameof(publicKey), "publicKey cannot be null");
            PublicKey = publicKey;
            PrivateKey = privateKey;
            SeedBytes = seed;
        }

        /// <summary>
        /// The public key.
        /// </summary>
        public byte[] PublicKey { get; }

        /// <summary>
        /// The private key.
        /// </summary>
        public byte[] PrivateKey { get; }


        /// <summary>
        /// The bytes of the Secret Seed
        /// </summary>
        public byte[] SeedBytes { get; }


        /// <summary>
        /// Address
        /// </summary>
        public string Address => StrKey.EncodeCheck(StrKey.VersionByte.ACCOUNT_ID, PublicKey);

        /// <summary>
        /// SecretSeed
        /// </summary>
        public string SecretSeed => StrKey.EncodeStellarSecretSeed(SeedBytes);


        /// <summary>
        ///     Creates a new Stellar keypair from a raw 32 byte secret seed.
        /// </summary>
        /// <param name="seed">seed The 32 byte secret seed.</param>
        /// <returns>
        ///     <see cref="KeyPair" />
        /// </returns>
        public static KeyPair FromSecretSeed(byte[] seed)
        {
            byte[] publicKey;
            byte[] privateKey;
            Ed25519.KeyPairFromSeed(out /*var*/ publicKey, out /*var*/ privateKey, seed);

            return new KeyPair(publicKey, privateKey, seed);
        }
    }
}
