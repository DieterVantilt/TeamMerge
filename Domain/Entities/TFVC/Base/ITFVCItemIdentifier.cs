
namespace Domain.Entities.TFVC.Base
{
    public interface ITFVCItemIdentifier
    {
        string Item { get; }
        bool IsDeleted { get; }
    }
}