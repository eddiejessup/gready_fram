using ConsoleApplication1.DataTypes;
using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ConsoleApplication1.HackerRank
{
	class HackerRank24
	{
		public void Go()
		{
			Compare();
		}

		public static void Performance()
		{
			const int n = 10000;

			var rnd = new Random(1337);
			var edges = RandomGenerator.RandomTree_Edges(rnd, n, 3);
			var k = rnd.Next(0, n + 1);

			var timer = Stopwatch.StartNew();
			Console.WriteLine(Solve_N2(n, k, edges));
			Console.WriteLine("Solving: " + timer.ElapsedMilliseconds);
		}

		public static void Example1()
		{
			Console.WriteLine(Solve(5, 2, new[]
			{
				new[] {2, 1},
				new[] {2, 0},
				new[] {0, 3},
				new[] {0, 4},
			}));
		}

		public static void Show_Traverse_Dfs_WithEvents()
		{
			// http://i.stack.imgur.com/0SEab.png

			var edges = new[]
			{
				new[] { 1, 2 },
				new[] { 1, 3 },
				new[] { 1, 4 },
				new[] { 1, 5 },
				new[] { 2, 6 },
				new[] { 2, 7 },
				new[] { 2, 8 },
				new[] { 4, 9 },
				new[] { 5, 10 },
				new[] { 5, 11 },
				new[] { 9, 12 },
			};
			var adj = GraphHelper.ToAdjacencyList(edges, 13);

			foreach (var edgeInfo in GraphHelper.Traverse_Dfs_WithEvents(1, v => adj[v]))
				if (edgeInfo.IsDownNotUp)
					Console.WriteLine($"Down: {edgeInfo.Parent} - {edgeInfo.Child}");
				else
					Console.WriteLine($"Up  : {edgeInfo.Parent} - {edgeInfo.Child}");
		}

		public static void Show_PostOrder_NonRec_Path()
		{
			// http://i.stack.imgur.com/0SEab.png

			var edges = new[]
			{
				new[] { 1, 2 },
				new[] { 1, 3 },
				new[] { 1, 4 },
				new[] { 1, 5 },
				new[] { 2, 6 },
				new[] { 2, 7 },
				new[] { 2, 8 },
				new[] { 4, 9 },
				new[] { 5, 10 },
				new[] { 5, 11 },
				new[] { 9, 12 },
			};
			var adj = GraphHelper.ToAdjacencyList(edges, 13);

			foreach (var node in GraphHelper.Traverse_Tree_PostOrder_NonRec(1, v => adj[v]))
				Console.WriteLine(node);
		}

		public static void Compare()
		{
			var rnd = new Random(1337);
			for (var t = 0; t < 100000; t++)
			{
				var n = rnd.Next(1, 10);

				var edges = RandomGenerator.RandomTree_Edges(rnd, n, 3);
				var k = rnd.Next(0, n + 1);

				var brute = Solve_N2(n, k, edges);
				var actual = Solve(n, k, edges);

				if (actual != brute)
				{
					Console.WriteLine(new { n, k, brute, actual });
					Console.WriteLine(GraphHelper.ToString_AdjList(GraphHelper.ToAdjacencyList(edges, n)));
					throw new InvalidOperationException();
				}
			}
		}

		public static ulong Solve(int n, int k, int[][] edges)
		{
			foreach (var edge in edges)
			{
				edge[0]++;
				edge[1]++;
			}

			var adj = GraphHelper.ToAdjacencyList(edges, n + 1);
			var bit = new BinaryIndexedTree(n);

			var notHeads = new bool[n + 1];
			notHeads[0] = true;
			foreach (var edge in edges)
				notHeads[edge[1]] = true;

			var head = notHeads.FirstIndexOf(nh => !nh);

			var count = Solve_Bit_Dfs(bit, adj, head, n, k);

			return (ulong)count;
		}

		public static long Solve_Bit_Dfs(BinaryIndexedTree bit, List<int>[] adj, int node, int n, int k)
		{
			var count = bit.Get(Math.Max(1, node - k), Math.Min(n, node + k));
			bit.SetDiff(node, 1);
			foreach (var child in adj[node])
				count += Solve_Bit_Dfs(bit, adj, child, n, k);
			bit.SetDiff(node, -1);
			return count;
		}

		public static ulong Solve_LLRBT(int n, int k, int[][] edges)
		{
			var adj = GraphHelper.ToAdjacencyList(edges, n);

			var notHeads = new bool[n];
			foreach (var edge in edges)
				notHeads[edge[1]] = true;

			var head = notHeads.FirstIndexOf(nh => !nh);

			var count = 0ul;

			var counts = new ulong[n];

			var tree = new LeftLeaningRedBlackTree_WithEvents<int>(
				(x, y) => x.CompareTo(y),
				v => counts[v.Key] = 1ul + (v.Left == null ? 0ul : counts[v.Left.Key]) + (v.Right == null ? 0ul : counts[v.Right.Key]));
			tree.Add(head);

			foreach (var edgeInfo in GraphHelper.Traverse_Dfs_WithEvents(head, v => adj[v]))
			{
				var child = edgeInfo.Child;

				if (edgeInfo.IsDownNotUp)
				{
					var countLeft = GetCountLeft(tree, counts, child - k - 1);
					var countRight = GetCountLeft(tree, counts, child + k);
					count += countRight - countLeft;
					tree.Add(child);
				}
				else
				{
					tree.Remove(child);
				}
			}

			return count;
		}

		public static ulong GetCountLeft(LeftLeaningRedBlackTree_WithEvents<int> tree, ulong[] counts, int val)
		{
			var cnt = 0ul;
			foreach (var node in tree.Search(n => n.Key == val ? (bool?)null : n.Key < val ? false : true))
				if (node.Key < val)
				{
					if (node.Left != null)
						cnt += counts[node.Left.Key] + 1;
					else if (node.Right == null)
						cnt += 1;
				}
				else if (node.Key == val)
				{
					cnt += 1;
					if (node.Left != null)
						cnt += counts[node.Left.Key];
				}

			return cnt;
		}

		public static ulong Solve_SortedSet(int n, int k, int[][] edges)
		{
			var adj = GraphHelper.ToAdjacencyList(edges, n);

			var notHeads = new bool[n];
			foreach (var edge in edges)
				notHeads[edge[1]] = true;

			var head = notHeads.FirstIndexOf(nh => !nh);

			var count = 0ul;

			var stats = new SortedSet<int>();
			stats.Add(head);

			foreach (var edgeInfo in GraphHelper.Traverse_Dfs_WithEvents(head, v => adj[v]))
			{
				var child = edgeInfo.Child;

				if (edgeInfo.IsDownNotUp)
				{
					count += (ulong)stats.GetViewBetween(child - k, child + k).Count;
					stats.Add(child);
				}
				else
				{
					stats.Remove(child);
				}
			}

			return count;
		}

		public static ulong Solve_N2(int n, int k, int[][] edges)
		{
			var adj = GraphHelper.ToAdjacencyList(edges, n);

			var count = 0ul;

			for (var v1 = 0; v1 < adj.Length; v1++)
			{
				foreach (var v2 in GraphHelper.Traverse_Tree_Dfs(v1, v => adj[v]))
				{
					if (v1 != v2 && Math.Abs(v1 - v2) <= k)
					{
						count++;
					}
				}
			}

			return count;
		}
	}
}
