using Domain.Entities.TFVC.Base;
using LogicVS2019.Wrappers;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace LogicVS2019.Wrappers
{
    public class MergeCandidateWrapper
        : ITFVCMergeCandidate
    {
        private readonly MergeCandidate _mergeCandidate;

        private readonly ChangesetWrapper _changeset;

        public MergeCandidateWrapper(MergeCandidate mergeCandidate)
        {
            _mergeCandidate = mergeCandidate;

            _changeset = new ChangesetWrapper(_mergeCandidate.Changeset);
        }

        public ITFVCChangeset Changeset => _changeset;
    }
}