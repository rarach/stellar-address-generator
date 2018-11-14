using System;
using System.Configuration;
using System.Threading;


namespace StellarAddressGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "--blackhole")
            {
                runBlackholePublicKeyGenerator();
            }
            else
            {
                startKeyPairGenerator();
            }
        }

        private static void startKeyPairGenerator()
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

        private static void runBlackholePublicKeyGenerator()
        {
            string prefixesStr = ConfigurationManager.AppSettings["prefixes"];
            string[] prefixes = prefixesStr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string filler = ConfigurationManager.AppSettings["blackholeFiller"];

            foreach (string prefix in prefixes)
            {
                string address = BlackholeAddressGenerator.GetAddress(prefix, filler[0]);
                Console.WriteLine("Address=" + address);
            }

            Console.WriteLine("Press ENTER to exit...\n");
            Console.ReadLine();
        }
    }
}
