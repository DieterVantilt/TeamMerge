using Microsoft.TeamFoundation.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamMerge.Merge;
using TeamMerge.Merge.Context;
using TeamMerge.Operations;
using TeamMerge.Services;
using TeamMerge.Services.Models;
using TeamMerge.Utils;

namespace TeamMerge.Tests.Merge
{
    [TestClass]
    public class TeamMergeViewModelTests
    {
        private TeamMergeViewModel _sut;

        private ITeamService _teamService;
        private IMergeOperation _mergeOperation;
        private IConfigManager _configManager;
        private IServiceProvider _serviceProvider;
        private ILogger _logger;
        private ISolutionService _solutionService;

        [TestInitialize]
        public void Initialize()
        {
            _teamService = MockRepository.GenerateStrictMock<ITeamService>();
            _mergeOperation = MockRepository.GenerateStrictMock<IMergeOperation>();
            _configManager = MockRepository.GenerateStrictMock<IConfigManager>();
            _serviceProvider = MockRepository.GenerateStrictMock<IServiceProvider>();
            _logger = MockRepository.GenerateStrictMock<ILogger>();
            _solutionService = MockRepository.GenerateStrictMock<ISolutionService>();

            _sut = new TeamMergeViewModel(_teamService, _mergeOperation, _configManager, _logger, _solutionService);
        }

        [TestMethod]
        public void TeamMergeViewModel_Initialize_WhenCalled_InitializesTheSelectableProjectNames()
        {
            var workspaceModel = new WorkspaceModel { Name = "Go test", OwnerName = "14525" };
            var projectName1 = "Project1";
            var projectName2 = "Project2";

            _teamService.Expect(x => x.GetProjectNames()).Return(Task.FromResult<IEnumerable<string>>(new List<string> { projectName1, projectName2 }));

            _teamService.Expect(x => x.AllWorkspaces()).Return(Task.FromResult<IEnumerable<WorkspaceModel>>(new List<WorkspaceModel> { workspaceModel }));
            _teamService.Expect(x => x.CurrentWorkspace()).Return(workspaceModel);

            _configManager.Expect(x => x.GetValue<bool>(ConfigKeys.SAVE_BRANCH_PERSOLUTION)).Return(false);
            _configManager.Expect(x => x.GetValue<string>(ConfigKeys.SELECTED_PROJECT_NAME)).Return(null);

            _sut.Initialize(this, new SectionInitializeEventArgs(_serviceProvider, null));

            Assert.AreEqual(2, _sut.ProjectNames.Count);
            Assert.IsTrue(_sut.ProjectNames.Contains(projectName1));
            Assert.IsTrue(_sut.ProjectNames.Contains(projectName2));
        }

        [TestMethod]
        public void TeamMergeViewModel_Initialize_WhenSavePerSolution_AndSolutionHasSavedPreferences_ThenRestoreFromSavedPreferences()
        {
            var workspaceModel = new WorkspaceModel { Name = "Go test", OwnerName = "14525" };

            const string solutionName = "Solution";
            const string projectName = "Project";
            const string selectedSource = "$/TFS/Dev";
            const string selectedTarget = "$/TFS/Main";

            var defaultMergeSetting = new DefaultMergeSettings{
                Solution = solutionName,
                ProjectName = projectName,
                SourceBranch = selectedSource,
                TargetBranch = selectedTarget
            };

            _teamService.Expect(x => x.GetProjectNames()).Return(Task.FromResult<IEnumerable<string>>(new List<string> { projectName }));

            _teamService.Expect(x => x.AllWorkspaces()).Return(Task.FromResult<IEnumerable<WorkspaceModel>>(new List<WorkspaceModel> { workspaceModel }));
            _teamService.Expect(x => x.CurrentWorkspace()).Return(workspaceModel);
            _teamService.Expect(x => x.GetBranches(projectName)).Return(new List<BranchModel>() { new BranchModel() { Name = selectedSource, Branches = new List<string>() { selectedTarget } } });

            _solutionService.Expect(x => x.GetDefaultMergeSettingsForCurrentSolution()).Return(defaultMergeSetting);

            _configManager.Expect(x => x.GetValue<bool>(ConfigKeys.SAVE_BRANCH_PERSOLUTION)).Return(true);

            _sut.Initialize(this, new SectionInitializeEventArgs(_serviceProvider, null));

            Assert.IsTrue(_sut.SelectedProjectName == projectName);
            Assert.IsTrue(_sut.SelectedSourceBranch == selectedSource);
            Assert.IsTrue(_sut.SelectedTargetBranch == selectedTarget);
        }

