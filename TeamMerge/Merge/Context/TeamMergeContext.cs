using System.Collections.ObjectModel;
using TeamMerge.Services.Models;

namespace TeamMerge.Merge.Context
{
    public class TeamMergeContext
    {
        public string SourceBranch { get; set; }

        public string TargetBranch { get; set; }

        public string SelectedProjectName { get; set; }

        public ObservableCollection<ChangesetModel> Changesets { get; set; }
    }
}