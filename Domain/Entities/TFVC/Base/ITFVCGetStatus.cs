
namespace Domain.Entities.TFVC.Base
{
    public interface ITFVCGetStatus
    {
        int NumConflicts { get; }
    }
}