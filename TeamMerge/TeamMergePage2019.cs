extern alias VS2019;
using TeamMergeBase;
using VS2019::Microsoft.TeamFoundation.Controls;
using VS2019::Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;

namespace TeamMerge
{
    [TeamExplorerPage(Guids.VS2019TeamMergePageId)]
    public class TeamMergePage2019 
        : TeamExplorerPageBase
    {
        public TeamMergePage2019()
        {
            Title = Resources.TeamMerge;            
        }
    }
}