using Combinatorics.Collections;
using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using static ConsoleApplication1.Helpers.GameTheoryHelper;

namespace ConsoleApplication1.HackerRank
{
	class HackerRank36
	{
		public void Go()
		{
			ShowOneGameStats();
		}

		public static void ShowOneGameStats()
		{
			var maxN = 20;

			var subs = ArrayHelper.Create(100, i => i);

			for (var K = 2; K <= 7; K++)
			{
				if (K == 3) maxN = 90;
				else maxN = 20;

				var gr = new int[maxN + 1];

				for (var n = 2; n <= maxN; n++)
				{
					var nSubs = ArrayHelper.Slice(subs, 1, n - 1);

					var next = new HashSet<int>();

					for (var k = 2; k <= K; k++)
						foreach (var comb in new Combinations<int>(nSubs, k, GenerateOption.WithRepetition))
							if (comb.Sum() == n)
							{
								var xor = 0;
								foreach (var i in comb)
									xor ^= gr[i];
								 
								next.Add(xor);
							}

					gr[n] = SpragueGrundy(next);
				}

				Console.WriteLine("K = " + K);
				Console.WriteLine(gr.Join());

				if (K == 3)
				{
					Console.WriteLine("Diff:");
					Console.WriteLine(gr.Select((g, i) => i == 0 ? 0 : (gr[i] - gr[i - 1])).Join());
					Console.WriteLine("N - gr");
					Console.WriteLine(gr.Select((g, i) => i - g).Join());
				}

				Console.WriteLine();
			}
		}
	}
}
