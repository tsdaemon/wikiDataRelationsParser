using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GetWikidataRelations.DataSource;
using GetWikidataRelations.Model;
using GetWikidataRelations.WikidataApi;

namespace GetWikidataRelations.Service
{
    public class Worker
    {
        private readonly string _id;
        private readonly int _workers;
        private readonly IDataSource _io;
        private List<UnitOfWork> _units;
        private WikidataQuery _api;

        public Worker(string id, int workers, IDataSource io)
        {
            _id = id;
            _io = io;

            _workers = workers;
            _api = new WikidataQuery();
        }

        public void Start()
        {
            _units = _io.GetWork(_id);
            // init units of work configuration
            if (!_units.Any())
            {
                var cc = _io.GetCategories(_id);
                var pp = _io.GetProperties(_id);
                foreach (var category in cc)
                {
                    foreach (var property in pp)
                    {
                        _units.Add(new UnitOfWork { Category = category, Property = property });
                    }
                }
                _io.SaveWork(_units);
            }
            // start processing
            ParallelLoopResult result = new ParallelLoopResult();
            var t = new Thread(() => result = Parallel.ForEach(_units.Where(u => !u.Done),
                new ParallelOptions {MaxDegreeOfParallelism = _workers},
                Do));
            t.Start();

            // block main thread until t is finished
            while (!result.IsCompleted)
            {
                Thread.Sleep(1000);
                var done = _units.Count(u => u.Done);
                Console.WriteLine("Pool: {0}, done {1}/{2}", _id, done, _units.Count);
                _io.SaveWork(_units);
            }
        }

        public void Do(UnitOfWork unit)
        {
            do
            {
                unit.Started = true;
                var query = GetQuery(unit);
                var result = _api.Get(query);
                if (result == null || !result.Results.Bindings.Any()) break;

                // save new offset
                unit.Offset += result.Results.Bindings.Length;

                // save result
                ProcessResult(unit.Property, unit.Category, result);
            } while (true);

            unit.Done = true;
        }

        const string WD_REL_QUERY = @"SELECT ?object ?objectLabel ?subject ?subjectLabel
WHERE
{{
	?object wdt:P31 wd:{0} . 
  	?object wdt:{1} ?subject .
	?object rdfs:label ?objectLabel filter (lang(?objectLabel) = 'uk') .
    ?subject rdfs:label ?subjectLabel filter (lang(?subjectLabel) = 'uk') .
}}
LIMIT 2000 OFFSET {2}";

        const string WD_REL_QUALIFIER_QUERY = @"SELECT ?object ?objectLabel ?subject ?subjectLabel ?qualifier ?qualifierLabel
WHERE
{{
	?object wdt:P31 wd:{0} . 
  	?object wdt:{1} ?subject .
    ?object p:{1} ?statement .
    ?statement pq:{2} ?qualifier .
	?object rdfs:label ?objectLabel filter (lang(?objectLabel) = 'uk') .
    ?subject rdfs:label ?subjectLabel filter (lang(?subjectLabel) = 'uk') .
    ?qualifier rdfs:label ?qualifierLabel filter (lang(?qualifierLabel) = 'uk') .
}}
LIMIT 2000 OFFSET {3}";

        private string GetQuery(UnitOfWork unit)
        {
            if (string.IsNullOrEmpty(unit.Property.Qualifier))
            {
                return string.Format(WD_REL_QUERY, unit.Category, unit.Property, unit.Offset);
            }
            else
            {
                return string.Format(WD_REL_QUALIFIER_QUERY, unit.Category.WikidataId, unit.Property.WikidataId, unit.Property.Qualifier, unit.Offset);
            }
        }

        private void ProcessResult(Property property, Category category, Wikidata result)
        {
            var validValues = result.Results.Bindings
                .Where(b => !string.IsNullOrEmpty(b["objectLabel"].With(w => w.Value))
                            && !string.IsNullOrEmpty(b["subjectLabel"].With(w => w.Value)));

            var documents = validValues.Select(r => new Triplet
            {
                Property = property,
                Category = category,
                SectionId = _id,
                WikiResult = r.ToDictionary(k => k.Key, k => k.Value.GetIdValue())
            });

            _io.SaveTriplets(documents);
        }
    }
}
