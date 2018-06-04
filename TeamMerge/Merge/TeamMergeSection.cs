using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;
using TeamMerge.Services;
using TeamMerge.Utils;

namespace TeamMerge.Merge
{
    [TeamExplorerSection(Guids.TeamMergeSectionId, Guids.TeamMergePageId, 10)]
    public class TeamMergeSection 
        : TeamExplorerSectionBase
    {
        protected override ITeamExplorerSection CreateViewModel(SectionInitializeEventArgs e)
        {
            var tfvcService = new TFVCService(ServiceProvider);

            return base.CreateViewModel(e) ?? new TeamMergeViewModel(new TeamService(ServiceProvider, tfvcService), new MergeService(ServiceProvider, tfvcService), new ConfigHelper());
        }

        protected override object CreateView(SectionInitializeEventArgs e)
        {
            return new TeamMergeView();
        }
    }
}