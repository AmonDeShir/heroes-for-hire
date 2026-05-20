namespace Heroes.GOAP.Core
{
    public interface IPlanExecutor
    {
        public void Update(float deltaTime);
        public void CalculatePlan();
        
        public event System.Action OnNextStepLoaded;
    }
}

