namespace Domain.Entities.TFVC.Base
{
    public interface ITFVCBranchProperties
    {
        ITFVCItemIdentifier RootItem { get; }
        ITFVCItemIdentifier ParentBranch { get; }
    }
}