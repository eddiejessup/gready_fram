using Combinatorics.Collections;
using ConsoleApplication1.Helpers;
using System;
using System.Diagnostics;
using System.Linq;

namespace ConsoleApplication1.HackerRank
{
	public class HackerRank11
	{
		//private const ulong MOD = 1000000007ul;
		private const ulong MOD = 31ul;

		private static ulong Factorial(ulong n)
		{
			if (n > 1000)
				Console.WriteLine("hi");
			return n == 1 ? 1 : (n * Factorial(n - 1));
		}

		public static ulong CountDivs(ulong L, ulong R, ulong div, ulong rem)
		{
			return (R + div -1 - rem) / div - (L + div - 1 - rem) / div + (R % div == rem ? 1ul : 0ul);
		}

		public static ulong GCF(ulong a, ulong b)
		{
			while (b != 0)
			{
				var temp = b;
				b = a % b;
				a = temp;
			}
			return a;
		}

		public static ulong LCM(ulong a, ulong b)
		{
			return (a / GCF(a, b)) * b;
		}

		public void Go()
		{
			Compare();
		}

		public static void CompareWithBrute()
		{
			var rnd = new Random(1337);
			for (var i = 1; i <= 1000; i++)
			{
				var len = rnd.Next(1, 4);
				var A = Enumerable.Repeat(0, len).Select(_ => (ulong)rnd.Next(1, 10)).ToArray();
				var L = (ulong)rnd.Next(1, 10);
				//var R = L + (ulong)rnd.Next(0, 10);
				var R = L;

				var expected = SolveBrute(A, L, R);
				var actual = SolveGcm(A, L, R);

				if (expected != actual)
				{
					Console.WriteLine(A.Join(","));
					Console.WriteLine(new { L, R, expected, actual });
					throw new InvalidOperationException();
				}
			}
		}

		public static void Performance()
		{
			var rnd = new Random(1337);

			const int maxA = 100000;

			//var A = Enumerable.Repeat(0, 10).Select(_ => (ulong)rnd.Next(1, maxA)).ToArray();
			var A = new ulong[] { 2ul, 3ul, 5ul, 7ul, 11ul, 13ul };

			while (true)
			{
				var mul = A.Aggregate(1ul, (x, y) => x * y);
				if (mul < 100000) break;
				A[A.IndexOfElementWithMax()] = (ulong)rnd.Next(1, maxA);
			}
			var L = (ulong)rnd.Next(1, 10000000);
			var R = L + (ulong)rnd.Next(0, 1000000);

			Console.WriteLine("Generated:");
			Console.WriteLine(A.Join());
			Console.WriteLine(new { L, R });
			Console.WriteLine(new { lcm = A.Aggregate(1ul, (x, y) => LCM(x, y)) });

			var timer = Stopwatch.StartNew();
			Console.WriteLine(SolveLcm2(A, L, R));
			Console.WriteLine("Solve time: " + timer.ElapsedMilliseconds);
		}

		public static void Compare()
		{
			var rnd = new Random(1337);
			for (var i = 1; i <= 10000; i++)
			{
				var len = rnd.Next(1, 5);
				var A = Enumerable.Repeat(0, len).Select(_ => (ulong)rnd.Next(1, 7)).ToArray();
				var L = (ulong)rnd.Next(1, 50);
				var R = L + (ulong)rnd.Next(0, 50);

				var expected = 0ul;
				for (var s = L; s <= R; s++)
					expected += SolveDp2(A, s, len);
				expected = expected % MOD;

				var actual = SolveLcm2(A, L, R);

				if (expected != actual)
				{
					Console.WriteLine(A.Join(","));
					Console.WriteLine(new { L, R, expected, actual });
					throw new InvalidOperationException();
				}
			}
		}

		public static ulong SolveLcm2(ulong[] A, ulong L, ulong R)
		{
			var factorial = 1ul;
			for (var i = 2ul; i < (ulong)A.Length; i++)
				factorial *= i;
			var modf = factorial * MOD;

			var lcm = A.Aggregate(1ul, (x, y) => LCM(x, y));

			var counters = FillCounters(A, lcm);

			var sum = 0ul;

			for (var s = L; s <= R; s++)
			{
				var a0 = s / lcm;
				var b0 = s % lcm;

				for (var j = 0ul; j < (ulong)A.Length; j++)
				{
					if (a0 < j) break;

					var ai = a0 - j;
					var bi = b0 + j * lcm;

					ulong leftPart;
					if (ai == 0 || A.Length == 1) leftPart = 1ul;
					else
					{
						var mul = 1ul;

						for (var i = 1ul; i < (ulong)A.Length; i++)
							mul = ((ai + i) * mul) % modf;

						leftPart = (mul / factorial) % MOD;
					}

					var rightPart = counters[bi];

					var res = (leftPart * rightPart) % MOD;
					sum = (sum + res) % MOD;
				}
			}

			return sum;
		}

