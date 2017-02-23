using System.Collections.Generic;

namespace ConsoleApplication1.DataTypes
{
	public static class ShortestPathHelper
	{
		public static long[] Dijkstra(int start, List<int[]>[] adjEdges, bool[] canUseNodes)
		{
			var n = adjEdges.Length;

			var paths = new long[n];
			for (var i = 0; i < n; i++)
				paths[i] = long.MaxValue;

			paths[start] = 0;

			var indexInHeap = new int[n];

			var visited = new bool[n];

			var front = new BinaryHeap<int>(
				(x, y) => paths[x].CompareTo(paths[y]),
				(vi, i) => indexInHeap[vi] = i);

			front.Insert(start);

			while (front.Count > 0)
			{
				var u = front.Min;

				front.DeleteMin();

				if (visited[u] || !canUseNodes[u])
					continue;

				visited[u] = true;

				foreach (var edge in adjEdges[u])
				{
					var to = edge[1];

					if (visited[to] || !canUseNodes[to])
						continue;

					var w = paths[u] + edge[2];

					if (w < paths[to])
					{
						if (front.Count > 0 && front[indexInHeap[to]] == to)
							front.Remove(indexInHeap[to]);
						paths[to] = w;
					}

					front.Insert(to);
				}
			}

			return paths;
		}
	}
}
