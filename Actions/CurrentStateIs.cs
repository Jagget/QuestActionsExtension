using System.Text.RegularExpressions;
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
                                          "player current-state is (?<IsInBeastForm>in beast form)";

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

            if (godMode || noClipMode || noTargetMode || isResting || isLoitering || readyToLevelUp || arrested || inPrison || isInBeastForm)
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
        }

        [FullSerializer.fsObject("v1")]
        private struct SaveDataV1
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
        }
    }
}