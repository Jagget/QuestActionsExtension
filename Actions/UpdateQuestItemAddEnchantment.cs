using System;
using System.Text.RegularExpressions;
using DaggerfallConnect.FallExe;
using DaggerfallWorkshop.Game.Questing;
using UnityEngine;

namespace Game.Mods.QuestActionsExtension.Actions
{
    public class UpdateQuestItemAddEnchantment : ActionTemplate
    {
        private Symbol _itemSymbol;
        private string _enchantmentType;
        private short _spellId;

        public UpdateQuestItemAddEnchantment(Quest parentQuest) : base(parentQuest)
        {
        }

        public override string Pattern => @"update-quest-item (?<anItem>[a-zA-Z0-9_.]+) add-enchantment type (?<enchantmentType>\w+) spell (?<spellId>[+-]?\d+)|" +
                                          @"update-quest-item (?<anItem>[a-zA-Z0-9_.]+) add-enchantment type (?<enchantmentType>\w+)";

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;


            if (string.IsNullOrEmpty(match.Groups["anItem"].Value))
            {
                Debug.LogError("Parameter 'Item' is missing.");
                SetComplete();
                return null;
            }


            if (string.IsNullOrEmpty(match.Groups["enchantmentType"].Value))
            {
                Debug.LogError("Parameter 'enchantmentType' is missing.");
                SetComplete();
                return null;
            }

            short spellId = -1;

            if (!string.IsNullOrEmpty(match.Groups["spellId"].Value))
            {
                spellId = (short)Parser.ParseInt(match.Groups["spellId"].Value);
            }

            var enchantmentType = match.Groups["enchantmentType"].Value;

            if (Enum.IsDefined(typeof(EnchantmentTypes), enchantmentType))
            {
                if ((int)Enum.Parse(typeof(EnchantmentTypes), enchantmentType) > 15)
                {
                    Debug.LogErrorFormat("Enchantment type `{0}` is not supported.", enchantmentType);
                    SetComplete();
                    return null;
                }

                return new UpdateQuestItemAddEnchantment(parentQuest)
                {
                    _itemSymbol = new Symbol(match.Groups["anItem"].Value),
                    _enchantmentType = enchantmentType,
                    _spellId = spellId,
                };
            }
            else
            {
                Debug.LogErrorFormat("Enchantment type `{0}` doesn't exist.", enchantmentType);
            }

            SetComplete();
            return null;
        }

        public override void Update(Task _)
        {
            // Get related Item resource
            Item item = ParentQuest.GetItem(_itemSymbol);
            if (item == null)
            {
                // Stop if Item does not exist
                SetComplete();
                return;
            }

            Helpers.AddEnchantmentToItem(item.DaggerfallUnityItem, _enchantmentType, _spellId);
            SetComplete();
        }

        public override object GetSaveData()
        {
            return new SaveDataV1
            {
                ItemSymbol = _itemSymbol,
                EnchantmentType = _enchantmentType,
                SpellId = _spellId,
            };
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null) return;

            var data = (SaveDataV1)dataIn;

            _itemSymbol = data.ItemSymbol;
            _enchantmentType = data.EnchantmentType;
            _spellId = data.SpellId;
        }

        [FullSerializer.fsObject("v1")]
        public struct SaveDataV1
        {
            public Symbol ItemSymbol;
            public string EnchantmentType;
            public short SpellId;
        }
    }
}