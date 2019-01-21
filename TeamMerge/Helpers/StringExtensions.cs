using System.Linq;

namespace TeamMerge.Helpers
{
    public static class StringExtensions
    {
        public static string GetBranchName(this string branchPath)
        {
            return branchPath.Split('/').Last();
        }
    }
}