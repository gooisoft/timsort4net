using System;
using System.Collections.Generic;
using System.Linq;

namespace TimSort
{
	#region class TimSortExtender

	public static class TimSortExtender2
	{
		internal class Sorter
		{
			Action<object> SortAll;
			Action<object, int, int> SortRange;
		}

		private static Dictionary<Type, Sorter> _sorters;

		private static Sorter GetComparableSortAll<T>(Type comparable)
	    {
			var type = typeof(T);
			Sorter sorter;
			if (!_sorters.TryGetValue(type, out sorter))
			{
				var staticType = typeof(ComparableArrayTimSort<>).MakeGenericType(typeof(T));
				var sortMethod = Delegate.CreateDelegate(staticType, staticType.GetMethod("SortAll"));
				sorter = (array) => sortMethod.DynamicInvoke(new object[] { array });
				_sortAllComparable[type] = sorter;
			}
			return sorter;
	    }

	    #region Array (T[])

		/// <summary>Sorts the specified array.</summary>
		/// <typeparam name="T">Type of item.</typeparam>
		/// <param name="array">The array.</param>
		public static void TimSort<T>(this T[] array)
		{
			if (typeof(T) == typeof(Byte)) ByteArrayTimSort.Sort(array as byte[]);
			else if (typeof(T) == typeof(int)) Int32ArrayTimSort.Sort(array as int[]);
			else if (typeof(T).IsSubclassOf(typeof(IComparable<T>))) GetComparableSortAll<T>()(array);
			else AnyArrayTimSort<T>.Sort(array, Comparer<T>.Default.Compare);
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
