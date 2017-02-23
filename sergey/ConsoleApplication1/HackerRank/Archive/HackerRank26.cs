using ConsoleApplication1.DataTypes;
using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication1.HackerRank
{
	class HackerRank26
	{
		public void Go()
		{
			Performance();
		}

		const ulong MOD = 1000000007ul;

		public static void Performance()
		{
			var state = CalcCache(5);
			var maxDir = (int)Math.Pow(8, 5);
			state.PrevCounts = new ulong[maxDir];
			state.Counts = new ulong[maxDir];

			var rnd = new Random(1337);

			for (var t = 0; t < 100; t++)
			{
				var board = new string[50];

				for (var i = 0; i < board.Length; i++)
					board[i] = RandomGenerator.RandomString(rnd, ".#", 5);

				var actual = Solve_2(state, board);
				Console.WriteLine(actual);
			}
		}

		public static void Compare()
		{
			var state = CalcCache(5);
			var maxDir = (int)Math.Pow(8, 5);
			state.PrevCounts = new ulong[maxDir];
			state.Counts = new ulong[maxDir];

			var rnd = new Random(1337);
			for (var t = 0; t < 100; t++)
			{
				var len = rnd.Next(1, 6);
				var board = new string[rnd.Next(1, 15)];

				for (var i = 0; i < board.Length; i++)
					board[i] = RandomGenerator.RandomString(rnd, ".#", len);

				var brute = Solve_Brute(board);
				var actual = Solve_2(state, board);

				if (actual != brute)
				{
					foreach (var line in board)
						Console.WriteLine(line);
					Console.WriteLine(new { t, brute, actual });
					throw new InvalidOperationException();
				}
			}
		}

		[Flags]
		public enum Direction : int
		{
			None = 0,
			Left = 1,
			Down = 2,
			Right = 4,

			LeftDown = 3,
			LeftRight = 5,
			DownRight = 6,

			LeftDownRight = 7
		}

		public class State
		{
			public List<int>[][] Cache;
			public int[][] PossibleOutputs;
			public ulong[] PrevCounts;
			public ulong[] Counts;
		}

		public static ulong Solve_2(string[] boardStrings)
		{
			var state = CalcCache(5);

			var maxDir = (int)Math.Pow(8, boardStrings[0].Length);
			state.PrevCounts = new ulong[maxDir];
			state.Counts = new ulong[maxDir];

			return Solve_2(CalcCache(5), boardStrings);
		}

		public static ulong Solve_2(State state, string[] boardStrings)
		{
			var maxDir = (int)Math.Pow(8, boardStrings[0].Length);

			var prevCounts = state.PrevCounts;
			var counts = state.Counts;
			var cache = state.Cache;

			for (var i = 0; i < maxDir; i++)
				prevCounts[i] = 1ul;

			for (var j = boardStrings.Length - 1; j >= 1; j--)
			{
				foreach (var i in state.PossibleOutputs[Encode(boardStrings[j - 1])])
				{
					counts[i] = CalcCount(cache[i], boardStrings[j], prevCounts);
				}

				var tmp = prevCounts;
				prevCounts = counts;
				counts = tmp;
			}

			var result = CalcCount(cache[0], boardStrings[0], prevCounts);
			return result == 0 ? 0 : (result - 1);
		}

		public static ulong CalcCount(List<int>[] cache, string line, ulong[] prevCounts)
		{
			var count = 0ul;

			var subCounts = cache[Encode(line)];
			foreach (var i in subCounts)
				count = (count + prevCounts[i]) % MOD;

			return count;
		}

		public static ulong Encode(string line)
		{
			var r = 0ul;
			var dMask = 5 - line.Length;
			for (var i = 0; i < dMask; i++)
				r += 1ul << i;
			for (var i = 0; i < line.Length; i++)
				if (line[i] == '#')
					r += 1ul << (dMask + i);
			return r;
		}

		public static char[] Decode(ulong line, int length)
		{
			var result = new char[length];
			for (var i = 0; i < result.Length; i++)
				result[i] = (line & (1ul << i)) > 0 ? '#' : '.';
			return result;
		}

		public static State CalcCache(int maxM)
		{
			var directions = EnumerateAllDirections(new Direction[maxM], 0).ToArray();
			var result = new List<int>[directions.Length][];

			var subRowsCache = new PointInt[1ul << maxM][];
			for (var line = 0ul; line < (ulong)subRowsCache.Length; line++)
				subRowsCache[line] = EnumerateSubRows(line, maxM).ToArray();

			var possibleOutputs = new HashSet<int>[subRowsCache.Length];
			for (var i = 0; i < possibleOutputs.Length; i++)
				possibleOutputs[i] = new HashSet<int>();

			for (var prevDirI = 0; prevDirI < directions.Length; prevDirI++)
			{
				var prevLineMask = directions[prevDirI];

				var subs = new List<int>[1ul << maxM];

				for (var line = 0ul; line < (ulong)subs.Length; line++)
				{
					var indexes = new List<int>();
					var lineStr = Decode(line, maxM);
					foreach (var queens in EnumerateQueens(prevLineMask, lineStr, subRowsCache[line], 0, 0))
					{
						var pow = 1;
						var dirI = 0;

						for (var i = lineStr.Length - 1; i >= 0; i--)
						{
							var nextLineDir = Direction.None;

							if (lineStr[i] != '#')
							{
								if (queens[i] == 'Q')
								{
									nextLineDir = Direction.LeftDownRight;
								}
								else
								{
									var hasQueenOnLeft = i > 0 && (prevLineMask[i - 1] & Direction.Right) != 0;
									var hasQueenOnUp = (prevLineMask[i] & Direction.Down) != 0;
									var hasQueenOnRight = i < prevLineMask.Length - 1 && (prevLineMask[i + 1] & Direction.Left) != 0;

									nextLineDir =
										(hasQueenOnLeft ? Direction.Right : Direction.None) |
										(hasQueenOnUp ? Direction.Down : Direction.None) |
										(hasQueenOnRight ? Direction.Left : Direction.None);
								}
							}

							dirI += pow * (int)nextLineDir;
							pow *= 8;
						}

						indexes.Add(dirI);
						possibleOutputs[line].Add(dirI);
					}

					subs[line] = indexes;
				}

				result[prevDirI] = subs;
			}

			return new State
			{
				Cache = result,
				PossibleOutputs = possibleOutputs.Select(h => h.ToArray()).ToArray(),
			};
		}

		public static IEnumerable<PointInt> EnumerateSubRows(ulong line, int length)
		{
			var start = (line & 1) > 0 ? -1 : 0;
			for (var i = 1; i < length; i++)
			{
				if ((line & (1ul << i)) > 0)
				{
					if (start >= 0) yield return new PointInt(start, i - 1);
					start = -1;
				}
				else if (start == -1)
					start = i;
			}
			if (start >= 0) yield return new PointInt(start, length - 1);
		}

		public static IEnumerable<Direction[]> EnumerateAllDirections(Direction[] arr, int i)
		{
			for (var a = 0; a <= 7; a++)
			{
				var result = new Direction[arr.Length];
				Array.Copy(arr, result, arr.Length);
				result[i] = (Direction)a;

				if (i == arr.Length - 1)
					yield return result;
				else
					foreach (var sub in EnumerateAllDirections(result, i + 1))
						yield return sub;
			}
		}

		public static ulong Solve(string[] boardStrings)
		{
			var board = boardStrings.Select(s => s.ToCharArray()).ToArray();

			var allSubRows = board.Select(s => EnumerateSubRows(s).ToArray()).ToArray();

			var count = Solve(board, allSubRows, 0, new Direction[board[0].Length]);

			return count == 0 ? 0 : (count - 1);
		}

		public static ulong Solve(char[][] board, PointInt[][] allSubRows, int lineIndex, Direction[] prevLineMask)
		{
			var count = 0ul;

			var line = board[lineIndex];

			var subRows = allSubRows[lineIndex];

			var isLast = lineIndex == board.Length - 1;

			foreach (var queens in EnumerateQueens(prevLineMask, line, subRows, 0, 0))
			{
		//		Console.WriteLine($"{lineIndex}: " + queens.Join("") + "    " + count);

				if (isLast) count = (count + 1) % MOD;
				else
				{
					var nextLineMask = new Direction[prevLineMask.Length];

					for (var i = 0; i < nextLineMask.Length; i++)
					{
						if (line[i] == '#') continue;

						if (queens[i] == 'Q')
						{
							nextLineMask[i] = Direction.LeftDownRight;
							continue;
						}

						var hasQueenOnLeft = i > 0 && (prevLineMask[i - 1] & Direction.Right) != 0;
						var hasQueenOnUp = (prevLineMask[i] & Direction.Down) != 0;
						var hasQueenOnRight = i < line.Length - 1 && (prevLineMask[i + 1] & Direction.Left) != 0;

						nextLineMask[i] = 
							(hasQueenOnLeft ? Direction.Right : Direction.None) |
							(hasQueenOnUp ? Direction.Down : Direction.None) |
							(hasQueenOnRight ? Direction.Left : Direction.None);
					}

					var subCount = Solve(board, allSubRows, lineIndex + 1, nextLineMask);

					count = (count + subCount) % MOD;
				}
			}

			return count;
		}

		public static IEnumerable<char[]> EnumerateQueens(Direction[] prevLineMask, char[] line, PointInt[] currSubRows, int subRowIndex, int dMask)
		{
			if (currSubRows.Length == 0)
			{
				yield return line;
				yield break;
			}

			if (subRowIndex < currSubRows.Length)
			{
				var isLast = subRowIndex == currSubRows.Length - 1;

				if (isLast)
					yield return line;
				else
					foreach (var next in EnumerateQueens(prevLineMask, line, currSubRows, subRowIndex + 1, dMask))
						yield return next;

				var sub = currSubRows[subRowIndex];

				for (var i = sub.X; i <= sub.Y; i++)
				{
					var hasQueen = (prevLineMask[i + dMask] & Direction.Down) != 0
						|| (i > 0 && (prevLineMask[i + dMask - 1] & Direction.Right) != 0)
						|| (i < line.Length - 1 && (prevLineMask[i + dMask + 1] & Direction.Left) != 0);

					if (hasQueen) continue;

					line[i] = 'Q';
					if (isLast)
						yield return line;
					else
						foreach (var next in EnumerateQueens(prevLineMask, line, currSubRows, subRowIndex + 1, dMask))
							yield return next;
					line[i] = '.';
				}
			}
		}

		public static IEnumerable<PointInt> EnumerateSubRows(char[] line)
		{
			var start = line[0] == '#' ? -1 : 0;
			for (var i = 1; i < line.Length; i++)
			{
				if (line[i] == '#')
				{
					if (start >= 0) yield return new PointInt(start, i - 1);
					start = -1;
				}
				else if (start == -1)
					start = i;
			}
			if (start >= 0) yield return new PointInt(start, line.Length - 1);
		}

		public static ulong Solve_Brute(string[] boardStrings)
		{
			var board = boardStrings.Select(s => s.ToCharArray()).ToArray();

			var count = Solve_Brute(board, 0);

			return count == 0 ? 0 : (count - 1);
		}

		public static ulong Solve_Brute(char[][] board, int lineI)
		{
			if (lineI >= board.Length) return 0ul;

			var line = board[lineI];

			var count = lineI == board.Length - 1 ? 1ul : Solve_Brute(board, lineI + 1);

			count += Solve_Brute_Row(board, lineI, 0);

			return count;
		}

		private static ulong Solve_Brute_Row(char[][] board, int lineI, int lineJ)
		{
			var line = board[lineI];

			var count = 0ul;

			for (var j = lineJ; j < line.Length; j++)
				if (line[j] == '.' && Check(board, lineI, j))
				{
					line[j] = 'Q';

					if (lineI == board.Length - 1)
					{
						count++;
					}
					else
					{
						count += Solve_Brute(board, lineI + 1);
					}

					count += Solve_Brute_Row(board, lineI, j + 1);

					line[j] = '.';
				}

			return count;
		}

		private static bool Check(char[][] board, int lineI, int lineJ)
		{
			// left
			for (var j = lineJ - 1; j >= 0; j--)
				if (board[lineI][j] == 'Q')
					return false;
				else if (board[lineI][j] == '#')
					break;

			// up
			for (var i = lineI - 1; i >= 0; i--)
				if (board[i][lineJ] == 'Q')
					return false;
				else if (board[i][lineJ] == '#')
					break;

			// left-up
			for (var i = lineI - 1; i >= 0; i--)
			{
				var di = lineI - i;
				if (!board.AreIndicesAllowed(i, lineJ - di))
					break;

				if (board[i][lineJ - di] == 'Q')
					return false;
				else if (board[i][lineJ - di] == '#')
					break;
			}

			// right-up
			for (var i = lineI - 1; i >= 0; i--)
			{
				var di = lineI - i;
				if (!board.AreIndicesAllowed(i, lineJ + di))
					break;

				if (board[i][lineJ + di] == 'Q')
					return false;
				else if (board[i][lineJ + di] == '#')
					break;
			}

			return true;
		}

		public void Example1()
		{
			Console.WriteLine(Solve_2(new[]
			{
				"...",
				"...",
				"...",
			}));
		}

		public void Example2()
		{
			Console.WriteLine(Solve_2(new[] 
			{
				".#.",
				".#.",
				"...",
			}));
		}

		public void Example3()
		{
			Console.WriteLine(Solve_2( new[]
			{
				".#..",
				"....",
			}));
		}

		public void Example4()
		{
			Console.WriteLine(Solve_2(new[]
			{
				"#",
			}));
		}

		public void Example5()
		{
			Console.WriteLine(Solve_2(new[]
			{
				"..#",
				"#..",
				"...",
			}));
		}

		public void Example6()
		{
			Console.WriteLine(Solve_2(new[]
			{
				"..###",
				"...#.",
				"###.#",
				"#...#",
				".#...",
				".....",
				"..#..",
				"#.#..",
				"##.#.",
				"..###",
				".#.#.",
				"#.#..",
				"..###",
				".#.##",
				"###.#",
				".....",
				"##..#",
				"..#.#",
				".....",
				"##..#",
				".##..",
				".####",
				"###..",
				".##.#",
				".#.#.",
				"##..#",
				"#.###",
				"...##",
				".##..",
				"####.",
				"#...#",
				"#.###",
				"##.##",
				"###.#",
				"####.",
				".##..",
				"..#.#",
			}));
		}
	}
}
