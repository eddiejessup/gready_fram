using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ConsoleApplication1.DataTypes
{
	public class LeftLeaningRedBlackTree<TKey, TValue>
	{
		private Comparison<TKey> _keyComparison;
		private Comparison<TValue> _valueComparison;
		private Node _rootNode;

		[DebuggerDisplay("Key={Key}, Value={Value}, Siblings={Siblings}")]
		public class Node
		{
			public TKey Key;
			public TValue Value;
			public Node Left;
			public Node Right;
			public bool IsBlack;
			public int Siblings;
		}

		public LeftLeaningRedBlackTree(Comparison<TKey> keyComparison)
		{
			if (null == keyComparison)
				throw new ArgumentNullException("keyComparison");
			_keyComparison = keyComparison;
		}

		public LeftLeaningRedBlackTree(Comparison<TKey> keyComparison, Comparison<TValue> valueComparison)
			: this(keyComparison)
		{
			if (null == valueComparison)
				throw new ArgumentNullException("valueComparison");
			_valueComparison = valueComparison;
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

		private bool IsMultiDictionary
		{
			get { return null != _valueComparison; }
		}

		public void Add(TKey key, TValue value)
		{
			_rootNode = Add(_rootNode, key, value);
			_rootNode.IsBlack = true;
		}

		public bool Remove(TKey key)
		{
			if (IsMultiDictionary)
				throw new InvalidOperationException("Remove is only supported when acting as a normal (non-multi) dictionary.");
			return Remove(key, default(TValue));
		}

		public bool Remove(TKey key, TValue value)
		{
			int initialCount = Count;
			if (null != _rootNode)
			{
				_rootNode = Remove(_rootNode, key, value);
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

		public TValue GetValueForKey(TKey key)
		{
			if (IsMultiDictionary)
				throw new InvalidOperationException("GetValueForKey is only supported when acting as a normal (non-multi) dictionary.");

			Node node = GetNodeForKey(key);
			if (null != node)
				return node.Value;
			else
				throw new KeyNotFoundException();
		}

		public IEnumerable<TValue> GetValuesForKey(TKey key)
		{
			return Traverse(GetNodeForKey(key), n => 0 == _keyComparison(n.Key, key), n => n.Value);
		}

		public IEnumerable<TValue> GetValuesForAllKeys()
		{
			return Traverse(_rootNode, n => true, n => n.Value);
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

		private Node Add(Node node, TKey key, TValue value)
		{
			if (null == node)
			{
				Count++;
				return new Node { Key = key, Value = value };
			}

			if (IsRed(node.Left) && IsRed(node.Right))
				FlipColor(node);

			// Find right place for new node
			int comparisonResult = KeyAndValueComparison(key, value, node.Key, node.Value);
			if (comparisonResult < 0)
			{
				node.Left = Add(node.Left, key, value);
			}
			else if (0 < comparisonResult)
			{
				node.Right = Add(node.Right, key, value);
			}
			else
			{
				if (IsMultiDictionary)
				{
					node.Siblings++;
					Count++;
				}
				else
				{
					node.Value = value;
				}
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

		private Node Remove(Node node, TKey key, TValue value)
		{
			int comparisonResult = KeyAndValueComparison(key, value, node.Key, node.Value);
			if (comparisonResult < 0)
			{
				if (null != node.Left)
				{
					if (!IsRed(node.Left) && !IsRed(node.Left.Left))
						node = MoveRedLeft(node);

					node.Left = Remove(node.Left, key, value);
				}
			}
			else
			{
				if (IsRed(node.Left))
				{
					node = RotateRight(node);
				}
				if ((0 == KeyAndValueComparison(key, value, node.Key, node.Value)) && (null == node.Right))
				{
					Debug.Assert(null == node.Left, "About to remove an extra node.");
					Count--;
					if (0 < node.Siblings)
					{
						Debug.Assert(IsMultiDictionary, "Should not have siblings if tree is not a multi-dictionary.");
						node.Siblings--;
						return node;
					}
					else
					{
						return null;
					}
				}
				if (null != node.Right)
				{
					if (!IsRed(node.Right) && !IsRed(node.Right.Left))
					{
						node = MoveRedRight(node);
					}
					if (0 == KeyAndValueComparison(key, value, node.Key, node.Value))
					{
						Count--;
						if (0 < node.Siblings)
						{
							Debug.Assert(IsMultiDictionary, "Should not have siblings if tree is not a multi-dictionary.");
							node.Siblings--;
						}
						else
						{
							Node m = GetExtreme(node.Right, n => n.Left, n => n);
							node.Key = m.Key;
							node.Value = m.Value;
							node.Siblings = m.Siblings;
							node.Right = DeleteMinimum(node.Right);
						}
					}
					else
					{
						node.Right = Remove(node.Right, key, value);
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

		private static Node RotateLeft(Node node)
		{
			Node x = node.Right;
			node.Right = x.Left;
			x.Left = node;
			x.IsBlack = node.IsBlack;
			node.IsBlack = false;
			return x;
		}

		private static Node RotateRight(Node node)
		{
			Node x = node.Left;
			node.Left = x.Right;
			x.Right = node;
			x.IsBlack = node.IsBlack;
			node.IsBlack = false;
			return x;
		}

		private static Node MoveRedLeft(Node node)
		{
			FlipColor(node);
			if (IsRed(node.Right.Left))
			{
				node.Right = RotateRight(node.Right);
				node = RotateLeft(node);
				FlipColor(node);

				// * Avoid creating right-leaning nodes
				if (IsRed(node.Right.Right))
				{
					node.Right = RotateLeft(node.Right);
				}
			}
			return node;
		}

		private static Node MoveRedRight(Node node)
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

			return FixUp(node);
		}

		private static Node FixUp(Node node)
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
						for (int i = 0; i <= current.Siblings; i++)
						{
							if (condition(current))
								yield return selector(current);
						}
						current = current.Right;
					}
					while ((null == current) &&
						   (0 < stack.Count) &&
						   (null != (current = stack.Pop())));
				}
			}
		}

		private int KeyAndValueComparison(TKey leftKey, TValue leftValue, TKey rightKey, TValue rightValue)
		{
			int comparisonResult = _keyComparison(leftKey, rightKey);
			if ((0 == comparisonResult) && (null != _valueComparison))
				comparisonResult = _valueComparison(leftValue, rightValue);
			return comparisonResult;
		}

		[Conditional("Debug")]
		public void AssertInvariants()
		{
			Debug.Assert((null == _rootNode) || _rootNode.IsBlack, "Root is not black");
			Dictionary<Node, Node> parents = new Dictionary<LeftLeaningRedBlackTree<TKey, TValue>.Node, LeftLeaningRedBlackTree<TKey, TValue>.Node>();
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
					Debug.Assert(0 > KeyAndValueComparison(node.Left.Key, node.Left.Value, node.Key, node.Value), "Left node is greater than its parent.");
				}
				if (null != node.Right)
				{
					Debug.Assert(0 < KeyAndValueComparison(node.Right.Key, node.Right.Value, node.Key, node.Value), "Right node is less than its parent.");
				}
				Debug.Assert(!IsRed(node) || (!IsRed(node.Left) && !IsRed(node.Right)), "Red node has a red child.");
				Debug.Assert(!IsRed(node.Right) || IsRed(node.Left), "Node is not left-leaning.");
			}
		}
	}
}