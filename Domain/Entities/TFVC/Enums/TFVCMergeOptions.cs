
namespace Domain.Entities.TFVC.Enums
{
    public enum TFVCMergeOptions
    {
        None = 1,
        ForceMerge = 2,
        Baseless = 4,
        NoMerge = 8,
        AlwaysAcceptMine = 16
    }
}