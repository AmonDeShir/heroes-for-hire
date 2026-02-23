using System;

namespace Heroes.GOAP.Core
{
    public delegate void RefAction<T>(ref T value);

    public static class AgentStateExtensions
    {
        public static AgentState Mutate(this AgentState state, RefAction<AgentState> mutator)
        {
            mutator(ref state);
            
            return state;
        }
        
        public static AgentState Clamp(this AgentState state, int beliefId, float max)
        {
            var value = state.GetBelieve(beliefId);
            
            if (value > max)
            {
                state.SetBelieve(beliefId, max);
            }
            
            if (value < 0f)
            {
                state.SetBelieve(beliefId, 0f);
            }

            return state;
        }

        public static AgentState Bucket(this AgentState state, int beliefId, float step)
        {
            var value = state.GetBelieve(beliefId);
            var b = MathF.Round(value / step) * step;
            
            state.SetBelieve(beliefId, b);
            
            return state;
        }
    }
}