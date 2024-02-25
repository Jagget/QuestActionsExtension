using System.Text.RegularExpressions;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Questing;

namespace Game.Mods.QuestActionsExtension.Actions
{
    public class CurrentMapPixel : ActionTemplate
    {
        private int _xCoord;
        private int _yCoord;
        private int _distance;

        public CurrentMapPixel(Quest parentQuest) : base(parentQuest)
        {
            IsTriggerCondition = true;
            IsAlwaysOnTriggerCondition = true;
        }

        public override string Pattern => @"player currentmappixel x (?<xCoord>\d+) y (?<yCoord>\d+) delta (?<distance>\d+)|" +
                                          @"player currentmappixel x (?<xCoord>\d+) y (?<yCoord>\d+)";

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            Match match = Test(source);
            if (!match.Success)
                return null;

            return new CurrentMapPixel(parentQuest)
            {
                _xCoord = Parser.ParseInt(match.Groups["xCoord"].Value),
                _yCoord = Parser.ParseInt(match.Groups["yCoord"].Value),
                _distance = match.Groups["distance"].Success ? Parser.ParseInt(match.Groups["distance"].Value) : 0,
            };
        }

        public override bool CheckTrigger(Task _)
        {
            var delta = GameManager.Instance.PlayerGPS.CurrentMapPixel.Distance(new DFPosition(_xCoord, _yCoord));
            return 0 <= delta && delta <= _distance;
        }

        public override object GetSaveData()
        {
            return new SaveData_v1()
            {
                xCoord = _xCoord,
                yCoord = _yCoord,
                distance = _distance,
            };
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;
            var data = (SaveData_v1)dataIn;

            _xCoord = data.xCoord;
            _yCoord = data.yCoord;
            _distance = data.distance;
        }

        [FullSerializer.fsObject("v1")]
        public struct SaveData_v1
        {
            public int xCoord;
            public int yCoord;
            public int distance;
        }
    }
}