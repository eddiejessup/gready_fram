using Combinatorics.Collections;
using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.HackerRank
{
	public class HackerRank55
	{
		public void Go()
		{
			//Console.WriteLine(Solve(12, 8, 3).Join());
			Compare();
		}

		public static void Compare()
		{
			var rnd = new Random(1337);
			for (var t = 0; t < 100000; t++)
			{
				var k = (ulong)rnd.Next(1, 10);
				var n = (ulong)rnd.Next(1, 30);
				var b = (ulong)rnd.Next(1, (int)k + 1);
				var brute = SolveBrute(n, k, b);
				var actual = Solve(n, k, b);

				var bruteS = brute == null ? "null" : brute.Join();
				var actualS = actual == null ? "null" : actual.Join();

				var isDiff = (brute == null && actual != null);

				isDiff |= (brute != null && actual == null);

				isDiff |= actual != null && (actual.Length != (int)b
					|| (actual.Sum() != n)
					|| (actual.Distinct().Count() != (int)b)
					|| actual.Any(i => i <= 0 || i > n));

				if (isDiff)
				{
					Console.WriteLine(new { n, k, b });
					Console.WriteLine(new { t, brute = bruteS, actual = actualS });
					throw new InvalidOperationException();
				}
			}
		}

		public static ulong[] SolveBrute(ulong n, ulong k, ulong b)
		{
			var all = new ulong[k];
			for (var i = 1ul; i <= k; i++)
				all[i - 1] = i;

			foreach (var ans in new Combinations<ulong>(all, (int)b))
				if (ans.Sum() == n)
					return ans.ToArray();

			return null;
		}

		public static ulong[] Solve(ulong n, ulong k, ulong b)
		{
			var ans = new ulong[b];
			var sum = 0ul;
			for (var i = 1ul; i <= b; i++)
			{
				ans[i - 1] = i;
				sum += i;
			}

			var lastX = b - 1;
			var lastY = k;

			while (true)
			{
				if (sum > n) return null;
				if (sum == n) return ans;

				// here: sum < n

				var last = ans[lastX];

				// here: n-sum > 0 => n-sum+last > 0

				var next = Math.Min(lastY, n - sum + last);

				if (next < last) return null;

				sum = sum - last + next;
				ans[lastX] = next;

				if (sum == n) return ans;
				if (lastX == 0 || lastY == 0) return null;

				lastY--;
				lastX--;
			}
		}
	}
}
