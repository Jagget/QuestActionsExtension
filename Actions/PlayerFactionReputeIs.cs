using System;
using System.Text.RegularExpressions;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Questing;

namespace Game.Mods.QuestActionsExtension.Actions
{
    public class PlayerFactionReputeIs : ActionTemplate
    {
        private int _amount;
        private bool _lower;
        private string _npcName;
        private int _npcFactionID;
        private int _anyFactionID;
        private bool _useAnyFactionID;

        public override string Pattern => @"player faction-repute with (?<individualNpcName>[a-zA-Z0-9_.-]+) is (?<lower>lower) than (?<amount>[+-]?\d+)|" +
                                          @"player faction-repute with (?<individualNpcName>[a-zA-Z0-9_.-]+) is (?<higher>higher) than (?<amount>[+-]?\d+)|" +
                                          @"player faction-repute with-faction-id (?<anyFactionID>\d+) is (?<lower>lower) than (?<amount>[+-]?\d+)|" +
                                          @"player faction-repute with-faction-id (?<anyFactionID>\d+) is (?<higher>higher) than (?<amount>[+-]?\d+)";

        public PlayerFactionReputeIs(Quest parentQuest) : base(parentQuest)
        {
            IsTriggerCondition = true;
            IsAlwaysOnTriggerCondition = true;
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            var lower = match.Groups["lower"].Success;
            var higher = match.Groups["higher"].Success;

            if (!lower && !higher)
            {
                DaggerfallUnity.LogMessage("Quest action 'player faction-repute' have incorrect syntax");
                return null;
            }

            if (lower != !higher)
            {
                DaggerfallUnity.LogMessage("Quest action 'player faction-repute' have incorrect syntax");
                return null;
            }

            _lower = lower;
            
            // Confirm this is an individual NPC
            var individualNpcName = match.Groups["individualNpcName"].Value;
            var haveAnyFactionID = match.Groups["anyFactionID"].Success;

            if (haveAnyFactionID)
            {
                var anyFactionID = Parser.ParseInt(match.Groups["anyFactionID"].Value);

                if (!GameManager.Instance.PlayerEntity.FactionData.GetFactionData(anyFactionID, out _))
                {
                    throw new Exception($"PlayerFactionReputeIs: Could not find FactionID {anyFactionID}");
                }

                return new PlayerFactionReputeIs(parentQuest)
                {
                    _amount = Parser.ParseInt(match.Groups["amount"].Value),
                    _lower = _lower,
                    _anyFactionID = anyFactionID,
                    _useAnyFactionID = true,
                };
            }

            var factionID = Person.GetIndividualFactionID(individualNpcName);

            if (factionID == -1)
            {
                throw new Exception($"PlayerFactionReputeIs: Could not find individualNpcName {individualNpcName}");
            }

            FactionFile.FactionData factionData = Person.GetFactionData(factionID);

            if (factionData.type != (int)FactionFile.FactionTypes.Individual)
            {
                throw new Exception($"PlayerFactionReputeIs: NPC {individualNpcName} with FactionID {factionID} is not an individual NPC");
            }

            return new PlayerFactionReputeIs(parentQuest)
            {
                _amount = Parser.ParseInt(match.Groups["amount"].Value),
                _lower = _lower,
                _npcName = individualNpcName,
                _npcFactionID = factionID,
            };
        }

        public override bool CheckTrigger(Task caller)
        {
            var factionID = _useAnyFactionID ? _anyFactionID : _npcFactionID;

            if (!GameManager.Instance.PlayerEntity.FactionData.GetFactionData(factionID, out var factionData))
            {
                return false;
            }

            if (_lower)
            {
                return factionData.rep <= _amount;
            }

            return factionData.rep >= _amount;
        }

        #region Serialization

        [FullSerializer.fsObject("v1")]
        public struct SaveDataV1
        {
            public int Amount;
            public bool Lower;
            public string NpcName;
            public int NpcFactionID;
            public int AnyFactionID;
            public bool UseAnyFactionID;
        }

        public override object GetSaveData()
        {
            SaveDataV1 data = new SaveDataV1
            {
                Amount = _amount,
                Lower = _lower,
                NpcName = _npcName,
                NpcFactionID = _npcFactionID,
                AnyFactionID = _anyFactionID,
                UseAnyFactionID = _useAnyFactionID,
            };

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveDataV1 data = (SaveDataV1)dataIn;
            _amount = data.Amount;
            _lower = data.Lower;
            _npcName = data.NpcName;
            _npcFactionID = data.NpcFactionID;
            _anyFactionID = data.AnyFactionID;
            _useAnyFactionID = data.UseAnyFactionID;
        }

        #endregion
    }
}
