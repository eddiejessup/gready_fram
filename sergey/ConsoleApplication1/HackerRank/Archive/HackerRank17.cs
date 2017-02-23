using ConsoleApplication1.DataTypes;
using ConsoleApplication1.Helpers;
using System;
using System.Linq;

namespace ConsoleApplication1.HackerRank
{
	public class HackerRank17
	{
		public void Go()
		{
			Solve(3, 3, 7);
		}

		public static void Solve(int rows, int cols, ulong R)
		{
			var matrix = Enumerable.Range(1, rows * cols).Select(i => (ulong)i).ByBatches(cols).Select(l => l.ToArray()).ToArray();

			foreach (var row in matrix)
				Console.WriteLine(row.Join());
			Console.WriteLine();

			var result = Solve(rows, cols, matrix, R);

			foreach (var row in result)
				Console.WriteLine(row.Join());
			Console.WriteLine();
		}

		public static ulong[][] Solve(int rows, int cols, ulong[][] matrix, ulong R)
		{
			var result = new ulong[rows][];
			for (var r = 0; r < rows; r++)
				result[r] = new ulong[cols];

			for (var loopIndex = 0; loopIndex < Math.Min((rows + 1) / 2, (cols + 1) / 2); loopIndex++)
			{
				var loopRows = rows - 2 * loopIndex;
				var loopCols = cols - 2 * loopIndex;

				var loopLength = LoopLength(loopRows, loopCols, loopIndex);

				var point0 = new PointInt(loopIndex, loopIndex);
				var r = (int)(R % (ulong)loopLength);

				for (var d = 0; d < loopLength; d++)
				{
					var pointFrom = point0 + GetLoopCoordinates(loopRows, loopCols, d);
					var a = Get(matrix, pointFrom);
					var pointTo = point0 + GetLoopCoordinates(loopRows, loopCols, (r + d) % loopLength);
					Set(result, pointTo, a);
				}
			}

			return result;
		}

		public static T Get<T>(T[][] arr, PointInt point)
		{
			return arr[point.Y][point.X];
		}

		public static void Set<T>(T[][] arr, PointInt point, T val)
		{
			arr[point.Y][point.X] = val;
		}

		public static PointInt GetLoopCoordinates(int rows, int cols, int d)
		{
			if (d <= rows - 1)
				return new PointInt(0, d);
			d -= rows - 1;
			if (d <= cols - 1)
				return new PointInt(d, rows - 1);
			d -= cols - 1;
			if (d <= rows - 1)
				return new PointInt(cols - 1, rows - 1 - d);
			d -= rows - 1;
			return new PointInt(cols - 1 - d, 0);
		}

		public static int LoopLength(int rows, int cols, int loopIndex)
		{
			return Math.Max(1, 2 * (rows - 1) + 2 * (cols - 1));
		}
	}
}
