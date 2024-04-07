using System.Collections.Generic;
using System.Linq;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.Items;

namespace Game.Mods.QuestActionsExtension
{
    public static class Helpers
    {
        public static int CalculateLengthOrStackCount(IReadOnlyCollection<DaggerfallUnityItem> itemList)
        {
            if (itemList == null) return 0;
            var count = 0;

            foreach (var item in itemList.Where(item => !item.IsSummoned))
            {
                count += item.IsAStack() ? item.stackCount : 1;
            }

            return count;
        }

        public static void RemoveNFromList(ItemCollection storage, List<DaggerfallUnityItem> itemList, int amount)
        {
            var itemsRemoved = 0;

            foreach (var item in itemList)
            {
                if (itemsRemoved >= amount)
                    break;

                if (item.IsAStack())
                {
                    var stackCount = item.stackCount;
                    if (stackCount <= amount - itemsRemoved)
                    {
                        // Remove the entire stack
                        storage.RemoveItem(item);
                        itemsRemoved += stackCount;
                    }
                    else
                    {
                        // Remove part of the stack
                        item.stackCount -= (amount - itemsRemoved);
                        itemsRemoved = amount;
                    }
                }
                else
                {
                    // Remove single item
                    storage.RemoveItem(item);
                    itemsRemoved++;
                }
            }
        }

        public static bool Contains(IEnumerable<DaggerfallUnityItem> items, ItemGroups itemGroup, int itemIndex)
        {
            var groupIndex = DaggerfallUnity.Instance.ItemHelper.GetGroupIndex(itemGroup, itemIndex);
            return items.Where(item => item != null).Any(item => item.ItemGroup == itemGroup && item.GroupIndex == groupIndex);
        }
    }
}