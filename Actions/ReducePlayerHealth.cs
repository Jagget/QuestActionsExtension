using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Questing;
using UnityEngine;

namespace Game.Mods.QuestActionsExtension.Actions
{
    public class ReducePlayerHealth : ActionTemplate
    {
        private bool _cankill;
        private int _amount;
        private int _interval;
        private int _numberOfTimes;
        private int _percent;

        // These fields are for internal use
        private float _nextTick = 0;

        public ReducePlayerHealth(Quest parentQuest) : base(parentQuest)
        {
        }

        public override string Pattern => @"reduce player health by (?<percent>\d+) every (?<interval>\d+) seconds (?<numberOfTimes>\d+) times(?<cankill> can kill)|" +
                                          @"reduce player health by (?<percent>\d+) every (?<interval>\d+) seconds (?<numberOfTimes>\d+) times|" +
                                          @"reduce player health by (?<percent>\d+) every (?<interval>\d+) seconds(?<cankill> can kill)|" +
                                          @"reduce player health by (?<percent>\d+) every (?<interval>\d+) seconds|" +
                                          @"reduce player health by (?<percent>\d+)|" +
                                          @"reduce player health on (?<amount>\d+)";

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            Match match = Test(source);

            if (!match.Success)
            {
                return null;
            }

            var amountGroup = match.Groups["amount"];
            var cankillGroup = match.Groups["cankill"];
            var intervalGroup = match.Groups["interval"];
            var numberOfTimesGroup = match.Groups["numberOfTimes"];
            var percentGroup = match.Groups["percent"];

            return new ReducePlayerHealth(parentQuest)
            {
                _amount = amountGroup.Success ? Parser.ParseInt(amountGroup.Value) : 0,
                _cankill = cankillGroup.Success,
                _interval = intervalGroup.Success ? Parser.ParseInt(intervalGroup.Value) : 0,
                _numberOfTimes = numberOfTimesGroup.Success ? Parser.ParseInt(numberOfTimesGroup.Value) : intervalGroup.Success ? 1000000 : 1,
                _percent = percentGroup.Success ? Parser.ParseInt(percentGroup.Value) : 0,
            };
        }

        public override void Update(Task _)
        {
            // Increment timer
            if (Time.realtimeSinceStartup < _nextTick)
                return;

            if (_numberOfTimes > 0)
            {
                _numberOfTimes -= 1;
                ReduseHealth();
            }
            else
            {
                SetComplete();
            }

            // Update timer
            _nextTick = Time.realtimeSinceStartup + _interval;
        }

        private void ReduseHealth()
        {
            PlayerEntity player = GameManager.Instance.PlayerEntity;

            var health = 0;
            if (_percent > 0)
                health = (int)(player.CurrentHealth - player.MaxHealth / 100f * _percent);

            if (_amount > 0)
                health = player.CurrentHealth - _amount;

            if (health < 1) health = _cankill ? 0 : 1;

            player.CurrentHealth = health;
        }

        public override object GetSaveData()
        {
            return new SaveData_v1
            {
                amount = _amount,
                cankill = _cankill,
                interval = _interval,
                numberOfTimes = _numberOfTimes,
                percent = _percent,
            };
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null) return;

            var data = (SaveData_v1)dataIn;

            _amount = data.amount;
            _cankill = data.cankill;
            _interval = data.interval;
            _numberOfTimes = data.numberOfTimes;
            _percent = data.percent;
        }

        [FullSerializer.fsObject("v1")]
        private struct SaveData_v1
        {
            public bool cankill;
            public int amount;
            public int interval;
            public int numberOfTimes;
            public int percent;
        }
    }
}