using System.Collections.Generic;
using System.Reflection;
using TimSort;

// ReSharper disable CheckNamespace
namespace System.Linq
{
	#region class TimSortExtender

    /// <summary>
    /// <![CDATA[T[], List<T> and IList<T>]]> extender providing TimSort extension methods.
    /// </summary>
	public static partial class TimSortExtender
    {
        #region dynamic invocation for IComparable

        private static bool IsIComparable<T>()
        {
            return (typeof(IComparable<T>)).IsAssignableFrom(typeof(T));
        }

        internal class SorterReference
        {
            #region fields (public)

            public Action<object> SortAll;
            public Action<object, int, int> SortRange;

            #endregion
        }

        private static readonly Dictionary<Type, SorterReference> SorterMap 
            = new Dictionary<Type, SorterReference>();

        private static SorterReference GetComparableSorter<TContainer, TItem>(Type sorterType)
            where TContainer : class
        {
            var key = sorterType;
            SorterReference sorter;

            if (!SorterMap.TryGetValue(key, out sorter))
            {
                const BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic;
                sorter = new SorterReference();
                var staticType = sorterType.MakeGenericType(typeof(TItem));
                var sortAll = (Action<TContainer>)Delegate.CreateDelegate(
                    typeof(Action<TContainer>),
                    staticType.GetMethod("SortAll", flags));
                var sortRange = (Action<TContainer, int, int>)Delegate.CreateDelegate(
                    typeof(Action<TContainer, int, int>),
                    staticType.GetMethod("SortRange", flags));
                sorter.SortAll = (array) => sortAll(array as TContainer);
                sorter.SortRange = (array, lo, hi) => sortRange(array as TContainer, lo, hi);
                SorterMap[key] = sorter;
            }

            return sorter;
        }

        private static bool TryComparableTimeSort<T>(T[] array)
        {
            if (!IsIComparable<T>()) return false;
            GetComparableSorter<T[], T>(typeof(ComparableArrayTimSort<>)).SortAll(array);
            return true;
        }

        private static bool TryComparableTimeSort<T>(T[] array, int lo, int hi)
        {
            if (!IsIComparable<T>()) return false;
            GetComparableSorter<T[], T>(typeof(ComparableArrayTimSort<>)).SortRange(array, lo, hi);
            return true;
        }

        private static bool TryComparableTimeSort<T>(List<T> list)
        {
            if (!IsIComparable<T>()) return false;
            GetComparableSorter<T[], T>(typeof(ComparableListTimSort<>)).SortAll(list);
            return true;
        }

        private static bool TryComparableTimeSort<T>(List<T> list, int lo, int hi)
        {
            if (!IsIComparable<T>()) return false;
            GetComparableSorter<T[], T>(typeof(ComparableListTimSort<>)).SortRange(list, lo, hi);
            return true;
        }

        private static bool TryComparableTimeSort<T>(IList<T> list)
        {
            if (!IsIComparable<T>()) return false;
            GetComparableSorter<T[], T>(typeof(ComparableIListTimSort<>)).SortAll(list);
            return true;
        }

        private static bool TryComparableTimeSort<T>(IList<T> list, int lo, int hi)
        {
            if (!IsIComparable<T>()) return false;
            GetComparableSorter<T[], T>(typeof(ComparableIListTimSort<>)).SortRange(list, lo, hi);
            return true;
        }

        #endregion

	    private static readonly Dictionary<Type, FieldInfo> ItemMemberMap = 
            new Dictionary<Type, FieldInfo>();

        private static T[] GetInternalMember<T>(List<T> list)
        {
            FieldInfo member;
            var listType = typeof (List<T>);
            if (!ItemMemberMap.TryGetValue(listType, out member))
            {
                member = typeof (List<T>).GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance);
                ItemMemberMap.Add(listType, member);
            }
            if (member == null) return null;
            return (T[])member.GetValue(list);
        }

        // ReSharper disable ParameterTypeCanBeEnumerable.Local

        private static List<T> GetInternalMember<T>(IList<T> list)
        {
            return list as List<T>;
        }

        // ReSharper restore ParameterTypeCanBeEnumerable.Local
    }

	#endregion
}
// ReSharper restore CheckNamespace
