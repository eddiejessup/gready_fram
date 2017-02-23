using System;
using System.Collections.Generic;

namespace ConsoleApplication1.Helpers
{
	public static class BinarySearchHelper
	{
		// If array contains x: return most left index of x
		// Else: return most right index of item, that less than x
		public static int BinarySearchLeftBiased<T>(IList<T> arr, Func<T, int> compare)
		{
			var li = 0;
			var ri = arr.Count - 1;

			while (true)
			{
				var mi = (li + ri) / 2;

				if (mi == li)
				{
					return compare(arr[ri]) > 0 ? li : compare(arr[li]) == 0 ? li : ri;
				}

				var miCompare = compare(arr[mi]);

				if (miCompare < 0)
				{
					li = mi;
				}
				else if (miCompare == 0)
				{
					ri = mi;
				}
				else
				{
					ri = mi;
				}
			}
		}

		public static int BinarySearchLeftBiased<T>(IList<T> arr, Func<T, ulong> convert, ulong searchme)
		{
			var li = 0;
			var ri = arr.Count - 1;

			while (true)
			{
				var mi = (li + ri) / 2;

				if (mi == li)
				{
					return convert(arr[ri]) > searchme ? li : convert(arr[li]) == searchme ? li : ri;
				}

				var miValue = convert(arr[mi]);

				if (miValue < searchme)
				{
					li = mi;
				}
				else if (miValue == searchme)
				{
					ri = mi;
				}
				else
				{
					ri = mi;
				}
			}
		}

		public static int BinarySearchLeftBiased(IList<ulong> arr, ulong searchme)
		{
			var li = 0;
			var ri = arr.Count - 1;

			while (true)
			{
				var mi = (li + ri) / 2;

				if (mi == li)
				{
					return arr[ri] > searchme ? li : arr[li] == searchme ? li : ri;
				}

				var miValue = arr[mi];

				if (miValue < searchme)
				{
					li = mi;
				}
				else if (miValue == searchme)
				{
					ri = mi;
				}
				else
				{
					ri = mi;
				}
			}
		}

		public static int BinarySearchLeftBiased(IList<int> arr, int searchme)
		{
			var li = 0;
			var ri = arr.Count - 1;

			while (true)
			{
				var mi = (li + ri) / 2;

				if (mi == li)
				{
					return arr[ri] > searchme ? li : arr[li] == searchme ? li : ri;
				}

				var miValue = arr[mi];

				if (miValue < searchme)
				{
					li = mi;
				}
				else if (miValue == searchme)
				{
					ri = mi;
				}
				else
				{
					ri = mi;
				}
			}
		}

		// If array contains x: return most right index of x
		// Else: return most left index of item, that greater than x
		public static int BinarySearchRightBiased<T>(IList<T> arr, Func<T, int> compare)
		{
			var li = 0;
			var ri = arr.Count - 1;

			while (true)
			{
				var mi = (li + ri) / 2;

				if (mi == li)
				{
					return compare(arr[li]) < 0 ? ri : compare(arr[ri]) == 0 ? ri : li;
				}

				var miCompare = compare(arr[mi]);

				if (miCompare < 0)
				{
					li = mi;
				}
				else if (miCompare == 0)
				{
					li = mi;
				}
				else
				{
					ri = mi;
				}
			}
		}

		public static int BinarySearchRightBiased<T>(IList<T> arr, Func<T, ulong> convert, ulong searchme)
		{
			var li = 0;
			var ri = arr.Count - 1;

			while (true)
			{
				var mi = (li + ri) / 2;

				if (mi == li)
				{
					return convert(arr[li]) < searchme ? ri : convert(arr[ri]) == searchme ? ri : li;
				}

				var miValue = convert(arr[mi]);

				if (miValue < searchme)
				{
					li = mi;
				}
				else if (miValue == searchme)
				{
					li = mi;
				}
				else
				{
					ri = mi;
				}
			}
		}
	}
}
