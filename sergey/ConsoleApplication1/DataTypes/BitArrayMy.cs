using System;

namespace ConsoleApplication1.DataTypes
{
	public class BitArrayMy
	{
		private readonly int[] arr;
		public int Length { get; }

		public BitArrayMy(int length, bool setAllValue)
		{
			arr = new int[GetArrayLength(length, 32)];
			Length = length;
            var fillValue = setAllValue ? unchecked(((int)0xffffffff)) : 0;
			for (var i = 0; i < arr.Length; i++)
				arr[i] = fillValue;
		}

		public BitArrayMy(int length)
		{
			arr = new int[GetArrayLength(length, 32)];
			Length = length;
			for (var i = 0; i < arr.Length; i++)
				arr[i] = 0;
		}

		public BitArrayMy(BitArrayMy bits)
		{
			var arrayLength = GetArrayLength(bits.Length, 32);

			arr = new int[arrayLength];
			Length = bits.Length;

			Array.Copy(bits.arr, arr, arrayLength);
		}

		public BitArrayMy(BitArrayMy bits, int additionalLength)
		{
			Length = bits.Length + additionalLength;

			arr = new int[GetArrayLength(Length, 32)];

			Array.Copy(bits.arr, arr, bits.arr.Length);
		}

		public BitArrayMy(bool[] values)
		{
			arr = new int[GetArrayLength(values.Length, 32)];
			Length = values.Length;

			for (var i = 0; i < values.Length; i++)
				if (values[i])
					arr[i / 32] |= (1 << (i % 32));
		}

		public bool[] ToArray()
		{
			var result = new bool[Length];
			for (var i = 0; i < result.Length; i++)
				result[i] = this[i];
			return result;
		}

		public bool this[int index] 
		{
			get
			{
				return (arr[index / 32] & (1 << (index % 32))) != 0;
			}
			set
			{
				if (value)
					arr[index / 32] |= (1 << (index % 32));
				else
					arr[index / 32] &= ~(1 << (index % 32));
			}
		}

		public bool AllZero
		{
			get
			{
				foreach (var i in arr)
					if (i != 0)
						return false;
				return true;
			}
		}

		public void And(BitArrayMy value)
		{
			if (Length != value.Length)
				throw new ArgumentException("Lengths are differ");

			var ints = GetArrayLength(Length, 32);
			for (var i = 0; i < ints; i++)
				arr[i] &= value.arr[i];
		}

		private static int GetArrayLength(int n, int div)
		{
			return n > 0 ? (((n - 1) / div) + 1) : 0;
		}
	}
}
