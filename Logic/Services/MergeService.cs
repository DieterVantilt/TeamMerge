using Domain.Entities;
using Domain.Entities.TFVC.Base;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logic.Services
{
    public interface IMergeService
    {
        bool HasIncludedPendingChanges(Workspace workspace);
        bool HasConflicts(Workspace workspace);
        Task<bool> GetLatestVersionAsync(Workspace workspace, params string[] branchNames);
        Task ResolveConflictsAsync(Workspace workspace);
        Task MergeBranchesAsync(Workspace workspace, string source, string target, int from, int to);
        Task<IEnumerable<int>> GetWorkItemIdsAsync(IEnumerable<int> changesetIds, IEnumerable<string> workItemTypesToExclude);
    }

    public class MergeService
        : IMergeService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITFVCService _tfvcService;

        public MergeService(IServiceProvider serviceProvider, ITFVCService tFVCService)
        {
            _serviceProvider = serviceProvider;
            _tfvcService = tFVCService;
        }

        public bool HasIncludedPendingChanges(Workspace workspace)
        {
            var tfvcWorkspace = _tfvcService.GetWorkspace(workspace.Name, workspace.OwnerName);

            var anyIncludedPendingChanges = tfvcWorkspace.GetPendingChanges().Select(x => !tfvcWorkspace.LastSavedCheckin.IsExcluded(x.ServerItem)).Any(x => x);

            return anyIncludedPendingChanges;
        }

        public bool HasConflicts(Workspace workspace)
        {
            var tfvcWorkspace = _tfvcService.GetWorkspace(workspace.Name, workspace.OwnerName);

            return tfvcWorkspace.QueryConflicts(new string[0], true).Any();
        }

        public async Task<bool> GetLatestVersionAsync(Workspace workspace, params string[] branchNames)
        {
            var tfvcWorkspace = _tfvcService.GetWorkspace(workspace.Name, workspace.OwnerName);

            return await _tfvcService.GetLatestVersionAsync(tfvcWorkspace, branchNames);
        }

        public async Task ResolveConflictsAsync(Workspace workspace)
        {
            var tfvcWorkspace = _tfvcService.GetWorkspace(workspace.Name, workspace.OwnerName);

            var conflicts = tfvcWorkspace.QueryConflicts(new string[0], true);

            foreach (var conflict in conflicts)
            {
                if (await Task.Run(() => tfvcWorkspace.MergeContent(conflict, true)))
                {
                    conflict.Resolution = TFVCConflictResolution.AcceptMerge;
                    tfvcWorkspace.ResolveConflict(conflict);
                }
            }
        }

        public async Task MergeBranchesAsync(Workspace workspace, string source, string target, int from, int to)
        {
            var tfvcWorkspace = _tfvcService.GetWorkspace(workspace.Name, workspace.OwnerName);

            await _tfvcService.MergeAsync(tfvcWorkspace, source, target, from, to);
        }

        public async Task<IEnumerable<int>> GetWorkItemIdsAsync(IEnumerable<int> changesetIds, IEnumerable<string> workItemTypesToExclude)
        {
            var workItemIds = new ConcurrentBag<int>();

            var tasks = new List<Task>();

            workItemTypesToExclude = workItemTypesToExclude ?? Enumerable.Empty<string>();

            foreach (var changesetId in changesetIds)
            {
                tasks.Add(GetAssociatedWorkItemIdsAsync(changesetId, workItemIds, workItemTypesToExclude));
            }

            await Task.WhenAll(tasks.ToArray());

            return workItemIds.Distinct().OrderBy(x => x).ToList();
        }

        private async Task GetAssociatedWorkItemIdsAsync(int changesetId, ConcurrentBag<int> concurrentbag, IEnumerable<string> workItemTypesToExclude)
        {
            var changeset = await _tfvcService.GetChangesetAsync(changesetId);

            var associatedWorkItemIds = changeset.GetAssociatedWorkItems()?
                .Where(x => !workItemTypesToExclude.Contains(x.WorkItemType))
                .Select(x => x.Id) ?? new List<int>();

            associatedWorkItemIds.ToList().ForEach(x => concurrentbag.Add(x));
        }
    }
}
