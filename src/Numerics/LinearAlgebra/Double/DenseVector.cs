﻿// <copyright file="DenseVector.cs" company="Math.NET">
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

namespace MathNet.Numerics.LinearAlgebra.Double
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using NumberTheory;
    using Properties;
    using Threading;

    /// <summary>
    /// A vector using dense storage.
    /// </summary>
    public class DenseVector : Vector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DenseVector"/> class with a given size.
        /// </summary>
        /// <param name="size">
        /// the size of the vector.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="size"/> is less than one.
        /// </exception>
        public DenseVector(int size)
            : base(size)
        {
            Data = new double[size];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DenseVector"/> class with a given size
        /// and each element set to the given value;
        /// </summary>
        /// <param name="size">
        /// the size of the vector.
        /// </param>
        /// <param name="value">
        /// the value to set each element to.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="size"/> is less than one.
        /// </exception>
        public DenseVector(int size, double value)
            : this(size)
        {
            for (var index = 0; index < Data.Length; index++)
            {
                Data[index] = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DenseVector"/> class by
        /// copying the values from another.
        /// </summary>
        /// <param name="other">
        /// The vector to create the new vector from.
        /// </param>
        public DenseVector(Vector other)
            : this(other.Count)
        {
            var vector = other as DenseVector;
            if (vector == null)
            {
                Parallel.For(0, Count, index => this[index] = other[index]);
            }
            else
            {
                Buffer.BlockCopy(vector.Data, 0, Data, 0, Data.Length * Constants.SizeOfDouble);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DenseVector"/> class by
        /// copying the values from another.
        /// </summary>
        /// <param name="other">
        /// The vector to create the new vector from.
        /// </param>
        public DenseVector(DenseVector other)
            : this(other.Count)
        {
            Buffer.BlockCopy(other.Data, 0, Data, 0, Data.Length * Constants.SizeOfDouble);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DenseVector"/> class for an array.
        /// </summary>
        /// <param name="array">The array to create this vector from.</param>
        /// <remarks>The vector does not copy the array, but keeps a reference to it. Any 
        /// changes to the vector will also change the array.</remarks>
        public DenseVector(double[] array) : base(array.Length)
        {
            Data = array;
        }

        /// <summary>
        ///  Gets the vector's internal data.
        /// </summary>
        /// <value>The vector's internal data.</value>
        /// <remarks>Changing values in the array also changes the corresponding value in vector. Use with care.</remarks>
        internal double[] Data
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns a reference to the internal data structure.
        /// </summary>
        /// <param name="vector">The DenseVector whose internal data we are
        /// returning.</param>
        /// <returns>
        /// A reference to the internal date of the given vector.
        /// </returns>
        public static implicit operator double[](DenseVector vector)
        {
            if (vector == null)
            {
                throw new ArgumentNullException();
            }

            return vector.Data;
        }

        /// <summary>
        /// Returns a vector bound directly to a reference of the provided array.
        /// </summary>
        /// <param name="array">The array to bind to the DenseVector object.</param>
        /// <returns>
        /// A DenseVector whose values are bound to the given array.
        /// </returns>
        public static implicit operator DenseVector(double[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException();
            }

            return new DenseVector(array);
        }

        /// <summary>
        /// Create a matrix based on this vector in column form (one single column).
        /// </summary>
        /// <returns>This vector as a column matrix.</returns>
        public override Matrix ToColumnMatrix()
        {
            var matrix = new DenseMatrix(Count, 1);
            for (var i = 0; i < Data.Length; i++)
            {
                matrix[i, 0] = Data[i];
            }

            return matrix;
        }

        /// <summary>
        /// Create a matrix based on this vector in row form (one single row).
        /// </summary>
        /// <returns>This vector as a row matrix.</returns>
        public override Matrix ToRowMatrix()
        {
            var matrix = new DenseMatrix(1, Count);
            for (var i = 0; i < Data.Length; i++)
            {
                matrix[0, i] = Data[i];
            }

            return matrix;
        }

        /// <summary>Gets or sets the value at the given <paramref name="index"/>.</summary>
        /// <param name="index">The index of the value to get or set.</param>
        /// <returns>The value of the vector at the given <paramref name="index"/>.</returns> 
        /// <exception cref="IndexOutOfRangeException">If <paramref name="index"/> is negative or 
        /// greater than the size of the vector.</exception>
        public override double this[int index]
        {
            get
            {
                return Data[index];
            }

            set
            {
                Data[index] = value;
            }
        }

        /// <summary>
        /// Creates a matrix with the given dimensions using the same storage type
        /// as this vector.
        /// </summary>
        /// <param name="rows">
        /// The number of rows.
        /// </param>
        /// <param name="columns">
        /// The number of columns.
        /// </param>
        /// <returns>
        /// A matrix with the given dimensions.
        /// </returns>
        public override Matrix CreateMatrix(int rows, int columns)
        {
            return new DenseMatrix(rows, columns);
        }

        /// <summary>
        /// Creates a <strong>Vector</strong> of the given size using the same storage type
        /// as this vector.
        /// </summary>
        /// <param name="size">
        /// The size of the <strong>Vector</strong> to create.
        /// </param>
        /// <returns>
        /// The new <c>Vector</c>.
        /// </returns>
        public override Vector CreateVector(int size)
        {
            return new DenseVector(size);
        }

        /// <summary>
        /// Copies the values of this vector into the target vector.
        /// </summary>
        /// <param name="target">
        /// The vector to copy elements into.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="target"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="target"/> is not the same size as this vector.
        /// </exception>
        public override void CopyTo(Vector target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (Count != target.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "target");
            }
            
            if (ReferenceEquals(this, target))
            {
                return;
            }

            var otherVector = target as DenseVector;
            if (otherVector == null)
            {
                Parallel.For(0, Data.Length, index => target[index] = Data[index]);
            }
            else
            {
                Buffer.BlockCopy(Data, 0, otherVector.Data, 0, Data.Length * Constants.SizeOfDouble);
            }
        }

        /// <summary>
        /// Adds a scalar to each element of the vector.
        /// </summary>
        /// <param name="scalar">The scalar to add.</param>
        public override void Add(double scalar)
        {
            if (scalar == 0.0)
            {
                return;
            }

            Parallel.For(0, Count, i => Data[i] += scalar);
        }

        /// <summary>
        /// Adds a scalar to each element of the vector and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to add.</param>
        /// <param name="result">The vector to store the result of the addition.</param>
        /// <exception cref="ArgumentNullException">If the result vector is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public override void Add(double scalar, Vector result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (Count != result.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "result");
            }

            CopyTo(result);
            result.Add(scalar);
        }

        /// <summary>
        /// Adds another vector to this vector.
        /// </summary>
        /// <param name="other">The vector to add to this one.</param>
        /// <exception cref="ArgumentNullException">If the other vector is <see langword="null" />.</exception> 
        /// <exception cref="ArgumentException">If this vector and <paramref name="other"/> are not the same size.</exception>
        public override void Add(Vector other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            if (Count != other.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "other");
            }

            var denseVector = other as DenseVector;

            if (denseVector == null)
            {
                base.Add(other);
            }
            else
            {
                Control.LinearAlgebraProvider.AddVectorToScaledVector(Data, 1.0, denseVector.Data);
            }
        }

        /// <summary>
        /// Adds another vector to this vector and stores the result into the result vector.
        /// </summary>
        /// <param name="other">The vector to add to this one.</param>
        /// <param name="result">The vector to store the result of the addition.</param>
        /// <exception cref="ArgumentNullException">If the other vector is <see langword="null" />.</exception> 
        /// <exception cref="ArgumentNullException">If the result vector is <see langword="null" />.</exception> 
        /// <exception cref="ArgumentException">If this vector and <paramref name="other"/> are not the same size.</exception>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public override void Add(Vector other, Vector result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (Count != other.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "other");
            }

            if (Count != result.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "result");
            }

            if (ReferenceEquals(this, result) || ReferenceEquals(other, result))
            {
                var tmp = result.CreateVector(result.Count);
                Add(other, tmp);
                tmp.CopyTo(result);
            }
            else
            {
                CopyTo(result);
                result.Add(other);
            }
        }

        /// <summary>
        /// Returns a <strong>Vector</strong> containing the same values of rightSide. 
        /// </summary>
        /// <remarks>This method is included for completeness.</remarks>
        /// <param name="rightSide">The vector to get the values from.</param>
        /// <returns>A vector containing a the same values as <paramref name="rightSide"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static Vector operator +(DenseVector rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            return rightSide.Plus();
        }

        /// <summary>
        /// Adds two <strong>Vectors</strong> together and returns the results.
        /// </summary>
        /// <param name="leftSide">One of the vectors to add.</param>
        /// <param name="rightSide">The other vector to add.</param>
        /// <returns>The result of the addition.</returns>
        /// <exception cref="ArgumentException">If <paramref name="leftSide"/> and <paramref name="rightSide"/> are not the same size.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static Vector operator +(DenseVector leftSide, DenseVector rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            if (leftSide.Count != rightSide.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "rightSide");
            }

            var ret = leftSide.Clone();
            ret.Add(rightSide);
            return ret;
        }

        /// <summary>
        /// Subtracts a scalar from each element of the vector.
        /// </summary>
        /// <param name="scalar">The scalar to subtract.</param>
        public override void Subtract(double scalar)
        {
            if (scalar == 0.0)
            {
                return;
            }

            Parallel.For(0, Count, i => Data[i] -= scalar);
        }

        /// <summary>
        /// Subtracts a scalar from each element of the vector and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to subtract.</param>
        /// <param name="result">The vector to store the result of the subtraction.</param>
        /// <exception cref="ArgumentNullException">If the result vector is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public override void Subtract(double scalar, Vector result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (Count != result.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "result");
            }

            CopyTo(result);
            result.Subtract(scalar);
        }

        /// <summary>
        /// Subtracts another vector from this vector.
        /// </summary>
        /// <param name="other">The vector to subtract from this one.</param>
        /// <exception cref="ArgumentNullException">If the other vector is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If this vector and <paramref name="other"/> are not the same size.</exception>
         public override void Subtract(Vector other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            if (Count != other.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "other");
            }

            var denseVector = other as DenseVector;

            if (denseVector == null)
            {
                base.Subtract(other);
            }
            else
            {
                Control.LinearAlgebraProvider.AddVectorToScaledVector(Data, -1.0, denseVector.Data);
            }
        }

         /// <summary>
         /// Subtracts another vector to this vector and stores the result into the result vector.
         /// </summary>
         /// <param name="other">The vector to subtract from this one.</param>
         /// <param name="result">The vector to store the result of the subtraction.</param>
         /// <exception cref="ArgumentNullException">If the other vector is <see langword="null"/>.</exception>
         /// <exception cref="ArgumentNullException">If the result vector is <see langword="null"/>.</exception>
         /// <exception cref="ArgumentException">If this vector and <paramref name="other"/> are not the same size.</exception>
         /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public override void Subtract(Vector other, Vector result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (Count != other.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "other");
            }

            if (Count != result.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "result");
            }

            if (ReferenceEquals(this, result) || ReferenceEquals(other, result))
            {
                var tmp = result.CreateVector(result.Count);
                Subtract(other, tmp);
                tmp.CopyTo(result);
            }
            else
            {
                CopyTo(result);
                result.Subtract(other);
            }
        }

        /// <summary>
        /// Returns a <strong>Vector</strong> containing the negated values of rightSide. 
        /// </summary>
        /// <param name="rightSide">The vector to get the values from.</param>
        /// <returns>A vector containing the negated values as <paramref name="rightSide"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static Vector operator -(DenseVector rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            return rightSide.Negate();
        }

        /// <summary>
        /// Subtracts two <strong>Vectors</strong> and returns the results.
        /// </summary>
        /// <param name="leftSide">The vector to subtract from.</param>
        /// <param name="rightSide">The vector to subtract.</param>
        /// <returns>The result of the subtraction.</returns>
        /// <exception cref="ArgumentException">If <paramref name="leftSide"/> and <paramref name="rightSide"/> are not the same size.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static Vector operator -(DenseVector leftSide, DenseVector rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            if (leftSide.Count != rightSide.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "rightSide");
            }

            var ret = leftSide.Clone();
            ret.Subtract(rightSide);
            return ret;
        }

        /// <summary>
        /// Returns a negated vector.
        /// </summary>
        /// <returns>The negated vector.</returns>
        /// <remarks>Added as an alternative to the unary negation operator.</remarks>
        public override Vector Negate()
        {
            var result = new DenseVector(Count);
            Parallel.For(0, Count, i => result[i] = -Data[i]);
            return result;
        }

        /// <summary>
        /// Multiplies a scalar to each element of the vector.
        /// </summary>
        /// <param name="scalar">The scalar to multiply.</param>
        public override void Multiply(double scalar)
        {
            if (scalar == 1.0)
            {
                return;
            }

            Control.LinearAlgebraProvider.ScaleArray(scalar, Data);
        }

        /// <summary>
        /// Computes the dot product between this vector and another vector.
        /// </summary>
        /// <param name="other">The other vector to add.</param>
        /// <returns>The result of the addition.</returns>
        /// <exception cref="ArgumentException">If <paramref name="other"/> is not of the same size.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="other"/> is <see langword="null" />.</exception>
        public override double DotProduct(Vector other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            if (Count != other.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "other");
            }


            var denseVector = other as DenseVector;

            if (denseVector == null)
            {
                return base.DotProduct(other);
            }
            else
            {
                return Control.LinearAlgebraProvider.DotProduct(this.Data, denseVector.Data);
            }
        }

        /// <summary>
        /// Multiplies a vector with a scalar.
        /// </summary>
        /// <param name="leftSide">The vector to scale.</param>
        /// <param name="rightSide">The scalar value.</param>
        /// <returns>The result of the multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> is <see langword="null" />.</exception>
        public static DenseVector operator *(DenseVector leftSide, double rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            var ret = (DenseVector)leftSide.Clone();
            ret.Multiply(rightSide);
            return ret;
        }

        /// <summary>
        /// Multiplies a vector with a scalar.
        /// </summary>
        /// <param name="leftSide">The scalar value.</param>
        /// <param name="rightSide">The vector to scale.</param>
        /// <returns>The result of the multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static DenseVector operator *(double leftSide, DenseVector rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            var ret = (DenseVector)rightSide.Clone();
            ret.Multiply(leftSide);
            return ret;
        }

        /// <summary>
        /// Computes the dot product between two <strong>Vectors</strong>.
        /// </summary>
        /// <param name="leftSide">The left row vector.</param>
        /// <param name="rightSide">The right column vector.</param>
        /// <returns>The dot product between the two vectors.</returns>
        /// <exception cref="ArgumentException">If <paramref name="leftSide"/> and <paramref name="rightSide"/> are not the same size.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static double operator *(DenseVector leftSide, DenseVector rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            if (leftSide.Count != rightSide.Count)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "rightSide");
            }

            return Control.LinearAlgebraProvider.DotProduct(leftSide.Data, rightSide.Data);
        }

        /// <summary>
        /// Divides a vector with a scalar.
        /// </summary>
        /// <param name="leftSide">The vector to divide.</param>
        /// <param name="rightSide">The scalar value.</param>
        /// <returns>The result of the division.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> is <see langword="null" />.</exception>
        public static DenseVector operator /(DenseVector leftSide, double rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            var ret = (DenseVector)leftSide.Clone();
            ret.Multiply(1.0 / rightSide);
            return ret;
        }

        #region Vector Norms

        /// <summary>
        /// Euclidean Norm also known as 2-Norm.
        /// </summary>
        /// <returns>Scalar ret = sqrt(sum(this[i]^2))</returns>
        public override double Norm()
        {
            var sum = 0.0;
            for (var i = 0; i < Data.Length; i++)
            {
                sum = SpecialFunctions.Hypotenuse(sum, Data[i]);
            }

            return sum;
        }

        /// <summary>
        /// 1-Norm also known as Manhattan Norm or Taxicab Norm.
        /// </summary>
        /// <returns>Scalar ret = sum(abs(this[i]))</returns>
        public override double Norm1()
        {
            var sum = 0.0;
            var syncLock = new object();
            
            Parallel.For(
                0, 
                Count,
                () => 0.0,
                (index, localData) =>
                {
                    localData += Math.Abs(Data[index]);
                    return localData;
                },
                localResult =>
                {
                    lock (syncLock)
                    {
                        sum += localResult;
                    }
                });

            return sum;
        }

        /// <summary>
        /// Computes the p-Norm.
        /// </summary>
        /// <param name="p">The p value.</param>
        /// <returns>Scalar ret = (sum(abs(this[i])^p))^(1/p)</returns>
        public override double NormP(int p)
        {
            if (1 > p)
            {
                throw new ArgumentOutOfRangeException("p");
            }

            if (1 == p)
            {
                return Norm1();
            }

            if (2 == p)
            {
                return Norm();
            }

            var sum = 0.0;
            var syncLock = new object();
           
            Parallel.For(
                0, 
                Count,
                () => 0.0,
                (index, localData) =>
                {
                    localData += Math.Pow(Math.Abs(Data[index]), p);
                    return localData;
                },
                localResult =>
                {
                    lock (syncLock)
                    {
                        sum += localResult;
                    }
                });

            return Math.Pow(sum, 1.0 / p);
        }

        /// <summary>
        /// Infinity Norm.
        /// </summary>
        /// <returns>Scalar ret = max(abs(this[i]))</returns>
        public override double NormInfinity()
        {
            double max = 0;

            var syncLock = new object();

            Parallel.For(
                0, 
                Count,
                () => 0.0,
                (index, localData) =>
                {
                    localData = Math.Max(localData, Math.Abs(Data[index]));
                    return localData;
                },
                localResult =>
                {
                    lock (syncLock)
                    {
                        max = Math.Max(max, localResult);
                    }
                });

            return max;
        }

        #endregion

        #region Parse Functions

        /// <summary>
        /// Creates a double dense vector based on a string. The string can be in the following formats (without the
        /// quotes): 'n', 'n,n,..', '(n,n,..)', '[n,n,...]', where n is a double.
        /// </summary>
        /// <returns>
        /// A double dense vector containing the values specified by the given string.
        /// </returns>
        /// <param name="value">
        /// The string to parse.
        /// </param>
        public static DenseVector Parse(string value)
        {
            return Parse(value, null);
        }

        /// <summary>
        /// Creates a double dense vector based on a string. The string can be in the following formats (without the
        /// quotes): 'n', 'n,n,..', '(n,n,..)', '[n,n,...]', where n is a double.
        /// </summary>
        /// <returns>
        /// A double dense vector containing the values specified by the given string.
        /// </returns>
        /// <param name="value">
        /// the string to parse.
        /// </param>
        /// <param name="formatProvider">
        /// An <see cref="IFormatProvider"/> that supplies culture-specific formatting information.
        /// </param>
        public static DenseVector Parse(string value, IFormatProvider formatProvider)
        {
            if (value == null)
            {
                throw new ArgumentNullException(value);
            }

            value = value.Trim();
            if (value.Length == 0)
            {
                throw new FormatException();
            }

            // strip out parens
            if (value.StartsWith("(", StringComparison.Ordinal))
            {
                if (!value.EndsWith(")", StringComparison.Ordinal))
                {
                    throw new FormatException();
                }

                value = value.Substring(1, value.Length - 2).Trim();
            }

            if (value.StartsWith("[", StringComparison.Ordinal))
            {
                if (!value.EndsWith("]", StringComparison.Ordinal))
                {
                    throw new FormatException();
                }

                value = value.Substring(1, value.Length - 2).Trim();
            }

            // keywords
            var textInfo = formatProvider.GetTextInfo();
            var keywords = new[] { textInfo.ListSeparator };

            // lexing
            var tokens = new LinkedList<string>();
            GlobalizationHelper.Tokenize(tokens.AddFirst(value), keywords, 0);
            var token = tokens.First;

            if (token == null || tokens.Count.IsEven())
            {
                throw new FormatException();
            }

            // parsing
            var data = new double[(tokens.Count + 1) >> 1];
            for (int i = 0; i < data.Length; i++)
            {
                if (token == null || token.Value == textInfo.ListSeparator)
                {
                    throw new FormatException();
                }

                data[i] = Double.Parse(token.Value, NumberStyles.Any, formatProvider);

                token = token.Next;
                if (token != null)
                {
                    token = token.Next;
                }
            }

            return new DenseVector(data);
        }

        /// <summary>
        /// Converts the string representation of a real dense vector to double-precision dense vector equivalent.
        /// A return value indicates whether the conversion succeeded or failed.
        /// </summary>
        /// <param name="value">
        /// A string containing a real vector to convert.
        /// </param>
        /// <param name="result">
        /// The parsed value.
        /// </param>
        /// <returns>
        /// If the conversion succeeds, the result will contain a complex number equivalent to value.
        /// Otherwise the result will be <c>null</c>.
        /// </returns>
        public static bool TryParse(string value, out DenseVector result)
        {
            return TryParse(value, null, out result);
        }

        /// <summary>
        /// Converts the string representation of a real dense vector to double-precision dense vector equivalent.
        /// A return value indicates whether the conversion succeeded or failed.
        /// </summary>
        /// <param name="value">
        /// A string containing a real vector to convert.
        /// </param>
        /// <param name="formatProvider">
        /// An <see cref="IFormatProvider"/> that supplies culture-specific formatting information about value.
        /// </param>
        /// <param name="result">
        /// The parsed value.
        /// </param>
        /// <returns>
        /// If the conversion succeeds, the result will contain a complex number equivalent to value.
        /// Otherwise the result will be <c>null</c>.
        /// </returns>
        public static bool TryParse(string value, IFormatProvider formatProvider, out DenseVector result)
        {
            bool ret;
            try
            {
                result = Parse(value, formatProvider);
                ret = true;
            }
            catch (ArgumentNullException)
            {
                result = null;
                ret = false;
            }
            catch (FormatException)
            {
                result = null;
                ret = false;
            }

            return ret;
        }

        #endregion
    }
}