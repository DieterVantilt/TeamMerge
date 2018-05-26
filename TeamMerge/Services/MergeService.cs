using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;
using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TeamMerge.Services
{
    public class MergeService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TFSService _tfsService;
        private readonly ITeamExplorer _teamExplorer;

        public MergeService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _tfsService = new TFSService(serviceProvider);
            _teamExplorer = (ITeamExplorer)_serviceProvider.GetService(typeof(ITeamExplorer));
        }

        public async Task MergeBranches(string source, string target, int from, int to)
        {
            await _tfsService.Merge(source, target, from, to);
        }

        public async Task AddWorkItemsAndNavigate(IEnumerable<int> changesetIds)
        {
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
            var method = modelType.GetMethod("AddWorkItemsByIdAsync", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            method.Invoke(pendingChangeModel, new object[] { workItemIds.ToArray(), 1 });
        }

        private async Task GetAssociatedWorkItemIds(int changesetId, ConcurrentBag<int> concurrentbag)
        {
            var changeset = await _tfsService.GetChangeset(changesetId);

            var associatedWorkItemIds = changeset.AssociatedWorkItems?.Select(x => x.Id) ?? new List<int>();

            associatedWorkItemIds.ToList().ForEach(x => concurrentbag.Add(x));
        }
    }
}