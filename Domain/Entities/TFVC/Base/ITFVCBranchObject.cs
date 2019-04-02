namespace Domain.Entities.TFVC.Base
{
    public interface ITFVCBranchObject
    {
        ITFVCBranchProperties Properties { get; }
        ITFVCItemIdentifier[] ChildBranches { get; }
    }
}