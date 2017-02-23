using Combinatorics.Collections;
using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.HackerRank
{
	class HackerRank28
	{
		public void Go()
		{
			//Console.WriteLine(SolveBrute(10, 5));
			ShowTable();
			//Console.WriteLine(SolveBrute(4, 3));
		}

		public static void ShowTable()
		{
			for (var m = 1; m <= 10; m++)
			{
				for (var n = 1; n <= m; n++)
				{
					Console.WriteLine(new { m, n, count = SolveBrute(m, n) });
				}
				Console.WriteLine();
			}
		}

		const ulong MOD = 1000000007ul;

		public static ulong SolveBrute(int m, int n)
		{
			var elements = Enumerable.Range(1, n).ToArray();
			var count = 0ul;
			foreach (var comb in new Variations<int>(elements, m, GenerateOption.WithRepetition))
			{
				var num = 1;
				for (var i = 0; i < comb.Count; i++)
					if (comb[i] == num)
						num++;
				if (num == n + 1)
					count++;
			}
			return count;
		}
	}
}
