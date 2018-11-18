namespace TeamMerge.Services.Models
{
    public class DefaultMergeSettings
    {
        public string Solution { get; set; }
        public string ProjectName { get; set; }
        public string SourceBranch { get; set; }
        public string TargetBranch { get; set; }

        public bool IsValidSettings()
        {
            return !string.IsNullOrWhiteSpace(ProjectName) && !string.IsNullOrWhiteSpace(SourceBranch) && !string.IsNullOrWhiteSpace(TargetBranch);
        }
    }
}
