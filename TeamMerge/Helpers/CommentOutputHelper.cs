using System;
using System.Collections.Generic;
using System.Globalization;
using TeamMerge.Settings.Enums;

namespace TeamMerge.Helpers
{
    public static class CommentOutputHelper
    {
        public static string GetCheckInComment(CheckInComment checkInCommentChoice, string commentFormat, string sourceBranch, string targetBranch, IEnumerable<int> workItemIds, bool isLatestVersion)
        {
            var comment = string.Empty;

            try
            {
                if (checkInCommentChoice == CheckInComment.MergeDirection)
                {
                    comment = string.Format(CultureInfo.CurrentCulture, commentFormat, sourceBranch.GetBranchName(), targetBranch.GetBranchName());
                }
                else if (checkInCommentChoice == CheckInComment.WorkItemIds)
                {
                    comment = string.Format(CultureInfo.CurrentCulture, commentFormat, GetWorkItemsComment(workItemIds, isLatestVersion));
                }
                else if (checkInCommentChoice == CheckInComment.Fixed)
                {
                    comment = commentFormat;
                }
                else if (checkInCommentChoice == CheckInComment.MergeDirectionAndWorkItems)
                {
                    comment = string.Format(CultureInfo.CurrentCulture, commentFormat, sourceBranch.GetBranchName(), targetBranch.GetBranchName(), GetWorkItemsComment(workItemIds, isLatestVersion));
                }
            }
            catch (FormatException)
            {
                comment = Resources.InvalidFormat;
            }

            return comment;
        }

        private static string GetWorkItemsComment(IEnumerable<int> workItemIds, bool isLatestVersion)
        {
            if (isLatestVersion)
            {
                return Resources.LatestVersion;
            }

            return string.Join(", ", workItemIds);
        }
    }
}