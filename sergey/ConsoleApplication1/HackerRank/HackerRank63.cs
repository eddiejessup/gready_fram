using ConsoleApplication1.DataTypes;
using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1.HackerRank
{
	class HackerRank63
	{
		public void Go()
		{
			ShowInnerCombCoeff();
		}

		private static ulong MOD = 1000000007ul;

		public static void Compare()
		{
			var rnd = new Random(1337);
			for (var n = 2; n <= 10; n++)
			{
				var arr = ArrayHelper.Create(n, i => i + 1);
				var brute = SolveBrute(arr);
				var actual = Solve(arr);

				if (!actual.SequenceEqual(brute))
				{
					Console.WriteLine(brute.Join());
					Console.WriteLine(actual.Join());
					Console.WriteLine(new { n });
					throw new InvalidOperationException();
				}

				Console.WriteLine(n + " ok");
			}
		}

		public static ulong[] Solve(int[] B)
		{
			var max = 301;

			var factorials = new ulong[max];
			factorials[0] = 1;
			for (var i = 1ul; i < (ulong)factorials.Length; i++)
				factorials[i] = factorials[i - 1] * i % MOD;

			var inverses = new ulong[301];
			for (var i = 0ul; i < (ulong)factorials.Length; i++)
				inverses[i] = MyMath.ModularInverse(factorials[i], MOD);

			var c = (Func<int, int, ulong>)((l, h) => l > h ? 0ul : (factorials[h] * inverses[l] % MOD) * inverses[h - l] % MOD);

			var n = B.Length;

			var result = new ulong[n - 1];

			result[0] = c(2, n);
			if (n > 2) result[1] = 1 + 3 * c(4, n) + 2 * c(3, n);

			var brute = SolveBrute(B);
			for (var i = 2; i < brute.Length; i++)
				result[i] = brute[i];

			return result;
		}

		public static void Example1()
		{
			for (var n = 2; n <= 10; n++)
			{
				Console.WriteLine(new { n, ans = SolveBrute(ArrayHelper.Create(n, i => i + 1)).Join() });
			}
		}

		public static void Example2()
		{
			var max = 301;

			var factorials = new ulong[max];
			factorials[0] = 1;
			for (var i = 1ul; i < (ulong)factorials.Length; i++)
				factorials[i] = factorials[i - 1] * i % MOD;

			var inverses = new ulong[301];
			for (var i = 0ul; i < (ulong)factorials.Length; i++)
				inverses[i] = MyMath.ModularInverse(factorials[i], MOD);

			Console.WriteLine(new
			{
				p1 = 1,
				p2_2 = C(factorials, inverses, 4, 6),
				p3 = C(factorials, inverses, 3, 6),
				p4 = C(factorials, inverses, 4, 6)
			});

			Console.WriteLine(C(factorials, inverses, 4, 6));
			Console.WriteLine(C(factorials, inverses, 3, 6));

			//SolveBrute(ArrayHelper.Create(6, i => i + 1));
		}

		public static void ShowInnerCombCoeff()
		{
			var max = 301;

			var factorials = new ulong[max];
			factorials[0] = 1;
			for (var i = 1ul; i < (ulong)factorials.Length; i++)
				factorials[i] = factorials[i - 1] * i % MOD;

			var inverses = new ulong[301];
			for (var i = 0ul; i < (ulong)factorials.Length; i++)
				inverses[i] = MyMath.ModularInverse(factorials[i], MOD);

			Console.WriteLine(InnerCombCoeff(factorials, inverses, new[] { 3, 2 }));
		}

		public static ulong InnerCombCoeff(ulong[] factorials, ulong[] inverses, int[] cycleLengths)
		{
			var cnt = 1ul;

			var n = cycleLengths.Sum();

			foreach (var cycleLen in cycleLengths)
			{
				cnt = cnt * C(factorials, inverses, cycleLen, n) % MOD;

				n -= cycleLen;
			}

			return cnt;
		}

		public static void ShowCycleCoeff()
		{
			for (var len = 1; len <= 50; len++)
				Console.WriteLine(new { len, cycle_coeff = CycleCoeff(len) });
		}

		public static ulong CycleCoeff(int cycleLen)
		{
			var cnt = 1ul;
			for (var i = 2; i < cycleLen; i++)
				if (MyMath.ExtendedEuclidGcd((ulong)i, (ulong)cycleLen) == 1)
					cnt++;
			return cnt;
		}

		public static ulong C(ulong[] factorials, ulong[] inverses, int low, int high)
		{
			return (factorials[high] * inverses[low] % MOD) * inverses[high - low] % MOD;
		}

		public static ulong[] SolveBrute(int[] B)
		{
			var n = B.Length;

			var result = new ulong[n - 1];

			var permutations = new List<int[]> { B };

			for (var k = 1; k < n; k++)
			{
				var hash = new HashSet<string>();

				var nextperms = new List<int[]>();

				foreach (var comb in permutations)
				{
					for (var i = 0; i < n; i++)
						for (var j = i + 1; j < n; j++)
						{
							Swap(comb, i, j);

							if (hash.Add(string.Join(" ", comb)))
								nextperms.Add(comb.ToArray());

							Swap(comb, i, j);
						}
				}

				//var stats = new List<int>();
				//foreach (var s in hash)
				//{
				//	var arr = s.Split().Select((c, i) => c == (i + 1).ToString() ? "-" : c).ToArray();
				//	var cnt = arr.Count(c => c != "-");
				//	stats.Add(cnt);
				//	Console.WriteLine(arr.Join() + "; " + cnt);
				//}
				//Console.WriteLine(stats.GroupBy(i => i).Select(gr => new { gr.Key, cnt = gr.Count() }.ToString()).Join("; "));
				//Console.WriteLine();
				//if (k == 2) break;

				permutations = nextperms;

				result[k - 1] = (ulong)permutations.Count % MOD;
			}

			return result;
		}

		public static void Swap(int[] A, int i, int j)
		{
			var tmp = A[i];
			A[i] = A[j];
			A[j] = tmp;
		}
	}
}
