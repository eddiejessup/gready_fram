using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using static ConsoleApplication1.Helpers.GraphHelper;

// https://www.hackerrank.com/contests/w23/challenges/gravity-1

namespace ConsoleApplication1.HackerRank
{
	class HackerRank61
	{
		public void Go()
		{
			//Example2();
			//Performance();
			Compare();
			//SolveByConsole();
		}

		public void SolveByConsole()
		{
			//var parents = RandomGenerator.RandomIntArray(new Random(), 10000000, 0, 1000000000);
			//File.WriteAllText("graph.txt", parents.Join());


			// File: tree.parents.1M.txt
			// Elapsed milliseconds: 3555
			//var parents = HR.ReadIntArray();
			//foreach (var i in parents)
			//	Console.WriteLine(i + 1);

			// File: tree.parents.1M.txt: 585, 566, 569
			// File: random.10M.txt: 7106, 7206
			//var parents = HR.ReadIntArray();
			//var sb = new StringBuilder();
			//foreach (var i in parents)
			//	sb.AppendLine((i + 1).ToString());
			//Console.WriteLine(sb);

			// File: random.10M.txt: 6750, 6810
			//var str = Console.ReadLine();
			//var tokens = str.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
			//var parents = new int[tokens.Length];
			//for (var i = 0; i < tokens.Length; i++) parents[i] = int.Parse(tokens[i]);
			//var sb = new StringBuilder();
			//foreach (var i in parents)
			//	sb.AppendLine((i + 1).ToString());
			//Console.WriteLine(sb);

			var str = Console.ReadLine();
			var tokens = str.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
			var parents = new int[tokens.Length];
			for (var i = 0; i < tokens.Length; i++) parents[i] = int.Parse(tokens[i]);
			foreach (var i in parents)
			{
				var istr = i.ToString();
				foreach (var c in istr)
					Console.Write(c);
				Console.Write('\n');
			}
		}

		public static void Compare()
		{
			var rnd = new Random(1337);
			for (var t = 0; t < 20000; t++)
			{
				var n = rnd.Next(1, 10);
				var tree_edges = RandomGenerator.RandomTree_Edges(rnd, n, 3, false);

				var parents = new int[n - 1];
				foreach (var edge in tree_edges)
					parents[edge[1] - 1] = edge[0] + 1;

				var children = ConvertParentsArrayToAdj(parents);

				var data = CalcData(parents);

				var uvs = new List<int[]>();

				for (var q = 0; q < 200; q++)
				{
					var u = rnd.Next(1, n + 1);
					var v = rnd.Next(1, n + 1);

					uvs.Add(new[] { u, v });
				}

				var brute = uvs.Select(uv => Solve(data, uv[0], uv[1])).ToArray();

				var arguments = new List<int>();
				arguments.Add(n);
				arguments.AddRange(parents);
				arguments.Add(uvs.Count);
				arguments.AddRange(uvs.SelectMany(uv => uv));
				var uwi = new HackerRank61_Uwi().Solve(arguments);

				for (var q = 0; q < uvs.Count; q++)
				{
					if (uwi[q] != brute[q])
					{
						Console.WriteLine(new { t, q, n, brute = brute[q], uwi = uwi[q] });
						throw new InvalidOperationException();
					}
				}
			}
		}

		public static void Performance()
		{
			var n = 100000;
			var qq = 100000;

			var rnd = new Random(1337);
			//var tree_edges = RandomGenerator.RandomTree_Edges(rnd, n, 3, false);
			//var parents = new int[n - 1];
			//foreach (var edge in tree_edges)
			//	parents[edge[1] - 1] = edge[0] + 1;

			var parents = new int[n - 1];
			for (var i = 0; i < parents.Length; i++)
				parents[i] = i + 1;

			var timer = Stopwatch.StartNew();
			var data = CalcData(parents);
			var sb = new StringBuilder();
			for (var q = 0; q < qq; q++)
				sb.AppendLine(Solve(data, rnd.Next(1, n + 1), rnd.Next(1, n + 1)).ToString());
			Console.WriteLine(sb.Length);
			Console.WriteLine("Solving time: " + timer.ElapsedMilliseconds);
		}

		public static void Example1()
		{
			var parents = new[] { 1, 2, 2, 4 };
			var data = CalcData(parents);
			Console.WriteLine(Solve(data, 2, 1));
			Console.WriteLine(Solve(data, 1, 4));
		}

		public static void Example2()
		{
			var parents = new[] { 1, 2, 3, 4 };
			var data = CalcData(parents);
			Console.WriteLine(Solve(data, 2, 1));
			Console.WriteLine(Solve(data, 1, 4));
		}

