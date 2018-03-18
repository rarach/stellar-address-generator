using System;
using System.Configuration;
using System.Threading;


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
}
