using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Questing;
using UnityEngine;

namespace Game.Mods.QuestActionsExtension.Actions
{
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
}