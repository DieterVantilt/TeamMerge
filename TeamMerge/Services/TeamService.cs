using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamMerge.Services.Models;

namespace TeamMerge.Services
{
    public interface ITeamService
    {
        IEnumerable<BranchModel> GetBranches(string projectName);
        Task<IEnumerable<ChangesetModel>> GetChangesets(string source, string target);
        Task<IEnumerable<string>> GetProjectNames();
        Task<IEnumerable<WorkspaceModel>> AllWorkspaces();
        WorkspaceModel CurrentWorkspace();
        IEnumerable<string> GetAllWorkItemTypes();
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

        public async Task<IEnumerable<string>> GetProjectNames()
        {
            var projects = await _tfvcService.ListTfsProjects();

            return projects.Select(x => x.Name).ToList();
        }

        public IEnumerable<BranchModel> GetBranches(string projectName)
        {
            var result = new List<BranchModel>();

            var branches = _tfvcService.ListBranches(projectName);

            foreach (var branchObject in branches)
            {
                var branchModel = new BranchModel
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

        public async Task<IEnumerable<ChangesetModel>> GetChangesets(string source, string target)
        {
            var mergeCandidates = await _tfvcService.GetMergeCandidates(source, target);

            return mergeCandidates.Select(x => new ChangesetModel
            {
                ChangesetId = x.ChangesetId,
                Comment = x.Comment,
                CreationDate = x.CreationDate,
                Owner = x.OwnerDisplayName
            })
            .OrderByDescending(x => x.CreationDate)
            .ToList();
        }

        public async Task<IEnumerable<WorkspaceModel>> AllWorkspaces()
        {
            var workspaces = await _tfvcService.AllWorkspaces();

            return workspaces.Select(x => new WorkspaceModel { Name = x.Name, OwnerName = x.OwnerName });
        }

        public WorkspaceModel CurrentWorkspace()
        {
            var currentWorkspace = _tfvcService.CurrentWorkspace();

            return currentWorkspace == null ? null : new WorkspaceModel { Name = currentWorkspace.Name, OwnerName = currentWorkspace.OwnerName };
        }

        public IEnumerable<string> GetAllWorkItemTypes()
        {
            return _tfvcService.GetAllWorkItemTypes();
        }
    }
}