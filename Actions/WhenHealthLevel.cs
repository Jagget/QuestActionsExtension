using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Questing;

namespace Game.Mods.QuestActionsExtension.Actions
{
    public class WhenHealthLevel : ActionTemplate
    {
        private int _minHealthPoints;
        private int _minHealthPercent;

        public override string Pattern => @"pchealth lower than (?<minHealthPoints>\d+)|" +
                                          @"player health less than (?<minHealthPoints>\d+) pt|" +
                                          @"pchealthp lower than (?<minHealthPercent>\d+)|" +
                                          @"player health less than (?<minHealthPercent>\d+)%";

        public WhenHealthLevel(Quest parentQuest) : base(parentQuest)
        {
            IsTriggerCondition = true;
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            var amountGroup = match.Groups["minHealthPoints"];
            var percentGroup = match.Groups["minHealthPercent"];

            // Factory new action
            return new WhenHealthLevel(parentQuest)
            {
                _minHealthPoints = amountGroup.Success ? Parser.ParseInt(amountGroup.Value) : 0,
                _minHealthPercent = percentGroup.Success ? Parser.ParseInt(percentGroup.Value) : 0,
            };
        }

        public override bool CheckTrigger(Task caller)
        {
            if (_minHealthPercent > 0)
            {
                var currentHealth = GameManager.Instance.PlayerEntity.CurrentHealthPercent * 100;
                return currentHealth <= _minHealthPercent;
            }

            if (_minHealthPoints > 0)
            {
                return GameManager.Instance.PlayerEntity.CurrentHealth <= _minHealthPoints;
            }

            return false;
        }

        #region Serialization

        [FullSerializer.fsObject("v1")]
        private struct SaveData_v1
        {
            public int minHealthPoints;
            public int minHealthPercent;
        }

        public override object GetSaveData()
        {
            return new SaveData_v1
            {
                minHealthPoints = _minHealthPoints,
                minHealthPercent = _minHealthPercent,
            };
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            _minHealthPoints = data.minHealthPoints;
            _minHealthPercent = data.minHealthPercent;
        }

        #endregion
    }
}