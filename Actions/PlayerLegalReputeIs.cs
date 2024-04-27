using System.Text.RegularExpressions;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Questing;

namespace Game.Mods.QuestActionsExtension.Actions
{
    public class PlayerLegalReputeIs : ActionTemplate
    {
        private int _amount;
        private bool _lower;

        public override string Pattern => @"player legal-repute is (?<lower>lower) than (?<amount>[+-]?\d+)|" +
                                          @"player legal-repute is (?<higher>higher) than (?<amount>[+-]?\d+)";

        public PlayerLegalReputeIs(Quest parentQuest) : base(parentQuest)
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

            var lower = match.Groups["lower"].Success;
            var higher = match.Groups["higher"].Success;

            if (!lower && !higher)
            {
                DaggerfallUnity.LogMessage("Quest action 'player legal-repute' have incorrect syntax");
                return null;
            }

            if (lower != !higher)
            {
                DaggerfallUnity.LogMessage("Quest action 'player legal-repute' have incorrect syntax");
                return null;
            }

            _lower = lower;

            return new PlayerLegalReputeIs(parentQuest)
            {
                _amount = Parser.ParseInt(match.Groups["amount"].Value),
                _lower = _lower,
            };
        }

        public override bool CheckTrigger(Task caller)
        {
            var region = GameManager.Instance.PlayerGPS.CurrentRegionIndex;

            if (_lower)
            {
                return GameManager.Instance.PlayerEntity.RegionData[region].LegalRep <= _amount;
            }

            return GameManager.Instance.PlayerEntity.RegionData[region].LegalRep >= _amount;
        }

        #region Serialization

        [FullSerializer.fsObject("v1")]
        public struct SaveDataV1
        {
            public int Amount;
            public bool Lower;
        }

        public override object GetSaveData()
        {
            SaveDataV1 data = new SaveDataV1
            {
                Amount = _amount,
                Lower = _lower,
            };

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveDataV1 data = (SaveDataV1)dataIn;
            _amount = data.Amount;
            _lower = data.Lower;
        }

        #endregion
    }
}
