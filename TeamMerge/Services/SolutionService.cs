using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using TeamMerge.Services.Models;
using TeamMerge.Utils;

namespace TeamMerge.Services
{
    public interface ISolutionService
    {
        SolutionModel GetActiveSolution();
        DefaultMergeSettings GetDefaultMergeSettingsForCurrentSolution();
    }

    public class SolutionService : ISolutionService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfigHelper _configHelper;

        public SolutionService(IServiceProvider serviceProvider, IConfigHelper configHelper)
        {
            _serviceProvider = serviceProvider;
            _configHelper = configHelper;
        }

        public SolutionModel GetActiveSolution()
        {
            var dte = (DTE)_serviceProvider.GetService(typeof(DTE));

            return dte.Solution != null ? new SolutionModel(dte.Solution.FullName) : null;
        }

        public DefaultMergeSettings GetDefaultMergeSettingsForCurrentSolution()
        {
            DefaultMergeSettings settings = null;

            var solution = GetActiveSolution();

            if (!string.IsNullOrWhiteSpace(solution?.FullName))
            {
                var defaultMergeSettings = _configHelper.GetValue<List<DefaultMergeSettings>>(ConfigKeys.SOLUTIONWISE_SELECTEDMERGE_SETTINGS) ?? new List<DefaultMergeSettings>();
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
    }
}
