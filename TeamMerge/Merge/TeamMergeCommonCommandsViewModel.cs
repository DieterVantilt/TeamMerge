using Domain.Entities;
using Logic.Services;
using Shared.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TeamMerge.Base;
using TeamMerge.Commands;
using TeamMerge.Helpers;
using TeamMerge.Merge.Context;
using TeamMerge.Operations;
using TeamMerge.Settings.Dialogs;

namespace TeamMerge.Merge
{
    public class TeamMergeCommonCommandsViewModel 
        : ViewModelBase
    {
        private readonly ITeamService _teamService;
        private readonly IMergeOperation _mergeOperation;
        private readonly IConfigManager _configManager;
        private readonly ILogger _logger;
        private readonly ISolutionService _solutionService;
        private readonly Func<Func<Task>, Task> _setBusyWhileExecutingAsync;
        private List<Branch> _currentBranches;

        public TeamMergeCommonCommandsViewModel(ITeamService teamService, IMergeOperation mergeOperation, IConfigManager configManager, ILogger logger, ISolutionService solutionService, Func<Func<Task>, Task> setBusyWhileExecutingAsync)
        {
            _teamService = teamService;
            _mergeOperation = mergeOperation;
            _configManager = configManager;
            _logger = logger;
            _solutionService = solutionService;
            _setBusyWhileExecutingAsync = setBusyWhileExecutingAsync;

            MergeCommand = new AsyncRelayCommand(MergeAsync, CanMerge);
            FetchChangesetsCommand = new AsyncRelayCommand(FetchChangesetsAsync, CanFetchChangesets);
            SelectWorkspaceCommand = new RelayCommand<Workspace>(SelectWorkspace);
            OpenSettingsCommand = new RelayCommand(OpenSettings);

            SourcesBranches = new ObservableCollection<string>();
            TargetBranches = new ObservableCollection<string>();
            ProjectNames = new ObservableCollection<string>();

            Changesets = new ObservableCollection<Changeset>();
            SelectedChangesets = new ObservableCollection<Changeset>();
        }

        public IRelayCommand ViewChangesetDetailsCommand { get; }
        public IRelayCommand MergeCommand { get; }
        public IRelayCommand FetchChangesetsCommand { get; }
        public IRelayCommand SelectWorkspaceCommand { get; }
        public IRelayCommand OpenSettingsCommand { get; }

        public ObservableCollection<string> ProjectNames { get; set; }
        public ObservableCollection<string> SourcesBranches { get; set; }
        public ObservableCollection<string> TargetBranches { get; set; }

        private string _selectedProjectName;

        public string SelectedProjectName
        {
            get { return _selectedProjectName; }
            set
            {
                _selectedProjectName = value;
                RaisePropertyChanged(nameof(SelectedProjectName));

                _currentBranches = _teamService.GetBranches(SelectedProjectName).ToList();

                Changesets.Clear();
                SourcesBranches.Clear();
                TargetBranches.Clear();
                SourcesBranches.AddRange(_currentBranches.Select(x => x.Name));
            }
        }

        private string _selectedSourceBranch;

        public string SelectedSourceBranch
        {
            get { return _selectedSourceBranch; }
            set
            {
                _selectedSourceBranch = value;
                RaisePropertyChanged(nameof(SelectedSourceBranch));
                InitializeTargetBranches();

                FetchChangesetsCommand.RaiseCanExecuteChanged();
            }
        }

        public void InitializeTargetBranches()
        {
            TargetBranches.Clear();

            if (SelectedSourceBranch != null)
            {
                TargetBranches.AddRange(_currentBranches.Single(x => x.Name == SelectedSourceBranch).Branches);
            }
        }

        private string _selectedTargetBranch;

        public string SelectedTargetBranch
        {
            get { return _selectedTargetBranch; }
            set
            {
                _selectedTargetBranch = value;
                RaisePropertyChanged(nameof(SelectedTargetBranch));

                FetchChangesetsCommand.RaiseCanExecuteChanged();
            }
        }

        private Changeset _selectedChangeset;

        public Changeset SelectedChangeset
        {
            get { return _selectedChangeset; }
            set
            {
                _selectedChangeset = value;
                RaisePropertyChanged(nameof(SelectedChangeset));
            }
        }

        private ObservableCollection<Changeset> _selectedChangesets;

        public ObservableCollection<Changeset> SelectedChangesets
        {
            get { return _selectedChangesets; }
            set
            {
                _selectedChangesets = value;
                RaisePropertyChanged(nameof(SelectedChangesets));

                if (_selectedChangesets != null)
                {
                    _selectedChangesets.CollectionChanged += SelectedChangesets_CollectionChanged;
                }
            }
        }

        private void SelectedChangesets_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            MergeCommand.RaiseCanExecuteChanged();
        }

