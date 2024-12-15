namespace YSMConcept.API.Helpers
{
    public class FileSettings
    {
        public long MaxSize { get; set; }
        public string[]? AllowedTypes { get; set; }
        public FileSettings() { }
        public FileSettings(
            long maxSize,
            string[]? allowedTypes)
        {
            MaxSize = maxSize;
            AllowedTypes = allowedTypes;
        }
    }
}
