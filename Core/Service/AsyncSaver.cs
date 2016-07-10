using System;
using System.Collections.Concurrent;
using System.Threading;
using Core.Model;
using MongoDB.Driver;

namespace Core.Service
{
    public class AsyncSaver : IDisposable
    {
        private IMongoCollection<Triplet> _triplets;
        private ConcurrentQueue<Triplet> _queue;
        private Thread _thread;

        public AsyncSaver(IMongoCollection<Triplet> triplets)
        {
            _triplets = triplets;
            _queue = new ConcurrentQueue<Triplet>();

            _thread = new Thread(Go);
            _thread.Start();
        }

        public void Save(Triplet t)
        {
            _queue.Enqueue(t);
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
                while (_queue.IsEmpty)
                {

                }
                Triplet retValue;
                if(_queue.TryDequeue(out retValue))
                {
                    try
                    {
                        _triplets.ReplaceOne(t => t.Id == retValue.Id, retValue);
                    }
                    catch
                    {
                        
                    }
                }
            }
        }
    }
}
