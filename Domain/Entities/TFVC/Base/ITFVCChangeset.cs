using Domain.Entities.TFVCBase;
using System;
using System.Collections.Generic;

namespace Domain.Entities.TFVC.Base
{
    public interface ITFVCChangeset
    {
        IEnumerable<TFVCAssociatedWorkItem> GetAssociatedWorkItems();
        int ChangesetId { get; }
        string Comment { get; }
        DateTime CreationDate { get; }
        string OwnerDisplayName { get; }
    }
}