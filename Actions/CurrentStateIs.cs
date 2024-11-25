using System.Text.RegularExpressions;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Questing;

namespace Game.Mods.QuestActionsExtension.Actions
{
    public class CurrentStateIs : ActionTemplate
    {
        private bool _godMode;
        private bool _noClipMode;
        private bool _noTargetMode;
        private bool _isResting;
        private bool _isLoitering;
        private bool _readyToLevelUp;
        private bool _arrested;
        private bool _inPrison;
        private bool _isInBeastForm;
        private bool _isVampire;
        private bool _isWereboar;
        private bool _isWerewolf;

        public CurrentStateIs(Quest parentQuest) : base(parentQuest)
        {
            IsTriggerCondition = true;
            IsAlwaysOnTriggerCondition = true;
        }

        public override string Pattern => "player current-state is (?<GodMode>god mode)|" +
                                          "player current-state is (?<NoClipMode>no clip mode)|" +
                                          "player current-state is (?<NoTargetMode>no target mode)|" +
                                          "player current-state is (?<IsResting>resting)|" +
                                          "player current-state is (?<IsLoitering>loitering)|" +
                                          "player current-state is (?<ReadyToLevelUp>ready to level up)|" +
                                          "player current-state is (?<Arrested>arrested)|" +
                                          "player current-state is (?<InPrison>in prison)|" +
                                          "player current-state is (?<IsInBeastForm>in beast form)" +
                                          "player current-state is (?<Vampire>vampire)" +
                                          "player current-state is (?<Wereboar>wereboar)" +
                                          "player current-state is (?<Werewolf>werewolf)";

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            Match match = Test(source);

            if (!match.Success) return null;

            var godMode = match.Groups["GodMode"].Success;
            var noClipMode = match.Groups["NoClipMode"].Success;
            var noTargetMode = match.Groups["NoTargetMode"].Success;
            var isResting = match.Groups["IsResting"].Success;
            var isLoitering = match.Groups["IsLoitering"].Success;
            var readyToLevelUp = match.Groups["ReadyToLevelUp"].Success;
            var arrested = match.Groups["Arrested"].Success;
            var inPrison = match.Groups["InPrison"].Success;
            var isInBeastForm = match.Groups["IsInBeastForm"].Success;
            var isVampire = match.Groups["Vampire"].Success;
            var isWereboar = match.Groups["Wereboar"].Success;
            var isWerewolf = match.Groups["Werewolf"].Success;

            if (godMode || noClipMode || noTargetMode || isResting || isLoitering || readyToLevelUp || arrested || inPrison || isInBeastForm || isVampire || isWereboar || isWerewolf)
            {
                return new CurrentStateIs(parentQuest)
                {
                    _godMode = godMode,
                    _noClipMode = noClipMode,
                    _noTargetMode = noTargetMode,
                    _isResting = isResting,
                    _isLoitering = isLoitering,
                    _readyToLevelUp = readyToLevelUp,
                    _arrested = arrested,
                    _inPrison = inPrison,
                    _isInBeastForm = isInBeastForm,
                    _isVampire = isVampire,
                    _isWereboar = isWereboar,
                    _isWerewolf = isWerewolf,
                };
            }

            return null;
        }

        public override bool CheckTrigger(Task _)
        {
            if (_godMode)
            {
                return GameManager.Instance.PlayerEntity.GodMode;
            }

            if (_noClipMode)
            {
                return GameManager.Instance.PlayerEntity.NoClipMode;
            }

            if (_noTargetMode)
            {
                return GameManager.Instance.PlayerEntity.NoTargetMode;
            }

            if (_isResting)
            {
                return GameManager.Instance.PlayerEntity.IsResting;
            }

            if (_isLoitering)
            {
                return GameManager.Instance.PlayerEntity.IsLoitering;
            }

            if (_readyToLevelUp)
            {
                return GameManager.Instance.PlayerEntity.ReadyToLevelUp;
            }

            if (_arrested)
            {
                return GameManager.Instance.PlayerEntity.Arrested;
            }

            if (_inPrison)
            {
                return GameManager.Instance.PlayerEntity.InPrison;
            }

            if (_isInBeastForm)
            {
                return GameManager.Instance.PlayerEntity.IsInBeastForm;
            }

            if (_isVampire)
            {
                return GameManager.Instance.PlayerEffectManager.HasVampirism();
            }

            if (_isWereboar)
            {
                return GameManager.Instance.PlayerEffectManager.LycanthropyType() == LycanthropyTypes.Wereboar;
            }

            if (_isWerewolf)
            {
                return GameManager.Instance.PlayerEffectManager.LycanthropyType() == LycanthropyTypes.Werewolf;
            }

            return false;
        }

        public override object GetSaveData()
        {
            return new SaveDataV1
            {
                GodMode = _godMode,
                NoClipMode = _noClipMode,
                NoTargetMode = _noTargetMode,
                IsResting = _isResting,
                IsLoitering = _isLoitering,
                ReadyToLevelUp = _readyToLevelUp,
                Arrested = _arrested,
                InPrison = _inPrison,
                IsInBeastForm = _isInBeastForm,
                IsVampire = _isVampire,
                IsWereboar = _isWereboar,
                IsWerewolf = _isWerewolf,
            };
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null) return;

            var data = (SaveDataV1)dataIn;

            _godMode = data.GodMode;
            _noClipMode = data.NoClipMode;
            _noTargetMode = data.NoTargetMode;
            _isResting = data.IsResting;
            _isLoitering = data.IsLoitering;
            _readyToLevelUp = data.ReadyToLevelUp;
            _arrested = data.Arrested;
            _inPrison = data.InPrison;
            _isInBeastForm = data.IsInBeastForm;
            _isVampire = data.IsVampire;
            _isWereboar = data.IsWereboar;
            _isWerewolf = data.IsWerewolf;
        }

        [FullSerializer.fsObject("v1")]
        public struct SaveDataV1
        {
            public bool GodMode;
            public bool NoClipMode;
            public bool NoTargetMode;
            public bool IsResting;
            public bool IsLoitering;
            public bool ReadyToLevelUp;
            public bool Arrested;
            public bool InPrison;
            public bool IsInBeastForm;
            public bool IsVampire;
            public bool IsWereboar;
            public bool IsWerewolf;
        }
    }
}