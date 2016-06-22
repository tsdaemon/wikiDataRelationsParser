using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using GetWikidataRelations.WikidataApi;

namespace GetWikidataRelations
{
    class Program
    {
        static List<string> files = new List<string>
        {
           "person", "organization", "location"
        };

        const string WD_REL_QUERY = @"SELECT ?object ?objectLabel ?subject ?subjectLabel
WHERE
{{
	?object wdt:P31 wd:{0} . 
  	?object wdt:{1} ?subject .
	?object rdfs:label ?objectLabel filter (lang(?objectLabel) = 'uk') .
    ?subject rdfs:label ?subjectLabel filter (lang(?subjectLabel) = 'uk') .
}}
LIMIT 2000 OFFSET {2}";

        private static WikidataQuery _api;

        static void Main(string[] args)
        {
            _api = new WikidataQuery();
            Console.WriteLine("Starting aggregation of wikidata relations.");
            files.AsParallel().ForAll(k => ReadAllCategories(k));
        }

        private static void ReadAllCategories(string type)
        {
            var categoriesFile = "../../../categories_" + type + ".txt";
            var propertiesFile = "../../../properties_" + type + ".txt";
            var offsetFile = "../../../offset_" + type + ".txt";
            var resultFile = "../../../result_" + type + ".txt";

            // check previous offset
            var categoryIndex = 0;
            int propertyIndex = 0;
            long offset = 0;
            if (File.Exists(offsetFile))
            {
                using (var stream = new StreamReader(File.OpenRead(offsetFile)))
                {
                    var values = stream.ReadToEnd().Split(' ');
                    categoryIndex = int.Parse(values[0]);
                    propertyIndex = int.Parse(values[1]);
                    offset = long.Parse(values[2]);
                }
            }
            string[] categories;
            using (var stream = new StreamReader(File.OpenRead(categoriesFile)))
            {
                categories = stream.ReadToEnd()
                    .Split('\n')
                    .Where(s => !(string.IsNullOrEmpty(s) || s[0] == '#'))
                    .Select(s => s.Trim().Split('\t')[0])
                    .ToArray();
            }
            foreach (var category in categories.Skip(categoryIndex))
            {
                Console.WriteLine("Category {0} ({1}/{2}).", category, (categoryIndex+1), categories.Length);
                ReadAllTriplets(category, categoryIndex, propertyIndex, offset, offsetFile, propertiesFile, resultFile);
                categoryIndex++;
                // save new offset
                offset = 0;
                propertyIndex = 0;
                using (var wr = new StreamWriter(File.OpenWrite(offsetFile)))
                {
                    wr.Write("{0} {1} {2}", categoryIndex, propertyIndex, offset);
                }
            }

        }

        private static void ReadAllTriplets(string category, 
            int categoryIndex, 
            int propertyIndex, 
            long offset, 
            string offsetFile, 
            string propertiesFile, 
            string resultFile)
        {
            // get properties array
            string[] properties;
            using (var stream = new StreamReader(File.OpenRead(propertiesFile)))
            {
                properties = stream.ReadToEnd()
                    .Split('\n')
                    .Where(s => s[0] != '#')
                    .Select(s => s.Trim().Split('\t')[0])
                    .ToArray();
            }
            foreach (var property in properties.Skip(propertyIndex))
            {
                Console.WriteLine("Category {0}, property {1} ({2}/{3}).", category, property, (propertyIndex+1), properties.Length);
                ProcessProperty(category, categoryIndex, property, propertyIndex, offset, offsetFile, resultFile);
                propertyIndex++;
                // save new offset
                offset = 0;
                
                WriteOffset(categoryIndex, propertyIndex, offset, offsetFile);
            }
        }

        private static void WriteOffset(int categoryIndex, int propertyIndex, long offset, string offsetFile)
        {
            try
            {
                using (var wr = new StreamWriter(File.OpenWrite(offsetFile)))
                {
                    wr.Write("{0} {1} {2}", categoryIndex, propertyIndex, offset);
                }
            }
            catch (IOException)
            {
                Thread.Sleep(100);
                WriteOffset(categoryIndex, propertyIndex, offset, offsetFile);
            }
        }

        private static void ProcessProperty(string category, int categoryIndex, string property, int propertyIndex, long offset, string offsetFile, string resultFile)
        {
            do
            {
                var result = _api.Get(WD_REL_QUERY, category, property, offset);
                if (result == null || !result.Results.Bindings.Any()) break;

                // save new offset
                offset += result.Results.Bindings.Length;
                using (var wr = new StreamWriter(File.OpenWrite(offsetFile)))
                {
                    wr.Write("{0} {1} {2}", categoryIndex, propertyIndex, offset);
                }
                

                // save result
                ProcessResult(property, resultFile, result);
                Console.WriteLine("Category {0}, property {1}, processed {2}.", category, property, offset);
            } while (true);
        }

        private static void ProcessResult(string property, string resultFile, Wikidata result)
        {
            try
            {
                using (var wr = new StreamWriter(File.Open(resultFile, FileMode.Append)))
                {
                    var validValues = result.Results.Bindings
                        .Where(b => !string.IsNullOrEmpty(b["objectLabel"].With(w => w.Value))
                                    && !string.IsNullOrEmpty(b["subjectLabel"].With(w => w.Value)));

                    foreach (var triplet in validValues)
                    {
                        wr.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}",
                            triplet["object"].With(w => w.Value),
                            triplet["objectLabel"].With(w => w.Value),
                            property,
                            triplet["subject"].With(w => w.Value),
                            triplet["subjectLabel"].With(w => w.Value));
                    }
                }
            }
            catch (IOException)
            {
                Thread.Sleep(100);
                ProcessResult(property, resultFile, result);
            }
        }
    }
}
