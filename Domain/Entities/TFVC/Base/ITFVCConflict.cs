using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.TFVC.Base
{
    public interface ITFVCConflict
    {
        TFVCConflictResolution Resolution { get; set; }
    }

    public enum TFVCConflictResolution
    {
        None = 0,
        AcceptMerge = 1,
        AcceptYours = 2,
        AcceptTheirs = 3,
        DeleteConflict = 4,
        AcceptYoursRenameTheirs = 5,
        OverwriteLocal = 6
    }
}
