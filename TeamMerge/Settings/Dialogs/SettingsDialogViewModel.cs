using Microsoft.TeamFoundation.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TeamMerge.Commands;
using TeamMerge.Helpers;
using TeamMerge.Services;
using TeamMerge.Settings.Enums;
using TeamMerge.Settings.Models;
using TeamMerge.Utils;
using RelayCommand = TeamMerge.Commands.RelayCommand;

namespace TeamMerge.Settings.Dialogs
{
    public class SettingsDialogViewModel
        : ViewModelBase
    {
        private const string SOURCE_BRANCH_NAME_EXAMPLE = "$/DEV";
        private const string TARGET_BRANCH_NAME_EXAMPLE = "$/MAIN";

        private readonly IConfigManager _configManager;
        private readonly ITeamService _teamService;

        public SettingsDialogViewModel(IConfigManager configManager, ITeamService teamService)
        {
            _configManager = configManager;
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

        private SettingsModel _model;

        public SettingsModel Model
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
        
        public string CommentOutput
        {
            get { return CommentOutputHelper.GetCheckInComment(Model.CheckInComment, Model.CommenFormat, SOURCE_BRANCH_NAME_EXAMPLE, TARGET_BRANCH_NAME_EXAMPLE, new List<int> { 5, 12, 235 }); }
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

            Model = new SettingsModel
            {
                EnablePendingChangesWarning = _configManager.GetValue<bool>(ConfigKeys.ENABLE_WARNING_WHEN_PENDING_CHANGES),
                EnableAutoSelectAllChangesets = _configManager.GetValue<bool>(ConfigKeys.ENABLE_AUTO_SELECT_ALL_CHANGESETS),
                LatestVersionBranch = _configManager.GetValue<Branch>(ConfigKeys.LATEST_VERSION_FOR_BRANCH),
                ShouldResolveConflicts = _configManager.GetValue<bool>(ConfigKeys.SHOULD_RESOLVE_CONFLICTS),
                SaveSelectedBranchPerSolution = _configManager.GetValue<bool>(ConfigKeys.SAVE_BRANCH_PERSOLUTION),
                CheckInComment = _configManager.GetValue<CheckInComment>(ConfigKeys.CHECK_IN_COMMENT_OPTION),
                CommenFormat = _configManager.GetValue<string>(ConfigKeys.COMMENT_FORMAT),
                WorkItemTypesToExclude = new ObservableCollection<string>(_configManager.GetValue<ObservableCollection<string>>(ConfigKeys.WORK_ITEM_TYPES_TO_EXCLUDE) ?? Enumerable.Empty<string>()) 
            };

            IsDirty = false;
            Model.PropertyChanged += Model_PropertyChanged;
            Model.WorkItemTypesToExclude.CollectionChanged += (s, ea) => IsDirty = true;

            RaisePropertyChanged(nameof(CommentOutput));
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IsDirty = true;

            if (e.PropertyName == nameof(SettingsModel.CommenFormat))
            {
                RaisePropertyChanged(nameof(CommentOutput));
            }
        }

        public void Save()
        {
            _configManager.AddValue(ConfigKeys.ENABLE_AUTO_SELECT_ALL_CHANGESETS, Model.EnableAutoSelectAllChangesets);
            _configManager.AddValue(ConfigKeys.ENABLE_WARNING_WHEN_PENDING_CHANGES, Model.EnablePendingChangesWarning);
            _configManager.AddValue(ConfigKeys.LATEST_VERSION_FOR_BRANCH, Model.LatestVersionBranch);
            _configManager.AddValue(ConfigKeys.SHOULD_RESOLVE_CONFLICTS, Model.ShouldResolveConflicts);
            _configManager.AddValue(ConfigKeys.SAVE_BRANCH_PERSOLUTION, Model.SaveSelectedBranchPerSolution);
            _configManager.AddValue(ConfigKeys.WORK_ITEM_TYPES_TO_EXCLUDE, Model.WorkItemTypesToExclude);
            _configManager.AddValue(ConfigKeys.CHECK_IN_COMMENT_OPTION, Model.CheckInComment);
            _configManager.AddValue(ConfigKeys.COMMENT_FORMAT, Model.CommenFormat);

            _configManager.SaveDictionary();

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