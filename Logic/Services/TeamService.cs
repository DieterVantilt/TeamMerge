using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logic.Services
{
    public interface ITeamService
    {
        IEnumerable<Branch> GetBranches(string projectName);
        Task<IEnumerable<Changeset>> GetChangesetsAsync(string source, string target);
        Task<IEnumerable<string>> GetProjectNamesAsync();
        Task<IEnumerable<Workspace>> AllWorkspacesAsync();
        Workspace CurrentWorkspace();
        Task<IEnumerable<string>> GetAllWorkItemTypesAsync();
    }

    public class TeamService
        : ITeamService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITFVCService _tfvcService;

        public TeamService(IServiceProvider serviceProvider, ITFVCService tFVCService)
        {
            _serviceProvider = serviceProvider;
            _tfvcService = tFVCService;
        }

        public async Task<IEnumerable<string>> GetProjectNamesAsync()
        {
            var projects = await _tfvcService.ListTfsProjectsAsync();

            return projects.Select(x => x.Name).ToList();
        }

        public IEnumerable<Branch> GetBranches(string projectName)
        {
            var result = new List<Branch>();

            var branches = _tfvcService.ListBranches(projectName);

            foreach (var branchObject in branches)
            {
                var branchModel = new Branch
                {
                    Name = branchObject.Properties.RootItem.Item,
                    Branches = branchObject.ChildBranches.Where(x => !x.IsDeleted).Select(x => x.Item).ToList()
                };

                if (branchObject.Properties.ParentBranch != null)
                {
                    branchModel.Branches.Add(branchObject.Properties.ParentBranch.Item);
                }

                result.Add(branchModel);
            }

            return result.OrderBy(x => x.Name);
        }

        public async Task<IEnumerable<Changeset>> GetChangesetsAsync(string source, string target)
        {
            var mergeCandidates = await _tfvcService.GetMergeCandidatesAsync(source, target);

            return mergeCandidates.Select(x => new Changeset
            {
                ChangesetId = x.ChangesetId,
                Comment = x.Comment,
                CreationDate = x.CreationDate,
                Owner = x.OwnerDisplayName
            })
            .OrderByDescending(x => x.CreationDate)
            .ToList();
        }

        public async Task<IEnumerable<Workspace>> AllWorkspacesAsync()
        {
            var workspaces = await _tfvcService.AllWorkspacesAsync();

            return workspaces.Select(x => new Workspace { Name = x.Name, OwnerName = x.OwnerName });
        }

        public Workspace CurrentWorkspace()
        {
            var currentWorkspace = _tfvcService.CurrentWorkspace();

            return currentWorkspace == null ? null : new Workspace { Name = currentWorkspace.Name, OwnerName = currentWorkspace.OwnerName };
        }

        public Task<IEnumerable<string>> GetAllWorkItemTypesAsync()
        {
            return _tfvcService.GetAllWorkItemTypesAsync();
        }
    }
}