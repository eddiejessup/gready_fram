using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApplication1.Helpers.GraphHelper;
using static ConsoleApplication1.Helpers.CombinatoricsHelper;
using ConsoleApplication1.DataTypes;

namespace ConsoleApplication1.HackerRank
{
	class HackerRank45
	{
		public void Go()
		{
			Compare();
		}

		public static void Compare()
		{
			var rnd = new Random(1337);
			for (var t = 0; t < 100000; t++)
			{
				var n = rnd.Next(1, 10);
				var tree = RandomGenerator.RandomTree_Edges(rnd, n, 3);
				var W = RandomGenerator.RandomLongArray(rnd, n, 0, 5);
				var A = RandomGenerator.RandomLongArray(rnd, (int)W.Sum() + 1, 0, 5);

				foreach (var edge in tree) { edge[0]++; edge[1]++; };

				var brute = SolveBrute(n, W, A, tree);
				var actual = Solve(n, W, A, tree);

				if (actual.ASum != brute.ASum || actual.Count != brute.Count)
				{
					Console.WriteLine("W: " + W.Join());
					Console.WriteLine("A: " + A.Join());
					Console.WriteLine(ToString_AdjList(ToDirectedTree(tree, n + 1)));
					Console.WriteLine(new { t, brute, actual });
					throw new InvalidOperationException();
				}
			}
		}

		public static Solution Solve(int n, long[] W, long[] A, int[][] edges)
		{
			if (n == 1) return new Solution { ASum = (ulong)A[W[0]], Count = 1 };

			var adj = ToAdjacencyList_UnDirected(edges, n + 1);

			var solution = Solve_Rec_AllSubtrees(W, A, adj);

			return solution;
		}

		public static Solution Solve_Rec_AllSubtrees(long[] W, long[] A, List<int>[] adj)
		{
			var inner = adj.FirstIndexOf(l => l.Count > 1);

			if (inner < 0)
			{
				var leafs = adj.IndicesWhere(l => l.Count == 1).ToArray();
				if (leafs.Length != 2) throw new InvalidOperationException("HZ!!!");

				var w0 = W[leafs[0] - 1];
				var w1 = W[leafs[1] - 1];

				return new Solution
				{
					ASum = (ulong)A[w0] + (ulong)A[w1] + (ulong)A[w0 + w1],
					Count = 3,
				};
			}

			{
				var children = adj[inner];

				adj[inner] = new List<int>();
				foreach (var child in children) adj[child].Remove(inner);



				var childsDict = new List<SubtreeInfo>[children.Count];

				var asum = 0ul;
				var count = 0ul;

				for (var ci = 0; ci < children.Count; ci++)
				{
					var child = children[ci];

					var subtreeAdj = UnDirected_GetDirectedSubTree(adj, child);

					childsDict[ci] = new List<SubtreeInfo> { new SubtreeInfo { NodesCount = 0, NodesWeight = 0, WithRoot = true } };

					foreach (var sub in EnumerateSubTrees(child, W, A, subtreeAdj))
					{
						asum += (ulong)A[sub.NodesWeight];
						count++;

						if (sub.WithRoot)
							childsDict[ci].Add(sub);
					}
				}

				foreach (var selection in Selections(childsDict))
				{
					asum += (ulong)A[selection.Sum(c => c.NodesWeight) + W[inner - 1]];
					count++;
				}

				return new Solution { ASum = asum, Count = count };
			}
		}

		public struct Solution
		{
			public ulong Count;
			public ulong ASum;

			public decimal Ev => (decimal)ASum / Count;

			public override string ToString()
			{
				return new { asum = ASum, count = Count }.ToString();
			}
		}

		public struct SubtreeInfo
		{
			public int NodesCount;
			public long NodesWeight;
			public bool WithRoot;

			public override string ToString()
			{
				return new { size = NodesCount, weight = NodesWeight, withroot = WithRoot }.ToString();
			}
		}

		public static IEnumerable<SubtreeInfo> EnumerateSubTrees(int root, long[] W, long[] A, List<int>[] adj)
		{
			var children = adj[root];

			var childsDict = new List<SubtreeInfo>[children.Count];

			for (var ci = 0; ci < children.Count; ci++)
			{
				var child = children[ci];
				var childsList = new List<SubtreeInfo> { new SubtreeInfo { NodesCount = 0, NodesWeight = 0, WithRoot = true } };
				childsDict[ci] = childsList;

				foreach (var subtree in EnumerateSubTrees(child, W, A, adj))
				{
					yield return new SubtreeInfo { NodesCount = subtree.NodesCount, NodesWeight = subtree.NodesWeight, WithRoot = false };
					if (subtree.WithRoot)
						childsList.Add(subtree);
				}
			}

			foreach (var comb in Selections(childsDict))
			{
				var nodesCount = 1;
				var nodesWeight = W[root - 1];
				foreach (var subtree in comb)
				{
					nodesCount += subtree.NodesCount;
					nodesWeight += subtree.NodesWeight;
				}
				yield return new SubtreeInfo { NodesCount = nodesCount, NodesWeight = nodesWeight, WithRoot = true };
			}
		}

		public static int FindRoot(int[][] edges, int verticesCount)
		{
			var notRoot = new bool[verticesCount];
			notRoot[0] = true;
			foreach (var edge in edges)
				notRoot[edge[1]] = true;
			return notRoot.FirstIndexOf(f => !f);
		}

		public static Solution SolveBrute(int n, long[] W, long[] A, int[][] edges)
		{
			if (n == 1) return new Solution { ASum = (ulong)A[W[0]], Count = 1 };

			var adj = ToDirectedTree(edges, n + 1);

			var root = FindRoot(edges, n + 1);

			var sum = 0L;
			var count = 0;
			foreach (var subtree in EnumerateSubTrees(root, W, A, adj))
			{
				//Console.WriteLine(subtree);
				sum += A[subtree.NodesWeight];
				count++;
			}
			return new Solution { ASum = (ulong)sum, Count = (ulong)count };
		}

		public static decimal Solve(int n, string[] commands)
		{
			var W = commands[0].Split().Select(long.Parse).ToArray();
			var A = commands[1].Split().Select(long.Parse).ToArray();
			var edges = new int[n - 1][];
			for (var i = 0; i < edges.Length; i++)
				edges[i] = commands[i + 2].Split().Select(int.Parse).ToArray();

			return Solve(n, W, A, edges).Ev;
		}

		public static void Example5()
		{
			Console.WriteLine(Solve(1, @"1
0 1".SplitToLines()));
			Console.WriteLine("Expected : 1");
		}

		public static void Example4()
		{
			Console.WriteLine(Solve(5, @"1 2 3 4 5
0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15
1 2
1 3
2 4
3 5".SplitToLines()));
			Console.WriteLine("Expected : 6.26");
		}

		public static void Example3()
		{
			Console.WriteLine(Solve(5, @"1 2 3 4 5
0 1 2 2 4 8 6 7 8 11 6 4 6 2 9 13
1 2
1 3
2 4
3 5".SplitToLines()));
			Console.WriteLine("Expected : 5.6");
		}

		public static void Example2()
		{
			Console.WriteLine(Solve(3, @"1 2 3
0 1 2 3 4 5 6
1 2
1 3".SplitToLines()));
		}

		public static void Example1()
		{
			Console.WriteLine(Solve(3, @"1 2 3
0 1 2 3 4 5 6
1 2
2 3".SplitToLines()));
		}

		public static void Example6()
		{
			Console.WriteLine(Solve(3, @"1 2 3
0 1 2 3 4 5 6
1 2
3 1".SplitToLines()));
		}
	}
}
