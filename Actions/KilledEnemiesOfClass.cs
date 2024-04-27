using System;
using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Utility;

namespace Game.Mods.QuestActionsExtension.Actions
{
    public class KilledEnemiesOfClass : ActionTemplate
    {
        private int _foeTypeID;
        private Symbol _placeSymbol;
        private int _targetAmount;
        private int _killedAmount;
        private int _p1;
        private int _p2;
        private int _p3;

        public override string Pattern => @"player slain (?<amount>\d+) enemies of class (?<aFoe>\w+) at any (?<placeType>\w+)|" +
                                          @"player slain (?<amount>\d+) enemies of class (?<aFoe>\w+) at (?<aPlace>\w+)|" +
                                          @"player slain (?<amount>\d+) enemies of class (?<aFoe>\w+)";

        public KilledEnemiesOfClass(Quest parentQuest) : base(parentQuest)
        {
            IsTriggerCondition = true;
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Match

            var amountGroup = match.Groups["amount"];
            var aFoeGroup = match.Groups["aFoe"];
            var placeTypeGroup = match.Groups["placeType"];
            var aPlaceGroup = match.Groups["aPlace"];

            // Parse

            var amount = Parser.ParseInt(amountGroup.Value);

            var foeTypeID = -1;

            if (aFoeGroup.Success)
            {
                Table foesTable = QuestMachine.Instance.FoesTable;
                if (foesTable.HasValue(aFoeGroup.Value))
                {
                    foeTypeID = Parser.ParseInt(foesTable.GetValue("id", aFoeGroup.Value));
                }
                else
                {
                    throw new Exception($"KilledEnemiesOfClass: Foes data table does not contain an entry for {aFoeGroup.Value}");
                }
            }

            Symbol placeSymbol = null;
            if (aPlaceGroup.Success)
            {
                placeSymbol = new Symbol(aPlaceGroup.Value);
            }

            var p1 = -1;
            var p2 = -100;
            var p3 = -100;
            if (placeTypeGroup.Success)
            {
                var name = placeTypeGroup.Value;
                Table placesTable = QuestMachine.Instance.PlacesTable;
                if (placesTable.HasValue(name))
                {
                    // Store values
                    p1 = Place.CustomParseInt(placesTable.GetValue("p1", name));
                    if (p1 != 0 && p1 != 1)
                    {
                        throw new Exception("KilledEnemiesOfClass: This trigger condition can only be used with building types (p1=0) and dungeon types (p1=1) in Quests-Places table.");
                    }

                    p2 = Place.CustomParseInt(placesTable.GetValue("p2", name));
                    p3 = Place.CustomParseInt(placesTable.GetValue("p3", name));
                }
                else
                {
                    throw new Exception($"KilledEnemiesOfClass: Could not find place type name in data table: '{name}'");
                }
            }

            // Check

            if (foeTypeID < 0) return null;

            if (amount <= 0) return null;

            // Factory

            var trigger = new KilledEnemiesOfClass(parentQuest)
            {
                _foeTypeID = foeTypeID,
                _placeSymbol = placeSymbol,
                _targetAmount = amount,
                _killedAmount = 0,
                _p1 = p1,
                _p2 = p2,
                _p3 = p3,
            };

            trigger.RegisterEvents();

            return trigger;
        }

        public override bool CheckTrigger(Task _)
        {
            return _killedAmount >= _targetAmount;
        }

        private void EnemyDeath_OnEnemyDeath(object sender, EventArgs e)
        {
            var enemyDeath = (EnemyDeath)sender;

            if (!enemyDeath.TryGetComponent(out DaggerfallEntityBehaviour entityBehaviour))
                return;

            if (!enemyDeath.TryGetComponent(out EnemySenses enemySenses))
                return;

            var enemyEntity = (EnemyEntity)entityBehaviour.Entity;

            if (enemyEntity == null)
                return;

            // Someone else killed it
            if (enemySenses.Target != GameManager.Instance.PlayerEntityBehaviour)
                return;

            // I did kill it, but it's not what we need
            if (enemyEntity.MobileEnemy.ID != _foeTypeID) return;

            // I did kill it, and it's what we need
            var shouldCheckPlaceType = _p2 + _p3 > -200;
            var shouldCheckPlace = _placeSymbol != null;
            bool isCorrectPlaceType;
            bool isCorrectPlace;

            if (shouldCheckPlace)
            {
                // Get place resource
                Place place = ParentQuest.GetPlace(_placeSymbol);
                if (place == null)
                    return;

                // Check if player at this place
                isCorrectPlace = place.IsPlayerHere();
            }
            else
            {
                isCorrectPlace = true;
            }

            if (shouldCheckPlaceType)
            {
                isCorrectPlaceType = _p1 == 1 ? Helpers.IsPlayerAtDungeonType(_p2) : Place.IsPlayerAtBuildingType(_p2, _p3);
            }
            else
            {
                isCorrectPlaceType = true;
            }

            if (isCorrectPlace && isCorrectPlaceType)
            {
                _killedAmount += 1;
            }
        }

        private void RegisterEvents()
        {
            EnemyDeath.OnEnemyDeath += EnemyDeath_OnEnemyDeath;
        }

        public override void SetComplete()
        {
            EnemyDeath.OnEnemyDeath -= EnemyDeath_OnEnemyDeath;
            base.SetComplete();
        }

        public override object GetSaveData()
        {
            return new SaveDataV1
            {
                FoeTypeID = _foeTypeID,
                PlaceSymbol = _placeSymbol,
                TargetAmount = _targetAmount,
                KilledAmount = _killedAmount,
                P1 = _p1,
                P2 = _p2,
                P3 = _p3,
            };
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;
            var data = (SaveDataV1)dataIn;

            _foeTypeID = data.FoeTypeID;
            _placeSymbol = data.PlaceSymbol;
            _targetAmount = data.TargetAmount;
            _killedAmount = data.KilledAmount;
            _p1 = data.P1;
            _p2 = data.P2;
            _p3 = data.P3;

            // Register events when restoring action
            RegisterEvents();
        }

        [FullSerializer.fsObject("v1")]
        public struct SaveDataV1
        {
            public int FoeTypeID;
            public Symbol PlaceSymbol;
            public int TargetAmount;
            public int KilledAmount;
            public int P1;
            public int P2;
            public int P3;
        }
    }
}