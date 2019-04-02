
namespace Domain.Entities.TFVC.Enums
{
    public enum TFVCGetOptions
    {
        None = 0,
        Overwrite = 1,
        GetAll = 2,
        Preview = 4,
        Remap = 8,
        NoAutoResolve = 16
    }
}