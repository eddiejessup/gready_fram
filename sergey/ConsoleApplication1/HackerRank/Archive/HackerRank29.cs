using ConsoleApplication1.DataTypes;
using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApplication1.Helpers.GraphHelper;

namespace ConsoleApplication1.HackerRank
{
	// https://www.hackerrank.com/contests/world-codesprint-5/challenges/balanced-forest

	public class HackerRank29
	{
		public void Go()
		{
			//Console.WriteLine(Solve(5, new[] {
			//	new[] {1, 2},
			//	new[] {1, 3},
			//	new[] {3, 5},
			//	new[] {4, 1},
			//}, new ulong[] { 1, 2, 2, 1, 1 }));
			Compare();
		}

		public static void Performance()
		{
			var rnd = new Random(1337);
			var n = 20000;
			var edges = RandomGenerator.RandomTree_Edges(rnd, n, 4, true).Select(e =>
			{
				e[0]++; e[1]++; return e;
			}).ToArray();
			var coins = RandomGenerator.RandomUlongArray(rnd, n, 1, 1000);

			var timer = Stopwatch.StartNew();
			Console.WriteLine(Solve(n, edges, coins));
			Console.WriteLine("Solving time " + timer.ElapsedMilliseconds);
		}

		public static void Compare()
		{
			var rnd = new Random(1337);
			for (var t = 0; t < 10000; t++)
			{
				var n = rnd.Next(2, 5);
				var edges = RandomGenerator.RandomTree_Edges(rnd, n, 4, true).Select(e =>
				{
					e[0]++; e[1]++; return e;
				}).ToArray();
				var coins = RandomGenerator.RandomUlongArray(rnd, n, 1, 50);

				var brute = SolveBrute(n, edges, coins);
				var actual = Solve(n, edges, coins);

				if (actual != brute)
				{
					Console.WriteLine(new { t, n, brute, actual });
					throw new InvalidOperationException();
				}
			}
		}

		public static ulong Solve(int n, int[][] edges, ulong[] coins)
		{
			if (n == 1) return ulong.MaxValue;

			var adj = ToDirectedTree(edges, n + 1);
			var head = edges[0][0];

			var sumsBelow = new ulong[n + 1];

			var stack = new Stack<int>(n + 1);
			var ignoreSums = new bool[n + 1];

			foreach (var node in Traverse_Tree_PostOrder_NonRec(head, v => adj[v]))
				sumsBelow[node] = coins[node - 1] + adj[node].Select(v => sumsBelow[v]).Sum();

			var total = sumsBelow[head];

			var sumsSet = new LeftLeaningRedBlackTree<ulong, int>((x, y) => x.CompareTo(y), (x, y) => x.CompareTo(y));
			for (var i = 1; i <= n; i++)
				sumsSet.Add(sumsBelow[i], i);

			var best = ulong.MaxValue;

			var path = new List<int>(n + 1) { head };
			var notInA = new bool[n + 1];
			notInA[head] = true;

			foreach (var edgeInfo in Traverse_Dfs_WithEvents(head, v => adj[v]))
			{
				var aNode = edgeInfo.Child;

				if (edgeInfo.IsDownNotUp)
				{
					path.Add(aNode);

					notInA[edgeInfo.Parent] = true;
					foreach (var otherChild in adj[edgeInfo.Parent])
						if (otherChild != edgeInfo.Child)
							foreach (var node in Traverse_Tree_Dfs(otherChild, v => adj[v], stack))
								notInA[node] = true;
				}
				else
				{
					path.RemoveAt(path.Count - 1);

					notInA[edgeInfo.Parent] = false;
					foreach (var otherChild in adj[edgeInfo.Parent])
						if (otherChild != edgeInfo.Child)
							foreach (var node in Traverse_Tree_Dfs(otherChild, v => adj[v], stack))
								notInA[node] = false;

					continue;
				}

				var a = sumsBelow[aNode];

				if (a == total - a)
					best = Math.Min(best, a);

				foreach (var pathNode in path)
					if (pathNode != aNode)
					{
						var sum = sumsBelow[pathNode];
						sumsSet.Remove(sum, pathNode);
						sumsSet.Add(sum - a, pathNode);
					}

				var searchme = new List<ulong>(3) { a };
				if (total > 2 * a) searchme.Add(total - 2 * a);
				if (total != a && (total - a) % 2 == 0) searchme.Add((total - a) / 2);

				foreach (var sum in searchme)
				{
					if (sumsSet.GetValuesForKey(sum).Any(i => notInA[i]))
					{
						best = CalcBest(best, a, sum, total - a - sum);
					}
				}

				foreach (var pathNode in path)
					if (pathNode != aNode)
					{
						var sum = sumsBelow[pathNode];
						sumsSet.Remove(sum - a, pathNode);
						sumsSet.Add(sum, pathNode);
					}
			}

			return best;
		}

