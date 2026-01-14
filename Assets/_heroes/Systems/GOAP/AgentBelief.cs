using System;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class AgentBelief
    {
        public string Name { get; }

        public Vector3 Location => _observedLocation();

        private Func<bool> _condition = () => false;
        private Func<Vector3> _observedLocation = () => Vector3.zero;

        private AgentBelief(string name)
        {
            Name = name;
        }

        public bool Evaluate()
        {
            return _condition();
        }

        public class Builder
        {
            private readonly AgentBelief _belief;

            public Builder(string name)
            {
                _belief = new AgentBelief(name);
            }

            public Builder WithLocation(Func<Vector3> location)
            {
                _belief._observedLocation = location;

                return this;
            }

            public Builder WithCondition(Func<bool> condition)
            {
                _belief._condition = condition;

                return this;
            }

            public AgentBelief Build()
            {
                return _belief;
            }
        }
    }

    public class BeliefFactory
    {
        private readonly GoapAgent _agent;
        private readonly Dictionary<string, AgentBelief> _beliefs;

        public BeliefFactory(GoapAgent agent, Dictionary<string, AgentBelief> beliefs)
        {
            _agent = agent;
            _beliefs = beliefs;
        }

        public void AddBelief(string name, Func<bool> condition)
        {
            _beliefs.Add(name, new AgentBelief.Builder(name).WithCondition(condition).Build());
        }

        public void AddLocationBelief(string name, float minimalDistance, Transform transform)
        {
            AddLocationBelief(name, minimalDistance, transform.position);
        }

        public void AddLocationBelief(string name, float minimalDistance, Vector3 location)
        {
            _beliefs.Add(name, new AgentBelief.Builder(name)
                .WithCondition(() => IsAgentInRangeOf(location, minimalDistance))
                .WithLocation(() => location)
                .Build());
        }

        public void AddSensorBelief(string name, Sensor sensor)
        {
            _beliefs.Add(name, new AgentBelief.Builder(name)
                .WithCondition(() => sensor.IsTargetInRange)
                .WithLocation(() => sensor.TargetPosition)
                .Build());
        }

        private bool IsAgentInRangeOf(Vector3 pos, float range)
        {
            return Vector3.Distance(_agent.transform.position, pos) <= range;
        }
    }
}