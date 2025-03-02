using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Questing;

namespace Game.Mods.QuestActionsExtension.Actions
{
    public class WhenFatigueLevel : ActionTemplate
    {
        private int _minPoints;
        private int _minPercent;

        public override string Pattern => @"player fatigue is less than (?<minPoints>\d+) pt|" +
                                          @"player fatigue is less than (?<minPercent>\d+)%";

        public WhenFatigueLevel(Quest parentQuest) : base(parentQuest)
        {
            IsTriggerCondition = true;
            IsAlwaysOnTriggerCondition = true;
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            Match match = Test(source);
            if (!match.Success)
                return null;

            var amountGroup = match.Groups["minPoints"];
            var percentGroup = match.Groups["minPercent"];

            return new WhenFatigueLevel(parentQuest)
            {
                _minPoints = amountGroup.Success ? Parser.ParseInt(amountGroup.Value) : 0,
                _minPercent = percentGroup.Success ? Parser.ParseInt(percentGroup.Value) : 0,
            };
        }

        public override bool CheckTrigger(Task caller)
        {
            if (_minPercent > 0)
            {
                var currentFatigue = (float)GameManager.Instance.PlayerEntity.CurrentFatigue * 100 / GameManager.Instance.PlayerEntity.MaxFatigue;
                return currentFatigue <= _minPercent;
            }

            if (_minPoints > 0)
            {
                return GameManager.Instance.PlayerEntity.CurrentFatigue <= _minPoints * DaggerfallEntity.FatigueMultiplier;
            }

            return false;
        }

        #region Serialization

        [FullSerializer.fsObject("v1")]
        public struct SaveData_v1
        {
            public int minPoints;
            public int minPercent;
        }

        public override object GetSaveData()
        {
            return new SaveData_v1
            {
                minPoints = _minPoints,
                minPercent = _minPercent,
            };
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            _minPoints = data.minPoints;
            _minPercent = data.minPercent;
        }

        #endregion
    }
}