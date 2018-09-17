using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamMerge.Utils
{
    public static class ConfigKeys
    {
        public static readonly string SELECTED_PROJECT_NAME = "SelectedProjectName";
        public static readonly string SOURCE_BRANCH = "SourceBranch";
        public static readonly string TARGET_BRANCH = "TargetBranch";
        public static readonly string ENABLE_WARNING_WHEN_PENDING_CHANGES = "EnableWarning";
        public static readonly string ENABLE_AUTO_SELECT_ALL_CHANGESETS = "EnableAutoSelectAllChangesets";
        public static readonly string LATEST_VERSION_FOR_BRANCH = "LatestVersionForBranch";
        public static readonly string SHOULD_RESOLVE_CONFLICTS = "ShouldResolveConflicts";
    }
}
