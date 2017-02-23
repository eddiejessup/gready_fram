using Combinatorics.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication1
{
	public class HackerRank1
	{
		private static ulong MOD = 1000000007ul;
		//private const ulong MOD = 13ul;
		
		public void Go()
		{
			//GoBrute(2, 3);
			//GoBrute(3, 3);
			//GoBrute(7, 7);
			//GoDynProg(7, 7);
			//GoDynProg2Optimized(7, 13);

			//Console.WriteLine(GoDynProg(4, 3));

			//SimpleTest();
			TestWithBrute();
			//PerfomanceTest();

			//Console.WriteLine(new { expected = 430492191ul, actual = GoDynProg2Optimized(1000, 1000000),  });
		}

		public void PerfomanceTest()
		{
			var sum = 0ul;

			for (var n = 2; n <= 150; n++)
				for (var p = 1ul; p <= 200; p++)
				{
					sum = (sum + GoDynProg2Optimized(n, p)) % MOD;
				}

			Console.WriteLine(sum);
		}

		public void TestWithBrute()
		{
			// GoDynProg2Optimized проверен с MOD = 13, n <= 7, p <= 20

			MOD = 13;

			for (var n = 2; n <= 7; n++)
				for (var p = 1ul; p <= 7; p++)
				{
					var brute = GoBrute(n, p);
					var dp = GoDynProg2Optimized(n, p);
					if (brute != dp)
						throw new InvalidOperationException(new { n, p, brute, dp }.ToString());
				}
		}

		public void SimpleTest()
		{
			for (var n = 2; n <= 100; n++)
				for (var p = 1ul; p <= 100; p++)
				{
					var brute = GoDynProg(n, p);
					var dp = GoDynProg2Optimized(n, p);
					if (brute != dp)
						throw new InvalidOperationException(new { n, p, brute, dp }.ToString());
				}
		}

		public ulong GoDynProg2Optimized(int n, ulong p)
		{
			if (p == 1) return 1;

			var high_count = p % 2 == 0 ? (p / 2) : (p / 2 + 1);

			var t = new ulong[p/2 + 1];
			for (var i = 1; i < t.Length; i++)
				t[i] = 1;
			var t_high = 1ul;

			var t2 = new ulong[p/2 + 1];

			for (var nn = 2; nn <= n; nn++)
			{
				Console.WriteLine(string.Join(",", t));
				Console.WriteLine(t_high);

				var d = 1ul;
				var x = t[1];
				for (var i = p / 2; i >= 2; i--)
				{
					var new_d = p / i;
					if (new_d != d)
					{
						for (var j = d + 1; j <= new_d; j++)
							x = (x + t[j]) % MOD;

						d = new_d;
					}

					t2[i] = x;
				}

				t2[1] = Sum(t, high_count, t_high);

				t_high = t[1];

				var temp = t;
				t = t2;
				t2 = temp;
			}

			Console.WriteLine(string.Join(",", t));
			Console.WriteLine(t_high);

			return Sum(t, high_count, t_high);
		}

		private static ulong Sum(ulong[] arr, ulong high_count, ulong t_high)
		{
			//var sum = 0ul;

			//for (var i = 0ul; i < high_count; i++)
			//	sum = (sum + t_high) % MOD;

			//foreach (var i in arr)
			//	sum = (sum + i) % MOD;

			//return sum;

			var sum = high_count * t_high % MOD;
			foreach (var tt in arr)
				sum = (sum + tt) % MOD;
			return sum;
		}

		public ulong GoDynProg2(int n, ulong p)
		{
			if (p == 1) return 1;

			var t = new ulong[p + 1];
			for (var i = 1; i < t.Length; i++)
				t[i] = 1;

			var t2 = new ulong[p + 1];
			
			for (var nn = 2; nn <= n; nn++)
			{
				var d = 1ul;
				var x = t[1];
				for (var i = p; i >= 1; i--)
				{
					var new_d = p / i;
					if (new_d != d)
					{
						for (var j = d + 1; j <= new_d; j++)
							x = (x + t[j]) % MOD;

						d = new_d;
					}

					t2[i] = x;
				}

				var temp = t;
				t = t2;
				t2 = temp;
			}

			{
				var sum = 0ul;
				foreach (var tt in t)
					sum = (sum + tt) % MOD;
				return sum;
			}
		}

		public ulong GoDynProgOptimized(int n, ulong p)
		{
			if (p == 1) return 1;

			var high_count = p % 2 == 0 ? (p / 2) : (p / 2 + 1);

			var t = new ulong[p/2 + 1];
			for (var i = 1ul; i <= p / 2; i++)
				t[i] = 1;
			var t_high = 1ul;

			var tn1 = new ulong[p/2 + 1];

			for (var nn = 2; nn <= n; nn++)
			{
				{
					var sum = high_count * t_high % MOD;

					for (var j = 1ul; j <= p / 2; j++)
						sum = (sum + t[j]) % MOD;

					tn1[1] = sum;
				}

				for (var i = 2ul; i <= p / 2; i++)
				{
					var sum = 0ul;

					for (var j = 1ul; j * i <= p; j++)
						sum = (sum + t[j]) % MOD;

					tn1[i] = sum;
				}

				t_high = t[1];

				var temp = t;
				t = tn1;
				tn1 = temp;
			}

			{
				var sum = high_count * t_high % MOD;
				foreach (var tt in t)
					sum = (sum + tt) % MOD;
				return sum;
			}
		}

		public ulong GoDynProg(int n, ulong p)
		{
			var t = new ulong[p + 1];
			for (var i = 1ul; i <= p; i++)
				t[i] = 1;

			var tn1 = new ulong[p + 1];

			for (var nn = 2; nn <= n; nn++)
			{
				for (var i = 1ul; i <= p; i++)
				{
					var sum = 0ul;

					for (var j = 1ul; j * i <= p; j++)
						sum = (sum + t[j]) % MOD;

					tn1[i] = sum;
				}

				var temp = t;
				t = tn1;
				tn1 = temp;
			}

			{
				var sum = 0ul;
				foreach (var tt in t)
					sum = (sum + tt) % MOD;
				return sum;
			}
		}

		public ulong GoBrute(int n, ulong p)
		{
			var values = new List<ulong>();
			for (var i = 1ul; i <= p; i++)
				values.Add(i);

			var count = 0ul;

			foreach (var seq in new Variations<ulong>(values, n, GenerateOption.WithRepetition))
				if (IsPSeq(seq, p))
				{
					//Console.WriteLine(string.Join(",", seq));
					count++;
					count = count % MOD;
				}

			Console.WriteLine(new { n, p, count });

			return count;
		}
		
		private static bool IsPSeq(IList<ulong> seq, ulong p)
		{
			for (var i = 1; i < seq.Count; i++)
				if (seq[i - 1] * seq[i] > p)
					return false;
			return true;
		}
	}
}
