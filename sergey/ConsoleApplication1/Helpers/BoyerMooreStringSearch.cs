using System;
using System.Collections.Generic;

// https://en.wikipedia.org/wiki/Boyer%E2%80%93Moore_string_search_algorithm

namespace ConsoleApplication1.Helpers
{
	public static class BoyerMooreStringSearch
	{
		public static IEnumerable<int> FindAllIndexes(string haystack, string needle)
		{
			if (needle.Length == 0)
			{
				for (var i = 0; i < haystack.Length; i++)
					yield return i;
				yield break;
			}

			if (string.IsNullOrEmpty(haystack))
				yield break;

			var charTable = MakeCharTable(needle);
			var offsetTable = MakeOffsetTable(needle);

			for (int i = needle.Length - 1, j; i < haystack.Length;)
			{
				for (j = needle.Length - 1; needle[j] == haystack[i]; --i, --j)
					if (j == 0)
					{
						yield return i;
						break;
					}

				i += Math.Max(offsetTable[needle.Length - 1 - j], charTable[haystack[i]]);
			}
		}

		/// <summary>
		/// Returns the index within this string of the first occurrence of the
		/// specified substring. If it is not a substring, return -1.
		/// </summary>
		/// <param name="haystack">The string to be scanned</param>
		/// <param name="needle">The target string to search</param>
		/// <returns>The start index of the substring</returns>
		public static int IndexOf(string haystack, string needle)
		{
			if (needle.Length == 0)
				return 0;

			if (string.IsNullOrEmpty(haystack))
				return -1;

			var charTable = MakeCharTable(needle);
			var offsetTable = MakeOffsetTable(needle);

			for (int i = needle.Length - 1, j; i < haystack.Length;)
			{
				for (j = needle.Length - 1; needle[j] == haystack[i]; --i, --j)
					if (j == 0)
						return i;
				// i += needle.Length - j; // For naive method
				i += Math.Max(offsetTable[needle.Length - 1 - j], charTable[haystack[i]]);
			}
			return -1;
		}

		/// <summary>
		/// Makes the jump table based on the mismatched character information.
		/// </summary>
		private static int[] MakeCharTable(string needle)
		{
			const int ALPHABET_SIZE = 256;
			int[] table = new int[ALPHABET_SIZE];
			for (int i = 0; i < table.Length; ++i)
			{
				table[i] = needle.Length;
			}
			for (int i = 0; i < needle.Length - 1; ++i)
			{
				table[needle[i]] = needle.Length - 1 - i;
			}
			return table;
		}

		/// <summary>
		/// Makes the jump table based on the scan offset which mismatch occurs.
		/// </summary>
		private static int[] MakeOffsetTable(string needle)
		{
			int[] table = new int[needle.Length];
			int lastPrefixPosition = needle.Length;
			for (int i = needle.Length - 1; i >= 0; --i)
			{
				if (IsPrefix(needle, i + 1))
				{
					lastPrefixPosition = i + 1;
				}
				table[needle.Length - 1 - i] = lastPrefixPosition - i + needle.Length - 1;
			}
			for (int i = 0; i < needle.Length - 1; ++i)
			{
				int slen = SuffixLength(needle, i);
				table[slen] = needle.Length - 1 - i + slen;
			}
			return table;
		}

		/// <summary>
		/// Is needle[p:end] a prefix of needle?
		/// </summary>
		private static bool IsPrefix(string needle, int p)
		{
			for (int i = p, j = 0; i < needle.Length; ++i, ++j)
				if (needle[i] != needle[j])
					return false;
			return true;
		}

		/// <summary>
		/// Returns the maximum length of the substring ends at p and is a suffix.
		/// </summary>
		private static int SuffixLength(string needle, int p)
		{
			int len = 0;
			for (int i = p, j = needle.Length - 1; i >= 0 && needle[i] == needle[j]; --i, --j)
				len += 1;
			return len;
		}
	}
}
