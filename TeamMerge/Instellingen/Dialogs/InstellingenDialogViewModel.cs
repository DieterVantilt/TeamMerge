using Microsoft.TeamFoundation.MVVM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TeamMerge.Commands;
using TeamMerge.Instellingen.Enums;
using TeamMerge.Instellingen.Models;
using TeamMerge.Services;
using TeamMerge.Utils;
using RelayCommand = TeamMerge.Commands.RelayCommand;

namespace TeamMerge.Instellingen.Dialogs
{
    public class InstellingenDialogViewModel
        : ViewModelBase
    {
        private readonly IConfigHelper _configHelper;
        private readonly ITeamService _teamService;

        public InstellingenDialogViewModel(IConfigHelper configHelper, ITeamService teamService)
        {
            _configHelper = configHelper;
            _teamService = teamService;

            SaveCommand = new RelayCommand(Save);
            CloseCommand = new RelayCommand(Close);
            AddWorkItemTypeToExcludeCommand = new RelayCommand<KeyEventArgs>(AddWorkItemTypeToExclude);
            RemoveWorkItemTypeCommand = new RelayCommand<string>(RemoveWorkItemType);
        }

        public IRelayCommand AddWorkItemTypeToExcludeCommand { get; }
        public IRelayCommand RemoveWorkItemTypeCommand { get; }
        public IRelayCommand SaveCommand { get; }
        public IRelayCommand CloseCommand { get; }

        private IEnumerable<string> _workItemTypes;

        public IEnumerable<string> WorkItemTypes
        {
            get { return _workItemTypes; }
            set { _workItemTypes = value; RaisePropertyChanged(nameof(WorkItemTypes)); }
        }

        private string _selectedWorkItemType;

        public string SelectedWorkItemType
        {
            get { return _selectedWorkItemType; }
            set { _selectedWorkItemType = value; RaisePropertyChanged(nameof(SelectedWorkItemType)); }
        }

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
            set
            {
                _isDirty = value;
                RaisePropertyChanged(nameof(IsDirty));
            }
        }

        private void AddWorkItemTypeToExclude(KeyEventArgs obj)
        {
            if (obj == null || obj.Key == Key.Enter)
            {
                if (!string.IsNullOrWhiteSpace(SelectedWorkItemType) && !Model.WorkItemTypesToExclude.Contains(SelectedWorkItemType))
                {
                    Model.WorkItemTypesToExclude.Add(SelectedWorkItemType);
                }

                SelectedWorkItemType = null;
            }
        }

        private void RemoveWorkItemType(string workItemTypeToRemove)
        {
            Model.WorkItemTypesToExclude.Remove(workItemTypeToRemove);
        }

        public void Initialize()
        {
            WorkItemTypes = _teamService.GetAllWorkItemTypes();

            Model = new InstellingenModel
            {
                EnablePendingChangesWarning = _configHelper.GetValue<bool>(ConfigKeys.ENABLE_WARNING_WHEN_PENDING_CHANGES),
                EnableAutoSelectAllChangesets = _configHelper.GetValue<bool>(ConfigKeys.ENABLE_AUTO_SELECT_ALL_CHANGESETS),
                LatestVersionBranch = (Branch)_configHelper.GetValue<int>(ConfigKeys.LATEST_VERSION_FOR_BRANCH),
                ShouldResolveConflicts = _configHelper.GetValue<bool>(ConfigKeys.SHOULD_RESOLVE_CONFLICTS),
                SaveSelectedBranchPerSolution = _configHelper.GetValue<bool>(ConfigKeys.SAVE_BRANCH_PERSOLUTION),
                WorkItemTypesToExclude = new ObservableCollection<string>(_configHelper.GetValue<ObservableCollection<string>>(ConfigKeys.WORK_ITEM_TYPES_TO_EXCLUDE) ?? Enumerable.Empty<string>()) 
            };

            IsDirty = false;
            Model.PropertyChanged += (s, ea) => IsDirty = true;
            Model.WorkItemTypesToExclude.CollectionChanged += (s, ea) => IsDirty = true;
        }

        public void Save()
        {
            _configHelper.AddValue(ConfigKeys.ENABLE_AUTO_SELECT_ALL_CHANGESETS, Model.EnableAutoSelectAllChangesets);
            _configHelper.AddValue(ConfigKeys.ENABLE_WARNING_WHEN_PENDING_CHANGES, Model.EnablePendingChangesWarning);
            _configHelper.AddValue(ConfigKeys.LATEST_VERSION_FOR_BRANCH, Model.LatestVersionBranch);
            _configHelper.AddValue(ConfigKeys.SHOULD_RESOLVE_CONFLICTS, Model.ShouldResolveConflicts);
            _configHelper.AddValue(ConfigKeys.SAVE_BRANCH_PERSOLUTION, Model.SaveSelectedBranchPerSolution);
            _configHelper.AddValue(ConfigKeys.WORK_ITEM_TYPES_TO_EXCLUDE, Model.WorkItemTypesToExclude);

            _configHelper.SaveDictionary();

            IsDirty = false;
        }

        public void OnCloseWindowRequest(CancelEventArgs e)
        {
            if (IsDirty)
            {
                var result = MessageBox.Show(Resources.SaveYourChanges, Resources.Save, MessageBoxButton.YesNoCancel, MessageBoxImage.Information);

                if (result == MessageBoxResult.Yes)
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