        [TestMethod]
        public void TeamMergeViewModel_Initialize_WhenSavePerSolution_ButSolutionHasNoPreferencesYet_ThenRestoreFromDefaultPreference()
        {
            var workspaceModel = new WorkspaceModel { Name = "Go test", OwnerName = "14525" };
            const string projectName = "Project";
            const string selectedSource = "$/TFS/Dev";
            const string selectedTarget = "$/TFS/Main";

            _teamService.Expect(x => x.GetProjectNames()).Return(Task.FromResult<IEnumerable<string>>(new List<string> { projectName }));

            _teamService.Expect(x => x.AllWorkspaces()).Return(Task.FromResult<IEnumerable<WorkspaceModel>>(new List<WorkspaceModel> { workspaceModel }));
            _teamService.Expect(x => x.CurrentWorkspace()).Return(workspaceModel);
            _teamService.Expect(x => x.GetBranches(projectName)).Return(new List<BranchModel>() { new BranchModel() { Name = selectedSource, Branches = new List<string>() { selectedTarget } } });

            _solutionService.Expect(x => x.GetDefaultMergeSettingsForCurrentSolution()).Return(null);

            _configManager.Expect(x => x.GetValue<bool>(ConfigKeys.SAVE_BRANCH_PERSOLUTION)).Return(true);
            _configManager.Expect(x => x.GetValue<string>(ConfigKeys.SELECTED_PROJECT_NAME)).Return(projectName);
            _configManager.Expect(x => x.GetValue<string>(ConfigKeys.SOURCE_BRANCH)).Return(selectedSource);
            _configManager.Expect(x => x.GetValue<string>(ConfigKeys.TARGET_BRANCH)).Return(selectedTarget);

            _sut.Initialize(this, new SectionInitializeEventArgs(_serviceProvider, null));

            Assert.IsTrue(_sut.SelectedProjectName == projectName);
            Assert.IsTrue(_sut.SelectedSourceBranch == selectedSource);
            Assert.IsTrue(_sut.SelectedTargetBranch == selectedTarget);
        }

        [TestMethod]
        public void TeamMergeViewModel_Initialize_WhenCurrentWorkspaceFound_SetAsSelectedWorkspace()
        {
            var workspaceModel1 = new WorkspaceModel { Name = "Go test1", OwnerName = "14590" };
            var workspaceModel2 = new WorkspaceModel { Name = "Go test2", OwnerName = "14525" };

            _teamService.Expect(x => x.GetProjectNames()).Return(Task.FromResult(Enumerable.Empty<string>()));

            _teamService.Expect(x => x.AllWorkspaces()).Return(Task.FromResult<IEnumerable<WorkspaceModel>>(new List<WorkspaceModel> { workspaceModel1, workspaceModel2 }));
            _teamService.Expect(x => x.CurrentWorkspace()).Return(workspaceModel2);

            _configManager.Expect(x => x.GetValue<bool>(ConfigKeys.SAVE_BRANCH_PERSOLUTION)).Return(false);
            _configManager.Expect(x => x.GetValue<string>(ConfigKeys.SELECTED_PROJECT_NAME)).Return(null);

            _sut.Initialize(this, new SectionInitializeEventArgs(_serviceProvider, null));

            Assert.AreEqual(2, _sut.Workspaces.Count);
            Assert.IsTrue(_sut.Workspaces.Contains(workspaceModel1));
            Assert.IsTrue(_sut.Workspaces.Contains(workspaceModel2));
            Assert.AreEqual(workspaceModel2, _sut.SelectedWorkspace);
        }

