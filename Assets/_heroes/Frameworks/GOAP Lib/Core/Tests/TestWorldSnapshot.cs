namespace Heroes.GOAP.Core.Tests
{
    public readonly struct TestWorldSnapshot : IReadOnlyWorldSnapshot
    {
        public int Version { get; }
        public bool IsValid { get; }

        public TestWorldSnapshot(int version = 0, bool isValid = true)
        {
            Version = version;
            IsValid = isValid;
        }
    }

    public sealed class TestWorldState : WorldStateBase<TestWorldSnapshot>
    {
        private bool isValid = true;

        public void SetValid(bool valid)
        {
            isValid = valid;
            BumpVersion();
        }

        public override TestWorldSnapshot CreateSnapshot()
        {
            return new TestWorldSnapshot(Version, isValid);
        }
    }
}
