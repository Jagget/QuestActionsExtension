using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Questing;

namespace Game.Mods.QuestActionsExtension.Actions
{
    public class ReducePlayerHealth : ActionTemplate
    {
        private int _percent;
        private int _amount;

        public ReducePlayerHealth(Quest parentQuest) : base(parentQuest)
        {
        }

        public override string Pattern => @"reduce player health by (?<percent>\d+)|" +
                                          @"reduce player health on (?<amount>\d+)";

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            Match match = Test(source);

            if (!match.Success) return null;

            var percentGroup = match.Groups["percent"];
            var amountGroup = match.Groups["amount"];

            return new ReducePlayerHealth(parentQuest)
            {
                _percent = percentGroup.Success ? Parser.ParseInt(percentGroup.Value) : 0,
                _amount = amountGroup.Success ? Parser.ParseInt(amountGroup.Value) : 0,
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
}