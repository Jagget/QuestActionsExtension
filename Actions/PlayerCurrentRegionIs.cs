using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Questing;

namespace Game.Mods.QuestActionsExtension.Actions
{
    public class PlayerCurrentRegionIs : ActionTemplate
    {
        private int _index;
        private bool _lower;

        public override string Pattern => @"player current-region-index is (?<index>\d+)";

        public PlayerCurrentRegionIs(Quest parentQuest) : base(parentQuest)
        {
            IsTriggerCondition = true;
            IsAlwaysOnTriggerCondition = true;
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            return new PlayerCurrentRegionIs(parentQuest)
            {
                _index = Parser.ParseInt(match.Groups["index"].Value),
            };
        }

        public override bool CheckTrigger(Task caller)
        {
            return GameManager.Instance.PlayerGPS.CurrentRegionIndex == _index;
        }

        #region Serialization

        [FullSerializer.fsObject("v1")]
        public struct SaveDataV1
        {
            public int Index;
        }

        public override object GetSaveData()
        {
            SaveDataV1 data = new SaveDataV1
            {
                Index = _index,
            };

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveDataV1 data = (SaveDataV1)dataIn;
            _index = data.Index;
        }

        #endregion
    }
}
