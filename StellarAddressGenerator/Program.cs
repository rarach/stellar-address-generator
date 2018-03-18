using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using StellarAddressGenerator.Stellar_DotNetCore_SDK;


namespace StellarAddressGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            string prefixesStr = ConfigurationManager.AppSettings["prefixes"];
            string[] prefixes = prefixesStr.Split(new [] {','}, StringSplitOptions.RemoveEmptyEntries);

            int threadCount = int.Parse(ConfigurationManager.AppSettings["threads"]);
            var generators = new AddressGenerator[threadCount];

            string outDir = ConfigurationManager.AppSettings["outputDir"];

            for (int i = 1; i <= threadCount; i++)
            {
                var generator = new AddressGenerator(i, $"{outDir}\\out_{i}.txt", prefixes);
                generators[i-1] = generator;
                Thread t = new Thread(generator.Start);
                t.Start();
            }

            Console.WriteLine($"Stellar address generator started on {threadCount} threads.");
            Console.WriteLine("Press ENTER to exit...\n");
            Console.ReadLine();

            foreach (AddressGenerator ag in generators)
            {
                ag.Stop();
            }
        }
    }

    internal class AddressGenerator
    {
        private const int INFO_INTERVAL = 10000;
        private int _counter;
        private readonly StreamWriter _writer;
        private readonly int _id;
        private readonly string[] _prefixes;
        private readonly BytesProvider _bytesProvider = new BytesProvider();
        private bool _gotKillSignal;


        internal AddressGenerator(int id, string outFilePath, string[] prefixes)
        {
            _id = id;
            var stream = new FileStream(outFilePath, FileMode.Append, FileAccess.Write);
            _writer = new StreamWriter(stream);
            _writer.WriteLine(" ");
            _prefixes = prefixes;
        }

        internal void Start()
        {
            while (!_gotKillSignal)
            {
                KeyPair pair = KeyPair.FromSecretSeed(_bytesProvider.GetBytes());
                string address = pair.Address;

                foreach (string prefix in _prefixes)
                {
                    if (address.StartsWith(prefix))
                    {
                        Console.Beep(10000, 300);
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Address=" + address);
                        Console.WriteLine("SecretKey=" + pair.SecretSeed);
                        Console.WriteLine(new string('=', 80));
                        Console.ResetColor();

                        _writer.WriteLine($"{address}  {pair.SecretSeed}  (prefix {prefix})");
                        _writer.Flush();
                    }
                }

                if (_counter++ == INFO_INTERVAL)
                {
                    _counter = 0;
                    Console.WriteLine($"Another {INFO_INTERVAL} seeds tested by generator #{_id}. Last:");
                    Console.WriteLine("Address=" + address);
                    Console.WriteLine("SecretKey=" + pair.SecretSeed);
                    Console.WriteLine(new string('=', 80));
                }
            }
        }

        internal void Stop()
        {
            Console.WriteLine($"Generator #{_id} received a kill signal and is halting.");
            _gotKillSignal = true;
            _writer.Flush();
            _writer.Close();
        }
    }

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
