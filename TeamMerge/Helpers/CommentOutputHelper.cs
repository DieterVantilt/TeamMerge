using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Domain.Entities;
using Microsoft.VisualStudio.Services.Common;
using TeamMerge.Settings.Enums;

namespace TeamMerge.Helpers
{
    public static class CommentOutputHelper
    {
        public static string GetCheckInComment(CheckInComment checkInCommentChoice, string commentFormat, string commentLineFormat, string sourceBranch, string targetBranch, IEnumerable<int> workItemIds, IEnumerable<Changeset> changesets, bool isLatestVersion)
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
                    comment = string.Format(CultureInfo.CurrentCulture, commentFormat, GetIdsSplitedOrShowLatestVersionComment(workItemIds, isLatestVersion));
                }
                else if (checkInCommentChoice == CheckInComment.Fixed)
                {
                    comment = commentFormat;
                }
                else if (checkInCommentChoice == CheckInComment.MergeDirectionAndWorkItems)
                {
                    comment = string.Format(CultureInfo.CurrentCulture, commentFormat, sourceBranch.GetBranchName(), targetBranch.GetBranchName(), GetIdsSplitedOrShowLatestVersionComment(workItemIds, isLatestVersion));
                }
                else if (checkInCommentChoice == CheckInComment.ChangesetIds)
                {
                    comment = string.Format(CultureInfo.CurrentCulture, commentFormat, GetIdsSplitedOrShowLatestVersionComment(changesets.Select(x => x.ChangesetId), isLatestVersion));
                }
                else if (checkInCommentChoice == CheckInComment.MergeDirectionAndChangesetIds)
                {
                    comment = string.Format(CultureInfo.CurrentCulture, commentFormat, sourceBranch.GetBranchName(), targetBranch.GetBranchName(), GetIdsSplitedOrShowLatestVersionComment(changesets.Select(x => x.ChangesetId), isLatestVersion));
                }
                else if (checkInCommentChoice == CheckInComment.ChangesetDetailsComment)
                {
                    if (!isLatestVersion)
                    {
                        comment = CreateLineChangesetDetailComment(commentFormat, changesets);
                    }
                    else
                    {
                        comment = Resources.LatestVersion;
                    }
                }
                else if (checkInCommentChoice == CheckInComment.MergeDirectionChangesetDetailsComment)
                {
                    comment = string.Format(CultureInfo.CurrentCulture, commentFormat, sourceBranch.GetBranchName(), targetBranch.GetBranchName());

                    if (!isLatestVersion)
                    {
                        if (changesets.Any())
                        {
                            comment += Environment.NewLine;

                            comment += CreateLineChangesetDetailComment(commentLineFormat ?? string.Empty, changesets);
                        }
                    }
                    else
                    {
                        comment += Resources.LatestVersion;
                    }
                }
            }
            catch (FormatException)
            {
                comment = Resources.InvalidFormat;
            }

            return comment;
        }

        private static string GetIdsSplitedOrShowLatestVersionComment(IEnumerable<int> workItemIds, bool isLatestVersion)
        {
            if (isLatestVersion)
            {
                return Resources.LatestVersion;
            }

            return string.Join(", ", workItemIds);
        }

        private static string CreateLineChangesetDetailComment(string commentFormat, IEnumerable<Changeset> changesets)
        {
            return string.Join(Environment.NewLine, changesets.Select(x => string.Format(CultureInfo.CurrentCulture, commentFormat, x.ChangesetId, x.CreationDate, x.Owner, x.Comment)));
        }
    }
}