        private ObservableCollection<Changeset> _changesets;

        public ObservableCollection<Changeset> Changesets
        {
            get { return _changesets; }
            protected set
            {
                _changesets = value;
                RaisePropertyChanged(nameof(Changesets));
            }
        }

        private ObservableCollection<Workspace> _workspaces;

        public ObservableCollection<Workspace> Workspaces
        {
            get { return _workspaces; }
            set { _workspaces = value; RaisePropertyChanged(nameof(Workspaces)); }
        }

        private Workspace _selectedWorkspace;

        public Workspace SelectedWorkspace
        {
            get { return _selectedWorkspace; }
            set { _selectedWorkspace = value; RaisePropertyChanged(nameof(SelectedWorkspace)); }
        }

        private string _myCurrentAction;

        public string MyCurrentAction
        {
            get { return _myCurrentAction; }
            set
            {
                _myCurrentAction = value;
                RaisePropertyChanged(nameof(MyCurrentAction));
            }
        }

        private async Task MergeAsync()
        {
            await _setBusyWhileExecutingAsync(async () =>
            {
                var orderedSelectedChangesets = SelectedChangesets.OrderBy(x => x.ChangesetId).ToList();

                _mergeOperation.MyCurrentAction += MergeOperation_MyCurrentAction;

                await _mergeOperation.ExecuteAsync(new MergeModel
                {
                    WorkspaceModel = SelectedWorkspace,
                    OrderedChangesetIds = orderedSelectedChangesets.Select(x => x.ChangesetId).ToList(),
                    SourceBranch = SelectedSourceBranch,
                    TargetBranch = SelectedTargetBranch
                });

                SaveDefaultSettings();
                SaveDefaultSettingsSolutionWide();
            });

            MyCurrentAction = null;
            _mergeOperation.MyCurrentAction -= MergeOperation_MyCurrentAction;
        }

        private void SaveDefaultSettingsSolutionWide()
        {
            _solutionService.SaveDefaultMergeSettingsForCurrentSolution(new DefaultMergeSettings
            {
                ProjectName = _selectedProjectName,
                SourceBranch = _selectedSourceBranch,
                TargetBranch = _selectedTargetBranch
            });
        }

        private void SaveDefaultSettings()
        {
            _configManager.AddValue(ConfigKeys.SELECTED_PROJECT_NAME, SelectedProjectName);
            _configManager.AddValue(ConfigKeys.SOURCE_BRANCH, SelectedSourceBranch);
            _configManager.AddValue(ConfigKeys.TARGET_BRANCH, SelectedTargetBranch);

            _configManager.SaveDictionary();
        }

        private void MergeOperation_MyCurrentAction(object sender, string e)
        {
            MyCurrentAction = e;
        }

        private bool CanMerge()
        {
            return SelectedChangesets != null
                && SelectedChangesets.Any()
                && Changesets.Count(x => x.ChangesetId >= SelectedChangesets.Min(y => y.ChangesetId) &&
                                         x.ChangesetId <= SelectedChangesets.Max(y => y.ChangesetId)) == SelectedChangesets.Count;
        }

