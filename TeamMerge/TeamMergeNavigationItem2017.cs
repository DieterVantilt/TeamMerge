extern alias VS2017;

using LogicVS2017.Helpers;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Composition;
using VS2017::Microsoft.TeamFoundation.Controls;
using VS2017::Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;

namespace TeamMerge
{
    [TeamExplorerNavigationItem(Guids.TeamMergeNavigationItemId, 400, TargetPageId = Guids.TeamMergePageId)]
    public class TeamMergeNavigationItem2017 
        : TeamExplorerNavigationItemBase
    {
        private readonly IServiceProvider _serviceProvider;

        [ImportingConstructor]
        public TeamMergeNavigationItem2017([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            
            Text = Resources.TeamMerge;
            Image = ResourceImage.TeamMerge;   
        }

        public override void Execute()
        {
            TeamExplorerUtils.Instance.NavigateToPage(Guids.TeamMergePageId.ToString(), _serviceProvider, null);
        }

        public override void Invalidate()
        {
            IsVisible = VersionControlHelper.IsConnectedToTfsCollectionAndProject(_serviceProvider);
        }
    }
}