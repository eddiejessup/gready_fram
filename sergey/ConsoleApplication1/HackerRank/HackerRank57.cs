using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static ConsoleApplication1.Helpers.ArrayHelper;
using static ConsoleApplication1.Helpers.GraphHelper;

namespace ConsoleApplication1.HackerRank
{
	class HackerRank57
	{
		public void Go()
		{
			//foreach (var s in GetAllPalindromicSubstrings("aab").OrderBy(s => s)) Console.WriteLine(s);
			//for (var k = 1; k <= 20; k++) Console.WriteLine(Solve("abacbc", k));
			//Console.WriteLine(new PalindromicTreeWithExample("acdccbc").ToStringNodes());
			Performance();
		}

		public static void Performance()
		{
			{
				var rnd = new Random(1337);
				var str = RandomGenerator.RandomString(rnd, "ab", 1000000);
				var timer = Stopwatch.StartNew();
				var sol = SolvePrepare(str);
				var sum = 0L;
				for (var k = 1; k <= 100000; k++)
					sum += Solve(sol, k);
				Console.WriteLine(sum);
				Console.WriteLine("Solve random string: " + timer.ElapsedMilliseconds);
			}

			{
				var str = new string('a', 100000);
				var timer = Stopwatch.StartNew();
				var sol = SolvePrepare(str);
				var sum = 0L;
				for (var k = 1; k <= 100000; k++)
					sum += Solve(sol, k);
				Console.WriteLine(sum);
				Console.WriteLine("Solve aaa...a string: " + timer.ElapsedMilliseconds);
			}

			{
				var a = "b" + new string('a', 300000) + "b";
				var str = string.Join("", Enumerable.Repeat(a, 5));
				var timer = Stopwatch.StartNew();
				var sol = SolvePrepare(str);
				var sum = 0L;
				for (var k = 1; k <= 100000; k++)
					sum += Solve(sol, k);
				Console.WriteLine(sum);
				Console.WriteLine("Solve [ba...ab]...[ba...ab] string: " + timer.ElapsedMilliseconds);
			}

			{
				var str = string.Join("", Enumerable.Repeat("ab", 50000));
				var timer = Stopwatch.StartNew();
				var sol = SolvePrepare(str);
				var sum = 0L;
				for (var k = 1; k <= 100000; k++)
					sum += Solve(sol, k);
				Console.WriteLine(sum);
				Console.WriteLine("Solve [ababab...ab] string: " + timer.ElapsedMilliseconds);
			}
		}

		public static void Compare()
		{
			var rnd = new Random(1337);
			for (var t = 0; t < 30000; t++)
			{
				var len = rnd.Next(1, 10);
				var str = RandomGenerator.RandomString(rnd, "abcd", len);

				var sol = SolvePrepare(str);

				for (var k = 1; k <= len * (len + 1) / 2; k++)
				{
					var brute = SolveBrute(str, k);
					var actual = Solve(sol, k);

					if (actual != brute)
					{
						Console.WriteLine(new { t, str, k, brute, actual });
						throw new InvalidOperationException();
					}
				}
			}
		}

		public class Solut
		{
			public PalindromicTreeWithExample PalTree;
			public string Str;
			public List<int>[] SuffRevAdj;
			public bool[] IsSuffRevAdjSorted;
			public long[] Sums;
			public long[] Hashes;
		}

		private static long MOD = 1000000007L;
		private static long A = 100001L;

		public static long HashStr(string str)
		{
			var sum = 0L;
			var pow = 1L;
			for (var i = str.Length - 1; i >= 0; i--)
			{
				var c = str[i] * pow % MOD;

				sum = (sum + c) % MOD;

				pow = pow * A % MOD;
			}
			return sum;
		}

