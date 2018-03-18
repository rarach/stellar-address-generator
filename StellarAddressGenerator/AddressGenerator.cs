using System;
using System.IO;
using StellarAddressGenerator.Stellar_DotNetCore_SDK;


namespace StellarAddressGenerator
{
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
}