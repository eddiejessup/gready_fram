using ConsoleApplication1.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication1.Helpers
{
	public static class ArrayHelper
	{
		public static T[] Slice<T>(this T[] arr, int iInclusive, int jInclusive)
		{
			var result = new T[jInclusive - iInclusive + 1];
			Array.Copy(arr, iInclusive, result, 0, result.Length);
			return result;
		}

		public static TResult[] Merge<T, TResult>(this T[] source, T[] other, Func<T, T, TResult> func)
		{
			var result = new TResult[source.Length];
			for (var i = 0; i < result.Length; i++)
				result[i] = func(source[i], other[i]);
			return result;
		}

		public static T[] CreateWithConcat<T>(this T[] source, T[] concatThis)
		{
			var result = new T[source.Length + concatThis.Length];
			Array.Copy(source, 0, result, 0, source.Length);
			Array.Copy(concatThis, 0, result, source.Length, concatThis.Length);
			return result;
		}

		public static T[,] ShallowClone<T>(this T[,] array)
		{
			return (T[,]) array.Clone();
		}

		public static T[] ShallowClone<T>(this T[] array)
		{
			return (T[]) array.Clone();
		}

		public static string ToStringData<T>(this T[][] jaggedArray, string separator = " ")
		{
			return string.Join(
				Environment.NewLine,
				jaggedArray.Select(a => string.Join(separator, a)));
		}

		public static string ToStringData<T>(this T[,] array)
		{
			return string.Join(
				Environment.NewLine,
				array.EnumerateRows().Select(a => string.Join(", ", a)));
		}

		public static void Foreach<T>(this T[,] array, Action<int, int> actXY)
		{
			for (var x = 0; x < array.GetLength(0); x++)
				for (var y = 0; y < array.GetLength(1); y++)
					actXY(x, y);
		}

		public static IEnumerable<T> GetRow<T>(this T[,] array, int i)
		{
			return Enumerable.Range(0, array.GetLength(1)).Select(j => array[i, j]);
		}

		public static IEnumerable<T> GetColumn<T>(this T[,] array, int j)
		{
			return Enumerable.Range(0, array.GetLength(0)).Select(i => array[i, j]);
		}

		public static IEnumerable<IEnumerable<T>> EnumerateRows<T>(this T[,] arr)
		{
			for (var i = 0; i < arr.GetLength(0); i++)
			{
				var row = i;
				yield return Enumerable.Range(0, arr.GetLength(1)).Select(j => arr[row, j]);
			}
		}

		public static IEnumerable<T> EnumerateFlat<T>(this T[,] array)
		{
			for (var x = 0; x < array.GetLength(0); x++)
				for (var y = 0; y < array.GetLength(1); y++)
					yield return array[x, y];
		}

		public static T[,] ToTwoDimesional<T>(this T[][] jaggedArray)
		{
			var ylen = jaggedArray[0].Length;

			var result = new T[jaggedArray.Length, ylen];

			for (var x = 0; x < jaggedArray.Length; x++)
				for (var y = 0; y < ylen; y++)
					result[x, y] = jaggedArray[x][y];

			return result;
		}

		public static void Print<T>(this T[,] array, string format = null)
		{
			foreach (var row in array.EnumerateRows())
				Console.WriteLine(", ".Join(row.Select(i => format == null ? i.ToString() : (string.Format(format, i)))));
			Console.WriteLine();
		}

		public static bool AreIndicesAllowed<T>(this T[,] arr, PointInt point)
		{
			return AreIndicesAllowed(arr, point.X, point.Y);
		}

		public static bool AreIndicesAllowed<T>(this T[,] arr, int i, int j)
		{
			if (i < 0 || j < 0) return false;
			if (i >= arr.GetLength(0) || j >= arr.GetLength(1)) return false;
			return true;
		}

		public static bool AreIndicesAllowed<T>(this T[][] arr, int i, int j)
		{
			if (i < 0 || j < 0) return false;
			if (i >= arr.Length || j >= arr[i].Length) return false;
			return true;
		}

		public static int FirstIndexOf<T>(this IEnumerable<T> seq, Func<T, bool> where)
		{
			var i = 0;
			foreach (var item in seq)
			{
				if (where(item))
					return i;
				i++;
			}
			return -1;
		}

		public static int FirstIndexOf<T>(this IEnumerable<T> seq, Func<int, T, bool> where)
		{
			var i = 0;
			foreach (var item in seq)
			{
				if (where(i, item))
					return i;
				i++;
			}
			return -1;
		}

		public static T[,] Transpose<T>(this T[,] arr)
		{
			var result = new T[arr.GetLength(1),arr.GetLength(0)];

			for (var x = 0; x < arr.GetLength(0); x++)
				for (var y = 0; y < arr.GetLength(1); y++)
					result[y, x] = arr[x, y];

			return result;
		}

		public static T[][] Transpose<T>(this T[][] arr)
		{
			var m = arr[0].Length;

			var result = new T[m][];

			for (var x = 0; x < m; x++)
			{
				result[x] = new T[arr.Length];
				for (var y = 0; y < arr.Length; y++)
					result[x][y] = arr[y][x];
			}

			return result;
		}

		public static T[] ConcatElementToArray<T>(this T[] array, T element)
		{
			var result = new T[array.Length + 1];
			Array.Copy(array, result, array.Length);
			result[result.Length - 1] = element;
			return result;
		}

		public static bool AreEqual(uint[,] data, uint[,] another)
		{
			if (data.GetLength(0) != another.GetLength(0) || data.GetLength(1) != another.GetLength(1))
				return false;

			for (var x = 0; x < data.GetLength(0); x++)
				for (var y = 0; y < data.GetLength(1); y++)
					if (data[x, y] != another[x, y])
						return false;

			return true;
		}

		public static ulong GetHashForUintsArray(this uint[,] array)
		{
			var hash = (ulong)array.Length;

			for (var x = 0; x < array.GetLength(0); x++)
				for (var y = 0; y < array.GetLength(1); y++)
					hash = unchecked(hash * 314159ul + array[x, y]);

			return hash;
		}

		// TODO: Use Uwi's solution in HackerRank39

		public static ulong[] MatrixMul(this ulong[,] arr, ulong[] x, ulong mod)
		{
			var result = new ulong[x.Length];
			for (var i = 0; i < arr.GetLength(0); i++)
			{
				var s = 0ul;
				for (var j = 0; j < arr.GetLength(0); j++)
					s = (s + (arr[i, j] * x[j] % mod)) % mod;
				result[i] = s;
			}
			return result;
		}

		public static ulong[,] MatrixMul(this ulong[,] arr, ulong[,] x, ulong mod)
		{
			var a0 = arr.GetLength(0);
			var a1 = arr.GetLength(1);
			var b0 = x.GetLength(0);
			var b1 = x.GetLength(1);

			if (a1 != b0) throw new InvalidOperationException();

			var result = new ulong[a0, b1];
			for (var i = 0; i < a0; i++)
				for (var j = 0; j < b1; j++)
				{
					var s = 0ul;
					for (var k = 0; k < a1; k++)
						s = (s + (arr[i, k] * x[k, j] % mod)) % mod;
					result[i, j] = s;
				}
			return result;
		}

		public static ulong[,] MatrixPow(this ulong[,] arr, ulong pow, ulong mod)
		{
			if (pow == 0) return Identity(arr.GetLength(0));
			if (pow == 1) return (ulong[,])arr.Clone();
			if (pow % 2 == 0) return MatrixPow(MatrixMul(arr, arr, mod), pow / 2, mod);
			return MatrixMul(MatrixPow(arr, pow - 1, mod), arr, mod);
		}

		public static ulong[,] Identity(int size)
		{
			var result = new ulong[size, size];
			for (var i = 0; i < size; i++)
				result[i, i] = 1;
			return result;
		}

		public static TOut[,] Map<T, TOut>(this T[,] array, Func<T, TOut> selector)
		{
			var result = new TOut[array.GetLength(0), array.GetLength(1)];

			for (var x = 0; x < array.GetLength(0); x++)
				for (var y = 0; y < array.GetLength(1); y++)
					result[x, y] = selector(array[x, y]);

			return result;
		}

		public static TOut[,] Map<T, TOut>(this T[,] array, Func<int, int, TOut> selector)
		{
			var result = new TOut[array.GetLength(0), array.GetLength(1)];

			for (var x = 0; x < array.GetLength(0); x++)
				for (var y = 0; y < array.GetLength(1); y++)
					result[x, y] = selector(x, y);

			return result;
		}

		public static TOut[,] Map<T, TOut>(this T[,] array, Func<int, int, T, TOut> selector)
		{
			var result = new TOut[array.GetLength(0), array.GetLength(1)];

			for (var x = 0; x < array.GetLength(0); x++)
				for (var y = 0; y < array.GetLength(1); y++)
					result[x, y] = selector(x, y, array[x, y]);

			return result;
		}

		public static int IndexOfMax(this int[] arr)
		{
			if (arr.Length == 0)
				throw new InvalidOperationException("Array is empty");

			var mi = 0;
			var max = arr[mi];

			for (var i = 1; i < arr.Length; i++)
			{
				var v = arr[i];
				if (v > max)
				{
					mi = i;
					max = v;
				}
			}

			return mi;
		}

		public static T[] RemoveAt<T>(this T[] source, int index)
		{
			var dest = new T[source.Length - 1];
			if (index > 0)
				Array.Copy(source, 0, dest, 0, index);

			if (index < source.Length - 1)
				Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

			return dest;
		}

		public static bool[] CreateTrues(int size)
		{
			var result = new bool[size];
			for (var i = 0; i < result.Length; i++)
				result[i] = true;
			return result;
		}

		public static T[] Create<T>(int size, Func<int, T> createByIndex)
		{
			var result = new T[size];
			for (var i = 0; i < size; i++)
				result[i] = createByIndex(i);
			return result;
		}

		public static T[][] Create<T>(int size1, int size2)
		{
			var result = new T[size1][];
			for (var i = 0; i < size1; i++)
				result[i] = new T[size2];
			return result;
		}

		public static void PredictionToOneHot(this double[] prediction)
		{
			var maxI = prediction.IndexOfElementWithMax(d => d);
			for (var i = 0; i < prediction.Length; i++)
				prediction[i] = i == maxI ? 1d : 0;
		}

		public static double[] ClassToOneHot(this int[] classes, int classNumber)
		{
			var result = new double[classes.Length];
			result[classNumber] = 1d;
			return result;
		}

		public static T GetSafe<T>(this T[] array, int index)
		{
			return index < array.Length ? array[index] : default(T);
		}

		public static int[] GetSortedIndices(int len, Comparison<int> comparison)
		{
			var indices = new int[len];
			for (var i = 0; i < indices.Length; i++)
				indices[i] = i;

			Array.Sort(indices, comparison);

			return indices;
		}

		public static void Swap<T>(this IList<T> arr, int i, int j)
		{
			var tmp = arr[i];
			arr[i] = arr[j];
			arr[j] = tmp;
		}
	}
}