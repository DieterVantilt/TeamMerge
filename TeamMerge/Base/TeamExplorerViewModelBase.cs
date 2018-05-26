using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;
using System;
using System.Threading.Tasks;

namespace TeamMerge.Base
{
    public abstract class TeamExplorerViewModelBase
        : TeamExplorerSectionViewModelBase
    {
        protected async Task SetBusyWhileExecutingAsync(Func<Task> task)
        {
            ShowBusy();

            try
            {
                await task();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }

            HideBusy();
        }
    }
}