using System.Collections.Concurrent;

namespace ToyTrainProject.Models
{
    public class MaxSizeQueue<T>
    {
        private readonly object privateLockObject = new object();

        private readonly ConcurrentQueue<T> queue = new ConcurrentQueue<T>();

        public int Size { get; }

        public MaxSizeQueue(int size)
        {
            Size = size;
        }

        public void Enqueue(T obj)
        {
            queue.Enqueue(obj);

            lock (privateLockObject)
            {
                while (queue.Count > Size)
                {
                    T outObj;
                    queue.TryDequeue(out outObj);
                }
            }
        }
    }
}