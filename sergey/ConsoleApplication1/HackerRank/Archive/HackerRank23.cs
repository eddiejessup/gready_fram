using ConsoleApplication1.DataTypes;
using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.HackerRank
{
	class HackerRank23
	{
		public void Go()
		{
			Performance();
			//Console.WriteLine(Solve_NlogN("6 7 4".Split().Select(ulong.Parse).ToArray(), 5)); 
			//Compare();
			//Console.WriteLine(Solve_Brute(new ulong[] { 3, 3, 9, 9, 5 }, 7));
		}

		public void Performance()
		{
			var rnd = new Random(1337);
			var arr = RandomGenerator.RandomUlongArray(rnd, 10000000, 1, 1000);  
			var m = (ulong)rnd.Next(2, 100000);
			Console.WriteLine(Solve_SortedSet(arr, m));
		}

		public static void Compare()
		{
			var rnd = new Random(1337);
			for (var t = 0; t < 1000000; t++)
			{
				var len = rnd.Next(3, 10);
				var arr = RandomGenerator.RandomUlongArray(rnd, len, 1, 100);
				var m = (ulong)rnd.Next(2, 1000);
				var brute = Solve_N2(arr, m);
				var actual = Solve(arr, m);

				if (actual != brute)
				{
					Console.WriteLine(arr.Join());
					Console.WriteLine(new { m, brute, actual });
					throw new InvalidOperationException();
				}
			}
		}

		public static ulong Solve_SortedSet(ulong[] arr, ulong m)
		{
			// 10000000 - ??? ms

			var sumsLeft = new ulong[arr.Length];
			sumsLeft[0] = arr[0] % m;
			for (var i = 1; i < arr.Length; i++)
				sumsLeft[i] = (sumsLeft[i - 1] + arr[i]) % m;

			var sumsSorted = new SortedSet<ulong>(sumsLeft);
			var sumsCount = new CountDictionary<ulong>(sumsLeft);

			var max = arr[0] % m;

			for (var i = 0; i < arr.Length; i++)
			{
				var left = i == 0 ? 0ul : sumsLeft[i - 1];

				var searchme = left == 0 ? (m - 1) : (left - 1);

				var searchResult = sumsSorted.GetViewBetween(0, searchme);

				if (searchResult.Count > 0)
					max = Math.Max(max, (searchResult.Max + m - left) % m);

				var removeMe = sumsLeft[i];

				sumsCount.Decrement(removeMe);

				if (sumsCount.Stat(removeMe) == 0)
					sumsSorted.Remove(removeMe);
			}

			return max;
		}

		public static ulong Solve_LLRBT(ulong[] arr, ulong m)
		{
			// 10000000 - 48545 ms

			var sumsLeft = new ulong[arr.Length];
			sumsLeft[0] = arr[0] % m;
			for (var i = 1; i < arr.Length; i++)
				sumsLeft[i] = (sumsLeft[i - 1] + arr[i]) % m;

			var sumsSorted = new LeftLeaningRedBlackTree<ulong, int>((x, y) => x.CompareTo(y), (x, y) => x.CompareTo(y));
			foreach (var item in sumsLeft)
				sumsSorted.Add(item, 0);

			var max = arr[0] % m;

			for (var i = 0; i < arr.Length; i++)
			{
				var left = i == 0 ? 0ul : sumsLeft[i - 1];

				var searchme = left == 0 ? (m - 1) : (left - 1);

				foreach (var node in sumsSorted.Search(n => n.Key == searchme ? (bool?)null : n.Key < searchme ? false : true))
					if (node.Key <= searchme)
						max = Math.Max(max, (node.Key + m - left) % m);

				sumsSorted.Remove(sumsLeft[i], 0);
			}

			return max;
		}

		public static ulong Solve(ulong[] arr, ulong m)
		{
			// 10000000 - 21146 ms

			var sumsLeft = new ulong[arr.Length];
			sumsLeft[0] = arr[0] % m;
			for (var i = 1; i < arr.Length; i++)
				sumsLeft[i] = (sumsLeft[i - 1] + arr[i]) % m;

			var index = new int[arr.Length];
			for (var i = 0; i < index.Length; i++)
				index[i] = i;
			index = index.OrderBy(i => sumsLeft[i]).ToArray();

			var max = arr[0] % m;

			for (var i = 0; i < arr.Length; i++)
			{
				var left = i == 0 ? 0ul : sumsLeft[i - 1];

				var searchme = left == 0 ? (m - 1) : (left - 1);

				var startSearch = BinarySearchRightBiased(index, ii => sumsLeft[ii], searchme);

				// Из-за этого цикла сложность получается N^2
				// Чтобы его убрать, надо использовать не массив, а сбалансированное дерево (RB-tree)
				// В дереве поиск тоже O(log n)
				// А чтобы убрать фильтр по индексу - нужно для каждого i из дерева удалять значение sumsLeft[i]		
				// Однако, на случайных данных этот алгоритм самый быстрый	
				for (var j = startSearch; j >= 0; j--)
					if (index[j] >= i)
					{
						var sum = (sumsLeft[index[j]] + m - left) % m;
						max = Math.Max(max, sum);
						break;
					}
			}

			return max;
		}

		public static int BinarySearchRightBiased(int[] arr, Func<int, ulong> getValue, ulong searchme)
		{
			var li = 0;
			var ri = arr.Length - 1;

			while (true)
			{
				var mi = (li + ri) / 2;

				if (mi == li)
				{
					return getValue(arr[li]) < searchme ? ri : getValue(arr[ri]) == searchme ? ri : li;
				}

				var miValue = getValue(arr[mi]);

				if (miValue < searchme)
				{
					li = mi;
				}
				else if (miValue == searchme)
				{
					li = mi;
				}
				else
				{
					ri = mi;
				}
			}
		}

		public static ulong Solve_N2(ulong[] arr, ulong m)
		{
			var sumsRight = new ulong[arr.Length + 1];
			sumsRight[arr.Length - 1] = arr[arr.Length - 1];
			for (var i = arr.Length - 2; i >= 0; i--)
				sumsRight[i] = sumsRight[i + 1] + arr[i];

			var total = sumsRight[0];

			var max = arr[0] % m;
			var sumLeft = 0ul;
			for (var i = 0; i <= arr.Length - 1; i++)
			{
				max = Math.Max(max, arr[i] % m);
				for (var j = i; j <= arr.Length - 1; j++)
				{
					var sum = total - sumLeft - sumsRight[j + 1];
					max = Math.Max(max, sum % m);
				}
				sumLeft = sumLeft + arr[i];
			}
			return max;
		}

		// O(n^3)
		public static ulong Solve_Brute(ulong[] arr, ulong m)
		{
			var max = arr[0] % m;
			for (var i = 0; i <= arr.Length - 1; i++)
				for (var j = i; j <= arr.Length - 1; j++)
				{
					var sum = 0ul;
					for (var k = i; k <= j; k++)
						sum += arr[k];
					max = Math.Max(max, sum % m);
				}
			return max;
		}
	}
}
