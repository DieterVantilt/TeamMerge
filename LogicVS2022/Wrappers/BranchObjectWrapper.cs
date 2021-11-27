using Domain.Entities.TFVC.Base;
using Microsoft.TeamFoundation.VersionControl.Client;
using System.Linq;

namespace LogicVS2022.Wrappers
{
    public class BranchObjectWrapper
        : ITFVCBranchObject
    {
        private readonly BranchObject _branchObject;

        private readonly ITFVCBranchProperties _properties;
        private readonly ITFVCItemIdentifier[] _childBranches;

        public BranchObjectWrapper(BranchObject branchObject)
        {
            _branchObject = branchObject;

            _properties = new BranchPropertiesWrapper(_branchObject.Properties);
            _childBranches = _branchObject.ChildBranches.Select(x => new ItemIdentifierWrapper(x)).ToArray();
        }

        public ITFVCBranchProperties Properties => _properties;

        public ITFVCItemIdentifier[] ChildBranches => _childBranches;
    }
}