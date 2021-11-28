using Domain.Entities.TFVC.Enums;
using System.Collections.Generic;

namespace Domain.Entities.TFVC.Base
{
    public interface ITFVCWorkspace
    {        
        bool Exists { get; }
        string OwnerName { get; }
        string Name { get; }
        ITFVCSavedCheckin LastSavedCheckin { get; }
        IEnumerable<ITFVCPendingChanges> GetPendingChanges();
        IEnumerable<ITFVCConflict> QueryConflicts(string[] pathFilters, bool recursive);
        bool MergeContent(ITFVCConflict conflict, bool useExternalMergeTool);
        void ResolveConflict(ITFVCConflict conflict);
        ITFVCGetStatus Get(TFVCGetRequest[] getRequests, TFVCGetOptions options);
        ITFVCGetStatus Merge(string sourcePath, string targetPath, TFVCChangesetVersionSpec versionFrom, TFVCChangesetVersionSpec versionTo, TFVCLockLevel lockLevel, TFVCRecursionType recursion, TFVCMergeOptions mergeOptions);
    }
}