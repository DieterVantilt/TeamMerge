using TeamMergeBase.Utils;

namespace TeamMergeBase.Settings.Enums
{
    public enum Branch
    {
        [LocalizedDescription(nameof(Resources.None), typeof(Resources))]
        None,
        [LocalizedDescription(nameof(Resources.Source), typeof(Resources))]
        Source,
        [LocalizedDescription(nameof(Resources.Target), typeof(Resources))]
        Target,
        [LocalizedDescription(nameof(Resources.SourceAndTarget), typeof(Resources))]
        SourceAndTarget
    }
}