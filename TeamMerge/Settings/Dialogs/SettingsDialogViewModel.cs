﻿using Logic.Services;
using Shared.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TeamMerge.Base;
using TeamMerge.Commands;
using TeamMerge.Helpers;
using TeamMerge.Settings.Enums;
using TeamMerge.Settings.Models;
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
            get
            {
                var comment = CommentOutputHelper.GetCheckInComment(Model.CheckInComment, Model.CommenFormat, SOURCE_BRANCH_NAME_EXAMPLE, TARGET_BRANCH_NAME_EXAMPLE, new List<int> { 5, 12, 235 }, new List<int> { 1, 2, 5, 10 }, false);

                if (Model.ShouldShowLatestVersionMerge)
                {
                    comment += Environment.NewLine + CommentOutputHelper.GetCheckInComment(Model.CheckInComment, Model.CommenFormat, SOURCE_BRANCH_NAME_EXAMPLE, TARGET_BRANCH_NAME_EXAMPLE, Enumerable.Empty<int>(), Enumerable.Empty<int>(), true);
                }

                return comment;
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

        public async Task InitializeAsync()
        {
            Model = new SettingsModel
            {
                EnablePendingChangesWarning = _configManager.GetValue<bool>(ConfigKeys.ENABLE_WARNING_WHEN_PENDING_CHANGES),
                EnableAutoSelectAllChangesets = _configManager.GetValue<bool>(ConfigKeys.ENABLE_AUTO_SELECT_ALL_CHANGESETS),
                LatestVersionBranch = _configManager.GetValue<Branch>(ConfigKeys.LATEST_VERSION_FOR_BRANCH),
                ShouldResolveConflicts = _configManager.GetValue<bool>(ConfigKeys.SHOULD_RESOLVE_CONFLICTS),
                SaveSelectedBranchPerSolution = _configManager.GetValue<bool>(ConfigKeys.SAVE_BRANCH_PERSOLUTION),
                CheckInComment = _configManager.GetValue<CheckInComment>(ConfigKeys.CHECK_IN_COMMENT_OPTION),
                CommenFormat = _configManager.GetValue<string>(ConfigKeys.COMMENT_FORMAT),
                ExcludeWorkItemsForMerge = _configManager.GetValue<bool>(ConfigKeys.EXCLUDE_WORK_ITEMS_FOR_MERGE),
                ShouldShowLatestVersionMerge = _configManager.GetValue<bool>(ConfigKeys.SHOULD_SHOW_LATEST_VERSION_IN_COMMENT),
                WorkItemTypesToExclude = new ObservableCollection<string>(_configManager.GetValue<ObservableCollection<string>>(ConfigKeys.WORK_ITEM_TYPES_TO_EXCLUDE) ?? Enumerable.Empty<string>()), 
                ShouldShowButtonSwitchingSourceTargetBranch = _configManager.GetValue<bool>(ConfigKeys.SHOULD_SHOW_BUTTON_SWITCHING_SOURCE_TARGET_BRANCH)
            };

            IsDirty = false;
            Model.PropertyChanged += Model_PropertyChanged;
            Model.WorkItemTypesToExclude.CollectionChanged += (s, ea) => IsDirty = true;

            RaisePropertyChanged(nameof(CommentOutput));

            WorkItemTypes = await _teamService.GetAllWorkItemTypesAsync();
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IsDirty = true;

            if (e.PropertyName == nameof(SettingsModel.CommenFormat) || e.PropertyName == nameof(SettingsModel.ShouldShowLatestVersionMerge))
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
            _configManager.AddValue(ConfigKeys.EXCLUDE_WORK_ITEMS_FOR_MERGE, Model.ExcludeWorkItemsForMerge);
            _configManager.AddValue(ConfigKeys.SHOULD_SHOW_LATEST_VERSION_IN_COMMENT, Model.ShouldShowLatestVersionMerge);
            _configManager.AddValue(ConfigKeys.SHOULD_SHOW_BUTTON_SWITCHING_SOURCE_TARGET_BRANCH, Model.ShouldShowButtonSwitchingSourceTargetBranch);

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