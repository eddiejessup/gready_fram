using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// https://www.hackerrank.com/challenges/array-splitting

namespace ConsoleApplication1.HackerRank
{
	class HackerRank22
	{
		public void Go()
		{
			Console.WriteLine(Solve(new ulong[] { 2, 2, 2, 2 }));
			Console.WriteLine(Solve(new ulong[] { 4, 1, 0, 1, 1, 0, 1 })); // суммы: 4 5 5 6 7 7 8
		}

		public static ulong Solve(ulong[] arr)
		{
			var sums = new ulong[arr.Length];
			for (var i = 0; i < arr.Length; i++)
				sums[i] = arr[i] + (i == 0 ? 0ul : sums[i - 1]);

			var result = P(sums, arr, 0, arr.Length - 1);
			return result;
		}

		public static int GetSplitIndex(ulong[] sums, int i, int j)
		{
			var sumBefore = i == 0 ? 0ul : sums[i - 1];
			var sumJ = sums[j];

			if ((sumJ - sumBefore) % 2 != 0) return -1;

			var searchme = sumBefore + (sumJ - sumBefore) / 2;

			var k = BinarySearchLeftBiased(sums, searchme, i, j);

			if (sums[k] != searchme)
				return -1;

			return k;
		}

		public static ulong P(ulong[] sums, ulong[] arr, int i, int j)
		{
			if (j <= i) return 0;

			//Console.WriteLine(arr.Skip(i).Take(j - i + 1).Join());

			var k = GetSplitIndex(sums, i, j);
			if (k < 0)
				return 0;

			var left = P(sums, arr, i, k);
			var right = P(sums, arr, k + 1, j);

			return 1ul + Math.Max(left, right);
		}

		public static int BinarySearchLeftBiased(IList<ulong> arr, ulong searchme, int li, int ri)
		{
			while (true)
			{
				var mi = (li + ri) / 2;

				if (mi == li)
				{
					return arr[ri] > searchme ? li : arr[li] == searchme ? li : ri;
				}

				var miValue = arr[mi];

				if (miValue < searchme)
				{
					li = mi;
				}
				else if (miValue == searchme)
				{
					ri = mi;
				}
				else
				{
					ri = mi;
				}
			}
		}
	}
}
