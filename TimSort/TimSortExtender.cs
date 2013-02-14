using System.Collections.Generic;
using System.Reflection;
using TimSort;

// ReSharper disable CheckNamespace

namespace System.Linq
{
	#region class TimSortExtender

	public static partial class TimSortExtender
    {
        #region class SorterReference

        internal class SorterReference
        {
            #region fields (public)

            public Action<object> SortAll;
		    public Action<object, int, int> SortRange;

            #endregion
        }

        #endregion

        #region class SorterKey

        internal class SorterKey
        {
            #region fields (public)

            public readonly Type SorterType;
            public readonly Type ContainerType;
            public readonly Type ItemType;

            #endregion

            #region constructor

            public SorterKey(Type sorterType, Type containerType, Type itemType)
	        {
                if (sorterType == null) throw new ArgumentNullException("sorterType");
                if (containerType == null) throw new ArgumentNullException("containerType");
                if (itemType == null) throw new ArgumentNullException("itemType");
	            SorterType = sorterType;
                ContainerType = containerType;
	            ItemType = itemType;
	        }

            #endregion

            #region overrides (for Dictionary)

            public override int GetHashCode()
            {
                return SorterType.GetHashCode() ^ ContainerType.GetHashCode() ^ ItemType.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(obj, null)) return false;
                if (ReferenceEquals(obj, this)) return true;
                var other = obj as SorterKey;
                if (ReferenceEquals(other, null)) return false;
                return 
                    other.ItemType == ItemType && 
                    other.ContainerType == ContainerType &&
                    other.SorterType == SorterType;
            }

            #endregion
        }

        #endregion

        private static Dictionary<SorterKey, SorterReference> _sorters;

        #region reflection helpers

	    private static readonly Type TypeOfArray = typeof (Array);

        private static bool IsIComparable<T>()
        {
            return (typeof(IComparable<T>)).IsAssignableFrom(typeof(T));
        }

	    private static Type MakeContainerType(Type container, Type item)
	    {
	        return
                container == TypeOfArray
                ? item.MakeArrayType() 
                : item.MakeGenericType(item);
	    }

        private static Type MakeActionType(Type parameterTypes)
        {
            return typeof (Action<>).MakeGenericType(parameterTypes);
        }

        private static Type MakeActionType(Type parameterA, Type parameterB, Type parameterC)
        {
            return typeof(Action<,,>).MakeGenericType(parameterA, parameterB, parameterC);
        }

        #endregion

        private static SorterReference GetComparableSorter<CT>(Type sorterType, Type containerType, Type itemType)
			where CT: class
	    {
			var key = new SorterKey(sorterType, containerType, itemType);
			SorterReference sorter;

		    if (_sorters == null)
		    {
                _sorters = new Dictionary<SorterKey, SorterReference>();
		    }

		    if (!_sorters.TryGetValue(key, out sorter))
			{
                const BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic;
                sorter = new SorterReference();
				var staticType = sorterType.MakeGenericType(itemType);
				var sortAll = (Action<CT>)Delegate.CreateDelegate(
                    typeof(Action<CT>), 
                    staticType.GetMethod("SortAll", flags));
				var sortRange = (Action<CT, int, int>)Delegate.CreateDelegate(
                    MakeActionType(MakeContainerType(containerType, itemType), typeof(int), typeof(int)), 
                    staticType.GetMethod("SortRange", flags));
				sorter.SortAll = (array) => sortAll(array as CT);
				sorter.SortRange = (array, lo, hi) => sortRange(array as CT, lo, hi);
				_sorters[key] = sorter;
			}

			return sorter;
	    }

	    #region Array (T[])

		/// <summary>Sorts the specified array.</summary>
		/// <typeparam name="T">Type of item.</typeparam>
		/// <param name="array">The array.</param>
		public static void TimSort<T>(this T[] array)
		{
		    if (TryNativeTimSort(array)) return; // done!

            if (IsIComparable<T>())
            {
                GetComparableSorter<T[]>(typeof (ComparableArrayTimSort<>), TypeOfArray, typeof (T))
                    .SortAll(array);
            }
		    else
		    {
                AnyArrayTimSort<T>.Sort(array, Comparer<T>.Default.Compare);
		    }
		}

