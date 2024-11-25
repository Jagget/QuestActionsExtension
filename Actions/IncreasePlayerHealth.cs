using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Questing;

namespace Game.Mods.QuestActionsExtension.Actions
{
    public class IncreasePlayerHealth : ActionTemplate
    {
        private int _amount;
        private int _percent;

        public IncreasePlayerHealth(Quest parentQuest) : base(parentQuest)
        {
        }

        public override string Pattern => @"increase player health by (?<percent>\d+)|" +
                                          @"increase player health on (?<amount>\d+)";

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            Match match = Test(source);

            if (!match.Success)
            {
                return null;
            }

            var amountGroup = match.Groups["amount"];
            var percentGroup = match.Groups["percent"];

            return new IncreasePlayerHealth(parentQuest)
            {
                _amount = amountGroup.Success ? Parser.ParseInt(amountGroup.Value) : 0,
                _percent = percentGroup.Success ? Parser.ParseInt(percentGroup.Value) : 0,
            };
        }

        public override void Update(Task _)
        {
            PlayerEntity player = GameManager.Instance.PlayerEntity;

            var health = 0;
            if (_percent > 0)
                health = (int)(player.CurrentHealth + player.MaxHealth / 100f * _percent);

            if (_amount > 0)
                health = player.CurrentHealth + _amount;

            if (health > player.MaxHealth) health = player.MaxHealth;

            player.CurrentHealth = health;
            SetComplete();
        }

        public override object GetSaveData()
        {
            return new SaveData_v1
            {
                amount = _amount,
                percent = _percent,
            };
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null) return;

            var data = (SaveData_v1)dataIn;

            _amount = data.amount;
            _percent = data.percent;
        }

        [FullSerializer.fsObject("v1")]
        public struct SaveData_v1
        {
            public int amount;
            public int percent;
        }
    }
}