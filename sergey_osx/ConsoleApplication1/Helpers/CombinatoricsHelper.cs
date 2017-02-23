using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.Helpers
{
	public static class CombinatoricsHelper
	{
		// Возвращает лексикографически упорядоченные подстроки s
		// abc -> a ab abc ac b bc c
		// В строке s не должно быть повторяющихся символов
		public static IEnumerable<string> GetLexicographicOrderedSubstrings(string s)
		{
			var index = new Dictionary<char, int>();
			for (var i = 0; i < s.Length; i++)
				index[s[i]] = i;

			var last = s[s.Length - 1];

			var cur = new List<char> { s[0] };

			yield return string.Join("", cur);

			while (true)
			{
				var peek = cur[cur.Count - 1];

				if (peek == last)
				{
					cur.RemoveAt(cur.Count - 1);

					if (cur.Count == 0)
						break;

					peek = cur[cur.Count - 1];
					cur.RemoveAt(cur.Count - 1);
				}

				cur.Add(s[index[peek] + 1]);

				yield return string.Join("", cur);
			}
		}

		public static IEnumerable<List<T>> Combinations<T>(IList<T> list)
		{
			var total = 1ul << list.Count;
			var result = new List<T>(list.Count);
			for (var i = 0ul; i < total; i++)
			{
				result.Clear();

				for (var j = 0; j < list.Count; j++)
					if ((i & (1ul << j)) > 0)
						result.Add(list[j]);

				yield return result;
			}
		}

		public static IEnumerable<T[]> Selections<T>(IList<T>[] alphabet, T[] result = null, int start = 0)
		{
			if (alphabet.Length == 0)
			{
				yield return new T[0];
				yield break;
			}

			result = result ?? new T[alphabet.Length];

			var letters = alphabet[start];

			foreach (var letter in letters)
			{
				result[start] = letter;

				if (start == alphabet.Length - 1)
					yield return result;

				else
					foreach (var sub in Selections(alphabet, result, start + 1))
						yield return result;
			}
		}
	}
}
