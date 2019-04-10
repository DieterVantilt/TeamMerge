extern alias VS2017;

using VS2017::Microsoft.TeamFoundation.Controls;
using VS2017::Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;

namespace TeamMerge
{
    [TeamExplorerPage(Guids.VS2017TeamMergePageId)]
    public class TeamMergePage2017
        : TeamExplorerPageBase
    {
        public TeamMergePage2017()
        {
            Title = Resources.TeamMerge;
        }
    }
}