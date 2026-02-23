using System;

namespace Heroes.GOAP.Core
{
    public sealed class TranspositionTable
    {
        private struct Entry
        {
            public AgentState State;
            public int Hash;
            public int Depth;
            public float BestCost;
            public bool HasValue;
        }

        private readonly Entry[] entries;
        private readonly int size;

        public TranspositionTable(int size = 1024)
        {
            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            this.size = size;
            entries = new Entry[size];
        }

        public void Clear()
        {
            Array.Clear(entries, 0, entries.Length);
        }

        public bool HasBetterOrEqual(AgentState state, float newCost)
        {
            var hash = state.GetHashCode();
            var idx = Index(hash);

            ref var entry = ref entries[idx];

            if (!entry.HasValue)
            {
                return false;
            }
            
            if (entry.Hash != hash)
            {
                return false;
            }
            
            if (!entry.State.Equals(state))
            {
                return false;
            }

            return entry.BestCost <= newCost;
        }


        public void AddOrImprove(AgentState state, float newCost)
        {
            var hash = state.GetHashCode();
            var idx = Index(hash);

            ref var entry = ref entries[idx];

            if (!entry.HasValue)
            {
                entry.State = state;
                entry.Hash = hash;
                entry.BestCost = newCost;
                entry.HasValue = true;
                
                return;
            }

            if (entry.Hash == hash && entry.State.Equals(state))
            {
                if (newCost < entry.BestCost)
                {
                    entry.BestCost = newCost;
                }

                return;
            }

            if (newCost < entry.BestCost)
            {
                entry.State = state;
                entry.Hash = hash;
                entry.BestCost = newCost;
                entry.HasValue = true;
            }
        }

        private int Index(int hash)
        {
            var idx = hash % size;
            return idx < 0 ? idx + size : idx;
        }
    }
}
