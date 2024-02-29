using System.Text.RegularExpressions;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Utility;

namespace Game.Mods.QuestActionsExtension.Actions
{
    public class RaiseTime : ActionTemplate
    {
        private int _hours;
        private int _minutes;
        private int _sayingID;

        public RaiseTime(Quest parentQuest) : base(parentQuest)
        {
        }

        public override string Pattern => @"raise time by (?<hours>\d+):(?<minutes>\d+) saying (?<sayingID>\d+)|" +
                                          @"raise time by (?<hours>\d+):(?<minutes>\d+)";

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            Match match = Test(source);

            if (!match.Success) return null;

            var hoursGroup = match.Groups["hours"];
            var minutesGroup = match.Groups["minutes"];
            var sayingIDGroup = match.Groups["sayingID"];

            var hours = hoursGroup.Success ? Parser.ParseInt(hoursGroup.Value) : 0;
            var mintes = minutesGroup.Success ? Parser.ParseInt(minutesGroup.Value) : 0;

            if (hours < 0 || mintes < 0) return null;
            if (hours == 0 && mintes == 0) return null;

            return new RaiseTime(parentQuest)
            {
                _hours = hours,
                _minutes = mintes,
                _sayingID = sayingIDGroup.Success ? Parser.ParseInt(sayingIDGroup.Value) : 0,
            };
        }

        public override void Update(Task _)
        {
            var now = DaggerfallUnity.Instance.WorldTime.Now;
            var seconds = DaggerfallDateTime.SecondsPerMinute * _minutes + DaggerfallDateTime.SecondsPerHour * _hours;
            now.RaiseTime(seconds);

            if (_sayingID > 0)
            {
                ParentQuest.ShowMessagePopup(_sayingID);
            }

            SetComplete();
        }

        public override object GetSaveData()
        {
            return new SaveData_v1
            {
                hours = _hours,
                minutes = _minutes,
                sayingID = _sayingID,
            };
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null) return;

            var data = (SaveData_v1)dataIn;

            _hours = data.hours;
            _minutes = data.minutes;
            _sayingID = data.sayingID;
        }

        [FullSerializer.fsObject("v1")]
        private struct SaveData_v1
        {
            public int hours;
            public int minutes;
            public int sayingID;
        }
    }
}