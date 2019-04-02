using Domain.Entities.TFVC.Base;
using Domain.Entities.TFVCBase;
using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicVS2017.Wrappers
{
    public class ChangesetWrapper
        : ITFVCChangeset
    {
        private readonly Changeset _changeset;

        public ChangesetWrapper(Changeset changeset)
        {
            _changeset = changeset;
        }

        public int ChangesetId => _changeset.ChangesetId;

        public string Comment => _changeset.Comment;

        public DateTime CreationDate => _changeset.CreationDate;

        public string OwnerDisplayName => _changeset.OwnerDisplayName;

        public IEnumerable<TFVCAssociatedWorkItem> GetAssociatedWorkItems()
        {
            return _changeset.AssociatedWorkItems.Select(x => new TFVCAssociatedWorkItem(x.WorkItemType, x.Id)).ToList();
        }
    }
}