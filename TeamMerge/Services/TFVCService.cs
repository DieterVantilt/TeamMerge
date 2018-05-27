using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamMerge.Helpers;

namespace TeamMerge.Services
{
    public class TFVCService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITeamFoundationContext _context;
        private readonly VersionControlServer _versionControlServer;

        public TFVCService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _context = VersionControlHelper.GetTeamFoundationContext(_serviceProvider);
            _versionControlServer = _context.TeamProjectCollection.GetService<VersionControlServer>();
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

        public async Task Merge(string source, string target, int changesetFrom, int changesetTo)
        {
            var workspaces = await Task.Run(() => _versionControlServer.QueryWorkspaces(null, _context.TeamProjectCollection.AuthorizedIdentity.UniqueName, Environment.MachineName));

            var firstWorkSpace = workspaces.First();

            await Task.Run(() => firstWorkSpace.Merge(source, target, new ChangesetVersionSpec(changesetFrom), new ChangesetVersionSpec(changesetTo), LockLevel.None, RecursionType.Full, MergeOptions.None));
        }

        public async Task<Changeset> GetChangeset(int changesetId)
        {
            var changeset = await Task.Run(() =>_versionControlServer.GetChangeset(changesetId, false, false));

            return changeset;
        }

        public async Task<IEnumerable<Changeset>> GetMergeCandidates(string sourceBranch, string targetBranch)
        {
            var mergeCandidates = await Task.Run(() => _versionControlServer.GetMergeCandidates(sourceBranch, targetBranch, RecursionType.Full));

            return mergeCandidates.Select(x => x.Changeset).ToList();
        }
    }
}