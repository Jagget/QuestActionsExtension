using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Questing;

namespace Game.Mods.QuestActionsExtension.Actions
{
    public class EquippedWithItem : ActionTemplate
    {
        private int _itemClass;
        private int _itemSubClass;

        public EquippedWithItem(Quest parentQuest) : base(parentQuest)
        {
            IsTriggerCondition = true;
            IsAlwaysOnTriggerCondition = true;
        }

        public override string Pattern => @"player equipped with item class (?<itemClass>\d+) subclass (?<itemSubClass>\d+)";

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            Match match = Test(source);
            if (!match.Success)
                return null;

            return new EquippedWithItem(parentQuest)
            {
                _itemClass = Parser.ParseInt(match.Groups["itemClass"].Value),
                _itemSubClass = Parser.ParseInt(match.Groups["itemSubClass"].Value),
            };
        }

        public override bool CheckTrigger(Task _)
        {
            return Helpers.Contains(GameManager.Instance.PlayerEntity.ItemEquipTable.EquipTable, (ItemGroups)_itemClass, _itemSubClass);
        }

        public override object GetSaveData()
        {
            return new SaveData_v1()
            {
                itemClass = _itemClass,
                itemSubClass = _itemSubClass,
            };
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;
            var data = (SaveData_v1)dataIn;

            _itemClass = data.itemClass;
            _itemSubClass = data.itemSubClass;
        }

        [FullSerializer.fsObject("v1")]
        public struct SaveData_v1
        {
            public int itemClass;
            public int itemSubClass;
        }
    }
}