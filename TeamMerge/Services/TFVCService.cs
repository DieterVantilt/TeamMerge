using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamMerge.Helpers;

namespace TeamMerge.Services
{
    public interface ITFVCService
    {
        Task<Changeset> GetChangeset(int changesetId);
        Task<IEnumerable<Changeset>> GetMergeCandidates(string sourceBranch, string targetBranch);
        IEnumerable<BranchObject> ListBranches(string projectName);
        Task<IEnumerable<TeamProject>> ListTfsProjects();
        Task Merge(Workspace workspace, string source, string target, int changesetFrom, int changesetTo);
        Task<IEnumerable<Workspace>> AllWorkspaces();
        Workspace CurrentWorkspace();
        Workspace GetWorkspace(string workspaceName, string workspaceOwner);
        Task<bool> GetLatestVersion(Workspace workspace, params string[] branchNames);
        IEnumerable<string> GetAllWorkItemTypes();
    }

    public class TFVCService
        : ITFVCService
    {
        private readonly ISolutionService _solutionService;
        private readonly VersionControlServer _versionControlServer;

        public TFVCService(IServiceProvider serviceProvider, ISolutionService solutionService)
        {
            _solutionService = solutionService;
            var context = VersionControlHelper.GetTeamFoundationContext(serviceProvider);
            _versionControlServer = context.TeamProjectCollection.GetService<VersionControlServer>();
        }

        public async Task<IEnumerable<TeamProject>> ListTfsProjects()
        {
            return await Task.Run(() => _versionControlServer.GetAllTeamProjects(true));
        }

        public IEnumerable<BranchObject> ListBranches(string projectName)
        {
            var branchObjects = _versionControlServer.QueryRootBranchObjects(RecursionType.Full);

            var result = new List<BranchObject>();
            foreach (var branchObject in branchObjects)
            {
                var ro = branchObject.Properties.RootItem;

                if (!ro.IsDeleted && ro.Item.Replace(@"$/", "").StartsWith(projectName + @"/"))
                {
                    result.Add(branchObject);
                }
            }

            return result;
        }

        public async Task Merge(Workspace workspace, string source, string target, int changesetFrom, int changesetTo)
        {
            await Task.Run(() => workspace.Merge(source, target, new ChangesetVersionSpec(changesetFrom), new ChangesetVersionSpec(changesetTo), LockLevel.None, RecursionType.Full, MergeOptions.None));
        }

        public async Task<Changeset> GetChangeset(int changesetId)
        {
            var changeset = await Task.Run(() => _versionControlServer.GetChangeset(changesetId, false, false));

            return changeset;
        }

        public async Task<IEnumerable<Changeset>> GetMergeCandidates(string sourceBranch, string targetBranch)
        {
            var mergeCandidates = await Task.Run(() => _versionControlServer.GetMergeCandidates(sourceBranch, targetBranch, RecursionType.Full));

            return mergeCandidates.Select(x => x.Changeset).ToList();
        }

        public async Task<IEnumerable<Workspace>> AllWorkspaces()
        {
            return await Task.Run(() => _versionControlServer.QueryWorkspaces(null, null, Environment.MachineName));
        }

        public Workspace CurrentWorkspace()
        {
            Workspace result = null;

            var fullName = _solutionService.GetActiveSolution().FullName;

            if (!string.IsNullOrWhiteSpace(fullName))
            {
                var solutionDir = System.IO.Path.GetDirectoryName(fullName);

                result = _versionControlServer.TryGetWorkspace(solutionDir);
            }

            return result;
        }

        public Workspace GetWorkspace(string workspaceName, string workspaceOwner)
        {
            return _versionControlServer.GetWorkspace(workspaceName, workspaceOwner);
        }

        public async Task<bool> GetLatestVersion(Workspace workspace, params string[] branchNames)
        {
            var getRequests = branchNames.Select(x => new GetRequest(x, RecursionType.Full, VersionSpec.Latest));

            var getStatusResult = await Task.Run(() => workspace.Get(getRequests.ToArray(), GetOptions.None));

            return getStatusResult.NumConflicts > 0;
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
    }
}