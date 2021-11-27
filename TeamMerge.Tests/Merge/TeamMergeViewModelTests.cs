using Domain.Entities;
using Logic.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamMergeBase.Merge;
using TeamMergeBase.Merge.Context;
using TeamMergeBase.Operations;

namespace TeamMergeBase.Tests.Merge
{
    [TestClass]
    public class TeamMergeCommonCommandsViewModelTests
    {
        private TeamMergeCommonCommandsViewModel _sut;

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

            _sut = new TeamMergeCommonCommandsViewModel(_teamService, _mergeOperation, _configManager, _logger, _solutionService, SetThingsBusyAndStuffAsync);
        }

        [TestMethod]
        public async Task TeamMergeViewModel_Initialize_WhenCalled_InitializesTheSelectableProjectNames_Async()
        {
            var workspaceModel = new Workspace { Name = "Go test", OwnerName = "14525" };
            var projectName1 = "Project1";
            var projectName2 = "Project2";

            _teamService.Expect(x => x.GetProjectNamesAsync()).Return(Task.FromResult<IEnumerable<string>>(new List<string> { projectName1, projectName2 }));

            _teamService.Expect(x => x.AllWorkspacesAsync()).Return(Task.FromResult<IEnumerable<Workspace>>(new List<Workspace> { workspaceModel }));
            _teamService.Expect(x => x.CurrentWorkspace()).Return(workspaceModel);

            _configManager.Expect(x => x.GetValue<bool>(ConfigKeys.SAVE_BRANCH_PERSOLUTION)).Return(false);
            _configManager.Expect(x => x.GetValue<string>(ConfigKeys.SELECTED_PROJECT_NAME)).Return(null);

            _configManager.Expect(x => x.GetValue<bool>(ConfigKeys.SHOULD_SHOW_BUTTON_SWITCHING_SOURCE_TARGET_BRANCH)).Return(true);

            await _sut.InitializeAsync(null);

            Assert.AreEqual(2, _sut.ProjectNames.Count);
            Assert.IsTrue(_sut.ProjectNames.Contains(projectName1));
            Assert.IsTrue(_sut.ProjectNames.Contains(projectName2));
            Assert.IsTrue(_sut.ShouldShowButtonSwitchingSourceTargetBranch);
        }

        [TestMethod]
        public async Task TeamMergeViewModel_Initialize_WhenSavePerSolution_AndSolutionHasSavedPreferences_ThenRestoreFromSavedPreferences_Async()
        {
            var workspaceModel = new Workspace { Name = "Go test", OwnerName = "14525" };

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

            _teamService.Expect(x => x.GetProjectNamesAsync()).Return(Task.FromResult<IEnumerable<string>>(new List<string> { projectName }));

            _teamService.Expect(x => x.AllWorkspacesAsync()).Return(Task.FromResult<IEnumerable<Workspace>>(new List<Workspace> { workspaceModel }));
            _teamService.Expect(x => x.CurrentWorkspace()).Return(workspaceModel);
            _teamService.Expect(x => x.GetBranches(projectName)).Return(new List<Branch>() { new Branch() { Name = selectedSource, Branches = new List<string>() { selectedTarget } } });

            _solutionService.Expect(x => x.GetDefaultMergeSettingsForCurrentSolution()).Return(defaultMergeSetting);

            _configManager.Expect(x => x.GetValue<bool>(ConfigKeys.SAVE_BRANCH_PERSOLUTION)).Return(true);

            _configManager.Expect(x => x.GetValue<bool>(ConfigKeys.SHOULD_SHOW_BUTTON_SWITCHING_SOURCE_TARGET_BRANCH)).Return(false);

            await _sut.InitializeAsync(null);

            Assert.IsTrue(_sut.SelectedProjectName == projectName);
            Assert.IsTrue(_sut.SelectedSourceBranch == selectedSource);
            Assert.IsTrue(_sut.SelectedTargetBranch == selectedTarget);
            Assert.IsFalse(_sut.ShouldShowButtonSwitchingSourceTargetBranch);
        }

