﻿using Microsoft.TeamFoundation.Client;
using System;

namespace TeamMerge.Helpers
{
    public static class VersionControlHelper
    {
        public static bool IsConnectedToTfsCollectionAndProject(IServiceProvider provider)
        {
            var context = GetTeamFoundationContext(provider);
            if (context != null)
            {
                return context.HasCollection && context.HasTeamProject;
            }

            return false;
        }

        public static ITeamFoundationContext GetTeamFoundationContext(IServiceProvider serviceProvider)
        {
            if (serviceProvider != null)
            {
                var tfContextManager = (ITeamFoundationContextManager) serviceProvider.GetService(typeof(ITeamFoundationContextManager));

                return tfContextManager?.CurrentContext;
            }

            return null;
        }
    }
}