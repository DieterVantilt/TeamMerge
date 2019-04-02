
namespace Domain.Entities.TFVCBase
{
    public class TFVCAssociatedWorkItem
    {
        public TFVCAssociatedWorkItem(string workItemType, int id)
        {
            WorkItemType = workItemType;
            Id = id;
        }

        public string WorkItemType { get; }
        public int Id { get; }
    }
}