        [TestMethod]
        public async Task TeamMergeViewModel_Initialize_WhenSavePerSolution_ButSolutionHasNoPreferencesYet_ThenRestoreFromDefaultPreference_Async()
        {
            var workspaceModel = new Workspace { Name = "Go test", OwnerName = "14525" };
            const string projectName = "Project";
            const string selectedSource = "$/TFS/Dev";
            const string selectedTarget = "$/TFS/Main";

            _teamService.Expect(x => x.GetProjectNamesAsync()).Return(Task.FromResult<IEnumerable<string>>(new List<string> { projectName }));

            _teamService.Expect(x => x.AllWorkspacesAsync()).Return(Task.FromResult<IEnumerable<Workspace>>(new List<Workspace> { workspaceModel }));
            _teamService.Expect(x => x.CurrentWorkspace()).Return(workspaceModel);
            _teamService.Expect(x => x.GetBranches(projectName)).Return(new List<Branch>() { new Branch() { Name = selectedSource, Branches = new List<string>() { selectedTarget } } });

            _solutionService.Expect(x => x.GetDefaultMergeSettingsForCurrentSolution()).Return(null);

            _configManager.Expect(x => x.GetValue<bool>(ConfigKeys.SAVE_BRANCH_PERSOLUTION)).Return(true);
            _configManager.Expect(x => x.GetValue<string>(ConfigKeys.SELECTED_PROJECT_NAME)).Return(projectName);
            _configManager.Expect(x => x.GetValue<string>(ConfigKeys.SOURCE_BRANCH)).Return(selectedSource);
            _configManager.Expect(x => x.GetValue<string>(ConfigKeys.TARGET_BRANCH)).Return(selectedTarget);

            _configManager.Expect(x => x.GetValue<bool>(ConfigKeys.SHOULD_SHOW_BUTTON_SWITCHING_SOURCE_TARGET_BRANCH)).Return(false);

            await _sut.InitializeAsync(null);

            Assert.IsTrue(_sut.SelectedProjectName == projectName);
            Assert.IsTrue(_sut.SelectedSourceBranch == selectedSource);
            Assert.IsTrue(_sut.SelectedTargetBranch == selectedTarget);
        }

        [TestMethod]
        public async Task TeamMergeViewModel_Initialize_WhenCurrentWorkspaceFound_SetAsSelectedWorkspace_Async()
        {
            var workspaceModel1 = new Workspace { Name = "Go test1", OwnerName = "14590" };
            var workspaceModel2 = new Workspace { Name = "Go test2", OwnerName = "14525" };

            _teamService.Expect(x => x.GetProjectNamesAsync()).Return(Task.FromResult(Enumerable.Empty<string>()));

            _teamService.Expect(x => x.AllWorkspacesAsync()).Return(Task.FromResult<IEnumerable<Workspace>>(new List<Workspace> { workspaceModel1, workspaceModel2 }));
            _teamService.Expect(x => x.CurrentWorkspace()).Return(workspaceModel2);

            _configManager.Expect(x => x.GetValue<bool>(ConfigKeys.SAVE_BRANCH_PERSOLUTION)).Return(false);
            _configManager.Expect(x => x.GetValue<string>(ConfigKeys.SELECTED_PROJECT_NAME)).Return(null);

            _configManager.Expect(x => x.GetValue<bool>(ConfigKeys.SHOULD_SHOW_BUTTON_SWITCHING_SOURCE_TARGET_BRANCH)).Return(false);

            await _sut.InitializeAsync(null);

            Assert.AreEqual(2, _sut.Workspaces.Count);
            Assert.IsTrue(_sut.Workspaces.Contains(workspaceModel1));
            Assert.IsTrue(_sut.Workspaces.Contains(workspaceModel2));
            Assert.AreEqual(workspaceModel2, _sut.SelectedWorkspace);
        }

        [TestMethod]
        public async Task TeamMergeViewModel_Initialize_WhenCurrentWorkspaceNotFound_SetFirstWorkspaceFromWorkspacesAsSelectedWorkspace_Async()
        {
            var workspaceModel1 = new Workspace { Name = "Go test1", OwnerName = "14590" };
            var workspaceModel2 = new Workspace { Name = "Go test2", OwnerName = "14525" };

            _teamService.Expect(x => x.GetProjectNamesAsync()).Return(Task.FromResult(Enumerable.Empty<string>()));

            _teamService.Expect(x => x.AllWorkspacesAsync()).Return(Task.FromResult<IEnumerable<Workspace>>(new List<Workspace> { workspaceModel1, workspaceModel2 }));
            _teamService.Expect(x => x.CurrentWorkspace()).Return(null);

            _configManager.Expect(x => x.GetValue<bool>(ConfigKeys.SAVE_BRANCH_PERSOLUTION)).Return(false);
            _configManager.Expect(x => x.GetValue<string>(ConfigKeys.SELECTED_PROJECT_NAME)).Return(null);

            _configManager.Expect(x => x.GetValue<bool>(ConfigKeys.SHOULD_SHOW_BUTTON_SWITCHING_SOURCE_TARGET_BRANCH)).Return(false);

            await _sut.InitializeAsync(null);

            Assert.AreEqual(2, _sut.Workspaces.Count);
            Assert.IsTrue(_sut.Workspaces.Contains(workspaceModel1));
            Assert.IsTrue(_sut.Workspaces.Contains(workspaceModel2));
            Assert.AreEqual(workspaceModel1, _sut.SelectedWorkspace);
        }

