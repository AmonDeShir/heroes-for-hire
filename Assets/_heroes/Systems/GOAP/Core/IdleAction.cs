using System;

namespace Heroes.GOAP.Core
{
    public sealed class IdleAction<TAgent, TSnapshot> where TSnapshot : IReadOnlyWorldSnapshot
    {
        public string Name { get; }
        public string Description { get; }
        public Func<TAgent, AgentContext<TSnapshot>, IActionStrategy> Implementation { get; }

        public IdleAction(string name, string description, Func<TAgent, AgentContext<TSnapshot>, IActionStrategy> implementation)
        {
            Name = name ?? string.Empty;
            Description = description ?? string.Empty;
            Implementation = implementation;
        }
    }
}
