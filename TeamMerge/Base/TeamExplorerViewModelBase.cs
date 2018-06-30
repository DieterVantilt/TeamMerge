using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;
using System;
using System.Threading.Tasks;
using TeamMerge.Exceptions;

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
            catch(MergeActionException mergeActionEx)
            {
                ShowMessage(mergeActionEx.Message);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }

            HideBusy();
        }
    }
}