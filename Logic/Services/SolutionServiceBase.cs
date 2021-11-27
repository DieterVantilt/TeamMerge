using Domain.Entities;
using Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic.Services
{
    public interface ISolutionService
    {
        SolutionModel GetActiveSolution();
        DefaultMergeSettings GetDefaultMergeSettingsForCurrentSolution();
        void SaveDefaultMergeSettingsForCurrentSolution(DefaultMergeSettings defaultMergeSettings);
    }

    public abstract class SolutionServiceBase
        : ISolutionService
    {
        private readonly IConfigManager _configManager;

        protected SolutionServiceBase(IConfigManager configManager)
        {
            _configManager = configManager;
        }

        public abstract SolutionModel GetActiveSolution();
        
            //Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            //var dte = (DTE)_serviceProvider.GetService(typeof(DTE));

            //return dte.Solution != null ? new SolutionModel(dte.Solution.FullName) : null;
        

        public DefaultMergeSettings GetDefaultMergeSettingsForCurrentSolution()
        {
            DefaultMergeSettings settings = null;

            var solution = GetActiveSolution();

            if (!string.IsNullOrWhiteSpace(solution?.FullName))
            {
                var defaultMergeSettings = _configManager.GetValue<List<DefaultMergeSettings>>(ConfigKeys.SOLUTIONWIDE_SELECTEDMERGE_SETTINGS) ?? new List<DefaultMergeSettings>();
                if (defaultMergeSettings.Any())
                {
                    var currentSolutionInCache = defaultMergeSettings.SingleOrDefault(m => m.Solution == solution.FullName);
                    if (currentSolutionInCache != null)
                    {
                        settings = currentSolutionInCache;
                    }
                }
            }

            return settings;
        }

        public void SaveDefaultMergeSettingsForCurrentSolution(DefaultMergeSettings defaultMergeSettings)
        {
            var saveSelectedBranchSettingsBySolution = _configManager.GetValue<bool>(ConfigKeys.SAVE_BRANCH_PERSOLUTION);
            if (saveSelectedBranchSettingsBySolution)
            {
                var currentSolutionName = GetActiveSolution()?.FullName;
                defaultMergeSettings.Solution = currentSolutionName;

                if (!string.IsNullOrWhiteSpace(currentSolutionName))
                {
                    var currentSettings = _configManager.GetValue<List<DefaultMergeSettings>>(ConfigKeys.SOLUTIONWIDE_SELECTEDMERGE_SETTINGS) ?? new List<DefaultMergeSettings>();
                    var currentSolutionSetting = currentSettings.SingleOrDefault(c => c.Solution == currentSolutionName);
                    if (currentSolutionSetting != null)
                    {
                        currentSolutionSetting.SourceBranch = defaultMergeSettings.SourceBranch;
                        currentSolutionSetting.TargetBranch = defaultMergeSettings.TargetBranch;
                        currentSolutionSetting.ProjectName = defaultMergeSettings.ProjectName;
                    }
                    else
                    {
                        currentSettings.Add(defaultMergeSettings);
                    }

                    _configManager.AddValue(ConfigKeys.SOLUTIONWIDE_SELECTEDMERGE_SETTINGS, currentSettings);
                    _configManager.SaveDictionary();
                }
            }
        }
    }
}
