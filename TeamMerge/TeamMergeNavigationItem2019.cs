extern alias VS2019;

using LogicVS2019.Helpers;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Composition;
using VS2019::Microsoft.TeamFoundation.Controls;
using VS2019::Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;

namespace TeamMerge
{
    [TeamExplorerNavigationItem(Guids.VS2019TeamMergeNavigationItemId, 400, TargetPageId = Guids.VS2019TeamMergePageId)]
    public class TeamMergeNavigationItem2019
        : TeamExplorerNavigationItemBase
    {
        private readonly IServiceProvider _serviceProvider;

        [ImportingConstructor]
        public TeamMergeNavigationItem2019([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            
            Text = Resources.TeamMerge;
            Image = ResourceImage.TeamMerge;   
        }

        public override void Execute()
        {
            TeamExplorerUtils.Instance.NavigateToPage(Guids.VS2019TeamMergePageId.ToString(), _serviceProvider, null);
        }

        public override void Invalidate()
        {
            IsVisible = VersionControlHelper.IsConnectedToTfsCollectionAndProject(_serviceProvider);
        }
    }
}