using Domain.Entities.TFVC.Base;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace LogicVS2022.Wrappers
{
    class TeamProjectWrapper
        : ITFVCTeamProject
    {
        private readonly TeamProject _teamProject;

        public TeamProjectWrapper(TeamProject teamProject)
        {
            _teamProject = teamProject;
        }

        public string Name => _teamProject.Name;
    }
}