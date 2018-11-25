using System.ComponentModel;

namespace TeamMerge.Settings.Enums
{
    public enum Branch
    {
        [Description("None")]
        None,
        [Description("Source")]
        Source,
        [Description("Target")]
        Target,
        [Description("Source and target")]
        SourceAndTarget
    }
}