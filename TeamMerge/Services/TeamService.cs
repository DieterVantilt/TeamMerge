﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamMerge.Services.Models;

namespace TeamMerge.Services
{
    public class TeamService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TFVCService _tfvcService;

        public TeamService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _tfvcService = new TFVCService(_serviceProvider);
        }

        public async Task<IEnumerable<string>> GetProjectNames()
        {
            var projects = await _tfvcService.ListTfsProjects();

            return projects.Select(x => x.Name).ToList();
        }

        public IEnumerable<Branch> GetBranches(string projectName)
        {
            var result = new List<Branch>();

            var branches = _tfvcService.ListBranches(projectName);

            foreach(var branchObject in branches)
            {
                var branch = new Branch
                {
                    Name = branchObject.Properties.RootItem.Item,
                    Branches = branchObject.ChildBranches.Where(x => !x.IsDeleted).Select(x => x.Item).ToList()
                };

                if (branchObject.Properties.ParentBranch != null)
                {
                    branch.Branches.Add(branchObject.Properties.ParentBranch.Item);
                }

                result.Add(branch);
            }

            return result;
        }

        public async Task<IEnumerable<ChangesetModel>> GetChangesets(string source, string target)
        {
            var changesets = new List<ChangesetModel>();

            var mergeCandidates = await _tfvcService.GetMergeCandidates(source, target);

            return mergeCandidates.Select(x => new ChangesetModel
            {
                ChangesetId = x.ChangesetId,
                Comment = x.Comment,
                CreationDate = x.CreationDate,
                Owner = x.OwnerDisplayName
            });
        }
    }
}
