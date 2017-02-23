using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// https://www.hackerrank.com/challenges/cross-matrix

namespace ConsoleApplication1.HackerRank
{
	class HackerRank48
	{
		public const ulong MOD = 1000000007ul;

		public void Go()
		{
			//Console.WriteLine(SolveBrute(new[] { "11111" }));
			//for (var i = 2; i <= 10; i++) Console.WriteLine(i.ToString() + " " + SolveBrute(new[] { new string('1', i) }));
			//for (var i = 2; i <= 10; i++) Console.WriteLine(i.ToString() + " " + Solve(new[] { new string('1', i) }));
			Compare();
		}

		public static void Compare()
		{
			var rnd = new Random(1337);
			for (var t = 0; t < 1000; t++)
			{
				var N = rnd.Next(2, 8);
				var board = new string[2];
				for (var i = 0; i < board.Length; i++)
					board[i] = RandomGenerator.RandomString(rnd, "01", N);
				var brute = SolveBrute(board);
				var actual = Solve(board);

				if (actual != brute)
				{
					Console.WriteLine(board.Join("\r\n"));
					Console.WriteLine(new { t, brute, actual });
					throw new InvalidOperationException();
				}
			}
		}

		public static ulong Solve(string[] board)
		{
			var N = (ulong)board[0].Length;

			var onerow = new ulong[N + 1];
			for (var n = 3ul; n <= N; n++)
				onerow[n] = (onerow[n - 1] + (n * (n - 1) * (n - 2) / 3)) % MOD;

			var C = new int[N];
			var S = new ulong[N, N];

			{
				var firstLine = board[0];
				for (var i = 0; i < firstLine.Length; i++)
					C[i] = firstLine[i] == '1' ? 1 : 0;

				var len = 0;
				for (var i = 0; i <= firstLine.Length; i++)
				{
					if (i == firstLine.Length || firstLine[i] == '0')
					{
						if (len >= 3)
							S[i - len, i - 1] = onerow[len];
						len = 0;
					}
					else
					{
						len++;
					}
				}
			}

			var count = 0ul;
			foreach (var sv in S)
				count = (count + sv) % MOD;

			for (var row = 1; row < board.Length; row++)
			{
				var line = board[row];

				for (var i = 0; i < line.Length; i++)
					C[i] = line[i] == '1' ? (C[i] + 1) : 0;

				var newS = new ulong[N, N];

				var rectanglesByLen = new List<int[]>[N + 1];

				rectanglesByLen[1] = new List<int[]>();
				for (var i = 0; i < line.Length; i++)
					rectanglesByLen[1].Add(new[] { i, i, C[i] });

				for (var len = 2; len <= line.Length; len++)
				{
					rectanglesByLen[len] = new List<int[]>();
					foreach (var prevLen in rectanglesByLen[len - 1])
					{
						var i = prevLen[0];
						var j = prevLen[1] + 1;
						var h = j < line.Length ? Math.Min(prevLen[2], C[j]) : 0;
						if (h > 0)
							rectanglesByLen[len].Add(new[] { i, j, h });
					}
				}

				// MAGIC

				var tmp = S;
				S = newS;
				newS = tmp;
			}

			return count;
		}

		public static void Example1()
		{
			Console.WriteLine(SolveBrute(@"0010
0001
1010
1110".SplitToLines()));
		}

		public static ulong SolveBrute(string[] board)
		{
			var N = board.Length;
			var M = board[0].Length;

			var count = 0ul;

			var rectangles = new List<int[]>();

			for (var x1 = 0; x1 < M; x1++)
			for (var x2 = x1; x2 < M; x2++)
			for (var y1 = 0; y1 < N; y1++)
			for (var y2 = y1; y2 < N; y2++)
			{
				if (x1 == x2 && y1 == y2) continue;

				if (IsFilledBy1(board, x1, x2, y1, y2))
					rectangles.Add(new[] { x1, x2, y1, y2 });
			}

			for (var ai = 0; ai < rectangles.Count; ai++)
				for (var bi = ai + 1; bi < rectangles.Count; bi++)
				{
					var A = rectangles[ai];
					var B = rectangles[bi];

					if (IsXOut(A, B) && IsXOut(B, A) && AreIntersected(A, B))
					{
					//	Console.WriteLine(A.Join() + " ~ " + B.Join());
						count = (count + 2) % MOD;
					}
				}

			return count;
		}

		public static bool IsFilledBy1(string[] board, int x1, int x2, int y1, int y2)
		{
			for (var x = x1; x <= x2; x++)
				for (var y = y1; y <= y2; y++)
					if (board[y][x] != '1')
						return false;
			return true;
		}

		public static bool IsXOut(int[] X, int[] Y)
		{
			for (var x = X[0]; x <= X[1]; x++)
				for (var y = X[2]; y <= X[3]; y++)
				{
					var isXinB = x >= Y[0] && x <= Y[1];
					var isYinB = y >= Y[2] && y <= Y[3];
					if (!isXinB || !isYinB) return true;
				}
			return false;
		}

		public static bool AreIntersected(int[] X, int[] Y)
		{
			for (var x = X[0]; x <= X[1]; x++)
				for (var y = X[2]; y <= X[3]; y++)
				{
					var isXinB = x >= Y[0] && x <= Y[1];
					var isYinB = y >= Y[2] && y <= Y[3];
					if (isXinB && isYinB) return true;
				}
			return false;
		}
	}
}
