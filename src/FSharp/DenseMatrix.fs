// <copyright file="DenseMatrix.fs" company="Math.NET">
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

open MathNet.Numerics.LinearAlgebra

/// A module which implements functional dense vector operations.
module DenseMatrix =

    /// Initialize a matrix by calling a construction function for every element.
    let inline init (n: int) (m: int) f =
        let A = new DenseMatrix(n,m)
        for i=0 to n-1 do
            for j=0 to m-1 do
                A.[i,j] <- f i j
        A

    /// Create the identity matrix.
    let inline identity (n:int) =
        let A = new DenseMatrix(n, n, 0.0)
        for i=0 to n-1 do
            A.[i,i] <- 1.0
        A
    
    /// Create a matrix from a list of float lists. Every list in the master list specifies a row.
    let inline ofList (fll: float list list) =
        let n = List.length fll
        let m = List.length (List.head fll)
        let A = DenseMatrix(n,m)
        fll |> List.iteri (fun i fl ->
                            if (List.length fl) <> m then failwith "Each subrow must be of the same length." else
                            List.iteri (fun j f -> A.[i,j] <- f) fl)
        A
    
    /// Create a matrix from a list of sequences. Every sequence in the master sequence specifies a row.
    let inline ofSeq (fss: #seq<#seq<float>>) =
        let n = Seq.length fss
        let m = Seq.length (Seq.head fss)
        let A = DenseMatrix(n,m)
        fss |> Seq.iteri (fun i fs ->
                            if (Seq.length fs) <> m then failwith "Each subrow must be of the same length." else
                            Seq.iteri (fun j f -> A.[i,j] <- f) fs)
        A

    /// Create a matrix from a 2D array of floating point numbers.
    let inline ofArray2 (arr: float[,]) = new DenseMatrix(arr)
    
    /// Create a matrix with the given entries.
    let inline initDense (n: int) (m: int) (es: #seq<int * int * float>) =
        let A = new DenseMatrix(n,m)
        Seq.iter (fun (i,j,f) -> A.[i,j] <- f) es
        A
    
    /// Create a square matrix with constant diagonal entries.
    let inline constDiag (n: int) (f: float) =
        let A = new DenseMatrix(n,n)
        for i=0 to n-1 do
            A.[i,i] <- f
        A
    
    /// Create a square matrix with the vector elements on the diagonal.
    let inline diag (v: #Vector) =
        let n = v.Count
        let A = new DenseMatrix(n,n)
        for i=0 to n-1 do
            A.[i,i] <- v.Item(i)
        A
    
    (*

    /// Initialize a matrix by calling a construction function for every row.
    let inline init_row (n: int) (m: int) (f: int -> #Vector) =
        let A = new DenseMatrix(n,m)
        for i=0 to n-1 do
            let row = f i
            if row.Count <> m then failwith "Row generator does not create rows of the appropriate size."
            A.SetRow(i, row)
        A

    /// Initialize a matrix by calling a construction function for every column.
    let inline init_col (n: int) (m: int) (f: int -> #Vector) =
        let A = new DenseMatrix(n,m)
        for i=0 to m-1 do
            let col = f i
            if col.Count <> n then failwith "Column generator does not create columns of the appropriate size."
            A.SetColumn(i, col)
        A
    
    /// Create a 1xn dimensional matrix from a row vector.
    let inline of_rowvector (v: #Vector) =
        let n = v.Count
        let A = new DenseMatrix(1, n)
        A.SetRow(0, v)
        A
    
    /// Create an nx1 dimensional matrix from a column vector.
    let inline of_vector (v: #Vector) =
        let n = v.Count
        let A = new DenseMatrix(n, 1)
        A.SetColumn(0, v)
        A*)