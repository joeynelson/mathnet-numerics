// <copyright file="Histogram.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2009 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

namespace MathNet.Numerics.Statistics
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Properties;

    /// <summary>
    /// A <see cref="Histogram"/> consists of a series of <see cref="Bucket"/>s, 
    /// each representing a region limited by a lower bound (exclusive) and an upper bound (inclusive).
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class Bucket :
#if SILVERLIGHT
   IComparable<Bucket>
#else
    IComparable<Bucket>, ICloneable
#endif        
    {
        /// <summary>
        /// This <c>IComparer</c> performs comparisons between a point and a bucket.
        /// </summary>
        private sealed class PointComparer : IComparer<Bucket>
        {
            /// <summary>
            /// Compares a point and a bucket. The point will be encapsulated in a bucket with width 0.
            /// </summary>
            /// <param name="bkt1">The first bucket to compare.</param>
            /// <param name="bkt2">The second bucket to compare.</param>
            /// <returns>-1 when the point is less than this bucket, 0 when it is in this bucket and 1 otherwise.</returns>
            public int Compare(Bucket bkt1, Bucket bkt2)
            {
                if (bkt2.Width == 0.0)
                {
                    return -bkt1.Contains(bkt2.UpperBound);
                }
                else
                {
                    return -bkt2.Contains(bkt1.UpperBound);
                }
            }
        }

        private static PointComparer pointComparer = new PointComparer();

        /// <summary>
        /// Lower Bound of the Bucket.
        /// </summary>
        public double LowerBound { get; set; }

        /// <summary>
        /// Upper Bound of the Bucket.
        /// </summary>
        public double UpperBound { get; set; }

        /// <summary>
        /// The number of datapoints in the bucket.
        /// </summary>
        public double Count { get; set; }

        /// <summary>
        /// Initializes a new instance of the Bucket class.
        /// </summary>
        public Bucket(double lowerBound, double upperBound) : this(lowerBound, upperBound, 0.0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Bucket class.
        /// </summary>
        public Bucket(double lowerBound, double upperBound, double count)
        {
            if (lowerBound > upperBound)
            {
                throw new ArgumentOutOfRangeException(Resources.ArgumentLowerBoundLargerThanUpperBound);
            }

            if (count < 0.0)
            {
                throw new ArgumentOutOfRangeException(Resources.ArgumentMustBePositive);
            }

            LowerBound = lowerBound;
            UpperBound = upperBound;
            Count = count;
        }

        /// <summary>
        /// Creates a copy of the Bucket with the lowerbound, upperbound and counts exactly equal.
        /// </summary>
        /// <returns>A cloned Bucket object.</returns>
        public object Clone()
        {
            return new Bucket(LowerBound, UpperBound, Count);
        }

        /// <summary>
        /// Width of the Bucket.
        /// </summary>
        public double Width
        {
            get { return UpperBound - LowerBound; }
        }

        /// <summary>
        /// Default comparer.
        /// </summary>
        public static IComparer<Bucket> DefaultPointComparer
        {
            get { return pointComparer; }
        }

        /// <summary>
        /// This method check whether a point is contained within this bucket.
        /// </summary>
        /// <param name="x">The point to check.</param>
        /// <returns>0 if the point falls within the bucket boundaries; -1 if the point is
        /// smaller than the bucket, +1 if the point is larger than the bucket.</returns>
        public int Contains(double x)
        {
            if (LowerBound < x)
            {
                if (UpperBound >= x)
                {
                    return 0;
                }

                return 1;
            }

            return -1;
        }

        /// <summary>
        /// Comparison of two disjoint buckets. The buckets cannot be overlapping.
        /// </summary>
        public int CompareTo(Bucket bucket)
        {
            if (this.UpperBound > bucket.LowerBound && this.LowerBound < bucket.LowerBound)
            {
                throw new ArgumentException(Resources.PartialOrderException);
            }

            if (Precision.AlmostEqual(this.UpperBound, bucket.UpperBound) 
                && Precision.AlmostEqual(this.LowerBound, bucket.LowerBound))
            {
                return 0;
            }

            if (bucket.UpperBound <= this.LowerBound)
            {
                return 1;
            }

            return -1;
        }

        /// <summary>
        /// Checks whether two Buckets are equal; this method tolerates a difference in lowerbound, upperbound
        /// and count given by <seealso cref="Precsion.AlmostEqual"/>. 
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is Bucket))
            {
                return false;
            }

            Bucket b = (Bucket) obj;
            return Precision.AlmostEqual(this.LowerBound, b.LowerBound)
                && Precision.AlmostEqual(this.UpperBound, b.UpperBound)
                && Precision.AlmostEqual(this.Count, b.Count);
        }

        /// <summary>
        /// Provides a hash code for this bucket.
        /// </summary>
        public override int GetHashCode()
        {
            return LowerBound.GetHashCode() ^ UpperBound.GetHashCode() ^ Count.GetHashCode();
        }

        /// <summary>
        /// Formats a human-readable string for this bucket.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "(" + this.LowerBound + ";" + this.UpperBound + "] = " + this.Count;
        }
    }
    
    /// <summary>
    /// A class which computes histograms of data.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class Histogram
    {
        /// <summary>
        /// Contains all the <c>Bucket</c>s of the <c>Histogram</c>.
        /// </summary>
        private List<Bucket> buckets;

        /// <summary>
        /// Indicates whether the elements of <c>buckets</c> are currently sorted.
        /// </summary>
        private bool areBucketsSorted;

        /// <summary>
        /// Initializes a new instance of the Histogram class.
        /// </summary>
        public Histogram()
        {
            buckets = new List<Bucket>();
            areBucketsSorted = true;
        }

        /// <summary>
        /// Constructs a Histogram with a specific number of equally sized buckets. The upper and lower bound of the histogram
        /// will be set to the smallest and largest datapoint.
        /// </summary>
        /// <param name="data">The datasequence to build a histogram on.</param>
        /// <param name="nbuckets">The number of buckets to use.</param>
        public Histogram(IEnumerable<double> data, int nbuckets) 
            : this()
        {
            if (nbuckets < 1)
            {
                throw new ArgumentOutOfRangeException("The number of bins in a histogram should be at least 1.");
            }

            double lower = data.Minimum();
            double upper = data.Maximum();
            double width = (upper - lower) / nbuckets;

            // Add buckets for each bin; the smallest bucket's lowerbound must be slightly smaller
            // than the minimal element.
            AddBucket(new Bucket(lower.Decrement(), lower + width));
            for (int n = 1; n < nbuckets; n++)
            {
                AddBucket(new Bucket(lower + n * width, lower + (n + 1) * width));
            }

            AddData(data);
        }

        /// <summary>
        /// Constructs a Histogram with a specific number of equally sized buckets.
        /// </summary>
        /// <param name="data">The datasequence to build a histogram on.</param>
        /// <param name="nbuckets">The number of buckets to use.</param>
        /// <param name="lower">The histogram lower bound.</param>
        /// <param name="upper">The histogram upper bound.</param>
        public Histogram(IEnumerable<double> data, int nbuckets, double lower, double upper)
            : this()
        {
            if (lower > upper)
            {
                throw new ArgumentOutOfRangeException("The histogram lowerbound must be smaller than the upper bound.");
            }

            if (nbuckets < 1)
            {
                throw new ArgumentOutOfRangeException("The number of bins in a histogram should be at least 1.");
            }

            double width = (upper - lower) / nbuckets;

            // Add buckets for each bin.
            for (int n = 0; n < nbuckets; n++)
            {
                AddBucket(new Bucket(lower + n * width, lower + (n + 1) * width));
            }

            AddData(data);
        }

        /// <summary>
        /// Add one data point to the histogram. If the datapoint falls outside the range of the histogram,
        /// the lowerbound or upperbound will automatically adapt.
        /// </summary>
        /// <param name="d">The datapoint which we want to add.</param>
        public void AddData(double d)
        {
            // Sort if needed.
            LazySort();

            if (d < this.LowerBound)
            {
                // Make the lower bound just slightly smaller than the datapoint so it is contained in this bucket.
                buckets[0].LowerBound = d.Decrement();
                buckets[0].Count++;
            }
            else if (d > this.UpperBound)
            {
                buckets[BucketCount - 1].UpperBound = d;
                buckets[BucketCount - 1].Count++;
            }
            else
            {
                buckets[GetBucketIndexOf(d)].Count++;
            }
        }

        /// <summary>
        /// Add a sequence of data point to the histogram. If the datapoint falls outside the range of the histogram,
        /// the lowerbound or upperbound will automatically adapt.
        /// </summary>
        /// <param name="data">The sequence of datapoints which we want to add.</param>
        public void AddData(IEnumerable<double> data)
        {
            foreach (double d in data)
            {
                AddData(d);
            }
        }

        /// <summary>
        /// Adds a <c>Bucket</c> to the <c>Histogram</c>.
        /// </summary>
        public void AddBucket(Bucket bucket)
        {
            buckets.Add(bucket);
            areBucketsSorted = false;
        }

        /// <summary>
        /// Sort the buckets if needed.
        /// </summary>
        private void LazySort()
        {
            if (!areBucketsSorted)
            {
                buckets.Sort();
                areBucketsSorted = true;
            }
        }

        /// <summary>
        /// Returns the <c>Bucket</c> that contains the value <c>v</c>. 
        /// </summary>
        /// <param name="v">The point to search the bucket for.</param>
        /// <returns>A copy of the bucket containing point <paramref name="v"/>.</returns>
        public Bucket GetBucketOf(double v)
        {
            return (Bucket) buckets[GetBucketIndexOf(v)].Clone();
        }

        /// <summary>
        /// Returns the index in the <c>Histogram</c> of the <c>Bucket</c>
        /// that contains the value <c>v</c>.
        /// </summary>
        /// <param name="v">The point to search the bucket index for.</param>
        /// <returns>The index of the bucket containing the point.</returns>
        public int GetBucketIndexOf(double v)
        {
            // Sort if needed.
            LazySort();

            // Binary search for the bucket index.
            int index = buckets.BinarySearch(new Bucket(v, v), Bucket.DefaultPointComparer);

            if (index < 0)
            {
                throw new ArgumentException(Resources.ArgumentHistogramContainsNot);
            }

            return index;
        }

        /// <summary>
        /// Returns the lower bound of the histogram.
        /// </summary>
        public double LowerBound
        {
            get
            {
                LazySort();
                return buckets[0].LowerBound;
            }
        }

        /// <summary>
        /// Returns the upper bound of the histogram.
        /// </summary>
        public double UpperBound
        {
            get
            {
                LazySort();
                return buckets[buckets.Count - 1].UpperBound;
            }
        }

        /// <summary>
        /// Gets the <c>n</c>'th bucket.
        /// </summary>
        /// <param name="n">The index of the bucket to be returned.</param>
        /// <returns>A copy of the <c>n</c>'th bucket.</returns>
        public Bucket this[int n]
        {
            get
            {
                LazySort();
                return (Bucket) buckets[n].Clone();
            }
        }
        
        /// <summary>
        /// Gets the number of buckets.
        /// </summary>
        public int BucketCount
        {
            get { return buckets.Count; }
        }

        /// <summary>
        /// Gets the total number of datapoints in the histogram.
        /// </summary>
        public double DataCount
        {
            get
            {
                double totalCount = 0;
               
                for (int i = 0; i < this.BucketCount; i++)
                {
                    totalCount += this[i].Count;
                }

                return totalCount;
            }
        }

        /// <summary>
        /// Prints the buckets contained in the <see cref="Histogram"/>.
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
           
            foreach (Bucket b in buckets)
            {
                sb.Append(b.ToString());
            }

            return sb.ToString();
        }
    }
}
