using System.Collections.Generic;
using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Questing;
using UnityEngine;

namespace Game.Mods.QuestActionsExtension.Actions
{
    public class PlayerHandsover : ActionTemplate
    {
        private int _numberOfItems;
        private int _itemClass;
        private int _itemSubClass;

        public PlayerHandsover(Quest parentQuest) : base(parentQuest)
        {
        }

        public override string Pattern => @"player handsover (?<numberOfItems>\d+) items class (?<itemClass>\d+) subclass (?<itemSubClass>\d+)";

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            Match match = Test(source);
            if (!match.Success)
                return null;

            return new PlayerHandsover(parentQuest)
            {
                _numberOfItems = Parser.ParseInt(match.Groups["numberOfItems"].Value),
                _itemClass = Parser.ParseInt(match.Groups["itemClass"].Value),
                _itemSubClass = Parser.ParseInt(match.Groups["itemSubClass"].Value),
            };
        }

        public override void Update(Task _)
        {
            PlayerEntity player = GameManager.Instance.PlayerEntity;

            var inventoryItems = 0;
            var wagonItems = 0;
            List<DaggerfallUnityItem> Items = null;
            List<DaggerfallUnityItem> WagonItems = null;

            if (player.Items.Contains((ItemGroups)_itemClass, _itemSubClass))
            {
                Items = player.Items.SearchItems((ItemGroups)_itemClass, _itemSubClass);
                inventoryItems += Helpers.CalculateLengthOrStackCount(Items);
            }

            if (player.WagonItems.Contains((ItemGroups)_itemClass, _itemSubClass))
            {
                WagonItems = player.Items.SearchItems((ItemGroups)_itemClass, _itemSubClass);
                wagonItems += Helpers.CalculateLengthOrStackCount(WagonItems);
            }

            if (inventoryItems + wagonItems < _numberOfItems) return;

            var removeFromInventory = Mathf.Min(_numberOfItems, inventoryItems);
            var removeFromWagon = _numberOfItems - removeFromInventory;

            if (removeFromInventory > 0)
            {
                Helpers.RemoveNFromList(player.Items, Items, removeFromInventory);
            }

            if (removeFromWagon > 0)
            {
                Helpers.RemoveNFromList(player.WagonItems, WagonItems, removeFromWagon);
            }

            SetComplete();
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