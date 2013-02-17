using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace TimSort.Tests.App
{
    class Program
    {
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        static void Main(string[] args)
        {
            try
            {
                var suiteId = args[0];
                var summaryFilename = suiteId + ".csv";
                var testId = args[1];
                var algorithm = ParseEnum<AlgorithmType>(args[2]);
                var container = ParseEnum<ContainerType>(args[3]);
                var key = ParseEnum<KeyType>(args[4]);
                var sort = ParseEnum<SortType>(args[5]);
                var length = int.Parse(args[6]);
                testId = string.Format("{0}.{1}.{2}.{3}.{4}.{5}", testId, container, key, sort, length, algorithm);
                var statsFilename = testId + ".tmp";

                var random = new Random(suiteId.GetHashCode());

                Console.WriteLine(testId);

                var stats = LoadStats(statsFilename);
                ExecuteTest(algorithm, container, key, sort, length, random, stats);
                SaveStats(statsFilename, stats);

                UpdateSummaryFile(summaryFilename, testId, stats);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception {0}: {1}", e.GetType().Name, e.Message);
                Help();
            }
        }

        private static void UpdateSummaryFile(string summaryFilename, string testId, List<double> stats)
        {
            Console.WriteLine("  Updating summary...");
            var token = testId + "\t";

            using (var output = File.CreateText(summaryFilename + ".tmp"))
            {
                bool written = false;
                Action writeStats = () =>
                    {
                        if (!written)
                        {
                            // ReSharper disable AccessToDisposedClosure
                            output.WriteLine("{0}\t{1}\t{2}", testId, stats.Min(), stats.Average());
                            // ReSharper restore AccessToDisposedClosure
                            written = true;
                        }
                    };

                if (File.Exists(summaryFilename))
                {
                    foreach (var line in File.ReadLines(summaryFilename))
                    {
                        if (line.StartsWith(token))
                        {
                            writeStats();
                        }
                        else
                        {
                            output.WriteLine(line);
                        }
                    }
                }

                writeStats();
            }

            File.Delete(summaryFilename);
            File.Move(summaryFilename + ".tmp", summaryFilename);
        }

        private static void ExecuteTest(AlgorithmType algorithm, ContainerType container, KeyType key, SortType sort, int length, Random random, List<double> stats)
        {
            Console.WriteLine("  Preparing data...");

            switch (key)
            {
                case KeyType.Int32:
                    ExecuteTest(algorithm, container, MakeRandomInts(length, sort, random), stats);
                    break;
                case KeyType.Guid:
                    ExecuteTest(algorithm, container, MakeRandomGuids(length, sort, random), stats);
                    break;
                case KeyType.String:
                    ExecuteTest(algorithm, container, MakeRandomStrings(length, sort, random), stats);
                    break;
                case KeyType.Comparable:
                    ExecuteTest(algorithm, container, MakeRandomComparable(length, sort, random), stats);
                    break;
                case KeyType.NonComparable:
                    ExecuteTest(algorithm, container, MakeRandomNonComparable(length, sort, random), stats, NonComparableObject.Compare);
                    break;
            }
        }

        private static void ExecuteTest<T>(AlgorithmType algorithm, ContainerType container, T[] array, List<double> stats, Comparison<T> compare = null)
        {
            switch (container)
            {
                case ContainerType.Array:
                    ExecuteTest(algorithm, array, stats, compare);
                    return;
                case ContainerType.List:
                    ExecuteTest(algorithm, array.ToList(), stats, compare);
                    return;
                case ContainerType.IList:
                    ExecuteTest(algorithm, (IList<T>)array.ToList(), stats, compare);
                    return;
                case ContainerType.SlowList:
                    ExecuteTest(algorithm, new SlowList<T>(array), stats, compare);
                    return;
                default:
                    throw new ArgumentException();
            }
        }

        private static void ExecuteTest(Action action, List<double> stats)
        {
            Console.WriteLine("  Executing...");
            var timer = Stopwatch.StartNew();
            action();
            timer.Stop();

            stats.Add(timer.Elapsed.TotalMilliseconds);
        }

        // ReSharper disable ImplicitlyCapturedClosure
        // ReSharper disable ConvertClosureToMethodGroup

        private static void ExecuteTest<T>(AlgorithmType algorithm, T[] array, List<double> stats, Comparison<T> compare)
        {
            Action action;

            if (compare == null)
            {
                if (algorithm == AlgorithmType.QuickSort) action = () => Array.Sort(array);
                else if (algorithm == AlgorithmType.TimSort) action = () => array.TimSort();
                else throw new ArgumentException();
            }
            else
            {
                if (algorithm == AlgorithmType.QuickSort) action = () => Array.Sort(array, compare);
                else if (algorithm == AlgorithmType.TimSort) action = () => array.TimSort(compare);
                else throw new ArgumentException();
            }

            ExecuteTest(action, stats);
        }

        private static void ExecuteTest<T>(AlgorithmType algorithm, List<T> list, List<double> stats, Comparison<T> compare)
        {
            Action action;

            if (compare == null)
            {
                if (algorithm == AlgorithmType.QuickSort) action = () => list.Sort();
                else if (algorithm == AlgorithmType.TimSort) action = () => list.TimSort();
                else throw new ArgumentException();
            }
            else
            {
                if (algorithm == AlgorithmType.QuickSort) action = () => list.Sort(compare);
                else if (algorithm == AlgorithmType.TimSort) action = () => list.TimSort(compare);
                else throw new ArgumentException();
            }

            ExecuteTest(action, stats);
        }

        private static void ExecuteTest<T>(AlgorithmType algorithm, IList<T> list, List<double> stats, Comparison<T> compare)
        {
            Action action;

            if (compare == null)
            {
                if (algorithm == AlgorithmType.TimSort) action = () => list.TimSort();
                else throw new ArgumentException();
            }
            else
            {
                if (algorithm == AlgorithmType.TimSort) action = () => list.TimSort(compare);
                else throw new ArgumentException();
            }

            ExecuteTest(action, stats);
        }

        // ReSharper restore ConvertClosureToMethodGroup
        // ReSharper restore ImplicitlyCapturedClosure

        private static int[] MakeRandomInts(int length, SortType type, Random random)
        {
            var array = new int[length];
            for (int i = 0; i < length; i++) array[i] = random.Next();
            return ApplySortType(array, type, random);
        }

        private static Guid[] MakeRandomGuids(int length, SortType type, Random random)
        {
            var i = 0;
            var array = new Guid[length];
            foreach (var guid in GuidSequence(length, random)) array[i++] = guid;
            return ApplySortType(array, type, random);
        }

        private static string[] MakeRandomStrings(int length, SortType type, Random random)
        {
            var i = 0;
            var array = new string[length];
            foreach (var guid in StringSequence(length, 6, 64, random)) array[i++] = guid;
            return ApplySortType(array, type, random);
        }

        private static ComparableObject[] MakeRandomComparable(int length, SortType type, Random random)
        {
            var array = new ComparableObject[length];
            for (int i = 0; i < length; i++) array[i] = new ComparableObject(random);
            return ApplySortType(array, type, random);
        }

        private static NonComparableObject[] MakeRandomNonComparable(int length, SortType type, Random random)
        {
            var array = new NonComparableObject[length];
            for (int i = 0; i < length; i++) array[i] = new NonComparableObject(random);
            return ApplySortType(array, type, random, NonComparableObject.Compare);
        }

        private static T[] ApplySortType<T>(T[] array, SortType sortType, Random random, Comparison<T> compare = null)
        {
            if (sortType == SortType.Random) return array;

            Console.WriteLine("  Sorting...");

            if (compare == null)
                Array.Sort(array);
            else
                Array.Sort(array, compare);

            switch (sortType)
            {
                case SortType.Ascending:
                    return array;
                case SortType.Ascending80:
                    Randomize20(array, random);
                    return array;
                case SortType.Descending:
                    Array.Reverse(array);
                    return array;
                case SortType.Descending80:
                    Array.Reverse(array);
                    Randomize20(array, random);
                    return array;
                default:
                    throw new ArgumentException();
            }
        }

        private static void Randomize20<T>(T[] array, Random random)
        {
            Console.WriteLine("  Randomizing...");

            var length = array.Length;
            var limit = length / 10;
            for (int i = 0; i < limit; i++)
            {
                var x = random.Next(length);
                var y = random.Next(length);
                var t = array[x];
                array[x] = array[y];
                array[y] = t;
            }
        }

        private static List<double> LoadStats(string fileName)
        {
            if (!File.Exists(fileName)) return new List<double>();
            using (var reader = new BinaryReader(File.OpenRead(fileName)))
            {
                var result = new List<double>();
                var count = reader.ReadInt32();
                while (count-- > 0) result.Add(reader.ReadDouble());
                return result;
            }
        }

        private static void SaveStats(string fileName, IEnumerable<double> stats)
        {
            var array = stats.ToArray();
            using (var writer = new BinaryWriter(File.Create(fileName)))
            {
                writer.Write(array.Length);
                foreach (var item in array) writer.Write(item);
            }
        }

        /// <summary>Produces sequence or random strings.</summary>
        /// <param name="size">The size.</param>
        /// <param name="minimumLength">The minimum length.</param>
        /// <param name="maximumLength">The maximum length.</param>
        /// <param name="random">The random.</param>
        /// <returns>Sequence of strings.</returns>
        private static IEnumerable<string> StringSequence(int size, int minimumLength, int maximumLength, Random random)
        {
            var b = new char[maximumLength];
            var width = maximumLength - minimumLength;

            while (size > 0)
            {
                var l = minimumLength + random.Next(width);
                for (int i = 0; i < l; i++) b[i] = (char)(32 + random.Next(95));
                yield return new string(b, 0, l);
                size--;
            }
        }

        private static IEnumerable<Guid> GuidSequence(int size, Random random)
        {
            var guidBytes = new byte[16];

            while (size > 0)
            {
                random.NextBytes(guidBytes);
                yield return new Guid(guidBytes);
                size--;
            }
        }

        private static void Help()
        {
            Console.WriteLine("Arguments:");
            Console.WriteLine("  SuiteId: test group id (file name for summary file");
            Console.WriteLine("  TestId: test id (summary file will be sorted by this field)");
            Console.WriteLine("  Algorithm: QuickSort|TimSort");
            Console.WriteLine("  Containder: Array|List|IList|SlowList");
            Console.WriteLine("  Key: Int32|Guid|String|Comparable|NonComparable");
            Console.WriteLine("  Sort: Ascending|Ascending80|Random|Descending80|Descending");
            Console.WriteLine("  Length: number of elements");
        }
    }
}
