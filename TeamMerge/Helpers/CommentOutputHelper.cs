using System;
using System.Collections.Generic;
using System.Globalization;
using TeamMerge.Settings.Enums;

namespace TeamMerge.Helpers
{
    public static class CommentOutputHelper
    {
        public static string GetCheckInComment(CheckInComment checkInCommentChoice, string commentFormat, string sourceBranch, string targetBranch, IEnumerable<int> workItemIds)
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
                    comment = string.Format(CultureInfo.CurrentCulture, commentFormat, string.Join(", ", workItemIds));
                }
                else if (checkInCommentChoice == CheckInComment.Fixed)
                {
                    comment = commentFormat;
                }
            }
            catch (FormatException)
            {
                comment = Resources.InvalidFormat;
            }

            return comment;
        }
    }
}