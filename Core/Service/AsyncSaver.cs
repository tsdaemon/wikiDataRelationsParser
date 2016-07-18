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
        public int InQueue { get { return _dic.Count; } }

        private IMongoCollection<Triplet> _triplets;
        private ConcurrentDictionary<ObjectId, List<AnotherArticlePosition>> _dic;
        private Thread _thread;

        private static readonly Object obj = new object();

        public AsyncSaver(IMongoCollection<Triplet> triplets)
        {
            _triplets = triplets;
            _dic = new ConcurrentDictionary<ObjectId, List<AnotherArticlePosition>>();

            _thread = new Thread(Go);
            _thread.Start();
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

        private void Go()
        {
            while (true)
            {
                var next = GetNext();
                try
                {
                    var filter = Builders<Triplet>.Filter.Eq(t => t.Id, next.Item1);
                    var update = Builders<Triplet>.Update.PushEach(x => x.ArticlePositions, next.Item2);
                    _triplets.UpdateOneAsync(filter, update);
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
                    List<AnotherArticlePosition> list;
                    if (nextId != default(ObjectId) && _dic.TryRemove(nextId, out list))
                    {
                        return new Tuple<ObjectId, AnotherArticlePosition[]>(nextId, list.ToArray());
                    }
                }
                if (CheckEmpty(_dic)) Thread.Sleep(100);
            }
        }

        private bool CheckEmpty(ConcurrentDictionary<ObjectId, List<AnotherArticlePosition>> dic)
        {
            return _dic.Values.All(l => !l.Any());
        }
    }
}
