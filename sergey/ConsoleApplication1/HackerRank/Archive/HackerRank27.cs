using Combinatorics.Collections;
using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.HackerRank
{
	class HackerRank27
	{
		public void Go()
		{
			//Console.WriteLine(Solve_Brute("abbba"));
			//Solve("abbaab");
			Compare();
		}

		public static void Performance()
		{
			var rnd = new Random(1337); 
			var str = RandomGenerator.RandomString(rnd, "abcdefghijklmnopqrstuvwxyz", 10000000);

			// My : 4476, 4477, 4483
			// Uwi: 936, 938, 952

			var timer = Stopwatch.StartNew();
			Console.WriteLine(Solve_Uwi(str));
			Console.WriteLine("Solving: " + timer.ElapsedMilliseconds);
		}

		public static void Compare()
		{
			var rnd = new Random(1337);
			for (var t = 0; t < 100000; t++)
			{
				var len = rnd.Next(4, 10);
				var str = RandomGenerator.RandomString(rnd, "abcd", len);
				var brute = Solve_Uwi(str);
				var actual = Solve(str);

				if (actual != brute)
				{
					Console.WriteLine(new { str, brute, actual });
					throw new InvalidOperationException();
				}
			}
		}

		public static ulong Solve_Brute(string s)
		{
			var count = 0ul;
			foreach (var comb in new Combinations<char>(s.ToCharArray(), 4))
				if (comb[0] == comb[3] && comb[1] == comb[2])
					count = (count + 1) % MOD;
			return count;
		}

		const ulong MOD = 1000000007ul;
		const int ALFABET_SIZE = 26;

		public static ulong Solve_V2(string s)
		{
			//var indexes = new List<int>[ALFABET_SIZE];
			//for (var i = 0; i < indexes.Length; i++)
			//	indexes[i] = new List<int>();

			//for (var i = 0; i < s.Length; i++)
			//	indexes[s[i] - 'a'].Add(i);

			var abCounts = GetSubstrCounts_Ab(s);
			var total = Get_Abba(s, abCounts);

			return total;
		}

		public static ulong Solve_Uwi(string s)
		{
			int n = s.Length;
			long gret = 0;
			for (int c = 0; c < 26; c++)
			{
				long[] f = new long[26];	// f[d] сколько подстрок cd в строке. Например abbbbdada, для c='a': f=[0,4,0,3,0,...]
				long ret = 0;
				int plus = 0;    // Это сколько раз в строке встретилась буква c
				long buf = 0;	 // Когда идем по строке, в buf копится 
				for (int i = 0; i < n; i++)
				{
					if (s[i] == 'a' + c)
					{
						plus++;
						ret += buf;		
						ret %= (long)MOD;
					}
					else
					{
						buf += f[s[i] - 'a'];
						f[s[i] - 'a'] += plus;
					}
				}
				gret += ret;
				gret %= (long)MOD;
			}
			long[] ff = new long[26];
			for (int i = 0; i < n; i++)
			{
				ff[s[i] - 'a']++;
			}
			for (int i = 0; i < 26; i++)
			{
				if (ff[i] <= 3) continue;
				long d = 24;
				long val = 1;
				for (int j = 0; j < 4; j++)
				{
					long v = ff[i] - j;
					long g = gcd(v, d);
					val = val * (v / g) % (long)MOD;
					d /= g;
				}
				gret += val;
			}
			return (ulong)gret % MOD;
		}

		public static long gcd(long a, long b)
		{
			while (b > 0)
			{
				long c = a;
				a = b;
				b = c % b;
			}
			return a;
		}

		public static ulong Solve(string s)
		{
			var abCounts = new ulong[ALFABET_SIZE * ALFABET_SIZE];

			var aCounts = new ulong[ALFABET_SIZE];
			aCounts[s[0] - 'a'] = 1;

			var abbCounts = new ulong[ALFABET_SIZE * ALFABET_SIZE];

			var total = 0ul;

			for (var i = 1; i < s.Length; i++)
			{
				var letter = s[i] - 'a';

				for (var b = 0; b < ALFABET_SIZE; b++)
					total = (total + abbCounts[b * ALFABET_SIZE + letter]) % MOD;

				for (var a = 0; a < ALFABET_SIZE; a++)
				{
					var j = letter * ALFABET_SIZE + a;

					var val = abCounts[j];
					var valAbb = abbCounts[j];

					val = (val + aCounts[a]) % MOD;
					valAbb = (valAbb + abCounts[j]) % MOD;

					abCounts[j] = val;
					abbCounts[j] = valAbb;
				}

				aCounts[letter]++;
			}

			return total;
		}

		public static ulong Get_Abba(string s, ulong[][] abCounts)
		{
			var abbCounts = new ulong[abCounts.Length];
			var nextCounts = new ulong[abCounts.Length];

			var total = 0ul;

			for (var j = 1; j < s.Length; j++)
			{
				var letter = s[j] - 'a';

				for (var i = 0; i < abbCounts.Length; i++)
				{
					var b = i / ALFABET_SIZE;

					var val = abbCounts[i];

					if (letter == b)
						val = (val + abCounts[i][j - 1]) % MOD;

					nextCounts[i] = val;
				}

				for (var b = 0; b < ALFABET_SIZE; b++)
				{
					total = (total + abbCounts[b * ALFABET_SIZE + letter]) % MOD;
				}

				var tmp = abbCounts;
				abbCounts = nextCounts;
				nextCounts = tmp;
			}

			return total;
		}

		public static ulong[][] GetSubstrCounts_Ab(string s)
		{
			var result = new ulong[ALFABET_SIZE * ALFABET_SIZE][];

			for (var i = 0; i < result.Length; i++)
				result[i] = new ulong[s.Length];

			var aCounts = new ulong[ALFABET_SIZE];
			aCounts[s[0] - 'a'] = 1;

			for (var i = 1; i < s.Length; i++)
			{
				var letter = s[i] - 'a';

				for (var j = 0; j < result.Length; j++)
				{
					var a = j % ALFABET_SIZE;
					var b = j / ALFABET_SIZE;

					var val = result[j][i - 1];
					if (letter == b)
						val = (val + aCounts[a]) % MOD;
					result[j][i] = val;
				}

				aCounts[letter]++;
			}

			return result;
		}

		public static ulong GetSubstrCounts_Abba(string s, ulong[][] abbCounts)
		{
			var total = 0ul;

			for (var i = 1; i < s.Length; i++)
				for (var b = 0; b < ALFABET_SIZE; b++)
					total = (total + abbCounts[b * ALFABET_SIZE + s[i] - 'a'][i - 1]) % MOD;

			return total;
		}

		public static ulong[][] GetSubstrCounts_Abb(string s, ulong[][] abCounts)
		{
			var result = new ulong[abCounts.Length][];

			for (var i = 0; i < result.Length; i++)
				result[i] = new ulong[s.Length];

			for (var j = 1; j < s.Length; j++)
			{
				var letter = s[j] - 'a';

				for (var i = 0; i < result.Length; i++)
				{
					var b = i / ALFABET_SIZE;

					var val = result[i][j - 1];
					if (letter == b)
						val = (val + abCounts[i][j - 1]) % MOD;
					result[i][j] = val;
				}
			}

			return result;
		}

		// a, i => # of 'a' which lie left then i
		public static ulong[][] GetACounts(string s)
		{
			var result = new ulong[ALFABET_SIZE][];

			for (var a = 0; a < ALFABET_SIZE; a++)
			{
				var letter = a + 'a';
				var count = 0ul;
				var stat = new ulong[s.Length];
				for (var j = 0; j < s.Length; j++)
				{
					if (s[j] == letter)
						count++;
					stat[j] = count;
				}
				result[a] = stat;
			}

			return result;
		}

		public static ulong Solve_Old(string s)
		{
			var indexes = new List<int>[27];
			for (var i = 0; i < indexes.Length; i++)
				indexes[i] = new List<int>();

			for (var i = 0; i < s.Length; i++)
				indexes[s[i] - 'a'].Add(i);

			var leftStats = new ulong[indexes.Length][];
			for (var i = 0; i < indexes.Length; i++)
			{
				var letter = i + 'a';
				var count = 0ul;
				var stat = new ulong[s.Length];
				for (var j = 0; j < s.Length; j++)
				{
					if (s[j] == letter)
						count++;
					stat[j] = count;
				}
				leftStats[i] = stat;
			}

			return 42;
		}

		/// <summary>
		/// c -> d -> sIndex -> count
		/// </summary>
		public static ulong[][][] GetDcStats(string s, List<int>[] indexes)
		{
			var rightStats = new ulong[indexes.Length][];
			for (var i = 0; i < indexes.Length; i++)
			{
				var letter = i + 'a';
				var count = 0ul;
				var stat = new ulong[s.Length];
				for (var j = s.Length - 1; j >= 0; i--)
				{
					if (s[j] == letter)
						count++;
					stat[j] = count;
				}
				rightStats[i] = stat;
			}

			var result = new ulong[indexes.Length][][];

			for (var c = 0; c < indexes.Length; c++)
			{
				var arr = new ulong[indexes.Length][];

				for (var d = 0; d < indexes.Length; d++)
					arr[d] = new ulong[s.Length];

				foreach (var ci in indexes[c])
					if (ci < s.Length)
						for (var d = 0; d < indexes.Length; d++)
							arr[d][ci] = rightStats[d][ci + 1];

				result[c] = arr;
			}

			return result;
		}

		/// <summary>
		/// b -> a -> sIndex -> count
		/// </summary>
		public static ulong[][][] GetAbStats(string s, List<int>[] indexes)
		{
			var leftStats = new ulong[indexes.Length][];
			for (var i = 0; i < indexes.Length; i++)
			{
				var letter = i + 'a';
				var count = 0ul;
				var stat = new ulong[s.Length];
				for (var j = 0; j < s.Length; j++)
				{
					if (s[j] == letter)
						count++;
					stat[j] = count;
				}
				leftStats[i] = stat;
			}

			var result = new ulong[indexes.Length][][];

			for (var b = 0; b < indexes.Length; b++)
			{
				var arr = new ulong[indexes.Length][];

				for (var a = 0; a < indexes.Length; a++)
					arr[a] = new ulong[s.Length];

				foreach (var bi in indexes[b])
					if (bi > 0)
						for (var a = 0; a < indexes.Length; a++)
							arr[a][bi] = leftStats[a][bi - 1];

				result[b] = arr;
			}

			return result;
		}
	}

}
