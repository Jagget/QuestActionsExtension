using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Questing;

namespace Game.Mods.QuestActionsExtension.Actions
{
    public class IncreasePlayerMagicka : ActionTemplate
    {
        private int _amount;
        private int _percent;

        public IncreasePlayerMagicka(Quest parentQuest) : base(parentQuest)
        {
        }

        public override string Pattern => @"increase player magicka by (?<percent>\d+)|" +
                                          @"increase player magicka on (?<amount>\d+)";

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            Match match = Test(source);

            if (!match.Success)
            {
                return null;
            }

            var percentGroup = match.Groups["percent"];
            var amountGroup = match.Groups["amount"];

            return new IncreasePlayerMagicka(parentQuest)
            {
                _amount = amountGroup.Success ? Parser.ParseInt(amountGroup.Value) : 0,
                _percent = percentGroup.Success ? Parser.ParseInt(percentGroup.Value) : 0,
            };
        }

        public override void Update(Task _)
        {
            PlayerEntity player = GameManager.Instance.PlayerEntity;

            var magicka = 0;
            if (_percent > 0)
                magicka = (int)(player.CurrentMagicka + player.MaxMagicka * _percent / 100f);

            if (_amount > 0)
                magicka = player.CurrentMagicka + _amount;

            if (magicka > player.MaxMagicka) magicka = player.MaxMagicka;

            player.CurrentMagicka = magicka;
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