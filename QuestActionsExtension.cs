using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using UnityEngine;

namespace Game.Mods.QuestActionsExtension
{
    public class QuestActionsExtension : MonoBehaviour
    {
        private static Mod _mod;

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            _mod = initParams.Mod;

            var go = new GameObject(_mod.Title);
            go.AddComponent<QuestActionsExtension>();

            QuestMachine questMachine = GameManager.Instance.QuestMachine;
            questMachine.RegisterAction(new ReducePlayerHealth(null));
            questMachine.RegisterAction(new ReducePlayerFatigue(null));
            questMachine.RegisterAction(new ReducePlayerMagicka(null));
            questMachine.RegisterAction(new WithinUnits(null));
            questMachine.RegisterAction(new PlayerPossesses(null));
            questMachine.RegisterAction(new PlayerHandsover(null));
            questMachine.RegisterAction(new InfectPlayerWith(null));
            _mod.IsReady = true;
        }
    }

    public class ReducePlayerHealth : ActionTemplate
    {
        private int _percent;
        private int _amount;

        public ReducePlayerHealth(Quest parentQuest) : base(parentQuest)
        {
        }

        public override string Pattern => @"reduce player health by (?<percent>\d+)|reduce player health on (?<amount>\d+)";

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            Match match = Test(source);

            if (!match.Success) return null;

            var percentGroup = match.Groups["percent"];
            var amountGroup = match.Groups["amount"];

            return new ReducePlayerHealth(parentQuest)
            {
                _percent = percentGroup.Success ? Parser.ParseInt(match.Groups["percent"].Value) : 0,
                _amount = amountGroup.Success ? Parser.ParseInt(match.Groups["amount"].Value) : 0,
            };
        }

        public override void Update(Task _)
        {
            PlayerEntity player = GameManager.Instance.PlayerEntity;

            var health = 0;
            if (_percent > 0)
                health = (int)(player.CurrentHealth - player.MaxHealth / 100f * _percent);

            if (_amount > 0)
                health = player.CurrentHealth - _amount;

            if (health < 1) health = 1;

            player.CurrentHealth = health;
            SetComplete();
        }

        public override object GetSaveData()
        {
            return new SaveData_v1
            {
                percent = _percent,
                amount = _amount,
            };
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null) return;

            var data = (SaveData_v1)dataIn;

            _percent = data.percent;
            _amount = data.amount;
        }

        [FullSerializer.fsObject("v1")]
        private struct SaveData_v1
        {
            public int percent;
            public int amount;
        }
    }

    public class ReducePlayerFatigue : ActionTemplate
    {
        private int _percent;
        private int _amount;

        public ReducePlayerFatigue(Quest parentQuest) : base(parentQuest)
        {
        }

        public override string Pattern => @"reduce player fatigue by (?<percent>\d+)|reduce player fatigue on (?<amount>\d+)";

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            Match match = Test(source);

            if (!match.Success) return null;

            var percentGroup = match.Groups["percent"];
            var amountGroup = match.Groups["amount"];

            return new ReducePlayerFatigue(parentQuest)
            {
                _percent = percentGroup.Success ? Parser.ParseInt(match.Groups["percent"].Value) : 0,
                _amount = amountGroup.Success ? Parser.ParseInt(match.Groups["amount"].Value) : 0,
            };
        }

        public override void Update(Task _)
        {
            PlayerEntity player = GameManager.Instance.PlayerEntity;

            var fatigue = 0;
            if (_percent > 0)
                fatigue = (int)(player.CurrentFatigue - player.MaxFatigue / 100f * _percent);

            if (_amount > 0)
                fatigue = player.CurrentFatigue - _amount;

            if (fatigue < 1) fatigue = 1;

            player.CurrentFatigue = fatigue;
            SetComplete();
        }

        public override object GetSaveData()
        {
            return new SaveData_v1
            {
                percent = _percent,
                amount = _amount,
            };
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null) return;

            var data = (SaveData_v1)dataIn;

            _percent = data.percent;
            _amount = data.amount;
        }

        [FullSerializer.fsObject("v1")]
        private struct SaveData_v1
        {
            public int percent;
            public int amount;
        }
    }

    public class ReducePlayerMagicka : ActionTemplate
    {
        private int _percent;
        private int _amount;

        public ReducePlayerMagicka(Quest parentQuest) : base(parentQuest)
        {
        }

        public override string Pattern => @"reduce player magicka by (?<percent>\d+)|reduce player magicka on (?<amount>\d+)";

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            Match match = Test(source);

            if (!match.Success) return null;

            var percentGroup = match.Groups["percent"];
            var amountGroup = match.Groups["amount"];

            return new ReducePlayerMagicka(parentQuest)
            {
                _percent = percentGroup.Success ? Parser.ParseInt(match.Groups["percent"].Value) : 0,
                _amount = amountGroup.Success ? Parser.ParseInt(match.Groups["amount"].Value) : 0,
            };
        }

        public override void Update(Task _)
        {
            PlayerEntity player = GameManager.Instance.PlayerEntity;

            var magicka = 0;
            if (_percent > 0)
                magicka = (int)(player.CurrentMagicka - player.MaxMagicka / 100f * _percent);

            if (_amount > 0)
                magicka = player.CurrentMagicka - _amount;

            if (magicka < 1) magicka = 1;

            player.CurrentMagicka = magicka;
            SetComplete();
        }

        public override object GetSaveData()
        {
            return new SaveData_v1
            {
                percent = _percent,
                amount = _amount,
            };
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null) return;

            var data = (SaveData_v1)dataIn;

            _percent = data.percent;
            _amount = data.amount;
        }

        [FullSerializer.fsObject("v1")]
        private struct SaveData_v1
        {
            public int percent;
            public int amount;
        }
    }

    public class WithinUnits : ActionTemplate
    {
        private int _distance;
        private Symbol _symbol;

        public WithinUnits(Quest parentQuest) : base(parentQuest)
        {
            IsTriggerCondition = true;
        }

        public override string Pattern => @"player within (?<distance>\d+) units of foe (?<foe>[a-zA-Z0-9_.-]+)|player within (?<distance>\d+) units of item (?<item>[a-zA-Z0-9_.-]+)";

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            Match match = Test(source);
            if (!match.Success)
                return null;

            Group group = match.Groups["foe"];

            return new WithinUnits(parentQuest)
            {
                _distance = Parser.ParseInt(match.Groups["distance"].Value),
                _symbol = !group.Success ? new Symbol(match.Groups["item"].Value) : new Symbol(group.Value)
            };
        }

        public override bool CheckTrigger(Task _)
        {
            QuestResource resource = ParentQuest.GetResource(_symbol);

            if (resource == null || resource.QuestResourceBehaviour == null)
                return false;

            var targetPosition = resource.QuestResourceBehaviour.transform.position;
            var targetFlatPosition = new Vector2(targetPosition.x, targetPosition.z);
            var playerPosition = GameManager.Instance.PlayerGPS.transform.position;
            var playerFlatPosition = new Vector2(playerPosition.x, playerPosition.z);

            var theSameFloor = Mathf.Abs(targetPosition.y - playerPosition.y) <= 1.0;

            var closeHorizontally = Vector2.Distance(targetFlatPosition, playerFlatPosition) <= (double)_distance;

            return closeHorizontally && theSameFloor;
        }

        public override object GetSaveData()
        {
            return new SaveData_v1()
            {
                distance = _distance,
                symbol = _symbol
            };
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;
            var data = (SaveData_v1)dataIn;
            _distance = data.distance;
            _symbol = data.symbol;
        }

        [FullSerializer.fsObject("v1")]
        public struct SaveData_v1
        {
            public int distance;
            public Symbol symbol;
        }
    }

    public class PlayerPossesses : ActionTemplate
    {
        private int _numberOfItems;
        private int _itemClass;
        private int _itemSubClass;

        public PlayerPossesses(Quest parentQuest) : base(parentQuest)
        {
            IsTriggerCondition = true;
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
                var items = player.Items.SearchItems((ItemGroups)_itemClass, _itemSubClass);
                total += Helpers.CalculateLengthOrStackCount(items);
            }

            if (player.WagonItems.Contains((ItemGroups)_itemClass, _itemSubClass))
            {
                var items = player.WagonItems.SearchItems((ItemGroups)_itemClass, _itemSubClass);
                total += Helpers.CalculateLengthOrStackCount(items);
            }

            return total > _numberOfItems;
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

    public class InfectPlayerWith : ActionTemplate
    {
        private bool _vampire;
        private bool _werewolf;
        private bool _wereboar;

        public InfectPlayerWith(Quest parentQuest) : base(parentQuest)
        {
        }

        public override string Pattern => "infect player as (?<vampire>vampire)|infect player as (?<werewolf>werewolf)|infect player as (?<wereboar>wereboar)";

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            Match match = Test(source);

            if (!match.Success) return null;

            var vampire = match.Groups["vampire"].Success;
            var werewolf = match.Groups["werewolf"].Success;
            var wereboar = match.Groups["wereboar"].Success;

            return new InfectPlayerWith(parentQuest)
            {
                _vampire = vampire,
                _werewolf = werewolf,
                _wereboar = wereboar,
            };
        }

        public override void Update(Task _)
        {
            if (_vampire)
            {
                // Infect player with vampirism stage one
                EntityEffectBundle bundle = GameManager.Instance.PlayerEffectManager.CreateVampirismDisease();
                GameManager.Instance.PlayerEffectManager.AssignBundle(bundle, AssignBundleFlags.SpecialInfection);
            }

            if (_werewolf)
            {
                // Infect player with werewolf lycanthropy stage one
                EntityEffectBundle bundle = GameManager.Instance.PlayerEffectManager.CreateLycanthropyDisease(LycanthropyTypes.Werewolf);
                GameManager.Instance.PlayerEffectManager.AssignBundle(bundle, AssignBundleFlags.SpecialInfection);
            }

            if (_wereboar)
            {
                // Infect player with wereboar lycanthropy stage one
                EntityEffectBundle bundle = GameManager.Instance.PlayerEffectManager.CreateLycanthropyDisease(LycanthropyTypes.Wereboar);
                GameManager.Instance.PlayerEffectManager.AssignBundle(bundle, AssignBundleFlags.SpecialInfection);
            }

            SetComplete();
        }

        public override object GetSaveData()
        {
            return new SaveData_v1
            {
                vampire = _vampire,
                werewolf = _werewolf,
                wereboar = _wereboar,
            };
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null) return;

            var data = (SaveData_v1)dataIn;

            _vampire = data.vampire;
            _werewolf = data.werewolf;
            _wereboar = data.wereboar;
        }

        [FullSerializer.fsObject("v1")]
        private struct SaveData_v1
        {
            public bool vampire;
            public bool werewolf;
            public bool wereboar;
        }
    }

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
    }
}