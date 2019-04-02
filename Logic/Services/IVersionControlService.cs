using Domain.Entities.TFVC;
using Domain.Entities.TFVC.Base;
using Domain.Entities.TFVC.Enums;
using System.Collections.Generic;

namespace Logic.Services
{
    public interface IVersionControlService
    {
        IEnumerable<ITFVCWorkspace> QueryWorkspaces(string workspaceName, string workspaceOwner, string computer);
        ITFVCWorkspace TryGetWorkspace(string localPath);
        IEnumerable<string> GetAllWorkItemTypes();
        ITFVCChangeset GetChangeset(int changesetId, bool includeChanges, bool includeDownloadInfo);
        IEnumerable<ITFVCMergeCandidate> GetMergeCandidates(string sourcePath, string targetPath, TFVCRecursionType recursion);
        ITFVCWorkspace GetWorkspace(string workspaceName, string workspaceOwner);
        IEnumerable<ITFVCBranchObject> QueryRootBranchObjects(TFVCRecursionType recursion);
        IEnumerable<ITFVCTeamProject> GetAllTeamProjects(bool refresh);
    }
}