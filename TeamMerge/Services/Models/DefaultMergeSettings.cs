namespace TeamMerge.Services.Models
{
    public class DefaultMergeSettings
    {
        public DefaultMergeSettings()
        {
        }

        public DefaultMergeSettings(string solution, string projectName, string sourceBranch, string targetBranch)
        {
            Solution = solution;
            ProjectName = projectName;
            SourceBranch = sourceBranch;
            TargetBranch = targetBranch;
        }

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
