﻿using System;
using System.Collections.Generic;
using System.Globalization;
using MathNet.Numerics.LinearAlgebra.Double;
using MbUnit.Framework;

namespace MathNet.Numerics.UnitTests.LinearAlgebraTests.Double
{
    public abstract partial class MatrixTests
    {
        [Test]
        [Row(0)]
        [Row(1)]
        [Row(2.2)]
        [MultipleAsserts]
        public void CanMultiplyWithScalar(double scalar)
        {
            var matrix = testMatrices["Singular3x3"];
            var clone = matrix.Clone();
            clone.Multiply(scalar);

            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    Assert.AreEqual(matrix[i, j] * scalar, clone[i, j]);
                }
            }
        }

        [Test]
        public void CanMultiplyWithVector()
        {
            var A = testMatrices["Singular3x3"];
            var x = new DenseVector(new double[] { 1.0, 2.0, 3.0 });
            var y = A * x;

            Assert.AreEqual(A.RowCount, y.Count);

            for (int i = 0; i < A.RowCount; i++)
            {
                var ar = A.GetRow(i);
                var dot = ar * x;
                Assert.AreEqual(dot, y[i]);
            }
        }

        [Test]
        public void CanMultiplyWithVectorIntoResult()
        {
            var A = testMatrices["Singular3x3"];
            var x = new DenseVector(new double[] { 1.0, 2.0, 3.0 });
            var y = new DenseVector(3);
            A.Multiply(x, y);

            for (int i = 0; i < A.RowCount; i++)
            {
                var ar = A.GetRow(i);
                var dot = ar * x;
                Assert.AreEqual(dot, y[i]);
            }
        }

        [Test]
        public void CanMultiplyWithVectorIntoResultWhenUpdatingInputArgument()
        {
            var A = testMatrices["Singular3x3"];
            var x = new DenseVector(new double[] { 1.0, 2.0, 3.0 });
            var y = x;
            A.Multiply(x, x);

            Assert.AreSame(y, x);

            y = new DenseVector(new double[] { 1.0, 2.0, 3.0 });
            for (int i = 0; i < A.RowCount; i++)
            {
                var ar = A.GetRow(i);
                var dot = ar * y;
                Assert.AreEqual(dot, x[i]);
            }
        }

        [Test]
        [ExpectedArgumentNullException]
        public void MultiplyWithVectorIntoResultFailsWhenResultIsNull()
        {
            var A = testMatrices["Singular3x3"];
            var x = new DenseVector(new double[] { 1.0, 2.0, 3.0 });
            Vector y = null;
            A.Multiply(x, y);
        }

        [Test]
        [ExpectedArgumentException]
        public void MultiplyWithVectorIntoResultFailsWhenResultIsTooLarge()
        {
            var A = testMatrices["Singular3x3"];
            var x = new DenseVector(new double[] { 1.0, 2.0, 3.0 });
            Vector y = new DenseVector(4);
            A.Multiply(x, y);
        }

        [Test]
        [Row(0)]
        [Row(1)]
        [Row(2.2)]
        [MultipleAsserts]
        public void CanOperatorLeftMultiplyWithScalar(double scalar)
        {
            var matrix = testMatrices["Singular3x3"];
            var clone = matrix * scalar;

            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    Assert.AreEqual(matrix[i, j] * scalar, clone[i, j]);
                }
            }
        }

        [Test]
        [Row(0)]
        [Row(1)]
        [Row(2.2)]
        [MultipleAsserts]
        public void CanOperatorRightMultiplyWithScalar(double scalar)
        {
            var matrix = testMatrices["Singular3x3"];
            var clone = matrix * scalar;

            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    Assert.AreEqual(matrix[i, j] * scalar, clone[i, j]);
                }
            }
        }

        [Test]
        [Row(0)]
        [Row(1)]
        [Row(2.2)]
        [MultipleAsserts]
        public void CanMultiplyWithScalarIntoResult(double scalar)
        {
            var matrix = testMatrices["Singular3x3"];
            var result = matrix.Clone();
            matrix.Multiply(scalar, result);

            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    Assert.AreEqual(matrix[i, j] * scalar, result[i, j]);
                }
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MultiplyWithScalarIntoResultFailsWhenResultIsNull()
        {
            var matrix = testMatrices["Singular3x3"];
            Matrix result = null;
            matrix.Multiply(2.3, result);
        }

        [Test]
        [ExpectedArgumentException]
        public void MultiplyWithScalarFailsWhenResultHasMoreRows()
        {
            var matrix = testMatrices["Singular3x3"];
            Matrix result = CreateMatrix(matrix.RowCount + 1, matrix.ColumnCount);
            matrix.Multiply(2.3, result);
        }

        [Test]
        [ExpectedArgumentException]
        public void MultiplyWithScalarFailsWhenResultHasMoreColumns()
        {
            var matrix = testMatrices["Singular3x3"];
            Matrix result = CreateMatrix(matrix.RowCount, matrix.ColumnCount + 1);
            matrix.Multiply(2.3, result);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OperatorLeftMultiplyWithScalarFailsWhenMatrixIsNull()
        {
            Matrix matrix = null;
            Matrix result = 2.3 * matrix;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OperatorRightMultiplyWithScalarFailsWhenMatrixIsNull()
        {
            Matrix matrix = null;
            Matrix result = matrix * 2.3;
        }

        [Test]
        [Row("Singular3x3", "Square3x3")]
        [Row("Singular4x4", "Square4x4")]
        public void CanAddMatrix(string mtxA, string mtxB)
        {
            var A = testMatrices[mtxA];
            var B = testMatrices[mtxB];

            Matrix matrix = A.Clone();
            matrix.Add(B);
            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    Assert.AreEqual(matrix[i, j], A[i, j] + B[i, j]);
                }
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddMatrixThrowsExceptionWhenArgumentIsNull()
        {
            Matrix matrix = testMatrices["Singular4x4"];
            Matrix other = null;
            matrix.Add(other);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddMatrixThrowsExceptionArgumentHasTooFewColumns()
        {
            Matrix matrix = testMatrices["Singular3x3"];
            Matrix other = testMatrices["Tall3x2"];
            matrix.Add(other);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddMatrixThrowsExceptionArgumentHasTooFewRows()
        {
            Matrix matrix = testMatrices["Singular3x3"];
            Matrix other = testMatrices["Wide2x3"];
            matrix.Add(other);
        }

        [Test]
        [Row("Singular3x3", "Square3x3")]
        [Row("Singular4x4", "Square4x4")]
        public void AddOperator(string mtxA, string mtxB)
        {
            var A = testMatrices[mtxA];
            var B = testMatrices[mtxB];
            
            Matrix result = A + B;
            for (int i = 0; i < A.RowCount; i++)
            {
                for (int j = 0; j < A.ColumnCount; j++)
                {
                    Assert.AreEqual(result[i,j], A[i, j] + B[i, j]);
                }
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddOperatorThrowsExceptionWhenLeftsideIsNull()
        {
            Matrix matrix = null;
            Matrix other = testMatrices["Singular3x3"];
            Matrix result = matrix + other;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddOperatorThrowsExceptionWhenRightsideIsNull()
        {
            Matrix matrix = testMatrices["Singular3x3"];
            Matrix other = null;
            Matrix result = matrix + other;
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddOperatorThrowsExceptionWhenRightsideHasTooFewColumns()
        {
            Matrix matrix = testMatrices["Singular3x3"];
            Matrix other = testMatrices["Tall3x2"];
            Matrix result = matrix + other;
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddOperatorThrowsExceptionWhenRightsideHasTooFewRows()
        {
            Matrix matrix = testMatrices["Singular3x3"];
            Matrix other = testMatrices["Wide2x3"];
            Matrix result = matrix + other;
        }

        [Test]
        [Row("Singular3x3", "Square3x3")]
        [Row("Singular4x4", "Square4x4")]
        public void CanSubtractMatrix(string mtxA, string mtxB)
        {
            var A = testMatrices[mtxA];
            var B = testMatrices[mtxB];

            Matrix matrix = A.Clone();
            matrix.Subtract(B);
            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    Assert.AreEqual(matrix[i, j], A[i, j] - B[i, j]);
                }
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SubtractMatrixThrowsExceptionWhenRightSideIsNull()
        {
            Matrix matrix = testMatrices["Singular4x4"];
            Matrix other = null;
            matrix.Subtract(other);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SubtractMatrixThrowsExceptionWhenRightSideHasTooFewColumns()
        {
            Matrix matrix = testMatrices["Singular3x3"];
            Matrix other = testMatrices["Tall3x2"];
            matrix.Subtract(other);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SubtractMatrixThrowsExceptionWhenRightSideHasTooFewRows()
        {
            Matrix matrix = testMatrices["Singular3x3"];
            Matrix other = testMatrices["Wide2x3"];
            matrix.Subtract(other);
        }

        [Test]
        [Row("Singular3x3", "Square3x3")]
        [Row("Singular4x4", "Square4x4")]
        public void SubtractOperator(string mtxA, string mtxB)
        {
            var A = testMatrices[mtxA];
            var B = testMatrices[mtxB];

            Matrix result = A - B;
            for (int i = 0; i < A.RowCount; i++)
            {
                for (int j = 0; j < A.ColumnCount; j++)
                {
                    Assert.AreEqual(result[i, j], A[i, j] - B[i, j]);
                }
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SubtractOperatorThrowsExceptionWhenLeftsideIsNull()
        {
            Matrix matrix = null;
            Matrix other = testMatrices["Singular3x3"];
            Matrix result = matrix - other;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SubtractOperatorThrowsExceptionWhenRightsideIsNull()
        {
            Matrix matrix = testMatrices["Singular3x3"];
            Matrix other = null;
            Matrix result = matrix - other;
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SubtractOperatorThrowsExceptionWhenRightsideHasTooFewColumns()
        {
            Matrix matrix = testMatrices["Singular3x3"];
            Matrix other = testMatrices["Tall3x2"];
            Matrix result = matrix - other;
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SubtractOperatorThrowsExceptionWhenRightsideHasTooFewRows()
        {
            Matrix matrix = testMatrices["Singular3x3"];
            Matrix other = testMatrices["Wide2x3"];
            Matrix result = matrix - other;
        }

        [Test]
        [Row("Singular3x3", "Square3x3")]
        [Row("Singular4x4", "Square4x4")]
        [Row("Wide2x3", "Square3x3")]
        [Row("Wide2x3", "Tall3x2")]
        [Row("Tall3x2", "Wide2x3")]
        [MultipleAsserts]
        public void CanMultiplyMatrixWithMatrix(string nameA, string nameB)
        {
            Matrix A = testMatrices[nameA];
            Matrix B = testMatrices[nameB];
            Matrix C = A * B;

            Assert.AreEqual(C.RowCount, A.RowCount);
            Assert.AreEqual(C.ColumnCount, B.ColumnCount);

            for (int i = 0; i < C.RowCount; i++)
            {
                for (int j = 0; j < C.ColumnCount; j++)
                {
                    AssertHelpers.AlmostEqual(A.GetRow(i) * B.GetColumn(j), C[i, j], 15);
                }
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void MultiplyMatrixMatrixFailsWhenSizesAreIncompatible()
        {
            Matrix matrix = testMatrices["Singular3x3"];
            Matrix other = testMatrices["Wide2x3"];
            Matrix result = matrix * other;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MultiplyMatrixMatrixFailsWhenLeftArgumentIsNull()
        {
            Matrix matrix = null;
            Matrix other = testMatrices["Wide2x3"];
            Matrix result = matrix * other;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MultiplyMatrixMatrixFailsWhenRightArgumentIsNull()
        {
            Matrix matrix = testMatrices["Wide2x3"];
            Matrix other = null;
            Matrix result = matrix * other;
        }

        [Test]
        [Row("Singular3x3", "Square3x3")]
        [Row("Singular4x4", "Square4x4")]
        [Row("Wide2x3", "Square3x3")]
        [Row("Wide2x3", "Tall3x2")]
        [Row("Tall3x2", "Wide2x3")]
        [MultipleAsserts]
        public void CanMultiplyMatrixWithMatrixIntoResult(string nameA, string nameB)
        {
            Matrix A = testMatrices[nameA];
            Matrix B = testMatrices[nameB];
            Matrix C = CreateMatrix(A.RowCount, B.ColumnCount);
            A.Multiply(B, C);

            Assert.AreEqual(C.RowCount, A.RowCount);
            Assert.AreEqual(C.ColumnCount, B.ColumnCount);

            for (int i = 0; i < C.RowCount; i++)
            {
                for (int j = 0; j < C.ColumnCount; j++)
                {
                    AssertHelpers.AlmostEqual(A.GetRow(i) * B.GetColumn(j), C[i, j], 15);
                }
            }
        }

        [Test]
        [Row("Singular3x3")]
        [Row("Square3x3")]
        [Row("Square4x4")]
        [Row("Tall3x2")]
        [Row("Wide2x3")]
        [MultipleAsserts]
        public void CanNegate(string name)
        {
            var matrix = testMatrices[name];
            var copy = matrix.Clone();

            copy.Negate();

            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    Assert.AreEqual(-matrix[i, j], copy[i, j]);
                }
            }
        }

        [Test]
        [Row("Singular3x3")]
        [Row("Square3x3")]
        [Row("Square4x4")]
        [Row("Tall3x2")]
        [Row("Wide2x3")]
        [MultipleAsserts]
        public void CanNegateIntoResult(string name)
        {
            var matrix = testMatrices[name];
            var copy = matrix.Clone();

            matrix.Negate(copy);

            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    Assert.AreEqual(-matrix[i, j], copy[i, j]);
                }
            }
        }

        [Test]
        [ExpectedArgumentNullException]
        public void NegateIntoResultFailsWhenResultIsNull()
        {
            var matrix = testMatrices["Singular3x3"];
            Matrix copy = null;
            matrix.Negate(copy);
        }

        [Test]
        [ExpectedArgumentException]
        public void NegateIntoResultFailsWhenResultHasMoreRows()
        {
            Matrix matrix = testMatrices["Singular3x3"];
            Matrix target = CreateMatrix(matrix.RowCount + 1, matrix.ColumnCount);
            matrix.Negate(target);
        }

        [Test]
        [ExpectedArgumentException]
        public void NegateIntoResultFailsWhenResultHasMoreColumns()
        {
            Matrix matrix = testMatrices["Singular3x3"];
            Matrix target = CreateMatrix(matrix.RowCount + 1, matrix.ColumnCount);
            matrix.Negate(target);
        }
    }
}