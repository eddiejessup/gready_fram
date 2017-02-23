using ConsoleApplication1.DataTypes;
using System;
using System.Collections.Generic;

namespace ConsoleApplication1.Helpers
{
	public static class PointIntHelper
	{
		public static IEnumerable<PointInt> ToPoints<T>(this T[,] array)
		{
			for (var x = 0; x < array.GetLength(0); x++)
				for (var y = 0; y < array.GetLength(1); y++)
					yield return new PointInt(x, y);
		}

		public static IEnumerable<PointInt> ToPoints<T>(this T[][] array)
		{
			for (var x = 0; x < array.Length; x++)
				for (var y = 0; y < array[x].Length; y++)
					yield return new PointInt(x, y);
		}

		public static T Get<T>(this T[][] arr, PointInt point)
		{
			return arr[point.X][point.Y];
		}

		public static T Get<T>(this T[,] arr, PointInt point)
		{
			return arr[point.X, point.Y];
		}

		public static T GetOrSet<T>(this T[,] arr, PointInt point, Func<T> createFunc) where T : class
		{
			var item = arr[point.X, point.Y];
			if (item == null)
			{
				item = createFunc();
				arr[point.X, point.Y] = item;
			}
			return item;
		}

		public static void Set<T>(this T[,] arr, PointInt point, T value)
		{
			arr[point.X, point.Y] = value;
		}

		public static T[,] SubArray<T>(this T[,] arr, PointInt topleft, PointInt bottomright)
		{
			// both points are inclusive
			var result = new T[bottomright.X - topleft.X + 1, bottomright.Y - topleft.Y + 1];
			for (var x = 0; x < result.GetLength(0); x++)
				for (var y = 0; y < result.GetLength(1); y++)
					result[x, y] = arr[x + topleft.X, y + topleft.Y];
			return result;
		}

		public static T[][,] SplitToFour<T>(this T[,] arr, PointInt splitPoint)
		{
			var x = splitPoint.X;
			var y = splitPoint.Y;

			var width = arr.GetLength(0);
			var height = arr.GetLength(1);

			var sliced1 = arr.SubArray(new PointInt(0, 0), new PointInt(x, y));
			var sliced2 = arr.SubArray(new PointInt(x + 1, 0), new PointInt(width - 1, y));
			var sliced3 = arr.SubArray(new PointInt(0, y + 1), new PointInt(x, height - 1));
			var sliced4 = arr.SubArray(new PointInt(x + 1, y + 1), new PointInt(width - 1, height - 1));

			return new[]
				{
					sliced1,
					sliced2,
					sliced3,
					sliced4
				};
		}

		public static Pair<PointInt, PointInt> GetPointsEnvelope(this IEnumerable<PointInt> points)
		{
			var minX = int.MaxValue;
			var maxX = int.MinValue;
			var minY = int.MaxValue;
			var maxY = int.MinValue;
			foreach (var point in points)
			{
				minX = Math.Min(minX, point.X);
				maxX = Math.Max(maxX, point.X);
				minY = Math.Min(minY, point.X);
				maxY = Math.Max(maxY, point.X);
			}

			return new Pair<PointInt, PointInt>(new PointInt(minX, minY), new PointInt(maxX - minX, maxY - minY));
		}
	}
}