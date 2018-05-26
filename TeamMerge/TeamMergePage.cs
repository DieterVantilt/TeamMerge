using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamMerge
{
    [TeamExplorerPage(Guids.TeamMergePageId)]
    public class TeamMergePage : TeamExplorerPageBase
    {
        public TeamMergePage()
        {
            Title = "Grote titel0";
        }
    }
}
