using System;
using System.Collections.Generic;

namespace Nvelope.Combinatorics
{
    public class Combination
    {
        // This class was adapted from one written and published by Dr. James McCaffrey on the following websites:
        // http://msdn.microsoft.com/en-gb/magazine/cc163957.aspx
        // http://msdn.microsoft.com/en-us/library/aa289166.aspx
        // This class stores only one combination. Upon creation, a combination C(n,k) will be initialized to the first
        // combination in the sequence: { 1,2,..,k-1,k }. You can iterate over all possible combinations by calling
        // Successor(), which will increment the combination to the next one in the sequence:
        // { 1,2,..,k-1,k+1 } { 1,2,..,k-1,k+2 } ... { 1,2,..,k-1,n }
        // { 1,2,..,k,k+1 } { 1,2,..,k,k+2 } ... { 1,2,..,k,n } etc
        
        //// Use case 1: generating and printing all possible poker hands
        //string[] deck = new string[] { "Ac", "Ad", "Ah", "As", "Kc", (...) };
        //Combination c = new Combination(52,5); // 52 cards, 5 at a time
        //string[] pokerHand = new string[5];
        //while (c != null)
        //{
        //  pokerHand = c.ApplyTo(deck);
        //  PrintHand(pokerHand);
        //  c = c.Successor();
        //}

        //// Use case 2: generating and printing random combinations based on an input array of integers
        //Console.WriteLine("\nThe random combinations are:");
        //// Allow duplicate random combinations for now.
        //Random rand = new Random();
        //for (int i = 0; i < numCombosToDisplay; i++)
        //{
        //    Combination c = new Combination(n, k, rand);
        //    c.MapTo(inputNumbers);
        //    Console.WriteLine(i + 1 + ": " + c.ToString());
        //}

        //// Use case 3: generating and printing in-order combinations based on an input array of integers
        //Console.WriteLine("\nThe in-order combinations are:");
        //Combination c = new Combination(n, k);
        //for (int i = 0; i < numCombosToDisplay; i++)
        //{
        //    if (c == null) break;
        //
        //    Combination numbers = new Combination(n, k, c.ApplyTo(inputNumbers));
        //    Console.WriteLine(i + 1 + ": " + numbers.ToString());
        //    c = c.Successor();
        //}

        // Use case 4:
        /// <summary>
        /// Generates and prints all n-digit combinations of 0 and 1
        /// </summary>
        /// <param name="n"></param>
        //private static void GenerateAndPrintAllBooleanCombos(int n)
        //{
        //    for (int k = 0; k <= n; k++)
        //    {
        //        Combination c = new Combination(n, k);

        //        // Define our Boolean combo array. Any given index in this array will be FALSE unless that index
        //        // number is in the combination, in which case it will be set to TRUE
        //        bool[] b = new bool[n]; // initializes to false by default

        //        Console.WriteLine();
        //        Console.WriteLine("Choose({0},{1}):", n, k);
        //        while (c != null)
        //        {
        //            // Initialize boolean array to false
        //            for (int i = 0; i < n; i++)
        //            {
        //                b[i] = false;
        //            }

        //            long[] data = c.GetData(); // the k indices we have chosen to be TRUE
        //            for (int i = 0; i < k; i++)
        //            {
        //                b[data[i]] = true;
        //            }

        //            // Print contents of b[]
        //            for (int i = 0; i < n; i++)
        //            {
        //                Console.Write(b[i] ? 1 : 0);
        //            }
        //            Console.WriteLine();

        //            c = c.Successor();
        //        }
        //    }
        //}

        private long n = 0;
        private long k = 0;
        private long[] data;

        public Combination(long n, long k)
        {
            Validate(n, k);
            this.n = n;
            this.k = k;

            this.data = new long[k];
            for (long i = 0; i < k; ++i)
                this.data[i] = i;
        } // Combination(n,k)

        /// <summary>
        /// Creates a combination whose data values are set to input array a. Objects created using this constructor might have
        /// non-lexicographic data and fail IsValid(), so don't call Successor() method if that is the case.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="k"></param>
        /// <param name="a"></param>
        public Combination(long n, long k, long[] a) // Combination from a[]
        {
            this.n = n;
            this.k = k;

            if (k != a.Length)
                throw new Exception("Array length does not equal k");

            this.data = new long[k];
            for (long i = 0; i < a.Length; ++i)
                this.data[i] = a[i];
        } // Combination(n,k,a)

