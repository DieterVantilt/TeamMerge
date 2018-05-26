using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Composition;
using TeamMerge.Helpers;

namespace TeamMerge
{
    [TeamExplorerNavigationItem(Guids.TeamMergeNavigationItemId, 210, TargetPageId = Guids.TeamMergePageId)]
    public class TeamMergeNavigationItem 
        : TeamExplorerNavigationItemBase
    {
        private readonly IServiceProvider _serviceProvider;

        [ImportingConstructor]
        public TeamMergeNavigationItem([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            
            Text = Resources.TeamMerge;
            Image = Resources.Command1;
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