		public static ulong[] FillCounters(ulong[] A, ulong lcm)
		{
			var len = (ulong)A.Length * lcm;

			var prevRow = new ulong[len];
			var row = new ulong[len];

			var firstA = A[0];
			for (var j = 0ul; j < lcm; j += firstA)
				prevRow[j] = 1;

			for (var i = 1; i < A.Length; i++)
			{
				var a = A[i];

				for (var j = 0ul; j < len; j++)
				{
					row[j] = (row[j] + prevRow[j]) % MOD;

					if (j >= a)
						row[j] = (row[j] + row[j - a]) % MOD;

					if (j >= lcm)
						row[j] = (row[j] + MOD - prevRow[j - lcm]) % MOD;
				}

				var temp = row;
				row = prevRow;
				prevRow = temp;

				Array.Clear(row, 0, row.Length);
			}

			prevRow[0] = 1;
			return prevRow;
		}

		public static ulong SolveDp_WithMaxLcm(ulong[] A, ulong s, int p, ulong lcm)
		{
			if (s == 0) return 0;

			var lastA = A[p - 1];
			var maxCount = lcm / lastA;

			var cnt = 0ul;

			if (s % lastA == 0 && lcm > s)
				cnt = 1;

			if (p == 1) return cnt;

			for (var i = 0ul; i < Math.Min(maxCount, (s / lastA) + 1); i++)
				cnt = (SolveDp_WithMaxLcm(A, s - i * lastA, p - 1, lcm) + cnt) % MOD;

			return cnt;
		}

		public static ulong Pow(ulong a, ulong pow)
		{
			// TODO: fast pow
			if (pow == 0) return 1ul;
			var res = a;
			for (pow--; pow > 0; pow--)
				res = res * a;
			return res;
		}

		public static void FillCounters(ulong[] A, ulong lcm, int i, ulong sum, ulong[] counters)
		{
			for (var q = 0ul; q < lcm; q += A[i])
			{
				var qsum = q + sum;

				if (i == A.Length - 1)
					counters[qsum]++;
				else
					FillCounters(A, lcm, i + 1, qsum, counters);
			}
		}

		public struct Coin
		{
			public ulong Value;
			public ulong Weight;

			public override string ToString()
			{
				return $"value = {Value}; weight = {Weight}";
			}
		}

		public static ulong MultiplyWeight(ulong cnt, ulong weight)
		{
			if (weight == 1) return 1;
			if (cnt == 0) return 1;
			var mul = Factorial(cnt + weight - 1) / (Factorial(cnt) * Factorial(weight - 1));
			return mul;
		}

		public static ulong SolveGcm(ulong[] A, ulong L, ulong R)
		{
			var sum = SolveGcm(A.Select(a => new Coin { Value = a, Weight = 1 }).ToArray(), L, R);
			return sum;
		}

		public static ulong SolveGcm(Coin[] coins, ulong L, ulong R)
		{
			if (coins.Length == 1)
			{
				var coin = coins[0];

				var sumCoin1 = 0ul;
				
				for (var i = L; i <= R; i++)
					if (i % coin.Value == 0)
					{
						var mw = MultiplyWeight(i / coin.Value, coin.Weight);
						sumCoin1 += mw;
					}

				return sumCoin1;
			}

			coins = coins.OrderBy(c => c.Value).ToArray();

			var coin0 = coins[0];
			var coin1 = coins[1];

			var lcm = LCM(coin0.Value, coin1.Value);
			var d0 = lcm / coin0.Value;
			var d1 = lcm / coin1.Value;

			var newCoin = new Coin
			{
				Value = lcm,
				Weight = MultiplyWeight(d0, coin0.Weight) + MultiplyWeight(d1, coin1.Weight)
			};

			var newCoins = new Coin[coins.Length - 1];
			for (var i = 2; i < coins.Length; i++)
				newCoins[i - 2] = coins[i];
			newCoins[newCoins.Length - 1] = newCoin;

			var sum = 0ul;
			for (var i = 0ul; i < d0; i++)
				for (var j = 0ul; j < d1; j++)
				{
					var shift = i * coin0.Value + j * coin1.Value;
					if (R < shift) continue;

					var cur = SolveGcm(newCoins, L < shift ? 0 : L - shift, R - shift);

					if (i > 0) cur = cur * MultiplyWeight(i, coin0.Weight);
					if (j > 0) cur = cur * MultiplyWeight(i, coin1.Weight);

					sum += cur;
				}

			return sum;
		}

