using System.ComponentModel.DataAnnotations;

namespace TeamMerge.Instellingen.Enums
{
    public enum Branch
    {
        [Display(Description = nameof(Resources.None), ResourceType = typeof(Resources))]
        None,
        [Display(Description = nameof(Resources.Source), ResourceType = typeof(Resources))]
        Source,
        [Display(Description = nameof(Resources.Target), ResourceType = typeof(Resources))]
        Target,
        [Display(Description = nameof(Resources.SourceAndTarget), ResourceType = typeof(Resources))]
        SourceAndTarget
    }
}