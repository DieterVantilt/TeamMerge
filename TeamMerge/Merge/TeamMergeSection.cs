using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;
using TeamMerge.Operations;
using TeamMerge.Services;
using TeamMerge.Utils;

namespace TeamMerge.Merge
{
    [TeamExplorerSection(Guids.TeamMergeSectionId, Guids.TeamMergePageId, 10)]
    public class TeamMergeSection
        : TeamExplorerSectionBase
    {
        //when editing this function delete everything from this folder: 'C:\Users\YOUR_NAME\AppData\Local\Microsoft\VisualStudio\NUMBER_WITH_EXP_IN'
        //FYI: This will reset the whole VS experimental instance
        //or for only resetting the extension find it and delete the folder.
        protected override ITeamExplorerSection CreateViewModel(SectionInitializeEventArgs e)
        {
            var logger = new Logger();
            var configHelper = new ConfigHelper();

            var solutionService = new SolutionService(ServiceProvider, configHelper);
            var tfvcService = new TFVCService(ServiceProvider, solutionService);

            var teamService = new TeamService(ServiceProvider, tfvcService);
            var mergeService = new MergeService(ServiceProvider, tfvcService);

            var mergeOperation = new MergeOperation(mergeService, configHelper);

            return base.CreateViewModel(e) ?? new TeamMergeViewModel(teamService, mergeOperation, configHelper, logger, solutionService);
        }

        protected override object CreateView(SectionInitializeEventArgs e)
        {
            return new TeamMergeView();
        }
    }
}