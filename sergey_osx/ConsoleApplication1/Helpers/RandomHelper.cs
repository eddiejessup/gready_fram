using ConsoleApplication1.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication1.Helpers
{
	public static class RandomHelper
	{
		public static T Random<T>(this IList<T> arr, Random rnd)
		{
			if (arr.Count == 1)
				return arr[0];

			return arr[rnd.Next(arr.Count)];
		}

		public static T Random<T>(this T[,] arr, Random rnd)
		{
			return arr.Get(RandomIndex(arr, rnd));
		}

		public static T RandomElement<T>(this IEnumerable<T> source, Random rnd, bool throwExceptionIfEmpty = true)
		{
			var current = default(T);
			var count = 0;
			foreach (var element in source)
			{
				count++;
				if (rnd.Next(count) == 0)
					current = element;
			}
			
			if (count == 0 && throwExceptionIfEmpty)
				throw new InvalidOperationException("Sequence is empty");
			
			return current;
		}

		public static T RandomWeightedElement<T>(this KeyValuePair<T, double>[] itemsAndWeights, Random rnd)
		{
			var weightsSum = itemsAndWeights.Sum(kvp => kvp.Value);

			var p = rnd.NextDouble() * weightsSum;

			var last = default(T);
			foreach (var kvp in itemsAndWeights)
			{
				var w = kvp.Value;

				if (w > p)
					return kvp.Key;

				p -= w;

				last = kvp.Key;
			}
			return last;
		}

		public static PointInt RandomIndex<T>(this T[,] arr, Random rnd)
		{
			var x = arr.GetLength(0) == 1 ? 0 : rnd.Next(arr.GetLength(0));
			var y = arr.GetLength(1) == 1 ? 0 : rnd.Next(arr.GetLength(1));
			return new PointInt(x, y);
		}

		public static TKey RandomKey<TKey, TValue>(this IDictionary<TKey, TValue> dict, Random rnd)
		{
			if (dict.Count == 1)
				return dict.First().Key;

			return dict.Skip(rnd.Next(dict.Count)).First().Key;
		}

		public static T RandomGaussian<T>(this IList<T> arr, Random rnd)
		{
			if (arr.Count == 1)
				return arr[0];

			return arr[NextGaussian(rnd, 0, arr.Count - 1)];
		}

		public static T RandomGaussianMeanInZero<T>(this IList<T> arr, Random rnd)
		{
			return arr[IndexRandomGaussianMeanInZero(arr.Count, rnd)];
		}

		public static int IndexRandomGaussianMeanInZero(int count, Random rnd)
		{
			if (count == 1)
				return 0;

			var i = NextGaussian(rnd, -count, count - 1);

			if (i < 0) i = -i - 1;

			return i;
		}

		public static int RandomIndex<T>(this IList<T> arr, Random rnd)
		{
			if (arr.Count == 1)
				return 0;

			return rnd.Next(arr.Count);
		}

		public static int RandomIndex<T>(this ICollection<T> arr, Random rnd)
		{
			if (arr.Count == 1)
				return 0;

			return rnd.Next(arr.Count);
		}

		public static PointInt RandomPointInt(this Random rnd, int width, int height)
		{
			return new PointInt(
				rnd.Next(0, width),
				rnd.Next(0, height));
		}

		public static double BoxMuller(Random rnd)
		{
			double v1;
			double s;
			do
			{
				v1 = 2.0 * rnd.NextDouble() - 1.0;
				var v2 = 2.0 * rnd.NextDouble() - 1.0;
				s = v1 * v1 + v2 * v2;
			}
			while (s >= 1.0 || s < 0);

			s = Math.Sqrt((-2.0 * Math.Log(s)) / s);

			return v1 * s;
		}

		public static double BoxMuller(Random rnd, double mean, double standard_deviation)
		{
			return mean + BoxMuller(rnd) * standard_deviation;
		}

		public static int NextGaussian(Random rnd, int min, int max)
		{
			const double deviations = 2;
			int r;
			while ((r = (int)Math.Round(BoxMuller(rnd, min + (max - min) / 2.0, (max - min) / 2.0 / deviations))) > max || r < min)
			{
			}

			return r;
		}

		public static IList<T> Shuffle<T>(this IList<T> arr, Random rnd)
		{
			for (var i = 0; i < arr.Count; i++)
			{
				var j = rnd.Next(i, arr.Count);

				var tmp = arr[i];
				arr[i] = arr[j];
				arr[j] = tmp;
			}

			return arr;
		}
	}
}