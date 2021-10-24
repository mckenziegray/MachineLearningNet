using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace DotNetExtensions
{
    /// <summary>
    /// Modified jagged array that maintains a uniform number of columns and allows for O(1) extraction of rows and O(n) extraction of columns.
    /// </summary>
    /// <typeparam name="T">The type of items in the <see cref="Matrix{T}"/>.</typeparam>
    public class Matrix<T> : IList<T[]>, IReadOnlyList<T[]>, ICloneable
    {
        /// <summary>
        /// The current number of rows.
        /// </summary>
        public int RowCount => Data.Length;

        /// <summary>
        /// The current number of columns. Each row added to the matrix must have this many items.
        /// </summary>
        public int ColumnCount => Data.Length > 0 ? Data[0].Length : 0;

        /// <summary>
        /// The total number of items in the <see cref="Matrix{T}"/> across all rows and columns.
        /// </summary>
        public int Count => RowCount * ColumnCount;

        /// <summary>
        /// Gets a value indicating whether the <see cref="Matrix{T}"/> has a fixed size. This property is always true.
        /// </summary>
        public bool IsFixedSize => Data.IsFixedSize;

        /// <summary>
        /// Gets a value indicating whether the <see cref="Matrix{T}"/> is read-only. This property is always false.
        /// </summary>
        public bool IsReadOnly => Data.IsReadOnly;

        /// <summary>
        /// Gets a value indicating whether the <see cref="Matrix{T}"/> is synchronized (thread-safe). This property is always false.
        /// </summary>
        public bool IsSynchronized => Data.IsSynchronized;

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="Matrix{T}"/>.
        /// </summary>
        public object SyncRoot => Data.SyncRoot;

        public T[][] Rows => Data;

        public IEnumerable<IEnumerable<T>> Columns => Enumerable.Range(0, ColumnCount).Select(i => GetColumn(i));

        public IEnumerable<T> Flattened => Data.SelectMany(r => r);

        protected T[][] Data { get; set; }

        /// <summary>
        /// Gets the items in the row at the specified index.
        /// </summary>
        /// <param name="i">The row index.</param>
        /// <returns>An array of <typeparamref name="T"/>.</returns>
        public T[] this[int i]
        {
            get => Data[i];
            set
            {
                if (RowCount > 0 && value.Length != ColumnCount)
                    throw new ArgumentException($"Tried to add row that is not the correct length. Expected: {ColumnCount}; actual: {value.Length}.");

                Data[i] = value;
            }
        }

        /// <summary>
        /// Instantiates an empty <see cref="Matrix{T}"/>.
        /// </summary>
        public Matrix()
        {
            Data = Array.Empty<T[]>();
        }

        /// <summary>
        /// Instantiates a <see cref="Matrix{T}"/> of the given dimensions populated with the default value for <typeparamref name="T"/>.
        /// </summary>
        /// <param name="rowCount">The number of rows.</param>
        /// <param name="colCount">The number of columns.</param>
        public Matrix(int rowCount, int colCount)
        {
            T[][] temp = new T[rowCount][];

            for (int i = 0; i < rowCount; ++i)
                temp[i] = new T[colCount];

            Data = temp;
        }

        /// <summary>
        /// Creates a <see cref="Matrix{T}"/> using the given jagged array.
        /// </summary>
        /// <param name="data">The initial data for the <see cref="Matrix{T}"/>. The jagged array must be of uniform size (all rows must be the same length) or an <see cref="ArgumentException"/> will be thrown.</param>
        /// <remarks>Makes a shallow copy of <paramref name="data"/>.</remarks>
        public Matrix(T[][] data)
        {
            if (data.Length > 0 && data.Any(d => d.Length != data[0].Length))
                throw new ArgumentException("The given jagged array is not of uniform size.");
            else if (data is not null)
                Data = data;
        }

        /// <summary>
        /// Creates a <see cref="Matrix{T}"/> and populates it with the data in the given 2-D array.
        /// </summary>
        /// <param name="data">The initial data for the <see cref="Matrix{T}"/>.</param>
        /// <remarks>Makes a shallow copy of <paramref name="data"/>.</remarks>
        public Matrix(T[,] data)
        {
            if (data is not null)
                data.CopyTo(Data, 0);
        }

        /// <summary>
        /// Gets the items in the row at the specified index.
        /// </summary>
        /// <param name="rowIdx">The row index.</param>
        /// <returns>An array of <typeparamref name="T"/>.</returns>
        public T[] GetRow(int rowIdx)
        {
            if (rowIdx < 0 || rowIdx >= RowCount)
                throw new ArgumentOutOfRangeException(nameof(rowIdx));

            return Data[rowIdx];
        }

        /// <summary>
        /// Gets the items in the column at the specified index.
        /// </summary>
        /// <param name="colIdx">The column index.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> containing the items in column <paramref name="colIdx"/>.</returns>
        public IEnumerable<T> GetColumn(int colIdx)
        {
            if (colIdx < 0 || colIdx >= ColumnCount)
                throw new ArgumentOutOfRangeException(nameof(colIdx));

            return Data.Select(r => r[colIdx]);
        }

        /// <summary>
        /// Searches for the specified item and returns the coordinates of its first occurrence.
        /// </summary>
        /// <param name="item">The item to locate.</param>
        /// <returns>The row and column indices of the first occurrence of <see cref="item"/>, if found; otherwise, (-1, -1).</returns>
        public (int, int) PositionOf(T item)
        {
            for (int r = 0; r < RowCount; ++r)
            {
                for (int c = 0; c < ColumnCount; ++c)
                {
                    if (GenericHelpers.AreEqual(Data[r][c], item))
                        return (r, c);
                }
            }

            return (-1, -1);
        }

        #region Interface methods
        /// <summary>
        /// Creates a shallow copy of the <see cref="Matrix{T}"/>.
        /// </summary>
        /// <returns>A shallow copy of the <see cref="Matrix{T}"/>.</returns>
        public object Clone()
        {
            return new Matrix<T>((T[][])Data.Clone());
        }

        /// <summary>
        /// Copies all the elements of the <see cref="Matrix{T}"/> to the specified one-dimensional array starting at the specified destination array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the current array.</param>
        /// <param name="index">A 32-bit integer that represents the index in array at which copying begins.</param>
        public void CopyTo(Array array, int index)
        {
            Data.CopyTo(array, index);
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator"/> for the <see cref="Matrix{T}"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> for the <see cref="Matrix{T}"/>.</returns>
        public IEnumerator GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        /// <summary>
        /// Copies all the elements of the <see cref="Matrix{T}"/> to the specified jagged array starting at the specified destination array index.
        /// </summary>
        /// <param name="array">The array that is the destination of the elements copied from the <see cref="Matrix{T}"/>.</param>
        /// <param name="arrayIndex">A 32-bit integer that represents the index in <paramref name="array"/> at which copying begins.</param>
        public void CopyTo(T[][] array, int arrayIndex)
        {
            Data.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Copies all the elements of the <see cref="Matrix{T}"/> to the specified 2-D array starting at the specified destination array index.
        /// </summary>
        /// <param name="array">The array that is the destination of the elements copied from the <see cref="Matrix{T}"/>.</param>
        /// <param name="arrayIndex">A 32-bit integer that represents the index in <paramref name="array"/> at which copying begins.</param>
        public void CopyTo(T[,] array, int arrayIndex)
        {
            Data.CopyTo(array, arrayIndex);
        }
        #endregion

        #region Hidden interface methods
        /// <summary>
        /// Returns an <see cref="IEnumerator"/> for the <see cref="Matrix{T}"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> for the <see cref="Matrix{T}"/>.</returns>
        IEnumerator<T[]> IEnumerable<T[]>.GetEnumerator()
        {
            return (IEnumerator<T[]>)Data.GetEnumerator();
        }

        void IList<T[]>.Insert(int index, T[] row)
        {
            ((IList<T[]>)Data).Insert(index, row);
        }

        int IList<T[]>.IndexOf(T[] item)
        {
            return ((IList<T[]>)Data).IndexOf(item);
        }

        void IList<T[]>.RemoveAt(int index)
        {
            ((IList<T[]>)Data).RemoveAt(index);
        }

        public void Add(T[] row)
        {
            ((ICollection<T[]>)Data).Add(row);
        }

        bool ICollection<T[]>.Contains(T[] row)
        {
            return ((ICollection<T[]>)Data).Contains(row);
        }

        bool ICollection<T[]>.Remove(T[] row)
        {
            return ((ICollection<T[]>)Data).Remove(row);
        }

        void ICollection<T[]>.Clear()
        {
            ((ICollection<T[]>)Data).Clear();
        }
        #endregion

        // Note: Matrixes can be converted to and from jagged arrays in O(1) time, so the cast can be done implicitly.
        public static implicit operator Matrix<T>(T[][] a) => new(a);
        public static implicit operator T[][](Matrix<T> m) => m.Data;

        // Note: Converting a matrix to or from 2d arrays requires O(n^2) time, so the casts must be done explicitly.
        public static explicit operator Matrix<T>(T[,] a) => new(a);
        public static explicit operator T[,](Matrix<T> m)
        {
            T[,] array = new T[m.RowCount, m.ColumnCount];
            m.CopyTo(array, 0);

            return array;
        }
    }
}