        /// <summary>Sorts the specified array.</summary>
        /// <typeparam name="T">Type of item.</typeparam>
        /// <param name="array">The array.</param>
        public static void TimSort<T>(this T[] array, int lo, int hi)
        {
            if (TryNativeTimSort(array, lo, hi)) return; // done!

            if (IsIComparable<T>())
            {
                GetComparableSorter<T[]>(typeof (ComparableArrayTimSort<>), TypeOfArray, typeof (T))
                    .SortRange(array, lo, hi);
            }
            else
            {
                AnyArrayTimSort<T>.Sort(array, lo, hi, Comparer<T>.Default.Compare);
            }
        }

	    public static void TimSort<T>(this T[] array, Comparison<T> compare)
	    {
            if (compare == null) throw new ArgumentNullException("compare");
            AnyArrayTimSort<T>.Sort(array, compare);
	    }

        public static void TimSort<T>(this T[] array, Comparer<T> comparer)
        {
            if (comparer == null) throw new ArgumentNullException("comparer");
            AnyArrayTimSort<T>.Sort(array, comparer.Compare);
        }

        public static void TimSort<T>(this List<T> array, Comparison<T> compare)
        {
            if (compare == null) throw new ArgumentNullException("compare");
            AnyListTimSort<T>.Sort(array, compare);
        }

        public static void TimSort<T>(this List<T> array, Comparer<T> comparer)
        {
            if (comparer == null) throw new ArgumentNullException("comparer");
            AnyListTimSort<T>.Sort(array, comparer.Compare);
        }



	    ///// <summary>Sorts the specified array.</summary>
        ///// <typeparam name="T">Type of item.</typeparam>
        ///// <param name="array">The array.</param>
        ///// <param name="start">The start index.</param>
        ///// <param name="length">The length.</param>
        //public static void TimSort<T>(this T[] array, int start, int length)
        //{
        //    length = Math.Min(length, array.Length - start);
        //    ArrayTimSort<T>.Sort(array, start, start + length, Comparer<T>.Default.Compare);
        //}

        ///// <summary>Sorts the specified array.</summary>
        ///// <typeparam name="T">Type of item.</typeparam>
        ///// <param name="array">The array.</param>
        ///// <param name="comparer">The comparer.</param>
        //public static void TimSort<T>(this T[] array, Comparison<T> comparer)
        //{
        //    ArrayTimSort<T>.Sort(array, comparer);
        //}

        ///// <summary>Sorts the specified array.</summary>
        ///// <typeparam name="T">Type of item.</typeparam>
        ///// <param name="array">The array.</param>
        ///// <param name="start">The start index.</param>
        ///// <param name="length">The length.</param>
        ///// <param name="comparer">The comparer.</param>
        //public static void TimSort<T>(this T[] array, int start, int length, Comparison<T> comparer)
        //{
        //    length = Math.Min(length, array.Length - start);
        //    ArrayTimSort<T>.Sort(array, start, start + length, comparer);
        //}

        ///// <summary>Sorts the specified array.</summary>
        ///// <typeparam name="T">Type of item.</typeparam>
        ///// <param name="array">The array.</param>
        ///// <param name="comparer">The comparer.</param>
        //public static void TimSort<T>(this T[] array, Comparer<T> comparer)
        //{
        //    ArrayTimSort<T>.Sort(array, comparer.Compare);
        //}

        ///// <summary>Sorts the specified array.</summary>
        ///// <typeparam name="T">Type of item.</typeparam>
        ///// <param name="array">The array.</param>
        ///// <param name="start">The start.</param>
        ///// <param name="length">The length.</param>
        ///// <param name="comparer">The comparer.</param>
        //public static void TimSort<T>(this T[] array, int start, int length, Comparer<T> comparer)
        //{
        //    length = Math.Min(length, array.Length - start);
        //    ArrayTimSort<T>.Sort(array, start, start + length, comparer.Compare);
        //}

        //#endregion

        //#region List (IList<T>)

        ///// <summary>Sorts the specified array.</summary>
        ///// <typeparam name="T">Type of item.</typeparam>
        ///// <param name="array">The array.</param>
        //public static void TimSort<T>(this IList<T> array, bool buffered = true)
        //{
        //    if (buffered)
        //    {
        //        MergeBackSort(array, 0, array.Count, Comparer<T>.Default.Compare);
        //    }
        //    else
        //    {
        //        ListTimSort<T>.Sort(array, Comparer<T>.Default.Compare);
        //    }
        //}

