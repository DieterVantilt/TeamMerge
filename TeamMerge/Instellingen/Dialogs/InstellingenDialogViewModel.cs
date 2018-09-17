using System;
using System.ComponentModel;
using System.Windows;
using Microsoft.TeamFoundation.MVVM;
using TeamMerge.Commands;
using TeamMerge.Instellingen.Enums;
using TeamMerge.Instellingen.Models;
using TeamMerge.Utils;
using RelayCommand = TeamMerge.Commands.RelayCommand;

namespace TeamMerge.Instellingen.Dialogs
{
    public class InstellingenDialogViewModel
        : ViewModelBase
    {
        private IConfigHelper _configHelper;

        public InstellingenDialogViewModel(IConfigHelper configHelper)
        {
            _configHelper = configHelper;

            SaveCommand = new RelayCommand(Save);
            CloseCommand = new RelayCommand(Close);
        }

        public IRelayCommand SaveCommand { get; }

        public IRelayCommand CloseCommand { get; }

        private InstellingenModel _model;

        public InstellingenModel Model
        {
            get { return _model; }
            set { _model = value; RaisePropertyChanged(nameof(Model)); }
        }

        private bool _isDirty;

        public bool IsDirty
        {
            get { return _isDirty; }
            set {
                _isDirty = value;
                RaisePropertyChanged(nameof(IsDirty));
            }
        }

        public void Initialize()
        {
            Model = new InstellingenModel
            {
                EnablePendingChangesWarning = _configHelper.GetValue<bool>(ConfigKeys.ENABLE_WARNING_WHEN_PENDING_CHANGES),
                EnableAutoSelectAllChangesets = _configHelper.GetValue<bool>(ConfigKeys.ENABLE_AUTO_SELECT_ALL_CHANGESETS),
                LatestVersionBranch = (Branch)_configHelper.GetValue<int>(ConfigKeys.LATEST_VERSION_FOR_BRANCH),
                ShouldResolveConflicts = _configHelper.GetValue<bool>(ConfigKeys.SHOULD_RESOLVE_CONFLICTS)
            };

            IsDirty = false;
            Model.PropertyChanged += (s, ea) => IsDirty = true;
        }

        public void Save()
        {
            _configHelper.AddValue(ConfigKeys.ENABLE_AUTO_SELECT_ALL_CHANGESETS, Model.EnableAutoSelectAllChangesets);
            _configHelper.AddValue(ConfigKeys.ENABLE_WARNING_WHEN_PENDING_CHANGES, Model.EnablePendingChangesWarning);
            _configHelper.AddValue(ConfigKeys.LATEST_VERSION_FOR_BRANCH, Model.LatestVersionBranch);
            _configHelper.AddValue(ConfigKeys.SHOULD_RESOLVE_CONFLICTS, Model.ShouldResolveConflicts);

            _configHelper.SaveDictionary();
            IsDirty = false;
        }

        public void OnCloseWindowRequest(CancelEventArgs e)
        {
            if (IsDirty)
            {
                var result = MessageBox.Show(Resources.SaveYourChanges, Resources.Save, MessageBoxButton.YesNoCancel, MessageBoxImage.Information);

                if (result == MessageBoxResult.OK)
                {
                    Save();
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }                
            }
        }

        public event Action RequestClose;

        public virtual void Close()
        {
            RequestClose?.Invoke();
        }
    }
}