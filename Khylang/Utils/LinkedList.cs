namespace Khylang.Utils
{
    class Cons<T>
    {
        private readonly T _head;
        private readonly Cons<T> _tail;

        public Cons(T head, Cons<T> tail)
        {
            _head = head;
            _tail = tail;
        }

        public T Head
        {
            get { return _head; }
        }

        public Cons<T> Tail
        {
            get { return _tail; }
        }
    }

    static class LinkedList
    {
        public static Cons<T> Cons<T>(this Cons<T> item, T value)
        {
            return new Cons<T>(value, item);
        }
    }
}
