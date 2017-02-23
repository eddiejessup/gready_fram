using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.HackerRank
{
	class HackerRank39
	{
		public void Go()
		{
			Console.WriteLine(Solve(1, 2, 3, 2, 2, 1, 1, 4, 10));
		}

		public static string Solve(int a, int b, int c, ulong d, int e, int f, int g, ulong h, ulong n)
		{
			var mod = 1000000000ul;

			var k = new[] { a, b, c, e, f, g }.Max() - 1;

			var xni = k;
			var yni = 2 * k + 1;
			var di = 2 * k + 2;
			var hi = 2 * k + 4;

			var F = new ulong[2 * k + 6];
			for (var i = 0; i <= yni; i++)
				F[i] = 1;

			F[xni] = 3;
			F[yni] = 3;
			F[di] = d;
			F[di + 1] = d;
			F[hi] = h;
			F[hi + 1] = h;

			var T = new ulong[F.Length, F.Length];
			for (var i = 0; i <= 2 * k + 1; i++)
				T[i, i + 1] = 1;

			T[xni, xni + 1] = 0;
			T[xni, xni - a + 1]++;
			T[xni, yni - b + 1]++;
			T[xni, yni - c + 1]++;
			T[xni, di + 1] = 1;

			T[yni, yni + 1] = 0;
			T[yni, yni - e + 1]++;
			T[yni, xni - f + 1]++;
			T[yni, xni - g + 1]++;
			T[yni, hi + 1] = 1;

			T[di, di] = d;
			T[di + 1, di] = d;
			T[di + 1, di + 1] = d;

			T[hi, hi] = h;
			T[hi + 1, hi] = h;
			T[hi + 1, hi + 1] = h;

			var TpowN = T.MatrixPow(n, mod);

			var R = TpowN.MatrixMul(F, mod);

			//	Console.WriteLine(string.Join(" ", TpowN.MatrixMul(F, mod)));

			return string.Concat(R[xni], " ", R[yni]);
		}
	}
}


