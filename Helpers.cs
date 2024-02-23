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

            foreach (var item in itemList.Where(item => !item.IsSummoned && item.IsAStack()))
            {
                return item.stackCount;
            }

            return itemList.Count;
        }

        public static void RemoveNFromList(ItemCollection storage, List<DaggerfallUnityItem> itemList, int amount)
        {
            var isAStack = itemList[0].IsAStack();

            if (isAStack)
            {
                itemList[0].stackCount -= amount;

                if (itemList[0].stackCount == 0)
                {
                    storage.RemoveItem(itemList[0]);
                }
            }
            else
            {
                for (var i = 0; i < amount; i++)
                {
                    storage.RemoveItem(itemList[i]);
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