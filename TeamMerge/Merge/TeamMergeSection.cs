﻿using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;

namespace TeamMerge.Merge
{
    [TeamExplorerSection(Guids.TeamMergeSectionId, Guids.TeamMergePageId, 10)]
    public class TeamMergeSection 
        : TeamExplorerSectionBase
    {
        protected override ITeamExplorerSection CreateViewModel(SectionInitializeEventArgs e)
        {
            return base.CreateViewModel(e) ?? new TeamMergeViewModel();
        }

        protected override object CreateView(SectionInitializeEventArgs e)
        {
            return new TeamMergeView();
        }
    }
}