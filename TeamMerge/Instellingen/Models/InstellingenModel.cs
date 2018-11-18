using TeamMerge.Base;
using TeamMerge.Instellingen.Enums;

namespace TeamMerge.Instellingen.Models
{
    public class InstellingenModel
        : ModelBase
    {
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
    }
}