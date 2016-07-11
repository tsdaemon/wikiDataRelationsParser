using System;
using System.Collections.Concurrent;
using System.Threading;
using Core.Model;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Core.Service
{
    public class AsyncSaver : IDisposable
    {
        private IMongoCollection<Triplet> _triplets;
        private ConcurrentQueue<Triplet> _queue;
        private Thread _thread;
        private ConcurrentDictionary<ObjectId, int> _dictionary;

        public AsyncSaver(IMongoCollection<Triplet> triplets)
        {
            _triplets = triplets;
            _queue = new ConcurrentQueue<Triplet>();
            _dictionary = new ConcurrentDictionary<ObjectId, int>();

            _thread = new Thread(Go);
            _thread.Start();
        }

        public void Save(Triplet t)
        {
            _queue.Enqueue(t);
            _dictionary.AddOrUpdate(t.Id, 1, (id,inc) => inc++);
        }


        public void Dispose()
        {
            while (_queue.Count > 0)
            {
                Thread.Sleep(100);
            }
            _thread.Abort();
        }

        private void Go()
        {
            while (true)
            {
                var next = GetNext();
                try
                {
                    _triplets.ReplaceOne(t => t.Id == next.Id, next);
                }
                catch (Exception ex)
                {
                    ExceptionHelper.OutputException(ex);
                }
            }
        }

        private Triplet GetNext()
        {
            while (_queue.IsEmpty)
            {
                Thread.Sleep(100);
            }
            
            while(true)
            {
                Triplet retValue;
                if (_queue.TryDequeue(out retValue))
                {
                    int inQueue;
                    if (!_dictionary.TryGetValue(retValue.Id, out inQueue))
                    {
                        return retValue;
                    }
                    else
                    {
                        inQueue--;
                        var update = _dictionary.TryUpdate(retValue.Id, inQueue, inQueue + 1);

                        if (update && inQueue == 0)
                        {
                            return retValue;
                        }
                    }
                }
                if (_queue.IsEmpty) Thread.Sleep(100);
            }
        }
    }
}