		public static Solut SolvePrepare(string str)
		{
			var palTree = new PalindromicTreeWithExample(str);

			var pows = new long[str.Length + 1];
			pows[0] = 1L;
			for (var i = 1; i < pows.Length; i++)
				pows[i] = pows[i - 1] * A % MOD;

			var hashes = new long[palTree.num + 1];
			var letters = new char[palTree.num + 1];
			var revEdges = new int[palTree.num + 1];
			var suffRevAdj = Create(palTree.num + 1, i => new List<int>());

			{
				var queue = new Queue<int>();
				queue.Enqueue(1);
				queue.Enqueue(2);
				while (queue.Count > 0)
				{
					var node = queue.Dequeue();

					var suff = palTree.tree[node].sufflink;
					if (node > 2 && suff > 0)
						suffRevAdj[suff].Add(node);

					var adj = palTree.tree[node].next;

					for (var i = 0; i < adj.Length; i++)
						if (adj[i] > 0)
						{
							var next = adj[i];

							letters[next] = (char)('a' + i);
							revEdges[next] = node;
							queue.Enqueue(next);

							var c = (int)letters[next];
							if (node == 1)
								hashes[next] = c % MOD;
							else if (node == 2)
								hashes[next] = (c * pows[1] + c) % MOD;
							else
							{
								var nodeHash = hashes[node];
								var nextLen = palTree.tree[next].len;
								var nextHash = ((pows[nextLen - 1] * c) + (nodeHash * A) + c) % MOD;
								hashes[next] = nextHash;
							}
						}
				}
			}

			//for (var i = 0; i < suffRevAdj.Length; i++)
			//{
			//	var adj = suffRevAdj[i];
			//	if (adj.Count <= 1) continue;
			//	adj.Sort((v1, v2) => CompareNodes(i, v1, v2, palTree, str));
			//}

			var sums = new long[palTree.num + 1];
			foreach (var node in Traverse_Tree_PostOrder_NonRec(2, v => suffRevAdj[v]))
			{
				var sum = palTree.tree[node].num;
				foreach (var next in suffRevAdj[node])
					sum += sums[next];
				sums[node] = sum;
			}

			return new Solut
			{
				PalTree = palTree,
				Str = str,
				SuffRevAdj = suffRevAdj,
				Sums = sums,
				Hashes = hashes,
				IsSuffRevAdjSorted = new bool[palTree.num + 1]
			};
		}

		public static long Solve(Solut sol, long k)
		{
			var palTree = sol.PalTree;
			var str = sol.Str;
			var suffRevAdj = sol.SuffRevAdj;
			var sums = sol.Sums;
			var hashes = sol.Hashes;

			var node = 2;
			var sum = 0L;
			while (true)
			{
				var found = false;

				var adj = suffRevAdj[node];
				if (adj.Count > 1 && !sol.IsSuffRevAdjSorted[node])
				{
					adj.Sort((v1, v2) => CompareNodes(node, v1, v2, palTree, str));
					sol.IsSuffRevAdjSorted[node] = true;
				}

				foreach (var next in suffRevAdj[node])
				{
					var nextVal = palTree.tree[next].num;
					var nextSum = sums[next];

					if (sum + nextVal >= k)
						return hashes[next];

					if (sum + nextSum >= k)
					{
						sum += nextVal;
						node = next;
						found = true;
						break;
					}

					sum += nextSum;
				}
				if (!found)
					return -1;
			}
		}

		public static int CompareNodes(int parent, int v1, int v2, PalindromicTreeWithExample tree, string str)
		{
			var parentLen = tree.tree[parent].len;

			var left1 = tree.tree[v1].sampleLeft;
			var left2 = tree.tree[v2].sampleLeft;

			var len1 = tree.tree[v1].len;
			var len2 = tree.tree[v2].len;

			var i = parentLen;
			while (true)
			{
				if (i == len1 && i == len2) return 0;
				if (i == len1) return -1;
				if (i == len2) return 1;

				var comp = str[i + left1].CompareTo(str[i + left2]);
				if (comp != 0) return comp;
				i++;
			}
		}

