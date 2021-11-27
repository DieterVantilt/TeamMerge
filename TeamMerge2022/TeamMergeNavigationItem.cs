using LogicVS2022.Helpers;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Composition;
using TeamMergeBase;

namespace TeamMerge2022
{
    [TeamExplorerNavigationItem(Guids.TeamMergeNavigationItemId, 400, TargetPageId = Guids.TeamMergePageId)]
    public class TeamMergeNavigationItem
        : TeamExplorerNavigationItemBase
    {
        private readonly IServiceProvider _serviceProvider;

        [ImportingConstructor]
        public TeamMergeNavigationItem([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            Text = Resources.TeamMerge;
            Image = ResourceImage.TeamMerge;
        }

        public override void Execute()
        {
            TeamExplorerUtils.Instance.NavigateToPage(Guids.TeamMergePageId, _serviceProvider, null);
        }

        public override void Invalidate()
        {
            IsVisible = VersionControlHelper.IsConnectedToTfsCollectionAndProject(_serviceProvider);
        }
    }
}
