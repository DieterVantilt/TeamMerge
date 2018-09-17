using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TeamMerge.Exceptions;
using TeamMerge.Helpers;
using TeamMerge.Instellingen.Enums;
using TeamMerge.Services;
using TeamMerge.Services.Models;
using TeamMerge.Utils;

namespace TeamMerge.Operations
{
    public interface IMergeOperation
    {
        event EventHandler<string> MyCurrentAction;
        Task Execute(MergeModel mergeModel);
    }

    public class MergeOperation
        : IMergeOperation
    {
        private readonly IMergeService _mergeService;
        private readonly IConfigHelper _configHelper;

        public MergeOperation(IMergeService mergeService, IConfigHelper configHelper)
        {
            _mergeService = mergeService;
            _configHelper = configHelper;
        }

        public event EventHandler<string> MyCurrentAction;

        public async Task Execute(MergeModel mergeModel)
        {
            await CheckIfWorkspaceHasIncludedPendingChangesAsync(mergeModel.WorkspaceModel);

            await DoGetLatestOnBranchAsync(mergeModel.WorkspaceModel, mergeModel.SourceBranch, mergeModel.TargetBranch);

            SetCurrentAction(Resources.MergingBranches);
            await _mergeService.MergeBranches(mergeModel.WorkspaceModel, mergeModel.SourceBranch, mergeModel.TargetBranch, mergeModel.OrderedChangesetIds.First(), mergeModel.OrderedChangesetIds.Last());
            await _mergeService.AddWorkItemsAndNavigate(mergeModel.WorkspaceModel, mergeModel.OrderedChangesetIds);
        }

        private async Task CheckIfWorkspaceHasIncludedPendingChangesAsync(WorkspaceModel workspaceModel)
        {
            var shouldCheckForPendingChanges = _configHelper.GetValue<bool>(ConfigKeys.ENABLE_WARNING_WHEN_PENDING_CHANGES);

            if (shouldCheckForPendingChanges)
            {
                SetCurrentAction(Resources.CheckingPendingChanges);
                var hasPendingChanges = await Task.Run(() => _mergeService.HasIncludedPendingChanges(workspaceModel));

                if (hasPendingChanges)
                {
                    throw new MergeActionException(Resources.WorkspaceContainsPendingChanges);
                }
            }
        }

        private async Task DoGetLatestOnBranchAsync(WorkspaceModel workspaceModel, string sourceBranch, string targetBranch)
        {
            var latestVersionForBranches = (Branch)_configHelper.GetValue<int>(ConfigKeys.LATEST_VERSION_FOR_BRANCH);

            if (latestVersionForBranches != Branch.None)
            {
                SetCurrentAction(string.Format(CultureInfo.CurrentUICulture, Resources.GettingLatestVersionForBranch, latestVersionForBranches.GetDescription()));
                var branchNamesForLatestVersion = GetBranchesForExecutingGetLatest(latestVersionForBranches, sourceBranch, targetBranch);

                var hasConflicts = await _mergeService.GetLatestVersion(workspaceModel, branchNamesForLatestVersion.ToArray());                

                if (hasConflicts)
                {
                    var shouldResolveConflicts = _configHelper.GetValue<bool>(ConfigKeys.SHOULD_RESOLVE_CONFLICTS);

                    if (shouldResolveConflicts)
                    {
                        await _mergeService.ResolveConflicts(workspaceModel);

                        if (_mergeService.HasConflicts(workspaceModel))
                        {
                            throw new MergeActionException(Resources.GetLatestConflicts);
                        }
                    }
                    else
                    {
                        throw new MergeActionException(Resources.GetLatestConflicts);
                    }                                       
                }
            }
        }

        private IEnumerable<string> GetBranchesForExecutingGetLatest(Branch latestVersionForBranches, string sourceBranch, string targetBranch)
        {
            var branches = new List<string>();

            switch (latestVersionForBranches)
            {
                case Branch.Target:
                    branches.Add(targetBranch);
                    break;
                case Branch.Source:
                    branches.Add(sourceBranch);
                    break;
                case Branch.SourceAndTarget:
                    branches.Add(targetBranch);
                    branches.Add(sourceBranch);
                    break;
            }

            return branches;
        }

        private void SetCurrentAction(string currentAction)
        {
            MyCurrentAction?.Invoke(this, currentAction);
        }
    }

    public class MergeModel
    {
        public WorkspaceModel WorkspaceModel { get; set; }
        public IEnumerable<int> OrderedChangesetIds { get; set; }
        public string SourceBranch { get; set; }
        public string TargetBranch { get; set; }
    }
}