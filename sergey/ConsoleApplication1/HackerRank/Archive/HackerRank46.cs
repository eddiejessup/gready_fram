using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApplication1.Helpers.ArrayHelper;

// https://www.hackerrank.com/contests/projecteuler/challenges/euler149

namespace ConsoleApplication1.HackerRank
{
	public class HackerRank46
	{
		public void Go()
		{
			Compare();
		}

		public static void Performance()
		{
			var rnd = new Random(1337);
			var N = 5000;
			var S = Create(N, _ => new long[N]);

			for (var i = 0; i < N; i++)
				for (var j = 0; j < N; j++)
					S[i][j] = rnd.Next(-10, 10);

			var timer = Stopwatch.StartNew();
			Console.WriteLine(Solve(S).Count());
			Console.WriteLine("Solve time: " + timer.ElapsedMilliseconds);
		}

		public static void Compare()
		{
			var rnd = new Random(1337);
			for (var t = 0; t < 10000; t++)
			{
				var N = rnd.Next(1, 7);
				var S = Create(N, _ => new long[N]);

				for (var i = 0; i < N; i++)
					for (var j = 0; j < N; j++)
						S[i][j] = rnd.Next(-10, 10);

				var brute = SolveBrute(S).ToArray();
				var actual = Solve(S).ToArray();

				if (!actual.SequenceEqual(brute))
				{
					Console.WriteLine(S.ToStringData());
					Console.WriteLine(new { t, brute = brute.Join(), actual = actual.Join() });
					throw new InvalidOperationException();
				}
			}
		}

		public static IEnumerable<long> SolveBrute(long[][] S)
		{
			var N = S.Length;
			var max = S[0][0];
			yield return max;
			var dirs = new[] { new[] { 1, 0 }, new[] { 0, 1 }, new[] { 1, 1 }, new[] { 1, -1 } };
			for (var i = 1; i < N; i++)
			{
				for (var r = 0; r <= i; r++)
					for (var c = 0; c <= i; c++)
						foreach (var dir in dirs)
						{
							var sum = 0L;
							var rr = r;
							var cc = c;
							while (true)
							{
								sum += S[rr][cc];
								max = Math.Max(max, sum);
								rr += dir[0];
								cc += dir[1];
								if (rr < 0 || cc < 0 || rr > i || cc > i)
									break;
							}
						}

				yield return max;
			}
		}

		public static IEnumerable<long> Solve(long N, long l, long[] A, long[] F, long m, long[] B, long[] G)
		{
			var count = N * N;

			var fsum = F.Sum();
			var gsum = G.Sum();

			var S = new long[N][];
			for (var i = 0; i < S.Length; i++)
				S[i] = new long[N];

			for (var i = 0; i < 5; i++)
				S[i / N][i % N] = A[F[i]] + B[G[i]];

			for (var i = 5; i < count; i++)
			{
				var fi = fsum % l;
				var gi = gsum % m;

				S[i / N][i % N] = A[fi] + B[gi];

				fsum += fi - F[i % 5];
				gsum += gi - G[i % 5];

				F[i % 5] = fi;
				G[i % 5] = gi;
			}

			return Solve(S);
		}

