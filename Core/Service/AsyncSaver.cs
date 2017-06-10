using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.Model;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Core.Service
{
    public class AsyncSaver
    {
        public int InQueue => _dic.Count;

        private IMongoCollection<Triplet> _triplets;
        private ConcurrentDictionary<ObjectId, List<AnotherArticlePosition>> _dic;

        private static readonly object obj = new object();

        public AsyncSaver(IMongoCollection<Triplet> triplets, int threads = 10)
        {
            _triplets = triplets;
            _dic = new ConcurrentDictionary<ObjectId, List<AnotherArticlePosition>>();

            for (var i = 0; i < threads; i++)
            {
                var thread = new Thread(Go);
                thread.Start();
            }
        }

        public void Save(ObjectId id, AnotherArticlePosition position)
        {
            lock (obj)
            {
                _dic.AddOrUpdate(id, new List<AnotherArticlePosition> {position}, (i, ls) =>
                {
                    ls.Add(position);
                    return ls;
                });
            }
        }

        public void Join()
        {
            while (!CheckEmpty())
            {
                Thread.Sleep(1000);
            }
        }

        private void Go()
        {
            while (true)
            {
                var next = GetNext();
                try
                {
                    var filter = Builders<Triplet>.Filter.Eq(t => t.Id, next.Item1);
                    var update = Builders<Triplet>.Update.PushEach(x => x.ArticlePositions, next.Item2);
                    var task = _triplets.UpdateOneAsync(filter, update);
                    task.Wait();
                }
                catch (Exception ex)
                {
                    ExceptionHelper.OutputException(ex);
                }
            }
        }

        private Tuple<ObjectId, AnotherArticlePosition[]> GetNext()
        {
            while (_dic.IsEmpty)
            {
                Thread.Sleep(100);
            }
            
            while(true)
            {
                lock (obj)
                {
                    var nextId = _dic.Keys.FirstOrDefault();
                    if (nextId != default(ObjectId) && _dic.TryRemove(nextId, out List<AnotherArticlePosition> list))
                    {
                        return new Tuple<ObjectId, AnotherArticlePosition[]>(nextId, list.ToArray());
                    }
                }
                if (CheckEmpty()) Thread.Sleep(100);
            }
        }

        private bool CheckEmpty()
        {
            return _dic.Values.All(l => !l.Any());
        }
    }
}
