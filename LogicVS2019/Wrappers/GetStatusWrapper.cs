using Domain.Entities.TFVC.Base;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace LogicVS2019.Wrappers
{
    public class GetStatusWrapper
        : ITFVCGetStatus
    {
        private readonly GetStatus _getStatus;

        public GetStatusWrapper(GetStatus getStatus)
        {
            _getStatus = getStatus;
        }

        public int NumConflicts => _getStatus.NumConflicts;
    }
}