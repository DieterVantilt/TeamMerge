using Domain.Entities.TFVC;
using Domain.Entities.TFVC.Base;
using Domain.Entities.TFVC.Enums;
using LogicVS2022.Converters;
using Microsoft.TeamFoundation.VersionControl.Client;
using System.Collections.Generic;
using System.Linq;

namespace LogicVS2022.Wrappers
{
    public class WorkspaceWrapper
        : ITFVCWorkspace
    {
        public WorkspaceWrapper(Workspace workspace)
        {
            Workspace = workspace;
        }

        public string OwnerName => Workspace.OwnerName;
        public string Name => Workspace.Name;
        public ITFVCSavedCheckin LastSavedCheckin => new SavedCheckinWrapper(Workspace.LastSavedCheckin);

        public ITFVCGetStatus Get(TFVCGetRequest[] getRequests, TFVCGetOptions options)
        {
            return new GetStatusWrapper(Workspace.Get(getRequests.Select(x => x.Convert()).ToArray(), (GetOptions)(int)options));
        }

        public IEnumerable<ITFVCPendingChanges> GetPendingChanges()
        {
            return Workspace.GetPendingChanges().Select(x => new PendingChangesWrapper(x)).ToList();
        }

        public ITFVCGetStatus Merge(string sourcePath, string targetPath, TFVCChangesetVersionSpec versionFrom, TFVCChangesetVersionSpec versionTo, TFVCLockLevel lockLevel, TFVCRecursionType recursion, TFVCMergeOptions mergeOptions)
        {
            return new GetStatusWrapper(Workspace.Merge(sourcePath, targetPath, versionFrom.Convert(), versionTo.Convert(), (LockLevel)(int)lockLevel, (RecursionType)(int)recursion, (MergeOptions)(int)mergeOptions));
        }

        public bool MergeContent(ITFVCConflict tfvcConflict, bool useExternalMergeTool)
        {
            var conflictWrapper = (ConflictWrapper)tfvcConflict;

            return Workspace.MergeContent(conflictWrapper.Conflict, useExternalMergeTool);
        }

        public IEnumerable<ITFVCConflict> QueryConflicts(string[] pathFilters, bool recursive)
        {
            return Workspace.QueryConflicts(pathFilters, recursive).Select(x => new ConflictWrapper(x)).ToList();
        }

        public void ResolveConflict(ITFVCConflict conflict)
        {
            var conflictWrapper = (ConflictWrapper)conflict;

            Workspace.ResolveConflict(conflictWrapper.Conflict);
        }

        public Workspace Workspace { get; }
    }
}