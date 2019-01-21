using TeamMerge.Utils;

namespace TeamMerge.Settings.Enums
{
    public enum CheckInComment
    {
        [LocalizedDescription(nameof(Resources.None), typeof(Resources))]
        None,
        [LocalizedDescription(nameof(Resources.BranchDirectionComment), typeof(Resources))]
        BranchDirection,
        [LocalizedDescription(nameof(Resources.WorkItemIdsComment), typeof(Resources))]
        WorkItemIds,
        [LocalizedDescription(nameof(Resources.FixedComment), typeof(Resources))]
        Fixed
    }
}