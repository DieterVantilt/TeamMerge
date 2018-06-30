using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamMerge.Exceptions;
using TeamMerge.Instellingen.Enums;
using TeamMerge.Services;
using TeamMerge.Services.Models;
using TeamMerge.Utils;

namespace TeamMerge.Operations
{
    public interface IMergeOperation
    {
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

        public async Task Execute(MergeModel mergeModel)
        {
            CheckIfPendingChanges(mergeModel.WorkspaceModel);

            await DoGetLatest(mergeModel.WorkspaceModel, mergeModel.SourceBranch, mergeModel.TargetBranch);

            await _mergeService.MergeBranches(mergeModel.WorkspaceModel, mergeModel.SourceBranch, mergeModel.TargetBranch, mergeModel.FromChangesetId, mergeModel.ToChangesetId);

            await _mergeService.AddWorkItemsAndNavigate(mergeModel.WorkspaceModel, mergeModel.ChangesetIds);
        }

        private void CheckIfPendingChanges(WorkspaceModel workspaceModel)
        {
            var shouldCheckForPendingChanges = _configHelper.GetValue<bool>(ConfigKeys.ENABLE_WARNING_WHEN_PENDING_CHANGES);

            if (shouldCheckForPendingChanges)
            {
                var hasPendingChanges = _mergeService.HasPendingChanges(workspaceModel);

                if (hasPendingChanges)
                {
                    throw new MergeActionException(Resources.WorkspaceContainsPendingChanges);
                }
            }
        }

        private async Task DoGetLatest(WorkspaceModel workspaceModel, string sourceBranch, string targetBranch)
        {
            var shouldDoGetLatest = _configHelper.GetValue<bool>(ConfigKeys.LATEST_VERSION_FOR_BRANCH);

            if (shouldDoGetLatest)
            {
                var branchNamesForLatestVersion = GetBranchesForExecutingGetLatest(sourceBranch, targetBranch);

                var hasConflicts = await _mergeService.GetLatestVersion(workspaceModel, branchNamesForLatestVersion.ToArray());

                if (hasConflicts)
                {
                    throw new MergeActionException(Resources.GetLatestConflicts);
                }
            }
        }

        private IEnumerable<string> GetBranchesForExecutingGetLatest(string sourceBranch, string targetBranch)
        {
            var latestVersionForBranches = (Branch) _configHelper.GetValue<int>(ConfigKeys.LATEST_VERSION_FOR_BRANCH);
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
    }

    public class MergeModel
    {
        public WorkspaceModel WorkspaceModel { get; set; }
        public IEnumerable<int> ChangesetIds { get; set; }
        public string SourceBranch { get; set; }
        public string TargetBranch { get; set; }
        public int FromChangesetId { get; set; }
        public int ToChangesetId { get; set; }
    }
}