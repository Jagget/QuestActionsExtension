using System;
using System.Collections.Generic;
using System.Linq;
using DaggerfallConnect.FallExe;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Utility;
using UnityEngine;

namespace Game.Mods.QuestActionsExtension
{
    public static class Helpers
    {
        // Enchantment point/gold value data for item powers
        private static readonly int[] _extraSpellPtsEnchantPts = { 0x1F4, 0x1F4, 0x1F4, 0x1F4, 0xC8, 0xC8, 0xC8, 0x2BC, 0x320, 0x384, 0x3E8 };
        private static readonly int[] _potentVsEnchantPts = { 0x320, 0x384, 0x3E8, 0x4B0 };
        private static readonly int[] _regensHealthEnchantPts = { 0x0FA0, 0x0BB8, 0x0BB8 };
        private static readonly int[] _vampiricEffectEnchantPts = { 0x7D0, 0x3E8 };
        private static readonly int[] _increasedWeightAllowanceEnchantPts = { 0x190, 0x258 };
        private static readonly int[] _improvesTalentsEnchantPts = { 0x1F4, 0x258, 0x258 };
        private static readonly int[] _goodRepWithEnchantPts = { 0x3E8, 0x3E8, 0x3E8, 0x3E8, 0x3E8, 0x1388 };

        private static readonly int[][] _enchantmentPtsForItemPowerArrays =
        {
            null, null, null, _extraSpellPtsEnchantPts, _potentVsEnchantPts, _regensHealthEnchantPts,
            _vampiricEffectEnchantPts, _increasedWeightAllowanceEnchantPts, null, null, null, null, null,
            _improvesTalentsEnchantPts, _goodRepWithEnchantPts
        };

        private static readonly ushort[] _enchantmentPointCostsForNonParamTypes = { 0, 0x0F448, 0x0F63C, 0x0FF9C, 0x0FD44, 0, 0, 0, 0x384, 0x5DC, 0x384, 0x64, 0x2BC };

        private static int GetValueOfOneEnchantment(DaggerfallEnchantment enchantment)
        {
            switch (enchantment.type)
            {
                case EnchantmentTypes.CastWhenUsed:
                case EnchantmentTypes.CastWhenHeld:
                case EnchantmentTypes.CastWhenStrikes:
                    // Enchantments that cast a spell. The parameter is the spell index in SPELLS.STD.
                    return FormulaHelper.GetSpellEnchantPtCost(enchantment.param);
                case EnchantmentTypes.RepairsObjects:
                case EnchantmentTypes.AbsorbsSpells:
                case EnchantmentTypes.EnhancesSkill:
                case EnchantmentTypes.FeatherWeight:
                case EnchantmentTypes.StrengthensArmor:
                    // Enchantments that provide an effect that has no parameters
                    return _enchantmentPointCostsForNonParamTypes[(int)enchantment.type];
                case EnchantmentTypes.SoulBound:
                    // Bound soul
                    MobileEnemy mobileEnemy = GameObjectHelper.EnemyDict[enchantment.param];
                    return mobileEnemy.SoulPts; // TODO: Not sure about this. Should be negative? Needs to be tested.
                default:
                    // Enchantments that provide a non-spell effect with a parameter (parameter = when effect applies, what enemies are affected, etc.)
                    return _enchantmentPtsForItemPowerArrays[(int)enchantment.type][enchantment.param];
            }
        }

        private static int GetValueOfMagicItem(IEnumerable<DaggerfallEnchantment> legacyMagic)
        {
            return legacyMagic
                .Where(enchantment => enchantment.type != EnchantmentTypes.None && enchantment.type < EnchantmentTypes.ItemDeteriorates)
                .Sum(GetValueOfOneEnchantment);
        }

        public static void ApplyMagicTemplate(DaggerfallUnityItem item, int templateIndex)
        {
            MagicItemTemplate magicItem;
            try
            {
                magicItem = DaggerfallUnity.Instance.ItemHelper.MagicItemTemplates.First(t => t.index == templateIndex);
            }
            catch
            {
                throw new Exception($"Item index {templateIndex} does not exist!");
            }

            if (magicItem.type != MagicItemTypes.RegularMagicItem)
            {
                throw new Exception($"Item index {templateIndex} is not a RegularMagicItem!");
            }

            // Replace the regular item name with the magic item name
            item.shortName = TextManager.Instance.GetLocalizedMagicItemName((int)magicItem.index, magicItem.name);

            // Add the enchantments
            item.legacyMagic = magicItem.enchantments.Where(e => e.type != EnchantmentTypes.None).ToArray();

            // Set the condition/magic uses
            item.maxCondition = magicItem.uses;
            item.currentCondition = magicItem.uses;

            // Set the value of the item. This is determined by the enchantment point cost/spell-casting cost
            // of the enchantments on the item.
            var newValue = GetValueOfMagicItem(item.legacyMagic);
            if (newValue > 0)
            {
                item.value = newValue;
            }
        }

        public static void AddEnchantmentToItem(DaggerfallUnityItem item, string enchantmentType, short enchantmentParam)
        {
            var overrideValue = false;
            if (item.legacyMagic == null)
            {
                item.legacyMagic = new DaggerfallEnchantment[] { };
                overrideValue = true;
            }

            // Check if the length is no more than 10
            if (item.legacyMagic.Length >= 10)
            {
                throw new Exception("Can't apply more than 10 enchantments");
            }

            if (!Enum.IsDefined(typeof(EnchantmentTypes), enchantmentType))
            {
                throw new Exception($"Enchantment type {enchantmentType} doesn't exist");
            }

            EnchantmentTypes type = (EnchantmentTypes)Enum.Parse(typeof(EnchantmentTypes), enchantmentType);

            var enchantment = new DaggerfallEnchantment()
            {
                type = type,
                param = enchantmentParam,
            };

            // Add enchantment to a list 
            var legacyMagic = item.legacyMagic.Where(lm => lm.type != EnchantmentTypes.None).ToList();

            legacyMagic.Add(enchantment);

            item.legacyMagic = legacyMagic.ToArray();

            // Set the condition/magic uses
            // And yes, it's always 1500
            item.maxCondition = 1500;
            item.currentCondition = 1500;

            var enchValue = 0;
            try
            {
                enchValue = GetValueOfOneEnchantment(enchantment);
            }
            finally
            {
            }

            // Set the value of the item. This is determined by the enchantment point cost/spell-casting cost
            // of the enchantments on the item.
            if (overrideValue)
            {
                item.value = enchValue;
            }
            else
            {
                item.value += enchValue;
            }

            Debug.Log($"Enchantment type {enchantmentType} added");
        }

        // TODO: Switch to Place.IsPlayerAtDungeonType(), when https://github.com/Interkarma/daggerfall-unity/pull/2614 released!
        public static bool IsPlayerAtDungeonType(int p2)
        {
            // Get component handling player world status and transitions
            var playerEnterExit = GameManager.Instance.PlayerEnterExit;
            if (!playerEnterExit)
                return false;

            // Only dungeons
            if (!playerEnterExit.IsPlayerInsideDungeon)
                return false;

            // Any dungeon will do
            if (p2 == -1)
                return true;

            return p2 == (int)GameManager.Instance.PlayerEnterExit.Dungeon.Summary.DungeonType;
        }

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