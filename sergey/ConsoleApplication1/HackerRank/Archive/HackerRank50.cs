using ConsoleApplication1.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApplication1.Helpers.BitHelper;

namespace ConsoleApplication1.HackerRank
{
	class HackerRank50
	{
		public void Go()
		{
			//foreach (var line in Solve(8, "02B", "09F", "058")) Console.WriteLine(line);
			//foreach (var line in Solve(1, "11", "01", "11")) Console.WriteLine(line);
			//Console.WriteLine(ByteArrayToHex(StringToByteArray("123")));
			//Console.WriteLine(BitArrayToHexString(new BitArray(StringToByteArray("010")), true));
			CompareWith2();
			//ShowBitArray(StringToBitArray("10F"));
			//Console.WriteLine(BitArrayToHexString(StringToBitArray("00001"), true));
		}

		public static void CompareWith2()
		{
			var rnd = new Random(1337);
			for (var t = 0; t < 10000; t++)
			{
				var K = (long)rnd.Next(0, 17);
				var A = RandomGenerator.RandomString(rnd, "0123456789ABCDEF", 6);
				var B = RandomGenerator.RandomString(rnd, "0123456789ABCDEF", 6);
				var C = RandomGenerator.RandomString(rnd, "0123456789ABCDEF", 6);
				var solve1 = Solve(K, A, B, C) ?? new string[0];
				var solve2 = Solve2(K, A, B, C) ?? new string[0];

				if (!solve2.SequenceEqual(solve1))
				{
					Console.WriteLine(new { t, K, A, B, C, solve1 = solve1.Join(), solve2 = solve2.Join() });
					throw new InvalidOperationException();
				}
			}
		}

		public static void CompareWithBrute()
		{
			var rnd = new Random(1337);
			for (var t = 0; t < 10000; t++)
			{
				var K = (long)rnd.Next(0, 17);
				var A = RandomGenerator.RandomString(rnd, "0123456789ABCDEF", 2);
				var B = RandomGenerator.RandomString(rnd, "0123456789ABCDEF", 2);
				var C = RandomGenerator.RandomString(rnd, "0123456789ABCDEF", 2);
				var brute = SolveBrute(K, A, B, C) ?? new string[0];
				var actual = Solve(K, A, B, C) ?? new string[0];

				if (!actual.SequenceEqual(brute))
				{
					Console.WriteLine(new { t, K, A, B, C, brute = brute.Join(), actual = actual.Join() });
					throw new InvalidOperationException();
				}
			}
		}

		public static void ShowBitArray(BitArray bits)
		{
			for (var i = 0; i < bits.Length; i++)
				Console.Write(bits[i] ? "1" : "0");
			Console.WriteLine();
		}

		public static string[] SolveBrute(long K, string A, string B, string C)
		{
			var Abytes = StringToByteArray(A);
			var Bbytes = StringToByteArray(B);
			var Cbytes = StringToByteArray(C);

			var Au = (ulong)(Abytes[0]);
			var Bu = (ulong)(Bbytes[0]);
			var Cu = (ulong)(Cbytes[0]);

			var bestA = ulong.MaxValue;
			var bestB = ulong.MaxValue;

			for (var ap = 0ul; ap <= 0xFF; ap++)
				for (var bp = 0ul; bp <= 0xFF; bp++)
					if ((ap | bp) == Cu)
					{
						var k0 = NumberOfSetBits(ap ^ Au);
						var k1 = NumberOfSetBits(bp ^ Bu);
						if (k0 + k1 <= K)
						{
							if (ap < bestA)
							{
								bestA = ap;
								bestB = bp;
							}
							else if (ap == bestA && bp < bestB)
							{
								bestB = bp;
							}
						}
					}

			if (bestA == ulong.MaxValue)
				return null;
			else
				return new[] {
					ByteArrayToHex(new[] { (byte)(bestA & 0xFF) }, true),
					ByteArrayToHex(new[] { (byte)(bestB & 0xFF) }, true) };
		}

