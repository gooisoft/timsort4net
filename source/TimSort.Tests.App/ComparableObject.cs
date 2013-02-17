using System;

namespace TimSort.Tests.App
{
    public class ComparableObject: IComparable<ComparableObject>
    {
        private readonly int _x;
        private readonly int _y;
        private readonly int _z;

        public ComparableObject(Random random)
        {
            _x = random.Next();
            _y = random.Next();
            _z = random.Next();
        }

        public int CompareTo(ComparableObject other)
        {
            if (other == null) return -1;
            if (this == other) return 0;
            int c;
            if ((c = _x.CompareTo(other._x)) != 0) return c;
            if ((c = _y.CompareTo(other._y)) != 0) return c;
            return _z.CompareTo(other._z);
        }
    }
}
