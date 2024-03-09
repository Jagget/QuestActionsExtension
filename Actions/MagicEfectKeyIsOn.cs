using System;
using System.Linq;
using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.Questing;

namespace Game.Mods.QuestActionsExtension.Actions
{
    public class MagicEfectKeyIsOn : ActionTemplate
    {
        private Symbol _aFoe;
        private bool _player;
        private string _effectKey;

        public MagicEfectKeyIsOn(Quest parentQuest) : base(parentQuest)
        {
            IsTriggerCondition = true;
            IsAlwaysOnTriggerCondition = true;
        }

        public override string Pattern => @"magic-effect key (?<effectKey>[a-zA-Z0-9_.-]+) is on foe (?<aFoe>[a-zA-Z0-9_.-]+)|" +
                                          @"magic-effect key (?<effectKey>[a-zA-Z0-9_.-]+) is on (?<player>player)";

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            Match match = Test(source);
            if (!match.Success)
                return null;

            var effectKeyGroup = match.Groups["effectKey"];
            var aFoeGroup = match.Groups["aFoe"];
            var playerGroup = match.Groups["player"];

            var effectTemplateExist = false;
            if (effectKeyGroup.Success)
            {
                effectTemplateExist = GameManager.Instance.EntityEffectBroker.HasEffectTemplate(effectKeyGroup.Value);
            }

            if (!effectTemplateExist || !effectKeyGroup.Success)
            {
                throw new Exception($"MagicEfectKeyIsOn: magic-effect key {effectKeyGroup.Value} does not exist");
            }

            return new MagicEfectKeyIsOn(parentQuest)
            {
                _aFoe = aFoeGroup.Success ? new Symbol(aFoeGroup.Value) : null,
                _player = playerGroup.Success,
                _effectKey = effectKeyGroup.Value,
            };
        }

        public override bool CheckTrigger(Task _)
        {
            EntityEffectManager effectManager;

            if (_player)
            {
                effectManager = GameManager.Instance.PlayerEffectManager;
            }
            else
            {
                Foe foe = ParentQuest.GetFoe(_aFoe);

                if (foe == null)
                    return false;

                if (foe.QuestResourceBehaviour == null)
                    return false;

                effectManager = foe.QuestResourceBehaviour.GetComponent<EntityEffectManager>();
            }

            if (!effectManager || effectManager == null)
            {
                return false;
            }

            var effectAttached = false;

            foreach (var bundle in effectManager.EffectBundles)
            {
                if (bundle.liveEffects.Any(effect => effect.Key == _effectKey))
                {
                    effectAttached = true;
                }
            }

            return effectAttached;
        }

        public override object GetSaveData()
        {
            return new SaveDataV1
            {
                AFoe = _aFoe,
                Player = _player,
                EffectKey = _effectKey,
            };
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;
            var data = (SaveDataV1)dataIn;

            _aFoe = data.AFoe;
            _player = data.Player;
            _effectKey = data.EffectKey;
        }

        [FullSerializer.fsObject("v1")]
        private struct SaveDataV1
        {
            public Symbol AFoe;
            public bool Player;
            public string EffectKey;
        }
    }
}