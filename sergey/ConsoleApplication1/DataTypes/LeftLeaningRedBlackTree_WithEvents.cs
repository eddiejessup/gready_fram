using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ConsoleApplication1.DataTypes
{
	public class LeftLeaningRedBlackTree_WithEvents<TKey>
	{
		private Comparison<TKey> _keyComparison;
		private Action<Node> _onChanged;
		private Node _rootNode;

		public class Node
		{
			public TKey Key;
			public Node Left;
			public Node Right;
			public bool IsBlack;

			public override string ToString() => $"Key={Key}";
		}

		public LeftLeaningRedBlackTree_WithEvents(Comparison<TKey> keyComparison, Action<Node> onChanged)
		{
			if (null == keyComparison) throw new ArgumentNullException(nameof(keyComparison));
			if (null == onChanged) throw new ArgumentNullException(nameof(onChanged));
			_keyComparison = keyComparison;
			_onChanged = onChanged;
		}

		public IEnumerable<Node> Search(Func<Node, bool?> trueGoLeftNotRight, Node startNode = null)
		{
			var node = startNode ?? _rootNode;

			while (null != node)
			{
				yield return node;

				var next = trueGoLeftNotRight(node);

				if (next == true)
					node = node.Left;
				else if (next == false)
					node = node.Right;
				else
					yield break;
			}
		}

		public void Add(TKey key)
		{
			_rootNode = Add(_rootNode, key);
			_rootNode.IsBlack = true;
		}

		public bool Remove(TKey key)
		{
			int initialCount = Count;
			if (null != _rootNode)
			{
				_rootNode = Remove(_rootNode, key);
				if (null != _rootNode)
					_rootNode.IsBlack = true;
			}
			return initialCount != Count;
		}

		public void Clear()
		{
			_rootNode = null;
			Count = 0;
		}

		public IEnumerable<TKey> GetKeys()
		{
			TKey lastKey = default(TKey);
			bool lastKeyValid = false;
			return Traverse(
				_rootNode,
				n => !lastKeyValid || !object.Equals(lastKey, n.Key),
				n =>
				{
					lastKey = n.Key;
					lastKeyValid = true;
					return lastKey;
				});
		}

		public IEnumerable<Node> EnumerateNodes()
		{
			return Traverse(
				_rootNode,
				n => true,
				n => n);
		}

		public int Count { get; private set; }

		public TKey MinimumKey
		{
			get { return GetExtreme(_rootNode, n => n.Left, n => n.Key); }
		}

		public TKey MaximumKey
		{
			get { return GetExtreme(_rootNode, n => n.Right, n => n.Key); }
		}

		private static bool IsRed(Node node)
		{
			if (null == node)
				return false;
			return !node.IsBlack;
		}

		private Node Add(Node node, TKey key)
		{
			if (null == node)
			{
				Count++;
				var newNode = new Node { Key = key };
				_onChanged(newNode);
				return newNode;
			}

			if (IsRed(node.Left) && IsRed(node.Right))
				FlipColor(node);

			// Find right place for new node
			int comparisonResult = _keyComparison(key, node.Key);
			if (comparisonResult < 0)
			{
				node.Left = Add(node.Left, key);
				_onChanged(node);
			}
			else if (0 < comparisonResult)
			{
				node.Right = Add(node.Right, key);
				_onChanged(node);
			}

			if (IsRed(node.Right))
			{
				node = RotateLeft(node);
			}

			if (IsRed(node.Left) && IsRed(node.Left.Left))
			{
				node = RotateRight(node);
			}

			return node;
		}

		private Node Remove(Node node, TKey key)
		{
			int comparisonResult = _keyComparison(key, node.Key);
			if (comparisonResult < 0)
			{
				if (null != node.Left)
				{
					if (!IsRed(node.Left) && !IsRed(node.Left.Left))
						node = MoveRedLeft(node);

					node.Left = Remove(node.Left, key);
					_onChanged(node);
				}
			}
			else
			{
				if (IsRed(node.Left))
				{
					node = RotateRight(node);
				}
				if ((0 == _keyComparison(key, node.Key)) && (null == node.Right))
				{
					Count--;
					return null;
				}
				if (null != node.Right)
				{
					if (!IsRed(node.Right) && !IsRed(node.Right.Left))
					{
						node = MoveRedRight(node);
					}
					if (0 == _keyComparison(key, node.Key))
					{
						Count--;
						Node m = GetExtreme(node.Right, n => n.Left, n => n);
						node.Key = m.Key;
						node.Right = DeleteMinimum(node.Right);
						_onChanged(node);
					}
					else
					{
						node.Right = Remove(node.Right, key);
						_onChanged(node);
					}
				}
			}
			return FixUp(node);
		}

		private static void FlipColor(Node node)
		{
			node.IsBlack = !node.IsBlack;
			node.Left.IsBlack = !node.Left.IsBlack;
			node.Right.IsBlack = !node.Right.IsBlack;
		}

		private Node RotateLeft(Node node)
		{
			Node x = node.Right;
			node.Right = x.Left;
			x.Left = node;
			x.IsBlack = node.IsBlack;
			node.IsBlack = false;
			_onChanged(node);
			_onChanged(x);
			return x;
		}

		private Node RotateRight(Node node)
		{
			Node x = node.Left;
			node.Left = x.Right;
			x.Right = node;
			x.IsBlack = node.IsBlack;
			node.IsBlack = false;
			_onChanged(node);
			_onChanged(x);
			return x;
		}

		private Node MoveRedLeft(Node node)
		{
			FlipColor(node);
			if (IsRed(node.Right.Left))
			{
				node.Right = RotateRight(node.Right);
				_onChanged(node);
				node = RotateLeft(node);
				FlipColor(node);

				// * Avoid creating right-leaning nodes
				if (IsRed(node.Right.Right))
				{
					node.Right = RotateLeft(node.Right);
					_onChanged(node);
				}
			}
			return node;
		}

		private Node MoveRedRight(Node node)
		{
			FlipColor(node);
			if (IsRed(node.Left.Left))
			{
				node = RotateRight(node);
				FlipColor(node);
			}
			return node;
		}

		private Node DeleteMinimum(Node node)
		{
			if (null == node.Left)
				return null;

			if (!IsRed(node.Left) && !IsRed(node.Left.Left))
				node = MoveRedLeft(node);

			node.Left = DeleteMinimum(node.Left);
			_onChanged(node);

			return FixUp(node);
		}

		private Node FixUp(Node node)
		{
			if (IsRed(node.Right))
				node = RotateLeft(node);

			if (IsRed(node.Left) && IsRed(node.Left.Left))
				node = RotateRight(node);

			if (IsRed(node.Left) && IsRed(node.Right))
				FlipColor(node);

			if ((null != node.Left) && IsRed(node.Left.Right) && !IsRed(node.Left.Left))
			{
				node.Left = RotateLeft(node.Left);
				_onChanged(node);
				if (IsRed(node.Left))
					node = RotateRight(node);
			}

			return node;
		}

		private Node GetNodeForKey(TKey key)
		{
			Node node = _rootNode;
			while (null != node)
			{
				int comparisonResult = _keyComparison(key, node.Key);
				if (comparisonResult < 0)
				{
					node = node.Left;
				}
				else if (0 < comparisonResult)
				{
					node = node.Right;
				}
				else
				{
					return node;
				}
			}

			return null;
		}

		private static T GetExtreme<T>(Node node, Func<Node, Node> successor, Func<Node, T> selector)
		{
			T extreme = default(T);
			Node current = node;
			while (null != current)
			{
				extreme = selector(current);
				current = successor(current);
			}
			return extreme;
		}

		private IEnumerable<T> Traverse<T>(Node node, Func<Node, bool> condition, Func<Node, T> selector)
		{
			Stack<Node> stack = new Stack<Node>();
			Node current = node;
			while (null != current)
			{
				if (null != current.Left)
				{
					stack.Push(current);
					current = current.Left;
				}
				else
				{
					do
					{
						if (condition(current))
							yield return selector(current);
						current = current.Right;
					}
					while ((null == current) &&
						   (0 < stack.Count) &&
						   (null != (current = stack.Pop())));
				}
			}
		}

		[Conditional("Debug")]
		public void AssertInvariants()
		{
			Debug.Assert((null == _rootNode) || _rootNode.IsBlack, "Root is not black");
			Dictionary<Node, Node> parents = new Dictionary<LeftLeaningRedBlackTree_WithEvents<TKey>.Node, LeftLeaningRedBlackTree_WithEvents<TKey>.Node>();
			foreach (Node node in Traverse(_rootNode, n => true, n => n))
			{
				if (null != node.Left)
				{
					parents[node.Left] = node;
				}
				if (null != node.Right)
				{
					parents[node.Right] = node;
				}
			}
			if (null != _rootNode)
			{
				parents[_rootNode] = null;
			}
			int treeCount = -1;
			foreach (Node node in Traverse(_rootNode, n => (null == n.Left) || (null == n.Right), n => n))
			{
				int pathCount = 0;
				Node current = node;
				while (null != current)
				{
					if (current.IsBlack)
					{
						pathCount++;
					}
					current = parents[current];
				}
				Debug.Assert((-1 == treeCount) || (pathCount == treeCount), "Not all paths have the same number of black nodes.");
				treeCount = pathCount;
			}
			foreach (Node node in Traverse(_rootNode, n => true, n => n))
			{
				if (null != node.Left)
				{
					Debug.Assert(0 > _keyComparison(node.Left.Key, node.Key), "Left node is greater than its parent.");
				}
				if (null != node.Right)
				{
					Debug.Assert(0 < _keyComparison(node.Right.Key, node.Key), "Right node is less than its parent.");
				}
				Debug.Assert(!IsRed(node) || (!IsRed(node.Left) && !IsRed(node.Right)), "Red node has a red child.");
				Debug.Assert(!IsRed(node.Right) || IsRed(node.Left), "Node is not left-leaning.");
			}
		}
	}
}