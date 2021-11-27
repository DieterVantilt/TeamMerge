using Domain.Entities;
using EnvDTE;
using Logic.Services;
using Shared.Utils;
using System;

namespace TeamMerge2022.Services
{
    public class SolutionService
        : SolutionServiceBase
    {
        private readonly IServiceProvider _serviceProvider;

        public SolutionService(IServiceProvider serviceProvider, IConfigManager configManager) 
            : base(configManager)
        {
            _serviceProvider = serviceProvider;
        }

        public override SolutionModel GetActiveSolution()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            var dte = (DTE)_serviceProvider.GetService(typeof(DTE));

            return dte?.Solution != null ? new SolutionModel(dte.Solution.FullName) : null;
        }
    }
}