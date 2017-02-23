using System;
using System.Numerics;
using System.Text;

namespace ConsoleApplication1.Helpers
{
	public static class BigIntegerHelper
	{
		public static string ToBinaryString(this BigInteger bigint)
		{
			var bytes = bigint.ToByteArray();
			var i = bytes.Length - 1;

			var base2 = new StringBuilder(bytes.Length * 8);

			var binary = Convert.ToString(bytes[i], 2);

			if (binary[0] != '0' && bigint.Sign == 1)
				base2.Append('0');

			base2.Append(binary);

			for (i--; i >= 0; i--)
				base2.Append(Convert.ToString(bytes[i], 2).PadLeft(8, '0'));

			return base2.ToString();
		}

		public static string ToHexadecimalString(this BigInteger bigint)
		{
			return bigint.ToString("X");
		}

		public static string ToOctalString(this BigInteger bigint)
		{
			var bytes = bigint.ToByteArray();
			var idx = bytes.Length - 1;

			var base8 = new StringBuilder(((bytes.Length / 3) + 1) * 8);

			var extra = bytes.Length % 3;

			if (extra == 0)
				extra = 3;

			int int24 = 0;
			for (; extra != 0; extra--)
			{
				int24 <<= 8;
				int24 += bytes[idx--];
			}

			var octal = Convert.ToString(int24, 8);

			if (octal[0] != '0' && bigint.Sign == 1)
				base8.Append('0');

			base8.Append(octal);

			for (; idx >= 0; idx -= 3)
			{
				int24 = (bytes[idx] << 16) + (bytes[idx - 1] << 8) + bytes[idx - 2];
				base8.Append(Convert.ToString(int24, 8).PadLeft(8, '0'));
			}

			return base8.ToString();
		}
	}
}