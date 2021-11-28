using Domain.Entities.TFVC;
using Domain.Entities.TFVC.Base;
using Domain.Entities.TFVC.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logic.Services
{
    public interface ITFVCService
    {
        Task<ITFVCChangeset> GetChangesetAsync(int changesetId);
        Task<IEnumerable<ITFVCChangeset>> GetMergeCandidatesAsync(string sourceBranch, string targetBranch);
        IEnumerable<ITFVCBranchObject> ListBranches(string projectName);
        Task<IEnumerable<ITFVCTeamProject>> ListTfsProjectsAsync();
        Task MergeAsync(ITFVCWorkspace workspace, string source, string target, int changesetFrom, int changesetTo);
        Task<IEnumerable<ITFVCWorkspace>> AllWorkspacesAsync();
        ITFVCWorkspace CurrentWorkspace();
        ITFVCWorkspace GetWorkspace(string workspaceName, string workspaceOwner);
        Task<bool> GetLatestVersionAsync(ITFVCWorkspace workspace, params string[] branchNames);
        Task<IEnumerable<string>> GetAllWorkItemTypesAsync();
    }

    public class TFVCService
        : ITFVCService
    {
        private readonly IVersionControlService _versionControlService;
        private readonly ISolutionService _solutionService;

        public TFVCService(IVersionControlService versionControlService, ISolutionService solutionService)
        {
            _versionControlService = versionControlService;
            _solutionService = solutionService;
        }

        public async Task<IEnumerable<ITFVCWorkspace>> AllWorkspacesAsync()
        {
            return await Task.Run(() => _versionControlService.QueryWorkspaces(null, null, Environment.MachineName));
        }

        public ITFVCWorkspace CurrentWorkspace()
        {
            ITFVCWorkspace result = null;

            var fullName = _solutionService.GetActiveSolution()?.FullName;

            if (!string.IsNullOrWhiteSpace(fullName))
            {
                var solutionDir = System.IO.Path.GetDirectoryName(fullName);

                result = _versionControlService.TryGetWorkspace(solutionDir);
            }

            return result;
        }

        public Task<IEnumerable<string>> GetAllWorkItemTypesAsync()
        {
            return _versionControlService.GetAllWorkItemTypesAsync();
        }

        public async Task<ITFVCChangeset> GetChangesetAsync(int changesetId)
        {
            return await Task.Run(() => _versionControlService.GetChangeset(changesetId, false, false));
        }

        public async Task<bool> GetLatestVersionAsync(ITFVCWorkspace workspace, params string[] branchNames)
        {
            var getRequests = branchNames.Select(x => new TFVCGetRequest { Item = x, Recursion = TFVCRecursionType.Full, LatestVersion = true });

            var getStatusResult = await Task.Run(() => workspace.Get(getRequests.ToArray(), TFVCGetOptions.None));

            return getStatusResult.NumConflicts > 0;
        }

        public async Task<IEnumerable<ITFVCChangeset>> GetMergeCandidatesAsync(string sourceBranch, string targetBranch)
        {
            var mergeCandidates = await Task.Run(() => _versionControlService.GetMergeCandidates(sourceBranch, targetBranch, TFVCRecursionType.Full));

            return mergeCandidates.Select(x => x.Changeset).ToList();
        }

        public ITFVCWorkspace GetWorkspace(string workspaceName, string workspaceOwner)
        {
            return _versionControlService.GetWorkspace(workspaceName, workspaceOwner);
        }

        public IEnumerable<ITFVCBranchObject> ListBranches(string projectName)
        {
            var branchObjects = _versionControlService.QueryRootBranchObjects(TFVCRecursionType.Full);

            var result = new List<ITFVCBranchObject>();
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

        public async Task<IEnumerable<ITFVCTeamProject>> ListTfsProjectsAsync()
        {
            return await Task.Run(() => _versionControlService.GetAllTeamProjects(true));
        }

        public Task MergeAsync(ITFVCWorkspace workspace, string source, string target, int changesetFrom, int changesetTo)
        {
            return Task.Run(() => workspace.Merge(source, target, new TFVCChangesetVersionSpec(changesetFrom), new TFVCChangesetVersionSpec(changesetTo), TFVCLockLevel.None, TFVCRecursionType.Full, TFVCMergeOptions.None));
        }
    }
}
