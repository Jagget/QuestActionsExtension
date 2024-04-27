using System;
using System.Text.RegularExpressions;
using DaggerfallConnect.FallExe;
using DaggerfallWorkshop.Game.Questing;

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

        public override string Pattern => @"update-quest-item (?<anItem>[a-zA-Z0-9_.]+) add-enchantment type (?<enchantmentType>\w+) spell (?<spellId>\d+)";

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;


            if (string.IsNullOrEmpty(match.Groups["anItem"].Value))
            {
                SetComplete();
                return null;
            }


            if (string.IsNullOrEmpty(match.Groups["enchantmentType"].Value))
            {
                SetComplete();
                return null;
            }


            if (string.IsNullOrEmpty(match.Groups["spellId"].Value))
            {
                SetComplete();
                return null;
            }

            var enchantmentType = match.Groups["enchantmentType"].Value;

            var enchantmentExist = Enum.IsDefined(typeof(EnchantmentTypes), enchantmentType);

            if (enchantmentExist)
            {
                return new UpdateQuestItemAddEnchantment(parentQuest)
                {
                    _itemSymbol = new Symbol(match.Groups["anItem"].Value),
                    _enchantmentType = enchantmentType,
                    _spellId = (short)Parser.ParseInt(match.Groups["spellId"].Value),
                };
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