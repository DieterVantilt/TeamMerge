using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TeamMerge.Exceptions;
using TeamMerge.Helpers;
using TeamMerge.Services;
using TeamMerge.Services.Models;
using TeamMerge.Settings.Enums;
using TeamMerge.Utils;

namespace TeamMerge.Operations
{
    public interface IMergeOperation
    {
        event EventHandler<string> MyCurrentAction;
        Task ExecuteAsync(MergeModel mergeModel);
    }

    public class MergeOperation
        : IMergeOperation
    {
        private readonly IMergeService _mergeService;
        private readonly IConfigManager _configManager;

        public MergeOperation(IMergeService mergeService, IConfigManager configManager)
        {
            _mergeService = mergeService;
            _configManager = configManager;
        }

        public event EventHandler<string> MyCurrentAction;

        public async Task ExecuteAsync(MergeModel mergeModel)
        {
            await CheckIfWorkspaceHasIncludedPendingChangesAsync(mergeModel.WorkspaceModel);

            await DoGetLatestOnBranchAsync(mergeModel.WorkspaceModel, mergeModel.SourceBranch, mergeModel.TargetBranch);

            SetCurrentAction(Resources.MergingBranches);
            await _mergeService.MergeBranchesAsync(mergeModel.WorkspaceModel, mergeModel.SourceBranch, mergeModel.TargetBranch, mergeModel.OrderedChangesetIds.First(), mergeModel.OrderedChangesetIds.Last());

            var workItemIds = await _mergeService.GetWorkItemIdsAsync(mergeModel.OrderedChangesetIds, _configManager.GetValue<IEnumerable<string>>(ConfigKeys.WORK_ITEM_TYPES_TO_EXCLUDE));
            var comment = GetCommentForMerge(mergeModel.SourceBranch, mergeModel.TargetBranch, workItemIds);

            _mergeService.AddWorkItemsAndCommentThenNavigate(mergeModel.WorkspaceModel, comment, workItemIds);
        }

        private async Task CheckIfWorkspaceHasIncludedPendingChangesAsync(WorkspaceModel workspaceModel)
        {
            var shouldCheckForPendingChanges = _configManager.GetValue<bool>(ConfigKeys.ENABLE_WARNING_WHEN_PENDING_CHANGES);

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
            var latestVersionForBranches = _configManager.GetValue<Branch>(ConfigKeys.LATEST_VERSION_FOR_BRANCH);

            if (latestVersionForBranches != Branch.None)
            {
                SetCurrentAction(string.Format(CultureInfo.CurrentUICulture, Resources.GettingLatestVersionForBranch, latestVersionForBranches.GetDescription().ToLower()));
                var branchNamesForLatestVersion = GetBranchesForExecutingGetLatest(latestVersionForBranches, sourceBranch, targetBranch);

                var hasConflicts = await _mergeService.GetLatestVersionAsync(workspaceModel, branchNamesForLatestVersion.ToArray());                

                if (hasConflicts)
                {
                    var shouldResolveConflicts = _configManager.GetValue<bool>(ConfigKeys.SHOULD_RESOLVE_CONFLICTS);

                    if (shouldResolveConflicts)
                    {
                        await _mergeService.ResolveConflictsAsync(workspaceModel);

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

        private string GetCommentForMerge(string sourceBranch, string targetBranch, IEnumerable<int> workItemIds)
        {
            var checkInCommentChoice = _configManager.GetValue<CheckInComment>(ConfigKeys.CHECK_IN_COMMENT_OPTION);
            var commentFormat = _configManager.GetValue<string>(ConfigKeys.COMMENT_FORMAT);

            return CommentOutputHelper.GetCheckInComment(checkInCommentChoice, commentFormat, sourceBranch, targetBranch, workItemIds);
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