using ConsoleApplication1.DataTypes;
using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApplication1.Helpers.GraphHelper;

namespace ConsoleApplication1.HackerRank
{
	// https://www.hackerrank.com/challenges/synchronous-shopping

	class HackerRank30
	{
		public void Go()
		{
			Example4();
		}

		public void Performance()
		{
			const int n = 1000;

			var rnd = new Random(1337);
			var edges = RandomGenerator.RandomUndirectedGraph_Edges(rnd, n, 2000);
			var weights = RandomGenerator.RandomIntArray(rnd, edges.Length, 1, 10000);

			for (var i = 0; i < edges.Length; i++)
				edges[i] = new[] { edges[i][0] + 1, edges[i][1] + 1, weights[i] };

			var T = ArrayHelper.Create(n, i => new int[0]);

			var shops = ArrayHelper.Create(n, i => i).Shuffle(rnd).Take(10).ToArray();
			for (var i = 0; i < shops.Length; i++)
				T[shops[i]] = new[] { i + 1 };

			Console.WriteLine(Solve(T, edges, n, 10));
		}

		public void Example4()
		{
			var input = @"6 10 3
2 1 2
1 3
0
2 1 3
1 2
1 3
1 2 572
4 2 913
2 6 220
1 3 579
2 3 808
5 3 298
6 1 927
4 5 171
1 5 671
2 5 463".Split(new[] { "\r", "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

			var NMK = input[0].Split().Select(int.Parse).ToArray();
			var n = NMK[0];
			var M = NMK[1];
			var K = NMK[2];
			var T = new int[n][];
			for (var i = 1; i <= n; i++)
				T[i - 1] = input[i].Split().Select(int.Parse).Skip(1).ToArray();

			var edges = new int[M][];
			for (var i = 1; i <= M; i++)
				edges[i - 1] = input[i + 6].Split().Select(int.Parse).ToArray();

			// 792
			Console.WriteLine(Solve(T, edges, n, K));
		}

		public void Example3()
		{
			Console.WriteLine(Solve(
				new[] { new[] { 2 }, new[] { 2 }, new[] { 1 }, new[] { 3, 4 }, new[] { 3, 4 }, new[] { 4 } },
				new[] { new[] { 5, 4, 646 }, new[] { 4, 1, 997 }, new[] { 2, 1, 881 }, new[] { 2, 6, 114 }, new[] { 3, 1, 46 } },
				6, 4));
		}

		public void Example2()
		{
			// 40
			Console.WriteLine(Solve(
				new[] { new[] { 1 }, new[] { 2 }, new[] { 3 }, new[] { 4 }, new[] { 5 } },
				new[] { new[] { 1, 2, 10 }, new[] { 1, 3, 10 }, new[] { 1, 4, 10 }, new[] { 4, 5, 10 } },
				5, 5));
		}

		public void Example1()
		{
			Console.WriteLine(Solve(
				new[] { new[] { 1 }, new[] { 2 }, new[] { 3 }, new[] { 4 }, new[] { 5 } },
				new[] { new[] { 1, 2, 10 }, new[] { 1, 3, 10 }, new[] { 2, 4, 10 }, new[] { 3, 5, 10 }, new[] { 4, 5, 10 } },
				5, 5));
		}

		public static long Solve(int[][] T, int[][] edges, int n, int K)
		{
			var adjEdges = ToAdjacencyList_UnDirected_WithWeights(edges, n + 1);

			var maskSize = 1 << K;

			var stats = ArrayHelper.Create(n + 1, i => ArrayHelper.Create(maskSize, j => long.MaxValue));
			stats[1][CalcMask(T[0])] = 0;

			var nodeMasks = ArrayHelper.Create(n + 1, i => i == 0 ? 0u : CalcMask(T[i - 1]));

			var queue = new Queue<int>();
			queue.Enqueue(1);

			while (queue.Count > 0)
			{
				var node = queue.Dequeue();

				foreach (var nodeEdge in adjEdges[node])
				{
					var nextNode = nodeEdge[1];
					var weight = nodeEdge[2];

					var nextNodeMask = nodeMasks[nextNode];

					var doExplore = false;
					for (var mask = 0u; mask < maskSize; mask++)
						if (stats[node][mask] < long.MaxValue)
						{
							if (stats[nextNode][mask | nextNodeMask] > stats[node][mask] + weight)
							{
								stats[nextNode][mask | nextNodeMask] = stats[node][mask] + weight;
								doExplore = true;
							}
						}

					if (doExplore)
						queue.Enqueue(nextNode);
				}
			}

			{
				var nStats = stats[n];
				var best = long.MaxValue;
				var full = maskSize - 1;
				for (var i = 0; i < nStats.Length; i++)
				{
					if (i == full)
						best = Math.Min(best, nStats[i]);

					for (var j = i + 1; j < nStats.Length; j++)
						if ((i | j) == full)
							best = Math.Min(best, Math.Max(nStats[i], nStats[j]));
				}

				return best;
			}
		}

		public static long Solve_Bad(int[][] T, int[][] edges, int n, int K)
		{
			var adjEdges = ToAdjacencyList_UnDirected_WithWeights(edges, n + 1);

			var maskSize = 1 << K;

			var stats = ArrayHelper.Create(n + 1, i => ArrayHelper.Create(maskSize, j => long.MaxValue));
			stats[1][CalcMask(T[0])] = 0;

			var visited = new bool[n + 1];
			visited[1] = true;

			var visitedMask = CalcMask(T[0]);

			var front = new Queue<int>();
			foreach (var edge in adjEdges[1])
				if (!visited[edge[1]])
					front.Enqueue(edge[1]);

			while (front.Count > 0)
			{
				var x = front.Dequeue();
				if (visited[x]) continue;

				var xMask = CalcMask(T[x - 1]);

				foreach (var xEdge in adjEdges[x])
				{
					var a = xEdge[1];
					var w = xEdge[2];

					if (!visited[a])
						front.Enqueue(a);
					else
					{
						if (!visited[x])
						{
							visited[x] = true;

							for (var mask = 0u; mask < maskSize; mask++)
								if (stats[a][mask] < long.MaxValue)
									stats[x][mask | xMask] = Math.Min(stats[x][mask | xMask], stats[a][mask] + w);

							if ((xMask & visitedMask) > 0)
							{
								// В X есть новый тип
							}

							visitedMask |= xMask;
						}
						else
						{

						}
					}
				}
			}

			{
				var nStats = stats[n];
				var best = long.MaxValue;
				var full = maskSize - 1;
				for (var i = 0; i < nStats.Length; i++)
						for (var j = i + 1; j < nStats.Length; j++)
							if ((i | j) == full)
							{
								best = Math.Min(best, Math.Max(nStats[i], nStats[j]));
							}

				return best;
			}
		}

		public static List<int[]>[] ToAdjacencyList_UnDirected_WithWeights(int[][] edges, int verticesCount)
		{
			var result = new List<int[]>[verticesCount];

			for (var i = 0; i < result.Length; i++)
				result[i] = new List<int[]>();

			foreach (var edge in edges)
			{
				result[edge[0]].Add(edge);
				result[edge[1]].Add(new[] { edge[1], edge[0], edge[2] });
			}

			return result;
		}

		public static uint CalcMask(int[] types)
		{
			var result = 0u;
			foreach (var type in types)
				result |= 1u << (type - 1);
			return result;
		}
	}
}
