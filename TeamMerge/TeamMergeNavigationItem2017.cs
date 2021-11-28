extern alias VS2017;

using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Composition;
using TeamMergeBase;
using VS2017::LogicVS2017.Helpers;
using VS2017::Microsoft.TeamFoundation.Controls;
using VS2017::Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;

namespace TeamMerge
{
    [TeamExplorerNavigationItem(Guids.VS2017TeamMergeNavigationItemId, 400, TargetPageId = Guids.VS2017TeamMergePageId)]
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
            TeamExplorerUtils.Instance.NavigateToPage(Guids.VS2017TeamMergePageId.ToString(), _serviceProvider, null);
        }

        public override void Invalidate()
        {
            IsVisible = VersionControlHelper.IsConnectedToTfsCollectionAndProject(_serviceProvider);
        }
    }
}