/* Uwi solution:

import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.PrintWriter;
import java.util.Arrays;
import java.util.InputMismatchException;

public class C {
	InputStream is;
	PrintWriter out;
	String INPUT = "";
	
	void solve()
	{
		for(int T = ni();T > 0;T--){
			int a = ni(), b = ni(), c = ni(), d = ni(), e = ni(), f = ni(), g = ni(), h = ni();
			long n = nl();
			int[][] M = new int[10*2+2+2][10*2+2+2];
			for(int i = 1;i < 10;i++){
				M[i][i-1] = 1;
				M[i+10][i-1+10] = 1;
			}
			M[0][a-1] = 1; M[10][10+e-1] = 1;
			M[0][10+b-1]++; M[10][f-1]++;
			M[0][10+c-1]++; M[10][g-1]++;
			M[0][20]++; M[10][22]++;
			M[20][20] = d;
			M[20][21] = d;
			M[21][21] = d;
			M[22][22] = h;
			M[22][23] = h;
			M[23][23] = h;
			
			int[] v = new int[24];
			Arrays.fill(v, 1);
			v[20] = 0; v[21] = 1;
			v[22] = 0; v[23] = 1;
			v = pow(M, v, n+1);
			out.println(v[0] + " " + v[10]);
		}
	}
	
	public static final int mod = 1000000000;
	public static final long m2 = (long)mod*mod;
	public static final long BIG = 8L*m2;
	
	// A^e*v
	public static int[] pow(int[][] A, int[] v, long e)
	{
		for(int i = 0;i < v.length;i++){
			if(v[i] >= mod)v[i] %= mod;
		}
		int[][] MUL = A;
		for(;e > 0;e>>>=1) {
			if((e&1)==1)v = mul(MUL, v);
			MUL = p2(MUL);
		}
		return v;
	}
	
	// int matrix*int vector
	public static int[] mul(int[][] A, int[] v)
	{
		int m = A.length;
		int n = v.length;
		int[] w = new int[m];
		for(int i = 0;i < m;i++){
			long sum = 0;
			for(int k = 0;k < n;k++){
				sum += (long)A[i][k] * v[k];
				if(sum >= BIG)sum -= BIG;
			}
			w[i] = (int)(sum % mod);
		}
		return w;
	}
	
	// int matrix^2 (be careful about negative value)
	public static int[][] p2(int[][] A)
	{
		int n = A.length;
		int[][] C = new int[n][n];
		for(int i = 0;i < n;i++){
			long[] sum = new long[n];
			for(int k = 0;k < n;k++){
				for(int j = 0;j < n;j++){
					sum[j] += (long)A[i][k] * A[k][j];
					if(sum[j] >= BIG)sum[j] -= BIG;
				}
			}
			for(int j = 0;j < n;j++){
				C[i][j] = (int)(sum[j] % mod);
			}
		}
		return C;
	}
	
	void run() throws Exception
	{
		is = INPUT.isEmpty() ? System.in : new ByteArrayInputStream(INPUT.getBytes());
		out = new PrintWriter(System.out);
		
		long s = System.currentTimeMillis();
		solve();
		out.flush();
		if(!INPUT.isEmpty())tr(System.currentTimeMillis()-s+"ms");
	}
	
	public static void main(String[] args) throws Exception { new C().run(); }
	
	private byte[] inbuf = new byte[1024];
	private int lenbuf = 0, ptrbuf = 0;
	
	private int readByte()
	{
		if(lenbuf == -1)throw new InputMismatchException();
		if(ptrbuf >= lenbuf){
			ptrbuf = 0;
			try { lenbuf = is.read(inbuf); } catch (IOException e) { throw new InputMismatchException(); }
			if(lenbuf <= 0)return -1;
		}
		return inbuf[ptrbuf++];
	}
	
	private boolean isSpaceChar(int c) { return !(c >= 33 && c <= 126); }
	private int skip() { int b; while((b = readByte()) != -1 && isSpaceChar(b)); return b; }
	
	private double nd() { return Double.parseDouble(ns()); }
	private char nc() { return (char)skip(); }
	
	private String ns()
	{
		int b = skip();
		StringBuilder sb = new StringBuilder();
		while(!(isSpaceChar(b))){ // when nextLine, (isSpaceChar(b) && b != ' ')
			sb.appendCodePoint(b);
			b = readByte();
		}
		return sb.toString();
	}
	
	private char[] ns(int n)
	{
		char[] buf = new char[n];
		int b = skip(), p = 0;
		while(p < n && !(isSpaceChar(b))){
			buf[p++] = (char)b;
			b = readByte();
		}
		return n == p ? buf : Arrays.copyOf(buf, p);
	}
	
	private char[][] nm(int n, int m)
	{
		char[][] map = new char[n][];
		for(int i = 0;i < n;i++)map[i] = ns(m);
		return map;
	}
	
	private int[] na(int n)
	{
		int[] a = new int[n];
		for(int i = 0;i < n;i++)a[i] = ni();
		return a;
	}
	
	private int ni()
	{
		int num = 0, b;
		boolean minus = false;
		while((b = readByte()) != -1 && !((b >= '0' && b <= '9') || b == '-'));
		if(b == '-'){
			minus = true;
			b = readByte();
		}
		
		while(true){
			if(b >= '0' && b <= '9'){
				num = num * 10 + (b - '0');
			}else{
				return minus ? -num : num;
			}
			b = readByte();
		}
	}
	
	private long nl()
	{
		long num = 0;
		int b;
		boolean minus = false;
		while((b = readByte()) != -1 && !((b >= '0' && b <= '9') || b == '-'));
		if(b == '-'){
			minus = true;
			b = readByte();
		}
		
		while(true){
			if(b >= '0' && b <= '9'){
				num = num * 10 + (b - '0');
			}else{
				return minus ? -num : num;
			}
			b = readByte();
		}
	}
	
	private static void tr(Object... o) { System.out.println(Arrays.deepToString(o)); }
}
*/
