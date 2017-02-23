using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.Helpers
{
	public static class BitHelper
	{
		public static int NumberOfSetBits(ulong i)
		{
			i = i - ((i >> 1) & 0x5555555555555555UL);
			i = (i & 0x3333333333333333UL) + ((i >> 2) & 0x3333333333333333UL);
			return (int)(unchecked(((i + (i >> 4)) & 0xF0F0F0F0F0F0F0FUL) * 0x101010101010101UL) >> 56);
		}

		// NOT TESTED
		public static uint Log2N(uint v)
		{
			var b = new uint[] { 0x2, 0xC, 0xF0, 0xFF00, 0xFFFF0000 };
			var S = new[] { 1, 2, 4, 8, 16 };
			int i;

			uint r = 0;
			for (i = 4; i >= 0; i--)
			{
				if ((v & b[i]) > 0)
				{
					v >>= S[i];
					r |= (uint)S[i];
				}
			}

			return r;
		}

		public static BitArray StringToBitArray(string hex)
		{
			var arr = new BitArray(hex.Length * 4);
			for (int i = 0; i < hex.Length; i++)
			{
				var val = GetHexVal(hex[i]);
				for (var j = 0; j < 4; j++)
					if ((val & (1 << j)) > 0)
						arr[i * 4 + 3 - j] = true;
			}
			return arr;
		}

		public static byte[] StringToByteArray(string hex)
		{
			var arr = new byte[(hex.Length + 1) / 2];

			var isOdd = hex.Length % 2 == 1;

			if (isOdd)
				arr[0] = (byte)GetHexVal(hex[0]);

			for (int i = hex.Length - 2; i >= 0; i -= 2)
				arr[(i + 1) / 2] = (byte)((GetHexVal(hex[i]) << 4) + (GetHexVal(hex[i + 1])));

			return arr;
		}

		public static int GetHexVal(char hex)
		{
			var val = (int)hex;
			return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
		}

		public static string BitArrayToHexString(BitArray bits, bool skipLeadingZeroes)
		{
			var sb = new StringBuilder(bits.Length / 4);

			var hasAny = false;

			for (int i = 0; i < bits.Length; i += 4)
			{
				var v = (bits[i + 0] ? 8 : 0) |
						(bits[i + 1] ? 4 : 0) |
						(bits[i + 2] ? 2 : 0) |
						(bits[i + 3] ? 1 : 0);

				if (v == 0 && skipLeadingZeroes && !hasAny) continue;
				hasAny = true;

				sb.Append(v.ToString("X1"));
			}

			return sb.Length == 0 ? "0" : sb.ToString();
		}

		public static string ByteArrayToHex(byte[] arr, bool skipLeadingZeroes)
		{
			var sb = new StringBuilder(arr.Length * 2);

			var hasAny = false;

			foreach (byte b in arr)
			{
				if (skipLeadingZeroes && !hasAny)
				{
					if (b == 0) continue;

					else if (b < 16)
					{
						sb.Append(b.ToString("X1"));
						hasAny = true;
					}

					else
					{
						sb.Append(b.ToString("X2"));
						hasAny = true;
					}
				}
				else
					sb.Append(b.ToString("{0:X2}"));
			}

			return sb.Length == 0 ? "0" : sb.ToString();
		}

		public static int HighestOneBit(this int i)
		{
			i |= (i >> 1);
			i |= (i >> 2);
			i |= (i >> 4);
			i |= (i >> 8);
			i |= (i >> 16);
			return i - (i >> 1);
		}

		private static readonly int[] _numberOfTrailingZerosLookup =
		{
			32, 0, 1, 26, 2, 23, 27, 0, 3, 16, 24, 30, 28, 11, 0, 13, 4, 7, 17,
			0, 25, 22, 31, 15, 29, 10, 12, 6, 0, 21, 14, 9, 5, 20, 8, 19, 18
		};

		public static int NumberOfTrailingZeros(this int i)
		{
			return _numberOfTrailingZerosLookup[(i & -i) % 37];
		}
	}
}
