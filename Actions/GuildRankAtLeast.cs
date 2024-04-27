using System;
using System.Text.RegularExpressions;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Questing;

namespace Game.Mods.QuestActionsExtension.Actions
{
    public class GuildRankAtLeast : ActionTemplate
    {
        private int _minRank;
        private string _guildGroupName;

        public override string Pattern => @"player guild rank in (?<guildGroupName>[a-zA-Z0-9'_.-]+) at least (?<minRank>\d+)";

        public GuildRankAtLeast(Quest parentQuest) : base(parentQuest)
        {
            IsTriggerCondition = true;
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            Match match = Test(source);
            if (!match.Success)
                return null;

            var minRankGroup = match.Groups["minRank"];
            var guildGroupNameGroup = match.Groups["guildGroupName"];

            return new GuildRankAtLeast(parentQuest)
            {
                _minRank = minRankGroup.Success ? Parser.ParseInt(minRankGroup.Value) : 0,
                _guildGroupName = guildGroupNameGroup.Success ? guildGroupNameGroup.Value : "",
            };
        }

        public override bool CheckTrigger(Task caller)
        {
            var guildManager = GameManager.Instance.GuildManager;

            // Is the group a guild group?
            if (!Enum.IsDefined(typeof(FactionFile.GuildGroups), _guildGroupName))
            {
                return false;
            }

            var guildGroup = (FactionFile.GuildGroups)Enum.Parse(typeof(FactionFile.GuildGroups), _guildGroupName);

            if (guildManager.HasJoined(guildGroup))
            {
                return guildManager.GetGuild(guildGroup).Rank >= _minRank;
            }

            return false;
        }

        #region Serialization

        [FullSerializer.fsObject("v1")]
        public struct SaveData_v1
        {
            public int minRank;
            public string guildGroupName;
        }

        public override object GetSaveData()
        {
            return new SaveData_v1
            {
                minRank = _minRank,
                guildGroupName = _guildGroupName,
            };
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            _minRank = data.minRank;
            _guildGroupName = data.guildGroupName;
        }

        #endregion
    }
}