		public static ulong Solve_3(int n, int[][] edges, ulong[] coins)
		{
			var head = edges[0][0];
			var adj = ToAdjacencyList_UnDirected(edges, n + 1);

			var stack = new Stack<int>(n + 1);
			var visited = new bool[n + 1];

			var sumsBelow = new ulong[n + 1];
			foreach (var node in Traverse_Tree_PostOrder_Visited(head, v => adj[v], n + 1, stack, visited))
				sumsBelow[node] = coins[node - 1] + adj[node].Select(v => sumsBelow[v]).Sum();

			var total = sumsBelow[head];

			var best = ulong.MaxValue;

			foreach (var edge in edges)
			{
				var aNode = edge[0];
				var bNode = edge[1];

				var a = sumsBelow[bNode];
				if (a == total - a)
					best = Math.Min(best, a);

				Array.Clear(visited, 0, visited.Length);
				visited[bNode] = true;
				visited[aNode] = true;

				var sumsB = new ulong[n + 1];
				foreach (var cNode in Traverse_Tree_PostOrder_Visited(aNode, v => adj[v], n + 1, stack, visited))
				{
					var c = coins[cNode - 1];
					foreach (var d in adj[cNode])
						c += sumsB[d];

					sumsB[cNode] = c;

					best = CalcBest(best, a, total - a - c, c);
				}
			}

			return best;
		}

		public static IEnumerable<int> Traverse_Tree_PostOrder_Visited(int head, Func<int, IList<int>> expand, int n, Stack<int> stack, bool[] visited)
		{
			stack.Clear();
			stack.Push(head);

			visited[head] = true;

			while (stack.Count > 0)
			{
				var next = stack.Peek();

				var children = expand(next);

				var finishedSubtrees = false;
				var isLeaf = true;
				foreach (var child in children)
					if (!visited[child])
					{
						isLeaf = false;
						if (child.Equals(head))
						{
							finishedSubtrees = true;
							break;
						}
					}

				if (finishedSubtrees || isLeaf)
				{
					stack.Pop();
					yield return next;
					head = next;
				}
				else
				{
					foreach (var child in children)
						if (!visited[child])
						{
							stack.Push(child);
							visited[child] = true;
						}
				}
			}
		}

		public static ulong Solve_2(int n, int[][] edges, ulong[] coins)
		{
			if (n == 1) return ulong.MaxValue;

			var adj = ToDirectedTree(edges, n + 1);
			var head = edges[0][0];

			var sumsBelow = new ulong[n + 1];

			var stack = new Stack<int>(n + 1);

			for (var node = 1; node <= n; node++)
			{
				var sums = 0ul;

				foreach (var child in Traverse_Tree_Dfs(node, v => adj[v], stack))
					sums += coins[child - 1];

				sumsBelow[node] = sums;
			}

			var total = sumsBelow[head];

			var best = ulong.MaxValue;

			var path = new List<int> { head };
			foreach (var edgeInfo in GraphHelper.Traverse_Dfs_WithEvents(head, v => adj[v]))
			{
				var nodeAParent = edgeInfo.Parent;
				var nodeA = edgeInfo.Child;

				if (edgeInfo.IsDownNotUp) path.Add(nodeA);
				else
				{
					path.RemoveAt(path.Count - 1);
					continue;
				}

				var a = sumsBelow[nodeA];

				if (a == total - a)
					best = Math.Min(best, a);

				foreach (var nodeB in Traverse_Tree_Dfs(nodeA, v => adj[v], stack))
					if (nodeB != nodeA)
					{
						var b = sumsBelow[nodeB];
						best = CalcBest(best, b, a - b, total - a);
					}

				foreach (var nodeB in path)
					if (nodeB != nodeA)
					{
						var b = sumsBelow[nodeB];
						best = CalcBest(best, a, b - a, total - b);
					}

				for (var pi = 0; pi < path.Count - 1; pi++)
				{
					foreach (var nodeB in Traverse_Tree_Dfs_With_Forbidden(path[pi], v => adj[v], path[pi + 1], stack))
						if (nodeB != nodeA && nodeB != path[pi])
						{
							var b = sumsBelow[nodeB];
							best = CalcBest(best, a, b, total - a - b);
						}
				}
			}

			return best;
		}

