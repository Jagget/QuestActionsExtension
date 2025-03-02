using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Questing;
using UnityEngine;

namespace Game.Mods.QuestActionsExtension.Actions
{
    public class EnemyHealthLowerHigher : ActionTemplate
    {
        private int _highLimit;
        private int _lowLimit;
        private Symbol _aFoe;

        public EnemyHealthLowerHigher(Quest parentQuest) : base(parentQuest)
        {
            IsTriggerCondition = true;
            IsAlwaysOnTriggerCondition = true;
        }

        public override string Pattern => @"enemy (?<aFoe>[a-zA-Z0-9_.-]+) health is lower than (?<highLimit>\d+)% and higher than (?<lowLimit>\d+)%|" +
                                          @"enemy (?<aFoe>[a-zA-Z0-9_.-]+) health is lower than (?<highLimit>\d+)%";

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            Match match = Test(source);
            if (!match.Success)
                return null;

            var highLimitGroup = match.Groups["highLimit"];
            var lowLimitGroup = match.Groups["lowLimit"];
            var aFoeGroup = match.Groups["aFoe"];

            var highLimit = highLimitGroup.Success ? Mathf.Clamp(Parser.ParseInt(highLimitGroup.Value), 0, 99) : 0;

            if (highLimit == 0) return null;

            var lowLimit = lowLimitGroup.Success ? Mathf.Clamp(Parser.ParseInt(lowLimitGroup.Value), 1, highLimit) : 1;

            if (lowLimit >= highLimit) return null;

            var aFoe = aFoeGroup.Success ? new Symbol(aFoeGroup.Value) : null;

            return new EnemyHealthLowerHigher(parentQuest)
            {
                _highLimit = highLimit,
                _lowLimit = lowLimit,
                _aFoe = aFoe,
            };
        }

        public override bool CheckTrigger(Task _)
        {
            Foe foe = ParentQuest.GetFoe(_aFoe);

            if (foe == null)
                return false;

            if (foe.IsHidden || !foe.InjuredTrigger)
                return false;

            // Get the Entity of foe if it exists
            var enemy = foe.QuestResourceBehaviour.GetComponent<DaggerfallEntityBehaviour>()?.Entity;

            if (enemy == null)
                return false;

            var healthPercent = Mathf.RoundToInt(100f * enemy.CurrentHealth / enemy.MaxHealth);

            return _lowLimit <= healthPercent && healthPercent <= _highLimit;
        }

        public override object GetSaveData()
        {
            return new SaveData_v1()
            {
                highLimit = _highLimit,
                lowLimit = _lowLimit,
                aFoe = _aFoe,
            };
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;
            var data = (SaveData_v1)dataIn;

            _highLimit = data.highLimit;
            _lowLimit = data.lowLimit;
            _aFoe = data.aFoe;
        }

        [FullSerializer.fsObject("v1")]
        public struct SaveData_v1
        {
            public int highLimit;
            public int lowLimit;
            public Symbol aFoe;
        }
    }
}