        /// <summary>
        /// Constructs a random combination using one of two methods:
        /// Old way: generate k non-repeating integers between 0 and n-1 (inclusive) as the combination data.
        /// The combination will most likely not be in lexicographic order and will fail IsValid().
        /// So don't call Successor() on it.
        /// New way: generate a random combination element number between 0 and Choose(n,k)-1 and return that element
        /// Combinations generated in this manner will be in lexicographic order.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="k"></param>
        /// <param name="rand"></param>
        public Combination(long n, long k, Random rand, bool oldWay = false)
        {
            Validate(n, k);
            this.n = n;
            this.k = k;

            this.data = new long[k];
            if (oldWay)
            {
                /// TODO: Add boolean flag to put data in lexicographic order or not.
                long[] a = Random((int)n, (int)k, rand);
                for (long i = 0; i < a.Length; ++i)
                    this.data[i] = a[i];
            }
            else
            {
                // Get a random element number, convert it to the range [0, Choose(n,k)-1] and assign this.data to that
                // element. TODO: Change element function to be a static function taking n, k, m, and returning long[]
                long newMin = 0;
                long newMax = Combination.Choose(n, k) - 1;

                long r = Combination.RandomInt64(rand);
                Combination c = new Combination(n, k);
                this.data = c.Element(Combination.ConvertBetweenRanges(r, Int64.MinValue, Int64.MaxValue, newMin, newMax)).data;
            }
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="c"></param>
        public Combination(Combination c)
        {
            if (!c.IsValid())
                throw new Exception("Attempt to copy invalid Combination object");
            this.n = c.n;
            this.k = c.k;
            this.data = new long[c.data.Length];
            Array.Copy(c.data, this.data, c.data.Length);
        }

        private void Validate(long n, long k)
        {
            if (!IsValid(n, k)) // normally n >= k
                throw new Exception("N and/or K is negative");
        }

        /// <summary>
        /// Returns false if either n or k are negative, and true otherwise
        /// </summary>
        /// <returns></returns>
        private bool IsValid(long n, long k)
        {
            return (n < 0 || k < 0) ? false : true;
        }

        public bool IsValid()
        {
            if (!IsValid(this.n, this.k))
                return false;

            if (this.data.Length != this.k)
                return false; // corrupted

            for (long i = 0; i < this.k; ++i)
            {
                if (this.data[i] < 0 || this.data[i] > this.n - 1)
                    return false; // value out of range

                for (long j = i + 1; j < this.k; ++j)
                    if (this.data[i] >= this.data[j])
                        return false; // duplicate or not lexicographic
            }

            return true;
        } // IsValid()

        public long[] GetData()
        {
            return data;
        }

        public override string ToString()
        {
            string s = String.Empty;
            for (long i = 0; i < data.Length; ++i)
                s += data[i].ToString() + " ";
            return s;
        } // ToString()

        public Combination Successor()
        {
            if (!this.IsValid())
                throw new Exception("Calling Successor() on invalid combination object");

            if (this.data.Length == 0 || this.data[0] == this.n - this.k)
                return null;

            Combination ans = new Combination(this.n, this.k);

            long i;
            for (i = 0; i < this.k; ++i)
                ans.data[i] = this.data[i];

            for (i = this.k - 1; i > 0 && ans.data[i] == this.n - this.k + i; --i)
                ;

            ++ans.data[i];

            for (long j = i; j < this.k - 1; ++j)
                ans.data[j + 1] = ans.data[j] + 1;

            return ans;
        } // Successor()

        /// <summary>
        /// This function returns an array of k random non-repeating integers between 0 and n-1 (inclusive).
        /// The output array will most likely not be in lexicographic order.
        /// So don't create a combination using this data and call Successor() on it.
        /// TODO: Add boolean flag to put data in lexicographic order or not.
        /// TODO: Change this random method to just pick one random combination element between 1 and Choose(n,k) and
        /// return that element (once we have a random function that works well for longs)
        /// </summary>
        /// <returns></returns>
        private long[] Random(int n, int k, Random rand)
        {
            long[] ans = new long[k];

            // Create and initialize list of n numbers 0..n-1 to pick randomly from.
            List<int> remainingNums = new List<int>(n);
            for (int i = 0; i < n; i++)
            {
                remainingNums.Add(i);
            }

            // Pick k distinct indices. Distinctness is achieved by removing the number picked from remainingNums
            // See social.msdn.microsoft.com/Forums/en-US/Vsexpressvcs/thread/b935430a-5063-4540-a9da-18c31620199a/ for more info
            for (int i = 0; i < k; i++)
            {
                // Get a random element of remainingNums and assign index[j] the value of that element
                // Note rand.Next returns values less than maxValue, so we use remainingNums.Count instead of remainingNums.Count - 1
                int r = rand.Next(0, remainingNums.Count);
                ans[i] = remainingNums[r];
                remainingNums.RemoveAt(r);
            }

            return ans;
        } // Random()
        
        /// <summary>
        /// Returns a random Int64.
        /// </summary>
        /// <param name="rnd"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static long RandomInt64(Random rnd, bool positive = false)
        {
            // This method is based on the solution at http://stackoverflow.com/questions/677373/generate-random-values-in-c-sharp 
            byte[] buffer = new byte[sizeof(Int64)];
            rnd.NextBytes(buffer);
            long r = BitConverter.ToInt64(buffer, 0);
            return positive && r < 0 ? r * -1 : r;
        }

        /// <summary>
        /// Converts oldValue in the old range to a new value in the new range, maintaining ratio
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="oldMin"></param>
        /// <param name="oldMax"></param>
        /// <param name="newMin"></param>
        /// <param name="newMax"></param>
        /// <returns></returns>
        public static long ConvertBetweenRanges(long oldValueL, long oldMinL, long oldMaxL, long newMinL, long newMaxL)
        {
            // This method is based on the solution at http://stackoverflow.com/questions/929103/convert-a-number-range-to-another-range-maintaining-ratio
            if (oldValueL < oldMinL || oldValueL > oldMaxL)
                throw new Exception("oldValue out of range when converting between ranges");

            // Convert all longs to doubles before proceeding so we don't get any weird math errors
            double oldValue = (double)oldValueL;
            double oldMin = (double)oldMinL;
            double oldMax = (double)oldMaxL;
            double newMin = (double)newMinL;
            double newMax = (double)newMaxL;

            double d = ((oldValue - oldMin) / (oldMax - oldMin)) * (newMax - newMin) + newMin;
            return (long)Math.Round(d, 0);
        }

        public static long Choose(long n, long k)
        {
            if (n < 0 || k < 0)
                throw new Exception("Invalid negative parameter in Choose()");
            if (n < k)
                return 0;  // special case
            if (n == k || k == 0)
                return 1;

            long delta, iMax;

            if (k < n - k) // ex: Choose(100,3)
            {
                delta = n - k;
                iMax = k;
            }
            else         // ex: Choose(100,97)
            {
                delta = k;
                iMax = n - k;
            }

            long ans = delta + 1;

            for (long i = 2; i <= iMax; ++i)
            {
                checked { ans = (ans * (delta + i)) / i; }
            }

            return ans;
        } // Choose()

        // return the mth lexicographic element of combination C(n,k)
        public Combination Element(long m)
        {
            long[] ans = new long[this.k];

            long a = this.n;
            long b = this.k;
            long x = (Choose(this.n, this.k) - 1) - m; // x is the "dual" of m

            for (long i = 0; i < this.k; ++i)
            {
                ans[i] = LargestV(a, b, x); // largest value v, where v < a and vCb < x    
                x = x - Choose(ans[i], b);
                a = ans[i];
                b = b - 1;
            }

            for (long i = 0; i < this.k; ++i)
            {
                ans[i] = (n - 1) - ans[i];
            }

            Combination c = new Combination(this.n, this.k, ans);
            if (!c.IsValid())
                throw new Exception("Bad value from array");
            return c;
        } // Element()

        // return largest value v where v < a and  Choose(v,b) <= x
        private static long LargestV(long a, long b, long x)
        {
            long v = a - 1;

            while (Choose(v, b) > x)
                --v;

            return v;
        } // LargestV()

        // maps the current element (this) onto the input STRING array and returns it
        public string[] ApplyTo(string[] strarr)
        {
            ValidateArrayLength(strarr.Length);

            string[] result = new string[this.k];

            for (long i = 0; i < result.Length; ++i)
                result[i] = strarr[this.data[i]];

            return result;
        } // ApplyTo

        // maps the current combination (this) onto the input INT array and returns it
        public long[] ApplyTo(int[] intarr)
        {
            ValidateArrayLength(intarr.Length);

            long[] result = new long[this.k];

            for (long i = 0; i < result.Length; ++i)
                result[i] = intarr[this.data[i]];

            return result;
        } // ApplyTo

        /// <summary>
        /// Modifies THIS combination data by mapping it onto the input array using the existing combination data as indices.
        /// After calling this method, the object might fail IsValid(), so don't call Successor() method if that is the case.
        /// </summary>
        /// <param name="intarr"></param>
        public void MapTo(int[] intarr)
        {
            ValidateArrayLength(intarr.Length);

            for (long i = 0; i < this.data.Length; ++i)
                this.data[i] = intarr[this.data[i]];
        }

        private void ValidateArrayLength(int arrayLength)
        {
            if (arrayLength < this.n)
                throw new Exception("Bad array size");
        }
    } // Combination class
}
