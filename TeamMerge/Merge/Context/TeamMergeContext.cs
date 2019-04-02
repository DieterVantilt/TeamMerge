using Domain.Entities;
using System.Collections.ObjectModel;

namespace TeamMerge.Merge.Context
{
    public class TeamMergeContext
    {
        public string SourceBranch { get; set; }

        public string TargetBranch { get; set; }

        public string SelectedProjectName { get; set; }

        public ObservableCollection<Changeset> Changesets { get; set; }
    }
}