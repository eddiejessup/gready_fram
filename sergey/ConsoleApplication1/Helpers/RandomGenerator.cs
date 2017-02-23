using ConsoleApplication1.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication1.Helpers
{
	public class RandomGenerator
	{
		public static string RandomString(Random rnd, string alphabet, int len)
		{
			var result = new char[len];

			for (var i = 0; i < result.Length; i++)
				result[i] = alphabet[rnd.Next(alphabet.Length)];

			return new string(result);
		}

		public static ulong[] RandomUlongArray(Random rnd, int len, int minInclusive, int maxExclusive)
		{
			var result = new ulong[len];

			for (var i = 0; i < result.Length; i++)
				result[i] = (ulong)rnd.Next(minInclusive, maxExclusive);

			return result;
		}

		public static long[] RandomLongArray(Random rnd, int len, int minInclusive, int maxInclusive)
		{
			var result = new long[len];

			for (var i = 0; i < result.Length; i++)
				result[i] = rnd.Next(minInclusive, maxInclusive + 1);

			return result;
		}

		public static int[] RandomIntArray(Random rnd, int len, int minInclusive, int maxInclusive)
		{
			var result = new int[len];

			for (var i = 0; i < result.Length; i++)
				result[i] = rnd.Next(minInclusive, maxInclusive + 1);

			return result;
		}

		public static int[][] RandomUndirectedGraph_Edges(Random rnd, int verticesCount, int edgesCount)
		{
			var edges = RandomTree_Edges(rnd, verticesCount, 3, true).ToList();

			var adj = ArrayHelper.Create(verticesCount, i => new HashSet<int>());

			foreach (var edge in edges)
			{
				adj[edge[0]].Add(edge[1]);
				adj[edge[1]].Add(edge[0]);
			}

			while (edges.Count < edgesCount)
			{
				var a = rnd.Next(0, verticesCount);
				var b = rnd.Next(0, verticesCount);

				if (a != b && !adj[a].Contains(b))
				{
					adj[a].Add(b);
					adj[b].Add(a);
					edges.Add(new[] { a, b });
				}
			}

			return edges.ToArray();
		}

		public static int[][] RandomTree_Edges(Random rnd, int verticesCount, int maxChildCount, bool doPermutation = true)
		{
			var vertices = new int[verticesCount];
			for (var i = 0; i < verticesCount; i++)
				vertices[i] = i;

			var edges = new List<int[]>();

			var childCount = new int[verticesCount];

			var added = new WeightedSet<int>();
			added.Add(0, 1d);

			for (var i = 1; i < verticesCount; i++)
			{
				if (added.Count == 0) break;

				var parent = added.RandomElement(rnd);

				edges.Add(new[] { parent, i });
				childCount[parent]++;

				added.Add(i, 1d);
				added.MultiplyWeight(parent, 0.5);

				if (childCount[parent] == maxChildCount)
					added.Remove(parent);
			}

			if (doPermutation)
			{
				var permutation = vertices.ToArray().Shuffle(rnd);
				foreach (var edge in edges)
				{
					edge[0] = permutation[edge[0]];
					edge[1] = permutation[edge[1]];
				}
			}

			return edges.ToArray();
		}
	}
}
