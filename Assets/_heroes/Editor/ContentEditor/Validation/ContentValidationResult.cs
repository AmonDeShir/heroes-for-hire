namespace Heroes.Editor.ContentEditor.Validation
{
    public enum ContentValidationSeverity
    {
        Info,
        Warning,
        Error,
    }

    public readonly struct ContentValidationResult
    {
        public string Message { get; }
        public ContentValidationSeverity Severity { get; }

        public ContentValidationResult(string message, ContentValidationSeverity severity)
        {
            Message = message;
            Severity = severity;
        }
    }
}
