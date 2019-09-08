using Domain.Entities.TFVC.Base;
using Domain.Entities.TFVC.Enums;
using Logic.Services;
using LogicVS2019.Helpers;
using LogicVS2019.Wrappers;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogicVS2019.Services
{
    public class VersionControlServiceVS2019
        : IVersionControlService
    {
        private readonly VersionControlServer _versionControlServer;

        public VersionControlServiceVS2019(IServiceProvider serviceProvider)
        {
            var context = VersionControlHelper.GetTeamFoundationContext(serviceProvider);
            _versionControlServer = context.TeamProjectCollection.GetService<VersionControlServer>();
        }

        public IEnumerable<ITFVCTeamProject> GetAllTeamProjects(bool refresh)
        {
            return _versionControlServer.GetAllTeamProjects(refresh).Select(x => new TeamProjectWrapper(x)).ToList();
        }

        public async Task<IEnumerable<string>> GetAllWorkItemTypesAsync()
        {
            var vssConnection = new VssConnection(_versionControlServer.TeamProjectCollection.Uri, _versionControlServer.TeamProjectCollection.ClientCredentials);

            using (var projectClientHttpClient = await vssConnection.GetClientAsync<ProjectHttpClient>())
            using (var workItemTrackingHttpClient = await vssConnection.GetClientAsync<WorkItemTrackingHttpClient>())
            {
                var projects = await projectClientHttpClient.GetProjects(ProjectState.All, null, null, null, null);

                var projectGuids = projects.Select(x => x.Id).ToList();

                var result = await QueuingTask.WhenAll(projectGuids, x => workItemTrackingHttpClient.GetWorkItemTypesAsync(x));

                return result
                    .Select(x => x.Name)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
            }
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