namespace TfsBuildNotifier.ValueObjects
{
    public class BuildStatusDef
    {
        public int BuildId { get; set; }
        public string BuildName { get; set; }
        public BuildState BuildStatus { get; set; }
    }
}