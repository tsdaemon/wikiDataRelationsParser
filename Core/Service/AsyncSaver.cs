using System;
using System.Collections.Concurrent;
using System.Threading;
using Core.Model;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Core.Service
{
    public class AsyncSaver
    {
        private IMongoCollection<Triplet> _triplets;
        private ConcurrentQueue<Tuple<ObjectId, AnotherArticlePosition>> _queue;
        private Thread _thread;

        public AsyncSaver(IMongoCollection<Triplet> triplets)
        {
            _triplets = triplets;
            _queue = new ConcurrentQueue<Tuple<ObjectId, AnotherArticlePosition>>();

            _thread = new Thread(Go);
            _thread.Start();
        }

        public void Save(ObjectId id, AnotherArticlePosition position)
        {
            _queue.Enqueue(Tuple.Create(id, position));
        }

        private void Go()
        {
            while (true)
            {
                var next = GetNext();
                try
                {
                    var filter = Builders<Triplet>.Filter.Eq(t => t.Id, next.Item1);
                    var update = Builders<Triplet>.Update.Push(x => x.ArticlePositions, next.Item2);
                    _triplets.UpdateOneAsync(filter, update);
                }
                catch (Exception ex)
                {
                    ExceptionHelper.OutputException(ex);
                }
            }
        }

        private Tuple<ObjectId, AnotherArticlePosition> GetNext()
        {
            while (_queue.IsEmpty)
            {
                Thread.Sleep(100);
            }
            
            while(true)
            {
                Tuple<ObjectId, AnotherArticlePosition> retValue;
                if (_queue.TryDequeue(out retValue))
                {
                    return retValue;
                }
                if (_queue.IsEmpty) Thread.Sleep(100);
            }
        }
    }
}
