using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.DataSource;
using Core.Model;
using Core.WikidataApi;

namespace Core.Service
{
    public class Worker
    {
        private readonly string _id;
        private readonly int _workers;
        private readonly IDataSource _io;
        private Unit _unit;
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
            _unit = _io.GetWork(_id);
            if (_unit == null) _unit = new Unit {Id = _id};
            // init units of work configuration
            if (_unit == null || _unit.Parts == null || !_unit.Parts.Any())
            {
                var cc = _io.GetCategories(_id);
                var pp = _io.GetProperties(_id);
                _unit.Parts = new List<UnitPart>();
                foreach (var category in cc)
                {
                    foreach (var property in pp)
                    {
                        _unit.Parts.Add(new UnitPart { Category = category.WikidataId, Property = property.WikidataId });
                    }
                }
                _io.SaveWork(_unit);
            }
            // start processing
            ParallelLoopResult result = new ParallelLoopResult();
            var t = new Thread(() => result = Parallel.ForEach(_unit.Parts.Where(u => !u.Done),
                new ParallelOptions {MaxDegreeOfParallelism = _workers},
                Do));
            t.Start();

            // block main thread until t is finished
            while (!result.IsCompleted)
            {
                Thread.Sleep(5000);
                var done = _unit.Parts.Count(u => u.Done);
                Console.WriteLine("Pool: {0}, done {1}/{2}", _id, done, _unit.Parts.Count);
                _io.SaveWork(_unit);
            }
        }

        public void Do(UnitPart unit)
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

        const string WD_REL_QUERY = @"SELECT ?object ?objectLabel ?objectWiki ?subject ?subjectLabel ?subjectWiki
WHERE
{{
	?object wdt:P31 wd:{0} . ?objectWiki schema:about ?object ;	schema:isPartOf	<https://uk.wikipedia.org/> .
  	?object wdt:{1} ?subject . ?subjectWiki schema:about ?subject ;	schema:isPartOf	<https://uk.wikipedia.org/> .
	?object rdfs:label ?objectLabel filter (lang(?objectLabel) = 'uk') .
    ?subject rdfs:label ?subjectLabel filter (lang(?subjectLabel) = 'uk') .
}}
LIMIT 1000 OFFSET {2}";

        const string WD_REL_QUALIFIER_QUERY = @"SELECT ?object ?objectLabel ?objectWiki ?subject ?subjectLabel ?subjectWiki ?qualifier ?qualifierLabel
WHERE
{{
	?object wdt:P31 wd:{0} . ?objectWiki schema:about ?object ;	schema:isPartOf	<https://uk.wikipedia.org/> .
  	?object wdt:{1} ?subject . ?subjectWiki schema:about ?subject ;	schema:isPartOf	<https://uk.wikipedia.org/> .
	?object rdfs:label ?objectLabel filter (lang(?objectLabel) = 'uk') .
    ?subject rdfs:label ?subjectLabel filter (lang(?subjectLabel) = 'uk') .

    OPTIONAL 
    {{
        ?object p:{1} ?statement .
        ?statement pq:{2} ?qualifier .
        ?qualifier rdfs:label ?qualifierLabel filter (lang(?qualifierLabel) = 'uk') .
    }}
}}
LIMIT 1000 OFFSET {3}";

        private string GetQuery(UnitPart unit)
        {
            if (string.IsNullOrEmpty(unit.Qualifier))
            {
                return string.Format(WD_REL_QUERY, unit.Category, unit.Property, unit.Offset);
            }
            else
            {
                return string.Format(WD_REL_QUALIFIER_QUERY, unit.Category, unit.Property, unit.Qualifier, unit.Offset);
            }
        }

        private void ProcessResult(string property, string category, WikidataApi.Wikidata result)
        {
            var documents = result.Results.Bindings.Select(r =>
            {
                var wikiresult = r.ToDictionary(k => k.Key, k => k.Value.Value);
                var objectWikiName = Uri.UnescapeDataString(wikiresult["objectWiki"].Split('/').Last());
                var subjectWikiName = Uri.UnescapeDataString(wikiresult["subjectWiki"].Split('/').Last());
                return new Triplet
                {
                    Property = property,
                    Category = category,
                    SectionId = _id,
                    WikiResult = wikiresult,
                    ObjectWikiName = objectWikiName,
                    SubjectWikiName = subjectWikiName
                };
            });

            _io.SaveTriplets(documents);
        }
    }
}
