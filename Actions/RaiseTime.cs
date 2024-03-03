using System.Text.RegularExpressions;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Utility;
using UnityEngine;

namespace Game.Mods.QuestActionsExtension.Actions
{
    public class RaiseTime : ActionTemplate
    {
        private int _hours;
        private int _hoursTo;
        private int _minutes;
        private int _minutesTo;
        private int _sayingID;

        public RaiseTime(Quest parentQuest) : base(parentQuest)
        {
        }

        public override string Pattern => @"raise time to (?<hoursTo>\d+):(?<minutesTo>\d+) saying (?<sayingID>\d+)|" +
                                          @"raise time to (?<hoursTo>\d+):(?<minutesTo>\d+)" +
                                          @"raise time by (?<hours>\d+):(?<minutes>\d+) saying (?<sayingID>\d+)|" +
                                          @"raise time by (?<hours>\d+):(?<minutes>\d+)";

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            Match match = Test(source);

            if (!match.Success) return null;

            var hoursGroup = match.Groups["hours"];
            var minutesGroup = match.Groups["minutes"];
            var hoursToGroup = match.Groups["hoursTo"];
            var minutesToGroup = match.Groups["minutesTo"];
            var sayingIDGroup = match.Groups["sayingID"];

            var hours = hoursGroup.Success ? Mathf.Max(Parser.ParseInt(hoursGroup.Value), 0) : 0;
            var mintes = minutesGroup.Success ? Mathf.Max(Parser.ParseInt(minutesGroup.Value), 0) : 0;
            var hoursTo = hoursToGroup.Success ? Mathf.Clamp(Parser.ParseInt(hoursToGroup.Value), 0, 23) : 0;
            var mintesTo = minutesToGroup.Success ? Mathf.Clamp(Parser.ParseInt(minutesToGroup.Value), 0, 59) : 0;

            if (hours + mintes + hoursTo + mintesTo == 0) return null;

            return new RaiseTime(parentQuest)
            {
                _hours = hours,
                _minutes = mintes,
                _hoursTo = hoursTo,
                _minutesTo = mintesTo,
                _sayingID = sayingIDGroup.Success ? Parser.ParseInt(sayingIDGroup.Value) : 0,
            };
        }

        public override void Update(Task _)
        {
            var now = DaggerfallUnity.Instance.WorldTime.Now;

            if (_minutes + _hours > 0)
            {
                var seconds = DaggerfallDateTime.SecondsPerMinute * _minutes + DaggerfallDateTime.SecondsPerHour * _hours;
                now.RaiseTime(seconds);
            }
            else if (_minutesTo + _hoursTo > 0)
            {
                var currentTime = DaggerfallDateTime.SecondsPerMinute * now.Minute + DaggerfallDateTime.SecondsPerHour * now.Hour;
                var desiredTime = DaggerfallDateTime.SecondsPerMinute * _minutesTo + DaggerfallDateTime.SecondsPerHour * _hoursTo;

                if (desiredTime >= currentTime)
                {
                    now.RaiseTime(desiredTime - currentTime);
                }
                else
                {
                    now.RaiseTime(desiredTime - currentTime + DaggerfallDateTime.SecondsPerDay);
                }
            }

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
                hoursTo = _hoursTo,
                minutesTo = _minutesTo,
                sayingID = _sayingID,
            };
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null) return;

            var data = (SaveData_v1)dataIn;

            _hours = data.hours;
            _minutes = data.minutes;
            _hoursTo = data.hoursTo;
            _minutesTo = data.minutesTo;
            _sayingID = data.sayingID;
        }

        [FullSerializer.fsObject("v1")]
        private struct SaveData_v1
        {
            public int hours;
            public int minutes;
            public int hoursTo;
            public int minutesTo;
            public int sayingID;
        }
    }
}