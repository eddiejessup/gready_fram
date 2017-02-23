using Combinatorics.Collections;
using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication1.HackerRank
{
	class HackerRank12
	{
		public void Go()
		{
			Solve(new ulong[] { 9, 7 }, 6);

			var rnd = new Random(1337);

			for (var t = 0; t < 1000; t++)
			{
				var len = rnd.Next(1, 3);
				var S = Enumerable.Repeat(0, len).Select(i => (ulong)rnd.Next(1, 10)).ToArray();
				var K = (ulong)rnd.Next(1, 20);

				var expected = SolveBrute(S, K);
				var actual = Solve(S, K);

				if (expected != actual)
				{
					Console.WriteLine(new { len, K });
					Console.WriteLine(S.Join());
					Console.WriteLine(new { expected, actual });
					throw new InvalidOperationException();
				}
			}
		}

		public ulong SolveBrute(ulong[] S, ulong K)
		{
			var best = 0ul;

			for (var i = 1; i <= S.Length; i++)
			{
				foreach (var comb in new Combinations<ulong>(S, i, GenerateOption.WithoutRepetition))
				{
					if (Check(comb, K))
					{
						best = (ulong)i;
						break;
					}
				}
			}

			return best;
		}

		private static bool Check(IList<ulong> comb, ulong K)
		{
			for (var j = 0; j < comb.Count; j++)
				for (var k = 0; k < j; k++)
					if ((comb[j] + comb[k]) % K == 0)
						return false;
			return true;
		}

		public ulong Solve(ulong[] S, ulong K)
		{
			var divs = S.GroupBy(a => a % K).Select(gr => new { rem = gr.Key, count = (ulong)gr.Count() }).ToArray();

			var counts = new ulong[K];

			foreach (var item in divs)
				counts[item.rem] = item.count;

			var sum = counts[0] > 0 ? 1ul : 0ul;

			if (K % 2 == 0 && counts[(int)K / 2] > 0) sum++;

			for (var i = 1; i <= (int)K / 2; i++)
			{
				var a = counts[i];
				var b = counts[(int)K - i];

				if (!(i == (int)K / 2 && K % 2 == 0))
					sum += Math.Max(a, b);
			}

			return sum;
		}
	}
}
