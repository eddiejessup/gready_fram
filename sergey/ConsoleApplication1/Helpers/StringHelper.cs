using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApplication1.Helpers
{
	public static class StringHelper
	{
		public static string Join<T>(this IEnumerable<T> seq, string separator = " ")
		{
			return string.Join(separator, seq);
		}

		public static string Join<T>(this string s, IEnumerable<T> seq)
		{
			return string.Join(s, seq);
		}

		public static string JoinTrimmed<T>(this string s, IEnumerable<T> seq, int trimCount)
		{
			var sb = new StringBuilder();

			var i = 0;
			foreach (var item in seq)
			{
				if (i > 0) sb.Append(s);

				if (i >= trimCount)
				{
					sb.Append("...");
					break;
				}

				sb.Append((typeof(T).IsClass && Equals(item, default(T))) ? "null" : item.ToString());

				i++;
			}

			return sb.ToString();
		}

		public static string[] SplitToLines(this string s)
		{
			return s.Split(new[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
		}

		public static int SubstringSearch_KMP(string s, string w, int start = 0)
		{
			if (w.Length == 0) return start;

			var pos = 2;
			var cnd = 0;

			var T = new int[w.Length];
			T[0] = -1;

			while (pos < w.Length)
			{
				if (w[pos - 1] == w[cnd])
				{
					T[pos] = cnd + 1;
					cnd++;
					pos++;
				}
				else if (cnd > 0)
				{
					cnd = T[cnd];
				}
				else
				{
					T[pos] = 0;
					pos++;
				}
			}

			var m = start;
			var i = 0;

			while (m + i < s.Length)
			{
				if (w[i] == s[m + i])
				{
					if (i == w.Length - 1)
						return m;
					i++;
				}
				else
				{
					if (T[i] > -1)
					{
						m += i - T[i];
						i = T[i];
					}
					else
					{
						m++;
						i = 0;
					}
				}
			}

			return -1;
		}
	}
}