using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.TFVC.Base
{
    public interface ITFVCMergeCandidate
    {
        ITFVCChangeset Changeset { get; }
    }
}
