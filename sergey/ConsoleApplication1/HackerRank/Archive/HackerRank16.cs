using ConsoleApplication1.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// https://www.hackerrank.com/challenges/sherlock-and-anagrams

namespace ConsoleApplication1.HackerRank
{
	class HackerRank16
	{
		public void Go()
		{
			Console.WriteLine(Solve("abba"));
			Console.WriteLine(Solve("abcd"));
			Console.WriteLine(Solve("ifailuhkqq"));
			Console.WriteLine(Solve("hucpoltgty"));
			Console.WriteLine(Solve("ovarjsnrbf"));
			Console.WriteLine(Solve("pvmupwjjjf"));
			Console.WriteLine(Solve("iwwhrlkpek"));
		}

		public static ulong Solve(string s)
		{
			var substrings = new Dictionary<string, ulong>();

			var cnt = 0ul;

			for (var i = 0; i <= s.Length - 1; i++)
			{
				for (var len = 1; len + i <= s.Length; len++)
				{
					var substr = new string(s.Substring(i, len).OrderBy(c => c).ToArray());

					ulong oldCount;
					if (substrings.TryGetValue(substr, out oldCount))
					{
						cnt += oldCount;
						substrings[substr] = oldCount + 1;
					}
					else
					{
						substrings[substr] = 1ul;
					}
				}
			}

			return cnt;
		}
	}
}
