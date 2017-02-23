using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// https://www.hackerrank.com/contests/projecteuler/challenges/euler150

namespace ConsoleApplication1.HackerRank
{
	class HackerRank47
	{
		public void Go()
		{
			Performance();
		}

		public static void Performance()
		{
			var rnd = new Random(1337);
			var N = 350;
			var triangle = new long[N][];
			for (var row = 0; row < N; row++)
				triangle[row] = RandomGenerator.RandomLongArray(rnd, row + 1, -20, 20);
			Console.WriteLine(Solve(triangle).Take(10).Join());
		}

		public static void Example1()
		{
			foreach (var sum in Solve(@"15
-14 -7
20 -13 -5
-3 8 23 -26
1 -4 -5 -18 5
-16 31 2 9 28 3".SplitToLines().Select(s => s.Split().Select(long.Parse).ToArray()).ToArray()))
				Console.WriteLine(sum);
		}

		private static List<long> Solve(long[][] triangle)
		{
			var N = triangle.Length;

			var result = new List<long>();

			var sums = new long[N][][];
			for (var row = 0; row < N; row++)
			{
				sums[row] = new long[row + 1][];
				for (var col = 0; col <= row; col++)
					sums[row][col] = new long[N - row + 1];
			}

			for (var col = 0; col < N; col++)
			{
				var val = triangle[N - 1][col];
				sums[N - 1][col][0] = val;
				result.Add(val);
			}

			for (var row = N - 2; row >= 0; row--)
			{
				for (var col = 0; col <= row; col++)
				{
					var vert = triangle[row][col];
					sums[row][col][0] = vert;
					result.Add(vert);
	
					for (var height = 1; height < N - row; height++)
					{
						vert += triangle[row + height][col];
						var sum = vert + sums[row + 1][col + 1][height - 1];
						sums[row][col][height] = sum;
						result.Add(sum);
					}
				}
			}

			result.Sort();

			return result;
		}
	}
}