        private async Task FetchChangesetsAsync()
        {
            await _setBusyWhileExecutingAsync(async () =>
            {
                Changesets.Clear();

                var changesets = await _teamService.GetChangesetsAsync(SelectedSourceBranch, SelectedTargetBranch);

                Changesets = new ObservableCollection<Changeset>(changesets);

                if (_configManager.GetValue<bool>(ConfigKeys.ENABLE_AUTO_SELECT_ALL_CHANGESETS))
                {
                    SelectedChangesets.AddRange(Changesets.Except(SelectedChangesets));
                    RaisePropertyChanged(nameof(SelectedChangesets));
                }
            });

            MergeCommand.RaiseCanExecuteChanged();
        }

        private bool CanFetchChangesets()
        {
            return SelectedSourceBranch != null && SelectedTargetBranch != null;
        }

        private void SelectWorkspace(Workspace workspace)
        {
            SelectedWorkspace = workspace;
        }

        public async Task InitializeAsync(TeamMergeContext teamMergeContext)
        {
            await _setBusyWhileExecutingAsync(async () =>
            {
                var projectNames = await _teamService.GetProjectNamesAsync();

                Workspaces = new ObservableCollection<Workspace>(await _teamService.AllWorkspacesAsync());
                SelectedWorkspace = _teamService.CurrentWorkspace() ?? Workspaces.First();

                projectNames.ToList().ForEach(x => ProjectNames.Add(x));

                if (teamMergeContext != null)
                {
                    RestoreContext(teamMergeContext);
                }
                else
                {
                    SetSavedSelectedBranches();
                }
            });
        }

        private void SetSavedSelectedBranches()
        {
            var saveSelectedBranchSettingsBySolution = _configManager.GetValue<bool>(ConfigKeys.SAVE_BRANCH_PERSOLUTION);
            if (saveSelectedBranchSettingsBySolution)
            {
                SetDefaultSelectedSettingsPerSolution();
            }
            else
            {
                SetDefaultSelectedSettings();
            }
        }

        private void SetDefaultSelectedSettingsPerSolution()
        {
            var defaultMergeSettings = _solutionService.GetDefaultMergeSettingsForCurrentSolution();

            if (defaultMergeSettings != null && defaultMergeSettings.IsValidSettings())
            {
                SelectedProjectName = defaultMergeSettings.ProjectName;
                SelectedSourceBranch = defaultMergeSettings.SourceBranch;
                SelectedTargetBranch = defaultMergeSettings.TargetBranch;
            }
            else
            {
                SetDefaultSelectedSettings();
            }
        }

        private void SetDefaultSelectedSettings()
        {
            var projectName = _configManager.GetValue<string>(ConfigKeys.SELECTED_PROJECT_NAME);

            if (!string.IsNullOrWhiteSpace(projectName))
            {
                SelectedProjectName = projectName;
                SelectedSourceBranch = _configManager.GetValue<string>(ConfigKeys.SOURCE_BRANCH);
                SelectedTargetBranch = _configManager.GetValue<string>(ConfigKeys.TARGET_BRANCH);
            }
        }

        public void OpenSettings()
        {
            var viewModel = new SettingsDialogViewModel(_configManager, _teamService);
            viewModel.Initialize();

            var window = new SettingsDialog
            {
                DataContext = viewModel
            };

            window.Closing += (e, cancelEventArgs) => viewModel.OnCloseWindowRequest(cancelEventArgs);
            viewModel.RequestClose += () => window.Close();

            window.ShowDialog();
        }

        private void RestoreContext(TeamMergeContext context)
        {
            SelectedProjectName = context.SelectedProjectName;
            Changesets = context.Changesets;
            SelectedSourceBranch = context.SourceBranch;
            SelectedTargetBranch = context.TargetBranch;
        }

        public TeamMergeContext CreateContext()
        {
            return new TeamMergeContext
            {
                SelectedProjectName = SelectedProjectName,
                Changesets = Changesets,
                SourceBranch = SelectedSourceBranch,
                TargetBranch = SelectedTargetBranch
            };
        }

        public void Cleanup()
        {
            if (_selectedChangesets != null)
            {
                _selectedChangesets.CollectionChanged -= SelectedChangesets_CollectionChanged;
            }
        }
    }
}
