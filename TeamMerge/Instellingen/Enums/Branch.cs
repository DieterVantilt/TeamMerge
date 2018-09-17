using System.ComponentModel;

namespace TeamMerge.Instellingen.Enums
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