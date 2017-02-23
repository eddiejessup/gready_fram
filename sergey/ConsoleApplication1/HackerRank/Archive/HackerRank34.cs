using ConsoleApplication1.DataTypes;
using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApplication1.Helpers.GameTheoryHelper;

namespace ConsoleApplication1.HackerRank
{
	class HackerRank34
	{
		public void Go()
		{
			Solve();
			//ShowStat();
		}

		public static void Solve()
		{
			Console.WriteLine(long.MaxValue);

			var bests = new long[35];
			bests[1] = 1;
			for (var grn = 2; grn < bests.Length; grn++)
			{
				var bestDelta = long.MaxValue;
				for (var grj = 1; grj <= grn; grj++)
				{
					var grk = grn ^ 1 ^ grj;
					if (grk <= grj - 2)
					{
						var k = (1L << grk) - 1;
						var j = 1L << (grj - 1);
						bestDelta = Math.Min(bestDelta, j - k);
					}
					else if (grk == grj - 1)
					{
						var k = 1L << (grk - 1);
						bestDelta = Math.Min(bestDelta, k);
					}
				}
				bests[grn] = bestDelta;
			}

			Console.WriteLine("bests: ");
			Console.WriteLine(bests.Join());

			var t = new[] { 1, 10, 6, 8, 123456, 50 };
			var exp = new[] { 1, 7, 2, 7, 32768, 4 };

			for (var ti = 0; ti < t.Length; ti++)
			{
				var n = t[ti];

				long res;
				if (n % 2 == 1)
					res = 1;
				else
				{
					var grn = 0;
					for (var j = n; j > 0; j >>= 1) grn++;

					res = bests[grn];

					Console.WriteLine(new { n, grn, exp = exp[ti], res });
				}

				if (res != exp[ti])
					throw new InvalidOperationException();
			}
		}

		public void ShowStat()
		{
			var N = 1000ul;

			var gr = new ulong[N + 1];
			for (var i = 1ul; i <= N; i++)
			{
				var next = new HashSet<ulong>();

				for (var j = 0ul; j <= i / 2; j++)
					next.Add(gr[j]);

				gr[i] = SpragueGrundy(next);
			}
			Console.WriteLine("Brute SG calc:");
			Console.WriteLine(gr.Join());

			var guess = new ulong[N + 1];
			for (var i = 1ul; i <= N; i++)
			{
				var r = 0ul;
				for (var j = i; j > 0; j >>= 1) r++;
				guess[i] = r;
			}
			Console.WriteLine("Guess SG calc:");
			Console.WriteLine(guess.Join());

			var xors = new ulong[N + 1];
			for (var i = 1ul; i <= N; i++)
				xors[i] = xors[i - 1] ^ gr[i];
			Console.WriteLine("XORS:");
			Console.WriteLine(xors.Join());

			Console.WriteLine("XORS guess:");
			Console.WriteLine(gr.Select((g, i) => i == 0 ? 0ul : (i % 2 == 1) ? 1ul : (g ^ 1)).Join());
		}
	}
}
