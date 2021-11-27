using Logic.Services;
using LogicVS2022.Services;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;
using Shared.Utils;
using TeamMerge2022.Services;
using TeamMergeBase.Merge;
using TeamMergeBase.Operations;

namespace TeamMerge2022.Merge
{
    [TeamExplorerSection(Guids.TeamMergeSectionId, Guids.TeamMergePageId, 10)]
    public class TeamMergeSection
        : TeamExplorerSectionBase
    {
        protected override ITeamExplorerSection CreateViewModel(SectionInitializeEventArgs e)
        {
            var logger = new Logger();
            var configHelper = new ConfigManager(new ConfigFileHelper());
            var versionControlService = new VersionControlService(ServiceProvider);

            var solutionService = new SolutionService(ServiceProvider, configHelper);
            var tfvcService = new TFVCService(versionControlService, solutionService);

            var teamExplorerService = new TeamExplorerService(ServiceProvider, tfvcService);

            var teamService = new TeamService(ServiceProvider, tfvcService);
            var mergeService = new MergeService(ServiceProvider, tfvcService);

            var mergeOperation = new MergeOperation(mergeService, teamExplorerService, configHelper);

            return base.CreateViewModel(e) ?? new TeamMergeViewModel(teamService, mergeOperation, configHelper, logger, solutionService);
        }

        protected override object CreateView(SectionInitializeEventArgs e)
        {
            return new TeamMergeView();
        }
    }
}
