using Combinatorics.Collections;
using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1.HackerRank
{
	//https://arxiv.org/pdf/1101.1266.pdf

	public class HackerRank9
	{
		public void Go()
		{

		}

		public class Task
		{
			public Graph Graph;
			public int[] Weights;
		}

		public class Graph
		{
			public int N;
			public ulong[] Edges;
			public ulong AllVertices;
		}

		public struct Solution
		{
			public int WeightSum;
			public List<int[]> Subgraphs;
		}

		public static ulong[] ToAdjMatrix(int N, int[][] edgesList)
		{
			var result = new ulong[N];

			foreach (var edge in edgesList)
			{
				result[edge[0]] |= 1ul << edge[1];
				result[edge[1]] |= 1ul << edge[0];
			}

			return result;
		}

		public static Tuple<int, ulong> Solve_BronKerbosch(Task task)
		{
			task.Graph.AllVertices = (1ul << task.Graph.N) - 1;

			if (task.Weights.Sum() == 0)
				return Tuple.Create(0, Solve_ZeroWeights(task.Graph));

			var nonZeroGraph = ToComplement(task.Graph);

			var zeroWeights = 0ul;
			for (var i = 0; i < nonZeroGraph.N; i++)
				if (task.Weights[i] == 0)
				{
					zeroWeights |= 1ul << i;
					for (var j = 0; j < nonZeroGraph.N; j++)
					{
						nonZeroGraph.Edges[i] &= ~(1ul << j);
						nonZeroGraph.Edges[j] &= ~(1ul << i);
					}
				}

			var N = task.Graph.N;

			var bestWeight = 0;
			var bestGraphs = new List<ulong> { 0ul };

			foreach (var clique in GetMaxClique_BronKerbosch(nonZeroGraph, 0, nonZeroGraph.AllVertices, zeroWeights))
			{
				var weight = 0;
				for (var i = 0; i < N; i++)
					if ((clique & (1ul << i)) > 0)
						weight += task.Weights[i];

				if (weight >= bestWeight)
				{
					if (weight > bestWeight)
					{
						bestWeight = weight;
						bestGraphs = new List<ulong>();
					}

					bestGraphs.Add(clique);
				}
			}

			var count = 0ul;
			foreach (var clique in bestGraphs)
			{
				var used = task.Graph.AllVertices & ~zeroWeights;

				for (var i = 0; i < N; i++)
					if ((clique & (1ul << i)) > 0)
						used |= task.Graph.Edges[i];

				var zeros = Solve_ZeroWeights_Rec(task.Graph, used);

				count += zeros;
			}

			return Tuple.Create(bestWeight, count);
		}

		public static ulong Solve_ZeroWeights(Graph graph)
		{
			var N = graph.N;

			graph.AllVertices = (1ul << N) - 1;

			var result = Solve_ZeroWeights_Rec(graph, 0ul);
			return result;
		}

		private static int BitCount(ulong i)
		{
			i = i - ((i >> 1) & 0x5555555555555555UL);
			i = (i & 0x3333333333333333UL) + ((i >> 2) & 0x3333333333333333UL);
			return (int)(unchecked(((i + (i >> 4)) & 0xF0F0F0F0F0F0F0FUL) * 0x101010101010101UL) >> 56);
		}

		public static ulong Solve_ZeroWeights_Rec(Graph graph, ulong used)
		{
			var i = -1;
			for (var j = 0; j < graph.N; j++)
				if ((used & (1ul << j)) == 0)
				{
					i = j;
					break;
				}

			if (i == -1)
				return 1;

			var degree = BitCount(~used & graph.Edges[i]);
			if (degree == 0)
			{
				used |= 1ul << i;
				var result = 2ul * Solve_ZeroWeights_Rec(graph, used);
				return result;
			}
			else
			{
				used |= 1ul << i;
				var s1 = Solve_ZeroWeights_Rec(graph, used);

				used |= graph.Edges[i];
				var s2 = Solve_ZeroWeights_Rec(graph, used);

				return s1 + s2;
			}
		}

		public static IEnumerable<ulong> GetMaxClique_BronKerbosch(Graph graph, ulong C, ulong P, ulong S)
		{
			if (P == 0ul && S == 0ul)
			{
				yield return C;
			}
			else
			{
				for (var pi = 0; pi < graph.N; pi++)
				{
					var i = 1ul << pi;

					if ((P & i) > 0)
					{
						foreach (var rec in GetMaxClique_BronKerbosch(
												graph,
												C | i,
												P & graph.Edges[pi],
												S & graph.Edges[pi]))
							yield return rec;

						P &= ~i;
						S |= i;
					}
				}
			}
		}

		public static Graph ToComplement(Graph graph)
		{
			var allVertices = (1ul << graph.N) - 1;

			var edges = new ulong[graph.N];
			for (var i = 0; i < graph.N; i++)
				edges[i] = allVertices & ~graph.Edges[i] & ~(1ul << i);

			return new Graph
			{
				N = graph.N,
				Edges = edges,
				AllVertices = allVertices,
			};
		}

		public static Solution EnumerateMaxIndependentSets_Brute(Task task)
		{
			var vertices = Enumerable.Range(0, task.Graph.N).ToArray();

			var bestWeight = 0;
			var bestGraphs = new List<int[]> { new int[0] };

			for (var size = 1; size <= task.Graph.N; size++)
				foreach (var comb in new Combinations<int>(vertices, size, GenerateOption.WithoutRepetition))
				{
					if (CheckSolution(task.Graph, comb))
					{
						var weight = comb.Sum(i => task.Weights[i]);
						if (weight > bestWeight)
						{
							bestWeight = weight;
							bestGraphs.Clear();
							bestGraphs.Add(comb.ToArray());
						}
						else if (weight == bestWeight)
						{
							bestGraphs.Add(comb.ToArray());
						}
					}
				}

			return new Solution
			{
				WeightSum = bestWeight,
				Subgraphs = bestGraphs,
			};
		}

		private static bool CheckSolution(Graph graph, IList<int> subgraphVertices)
		{
			foreach (var v1 in subgraphVertices)
				foreach (var v2 in subgraphVertices)
					if ((graph.Edges[v1] & (1ul << v2)) > 0)
						return false;
			return true;
		}
	}
}
