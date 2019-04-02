using Domain.Entities.TFVC.Base;
using Domain.Entities.TFVC.Enums;
using Logic.Services;
using LogicVS2017.Helpers;
using LogicVS2017.Wrappers;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicVS2017.Services
{
    public class VersionControlServiceVS2017
        : IVersionControlService
    {
        private readonly VersionControlServer _versionControlServer;

        public VersionControlServiceVS2017(IServiceProvider serviceProvider)
        {
            var context = VersionControlHelper.GetTeamFoundationContext(serviceProvider);
            _versionControlServer = context.TeamProjectCollection.GetService<VersionControlServer>();
        }

        public IEnumerable<ITFVCTeamProject> GetAllTeamProjects(bool refresh)
        {
            return _versionControlServer.GetAllTeamProjects(refresh).Select(x => new TeamProjectWrapper(x)).ToList();
        }

        public IEnumerable<string> GetAllWorkItemTypes()
        {
            var wis = _versionControlServer.TeamProjectCollection.GetService<WorkItemStore>();

            return wis.Projects.Cast<Project>()
                .SelectMany(x => x.WorkItemTypes.Cast<WorkItemType>())
                .Select(x => x.Name)
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }

        public ITFVCChangeset GetChangeset(int changesetId, bool includeChanges, bool includeDownloadInfo)
        {
            return new ChangesetWrapper(_versionControlServer.GetChangeset(changesetId, includeChanges, includeDownloadInfo));
        }

        public IEnumerable<ITFVCMergeCandidate> GetMergeCandidates(string sourcePath, string targetPath, TFVCRecursionType recursion)
        {
            return _versionControlServer.GetMergeCandidates(sourcePath, targetPath, (RecursionType) (int) recursion).Select(x => new MergeCandidateWrapper(x));
        }

        public ITFVCWorkspace GetWorkspace(string workspaceName, string workspaceOwner)
        {
            return new WorkspaceWrapper(_versionControlServer.GetWorkspace(workspaceName, workspaceOwner));
        }

        public IEnumerable<ITFVCBranchObject> QueryRootBranchObjects(TFVCRecursionType recursion)
        {
            return _versionControlServer.QueryRootBranchObjects((RecursionType)(int)recursion).Select(x => new BranchObjectWrapper(x)).ToList();
        }

        public IEnumerable<ITFVCWorkspace> QueryWorkspaces(string workspaceName, string workspaceOwner, string computer)
        {
            return _versionControlServer.QueryWorkspaces(workspaceName, workspaceOwner, computer).Select(x => new WorkspaceWrapper(x)).ToList();
        }

        public ITFVCWorkspace TryGetWorkspace(string localPath)
        {
            return new WorkspaceWrapper(_versionControlServer.TryGetWorkspace(localPath));
        }
    }
}
