using Domain.Entities.TFVC.Base;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace LogicVS2017.Wrappers
{
    public class BranchPropertiesWrapper
        : ITFVCBranchProperties
    {
        private readonly BranchProperties _branchProperties;

        private readonly ITFVCItemIdentifier _rootItem;
        private readonly ITFVCItemIdentifier _parentBranch;

        public BranchPropertiesWrapper(BranchProperties branchProperties)
        {
            _branchProperties = branchProperties;

            _rootItem = _branchProperties.RootItem == null ? null : new ItemIdentifierWrapper(_branchProperties.RootItem);
            _parentBranch = _branchProperties.ParentBranch == null ? null : new ItemIdentifierWrapper(_branchProperties.ParentBranch);
        }        

        public ITFVCItemIdentifier RootItem => _rootItem;

        public ITFVCItemIdentifier ParentBranch => _parentBranch;
    }
}