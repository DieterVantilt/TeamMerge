using Domain.Entities.TFVC.Base;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace LogicVS2022.Wrappers
{
    public class ItemIdentifierWrapper
        : ITFVCItemIdentifier
    {
        private readonly ItemIdentifier _itemIdentifier;

        public ItemIdentifierWrapper(ItemIdentifier itemIdentifier)
        {
            _itemIdentifier = itemIdentifier;
        }

        public string Item => _itemIdentifier.Item;

        public bool IsDeleted => _itemIdentifier.IsDeleted;
    }
}