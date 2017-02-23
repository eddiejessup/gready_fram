namespace ConsoleApplication1.DataTypes
{
	/// <summary>
	/// 	Binary Indexed Tree(BIT), in its vanilla form, is a data-structure which helps us 
	/// 	to answer the following types of queries, on an array A[1..n], efficiently:
	/// 	
	/// 	Set(i, val) - This adds `val` to A[i]       O(log n)
	/// 	
	/// 	Get(i) - This returns ∑ (j = 1..i) A[i]		O(log n)
	/// </summary>
	public class BinaryIndexedTree
	{
		public long[] Array;
		public int N;

		public BinaryIndexedTree(int n)
		{
			N = n;
			Array = new long[n + 1];
		}

		/// <summary>
		/// i and j are starting and ending index, inclusive
		/// </summary>
		public long Get(int i, int j)
		{
			long sum = 0L;
			while (j > 0)
			{
				sum += Array[j];
				j -= j & -j;
			}
			i--;
			while (i > 0)
			{
				sum -= Array[i];
				i -= i & -i;
			}
			return sum;
		}

		/// <summary>
		/// Update array
		/// </summary>
		/// <param name="i">i is the index to be updated</param>
		/// <param name="diff">diff is (new_val - old_val) i.e. if want to increase diff is +ive and if want to decrease -ive</param>
		public void SetDiff(int i, long diff)
		{
			while (i <= N)
			{
				Array[i] += diff;
				i += i & -i;
			}
		}
	}
}
