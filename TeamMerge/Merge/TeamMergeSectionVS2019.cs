extern alias VS2019;

using Logic.Services;
using VS2019.Microsoft.TeamFoundation.Controls;
using VS2019.Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;
using Shared.Utils;
using TeamMerge.Operations;
using LogicVS2019.Services;

namespace TeamMerge.Merge
{
    [TeamExplorerSection(Guids.VS2019TeamMergeSectionId, Guids.VS2019TeamMergePageId, 10)]
    public class TeamMergeSectionVS2019
        : TeamExplorerSectionBase
    {
        //when editing this function delete everything from this folder: 'C:\Users\YOUR_NAME\AppData\Local\Microsoft\VisualStudio\NUMBER_WITH_EXP_IN'
        //FYI: This will reset the whole VS experimental instance
        //or for only resetting the extension find it and delete the folder.
        protected override ITeamExplorerSection CreateViewModel(SectionInitializeEventArgs e)
        {
            var logger = new Logger();
            var configHelper = new ConfigManager(new ConfigFileHelper());
            var versionControlService = new VersionControlServiceVS2019(ServiceProvider);            

            var solutionService = new SolutionService(ServiceProvider, configHelper);
            var tfvcService = new TFVCService(versionControlService, solutionService);

            var teamExplorerService = new TeamExplorerServiceVS2019(ServiceProvider, tfvcService);

            var teamService = new TeamService(ServiceProvider, tfvcService);
            var mergeService = new MergeService(ServiceProvider, tfvcService);

            var mergeOperation = new MergeOperation(mergeService, teamExplorerService, configHelper);

            return base.CreateViewModel(e) ?? new TeamMergeViewModel2019(teamService, mergeOperation, configHelper, logger, solutionService);
        }

        protected override object CreateView(SectionInitializeEventArgs e)
        {
            return new TeamMergeView();
        }
    }
}