using System;

namespace DotNetExtensions
{
    /// <summary>
    /// Wrapper class for an object that can be one of two different types. The inner object is accessed by casting the <see cref="Flex{T, S}"/> to its type.
    /// </summary>
    /// <typeparam name="T">The first type that the inner object can be.</typeparam>
    /// <typeparam name="S">The second type that the inner object can be.</typeparam>
    public class Flex<T, S>
    {
        /// <summary>
        /// The current type of the inner object.
        /// </summary>
        public Type InnerType { get; set; }

        protected T ItemT { get; set; }
        protected S ItemS { get; set; }

        /// <summary>
        /// Instantiates a <see cref="Flex{T, S}"/> with an inner object of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="item">The object to wrap in the <see cref="Flex{T, S}"/>.</param>
        public Flex(T item)
        {
            ItemT = item;
            InnerType = item.GetType();
        }

        /// <summary>
        /// Instantiates a <see cref="Flex{T, S}"/> with an inner object of type <typeparamref name="S"/>.
        /// </summary>
        /// <param name="item">The object to wrap in the <see cref="Flex{T, S}"/>.</param>
        public Flex(S item)
        {
            ItemS = item;
            InnerType = item.GetType();
        }

        public static implicit operator Flex<T, S>(T t) => new Flex<T, S>(t);
        public static implicit operator Flex<T, S>(S s) => new Flex<T, S>(s);

        public static explicit operator T(Flex<T, S> f)
        {
            if (f.InnerType.Is(typeof(T)))
                return f.ItemT;
            else
                throw new InvalidCastException();
        }

        public static explicit operator S(Flex<T, S> f)
        {
            if (f.InnerType.Is(typeof(S)))
                return f.ItemS;
            else
                throw new InvalidCastException();
        }
    }
}
