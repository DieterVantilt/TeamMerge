using Domain.Entities;
using System.Collections.Generic;

namespace Logic.Services
{
    public interface ITeamExplorerService
    {
        void AddWorkItemsAndCommentThenNavigate(Workspace workspaceModel, string comment, IEnumerable<int> workItemIds);
    }
}