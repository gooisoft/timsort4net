using System;
using System.Collections;
using System.Collections.Generic;

namespace TimSort.Tests.App
{
    public class SlowList<T>: IList<T>
    {
        private readonly T[] _array;

        public SlowList(T[] array)
        {
            _array = array;
        }

        public int Count { get { return _array.Length; } }

        public T this[int index]
        {
            get { return _array[index]; }
            set { _array[index] = value; }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(_array, 0, array, arrayIndex, _array.Length);
        }

        public bool IsReadOnly { get { return false; } }

        public IEnumerator<T> GetEnumerator() { return ((IList<T>)_array).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return _array.GetEnumerator(); }

        public int IndexOf(T item) { throw new NotSupportedException(); }
        public void Insert(int index, T item) { throw new NotSupportedException(); }
        public void RemoveAt(int index) { throw new NotSupportedException(); }
        public void Add(T item) { throw new NotSupportedException(); }
        public void Clear() { throw new NotSupportedException(); }
        public bool Contains(T item) { throw new NotSupportedException(); }
        public bool Remove(T item) { throw new NotSupportedException(); }
    }
}