        [TestMethod]
        public void TeamMergeViewModel_Initialize_WhenCurrentWorkspaceNotFound_SetFirstWorkspaceFromWorkspacesAsSelectedWorkspace()
        {
            var workspaceModel1 = new WorkspaceModel { Name = "Go test1", OwnerName = "14590" };
            var workspaceModel2 = new WorkspaceModel { Name = "Go test2", OwnerName = "14525" };

            _teamService.Expect(x => x.GetProjectNames()).Return(Task.FromResult(Enumerable.Empty<string>()));

            _teamService.Expect(x => x.AllWorkspaces()).Return(Task.FromResult<IEnumerable<WorkspaceModel>>(new List<WorkspaceModel> { workspaceModel1, workspaceModel2 }));
            _teamService.Expect(x => x.CurrentWorkspace()).Return(null);

            _configManager.Expect(x => x.GetValue<bool>(ConfigKeys.SAVE_BRANCH_PERSOLUTION)).Return(false);
            _configManager.Expect(x => x.GetValue<string>(ConfigKeys.SELECTED_PROJECT_NAME)).Return(null);

            _sut.Initialize(this, new SectionInitializeEventArgs(_serviceProvider, null));

            Assert.AreEqual(2, _sut.Workspaces.Count);
            Assert.IsTrue(_sut.Workspaces.Contains(workspaceModel1));
            Assert.IsTrue(_sut.Workspaces.Contains(workspaceModel2));
            Assert.AreEqual(workspaceModel1, _sut.SelectedWorkspace);
        }

        [TestMethod]
        public void TeamMergeViewModel_Initialize_WhenConfigurationFound_ThenSetsConfigurationData()
        {
            TestLoadingFromContextOrFromConfig(() =>
            {
                _configManager.Expect(x => x.GetValue<bool>(ConfigKeys.SAVE_BRANCH_PERSOLUTION)).Return(false);
                _configManager.Expect(x => x.GetValue<string>(ConfigKeys.SELECTED_PROJECT_NAME)).Return("Project1");
                _configManager.Expect(x => x.GetValue<string>(ConfigKeys.SOURCE_BRANCH)).Return("Branch1");
                _configManager.Expect(x => x.GetValue<string>(ConfigKeys.TARGET_BRANCH)).Return("SourceBranch1");

                _sut.Initialize(this, new SectionInitializeEventArgs(_serviceProvider, null));
            });
        }

        [TestMethod]
        public void TeamMergeViewModel_Initialize_WhenCalledWithContext_SetsTheContextData()
        {
            TestLoadingFromContextOrFromConfig(() =>
            {
                _sut.Initialize(this, new SectionInitializeEventArgs(_serviceProvider, new TeamMergeContext
                {
                    Changesets = new System.Collections.ObjectModel.ObservableCollection<ChangesetModel>(),
                    SelectedProjectName = "Project1",
                    SourceBranch = "Branch1",
                    TargetBranch = "SourceBranch1"
                }));
            });
        }

        private void TestLoadingFromContextOrFromConfig(Action setUpForContextOrConfig)
        {
            var projectName = "Project1";
            var branch = new BranchModel { Branches = new List<string> { "SourceBranch1", "SourceBranch2" }, Name = "Branch1" };
            var workspaceModel1 = new WorkspaceModel { Name = "Go test1", OwnerName = "14590" };

            _teamService.Expect(x => x.GetProjectNames()).Return(Task.FromResult<IEnumerable<string>>(new List<string> { "WrongProjectName", projectName }));

            _teamService.Expect(x => x.AllWorkspaces()).Return(Task.FromResult<IEnumerable<WorkspaceModel>>(new List<WorkspaceModel> { workspaceModel1 }));
            _teamService.Expect(x => x.CurrentWorkspace()).Return(null);

            _teamService.Expect(x => x.GetBranches(projectName)).Return(new List<BranchModel> { branch });

            setUpForContextOrConfig();

            Assert.AreEqual(1, _sut.SourcesBranches.Count);
            Assert.AreEqual(2, _sut.TargetBranches.Count);
            Assert.AreEqual(projectName, _sut.SelectedProjectName);
            Assert.AreEqual(branch.Name, _sut.SelectedSourceBranch);
            Assert.AreEqual(branch.Branches[0], _sut.SelectedTargetBranch);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _teamService.VerifyAllExpectations();
            _mergeOperation.VerifyAllExpectations();
            _configManager.VerifyAllExpectations();
            _serviceProvider.VerifyAllExpectations();
            _logger.VerifyAllExpectations();
        }
    }
}