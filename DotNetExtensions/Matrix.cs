using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace DotNetExtensions
{
    /// <summary>
    ///     Modified jagged array that maintains a uniform number of columns and allows for O(1) extraction of rows and O(n) extraction of columns.
    /// </summary>
    /// <typeparam name="T">The type of items in the matrix.</typeparam>
    public class Matrix<T> : IList, IList<T[]>, ICloneable, IReadOnlyList<T[]>
    {
        public int RowCount => Data.Length;
        public int ColumnCount => Data.Length > 0 ? Data[0].Length : 0;

        protected T[][] Data { get; set; }

        public bool IsFixedSize => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        object IList.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Matrix()
        {
            Data = Array.Empty<T[]>();
        }

        public Matrix(int rowCount, int colCount)
        {
            T[][] temp = new T[rowCount][];

            for (int i = 0; i < rowCount; ++i)
                temp[i] = new T[colCount];

            Data = temp;
        }

        public Matrix(T[][] data)
        {
            if (data.Length > 0 && data.Any(d => d.Length != data[0].Length))
                throw new ArgumentException("The given jagged array is not of uniform size.");
            else
                Data = data;
        }

        public T[] GetRow(int rowIdx)
        {
            if (rowIdx < 0 || rowIdx >= RowCount)
                throw new ArgumentOutOfRangeException(nameof(rowIdx));

            return Data[rowIdx];
        }

        public T[] GetColumn(int colIdx)
        {
            if (colIdx < 0 || colIdx >= ColumnCount)
                throw new ArgumentOutOfRangeException(nameof(colIdx));

            return Data.Select(r => r[colIdx]).ToArray();
        }

        #region Interface Methods
        public T[] this[int i]
        {
            get
            {
                return Data[i];
            }
            set
            {
                if (RowCount > 0 && value.Length != ColumnCount)
                    throw new ArgumentException($"Tried to add row that is not the correct length. Expected: {ColumnCount}; actual: {value.Length}.");

                Data[i] = value;
            }
        }

        public object Clone()
        {
            return new Matrix<T>((T[][])Data.Clone());
        }

        public void CopyTo(Array array, int index)
        {
            Data.CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        public int IndexOf(T[] item)
        {
            return Array.IndexOf(Data, item);
        }

        public void Insert(int index, T[] item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public void Add(T[] item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(T[] item)
        {
            return Data.Contains(item);
        }

        public void CopyTo(T[][] array, int arrayIndex)
        {
            Data.CopyTo(array, arrayIndex);
        }

        public bool Remove(T[] item)
        {
            throw new NotSupportedException();
        }

        IEnumerator<T[]> IEnumerable<T[]>.GetEnumerator()
        {
            return (IEnumerator<T[]>)Data.GetEnumerator();
        }

        public int Add(object value)
        {
            throw new NotSupportedException();
        }

        public bool Contains(object value)
        {
            return Data.Contains(value);
        }

        public int IndexOf(object value)
        {
            throw new NotSupportedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        public void Remove(object value)
        {
            throw new NotSupportedException();
        }
        #endregion

        public static implicit operator Matrix<T>(T[][] a) => new Matrix<T>(a);

        public static explicit operator T[][](Matrix<T> m) => m.Data;
    }
}
