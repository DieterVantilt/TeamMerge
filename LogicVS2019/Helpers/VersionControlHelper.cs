using Microsoft.TeamFoundation.Client;
using System;

namespace LogicVS2019.Helpers
{
    public static class VersionControlHelper
    {
        public static bool IsConnectedToTfsCollectionAndProject(IServiceProvider provider)
        {
            var context = GetTeamFoundationContext(provider);
            if (context != null)
            {
                return context.HasCollection && context.HasTeamProject
                    && context is TeamFoundationContext tfContext && tfContext.SourceControlType == TeamFoundationSourceControlType.TFVC;
            }

            return false;
        }

        public static ITeamFoundationContext GetTeamFoundationContext(IServiceProvider serviceProvider)
        {
            var tfContextManager = (ITeamFoundationContextManager)serviceProvider?.GetService(typeof(ITeamFoundationContextManager));

            return tfContextManager?.CurrentContext;
        }
    }
}