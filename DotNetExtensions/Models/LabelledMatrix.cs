using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetExtensions.Models
{
    /// <summary>
    /// An array of labeled data rows where each row has the same number of elements.
    /// </summary>
    /// <typeparam name="TData">The type of the data in the <see cref="LabelledMatrix{TData, TLabel}"/>.</typeparam>
    /// <typeparam name="TLabel">The type of the labels.</typeparam>
    public class LabelledMatrix<TData, TLabel> : IList<(TData[] DataRow, TLabel Label)>, IReadOnlyList<(TData[] DataRow, TLabel Label)>, ICloneable
    {
        public Matrix<TData> Data { get; set; }

        public TLabel[] Labels { get; set; }

        /// <summary>
        /// The number of rows in the <see cref="LabelledMatrix{TData, TLabel}"/>.
        /// </summary>
        public int Count => Data.RowCount;

        public bool IsReadOnly => Data.IsReadOnly && Labels.IsReadOnly;

        protected IEnumerable<(TData[] DataRow, TLabel Label)> AsEnumerable => Enumerable.Range(0, Count).Select(i => this[i]);

        public LabelledMatrix()
        {
            Data = new();
            Labels = Array.Empty<TLabel>();
        }


        /// <summary>
        /// Instantiates a <see cref="LabelledMatrix{TData, TLabel}"/> of the given dimensions 
        /// populated with the default values for <typeparamref name="TData"/> and <typeparamref name="TLabel"/>.
        /// </summary>
        /// <param name="rowCount">The number of rows.</param>
        /// <param name="colCount">The number of columns, including the label column.</param>
        public LabelledMatrix(int rowCount, int colCount)
        {
            Data = new(rowCount, colCount - 1);
            Labels = new TLabel[rowCount];
        }

        /// <summary>
        /// Creates a <see cref="LabelledMatrix{TData, TLabel}"/> using the given data and labels.
        /// </summary>
        /// <param name="data">
        /// The initial feature set. 
        /// Must have the same number of rows as the length of <paramref name="labels"/>.
        /// </param>
        /// <param name="labels">
        /// The initial labels. 
        /// The label at index i corresponds to row i of <paramref name="data"/>.
        /// Must have the same length as the number of rows of <paramref name="data"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="data"/> or <paramref name="labels"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the number of rows of <paramref name="data"/> does not equal the number of items in <paramref name="labels"/>.</exception>
        public LabelledMatrix(Matrix<TData> data, TLabel[] labels)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));
            
            if (labels is null)
                throw new ArgumentNullException(nameof(labels));
            
            if (data.RowCount != labels.Length)
                throw new ArgumentException($"The number of rows of {nameof(data)} ({data.RowCount}) is not the same as the number of items in {nameof(labels)} ({labels.Length}).");
            
            Data = data;
            Labels = labels;
        }

        public (TData[] DataRow, TLabel Label) this[int index]
        {
            get => (Data[index], Labels[index]);
            set
            {
                Data[index] = value.DataRow;
                Labels[index] = value.Label;
            }
        }

        #region Interface methods
        /// <summary>
        /// Creates a shallow copy of the <see cref="LabelledMatrix{TData, TLabel}"/>.
        /// </summary>
        /// <returns>A shallow copy of the <see cref="LabelledMatrix{TData, TLabel}"/>.</returns>
        public object Clone()
        {
            return new LabelledMatrix<TData, TLabel>((Matrix<TData>)Data.Clone(), (TLabel[])Labels.Clone());
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator{TData}"/> for the <see cref="LabelledMatrix{TData, TLabel}"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{TData}"/> for the <see cref="LabelledMatrix{TData, TLabel}"/>.</returns>
        public IEnumerator<(TData[] DataRow, TLabel Label)> GetEnumerator()
        {
            return AsEnumerable.GetEnumerator();
        }

        /// <summary>
        /// Copies all the elements of the <see cref="LabelledMatrix{TData, TLabel}"/> to the specified jagged array starting at the specified destination array index.
        /// </summary>
        /// <param name="array">The array that is the destination of the elements copied from the <see cref="LabelledMatrix{TData, TLabel}"/>.</param>
        /// <param name="arrayIndex">A 32-bit integer that represents the index in <paramref name="array"/> at which copying begins.</param>
        public void CopyTo((TData[] DataRow, TLabel Label)[] array, int arrayIndex)
        {
            for (int i = arrayIndex; i < array.Length; ++i)
            {
                array[i] = (this[i].DataRow, this[i].Label);
            }
        }
        #endregion

        #region Hidden interface methods
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void IList<(TData[] DataRow, TLabel Label)>.Insert(int index, (TData[] DataRow, TLabel Label) row)
        {
            ((IList<TData[]>)Data.Rows).Insert(index, row.DataRow);
            ((IList<TLabel>)Labels).Insert(index, row.Label);
        }

        int IList<(TData[] DataRow, TLabel Label)>.IndexOf((TData[] DataRow, TLabel Label) row)
        {
            int index = -1;

            GenericHelpers.BasicFor(Count, i =>
            {
                if (GenericHelpers.AreEqual(row.Label, this[i].Label) && Equals(row.DataRow, this[i].DataRow))
                {
                    index = i;
                    return;
                }
            });

            return index;
        }

        void IList<(TData[] DataRow, TLabel Label)>.RemoveAt(int index)
        {
            ((IList<TData[]>)Data.Rows).RemoveAt(index);
            ((IList<TLabel>)Labels).RemoveAt(index);
        }

        public void Add((TData[] DataRow, TLabel Label) row)
        {
            ((IList<TData[]>)Data.Rows).Add(row.DataRow);
            ((IList<TLabel>)Labels).Add(row.Label);
        }

        bool ICollection<(TData[] DataRow, TLabel Label)>.Contains((TData[] DataRow, TLabel Label) row)
        {
            return ((IList<(TData[], TLabel)>)this).IndexOf(row) >= 0;
        }

        bool ICollection<(TData[] DataRow, TLabel Label)>.Remove((TData[] DataRow, TLabel Label) row)
        {
            int index = ((IList<(TData[] DataRow, TLabel Label)>)this).IndexOf(row);

            if (index >= 0)
            {
                ((IList<TData[]>)Data.Rows).RemoveAt(index);
                ((IList<TLabel>)Labels).RemoveAt(index);

                return true;
            }
            else
            {
                return false;
            }
        }

        void ICollection<(TData[] DataRow, TLabel Label)>.Clear()
        {
            ((IList<TData[]>)Data.Rows).Clear();
            ((IList<TLabel>)Labels).Clear();
        }
        #endregion
    }
}