		public class Data
		{
			public List<int>[] Children;
			public int[] Parents;
			public ulong[] Sizes;
			public ulong[] DistSums;
			public ulong[] DistSumsSquared;
			public ulong[] Levels;
		}

		public static Data CalcData(int[] parents)
		{
			var children = ConvertParentsArrayToAdj(parents);

			var sizes = new ulong[children.Length];
			var distSums = new ulong[children.Length];
			var distSumsSquared = new ulong[children.Length];

			foreach (var node in Traverse_Tree_PostOrder_NonRec(1, i => children[i]))
			{
				sizes[node] = 1ul + children[node].Select(ci => sizes[ci]).Sum();

				distSums[node] = children[node].Select(ci => sizes[ci] + distSums[ci]).Sum();

				distSumsSquared[node] = children[node].Select(ci => sizes[ci] + 2ul * distSums[ci] + distSumsSquared[ci]).Sum();
			}

			var levels = new ulong[children.Length];

			{
				var queue = new Queue<int>();
				queue.Enqueue(1);
				while (queue.Count > 0)
				{
					var node = queue.Dequeue();

					var isOneChild = children[node].Count == 1;
					
					foreach (var child in children[node])
					{
						levels[child] = levels[node] + 1;
						queue.Enqueue(child);
					}
				}
			}

			return new Data
			{
				Children = children,
				Parents = parents,
				Sizes = sizes,
				DistSums = distSums,
				DistSumsSquared = distSumsSquared,
				Levels = levels,
			};
		}

		public static ulong Solve(Data data, int u, int v)
		{
			//Console.WriteLine(new { u, v });
			//Console.WriteLine(Tree_To_String(children, 1));

			var children = data.Children;
			var parents = data.Parents;
			var sizes = data.Sizes;
			var distSums = data.DistSums;
			var distSumsSquared = data.DistSumsSquared;
			var levels = data.Levels;

			if (u == v) return distSumsSquared[u];

			var isUunderV = false;
			var uUnderVdist = 0ul;

			if (levels[u] > levels[v])
			{
				var uhat = u;
				while (uhat != 1)
				{
					uhat = parents[uhat - 2];
					uUnderVdist++;
					if (uhat == v)
					{
						isUunderV = true;
						break;
					}
				}
			}

			if (isUunderV)
			{
				var ans = distSumsSquared[v] + 2 * uUnderVdist * distSums[v] - 4 * distSums[u] +
							uUnderVdist * uUnderVdist * sizes[v] - 4 * sizes[u];

				int prevUhat;
				var uhat = u;
				var s = 0ul;
				do
				{
					prevUhat = uhat;

					uhat = parents[uhat - 2];
					s++;

					if (uhat != v) ans -= 4 * (distSums[uhat] + (s + 1) * sizes[uhat]);
				}
				while (uhat != v);

				return ans;
			}
			else
			{
				var distance = 0ul;

				var low = levels[u] > levels[v] ? u : v;
				var high = levels[u] > levels[v] ? v : u;
				while (levels[low] > levels[high])
				{
					distance++;
					low = parents[low - 2];
				}
				while (low != high)
				{
					distance += 2;
					low = parents[low - 2];
					high = parents[high - 2];
				}

				var ans = distSumsSquared[v] + 2ul * distance * distSums[v] + distance * distance * sizes[v];
				return ans;
			}
		}

		public static List<int>[] ConvertParentsArrayToAdj(int[] parents)
		{
			var tree = new List<int>[parents.Length + 2];

			for (var i = 1; i < tree.Length; i++)
				tree[i] = new List<int>();

			for (var i = 0; i < parents.Length; i++)
				tree[parents[i]].Add(i + 2);

			return tree;
		}

		public static ulong SolveBrute(List<int>[] children, int[] parents, int u, int v)
		{
			var turnedOn = new bool[children.Length];
			foreach (var subV in Traverse_Tree_Bfs(v, i => children[i]))
				turnedOn[subV] = true;

			var ans = 0ul;

			var visited = new bool[children.Length];

			var queue = new Queue<int>();
			var dists = new Queue<ulong>();

			queue.Enqueue(u);
			dists.Enqueue(0);

			while (queue.Count > 0)
			{
				var node = queue.Dequeue();
				var dist = dists.Dequeue();

				if (visited[node])
					continue;

				visited[node] = true;

				if (turnedOn[node])
					ans += dist * dist;

				foreach (var next in children[node])
				{
					queue.Enqueue(next);
					dists.Enqueue(dist + 1);
				}

				if (node != 1)
				{
					queue.Enqueue(parents[node - 2]);
					dists.Enqueue(dist + 1);
				}
			}

			return ans;
		}
	}
}
