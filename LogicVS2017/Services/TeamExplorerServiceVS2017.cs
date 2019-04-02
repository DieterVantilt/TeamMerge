using Domain.Entities;
using Logic.Services;
using LogicVS2017.Wrappers;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;
using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Workspace = Domain.Entities.Workspace;

namespace LogicVS2017.Services
{
    public class TeamExplorerServiceVS2017
        : ITeamExplorerService
    {
        private readonly ITeamExplorer _teamExplorer;
        private readonly ITFVCService _tfvcService;

        public TeamExplorerServiceVS2017(IServiceProvider serviceProvider, ITFVCService tfvcService)
        {
            _teamExplorer = (ITeamExplorer)serviceProvider.GetService(typeof(ITeamExplorer));
            _tfvcService = tfvcService;
        }

        public void AddWorkItemsAndCommentThenNavigate(Workspace workspaceModel, string comment, IEnumerable<int> workItemIds)
        {
            var workspace = _tfvcService.GetWorkspace(workspaceModel.Name, workspaceModel.OwnerName);

            var pendingChangePage = (TeamExplorerPageBase)_teamExplorer.NavigateToPage(new Guid(TeamExplorerPageIds.PendingChanges), null);
            var pendingChangeModel = (IPendingCheckin)pendingChangePage.Model;

            var modelType = pendingChangeModel.GetType();

            var propertyInfo = modelType.GetProperty("Workspace");
            propertyInfo.SetValue(pendingChangeModel, ((WorkspaceWrapper) workspace).Workspace);

            var method = modelType.GetMethod("AddWorkItemsByIdAsync", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            method.Invoke(pendingChangeModel, new object[] { workItemIds.ToArray(), 1 });

            pendingChangeModel.PendingChanges.Comment = comment;
        }
    }
}