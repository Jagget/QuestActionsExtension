using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Questing;

namespace Game.Mods.QuestActionsExtension.Actions
{
    public class PlayerPossesses : ActionTemplate
    {
        private int _numberOfItems;
        private int _itemClass;
        private int _itemSubClass;

        public PlayerPossesses(Quest parentQuest) : base(parentQuest)
        {
            IsTriggerCondition = true;
            IsAlwaysOnTriggerCondition = true;
        }

        public override string Pattern => @"player possesses (?<numberOfItems>\d+) items class (?<itemClass>\d+) subclass (?<itemSubClass>\d+)";

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            Match match = Test(source);
            if (!match.Success)
                return null;

            return new PlayerPossesses(parentQuest)
            {
                _numberOfItems = Parser.ParseInt(match.Groups["numberOfItems"].Value),
                _itemClass = Parser.ParseInt(match.Groups["itemClass"].Value),
                _itemSubClass = Parser.ParseInt(match.Groups["itemSubClass"].Value),
            };
        }

        public override bool CheckTrigger(Task _)
        {
            PlayerEntity player = GameManager.Instance.PlayerEntity;

            var total = 0;

            if (player.Items.Contains((ItemGroups)_itemClass, _itemSubClass))
            {
                var items = player.Items.SearchItems((ItemGroups)_itemClass, _itemSubClass).FindAll(item => !item.IsQuestItem);
                total += Helpers.CalculateLengthOrStackCount(items);
            }

            if (player.WagonItems.Contains((ItemGroups)_itemClass, _itemSubClass))
            {
                var items = player.WagonItems.SearchItems((ItemGroups)_itemClass, _itemSubClass).FindAll(item => !item.IsQuestItem);
                total += Helpers.CalculateLengthOrStackCount(items);
            }

            return total >= _numberOfItems;
        }

        public override object GetSaveData()
        {
            return new SaveData_v1()
            {
                numberOfItems = _numberOfItems,
                itemClass = _itemClass,
                itemSubClass = _itemSubClass,
            };
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;
            var data = (SaveData_v1)dataIn;

            _numberOfItems = data.numberOfItems;
            _itemClass = data.itemClass;
            _itemSubClass = data.itemSubClass;
        }

        [FullSerializer.fsObject("v1")]
        public struct SaveData_v1
        {
            public int numberOfItems;
            public int itemClass;
            public int itemSubClass;
        }
    }
}