        ///// <summary>Sorts the specified array.</summary>
        ///// <typeparam name="T">Type of item.</typeparam>
        ///// <param name="array">The array.</param>
        ///// <param name="start">The start index.</param>
        ///// <param name="length">The length.</param>
        //public static void TimSort<T>(this IList<T> array, int start, int length, bool buffered = true)
        //{
        //    length = Math.Min(length, array.Count - start);

        //    if (buffered)
        //    {
        //        MergeBackSort(array, start, length, Comparer<T>.Default.Compare);
        //    }
        //    else
        //    {
        //        ListTimSort<T>.Sort(array, start, start + length, Comparer<T>.Default.Compare);
        //    }
        //}

        ///// <summary>Sorts the specified array.</summary>
        ///// <typeparam name="T">Type of item.</typeparam>
        ///// <param name="array">The array.</param>
        ///// <param name="comparer">The comparer.</param>
        //public static void TimSort<T>(this IList<T> array, Comparison<T> comparer, bool buffered = true)
        //{
        //    if (buffered)
        //    {
        //        MergeBackSort(array, 0, array.Count, comparer);
        //    }
        //    else
        //    {
        //        ListTimSort<T>.Sort(array, comparer);
        //    }
        //}

        ///// <summary>Sorts the specified array.</summary>
        ///// <typeparam name="T">Type of item.</typeparam>
        ///// <param name="array">The array.</param>
        ///// <param name="start">The start index.</param>
        ///// <param name="length">The length.</param>
        ///// <param name="comparer">The comparer.</param>
        //public static void TimSort<T>(this IList<T> array, int start, int length, Comparison<T> comparer, bool buffered = true)
        //{
        //    length = Math.Min(length, array.Count - start);

        //    if (buffered)
        //    {
        //        MergeBackSort(array, start, length, comparer);
        //    }
        //    else
        //    {
        //        ListTimSort<T>.Sort(array, start, start + length, comparer);
        //    }
        //}

        ///// <summary>Sorts the specified array.</summary>
        ///// <typeparam name="T">Type of item.</typeparam>
        ///// <param name="array">The array.</param>
        ///// <param name="comparer">The comparer.</param>
        //public static void TimSort<T>(this IList<T> array, Comparer<T> comparer, bool buffered = true)
        //{
        //    if (buffered)
        //    {
        //        MergeBackSort(array, 0, array.Count, comparer.Compare);
        //    }
        //    else
        //    {
        //        ListTimSort<T>.Sort(array, comparer.Compare);
        //    }
        //}

        ///// <summary>Sorts the specified array.</summary>
        ///// <typeparam name="T">Type of item.</typeparam>
        ///// <param name="array">The array.</param>
        ///// <param name="start">The start.</param>
        ///// <param name="length">The length.</param>
        ///// <param name="comparer">The comparer.</param>
        //public static void TimSort<T>(this IList<T> array, int start, int length, Comparer<T> comparer, bool buffered = true)
        //{
        //    length = Math.Min(length, array.Count - start);

        //    if (buffered)
        //    {
        //        MergeBackSort(array, start, length, comparer.Compare);
        //    }
        //    else
        //    {
        //        ListTimSort<T>.Sort(array, start, start + length, comparer.Compare);
        //    }
        //}

        //#endregion

        //#region MergeBackSort

        //private static void MergeBackSort<T>(IList<T> list, int start, int length, Comparison<T> comparer)
        //{
        //    Debug.Assert(start >= 0 && start < list.Count);
        //    Debug.Assert(length >= 0 && length <= list.Count - start);

        //    T[] array = new T[length];

        //    int left, src, dst;

        //    if (start == 0 && length >= list.Count)
        //    {
        //        list.CopyTo(array, 0); // this might be faster than copying one by one
        //    }
        //    else
        //    {
        //        left = length; src = start; dst = 0;
        //        while (left-- > 0) array[dst++] = list[src++];
        //    }

        //    ArrayTimSort<T>.Sort(array, comparer);

        //    left = length; src = 0; dst = start;
        //    while (left-- > 0) list[dst++] = array[src++];
        //}

        #endregion
	}

	#endregion
}

// ReSharper restore CheckNamespace
