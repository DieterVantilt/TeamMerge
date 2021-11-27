using Domain.Entities.TFVC.Base;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace LogicVS2022.Wrappers
{
    public class ConflictWrapper
        : ITFVCConflict
    {
        public ConflictWrapper(Conflict conflict)
        {
            Conflict = conflict;
        }

        public TFVCConflictResolution Resolution
        {
            get { return (TFVCConflictResolution)(int)Conflict.Resolution; }
            set { Conflict.Resolution = (Resolution)(int)value; }
        }

        public Conflict Conflict { get; }
    }
}