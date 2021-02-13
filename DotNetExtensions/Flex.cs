using System;

namespace DotNetExtensions
{
    public class Flex<T, S>
    {
        public  Type InnerType { get; set; }

        protected T ItemT { get; set; }
        protected S ItemS { get; set; }

        public Flex() { }

        public Flex(T item)
        {
            ItemT = item;
            InnerType = item.GetType();
        }

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
