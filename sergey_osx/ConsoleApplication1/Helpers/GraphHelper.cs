using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1.Helpers
{
	public static class GraphHelper
	{
		public struct TreeTraversalEdge<T>
		{
			public T Parent;
			public T Child;
			public bool IsDownNotUp;
		}

		public static IEnumerable<TreeTraversalEdge<T>> Traverse_Dfs_WithEvents<T>(T head, Func<T, IList<T>> expand)
		{
			var stack = new Stack<TreeTraversalEdge<T>>();

			stack.Push(new TreeTraversalEdge<T> { Parent = default(T), Child = head, IsDownNotUp = true });

			var isHead = true;

			while (stack.Count > 0)
			{
				var info = stack.Pop();

				if (!isHead && !info.IsDownNotUp)
				{
					yield return info;
					continue;
				}

				if (isHead)
					isHead = false;
				else if (info.IsDownNotUp)
					yield return info;

				foreach (var nextChild in expand(info.Child))
				{
					stack.Push(new TreeTraversalEdge<T> { Parent = info.Child, Child = nextChild, IsDownNotUp = false });
					stack.Push(new TreeTraversalEdge<T> { Parent = info.Child, Child = nextChild, IsDownNotUp = true });
				}
			}
		}

		// Нужно, если в expand возвращает все соседние вершины, не учитывая parent & child
		public static IEnumerable<int> Traverse_Tree_PostOrder_Visited(int head, Func<int, IList<int>> expand, int n)
		{
			var stack = new Stack<int>();
			stack.Push(head);

			var visited = new bool[n];
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

		// Time complexity = 3 * V
		public static IEnumerable<T> Traverse_Tree_PostOrder_NonRec<T>(T head, Func<T, IList<T>> expand) where T : IEquatable<T>
		{
			var stack = new Stack<T>();
			stack.Push(head);

			while (stack.Count > 0)
			{
				var next = stack.Peek();

				var children = expand(next);

				var finishedSubtrees = false;
				foreach (var child in children)
				{
					if (child.Equals(head))
					{
						finishedSubtrees = true;
						break;
					}
				}

				var isLeaf = children.Count == 0;

				if (finishedSubtrees || isLeaf)
				{
					stack.Pop();
					yield return next;
					head = next;
				}
				else
				{
					foreach (var child in children)
						stack.Push(child);
				}
			}
		}

		public static IEnumerable<T> Traverse_Tree_Bfs<T>(T start, Func<T, IEnumerable<T>> expand)
		{
			var queue = new Queue<T>();
			queue.Enqueue(start);

			while (queue.Count > 0)
			{
				var node = queue.Dequeue();

				yield return node;

				foreach (var child in expand(node))
					queue.Enqueue(child);
			}
		}

		// Time complexity = V
		public static IEnumerable<T> Traverse_Tree_Dfs<T>(T start, Func<T, IEnumerable<T>> expand)
		{
			var stack = new Stack<T>();
			stack.Push(start);

			while (stack.Count > 0)
			{
				var node = stack.Pop();

				yield return node;

				foreach (var child in expand(node))
					stack.Push(child);
			}
		}

		public static IEnumerable<int> Traverse_Graph_Dfs(int start, Func<int, IEnumerable<int>> expand, int nodes)
		{
			var stack = new Stack<int>();
			stack.Push(start);

			var visited = new bool[nodes];

			while (stack.Count > 0)
			{
				var node = stack.Pop();
				visited[node] = true;

				yield return node;

				foreach (var child in expand(node))
					if (!visited[child])
						stack.Push(child);
			}
		}

		public static IEnumerable<T> DFS_Rec<T>(HashSet<T> used, T node, Func<T, IEnumerable<T>> getChildren)
		{
			used.Add(node);

			foreach (var child in getChildren(node))
				if (!used.Contains(child))
					foreach (var res in DFS_Rec(used, child, getChildren))
						yield return res;

			yield return node;
		}

		public static IEnumerable<T> TopologicalSort_Tarjan<T>(IEnumerable<T> vertices, Func<T, IEnumerable<T>> getChildren)
		{
			var used = new HashSet<T>();

			var result = new Stack<T>();

			foreach (var outerV in vertices)
				if (!used.Contains(outerV))
					foreach (var v in DFS_Rec(used, outerV, getChildren))
						result.Push(v);

			return result;
		}

		public static Dictionary<T, List<T>> Reverse<T>(Dictionary<T, T[]> graph)
		{
			var result = new Dictionary<T, List<T>>();

			foreach (var kvp in graph)
			{
				var nodeFrom = kvp.Key;

				result.GetOrAddNew(nodeFrom);

				foreach (var nodeTo in kvp.Value)
					result.GetOrAddNew(nodeTo).Add(nodeFrom);
			}

			return result;
		}

		public static List<int>[] ToAdjacencyList(int[][] edges, int verticesCount)
		{
			var result = new List<int>[verticesCount];

			for (var i = 0; i < result.Length; i++)
				result[i] = new List<int>();

			foreach (var edge in edges)
				result[edge[0]].Add(edge[1]);

			return result;
		}

		public static List<int>[] ToAdjacencyList_UnDirected(int[][] edges, int verticesCount)
		{
			var result = new List<int>[verticesCount];

			for (var i = 0; i < result.Length; i++)
				result[i] = new List<int>();

			foreach (var edge in edges)
			{
				result[edge[0]].Add(edge[1]);
				result[edge[1]].Add(edge[0]);
			}

			return result;
		}

		public static IEnumerable<T> GetChain<T>(T node, Func<T, T> getNext) where T : class
		{
			while (node != null)
			{
				yield return node;
				node = getNext(node);
			}
		} 

		public static string ToString_AdjList<T>(IList<T>[] adj)
		{
			var sb = new StringBuilder();

			for (var i = 0; i < adj.Length; i++)
				sb.AppendFormat("{0}: {1}", i, adj[i].Join()).AppendLine();

			return sb.ToString();
		}

		public static List<int>[] UnDirected_GetDirectedSubTree(List<int>[] adj, int start)
		{
			var result = new List<int>[adj.Length];
			for (var i = 0; i < result.Length; i++)
				result[i] = new List<int>();

			var visited = new bool[adj.Length];
			visited[start] = true;

			var queue = new Queue<int>();
			queue.Enqueue(start);
			while (queue.Count > 0)
			{
				var node = queue.Dequeue();
				foreach (var child in adj[node])
					if (!visited[child])
					{
						result[node].Add(child);

						queue.Enqueue(child);
						visited[child] = true;
					}
			}

			return result;
		}

		// O (N^2)
		// Возвращает adjacency list 
		// Переделывает edges так, чтобы каждое ребро было в виде [parent, child]
		public static List<int>[] ToDirectedTree(int[][] edges, int n)
		{
			var adj = ToAdjacencyList_UnDirected(edges, n);

			if (edges.Length == 0)
				return adj;

			var head = edges[0][0];

			var stack = new Stack<int>();
			stack.Push(head);

			while (stack.Count > 0)
			{
				var node = stack.Pop();

				foreach (var child in adj[node])
				{
					adj[child].Remove(node);
					stack.Push(child);
				}
			}

			foreach (var edge in edges)
			{
				var parent = edge[0];
				var child = edge[1];
				if (!adj[parent].Contains(child))
				{
					edge[0] = child;
					edge[1] = parent;
				}
			}

			return adj;
		}

		public static string Tree_To_String(List<int>[] children, int root)
		{
			var sb = new StringBuilder();
			var nodes = Traverse_Tree_Bfs(root, v => children[v]).Count();
			sb.AppendLine($"Nodes = {nodes}, root = {root}");
			foreach (var node in Traverse_Tree_Bfs(root, v => children[v]))
				sb.AppendLine($"{node} -> {children[node].Join()}");
			return sb.ToString();
		}
	}
}