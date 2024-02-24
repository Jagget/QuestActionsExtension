using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Questing;

namespace Game.Mods.QuestActionsExtension.Actions
{
    public class ReducePlayerFatigue : ActionTemplate
    {
        private int _percent;
        private int _amount;

        public ReducePlayerFatigue(Quest parentQuest) : base(parentQuest)
        {
        }

        public override string Pattern => @"reduce player fatigue by (?<percent>\d+)|" +
                                          @"reduce player fatigue on (?<amount>\d+)";

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            Match match = Test(source);

            if (!match.Success) return null;

            var percentGroup = match.Groups["percent"];
            var amountGroup = match.Groups["amount"];

            return new ReducePlayerFatigue(parentQuest)
            {
                _percent = percentGroup.Success ? Parser.ParseInt(percentGroup.Value) : 0,
                _amount = amountGroup.Success ? Parser.ParseInt(amountGroup.Value) : 0,
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
}