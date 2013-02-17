using System;

namespace TimSort.Tests.App
{
    public class NonComparableObject
    {
        private readonly int _x;
        private readonly int _y;
        private readonly int _z;

        public NonComparableObject(Random random)
        {
            _x = random.Next();
            _y = random.Next();
            _z = random.Next();
        }

        public static int Compare(NonComparableObject a, NonComparableObject b)
        {
            if (a == b) return 0;
            if (a == null) return -1;
            if (b == null) return 1;
            int c;
            if ((c = a._x.CompareTo(b._x)) != 0) return c;
            if ((c = a._y.CompareTo(b._y)) != 0) return c;
            return a._z.CompareTo(b._z);
        }
    }
}
