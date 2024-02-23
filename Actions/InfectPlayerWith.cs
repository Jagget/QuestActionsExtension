using System.Text.RegularExpressions;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.Questing;

namespace Game.Mods.QuestActionsExtension.Actions
{
    public class InfectPlayerWith : ActionTemplate
    {
        private bool _vampire;
        private bool _werewolf;
        private bool _wereboar;

        public InfectPlayerWith(Quest parentQuest) : base(parentQuest)
        {
        }

        public override string Pattern => "infect player as (?<vampire>vampire)|infect player as (?<werewolf>werewolf)|infect player as (?<wereboar>wereboar)";

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            Match match = Test(source);

            if (!match.Success) return null;

            var vampire = match.Groups["vampire"].Success;
            var werewolf = match.Groups["werewolf"].Success;
            var wereboar = match.Groups["wereboar"].Success;

            return new InfectPlayerWith(parentQuest)
            {
                _vampire = vampire,
                _werewolf = werewolf,
                _wereboar = wereboar,
            };
        }

        public override void Update(Task _)
        {
            if (_vampire)
            {
                // Infect player with vampirism stage one
                EntityEffectBundle bundle = GameManager.Instance.PlayerEffectManager.CreateVampirismDisease();
                GameManager.Instance.PlayerEffectManager.AssignBundle(bundle, AssignBundleFlags.SpecialInfection);
            }

            if (_werewolf)
            {
                // Infect player with werewolf lycanthropy stage one
                EntityEffectBundle bundle = GameManager.Instance.PlayerEffectManager.CreateLycanthropyDisease(LycanthropyTypes.Werewolf);
                GameManager.Instance.PlayerEffectManager.AssignBundle(bundle, AssignBundleFlags.SpecialInfection);
            }

            if (_wereboar)
            {
                // Infect player with wereboar lycanthropy stage one
                EntityEffectBundle bundle = GameManager.Instance.PlayerEffectManager.CreateLycanthropyDisease(LycanthropyTypes.Wereboar);
                GameManager.Instance.PlayerEffectManager.AssignBundle(bundle, AssignBundleFlags.SpecialInfection);
            }

            SetComplete();
        }

        public override object GetSaveData()
        {
            return new SaveData_v1
            {
                vampire = _vampire,
                werewolf = _werewolf,
                wereboar = _wereboar,
            };
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null) return;

            var data = (SaveData_v1)dataIn;

            _vampire = data.vampire;
            _werewolf = data.werewolf;
            _wereboar = data.wereboar;
        }

        [FullSerializer.fsObject("v1")]
        private struct SaveData_v1
        {
            public bool vampire;
            public bool werewolf;
            public bool wereboar;
        }
    }
}