		public static ulong SolveDp3(ulong[] A, ulong L, ulong R, int p)
		{
			var lastA = A[p - 1];

			var c2 = CountDivs(L, R, lastA, 0);

			if (p == 1) return c2;

			var c1 = 0ul;
			for (var s = 0ul; s <= R; s++)
			{
				var a = CountDivs(Math.Max(L, s), R, lastA, s % lastA);
				var b = SolveDp2(A, s, p - 1);
				c1 += a * b;
			}

			return c1 + c2;
		}

		public static ulong SolveDp2(ulong[] A, ulong s, int p)
		{
			if (s == 0) return 0;
			if (p == 1) return s % A[0] == 0ul ? 1ul : 0ul;

			var cnt = 0ul;

			var lastA = A[p - 1];

			if (s % lastA == 0)
				cnt = 1;

			for (var i = 0ul; i <= s / lastA; i++)
				cnt += SolveDp2(A, s - i * lastA, p - 1);

			return cnt;
		}

		public static ulong SolveDp1(ulong[] A, ulong s, int p)
		{
			if (s == 0) return 0;
			if (p == 1) return s % A[0] == 0ul ? 1ul : 0ul;

			var cnt = 0ul;

			var lastA = A[p - 1];

			if (s > lastA)
				cnt = SolveDp1(A, s - lastA, p);
			else if (s == lastA)
				cnt = 1;

			cnt += SolveDp1(A, s, p - 1);

			return cnt;
		}

		public static ulong SolveBrute(ulong[] A, ulong L, ulong R)
		{
			var cnt = 0ul;
			for (var i = L; i <= R; i++)
			{
				var brute = SolveBrute(A, i);
				cnt += brute;
			}
			return cnt;
		}

		public static ulong SolveBrute(ulong[] A, ulong W)
		{
			var cnt = 0ul;
			for (var i = 1ul; i <= W; i++)
				foreach (var comb in new Combinations<ulong>(A, (int)i, GenerateOption.WithRepetition))
					if (comb.Sum() == W)
						cnt++;
			return cnt;
		}

		public void Example1()
		{
			//Console.WriteLine(SolveBrute(new ulong[] { 1, 1, 2 }, 2, 2));
			//Console.WriteLine(SolveGcm(new ulong[] { 2, 1, 3 }, 1, 6));
			//Console.WriteLine(SolveGcm(new ulong[] { 1, 2 }, 6, 6));
			//var A = new ulong[] { 1, 2, 6 }; var L = 8ul; var R = 8ul;
			var A = new ulong[] { 6, 6, 2, 3 }; var L = 12ul; var R = 12ul;
			for (var i = L; i <= R; i++)
			{
				var brute = SolveBrute(A, i, i) % MOD;
				var my = SolveLcm2(A, i, i);
				Console.WriteLine(new { i, brute, my });
			}

			//Console.WriteLine(SolveLcm2(A, L, R));
		}

		public void Example2()
		{
			Console.WriteLine(SolveDp2(new ulong[] { 4, 4, 4, 1 }, 4, 4));
		}

		public static void CheckCountDivs()
		{
			var rnd = new Random(1337);

			for (var t = 0; t <= 10000; t++)
			{
				var L = (ulong)rnd.Next(1, 20);
				var R = L + (ulong)rnd.Next(0, 20);
				var div = (ulong)rnd.Next(1, 10);
				var rem = (ulong)rnd.Next(0, (int)div);

				var expected = 0ul;
				for (var i = L; i <= R; i++)
					if (i % div == rem)
						expected++;

				var actual = CountDivs(L, R, div, rem);

				if (expected != actual)
				{
					Console.WriteLine(new { L, R, div, rem });
					Console.WriteLine(new { expected, actual });
					throw new InvalidOperationException();
				}
			}
		}
	}
}
