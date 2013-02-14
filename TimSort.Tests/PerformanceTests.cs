using System;
using NUnit.Framework;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using Performance;
using TimSort;

namespace TimSort.Tests
{
	[TestFixture]
	public class PerformanceTests
	{
		const int maxSize = (int.MaxValue / 100) / sizeof(int);
		const int seed = 1234;

		public static int Compare(int a, int b)
		{
			// Thread.Sleep(0); // make compare function slow
			return a.CompareTo(b);
		}

		[Test]
		public void RandomTests()
		{
			Console.WriteLine("<<< Random data >>>");

			var r = new Random(seed);
			int[] a = new int[maxSize];
			int[] b = new int[maxSize];

			Console.WriteLine("Preparing...");
			for (int i = 0; i < maxSize; i++) a[i] = b[i] = r.Next();

			Console.WriteLine("Sorting...");
			PerformanceTimer.Debug("builtin", 1, () => Array.Sort(b, Compare), maxSize);
			PerformanceTimer.Debug("timsort", 1, () => a.TimSort(Compare), maxSize);

			Console.WriteLine("Testing...");
			for (int i = 0; i < maxSize; i++) Assert.AreEqual(a[i], b[i]);
		}

		[Test]
		public void SortedTests()
		{
			Console.WriteLine("<<< Generally ascending data >>>");

			var r = new Random(seed);
			int[] a = new int[maxSize];
			int[] b = new int[maxSize];
			int value = int.MinValue;

			Console.WriteLine("Preparing...");
			for (int i = 0; i < maxSize; i++)
			{
				value = 
					r.Next(100) < 80 
					? value + r.Next(100) 
					: value - r.Next(100);
				a[i] = b[i] = value;
			}

			Console.WriteLine("Sorting...");
			PerformanceTimer.Debug("builtin", 1, () => Array.Sort(b, Compare), maxSize);
			PerformanceTimer.Debug("timsort", 1, () => a.TimSort(Compare), maxSize);

			Console.WriteLine("Testing...");
			for (int i = 0; i < maxSize; i++) Assert.AreEqual(a[i], b[i]);
		}

		[Test]
		public void ReversedTests()
		{
			Console.WriteLine("<<< Generally descending data >>>");

			var r = new Random(seed);
			int[] a = new int[maxSize];
			int[] b = new int[maxSize];
			int value = int.MaxValue;

			Console.WriteLine("Preparing...");
			for (int i = 0; i < maxSize; i++)
			{
				value = 
					r.Next(100) < 80 
					? value - r.Next(100) 
					: value + r.Next(100);
				a[i] = b[i] = value;
			}

			Console.WriteLine("Sorting...");
			PerformanceTimer.Debug("builtin", 1, () => Array.Sort(b, Compare), maxSize);
			PerformanceTimer.Debug("timsort", 1, () => a.TimSort(Compare), maxSize);

			Console.WriteLine("Testing...");
			for (int i = 0; i < maxSize; i++) Assert.AreEqual(a[i], b[i]);
		}

		[Test]
		public void RandomTests_List()
		{
			Console.WriteLine("<<< Random data (buffered List<T>) >>>");

			var r = new Random(seed);
			var a = new List<int>(maxSize);
			int[] b = new int[maxSize];

			Console.WriteLine("Preparing...");
			for (int i = 0; i < maxSize; i++) b[i] = r.Next();
			a.AddRange(b);

			Console.WriteLine("Sorting...");
			PerformanceTimer.Debug("builtin", 1, () => Array.Sort(b, Compare), maxSize);
			PerformanceTimer.Debug("timsort", 1, () => a.TimSort(Compare), maxSize);

			Console.WriteLine("Testing...");
			for (int i = 0; i < maxSize; i++) Assert.AreEqual(a[i], b[i]);
		}

		[Test]
		public void SortedTests_List()
		{
			Console.WriteLine("<<< Generally ascending data (buffered List<T>) >>>");

			var r = new Random(seed);
			var a = new List<int>(maxSize);
			int[] b = new int[maxSize];
			int value = int.MinValue;

			Console.WriteLine("Preparing...");
			for (int i = 0; i < maxSize; i++)
			{
				value = 
					r.Next(100) < 80 
					? value + r.Next(100) 
					: value - r.Next(100);
				b[i] = value;
			}
			a.AddRange(b);

			Console.WriteLine("Sorting...");
			PerformanceTimer.Debug("builtin", 1, () => Array.Sort(b, Compare), maxSize);
			PerformanceTimer.Debug("timsort", 1, () => a.TimSort(Compare), maxSize);

			Console.WriteLine("Testing...");
			for (int i = 0; i < maxSize; i++) Assert.AreEqual(a[i], b[i]);
		}

		[Test]
		public void ReversedTests_List()
		{
			Console.WriteLine("<<< Generally descending data (buffered List<T>) >>>");

			var r = new Random(seed);
			var a = new List<int>(maxSize);
			int[] b = new int[maxSize];
			int value = int.MaxValue;

			Console.WriteLine("Preparing...");
			for (int i = 0; i < maxSize; i++)
			{
				value = 
					r.Next(100) < 80 
					? value - r.Next(100) 
					: value + r.Next(100);
				b[i] = value;
			}
			a.AddRange(b);

			Console.WriteLine("Sorting...");
			PerformanceTimer.Debug("builtin", 1, () => Array.Sort(b, Compare), maxSize);
			PerformanceTimer.Debug("timsort", 1, () => a.TimSort(Compare), maxSize);

			Console.WriteLine("Testing...");
			for (int i = 0; i < maxSize; i++) Assert.AreEqual(a[i], b[i]);
		}

        //[Test]
        //public void RandomTests_ListWithMergeBack()
        //{
        //    Console.WriteLine("<<< Random data (buffered vs non-buffered List<T>) >>>");

        //    var r = new Random(seed);
        //    var a = new List<int>(maxSize);
        //    var b = new List<int>(maxSize);

        //    Console.WriteLine("Preparing...");
        //    for (int i = 0; i < maxSize; i++)
        //    {
        //        var value = r.Next();
        //        a.Add(value);
        //        b.Add(value);
        //    }

        //    Console.WriteLine("Sorting...");
        //    PerformanceTimer.Debug("timsort (non-buffered)", 1, () => a.TimSort(Compare, false), maxSize);
        //    PerformanceTimer.Debug("timsort (buffered)", 1, () => b.TimSort(Compare, true), maxSize);

        //    Console.WriteLine("Testing...");
        //    for (int i = 0; i < maxSize; i++) Assert.AreEqual(a[i], b[i]);
        //}

	    [Test]
	    public void TryNativeTest()
	    {
            int[] a = new int[1000];
            a.TimSort();

            Guid[] g = new Guid[1000];
            g.TimSort();
	    }
	}
}
