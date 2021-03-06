﻿extern alias VS2019;

using Logic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Workspace = Domain.Entities.Workspace;
using VS2019::Microsoft.TeamFoundation.Controls;
using VS2019::Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;

namespace LogicVS2019.Services
{
    public class TeamExplorerServiceVS2019
        : ITeamExplorerService
    {
        private readonly ITeamExplorer _teamExplorer;
        private readonly ITFVCService _tfvcService;

        public TeamExplorerServiceVS2019(IServiceProvider serviceProvider, ITFVCService tfvcService)
        {
            _teamExplorer = (ITeamExplorer)serviceProvider.GetService(typeof(ITeamExplorer));
            _tfvcService = tfvcService;
        }

        public void AddWorkItemsAndCommentThenNavigate(Workspace workspaceModel, string comment, IEnumerable<int> workItemIds)
        {
            var workspace = _tfvcService.GetWorkspace(workspaceModel.Name, workspaceModel.OwnerName);

            var pendingChangePage = (TeamExplorerPageBase)_teamExplorer.NavigateToPage(new Guid(TeamExplorerPageIds.PendingChanges), null);
            var pendingChangeModel = pendingChangePage.Model;

            var modelType = pendingChangeModel.GetType();

            var propertyInfo = modelType.GetProperty("Workspace");
            propertyInfo.SetValue(pendingChangeModel, workspace.GetType().GetProperty("Workspace").GetValue(workspace, null));

            var method = modelType.GetMethod("AddWorkItemsByIdAsync", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            method.Invoke(pendingChangeModel, new object[] { workItemIds.ToArray(), 1 });

            modelType.GetProperty("CheckinComment", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .SetValue(pendingChangeModel, comment);
        }
    }
}