		public static ulong Solve_N2(int n, int[][] edges, ulong[] coins)
		{
			if (n == 1) return ulong.MaxValue;

			var adj = ToDirectedTree(edges, n + 1);
			var head = edges[0][0];

			var stack = new Stack<int>(n + 1);

			var sumsBelow = new ulong[n + 1];

			for (var node = 1; node <= n; node++)
			{
				var sums = 0ul;
				foreach (var child in Traverse_Tree_Dfs(node, v => adj[v], stack))
					sums += coins[child - 1];
				sumsBelow[node] = sums;
			}

			var total = sumsBelow[head];

			var best = ulong.MaxValue;

			var path = new List<int> { head };
			foreach (var edgeInfo in GraphHelper.Traverse_Dfs_WithEvents(head, v => adj[v]))
			{
				var parent = edgeInfo.Parent;
				var child = edgeInfo.Child;

				if (edgeInfo.IsDownNotUp) path.Add(child);
				else
				{
					path.RemoveAt(path.Count - 1);
					continue;
				}

				var sumChild = sumsBelow[child];
				var sumParent = total - sumChild;

				if (sumChild == sumParent)
					best = Math.Min(best, sumChild);

				foreach (var subParent in Traverse_Tree_Dfs(child, v => adj[v], stack))
					foreach (var subChild in adj[subParent])
					{
						var sumSubChild = sumsBelow[subChild];
						var sumSubParent = sumChild - sumSubChild;

						best = CalcBest(best, sumParent, sumSubChild, sumSubParent);
					}

				for (var pi = 0; pi < path.Count - 1; pi++)
				{
					var pathNode = path[pi];
					var nextPath = path[pi + 1];

					foreach (var subParent in Traverse_Tree_Dfs_With_Forbidden(pathNode, v => adj[v], nextPath, stack))
						foreach (var subChild in adj[subParent])
							if (subChild != nextPath)
							{
								var sumSubChild = sumsBelow[subChild];
								var sumSubParent = sumParent - sumSubChild;

								best = CalcBest(best, sumChild, sumSubChild, sumSubParent);
							}
				}
			}

			return best;
		}

		public static IEnumerable<int> Traverse_Tree_Dfs(int start, Func<int, IEnumerable<int>> expand, Stack<int> stack)
		{
			stack.Clear();
			stack.Push(start);

			while (stack.Count > 0)
			{
				var node = stack.Pop();

				yield return node;

				foreach (var child in expand(node))
					stack.Push(child);
			}
		}

		public static IEnumerable<int> Traverse_Tree_Dfs_With_Forbidden(int start, Func<int, IEnumerable<int>> expand, int forbidden, Stack<int> stack)
		{
			stack.Clear();
			stack.Push(start);

			while (stack.Count > 0)
			{
				var node = stack.Pop();

				if (node == forbidden)
					continue;

				yield return node;

				foreach (var child in expand(node))
					stack.Push(child);
			}
		}

		public static ulong CalcBest(ulong best, ulong a, ulong b, ulong c)
		{
			if (a == b && c < a)
				return Math.Min(best, a - c);

			if (a == c && b < a)
				return Math.Min(best, a - b);

			if (b == c && a < b)
				return Math.Min(best, b - a);

			return best;
		}