        [TestMethod]
        public async Task TeamMergeViewModel_Initialize_WhenConfigurationFound_ThenSetsConfigurationData_Async()
        {
            await TestLoadingFromContextOrFromConfigWithAssertionsAsync(async () =>
            {
                _configManager.Expect(x => x.GetValue<bool>(ConfigKeys.SAVE_BRANCH_PERSOLUTION)).Return(false);
                _configManager.Expect(x => x.GetValue<string>(ConfigKeys.SELECTED_PROJECT_NAME)).Return("Project1");
                _configManager.Expect(x => x.GetValue<string>(ConfigKeys.SOURCE_BRANCH)).Return("Branch1");
                _configManager.Expect(x => x.GetValue<string>(ConfigKeys.TARGET_BRANCH)).Return("SourceBranch1");

                await _sut.InitializeAsync(null);
            });
        }

        [TestMethod]
        public async Task TeamMergeViewModel_Initialize_WhenConfigurationFoundAndSourceBranchDoesNotExistAnymoreInProject_ThenDoesNotCrash_Async()
        {
            await TestLoadingFromContextOrFromConfigAsync(async () =>
            {
                _configManager.Expect(x => x.GetValue<bool>(ConfigKeys.SAVE_BRANCH_PERSOLUTION)).Return(false);
                _configManager.Expect(x => x.GetValue<string>(ConfigKeys.SELECTED_PROJECT_NAME)).Return("Project1");
                _configManager.Expect(x => x.GetValue<string>(ConfigKeys.SOURCE_BRANCH)).Return("DeletedSourceBranch");

                await _sut.InitializeAsync(null);
            });
        }

        [TestMethod]
        public async Task TeamMergeViewModel_Initialize_WhenCalledWithContext_SetsTheContextData_Async()
        {
            await TestLoadingFromContextOrFromConfigWithAssertionsAsync(async () =>
            {
                await _sut.InitializeAsync(new TeamMergeContext
                {
                    Changesets = new System.Collections.ObjectModel.ObservableCollection<Changeset>(),
                    SelectedProjectName = "Project1",
                    SourceBranch = "Branch1",
                    TargetBranch = "SourceBranch1"
                });
            });
        }

        [TestMethod]
        public async Task TeamMergeViewModel_Initialize_WhenCalledWithContextAndSourceBranchDoesNotExistAnymoreInProject_ThenDoesNotCrash_Async()
        {
            await TestLoadingFromContextOrFromConfigAsync(async () =>
            {
                await _sut.InitializeAsync(new TeamMergeContext
                {
                    Changesets = new System.Collections.ObjectModel.ObservableCollection<Changeset>(),
                    SelectedProjectName = "Project1",
                    SourceBranch = "DeleteSourceBranch",
                    TargetBranch = "DeleteTargetBranch1"
                });
            });
        }

        private async Task TestLoadingFromContextOrFromConfigWithAssertionsAsync(Func<Task> setUpForContextOrConfig)
        {
            var (projectName, branch) = await TestLoadingFromContextOrFromConfigAsync(setUpForContextOrConfig);

            Assert.AreEqual(1, _sut.SourcesBranches.Count);
            Assert.AreEqual(2, _sut.TargetBranches.Count);
            Assert.AreEqual(projectName, _sut.SelectedProjectName);
            Assert.AreEqual(branch.Name, _sut.SelectedSourceBranch);
            Assert.AreEqual(branch.Branches[0], _sut.SelectedTargetBranch);
        }

        private async Task<(string projectnaam, Branch branch)> TestLoadingFromContextOrFromConfigAsync(Func<Task> setUpForContextOrConfig)
        {
            var projectName = "Project1";
            var branch = new Branch {Branches = new List<string> {"SourceBranch1", "SourceBranch2"}, Name = "Branch1"};
            var workspaceModel1 = new Workspace {Name = "Go test1", OwnerName = "14590"};

            _teamService.Expect(x => x.GetProjectNamesAsync())
                .Return(Task.FromResult<IEnumerable<string>>(new List<string> {"WrongProjectName", projectName}));

            _teamService.Expect(x => x.AllWorkspacesAsync())
                .Return(Task.FromResult<IEnumerable<Workspace>>(new List<Workspace> {workspaceModel1}));
            _teamService.Expect(x => x.CurrentWorkspace()).Return(null);

            _teamService.Expect(x => x.GetBranches(projectName)).Return(new List<Branch> {branch});

            _configManager.Expect(x => x.GetValue<bool>(ConfigKeys.SHOULD_SHOW_BUTTON_SWITCHING_SOURCE_TARGET_BRANCH))
                .Return(false);

            await setUpForContextOrConfig();

            return (projectName, branch);
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

        private async Task SetThingsBusyAndStuffAsync(Func<Task> task)
        {
            await task();
        }
    }
}