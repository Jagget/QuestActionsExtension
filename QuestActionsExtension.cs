using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using Game.Mods.QuestActionsExtension.Actions;
using UnityEngine;

namespace Game.Mods.QuestActionsExtension
{
    public class QuestActionsExtension : MonoBehaviour
    {
        private static Mod _mod;

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            _mod = initParams.Mod;

            var go = new GameObject(_mod.Title);
            go.AddComponent<QuestActionsExtension>();

            QuestMachine questMachine = GameManager.Instance.QuestMachine;
            questMachine.RegisterAction(new ReducePlayerHealth(null));
            questMachine.RegisterAction(new ReducePlayerFatigue(null));
            questMachine.RegisterAction(new ReducePlayerMagicka(null));
            questMachine.RegisterAction(new WithinUnits(null));
            questMachine.RegisterAction(new PlayerPossesses(null));
            questMachine.RegisterAction(new PlayerHandsover(null));
            questMachine.RegisterAction(new InfectPlayerWith(null));
            questMachine.RegisterAction(new EquippedWithItem(null));
            questMachine.RegisterAction(new WhenHealthLevel(null));
            questMachine.RegisterAction(new WhenFatigueLevel(null));
            _mod.IsReady = true;
        }
    }
}