		public static long SolveBrute(string str, int k)
		{
			var result = GetAllPalindromicSubstrings(str).OrderBy(s => s).Skip(k - 1).FirstOrDefault();
			if (result == null) return -1;
			return HashStr(result);
		}

		public static IEnumerable<string> GetAllPalindromicSubstrings(string str)
		{
			for (var i = 0; i < str.Length; i++)
			{
				for (var j = i; j < str.Length; j++)
				{
					var isPal = true;

					var ii = i;
					var jj = j;
					while (ii <= jj)
					{
						if (str[ii] != str[jj])
						{
							isPal = false;
							break;
						}
						ii++;
						jj--;
					}

					if (isPal)
						yield return str.Substring(i, j - i + 1);
				}
			}
		}

		public class PalindromicTreeWithExample
		{
			public class Node
			{
				public int[] next = new int[27];
				public int len;
				public int sufflink;
				public long num;
				public int sampleLeft;
			}

			public int len;
			public string str;
			public Node[] tree;
			public int num;     // node 1 - root with len -1, node 2 - root with len 0
			public int suff;    // max suffix palindrome 

			public PalindromicTreeWithExample(string s)
			{
				str = s;
				len = s.Length;

				tree = new Node[len + 3];
				for (var i = 0; i < tree.Length; i++)
					tree[i] = new Node();

				InitTree();

				for (int i = 0; i < len; i++)
				{
					AddLetter(i);
					tree[suff].num++;
				}

				for (int i = num; i > 2; i--)
				{
					tree[tree[i].sufflink].num += tree[i].num;
				}
			}

			public bool AddLetter(int pos)
			{
				int cur = suff, curlen = 0;
				int let = str[pos] - 'a';

				while (true)
				{
					curlen = tree[cur].len;
					if (pos - 1 - curlen >= 0 && str[pos - 1 - curlen] == str[pos])
						break;
					cur = tree[cur].sufflink;
				}
				if (tree[cur].next[let] > 0)
				{
					suff = tree[cur].next[let];
					return false;
				}

				num++;

				suff = num;
				var newLen = tree[cur].len + 2;
				tree[num].len = newLen;
				tree[num].sampleLeft = pos - newLen + 1;
				tree[cur].next[let] = num;

				if (newLen == 1)
				{
					tree[num].sufflink = 2;
					return true;
				}

				while (true)
				{
					cur = tree[cur].sufflink;
					curlen = tree[cur].len;
					if (pos - 1 - curlen >= 0 && str[pos - 1 - curlen] == str[pos])
					{
						tree[num].sufflink = tree[cur].next[let];
						break;
					}
				}

				return true;
			}

			public void InitTree()
			{
				num = 2; suff = 2;
				tree[1].len = -1; tree[1].sufflink = 1;
				tree[2].len = 0; tree[2].sufflink = 1;
			}

			public string ToStringNodes()
			{
				var sb = new StringBuilder();

				var pals = new string[num + 1];
				pals[1] = "-1";
				pals[2] = ".";

				var queue = new Queue<int>();
				queue.Enqueue(1);
				queue.Enqueue(2);
				while (queue.Count > 0)
				{
					var node = queue.Dequeue();

					var adjPals = new List<string>();

					var adj = tree[node].next;
					for (var i = 0; i < adj.Length; i++)
						if (adj[i] > 0)
						{
							var next = adj[i];
							var c = ((char)('a' + i)).ToString();
							pals[next] = node == 1 ? c : string.Concat(c, pals[node], c);
							queue.Enqueue(next);
							adjPals.Add(pals[next]);
						}

					sb.AppendLine($"[{node} {pals[node]}] len={tree[node].len} #={tree[node].num} left={tree[node].sampleLeft}");
					sb.AppendLine("    suffixlink " + pals[tree[node].sufflink]);
					sb.AppendLine("    links " + string.Join(" ", adjPals));
				}

				return sb.ToString();
			}
		}
	}
}
