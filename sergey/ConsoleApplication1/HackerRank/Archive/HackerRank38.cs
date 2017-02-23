using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// https://www.hackerrank.com/challenges/building-a-list

namespace ConsoleApplication1.HackerRank
{
	class HackerRank38
	{
		public void Go()
		{
			Performance();
			Console.WriteLine(Solve_Rec("abcde").Join());
			Console.WriteLine(Solve("abcde").Join());
			//Console.WriteLine(Solve("abcdefhijklmnopq").Count());
		}

		public static void Compare()
		{
			const string alphabet = "abcdefhijklmnopqrstuvwxyz";

			for (var len = 1; len < alphabet.Length; len++)
			{
				var s = alphabet.Substring(0, len);

				var isOk = Solve_Rec(s).SequenceEqual(Solve(s));

				if (!isOk)
					throw new InvalidOperationException();
			}
		}

		public static void Performance()
		{
			var timer = Stopwatch.StartNew();
			Console.WriteLine(Solve("abcdefghijklmnopqrstuvw").Count());
			Console.WriteLine("Solving time: " + timer.ElapsedMilliseconds);
		}

		public static IEnumerable<string> Solve(string s)
		{
			// abcdefhijklmnopqrstuv ~ 752 ms
			// abcdefhijklmnopqrstuvw ~ 3129 ms

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

		public static IEnumerable<string> Solve_Rec(string s)
		{
			// abcdefhijklmnopqrstuv ~ 1603 ms

			var letters = s.ToCharArray();
			Array.Sort(letters, 0, letters.Length);
			return Solve_Rec(letters, 0);
		}

		public static IEnumerable<string> Solve_Rec(char[] letters, int i)
		{
			var s0 = letters[i].ToString();

			yield return s0;

			if (i < letters.Length - 1)
			{
				foreach (var sub in Solve_Rec(letters, i + 1))
					yield return s0 + sub;

				foreach (var sub in Solve_Rec(letters, i + 1))
					yield return sub;
			}
		}
	}
}
