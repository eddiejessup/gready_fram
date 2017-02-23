using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

// https://www.hackerrank.com/contests/projecteuler/challenges/euler146

namespace ConsoleApplication1.HackerRank
{
	class HackerRank14
	{
		public void Go()
		{
			Solve();
		}

		public void FindClosePrimes()
		{
			var L = 10000000ul;

			var primes = new List<ulong>();

			var count = 0;

			for (var i = 1ul; i < L; i++)
			{
				var ii = i * i;
				primes.Clear();
				for (var j = ii + 1; j <= ii + 40; j++)
					if (MillerRabinPrimality.IsPrime(j))
						primes.Add(j);
				if (primes.Count >= 6)
				{
					Console.WriteLine(new { i, ii });
					Console.WriteLine(primes.Join());
					Console.WriteLine();
					count++;
				}

				if (count >= 100)
					break;
			}
		}

		public static void Solve()
		{
			var additionals = new ulong[] { 1, 3, 7, 9, 13, 27 };
			//var additionals = new ulong[] { 6, 10, 16, 18, 28, 30, 36 };
			var L = 10000000ul;

			var sieve = MyMath.CreatePrimesSieve(1000ul);
			var firstFewPrimes = MyMath.ConvertSieveToPrimes(sieve).ToArray();
			var allowedRemainders = CalcAllowedRemainders(additionals, firstFewPrimes);
			var isPrimes = ConvertToIsPrimes(additionals);

			var sum = 0ul;
			for (var i = 1ul; i < L; i++)
			{
				if (!CheckAllowedRemainder(i, allowedRemainders, firstFewPrimes))
					continue;

				if (Check(i * i, additionals, isPrimes))
				{
					sum += i;
					//Console.WriteLine(i);
				}
			}

			Console.WriteLine(sum);
		}

		private static ulong GetMul(ulong[] firstFewPrimes, ulong[] additionals)
		{
			// В цикле можно перебирать только i: for (var i = mul; i < L; i += mul)

			var primeFactors = new List<ulong>();

			foreach (var prime in firstFewPrimes)
			{
				var isPrimeFactor = true;
				for (var b = 1ul; b < prime; b++)
				{
					var isBBad = additionals.Any(rem =>
					{
						var bb2 = (b * b + rem) % prime;
						return bb2 == 0;
					});

					if (!isBBad)
					{
						isPrimeFactor = false;
						break;
					}
				}

				if (isPrimeFactor)
					primeFactors.Add(prime);
			}

			return primeFactors.Mul();
		}

		private static bool[][] CalcAllowedRemainders(ulong[] additionals, ulong[] firstFewPrimes)
		{
			var allowedRemainders = new bool[firstFewPrimes.Length][];
			for (var i = 0; i < firstFewPrimes.Length; i++)
			{
				var prime = firstFewPrimes[i];
				var allowed = new bool[prime];

				for (var j = 0ul; j < prime; j++)
				{
					var jsqr = (j * j) % prime;
					allowed[j] = !additionals.Any(a => (jsqr + a) % prime == 0);
				}

				allowedRemainders[i] = allowed;
			}
			return allowedRemainders;
		}

		private static bool CheckAllowedRemainder(ulong n, bool[][] allowedRemainders, ulong[] firstFewPrimes)
		{
			for (var i = 0; i < firstFewPrimes.Length; i++)
			{
				var prime = firstFewPrimes[i];
				if (prime >= n)
					return true;

				if (!allowedRemainders[i][n % prime])
					return false;
			}

			return true;
		}

		static bool Check(ulong ii, ulong[] additionals, bool[] isPrimes)
		{
			var first = additionals[0];
			var last = additionals[additionals.Length - 1];

			for (var rem = first; rem <= last; rem++)
			{
				var mr = MillerRabinPrimality.IsPrime(ii + rem);
				if (mr != isPrimes[rem])
					return false;
			}
			return true;
		}

		private static bool[] ConvertToIsPrimes(ulong[] additionals)
		{
			var result = new bool[41];

			var first = additionals[0];
			var last = additionals[additionals.Length - 1];

			for (var i = first; i <= last; i++)
				result[i] = additionals.Contains(i);

			return result;
		}
	}
}
