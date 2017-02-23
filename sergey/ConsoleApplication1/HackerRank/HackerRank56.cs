using Combinatorics.Collections;
using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.HackerRank
{
	class HackerRank56
	{
		public void Go()
		{
			//new Queens().Solve(22);
			for (var n = 1; n <= 11; n++)
			{
				var sol = Solve(n);
				Console.WriteLine("------ " + new { n, kn = sol.Count });
				foreach (var triple in sol)
					Console.WriteLine(triple.Join());
			}
		}

		public static List<int[]> Solve(int n)
		{
			var d = (n - 1) / 3 + 1;

			var result = new List<int[]>();

			for (int c = n - d, r = d; c >= 0; c -= 2, r++)
				result.Add(new[] { n - r - c, r, c });

			for (int c = n - d - 1, r = 0; c >= 0; c -= 2, r++)
				result.Add(new[] { n - r - c, r, c });

			return result;
		}

		public static void ShowInfo()
		{
			for (var n = 1; n <= 10; n++)
			{
				Console.WriteLine("-------: " + n);
				foreach (var elem in SolveBrute(n))
					Console.WriteLine(elem.ToString());
			}
		}

		public static Tuple<int, int, int>[] SolveBrute(int n)
		{
			var triplets = GetTripleSums(n).ToArray();

			for (var len = triplets.Select(t => t.Item1).Distinct().Count(); len >= 2; len--)
			{
				foreach (var comb in new Combinations<Tuple<int, int, int>>(triplets, len))
				{
					if (comb.Select(t => t.Item1).Distinct().Count() == comb.Count &&
						comb.Select(t => t.Item2).Distinct().Count() == comb.Count &&
						comb.Select(t => t.Item3).Distinct().Count() == comb.Count)
					{
						return comb.ToArray();
					}
				}
			}

			return new[] { triplets[0] };
		}

		public static IEnumerable<Tuple<int, int, int>> GetTripleSums(int n)
		{
			for (var xi = 0; xi <= n; xi++)
				for (var yi = xi; yi <= n; yi++)
					if (xi + yi <= n)
					{
						var zi = n - xi - yi;
						if (zi >= yi)
						{
							yield return Tuple.Create(xi, yi, zi);
							yield return Tuple.Create(xi, zi, yi);
							yield return Tuple.Create(yi, xi, zi);
							yield return Tuple.Create(yi, zi, xi);
							yield return Tuple.Create(zi, xi, yi);
							yield return Tuple.Create(zi, yi, xi);
						}
					}
		}

		class Queens
		{
			int N;

			bool Allowed(bool[,] board, int x, int y)
			{
				for (int i = 0; i <= x; i++)
				{
					if (board[i, y] || (i <= y && board[x - i, y - i]) || (y + i < N && board[x - i, y + i]))
					{
						return false;
					}
				}
				return true;
			}

			bool FindSolution(bool[,] board, int x)
			{
				for (int y = 0; y < N; y++)
				{
					if (Allowed(board, x, y))
					{
						board[x, y] = true;
						if (x == N - 1 || FindSolution(board, x + 1))
						{
							return true;
						}
						board[x, y] = false;
					}
				}
				return false;
			}

			public void Solve(int N)
			{
				this.N = N;

				bool[,] board = new bool[N, N];

				if (FindSolution(board, 0))
				{
					for (int i = 0; i < N; i++)
					{
						for (int j = 0; j < N; j++)
						{
							Console.Write(board[i, j] ? "|Q" : "| ");
						}
						Console.WriteLine("|");
					}
				}
				else
				{
					Console.WriteLine("No solution found for n = " + N + ".");
				}

				Console.ReadKey(true);
			}
		}
	}
}
