using System.Collections.ObjectModel;
using TeamMerge.Base;
using TeamMerge.Settings.Enums;

namespace TeamMerge.Settings.Models
{
    public class SettingsModel
        : ModelBase
    {
        public SettingsModel()
        {
            WorkItemTypesToExclude = new ObservableCollection<string>();
        }

        private bool _enablePendingChangesWarning;

        public bool EnablePendingChangesWarning
        {
            get { return _enablePendingChangesWarning; }
            set { _enablePendingChangesWarning = value; RaisePropertyChanged(nameof(EnablePendingChangesWarning)); }
        }

        private bool _enableAutoSelectAllChangesets;

        public bool EnableAutoSelectAllChangesets
        {
            get { return _enableAutoSelectAllChangesets; }
            set { _enableAutoSelectAllChangesets = value; RaisePropertyChanged(nameof(EnableAutoSelectAllChangesets)); }
        }

        private Branch _latestVersionBranch;

        public Branch LatestVersionBranch
        {
            get { return _latestVersionBranch; }
            set
            {
                _latestVersionBranch = value;
                RaisePropertyChanged(nameof(LatestVersionBranch));

                if (LatestVersionBranch == Branch.None)
                {
                    ShouldResolveConflicts = false;
                }
            }
        }

        private bool _shouldResolveConflicts;

        public bool ShouldResolveConflicts
        {
            get { return _shouldResolveConflicts; }
            set { _shouldResolveConflicts = value; RaisePropertyChanged(nameof(ShouldResolveConflicts)); }
        }

        private bool _saveSelectedBranchPerSolution;

        public bool SaveSelectedBranchPerSolution
        {
            get { return _saveSelectedBranchPerSolution; }
            set { _saveSelectedBranchPerSolution = value; RaisePropertyChanged(nameof(SaveSelectedBranchPerSolution)); }
        }

        private CheckInComment _checkInComment;

        public CheckInComment CheckInComment
        {
            get { return _checkInComment; }
            set
            {
                _checkInComment = value;
                RaisePropertyChanged(nameof(CheckInComment));

                if (_checkInComment == CheckInComment.None)
                {
                    CommentFormat = null;
                }
                else if (_checkInComment == CheckInComment.MergeDirection)
                {
                    CommentFormat = Resources.BranchDirectionFormat;
                }
                else if (_checkInComment == CheckInComment.WorkItemIds)
                {
                    CommentFormat = Resources.WorkItemIdsFormat;
                }
                else if (_checkInComment == CheckInComment.Fixed)
                {
                    CommentFormat = string.Empty;
                }
                else if (_checkInComment == CheckInComment.MergeDirectionAndWorkItems)
                {
                    CommentFormat = Resources.MergeDirectionAndWorkItemsFormat;
                }
                else if (_checkInComment == CheckInComment.ChangesetIds)
                {
                    CommentFormat = Resources.ChangesetIdsFormat;
                }
                else if (_checkInComment == CheckInComment.MergeDirectionAndChangesetIds)
                {
                    CommentFormat = Resources.MergeDirectionAndChangesetIdsFormat;
                }
                else if (_checkInComment == CheckInComment.ChangesetsDetails)
                {
                    CommentFormat = Resources.ChangesetCommentFormat;
                }
                else if (_checkInComment == CheckInComment.MergeDirectionChangesetsDetails)
                {
                    CommentFormat = Resources.BranchDirectionFormat;
                    CommentLineFormat = Resources.ChangesetCommentFormat;
                }
            }
        }

        private string _commentFormat;

        public string CommentFormat
        {
            get { return _commentFormat; }
            set { _commentFormat = value; RaisePropertyChanged(nameof(CommentFormat)); }
        }

        private string _commentLineFormat;

        public string CommentLineFormat
        {
            get { return _commentLineFormat; }
            set { _commentLineFormat = value; RaisePropertyChanged(nameof(CommentLineFormat)); }
        }

        private bool _shouldShowLatestVersionMerge;

        public bool ShouldShowLatestVersionMerge
        {
            get { return _shouldShowLatestVersionMerge; }
            set { _shouldShowLatestVersionMerge = value; RaisePropertyChanged(nameof(ShouldShowLatestVersionMerge)); }
        }

        private bool _excludeWorkItemsForMerge;

        public bool ExcludeWorkItemsForMerge
        {
            get { return _excludeWorkItemsForMerge; }
            set { _excludeWorkItemsForMerge = value; RaisePropertyChanged(nameof(ExcludeWorkItemsForMerge)); }
        }

        private bool _shouldShowButtonSwitchingSourceTargetBranch;

        public bool ShouldShowButtonSwitchingSourceTargetBranch
        {
            get { return _shouldShowButtonSwitchingSourceTargetBranch;  }
            set { _shouldShowButtonSwitchingSourceTargetBranch = value; RaisePropertyChanged(nameof(ShouldShowButtonSwitchingSourceTargetBranch)); }
        }

        public ObservableCollection<string> WorkItemTypesToExclude { get; set; }
    }
}