namespace Heroes.GOAP.Core
{
    public interface IActionStrategy
    {
        public bool CanPerform { get; }

        public bool Complete { get; }

        public void Start() { }
        public void Update(float deltaTime) { }
        public void Stop() { }
    }
}

