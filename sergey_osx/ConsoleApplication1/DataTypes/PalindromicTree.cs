using System;
using System.Collections.Generic;
using System.Text;

// http://codeforces.com/blog/entry/13959?locale=ru
// http://adilet.org/blog/25-09-14/
// http://pastebin.com/A9azCttf

namespace ConsoleApplication1.DataTypes
{
	public class PalindromicTree
	{
		public class Node
		{
			public int[] next = new int[27];
			public int len;
			public int sufflink;
			public long num;
		}

		public int len;
		public string str;
		public Node[] tree;
		public int num;		// node 1 - root with len -1, node 2 - root with len 0
		public int suff;    // max suffix palindrome 

		public PalindromicTree(string s)
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
			tree[num].len = tree[cur].len + 2;
			// tree[num].sampleLeft = pos - newLen + 1;
			tree[cur].next[let] = num;

			if (tree[num].len == 1)
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

		public IEnumerable<Tuple<string, long>> EnumeratePalindromeAndCount()
		{
			foreach (var t in EnumeratePalindromeAndCount(1)) yield return t;
			foreach (var t in EnumeratePalindromeAndCount(2)) yield return t;
		}

		public IEnumerable<Tuple<string, long>> EnumeratePalindromeAndCount(int v, string s = "")
		{
			if (s != "")
				yield return Tuple.Create(s, tree[v].num);

			var adj = tree[v].next;

			for (int i = 0; i < adj.Length; i++)
			{
				if (adj[i] == 0)
					continue;

				string goS = s;

				if (v == 1)
					goS += (char)('a' + i);
				else
				{
					var add = (char)('a' + i);
					goS = add + goS + add;
				}
				foreach (var t in EnumeratePalindromeAndCount(adj[i], goS))
					yield return t;
			}
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

				sb.AppendLine($"[{node} {pals[node]}] len={tree[node].len} #={tree[node].num}");
				sb.AppendLine("    suffixlink " + pals[tree[node].sufflink]);
				sb.AppendLine("    links " + string.Join(" ", adjPals));
			}

			return sb.ToString();
		}
	}
}
