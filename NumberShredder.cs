﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace lab1
{
    internal class NumberShredder
    {

        public static bool PrimeTest(long n, int k = 1)
        {
            if (n < 0) n = -n;
            if ((n == 2) || (n == 3) || (n == 5)) return true;
            if ((n % 2 == 0) || (n == 1)) return false;

            long x = 1;
            for (int i = 0; i < k; i++)
            {
                Random rand = new Random();
                x = rand.NextInt64(3, n / 2);
                if (MathUtils.GCD(x, n) > 1) return false;
                if (MathUtils.CheckStrongPrime(n, x)) return true;
            }

            return false;

        }


        public static Int64 Factorize(Int64 n)
        {
            return 0;
        }

        public static Int64[] CanonicalFactorization()
        {
            return Array.Empty<Int64>();
        }

        public static Int64 TrialDivisions(Int64 n)
        {
            for (Int64 d = 2; d < Math.Sqrt(n); d++)
            {
                if (n % d == 0) return d;
            }
            return 1;
        }

        // TODO: 3. from metodichka(theoreticly, this shit could not be working properly)
        public static long RhoMethod(long n, Func<long, long, long> f)
        {
            long x = 2;
            long y = 2;
            long d = 1;

            do
            {
                x = f(x, n);
                y = f(f(y, n), n);
                d = MathUtils.GCD(x - y, n);
            } while (d == 1);

            return d;

        }

        public static long BrilhartMorrison(long n)
        {
            StreamWriter sw = new("log.txt");
            Func<long, long> squareMod = x =>
            {
                Int128 t = x * x;
                x = MathUtils.Mod(t, n);
                if (x > n / 2)
                {
                    x -= n;
                }
                return x;
            };

            // Build factor base
            List<long> factorBase = [-1];

            sw.WriteLine("Number: {0}", n);

            long bound = (long)Math.Pow(Math.Exp(Math.Sqrt(Math.Log((double)n) * Math.Log(Math.Log((double)n)))), 1 / Math.Sqrt(2));
            for (long i = 2; i < bound; i++)
            {
                if (PrimeTest(i))
                {
                    var temp = MathUtils.LegandreSymbol(n, i);
                    if (temp == 0)
                    {
                        return i;
                    }
                    if (temp == 1)
                    {
                        factorBase.Add(i);
                    }
                }
            }

            sw.Write("FactorBase: {0}\n", string.Join(',', factorBase));
            sw.WriteLine();

            long v = 1;
            double alpha = Math.Sqrt(n);
            long a = (long)alpha;
            long u = a;

            sw.WriteLine("Starting coeffs: ");
            sw.WriteLine("  v:     {0}: ", v);
            sw.WriteLine("  alpha: {0}: ", alpha);
            sw.WriteLine("  a:     {0}: ", a);
            sw.WriteLine("  u:     {0}: ", u);

            List<long> bis = [0, 1, a];

            sw.WriteLine("Start calc b: \n\n");

            for (int i = 3; i < factorBase.Count * 2; i++)
            {
                sw.WriteLine("i: {0}:  ", i);
                v = (n - u * u) / v;
                alpha = (Math.Sqrt(n) + u) / v;
                a = (long)alpha;
                u = a * v - u;

                sw.WriteLine("  v:     {0}: ", v);
                sw.WriteLine("  alpha: {0}: ", alpha);
                sw.WriteLine("  a:     {0}: ", a);
                sw.WriteLine("  u:     {0}: ", u);

                long temp = MathUtils.Mod(a * bis[i - 1] + bis[i - 2], n);

                sw.WriteLine("  b:     {0}: ", temp);

                bis.Add(temp);
                sw.WriteLine();
            }

            List<bool[]> factorBis = [];

            bis.RemoveAt(0);
            bis.RemoveAt(0);

            sw.WriteLine("Start Calculating b^2 and b-smooth: ");

            for (int i = 0; i < bis.Count; i++)
            {

                bool[] arr = new bool[factorBase.Count];

                var b = squareMod(bis[i]);

                sw.WriteLine("(b^2)_{0}: {1}", i, b);

                arr[0] = b < 0;
                b = Math.Abs(b);


                for (int j = 1; j < factorBase.Count; j++)
                {
                    while (MathUtils.Mod(b, factorBase[j]) == 0)
                    {
                        arr[j] ^= true;
                        b /= factorBase[j];
                    }
                }

                if (b == 0)
                {
                    throw new Exception("CURSED SHIT!");
                }

                if (b != 1)
                {
                    bis[i] = 0;
                    continue;
                }

                factorBis.Add(arr);
            }

            bis.RemoveAll(x => x == 0);

            var matrix = new BitMatrix(factorBis);
            
            Console.WriteLine("FactorBase: {0}", string.Join(',', factorBase));
            Console.WriteLine("Bis: {0}", string.Join(',', bis));
            Console.WriteLine(matrix);
            sw.WriteLine("Matrix: {0}", matrix);

            sw.Close();

            var res = matrix.GetAllSolutions();

            foreach (var indexes in res)
            {
                long X = 1;
                long Y = 1;

                foreach (var e in indexes)
                {
                    var b = squareMod(bis[e]);

                    X *= bis[e];
                    Y *= b;

                    X = MathUtils.Mod(X, n);
                }

                Y = (long)Math.Sqrt(Y);
                Y = MathUtils.Mod(Y, n);

                if (X != Y && X != MathUtils.Mod(-Y, n))
                {
                    return MathUtils.GCD(X + Y, n);
                }
            }


            return 0;
        }

    }
}
