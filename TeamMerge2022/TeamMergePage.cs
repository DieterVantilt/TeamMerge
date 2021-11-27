using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;
using TeamMergeBase;

namespace TeamMerge2022
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