using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Questing;
using UnityEngine;

namespace Game.Mods.QuestActionsExtension.Actions
{
    public class CurrentInBlockPixel : ActionTemplate
    {
        private const float _pixMult = 6.4f;
        private int _xCoord;
        private int _yCoord;
        private float _distance;

        public CurrentInBlockPixel(Quest parentQuest) : base(parentQuest)
        {
            IsTriggerCondition = true;
            IsAlwaysOnTriggerCondition = true;
        }

        public override string Pattern => @"player inblock position x (?<xCoord>\d+) y (?<yCoord>\d+) delta (?<distance>\d+)|" +
                                          @"player inblock position x (?<xCoord>\d+) y (?<yCoord>\d+)";

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            Match match = Test(source);
            if (!match.Success)
                return null;

            return new CurrentInBlockPixel(parentQuest)
            {
                _xCoord = Mathf.Clamp(Parser.ParseInt(match.Groups["xCoord"].Value), 0, 128),
                _yCoord = Mathf.Clamp(Parser.ParseInt(match.Groups["yCoord"].Value), 0, 128),
                _distance = match.Groups["distance"].Success ? Parser.ParseInt(match.Groups["distance"].Value) : 0.1f,
            };
        }

        public override bool CheckTrigger(Task _)
        {
            var playerPosition = GameManager.Instance.PlayerGPS.transform.position;
            var playerFlatPosition = new Vector2(playerPosition.x, playerPosition.z);

            var delta = Vector2.Distance(new Vector2(_xCoord * _pixMult, _yCoord * _pixMult), playerFlatPosition);
            return 0 <= delta && delta <= _distance * _pixMult;
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
            public float distance;
        }
    }
}