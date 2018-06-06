using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TeamMerge.Base;
using TeamMerge.Commands;
using TeamMerge.Helpers;
using TeamMerge.Merge.Context;
using TeamMerge.Services;
using TeamMerge.Services.Models;
using TeamMerge.Utils;

namespace TeamMerge.Merge
{
    public class TeamMergeViewModel 
        : TeamExplorerViewModelBase
    {
        private readonly ITeamService _teamService;
        private readonly IMergeService _mergeService;
        private readonly IConfigHelper _configHelper;
        private List<BranchModel> _currentBranches;

        public TeamMergeViewModel(ITeamService teamService, IMergeService mergeService, IConfigHelper configHelper)
        {
            _teamService = teamService;
            _mergeService = mergeService;
            _configHelper = configHelper;

            ViewChangesetDetailsCommand = new RelayCommand(ViewChangeset, CanViewChangeset);
            MergeCommand = new AsyncRelayCommand(MergeAsync, CanMerge);
            FetchChangesetsCommand = new AsyncRelayCommand(FetchChangesetsAsync, CanFetchChangesets);
            SelectWorkspaceCommand = new RelayCommand<WorkspaceModel>(SelectWorkspace);

            SourcesBranches = new ObservableCollection<string>();
            TargetBranches = new ObservableCollection<string>();
            ProjectNames = new ObservableCollection<string>();

            Changesets = new ObservableCollection<ChangesetModel>();
            SelectedChangesets = new ObservableCollection<ChangesetModel>();

            Title = Resources.TeamMerge;
        }

        public IRelayCommand ViewChangesetDetailsCommand { get; private set; }
        public IRelayCommand MergeCommand { get; private set; }
        public IRelayCommand FetchChangesetsCommand { get; private set; }
        public IRelayCommand SelectWorkspaceCommand { get; private set; }

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

        private async Task MergeAsync()
        {
            await SetBusyWhileExecutingAsync(async () =>
            {
                var orderedSelectedChangesets = SelectedChangesets.OrderBy(x => x.ChangesetId).ToList();

                await _mergeService.MergeBranches(SelectedWorkspace, SelectedSourceBranch, SelectedTargetBranch, orderedSelectedChangesets.First().ChangesetId, orderedSelectedChangesets.Last().ChangesetId);

                await _mergeService.AddWorkItemsAndNavigate(orderedSelectedChangesets.Select(x => x.ChangesetId), SelectedWorkspace);

                _configHelper.AddValue(ConfigManager.SELECTED_PROJECT_NAME, SelectedProjectName);
                _configHelper.AddValue(ConfigManager.SOURCE_BRANCH, SelectedSourceBranch);
                _configHelper.AddValue(ConfigManager.TARGET_BRANCH, SelectedTargetBranch);

                _configHelper.SaveDictionary();
            });
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
                    var projectName = _configHelper.GetValue<string>(ConfigManager.SELECTED_PROJECT_NAME);

                    if (!string.IsNullOrEmpty(projectName))
                    {
                        SelectedProjectName = projectName;
                        SelectedSourceBranch = _configHelper.GetValue<string>(ConfigManager.SOURCE_BRANCH);
                        SelectedTargetBranch = _configHelper.GetValue<string>(ConfigManager.TARGET_BRANCH);
                    }
                }
            });
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
            var context = (TeamMergeContext) e.Context;

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