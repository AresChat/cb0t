using System.Collections.Generic;

namespace cb0t
{
    class SafeQueue<T>
    {
        private Queue<T> queue = new Queue<T>();
        private object padlock = new object();

        public void Enqueue(T item)
        {
            lock (this.padlock)
                this.queue.Enqueue(item);
        }

        public bool TryDequeue(out T item)
        {
            lock (this.padlock)
            {
                item = default(T);
                bool success = false;

                if (this.queue.Count > 0)
                {
                    try
                    {
                        item = this.queue.Dequeue();
                        success = true;
                    }
                    catch { }
                }

                return success;
            }
        }

        public bool Pending
        {
            get
            {
                lock (this.padlock)
                    return this.queue.Count > 0;
            }
        }
    }
}

