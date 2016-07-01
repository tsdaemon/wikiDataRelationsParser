using System;
using System.Threading.Tasks;
using Core.DataSource;
using Core.Service;

namespace GetWikidataRelations
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting aggregation of wikidata relations.");
            var dataSource = new MongoDataSource("127.0.0.1", 27017, "wikidata");
            var pools = new[]
            {
                new Worker("person", 16, dataSource),
                new Worker("organization", 128, dataSource),
                new Worker("location", 512, dataSource)
            };
            Parallel.ForEach(pools,
                new ParallelOptions { MaxDegreeOfParallelism = 3 },
                p => p.Start());
        }
    }
}
