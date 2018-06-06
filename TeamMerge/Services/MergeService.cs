using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;
using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TeamMerge.Services.Models;

namespace TeamMerge.Services
{
    public interface IMergeService
    {
        Task MergeBranches(WorkspaceModel workspaceModel, string source, string target, int from, int to);
        Task AddWorkItemsAndNavigate(IEnumerable<int> changesetIds, WorkspaceModel workspaceModel);
    }

    public class MergeService 
        : IMergeService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITFVCService _tfvcService;
        private readonly ITeamExplorer _teamExplorer;

        public MergeService(IServiceProvider serviceProvider, ITFVCService tFVCService)
        {
            _serviceProvider = serviceProvider;
            _tfvcService = tFVCService;
            _teamExplorer = (ITeamExplorer)_serviceProvider.GetService(typeof(ITeamExplorer));
        }

        public async Task MergeBranches(WorkspaceModel workspaceModel, string source, string target, int from, int to)
        {
            var workspace = _tfvcService.GetWorkspace(workspaceModel.Name, workspaceModel.OwnerName);

            await _tfvcService.Merge(workspace, source, target, from, to);
        }

        public async Task AddWorkItemsAndNavigate(IEnumerable<int> changesetIds, WorkspaceModel workspaceModel)
        {
            var workspace = _tfvcService.GetWorkspace(workspaceModel.Name, workspaceModel.OwnerName);
            var workItemIds = new ConcurrentBag<int>();

            var tasks = new List<Task>();

            foreach(var changesetId in changesetIds)
            {
                tasks.Add(GetAssociatedWorkItemIds(changesetId, workItemIds));
            }

            await Task.WhenAll(tasks.ToArray());

            var pendingChangePage = (TeamExplorerPageBase)_teamExplorer.NavigateToPage(new Guid(TeamExplorerPageIds.PendingChanges), null);
            var pendingChangeModel = (IPendingCheckin) pendingChangePage.Model;

            var modelType = pendingChangeModel.GetType();

            var propertyInfo = modelType.GetProperty("Workspace");
            propertyInfo.SetValue(pendingChangeModel, workspace);

            var method = modelType.GetMethod("AddWorkItemsByIdAsync", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            method.Invoke(pendingChangeModel, new object[] { workItemIds.ToArray(), 1 });
        }

        private async Task GetAssociatedWorkItemIds(int changesetId, ConcurrentBag<int> concurrentbag)
        {
            var changeset = await _tfvcService.GetChangeset(changesetId);

            var associatedWorkItemIds = changeset.AssociatedWorkItems?.Select(x => x.Id) ?? new List<int>();

            associatedWorkItemIds.ToList().ForEach(x => concurrentbag.Add(x));
        }
    }
}