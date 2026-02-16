using System.Collections.Generic;

namespace Heroes.Goap.Runtime.Planner
{
    class GoapPriorityQueue<T>
    {
        readonly List<(T Item, float Priority)> m_Items = new List<(T, float)>();

        public int Count => m_Items.Count;

        public void Enqueue(T item, float priority)
        {
            m_Items.Add((item, priority));
            SiftUp(m_Items.Count - 1);
        }

        public T Dequeue()
        {
            var root = m_Items[0].Item;
            var last = m_Items[m_Items.Count - 1];
            m_Items.RemoveAt(m_Items.Count - 1);
            if (m_Items.Count > 0)
            {
                m_Items[0] = last;
                SiftDown(0);
            }

            return root;
        }

        void SiftUp(int index)
        {
            while (index > 0)
            {
                int parent = (index - 1) / 2;
                if (m_Items[index].Priority >= m_Items[parent].Priority)
                    break;

                (m_Items[index], m_Items[parent]) = (m_Items[parent], m_Items[index]);
                index = parent;
            }
        }

        void SiftDown(int index)
        {
            int lastIndex = m_Items.Count - 1;
            while (true)
            {
                int left = index * 2 + 1;
                int right = index * 2 + 2;
                if (left > lastIndex)
                    break;

                int smallest = left;
                if (right <= lastIndex && m_Items[right].Priority < m_Items[left].Priority)
                    smallest = right;

                if (m_Items[index].Priority <= m_Items[smallest].Priority)
                    break;

                (m_Items[index], m_Items[smallest]) = (m_Items[smallest], m_Items[index]);
                index = smallest;
            }
        }
    }
}
