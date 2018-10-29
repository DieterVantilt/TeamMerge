using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TeamMerge.Base;
using TeamMerge.Commands;
using TeamMerge.Helpers;
using TeamMerge.Instellingen.Dialogs;
using TeamMerge.Merge.Context;
using TeamMerge.Operations;
using TeamMerge.Services;
using TeamMerge.Services.Models;
using TeamMerge.Utils;

namespace TeamMerge.Merge
{
    public class TeamMergeViewModel
        : TeamExplorerViewModelBase
    {
        private readonly ITeamService _teamService;
        private readonly IMergeOperation _mergeOperation;
        private readonly IConfigHelper _configHelper;
        private readonly ISolutionService _solutionService;
        private List<BranchModel> _currentBranches;

        public TeamMergeViewModel(ITeamService teamService, IMergeOperation mergeOperation, IConfigHelper configHelper, ILogger logger, ISolutionService solutionService)
            : base(logger)
        {
            _teamService = teamService;
            _mergeOperation = mergeOperation;
            _configHelper = configHelper;
            _solutionService = solutionService;

            ViewChangesetDetailsCommand = new RelayCommand(ViewChangeset, CanViewChangeset);
            MergeCommand = new AsyncRelayCommand(MergeAsync, CanMerge);
            FetchChangesetsCommand = new AsyncRelayCommand(FetchChangesetsAsync, CanFetchChangesets);
            SelectWorkspaceCommand = new RelayCommand<WorkspaceModel>(SelectWorkspace);
            OpenSettingsCommand = new RelayCommand(OpenSettings);

            SourcesBranches = new ObservableCollection<string>();
            TargetBranches = new ObservableCollection<string>();
            ProjectNames = new ObservableCollection<string>();

            Changesets = new ObservableCollection<ChangesetModel>();
            SelectedChangesets = new ObservableCollection<ChangesetModel>();

            Title = Resources.TeamMerge;
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

        private ChangesetModel _selectedChangeset;

        public ChangesetModel SelectedChangeset
        {
            get { return _selectedChangeset; }
            set
            {
                _selectedChangeset = value;
                RaisePropertyChanged(nameof(SelectedChangeset));
            }
        }

        private ObservableCollection<ChangesetModel> _selectedChangesets;

        public ObservableCollection<ChangesetModel> SelectedChangesets
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

        private ObservableCollection<ChangesetModel> _changesets;

        public ObservableCollection<ChangesetModel> Changesets
        {
            get { return _changesets; }
            protected set
            {
                _changesets = value;
                RaisePropertyChanged(nameof(Changesets));
            }
        }

        private ObservableCollection<WorkspaceModel> _workspaces;

        public ObservableCollection<WorkspaceModel> Workspaces
        {
            get { return _workspaces; }
            set { _workspaces = value; RaisePropertyChanged(nameof(Workspaces)); }
        }

        private WorkspaceModel _selectedWorkspace;

        public WorkspaceModel SelectedWorkspace
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
            await SetBusyWhileExecutingAsync(async () =>
            {
                var orderedSelectedChangesets = SelectedChangesets.OrderBy(x => x.ChangesetId).ToList();

                _mergeOperation.MyCurrentAction += MergeOperation_MyCurrentAction;

                await _mergeOperation.Execute(new MergeModel
                {
                    WorkspaceModel = SelectedWorkspace,
                    OrderedChangesetIds = orderedSelectedChangesets.Select(x => x.ChangesetId).ToList(),
                    SourceBranch = SelectedSourceBranch,
                    TargetBranch = SelectedTargetBranch
                });


                SaveDefaultSettings();
                SaveDefaultSettingsSolutionWide();

                _configHelper.SaveDictionary();
            });

            MyCurrentAction = null;
            _mergeOperation.MyCurrentAction -= MergeOperation_MyCurrentAction;
        }

        private void SaveDefaultSettingsSolutionWide()
        {
            var saveSelectedBranchSettingsBySolution = _configHelper.GetValue<bool>(ConfigKeys.SAVE_BRANCH_PERSOLUTION);
            if (saveSelectedBranchSettingsBySolution)
            {
                var currentSolution = _solutionService.GetActiveSolution()?.FullName;

                if (!string.IsNullOrWhiteSpace(currentSolution))
                {
                    var currentSettings = _configHelper.GetValue<List<DefaultMergeSettings>>(ConfigKeys.SOLUTIONWISE_SELECTEDMERGE_SETTINGS) ?? new List<DefaultMergeSettings>();
                    var currentSolutionSetting = currentSettings.SingleOrDefault(c => c.Solution == currentSolution);
                    if (currentSolutionSetting != null)
                    {
                        currentSolutionSetting.SourceBranch = SelectedSourceBranch;
                        currentSolutionSetting.TargetBranch = SelectedTargetBranch;
                        currentSolutionSetting.ProjectName = SelectedProjectName;
                    }
                    else
                    {
                        currentSettings.Add(new DefaultMergeSettings(currentSolution, SelectedProjectName, SelectedSourceBranch, SelectedTargetBranch));
                    }

                    _configHelper.AddValue(ConfigKeys.SOLUTIONWISE_SELECTEDMERGE_SETTINGS, currentSettings);
                }
            }
        }

        private void SaveDefaultSettings()
        {
            _configHelper.AddValue(ConfigKeys.SELECTED_PROJECT_NAME, SelectedProjectName);
            _configHelper.AddValue(ConfigKeys.SOURCE_BRANCH, SelectedSourceBranch);
            _configHelper.AddValue(ConfigKeys.TARGET_BRANCH, SelectedTargetBranch);
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
            await SetBusyWhileExecutingAsync(async () =>
            {
                Changesets.Clear();

                var changesets = await _teamService.GetChangesets(SelectedSourceBranch, SelectedTargetBranch);

                Changesets = new ObservableCollection<ChangesetModel>(changesets);

                if (_configHelper.GetValue<bool>(ConfigKeys.ENABLE_AUTO_SELECT_ALL_CHANGESETS))
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

        private bool CanViewChangeset()
        {
            return SelectedChangeset != null;
        }

        private void ViewChangeset()
        {
            TeamExplorerUtils.Instance.NavigateToPage(TeamExplorerPageIds.ChangesetDetails, ServiceProvider, SelectedChangeset.ChangesetId);
        }

        private void SelectWorkspace(WorkspaceModel workspace)
        {
            SelectedWorkspace = workspace;
        }

        public override async void Initialize(object sender, SectionInitializeEventArgs e)
        {
            base.Initialize(sender, e);

            await SetBusyWhileExecutingAsync(async () =>
            {
                var projectNames = await _teamService.GetProjectNames();

                Workspaces = new ObservableCollection<WorkspaceModel>(await _teamService.AllWorkspaces());
                SelectedWorkspace = _teamService.CurrentWorkspace() ?? Workspaces.First();

                projectNames.ToList().ForEach(x => ProjectNames.Add(x));

                if (e.Context != null)
                {
                    RestoreContext(e);
                }
                else
                {
                    SetSavedSelectedBranches();
                }
            });
        }

        private void SetSavedSelectedBranches()
        {
            var saveSelectedBranchSettingsBySolution = _configHelper.GetValue<bool>(ConfigKeys.SAVE_BRANCH_PERSOLUTION);
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
            var projectName = _configHelper.GetValue<string>(ConfigKeys.SELECTED_PROJECT_NAME);

            if (!string.IsNullOrWhiteSpace(projectName))
            {
                SelectedProjectName = projectName;
                SelectedSourceBranch = _configHelper.GetValue<string>(ConfigKeys.SOURCE_BRANCH);
                SelectedTargetBranch = _configHelper.GetValue<string>(ConfigKeys.TARGET_BRANCH);
            }
        }

        public void OpenSettings()
        {
            var viewModel = new InstellingenDialogViewModel(_configHelper);
            viewModel.Initialize();

            var window = new InstellingenDialog
            {
                DataContext = viewModel
            };

            window.Closing += (e, cancelEventArgs) => viewModel.OnCloseWindowRequest(cancelEventArgs);
            viewModel.RequestClose += () => window.Close();

            window.ShowDialog();
        }

        public override void SaveContext(object sender, SectionSaveContextEventArgs e)
        {
            base.SaveContext(sender, e);

            var context = new TeamMergeContext
            {
                SelectedProjectName = SelectedProjectName,
                Changesets = Changesets,
                SourceBranch = SelectedSourceBranch,
                TargetBranch = SelectedTargetBranch
            };

            e.Context = context;
        }

        private void RestoreContext(SectionInitializeEventArgs e)
        {
            var context = (TeamMergeContext)e.Context;

            SelectedProjectName = context.SelectedProjectName;
            Changesets = context.Changesets;
            SelectedSourceBranch = context.SourceBranch;
            SelectedTargetBranch = context.TargetBranch;
        }

        public override void Dispose()
        {
            base.Dispose();

            if (_selectedChangesets != null)
            {
                _selectedChangesets.CollectionChanged -= SelectedChangesets_CollectionChanged;
            }
        }
    }
}