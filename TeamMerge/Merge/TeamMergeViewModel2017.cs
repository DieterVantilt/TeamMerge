extern alias VS2017;

using System.Threading.Tasks;
using TeamMergeBase.Commands;
using TeamMergeBase.Merge.Context;
using TeamMergeBase.Operations;
using Logic.Services;
using Shared.Utils;
using VS2017::Microsoft.TeamFoundation.Controls;
using VS2017::Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;
using TeamMergeBase.Exceptions;
using System;
using TeamMergeBase.Merge;

namespace TeamMerge.Merge
{
    public class TeamMergeViewModel2017
        : TeamExplorerSectionViewModelBase
    {
        private readonly ILogger _logger;

        public TeamMergeViewModel2017(ITeamService teamService, IMergeOperation mergeOperation, IConfigManager configManager, ILogger logger, ISolutionService solutionService)
        {
            _logger = logger;

            TeamMergeCommandsViewModel = new TeamMergeCommonCommandsViewModel(teamService, mergeOperation, configManager, logger, solutionService, SetBusyWhileExecutingAsync);

            ViewChangesetDetailsCommand = new RelayCommand(ViewChangeset, CanViewChangeset);

            Title = Resources.TeamMerge;
        }

        public IRelayCommand ViewChangesetDetailsCommand { get; }

        public TeamMergeCommonCommandsViewModel TeamMergeCommandsViewModel { get; }

        private bool CanViewChangeset()
        {
            return TeamMergeCommandsViewModel.SelectedChangeset != null;
        }

        private void ViewChangeset()
        {
            TeamExplorerUtils.Instance.NavigateToPage(TeamExplorerPageIds.ChangesetDetails, ServiceProvider, TeamMergeCommandsViewModel.SelectedChangeset.ChangesetId);
        }

        public override async void Initialize(object sender, SectionInitializeEventArgs e)
        {
            base.Initialize(sender, e);

            await TeamMergeCommandsViewModel.InitializeAsync((TeamMergeContext)e.Context);
        }

        public override void SaveContext(object sender, SectionSaveContextEventArgs e)
        {
            base.SaveContext(sender, e);

            e.Context = TeamMergeCommandsViewModel.CreateContext();
        }

        public override void Dispose()
        {
            base.Dispose();

            TeamMergeCommandsViewModel.Cleanup();
        }

        private async Task SetBusyWhileExecutingAsync(Func<Task> task)
        {
            ShowBusy();

            try
            {
                await task();
            }
            catch (MergeActionException mergeActionEx)
            {
                ShowMessage(mergeActionEx.Message);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
                _logger.LogException(ex);
            }

            HideBusy();
        }
    }
}