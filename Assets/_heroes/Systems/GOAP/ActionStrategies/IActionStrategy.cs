namespace GOAP
{
    public interface IActionStrategy
    {
        public bool CanPreform { get; }
        public bool Complete { get; }

        public void Start() { }
        public void Update(float deltaTime) { }
        public void Stop() { }
    }
}