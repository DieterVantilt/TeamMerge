using Domain.Entities.TFVC.Base;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace LogicVS2017.Wrappers
{
    public class PendingChangesWrapper
        : ITFVCPendingChanges
    {
        private readonly PendingChange _pendingChange;

        public PendingChangesWrapper(PendingChange pendingChange)
        {
           _pendingChange = pendingChange;
        }

        public string ServerItem => _pendingChange.ServerItem;
    }
}