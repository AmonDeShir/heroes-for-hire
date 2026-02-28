using System;
using UnityEngine;

namespace Heroes.GOAP.Core
{
    public struct AgentState : IEquatable<AgentState>
    {
        private float[] believes;
        private Vector2 location;
        public Vector2 Location => location;
        public int BeliefCount => believes?.Length ?? 0;
        
        public AgentState(int believeCount)
        {
            believes = new float[believeCount];
            location = new Vector2();
        }

        public AgentState(AgentState other)
        {
            believes = (float[])other.believes?.Clone();
            location = other.Location;
        }

        public float GetBelieve(int believe)
        {
            if (believe < 0 || believe >= believes.Length)
            {
                UnityEngine.Debug.LogWarning($"GOAP ERROR: believe id is incorrect. {believe} is not in range (0..{believes.Length-1})");

                return 0f;
            }

            return believes[believe];
        }

        public void SetBelieve(int believe, float value)
        {
            believes[believe] = value;
        }
        
        public void SetLocation(float x, float z)
        {
            location = new Vector2(x, z);
        }
        
        public void SetLocation(Vector2 location)
        {
            SetLocation(location.x, location.y);
        }
        
        public void SetLocation(Vector3 location)
        {
            SetLocation(location.x, location.z);
        }

        public AgentState Clone() => new AgentState(this);

        public bool Equals(AgentState other)
        {
            if (!Location.Equals(other.Location))
            {
                return false;
            }
            
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
            
            hash.Add(Location);
            
            return hash.ToHashCode();
        }
    }
}
