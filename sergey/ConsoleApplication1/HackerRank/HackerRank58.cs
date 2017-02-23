using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.HackerRank
{
	class HackerRank58
	{
		public void Go()
		{
			//foreach (var ans in SolveBrute(3, 3, 3, 3)) Console.WriteLine(ans);
			Compare();
		}

		public static void Compare()
		{
			var rnd = new Random(1337);
			for (var t = 0; t < 10000; t++)
			{
				var arr = RandomGenerator.RandomIntArray(rnd, 4, 1, 10);
				var brute = SolveBrute2(arr[0], arr[1], arr[2], arr[3]);
				var actual = Solve(arr[0], arr[1], arr[2], arr[3]);

				if (actual != brute)
				{
					Console.WriteLine(arr.Join());
					Console.WriteLine(new { t, brute, actual });
					throw new InvalidOperationException();
				}
			}
		}

		public static ulong Solve(int A, int B, int C, int D)
		{
			return 42; 
		}

		public static ulong SolveBrute2(int A, int B, int C, int D)
		{
			var cnt = 0ul;

			var arr = new[] { A, B, C, D }.OrderBy(i => i).ToArray();

			for (var a = 1; a <= arr[0]; a++)
				for (var b = a; b <= arr[1]; b++)
					for (var c = b; c <= arr[2]; c++)
						for (var d = c; d <= arr[3]; d++)
							if ((a ^ b ^ c ^ d) != 0)
								cnt++;

			return cnt;
		}

		public static HashSet<string> SolveBrute(int A, int B, int C, int D)
		{
			var answers = new HashSet<string>();

			for (var a = 1; a <= A; a++)
				for (var b = 1; b <= B; b++)
					for (var c = 1; c <= C; c++)
						for (var d = 1; d <= D; d++)
							if ((a ^ b ^ c ^ d) != 0)
								answers.Add(new[] { a, b, c, d }.OrderBy(i => i).Join(" "));

			return answers;
		}
	}
}
