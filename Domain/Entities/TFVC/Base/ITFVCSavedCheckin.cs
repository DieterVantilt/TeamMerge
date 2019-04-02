
namespace Domain.Entities.TFVC.Base
{
    public interface ITFVCSavedCheckin
    {
        bool IsExcluded(string targetServerItem);
    }
}