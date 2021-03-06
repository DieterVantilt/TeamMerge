﻿extern alias VS2017;
using Shared.Utils;
using System;
using System.Threading.Tasks;
using TeamMerge.Exceptions;
using VS2017::Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;

namespace TeamMerge.Base
{
    public abstract class TeamExplorerViewModelBase
        : TeamExplorerSectionViewModelBase
    {
        private readonly ILogger _logger;

        protected TeamExplorerViewModelBase(ILogger logger)
        {
            _logger = logger;
        }

        protected async Task SetBusyWhileExecutingAsync(Func<Task> task)
        {
            ShowBusy();

            try
            {
                await task();
            }
            catch (MergeActionException mergeActionEx)
            {
                ShowMessage(mergeActionEx.Message);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
                _logger.LogException(ex);
            }

            HideBusy();
        }
    }
}