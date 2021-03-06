﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ConsoleApplication1.Helpers
{
	public static class MyMath
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint HashKnuthMultiplicative(long n, uint len)
		{
			unchecked
			{
				return (uint)(n * 2654435761) % len;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint HashAvalanche(long n, uint len)
		{
			unchecked
			{
				var k = n;
				k *= 357913941;
				k ^= k << 24;
				k += ~357913941;
				k ^= k >> 31;
				k ^= k << 31;
				return (uint)k % len;
			}
		}

		// 0,1 -> true; each other -> false means prime
		public static bool[] CreatePrimesSieve(ulong maxprime)
		{
			var sieve = new bool[maxprime];

			sieve[0] = true;
			sieve[1] = true;

			for (var i = 4; i < sieve.Length; i += 2)
				sieve[i] = true;

			for (var i = 3; i < sieve.Length / 3 + 1; i++)
			{
				if (sieve[i]) continue;

				for (var j = 2 * i; j < sieve.Length; j += i)
					sieve[j] = true;
			}
			return sieve;
		}

		public static ulong[] ConvertSieveToPrimes(bool[] sieve)
		{
			var cnt = sieve.Count(p => !p);
			var result = new ulong[cnt];
			ulong i = 0;
			for (ulong j = 0; j < (ulong)sieve.Length; j++)
				if (!sieve[j])
				{
					result[i] = j;
					i++;
				}
			return result;
		}

		public static bool IsPrime(ulong x)
		{
			if (x <= 1) return false;
			if (x == 2) return true;

			for (ulong i = 2; i < Math.Sqrt(x) + 1; i++)
			{
				if (x % i == 0)
					return false;
			}
			return true;
		}

		public static bool IsPrime(BigInteger x)
		{
			return IsPrime(x, x / 2);
		}

		public static bool IsPrime(BigInteger x, BigInteger checkLimit)
		{
			if (x <= 1) return false;
			if (x == 2) return true;
			if (x.IsEven) return false;

			for (var i = new BigInteger(3); i <= checkLimit; i += 2)
			{
				if (x % i == 0)
					return false;
			}

			return true;
		}

		public static ulong ExtendedEuclidGcd(ulong a, ulong b)
		{
			ulong s = 0;
			ulong t = 1;
			ulong r = b;
			ulong olds = 1;
			ulong oldt = 0;
			ulong oldr = a;
			while (r != 0)
			{
				var quotient = oldr / r;

				var temp1 = oldr;
				oldr = r;
				r = temp1 - quotient * r;

				var temp2 = olds;
				olds = s;
				s = temp2 - quotient * s;

				var temp3 = oldt;
				oldt = t;
				t = temp3 - quotient * t;
			}

			// Bézout coefficients: (olds, oldt)
			// greatest common divisor: oldr
			// quotients by the gcd: (t, s)

			return oldr;
		}

		public static ulong LCM(ulong a, ulong b)
		{
			return (a / ExtendedEuclidGcd(a, b)) * b;
		}

		// TODO: Remove recursion
		public static ulong ModularPow(ulong a, ulong p, ulong mod)
		{
			if (p == 0) return 1;
			if (p % 2 == 0) return ModularPow(ModularMul(a, a, mod), p / 2, mod);
			return ModularMul(ModularPow(a, p - 1, mod), a, mod);
		}

		public static ulong ModularMul(ulong a, ulong b, ulong mod)
		{
			int i;
			ulong now = 0;
			for (i = 63; i >= 0; i--) if (((a >> i) & 1) == 1) break;
			for (; i >= 0; i--)
			{
				now <<= 1;
				while (now > mod) now -= mod;
				if (((a >> i) & 1) == 1) now += b;
				while (now > mod) now -= mod;
			}
			return now;
		}

		public static ulong Pow(ulong a, ulong pow)
		{
			ulong y = 1;

			while (true)
			{
				if ((pow & 1) != 0) y = (a * y);
				pow = pow >> 1;
				if (pow == 0) return y;
				a = (a * a);
			}
		}

		public static double PowDouble(double a, ulong pow)
		{
			double y = 1;

			while (true)
			{
				if ((pow & 1) != 0) y = (a * y);
				pow = pow >> 1;
				if (pow == 0) return y;
				a = (a * a);
			}
		}

		public static ulong ModularInverse(ulong a, ulong b)
		{
			var dividend = (long)(a % b);
			var divisor = (long)b;

			var last_x = 1L;
			var curr_x = 0L;

			while (divisor > 0)
			{
				var quotient = dividend / divisor;
				var remainder = dividend % divisor;

				if (remainder <= 0)
					break;

				var next_x = last_x - curr_x * quotient;
				last_x = curr_x;
				curr_x = next_x;

				dividend = divisor;
				divisor = remainder;
			}

			return (ulong)(curr_x < 0 ? curr_x + (long)b : curr_x);
		}

		public static BigInteger ModularInverse(BigInteger a, BigInteger b)
		{
			var dividend = a % b;
			var divisor = b;

			var last_x = BigInteger.One;
			var curr_x = BigInteger.Zero;

			while (divisor.Sign > 0)
			{
				var quotient = dividend / divisor;
				var remainder = dividend % divisor;
				if (remainder.Sign <= 0)
					break;

				var next_x = last_x - curr_x * quotient;
				last_x = curr_x;
				curr_x = next_x;

				dividend = divisor;
				divisor = remainder;
			}

			//			if (divisor != BigInteger.One)
			//				throw new Exception("Numbers a and b are not relatively primes");

			return (curr_x.Sign < 0 ? curr_x + b : curr_x);
		}

		public static int FillDivisors(ulong n, ulong[] divisors)
		{
			divisors[0] = 1;
			divisors[1] = n;
			var cnt = 2;
			ulong i = 2;
			while (i < Math.Sqrt(n))
			{
				if (n % i == 0)
				{
					divisors[cnt] = n / i;
					divisors[cnt + 1] = i;
					cnt += 2;
				}
				i++;
			}
			if (i * i == n)
			{
				divisors[cnt] = i;
				cnt++;
			}
			return cnt;
		}

		public static ulong FindPrimitiveRootForPrime(ulong prime, ulong[] invertedFactors, ulong invertedFactorsLength)
		{
			for (ulong gen = 2; gen < prime; gen++)
			{
				var found = true;
				for (ulong i = 0; i < invertedFactorsLength; i++)
				{
					var test = ModularPow(gen, invertedFactors[i], prime);
					if (test == 1)
					{
						found = false;
						break;
					}
				}
				if (found)
					return gen;
			}

			throw new InvalidOperationException("No generator found for prime = " + prime);
		}

		public static void FindInvertedPrimeFactors(ulong x, IEnumerable<ulong> primes, ulong[] array, out ulong length)
		{
			ulong len = 0;
			ulong mult = 1;
			foreach (var prime in primes)
			{
				if (prime >= x / 2 + 1) break;
				if (x % prime == 0)
				{
					array[len] = (x * mult) / prime;
					len++;

					x = x / prime;
					mult = mult * prime;

					if (x % prime == 0)
					{
						x = x / prime;
						mult = mult * prime;
					}
				}
			}
			length = len;
		}
	}
}
