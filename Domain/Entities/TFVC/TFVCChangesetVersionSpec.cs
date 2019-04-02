
namespace Domain.Entities.TFVC
{
    public class TFVCChangesetVersionSpec
    {
        public TFVCChangesetVersionSpec(int changesetId)
        {
            ChangesetId = changesetId;
        }

        public int ChangesetId { get; }
    }
}