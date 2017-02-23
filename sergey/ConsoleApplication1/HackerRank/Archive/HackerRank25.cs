using ConsoleApplication1.DataTypes;
using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.HackerRank
{
	public class HackerRank25
	{
		public void Go()
		{
			Compare();
		}

		public void Performance()
		{
			var rnd = new Random(1337);
			var N = 300000;
			var S = 2000;

			var d = RandomGenerator.RandomIntArray(rnd, S, 0, N - 1).OrderByDescending(i => i).ToArray();
			var a = new int[S];
			var b = new int[S];
			for (var i = 0; i < S; i++)
			{
				var di = d[i];
				var aprev = i == 0 ? 1 : a[i - 1];
				var bprev = i == 0 ? 1 : b[i - 1];
				var dprev = i == 0 ? (N - 1) : d[i - 1];

				a[i] = rnd.Next(aprev, aprev + dprev - di + 1);
				b[i] = rnd.Next(bprev, bprev + dprev - di + 1);
			}

			var commands = new int[S][];
			for (var i = 0; i < S; i++)
				commands[i] = new[] { a[i], b[i], d[i] };

			var K = RandomGenerator.RandomLongArray(rnd, S, 0, N * N - 1).ToArray();

			var timer = Stopwatch.StartNew();

			var points = Solve(N, commands, K);

			Console.WriteLine(string.Join(Environment.NewLine, points.Select(p => $"{p.X} {p.Y}")));

			Console.WriteLine("Solve time: " + timer.ElapsedMilliseconds);
		}

		public void Example1()
		{
			Console.WriteLine(Solve(7, new[]
			{
				new[] { 1, 2, 4 },
				new[] { 2, 3, 3 },
				new[] { 3, 4, 1 },
				new[] { 3, 4, 0 },
			}, new[] {0L, 6, 9, 11, 24, 25, 48}).Join());
		}

		public void Example2()
		{
			Console.WriteLine(Solve_Brute(7, new[]
			{
				new[] { 7, 1, 0 },
			}, new[] { 0L, 6, 9, 11, 24, 25, 48 }).Join());
		}

		public void Compare()
		{
			var rnd = new Random(1337);
			for (var t = 0; t < 10000; t++)
			{
				var N = rnd.Next(1, 10);
				var S = rnd.Next(1, 50);

				var d = RandomGenerator.RandomIntArray(rnd, S, 0, N - 1).OrderByDescending(i => i).ToArray();
				var a = new int[S];
				var b = new int[S];
				for (var i = 0; i < S; i++)
				{
					var di = d[i];
					var aprev = i == 0 ? 1 : a[i - 1];
					var bprev = i == 0 ? 1 : b[i - 1];
					var dprev = i == 0 ? (N - 1) : d[i - 1];

					a[i] = rnd.Next(aprev, aprev + dprev - di + 1);
					b[i] = rnd.Next(bprev, bprev + dprev - di + 1);
				}

				var commands = new int[S][];
				for (var i = 0; i < S; i++)
					commands[i] = new[] { a[i], b[i], d[i] };

				for (var i = 1; i < S; i++)
				{
					var isOk = a[i - 1] <= a[i] && a[i] + d[i] <= a[i - 1] + d[i - 1]
						&& b[i - 1] <= b[i] && b[i] + d[i] <= b[i - 1] + d[i - 1];
					if (!isOk)
						throw new InvalidOperationException("Generation error");
				}

				var K = RandomGenerator.RandomLongArray(rnd, 100, 0, N * N - 1).ToArray();

				var brute = Solve_Brute(N, commands, K);
				var actual = Solve(N, commands, K);

				if (!actual.SequenceEqual(brute))
				{
					Console.WriteLine(new { t, N, S });
					Console.WriteLine("Commands:");
					foreach (var cmd in commands)
						Console.WriteLine(cmd.Join());
					Console.WriteLine("K:");
					Console.WriteLine(K.Join());
					Console.WriteLine("Expected:");
					Console.WriteLine(brute.Join());
					Console.WriteLine("Actual:");
					Console.WriteLine(actual.Join());
					throw new InvalidOperationException();
				}
			}
		}

		public class SquareInfo
		{
			public long Row;
			public long Col;
			public long Width;
			public long First;
			public long DeltaInRow;
			public long DeltaInCol;

			public long Direction;

			public long MinRem;
			public long MinDiv;
			public long MaxRem;
			public long MaxDiv;
			public long Min;
			public long Max;

			public void CalcExtra(int N)
			{
				Min = Direction == 0 ? First
					: Direction == 1 ? (First + (Width - 1) * DeltaInRow)
					: Direction == 2 ? (First + (Width - 1) * DeltaInRow + (Width - 1) * DeltaInCol)
					: (First + (Width - 1) * DeltaInCol);

				Max = Direction == 2 ? First
					: Direction == 3 ? (First + (Width - 1) * DeltaInRow)
					: Direction == 0 ? (First + (Width - 1) * DeltaInRow + (Width - 1) * DeltaInCol)
					: (First + (Width - 1) * DeltaInCol);

				MinRem = Min % N;
				MinDiv = Min / N;
				MaxRem = Max % N;
				MaxDiv = Max / N;
			}
		}

		public static PointInt[] Solve(int N, int[][] commands, long[] K)
		{
			var squares = new SquareInfo[commands.Length + 1];
			squares[0] = new SquareInfo { Row = 0, Col = 0, Width = N, First = 0, DeltaInRow = 1, DeltaInCol = N, Direction = 0 };
			squares[0].CalcExtra(N);
			for (var i = 1; i <= commands.Length; i++)
			{
				var command = commands[i - 1];
				var ai = command[0];
				var bi = command[1];
				var di = command[2];

				var prev = squares[i - 1];

				var newRow = ai - 1;
				var newCol = bi - 1;

				var square = new SquareInfo
				{
					Row = newRow,
					Col = newCol,
					Width = di + 1,
					First = prev.First + (newCol - prev.Col) * prev.DeltaInRow + (newRow - prev.Row + di) * prev.DeltaInCol,
					DeltaInRow = -prev.DeltaInCol,
					DeltaInCol = prev.DeltaInRow,
					Direction = i % 4,
				};
				square.CalcExtra(N);
				squares[i] = square;
			}

			var result = new PointInt[K.Length];
			for (var i = 0; i < K.Length; i++)
			{
				var wi = K[i];

				var si = BinarySearchHelper.BinarySearchRightBiased(squares, sq =>
				{
					var rem = wi % N;
					var div = wi / N;

					var contains = sq.MinRem <= rem && rem <= sq.MaxRem && sq.MinDiv <= div && div <= sq.MaxDiv;

					return contains ? 0 : 1;
				});

				result[i] = GetPoint(squares[si], wi, N);
			}
			return result;
		}

		public static PointInt GetPoint(SquareInfo square, long w, int N)
		{
			var ddiv = (w / N) - square.MinDiv;
			var drem = (w % N) - square.MinRem;

			if (square.Direction == 0)
				return new PointInt((int)(square.Row + ddiv + 1), (int)(square.Col + drem + 1));

			if (square.Direction == 1)
				return new PointInt((int)(square.Row + drem + 1), (int)(square.Col + square.Width - ddiv));

			if (square.Direction == 2)
				return new PointInt((int)(square.Row + square.Width - ddiv), (int)(square.Col + square.Width - drem));

			if (square.Direction == 3)
				return new PointInt((int)(square.Row + square.Width - drem), (int)(square.Col + ddiv + 1));

			return PointInt.MinusOne;
		}

		public static PointInt[] Solve_SL(int N, int[][] commands, long[] K)
		{
			var result = new PointInt[K.Length];

			for (var ki = 0; ki < K.Length; ki++)
			{
				var wi = K[ki];
				var point = new PointInt((int)(wi / N) + 1, (int)(wi % N) + 1);

				foreach (var command in commands)
				{
					var ai = command[0];
					var bi = command[1];
					var di = command[2];

					if (ai > point.X || bi > point.Y || di == 0) break;

					var dx = point.X - ai;
					var dy = point.Y - bi;

					if (dx > di || dy > di) break;

					point = new PointInt(ai + dy, bi + di - dx);
				}

				result[ki] = point;
			}

			return result;
		}

		public static PointInt[] Solve_Brute(int N, int[][] commands, long[] K)
		{
			var board = new long[N][];
			for (var i = 0; i < N; i++)
			{
				var line = new long[N];
				for (var j = 0; j < N; j++)
					line[j] = i * N + j;
				board[i] = line;
			}

			foreach (var command in commands)
			{
				var ai = command[0];
				var bi = command[1];
				var di = command[2];

				if (di == 0) continue;

				var rows = new List<long[]>();
				for (var i = ai - 1; i <= ai + di - 1; i++)
					rows.Add(board[i].Slice(bi - 1, bi + di - 1));

				for (var rowI = 0; rowI < rows.Count; rowI++)
					for (var rowJ = 0; rowJ <= di; rowJ++)
						board[ai + rowJ - 1][bi + di - rowI - 1] = rows[rowI][rowJ];
			}

			var result = new PointInt[K.Length];
			for (var i = 0; i < result.Length; i++)
			{
				var wi = K[i];
				var point = board.ToPoints().First(p => board[p.X][p.Y] == wi);
				result[i] = new PointInt(point.X + 1, point.Y + 1);
			}
			return result;
		}
	}
}
