namespace TimSort.Tests
{
	class Program
	{
		static void Main(string[] args)
		{
			var tests = new PerformanceTests();

			tests.RandomTests();
			tests.SortedTests();
			tests.ReversedTests();

			tests.RandomTests_List();
			tests.SortedTests_List();
			tests.ReversedTests_List();

            //tests.RandomTests_ListWithMergeBack();
		}
	}
}
