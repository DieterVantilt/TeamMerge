using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;

namespace TeamMerge
{
    [TeamExplorerPage(Guids.TeamMergePageId)]
    public class TeamMergePage 
        : TeamExplorerPageBase
    {
        public TeamMergePage()
        {
            Title = Resources.TeamMerge;            
        }
    }
}