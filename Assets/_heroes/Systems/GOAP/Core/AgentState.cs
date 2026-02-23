using System;
using UnityEngine;

namespace Heroes.GOAP.Core
{
    public struct AgentState : IEquatable<AgentState>
    {
        private float[] believes;

        public AgentState(int believeCount)
        {
            believes = new float[believeCount];
        }

        public AgentState(AgentState other)
        {
            believes = (float[])other.believes?.Clone();
        }

        public float GetBelieve(int believe)
        {
            if (believe < 0 || believe >= believes.Length)
            {
                Debug.LogWarning($"GOAP ERROR: believe id is incorrect. {believe} is not in range (0..{believes.Length-1})");

                return 0f;
            }

            return believes[believe];
        }

        public void SetBelieve(int believe, float value)
        {
            believes[believe] = value;
        }

        public AgentState Clone() => new AgentState(this);

        public bool Equals(AgentState other)
        {
            if (believes == other.believes)
            {
                return true;
            }
            
            if (believes is null || other.believes is null)
            {
                return false;
            }
            
            if (believes.Length != other.believes.Length)
            {
                return false;
            }

            for (var i = 0; i < believes.Length; i++)
            {
                if (!believes[i].Equals(other.believes[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object obj) => obj is AgentState s && Equals(s);

        public override int GetHashCode()
        {
            if (believes is null)
            {
                return 0;
            }

            var hash = new HashCode();
            
            foreach (var believe in believes)
            {
                hash.Add(BitConverter.SingleToInt32Bits(believe));
            }
            
            return hash.ToHashCode();
        }
    }
}