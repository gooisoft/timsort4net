using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimSort.Tests
{
	class Program
	{
		static void Main(string[] args)
		{
			PerformanceTests tests = new PerformanceTests();

			tests.RandomTests();
			tests.SortedTests();
			tests.ReversedTests();

			tests.RandomTests_List();
			tests.SortedTests_List();
			tests.ReversedTests_List();

			tests.RandomTests_ListWithMergeBack();
		}
	}
}
