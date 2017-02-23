using Combinatorics.Collections;
using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.HackerRank
{
	class HackerRank54
	{
		public void Go()
		{
			Console.WriteLine(Solve("daBBC", "ABC"));
			Console.WriteLine(Solve("daBC", "ABC"));
			Console.WriteLine(Solve("AbaC", "A"));
			//Compare();
		}

		public static void Compare()
		{
			var rnd = new Random(1337);
			for (var t = 0; t < 100000; t++)
			{
				var len = rnd.Next(1, 7);
				var a = RandomGenerator.RandomString(rnd, "abcABC", len);
				var b = RandomGenerator.RandomString(rnd, "ABC", rnd.Next(1, len + 1));
				var brute = SolveBrute(a, b);
				var actual = Solve(a, b);

				if (actual != brute)
				{
					Console.WriteLine(new { t, a, b, brute, actual });
					throw new InvalidOperationException();
				}
			}
		}

		public static bool Solve(string a, string b)
		{
			if (a == b || a.ToUpper() == b) return true;

			var dp = new bool[a.Length + 1, b.Length + 1];

			for (var j = 1; j <= b.Length; j++)
				dp[0, j] = false;

			var aHasUpper = false;
			for (var i = 1; i <= a.Length; i++)
			{
				aHasUpper |= char.IsUpper(a[i - 1]);
				dp[i, 0] = !aHasUpper;
			}

			dp[0, 0] = true;

			for (var i = 1; i <= a.Length; i++)
			{
				for (var j = 1; j <= b.Length; j++)
				{
					var eq = char.ToUpper(a[i - 1]) == b[j - 1];

					var isAup = char.IsUpper(a[i - 1]);

					if (eq && isAup)
						dp[i, j] = dp[i - 1, j - 1];

					else if (eq)
						dp[i, j] = dp[i - 1, j] || dp[i - 1, j - 1];

					else if (isAup)
						dp[i, j] = false;

					else
						dp[i, j] = dp[i - 1, j];
				}
			}

			return dp[a.Length, b.Length];
		}

		public static bool SolveBrute(string a, string b)
		{
			if (a == b || a.ToUpper() == b || (a.Where(char.IsUpper).Join("") == b)) return true;

			var aarr = a.ToCharArray();

			var indices = aarr.Select((c, i) => char.IsLower(c) ? i : -1).Where(i => i >= 0).ToArray();

			for (var len = 1; len <= a.Length; len++)
			{
				foreach (var sel in new Combinations<int>(indices, len))
				{
					var result = new List<char>();
					for (var i = 0; i < aarr.Length; i++)
						if (char.IsUpper(aarr[i]) || sel.Contains(i))
							result.Add(char.ToUpper(aarr[i]));
					if (result.Join("") == b)
						return true;
				}
			}

			return false;
		}
	}
}
