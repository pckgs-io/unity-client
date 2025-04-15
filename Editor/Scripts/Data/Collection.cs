using System;
using System.Collections.Generic;
using System.Linq;

namespace Pckgs
{
    public class Collection<T>
    {
        private readonly List<T> list = new();

        public IEnumerable<T> Data => list;

        public event Action<T> OnDataAdded;
        public event Action<T> OnDataRemoved;


        public Collection(IEnumerable<T> data)
        {
            list = data?.ToList() ?? new List<T>();
        }
        public void Add(T item)
        {
            list.Add(item);
            OnDataAdded?.Invoke(item);
        }
        public void Remove(T item)
        {
            if (list.Remove(item))
                OnDataRemoved?.Invoke(item);
        }
    }
}