		public static ulong SolveBrute(int n, int[][] edges, ulong[] coins)
		{
			var adj = GraphHelper.ToAdjacencyList_UnDirected(edges, n + 1);

			var best = ulong.MaxValue;

			for (var a = 1; a <= n; a++)
			{
				foreach (var b in adj[a].ToArray())
				{
					adj[a].Remove(b);
					adj[b].Remove(a);

					var sumA = GraphHelper.Traverse_Graph_Dfs(a, v => adj[v], n + 1).Select(v => coins[v - 1]).Sum();
					var sumB = GraphHelper.Traverse_Graph_Dfs(b, v => adj[v], n + 1).Select(v => coins[v - 1]).Sum();

					if (sumA == sumB)
						best = Math.Min(best, sumA);

					foreach (var c in GraphHelper.Traverse_Graph_Dfs(b, v => adj[v], n + 1))
					{
						foreach (var d in adj[c].ToArray())
						{
							adj[c].Remove(d);
							adj[d].Remove(c);

							var sumC = GraphHelper.Traverse_Graph_Dfs(c, v => adj[v], n + 1).Select(v => coins[v - 1]).Sum();
							var sumD = GraphHelper.Traverse_Graph_Dfs(d, v => adj[v], n + 1).Select(v => coins[v - 1]).Sum();

							best = CalcBest(best, sumA, sumC, sumD);

							adj[c].Add(d);
							adj[d].Add(c);
						}
					}

					adj[a].Add(b);
					adj[b].Add(a);
				}
			}

			return best;
		}

		public static ulong Solve_BadMemory(int n, int[][] edges, ulong[] coins)
		{
			if (n == 1) return ulong.MaxValue;

			var adj = ToDirectedTree(edges, n + 1);
			var head = edges[0][0];

			var sumsBelow = new ulong[n + 1];
			var isChild = new bool[n + 1, n + 1];

			for (var node = 1; node <= n; node++)
			{
				var sums = 0ul;

				foreach (var child in GraphHelper.Traverse_Tree_Dfs(node, v => adj[v]))
				{
					sums += coins[child - 1];

					isChild[node, child] = true;
				}

				sumsBelow[node] = sums;
			}

			var total = sumsBelow[head];

			var best = ulong.MaxValue;

			// Работает, так как ребра отсортированы, хехе
			for (var ai = 0; ai < edges.Length; ai++)
			{
				var aParent = edges[ai][0];
				var aChild = edges[ai][1];

				if (sumsBelow[aChild] == total - sumsBelow[aChild])
					best = Math.Min(best, sumsBelow[aChild]);

				for (var bi = ai + 1; bi < edges.Length; bi++)
				{
					var bParent = edges[bi][0];
					var bChild = edges[bi][1];

					if (isChild[aChild, bParent])
						best = CalcBest(best, sumsBelow[bChild], total - sumsBelow[aChild], sumsBelow[aChild] - sumsBelow[bChild]);

					else
						best = CalcBest(best, sumsBelow[bChild], sumsBelow[aChild], total - sumsBelow[aChild] - sumsBelow[bChild]);
				}
			}

			return best;
		}

		public static void Check_Sums_Below()
		{
			var rnd = new Random(1337);
			for (var t = 0; t < 1000; t++)
			{
				var n = rnd.Next(2, 10);
				var edges = RandomGenerator.RandomTree_Edges(rnd, n, 4, true).Select(e =>
				{
					e[0]++; e[1]++; return e;
				}).ToArray();
				var coins = RandomGenerator.RandomUlongArray(rnd, n, 1, 100);

				var bruteSumsBelow = new ulong[n + 1];
				{
					var adj = GraphHelper.ToAdjacencyList(edges, n + 1);
					for (var node = 1; node <= n; node++)
						bruteSumsBelow[node] = GraphHelper.Traverse_Graph_Dfs(node, v => adj[v], n + 1).Select(v => coins[v - 1]).Sum();
				}

				var actualSumsBelow = new ulong[n + 1];
				{
					var adj = ToDirectedTree(edges, n + 1);
					var head = edges[0][0];

					for (var node = 1; node <= n; node++)
						actualSumsBelow[node] = GraphHelper.Traverse_Tree_Dfs(node, v => adj[v]).Select(v => coins[v - 1]).Sum();
				}

				if (!bruteSumsBelow.SequenceEqual(actualSumsBelow))
				{
					Console.WriteLine(new { t, n, brute = bruteSumsBelow.Join(), actual = actualSumsBelow.Join() });
					throw new InvalidOperationException();
				}
			}
		}
	}
}
