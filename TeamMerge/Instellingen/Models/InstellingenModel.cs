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
            set { _latestVersionBranch = value; RaisePropertyChanged(nameof(LatestVersionBranch)); }
        }
    }
}