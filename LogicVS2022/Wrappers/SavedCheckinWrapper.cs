using Domain.Entities.TFVC.Base;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace LogicVS2022.Wrappers
{
    public class SavedCheckinWrapper
        : ITFVCSavedCheckin
    {
        private readonly SavedCheckin _savedCheckin;

        public SavedCheckinWrapper(SavedCheckin savedCheckin)
        {
            _savedCheckin = savedCheckin;
        }

        public bool IsExcluded(string targetServerItem)
        {
            return _savedCheckin.IsExcluded(targetServerItem);
        }
    }
}