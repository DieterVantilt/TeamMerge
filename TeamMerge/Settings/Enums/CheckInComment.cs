﻿using TeamMerge.Utils;

namespace TeamMerge.Settings.Enums
{
    public enum CheckInComment
    {
        [LocalizedDescription(nameof(Resources.None), typeof(Resources))]
        None,
        [LocalizedDescription(nameof(Resources.MergeDirectionComment), typeof(Resources))]
        MergeDirection,
        [LocalizedDescription(nameof(Resources.WorkItemIdsComment), typeof(Resources))]
        WorkItemIds,
        [LocalizedDescription(nameof(Resources.FixedComment), typeof(Resources))]
        Fixed,
        [LocalizedDescription(nameof(Resources.MergeDirectionAndWorkItemsComment), typeof(Resources))]
        MergeDirectionAndWorkItems,
        [LocalizedDescription(nameof(Resources.ChangesetIdsComment), typeof(Resources))]
        ChangesetIds,
        [LocalizedDescription(nameof(Resources.MergeDirectionAndChangesetIdsComment), typeof(Resources))]
        MergeDirectionAndChangesetIds,
        [LocalizedDescription(nameof(Resources.ChangesetDetailsComment), typeof(Resources))]
        ChangesetDetailsComment,
        [LocalizedDescription(nameof(Resources.MergeDirectionChangesetDetailsComment), typeof(Resources))]
        MergeDirectionChangesetDetailsComment
    }
}