		public static string[] Solve(long K, string A, string B, string C)
		{
			var Aarr = StringToBitArray(A);
			var Barr = StringToBitArray(B);
			var Carr = StringToBitArray(C);

			var n = Aarr.Length;

			var Ap = new BitArray(n);
			var Bp = new BitArray(n);

			// required operations
			for (var i = 0; i < n; i++)
			{
				var ai = Aarr[i];
				var bi = Barr[i];
				var ci = Carr[i];

				if (!ci)
				{
					if (ai) K--;
					if (bi) K--;
				}
				else if (!ai && !bi)
				{
					K--;
					Bp[i] = true;
				}
				else
				{
					Ap[i] = ai;
					Bp[i] = bi;
				}
			}

			// impossible
			if (K < 0) return null;

			// minimize A
			for (var i = 0; i < n; i++)
			{
				if (K == 0) break;

				var ai = Aarr[i];
				var bi = Barr[i];
				var ci = Carr[i];

				if (ci && (ai || bi))
				{
					if (ai && bi)
					{
						K--;
						Ap[i] = false;
					}
					else if (ai && !bi && K >= 2)
					{
						K -= 2;
						Ap[i] = false;
						Bp[i] = true;
					}
				}
			}

			return new[] { BitArrayToHexString(Ap, true), BitArrayToHexString(Bp, true) };
		}


		public static string[] Solve2(long K, string A, string B, string C)
		{
			var n = A.Length * 4;

			var Aarr = new BitArray(StringToByteArray2(A).Reverse().ToArray());
			var Barr = new BitArray(StringToByteArray2(B).Reverse().ToArray());
			var Carr = new BitArray(StringToByteArray2(C).Reverse().ToArray());

			var Ap = new BitArray(n);
			var Bp = new BitArray(n);

			// required operations
			for (var i = n - 1; i >= 0; i--)
			{
				var ai = Aarr[i];
				var bi = Barr[i];
				var ci = Carr[i];

				if (!ci)
				{
					if (ai) K--;
					if (bi) K--;
				}
				else if (!ai && !bi)
				{
					K--;
					Bp[n - i - 1] = true;
				}
				else
				{
					Ap[n - i - 1] = ai;
					Bp[n - i - 1] = bi;
				}
			}

			// impossible
			if (K < 0) return null;

			// minimize A
			for (var i = n - 1; i >= 0; i--)
			{
				if (K == 0) break;

				var ai = Aarr[i];
				var bi = Barr[i];
				var ci = Carr[i];

				if (ci && (ai || bi))
				{
					if (ai && bi)
					{
						K--;
						Ap[n - i - 1] = false;
					}
					else if (ai && !bi && K >= 2)
					{
						K -= 2;
						Ap[n - i - 1] = false;
						Bp[n - i - 1] = true;
					}
				}
			}

			return new[] { BitArrayToHexString2(Ap), BitArrayToHexString2(Bp) };
		}

		public static byte[] StringToByteArray2(string hex)
		{
			var arr = new byte[(hex.Length + 1) / 2];

			var isOdd = hex.Length % 2 == 1;

			if (isOdd)
				arr[0] = (byte)GetHexVal2(hex[0]);

			for (int i = hex.Length - 2; i >= 0; i -= 2)
				arr[i / 2 + (isOdd ? 1 : 0)] = (byte)((GetHexVal2(hex[i]) << 4) + (GetHexVal2(hex[i + 1])));

			return arr;
		}

		public static int GetHexVal2(char hex)
		{
			var val = (int)hex;
			return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
		}

		public static string BitArrayToHexString2(BitArray bits)
		{
			var sb = new StringBuilder(bits.Length / 4);

			var hasAny = false;

			for (int i = 0; i < bits.Length; i += 4)
			{
				var v = (bits[i] ? 8 : 0) |
						(bits[i + 1] ? 4 : 0) |
						(bits[i + 2] ? 2 : 0) |
						(bits[i + 3] ? 1 : 0);

				if (!hasAny && v == 0) continue;
				hasAny = true;

				sb.Append(v.ToString("X1"));
			}

			return sb.Length == 0 ? "0" : sb.ToString();
		}



	}
}