		public static IEnumerable<long> Solve(long[][] S)
		{
			var N = S.Length;

			// down right; right
			var dpCols = new[] { new long[N], new long[N] };
			var newDpCols = new[] { new long[N], new long[N] };
			dpCols[0][0] = S[0][0];
			dpCols[1][0] = S[0][0];

			// down right; down
			var dpRows = new[] { new long[N], new long[N] };
			var newDpRows = new[] { new long[N], new long[N] };
			dpRows[0][0] = S[0][0];
			dpRows[1][0] = S[0][0];

			var dpCorner = S[0][0];

			var antiDiagLeft = new long[N];
			var antiDiagRight = new long[N];
			var antiDiagSums = new long[N];
			antiDiagLeft[0] = S[0][0];
			antiDiagRight[0] = S[0][0];
			antiDiagSums[0] = S[0][0];

			var max = S[0][0];

			yield return S[0][0];

			for (var i = 1; i < N; i++)
			{
				// col:
				for (var r = 0; r < i; r++)
				{
					var val = S[r][i];
					for (var dir = 0; dir < dpCols.Length; dir++)
					{
						var cols = dpCols[dir];
						var newCols = newDpCols[dir];

						if (r == 0 && dir == 0)
							newCols[r] = val;

						else
						{
							var prevDp =
								r == i - 1 && dir == 2 ? S[r + 1][i - 1]
								: dir == 0 ? cols[r - 1]
								: cols[r];

							newCols[r] = Math.Max(val, val + prevDp);
						}

						max = Math.Max(max, newCols[r]);
					}
				}

				for (var dir = 0; dir < dpCols.Length; dir++)
				{
					var tmp = newDpCols[dir];
					newDpCols[dir] = dpCols[dir];
					dpCols[dir] = tmp;
				}

				// row:
				for (var c = 0; c < i; c++)
				{
					var val = S[i][c];
					for (var dir = 0; dir < dpRows.Length; dir++)
					{
						var rows = dpRows[dir];
						var newrows = newDpRows[dir];

						if (c == 0 && dir == 0)
							newrows[c] = val;

						else
						{
							var prevDp =
								c == i - 1 && dir == 2 ? S[i - 1][c + 1]
								: dir == 0 ? rows[c - 1]
								: rows[c];

							newrows[c] = Math.Max(val, val + prevDp);
						}

						max = Math.Max(max, newrows[c]);
					}
				}

				for (var dir = 0; dir < dpRows.Length; dir++)
				{
					var tmp = newDpRows[dir];
					newDpRows[dir] = dpRows[dir];
					dpRows[dir] = tmp;
				}

				// corner:
				{
					var val = S[i][i];
					dpCorner = Math.Max(val, dpCorner + val);
					max = Math.Max(max, dpCorner);
				}

				// in col:
				{
					var dpInCol = S[0][i];
					for (var j = 1; j <= i; j++)
					{
						var val = S[j][i];
						dpInCol = Math.Max(val, dpInCol + val);
						max = Math.Max(max, dpInCol);
					}
					dpRows[1][i] = dpInCol;
				}

				// in row:
				{
					var dpInRow = S[i][0];
					for (var j = 1; j <= i; j++)
					{
						var val = S[i][j];
						dpInRow = Math.Max(val, dpInRow + val);
						max = Math.Max(max, dpInRow);
					}
					dpCols[1][i] = dpInRow;
				}

				// anti-diag
				{
					for (var j = 0; j < i; j++)
					{
						antiDiagSums[j] = antiDiagSums[j + 1] + S[i][j] + S[j][i];
						max = Math.Max(max, antiDiagSums[j]);

						antiDiagLeft[j] = Math.Max(S[i][j], antiDiagLeft[j + 1] + S[i][j]);
						antiDiagLeft[j] = Math.Max(antiDiagLeft[j], antiDiagSums[j]);
						max = Math.Max(max, antiDiagLeft[j]);

						antiDiagRight[j] = Math.Max(S[j][i], antiDiagRight[j + 1] + S[j][i]);
						antiDiagRight[j] = Math.Max(antiDiagRight[j], antiDiagSums[j]);
						max = Math.Max(max, antiDiagRight[j]);
					}
					{
						var val = S[i][i];
						antiDiagLeft[i] = val;
						antiDiagRight[i] = val;
						antiDiagSums[i] = val;
						max = Math.Max(max, val);
					}
				}

				yield return max;
			}
		}

		public static void Example1()
		{
			foreach (var line in Solve(
				8, 
				4, 
				new long[] { 81, -89, 45, 6, }, 
				new long[] { 3, 2, 2, 1, 0 }, 
				3, 
				new long[] { -78, -45, 54 }, 
				new long[] { 1, 0, 0, 1, 2 }))
				Console.WriteLine(line);
		}
	}
}
