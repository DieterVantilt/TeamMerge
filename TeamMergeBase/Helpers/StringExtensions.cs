using System.Linq;

namespace TeamMergeBase.Helpers
{
    public static class StringExtensions
    {
        public static string GetBranchName(this string branchPath)
        {
            return branchPath.Split('/').Last();
        }
    }
}