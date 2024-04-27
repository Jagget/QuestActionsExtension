using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using Game.Mods.QuestActionsExtension.Actions;
using UnityEngine;
using Wenzil.Console;

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
            questMachine.RegisterAction(new WhenMagickaLevel(null));
            questMachine.RegisterAction(new CurrentMapPixel(null));
            questMachine.RegisterAction(new CurrentInBlockPixel(null));
            questMachine.RegisterAction(new GuildRankAtLeast(null));
            questMachine.RegisterAction(new RaiseTime(null));
            questMachine.RegisterAction(new EnemyHealthLowerHigher(null));
            questMachine.RegisterAction(new KilledEnemiesOfClass(null));
            questMachine.RegisterAction(new MagicEfectKeyIsOn(null));
            questMachine.RegisterAction(new CurrentStateIs(null));
            questMachine.RegisterAction(new UpdateQuestItemMaterial(null));
            questMachine.RegisterAction(new UpdateQuestItemAddEnchantment(null));
            questMachine.RegisterAction(new UpdateQuestItemApplyMagicTemplate(null));
            questMachine.RegisterAction(new PlayerLegalReputeIs(null));
            questMachine.RegisterAction(new PlayerCurrentRegionIs(null));

            ConsoleCommandsDatabase.RegisterCommand(ConsoleCommands.InBlockPosition.Name, ConsoleCommands.InBlockPosition.Description, ConsoleCommands.InBlockPosition.Usage, ConsoleCommands.InBlockPosition.Execute);
            ConsoleCommandsDatabase.RegisterCommand(ConsoleCommands.CurrentMapPixel.Name, ConsoleCommands.CurrentMapPixel.Description, ConsoleCommands.CurrentMapPixel.Usage, ConsoleCommands.CurrentMapPixel.Execute);
            ConsoleCommandsDatabase.RegisterCommand(ConsoleCommands.EnumerateInventory.Name, ConsoleCommands.EnumerateInventory.Description, ConsoleCommands.EnumerateInventory.Usage, ConsoleCommands.EnumerateInventory.Execute);
            ConsoleCommandsDatabase.RegisterCommand(ConsoleCommands.CurrentRegionIndex.Name, ConsoleCommands.CurrentRegionIndex.Description, ConsoleCommands.CurrentRegionIndex.Usage, ConsoleCommands.CurrentRegionIndex.Execute);

            _mod.IsReady = true;
        }
    }
}
