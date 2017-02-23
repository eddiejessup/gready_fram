using System;

namespace ConsoleApplication1.DataTypes
{
	public struct PointInt : IEquatable<PointInt>
	{
		public static readonly PointInt Zero = new PointInt(0, 0);
		public static readonly PointInt MinusOne = new PointInt(-1, -1);

		public PointInt(int x, int y)
		{
			X = x;
			Y = y;
		}

		public static bool operator ==(PointInt point1, PointInt point2)
		{
			return point1.X == point2.X && point1.Y == point2.Y;
		}

		public static bool operator !=(PointInt point1, PointInt point2)
		{
			return !(point1 == point2);
		}

		public static PointInt operator +(PointInt point1, PointInt point2)
		{
			return new PointInt(point1.X + point2.X, point1.Y + point2.Y);
		}

		public static PointInt operator -(PointInt point1, PointInt point2)
		{
			return new PointInt(point1.X - point2.X, point1.Y - point2.Y);
		}

		public static PointInt operator -(PointInt point)
		{
			return new PointInt(-point.X, -point.Y);
		}

		public static bool Equals(PointInt point1, PointInt point2)
		{
			return point1.X == point2.X && point1.Y == point2.Y;
		}

		public override bool Equals(object o)
		{
			if ((null == o) || !(o is PointInt))
				return false;

			var value = (PointInt)o;
			return Equals(this, value);
		}

		public bool Equals(PointInt value)
		{
			return Equals(this, value);
		}

		public override int GetHashCode()
		{
			return (X * 100003).GetHashCode() ^ Y.GetHashCode();
		}

		public readonly int X;
		public readonly int Y;

		public override string ToString()
		{
			return string.Concat("(", X.ToString(), ",", Y.ToString(), ")");
		}

		public static PointInt Parse(string str)
		{
			var parts = str.Replace("(", "").Replace(")", "")
				.Split(new[] { ':', 'x', '-', '.', ',' }, StringSplitOptions.RemoveEmptyEntries);

			var x = int.Parse(parts[0]);
			var y = int.Parse(parts[1]);
			return new PointInt(x, y);
		}

		public T AsIndex<T>(T[,] arr)
		{
			return arr[X, Y];
		}

		public ulong Distance(PointInt other)
		{
			var dx = (ulong)Math.Abs(other.X - X);
			var dy = (ulong)Math.Abs(other.Y - Y);
			return dx * dx + dy * dy;
		}
	}
}