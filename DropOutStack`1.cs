namespace _0G.Legacy
{
    public class DropOutStack<T>
    {
        private readonly T[] items;
        private int top = 0;

        public int Capacity => items.Length;

        public DropOutStack(int capacity)
        {
            items = new T[capacity];
        }

        public void Push(T item)
        {
            items[top] = item;
            top = (top + 1) % Capacity;
        }

        public T Pop()
        {
            top = (Capacity + top - 1) % Capacity;
            return items[top];
        }
    }
}