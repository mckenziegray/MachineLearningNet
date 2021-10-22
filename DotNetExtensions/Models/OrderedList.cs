#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DotNetExtensions
{
    /// <summary>
    /// Wrapper for a <see cref="List{T}"/> that keeps the <see cref="List{T}"/> constantly sorted using a given <see cref="IComparer{T}"/>. Allows duplicates.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OrderedList<T> : ICollection<T>, IOrderedEnumerable<T>, IReadOnlyCollection<T>
    {
        /// <summary>
        /// Gets the number of elements contained in the <see cref="OrderedList{T}"/>.
        /// </summary>
        public int Count => Items.Count;
        public int Capacity => Items.Capacity;

        /// <summary>
        /// Gets a value indicating whether the <see cref="OrderedList{T}"/> is read-only.
        /// </summary>
        public bool IsReadOnly => (Items as IList<T>).IsReadOnly;

        protected List<T> Items { get; private set; }
        protected IComparer<T> Comparer { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedList{T}"/> class that is empty and has the default initial capacity.
        /// </summary>
        /// <param name="comparer">The <see cref="IComparer{T}"/> that will be used to maintain the sort order of the <see cref="OrderedList{T}"/>.</param>
        public OrderedList(IComparer<T> comparer)
        {
            Comparer = comparer ?? Comparer<T>.Default;
            Items = new List<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedList{T}"/> class that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="comparer">The <see cref="IComparer{T}"/> that will be used to maintain the sort order of the <see cref="OrderedList{T}"/>.</param>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        public OrderedList(IComparer<T> comparer, int capacity)
        {
            Comparer = comparer ?? Comparer<T>.Default;
            Items = new List<T>(capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedList{T}"/> class that contains elements copied from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="comparer">The <see cref="IComparer{T}"/> that will be used to maintain the sort order of the <see cref="OrderedList{T}"/>.</param>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        public OrderedList(IComparer<T> comparer, IEnumerable<T> collection)
        {
            Comparer = comparer ?? Comparer<T>.Default;
            Items = new List<T>(collection.OrderBy(c => c, comparer));
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <returns>The element at the specified index.</returns>
        public T this[int index]
        {
            get => Items[index];
        }

        /// <summary>
        /// Creates a shallow copy of a range of elements in the source <see cref="OrderedList{T}"/>.
        /// </summary>
        /// <param name="startIndex">The zero-based <see cref="OrderedList{T}"/> index at which the range starts.</param>
        /// <param name="count">The number of elements in the range.</param>
        /// <returns>A shallow copy of a range of elements in the source <see cref="OrderedList{T}"/>.</returns>
        public OrderedList<T> GetRange(int startIndex, int count)
        {
            return new OrderedList<T>(Comparer, Items.GetRange(startIndex, count));
        }

        #region Search methods
        /// <summary>
        /// Determines whether an element is in the <see cref="OrderedList{T}"/> using binary search.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="OrderedList{T}"/>. The value can be null for reference types.</param>
        /// <returns>True if item is found in the <see cref="OrderedList{T}"/>; otherwise, false.</returns>
        public bool Contains(T item)
        {
            return BinarySearch(item) >= 0;
        }

        /// <summary>
        /// Determines whether the <see cref="OrderedList{T}"/> contains elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions of the elements to search for.</param>
        /// <returns>true if the <see cref="OrderedList{T}"/> contains one or more elements that match the conditions defined by the specified predicate; otherwise, false.</returns>
        public bool Exists(Predicate<T> match)
        {
            return Items.Exists(match);
        }

        /// <summary>
        /// Performs a binary search for the specified object and returns the zero-based index of the first occurrence within the entire <see cref="OrderedList{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="OrderedList{T}"/>. The value can be null for reference types.</param>
        /// <returns>The zero-based index of the first occurrence of <paramref name="item"/> within the entire <see cref="OrderedList{T}"/>, if found; otherwise, -1.</returns>
        public int IndexOf(T item)
        {
            return IndexOf(item, 0, Items.Count);
        }

        /// <summary>
        /// Performs a binary search for the specified object and returns the zero-based index of the first occurrence within the range of elements in the <see cref="OrderedList{T}"/> that extends from the specified index to the last element.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="OrderedList{T}"/>. The value can be null for reference types.</param>
        /// <param name="startIndex">The zero-based starting index of the search.</param>
        /// <returns>The zero-based index of the first occurrence of <paramref name="item"/> within the range of elements in the <see cref="OrderedList{T}"/> that extends from <paramref name="startIndex"/> to the last element, if found; otherwise, -1.</returns>
        public int IndexOf(T item, int startIndex)
        {
            return IndexOf(item, startIndex, Items.Count);
        }

        /// <summary>
        /// Performs a binary search for the specified object and returns the zero-based index of the first occurrence within the range of elements in the <see cref="OrderedList{T}"/> that starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="OrderedList{T}"/>. The value can be null for reference types.</param>
        /// <param name="startIndex">The zero-based starting index of the search.</param>
        /// <param name="range">The number of elements in the section to search.</param>
        /// <returns>The zero-based index of the first occurrence of <paramref name="item"/> within the range of elements in the <see cref="OrderedList{T}"/> that starts at <paramref name="startIndex"/> and contains <paramref name="range"/> number of elements, if found; otherwise, -1.</returns>
        public int IndexOf(T item, int startIndex, int range)
        {
            int matchIndex = BinarySearch(item, startIndex, range);

            // If at least one copy of the item exists in the list, walk back to the first instance
            while (matchIndex > startIndex && (Items[matchIndex - 1]?.Equals(item) ?? item is null))
                --matchIndex;

            return matchIndex < startIndex ? -1 : matchIndex;
        }

        /// <summary>
        /// Performs a binary search for the specified object and returns the zero-based index of the last occurrence within the entire <see cref="OrderedList{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="OrderedList{T}"/>. The value can be null for reference types.</param>
        /// <returns>The zero-based index of the last occurrence of <paramref name="item"/> within the entire <see cref="OrderedList{T}"/>, if found; otherwise, -1.</returns>
        public int LastIndexOf(T item)
        {
            return LastIndexOf(item, 0, Items.Count);
        }

        /// <summary>
        /// Performs a binary search for the specified object and returns the zero-based index of the last occurrence within the range of elements in the <see cref="OrderedList{T}"/> that extends from the first element to the specified index.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="OrderedList{T}"/>. The value can be null for reference types.</param>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <returns>The zero-based index of the last occurrence of <paramref name="item"/> within the range of elements in the <see cref="OrderedList{T}"/> that extends from the first element to <paramref name="startIndex"/>, if found; otherwise, -1.</returns>
        public int LastIndexOf(T item, int startIndex)
        {
            return LastIndexOf(item, startIndex, Items.Count);
        }

        /// <summary>
        /// Performs a binary search for the specified object and returns the zero-based index of the last occurrence within  the range of elements in the <see cref="OrderedList{T}"/> that contains the specified number of elements and ends at the specified index.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="OrderedList{T}"/>. The value can be null for reference types.</param>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <param name="range">The number of elements in the section to search.</param>
        /// <returns>The zero-based index of the last occurrence of <paramref name="item"/> within the range of elements in the <see cref="OrderedList{T}"/> that contains <paramref name="range"/> number of elements and ends at <paramref name="startIndex"/>, if found; otherwise, -1.</returns>
        public int LastIndexOf(T item, int startIndex, int range)
        {

            int matchIndex = BinarySearch(item, startIndex, range);

            if (matchIndex < 0)
            {
                return -1;
            }
            else
            {
                // If at least one copy of the item exists in the list, walk forward to the last instance
                while (matchIndex < startIndex - 1 && matchIndex > startIndex - range + 1 && (Items[matchIndex + 1]?.Equals(item) ?? item is null))
                    ++matchIndex;

                return matchIndex;
            }
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the first occurrence within the entire <see cref="OrderedList{T}"/>.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>The first element that matches the conditions defined by the specified predicate, if found; otherwise, the default value for type <typeparamref name="T"/>.</returns>
        public T? Find(Predicate<T> match)
        {
            return Items.Find(match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the last occurrence within the entire <see cref="OrderedList{T}"/>.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>The last element that matches the conditions defined by the specified predicate, if found; otherwise, the default value for type <typeparamref name="T"/>.</returns>
        public T? FindLast(Predicate<T> match)
        {
            return Items.FindLast(match);
        }

        /// <summary>
        /// Retrieves all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>A <see cref="OrderedList{T}"/> containing all the elements that match the conditions defined by the specified predicate, if found; otherwise, an empty <see cref="OrderedList{T}"/>.</returns>
        public List<T> FindAll(Predicate<T> match)
        {
            return Items.FindAll(match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the entire <see cref="OrderedList{T}"/>.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>The zero-based index of the first occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, -1.</returns>
        public int FindIndex(Predicate<T> match)
        {
            return Items.FindIndex(match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the range of elements in the <see cref="OrderedList{T}"/> that extends from the specified index to the last element.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <param name="startIndex">The zero-based starting index of the search.</param>
        /// <returns>The zero-based index of the first occurrence of an element that matches the conditions defined by <paramref name="match"/> within the range of elements in the <see cref="OrderedList{T}"/> that extends from <paramref name="startIndex"/> to the last element, if found; otherwise, -1.</returns>
        public int FindIndex(Predicate<T> match, int startIndex)
        {
            return Items.FindIndex(startIndex, match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the range of elements in the <see cref="OrderedList{T}"/> that starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <param name="startIndex">The zero-based starting index of the search.</param>
        /// <param name="range">The number of elements in the section to search.</param>
        /// <returns>The zero-based index of the first occurrence of an element that matches the conditions defined by <paramref name="match"/> within the range of elements in the <see cref="OrderedList{T}"/> that starts at <paramref name="startIndex"/> and contains <paramref name="range"/> number of elements, if found; otherwise, -1.</returns>
        public int FindIndex(Predicate<T> match, int startIndex, int range)
        {
            return Items.FindIndex(startIndex, range, match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the last occurrence within the entire <see cref="OrderedList{T}"/>.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>The zero-based index of the last occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, -1.</returns>
        public int FindLastIndex(Predicate<T> match)
        {
            return Items.FindLastIndex(match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the last occurrence within the range of elements in the <see cref="OrderedList{T}"/> that extends from the first element to the specified index.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <returns>The zero-based index of the last occurrence of an element that matches the conditions defined by <paramref name="match"/>within the range of elements in the <see cref="OrderedList{T}"/> that extends from <paramref name="startIndex"/> to the last element, if found; otherwise, -1.</returns>
        public int FindLastIndex(Predicate<T> match, int startIndex)
        {
            return Items.FindLastIndex(startIndex, match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the last occurrence within the range of elements in the <see cref="OrderedList{T}"/> that contains the specified number of elements and ends at the specified index.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <param name="range"></param>
        /// <returns>The zero-based index of the last occurrence of an element that matches the conditions defined by <paramref name="match"/>within the range of elements in the <see cref="OrderedList{T}"/> that starts at <paramref name="startIndex"/> and contains <paramref name="range"/> number of elements, if found; otherwise, -1.</returns>
        public int FindLastIndex(Predicate<T> match, int startIndex, int range)
        {
            return Items.FindLastIndex(startIndex, range, match);
        }

        /// <summary>
        /// Searches the entire sorted <see cref="OrderedList{T}"/> for an and returns the zero-based index of the element.
        /// </summary>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <returns>The zero-based index of item in the sorted <see cref="OrderedList{T}"/>, if item is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than item or, if there is no larger element, the bitwise complement of <see cref="Count"/>.</returns>
        public int BinarySearch(T item)
        {
            return BinarySearch(item, 0, Items.Count);
        }

        /// <summary>
        /// Searches a range of elements in the sorted <see cref="OrderedList{T}"/> for an element and returns the zero-based index of the element.
        /// </summary>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <param name="startIndex">The zero-based starting index of the range to search.</param>
        /// <param name="range">The length of the range to search.</param>
        /// <returns>The zero-based index of item in the sorted <see cref="OrderedList{T}"/>, if <paramref name="item"/> is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than item or, if there is no larger element, the bitwise complement of <see cref="Count"/>.</returns>
        public int BinarySearch(T item, int startIndex, int range)
        {
            return Items.BinarySearch(startIndex, range, item, Comparer);
        }
        #endregion

        #region Insert methods
        /// <summary>
        /// Adds an object to the end of the <see cref="OrderedList{T}"/>.
        /// </summary>
        /// <param name="item">The object to be added to the end of the <see cref="OrderedList{T}"/>. The value can be null for reference types.</param>
        public void Add(T item)
        {
            int index = BinarySearch(item);
            if (index < 0)
                index = ~index; // Bitwise complement

            Items.Insert(index, item);
        }

        /// <summary>
        /// Adds the elements of the specified collection to the <see cref="OrderedList{T}"/>.
        /// </summary>
        /// <param name="collection">The collection whose elements should be added to the <see cref="OrderedList{T}"/>. The collection itself cannot be null, but it can contain elements that are null, if type <typeparamref name="T"/> is a reference type.</param>
        public void AddRange(IEnumerable<T> collection)
        {
            foreach (T item in collection)
                Add(item);
        }
        #endregion

        #region Delete methods
        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="OrderedList{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="OrderedList{T}"/>. The value can be null for reference types.</param>
        /// <returns>True if item is successfully removed; otherwise, false. This method also returns false if item was not found in the <see cref="OrderedList{T}"/>.</returns>
        public bool Remove(T item)
        {
            int index = BinarySearch(item);

            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Removes the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        public void RemoveAt(int index)
        {
            Items.RemoveAt(index);
        }

        /// <summary>
        /// Removes all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions of the elements to remove</param>
        /// <returns>The number of elements removed from the <see cref="OrderedList{T}"/>.</returns>
        public int RemoveAll(Predicate<T> match)
        {
            return Items.RemoveAll(match);
        }

        /// <summary>
        /// Removes a range of elements from the <see cref="OrderedList{T}"/>.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        public void RemoveRange(int startIndex, int count)
        {
            Items.RemoveRange(startIndex, count);
        }

        /// <summary>
        /// Removes all elements from the <see cref="OrderedList{T}"/>.
        /// </summary>
        public void Clear()
        {
            Items.Clear();
        }

        /// <summary>
        /// Sets the capacity to the actual number of elements in the <see cref="OrderedList{T}"/>, if that number is less than a threshold value.
        /// </summary>
        public void TrimExcess()
        {
            Items.TrimExcess();
        }
        #endregion

        #region Enumeration methods
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="OrderedList{T}"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/> for the <see cref="OrderedList{T}"/>.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="OrderedList{T}"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> for the <see cref="OrderedList{T}"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        /// <summary>
        /// Performs the specified action on each element of the <see cref="OrderedList{T}"/>.
        /// </summary>
        /// <param name="action"></param>
        public void ForEach(Action<T> action)
        {
            Items.ForEach(action);
        }

        /// <summary>
        /// Determines whether every element in the <see cref="OrderedList{T}"/> matches the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public bool TrueForAll(Predicate<T> match)
        {
            return Items.TrueForAll(match);
        }
        #endregion

        #region Copy methods
        /// <summary>
        /// Copies the entire <see cref="OrderedList{T}"/> to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="OrderedList{T}"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Performs a subsequent ordering on the elements of the <see cref="OrderedList{T}"/> according to a key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key produced by <paramref name="keySelector"/>.</typeparam>
        /// <param name="keySelector">The <see cref="Func{T, TKey}"/> used to extract the key for each element.</param>
        /// <param name="comparer">The <see cref="IComparer{T}"/> used to compare keys for placement in the returned sequence.</param>
        /// <param name="descending">True to sort the elements in descending order; false to sort the elements in ascending order.</param>
        /// <returns>An <see cref="IOrderedEnumerable{T}"/> whose elements are sorted according to a key.</returns>
        public IOrderedEnumerable<T> CreateOrderedEnumerable<TKey>(Func<T, TKey> keySelector, IComparer<TKey>? comparer, bool descending)
        {
            IOrderedEnumerable<T> oe = Items.OrderBy(i => i, Comparer);
            return oe.CreateOrderedEnumerable(keySelector, comparer, descending);
        }

        /// <summary>
        /// Returns a shallow copy of the <see cref="OrderedList{T}"/> in the reverse order.
        /// </summary>
        /// <returns>An <see cref="OrderedList{T}"/> in the reverse order.</returns>
        public OrderedList<T> Reversed()
        {
            return new OrderedList<T>(Comparer.Reversed(), Items);
        }

        /// <summary>
        /// Reverses the order of the elements in the entire <see cref="OrderedList{T}"/> and reverses the sort order accordingly.
        /// </summary>
        public void Reverse()
        {
            Items.Reverse();
            Comparer = Comparer.Reversed();
        }

        /// <summary>
        /// Copies the elements of the <see cref="OrderedList{T}"/> to a new array.
        /// </summary>
        /// <returns>An array containing copies the elements of the <see cref="OrderedList{T}"/>.</returns>
        public T[] ToArray()
        {
            return Items.ToArray();
        }

        /// <summary>
        /// Returns a read-only <see cref="ReadOnlyCollection{T}"/> wrapper for the current collection.
        /// </summary>
        /// <returns>An object that acts as a read-only wrapper for the current collection.</returns>
        public ReadOnlyCollection<T> AsReadOnly()
        {
            return Items.AsReadOnly();
        }
        #endregion
    }
}