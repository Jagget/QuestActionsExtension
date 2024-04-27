using System.Collections.Generic;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Items;
using UnityEngine;

namespace Game.Mods.QuestActionsExtension
{
    public static class ConsoleCommands
    {
        public static class InBlockPosition
        {
            public const string Name = "qae_inblockposition";
            public const string Description = "Output current position coordinates inside the current block.";
            public const string Usage = "qae_inblockposition";

            public static string Execute(params string[] args)
            {
                var playerPosition = GameManager.Instance.PlayerGPS.transform.position;
                var x = Mathf.RoundToInt(playerPosition.x / 6.4f);
                var y = Mathf.RoundToInt(playerPosition.z / 6.4f);

                return $"Current in-block position:\nx:{x} y:{y}";
            }
        }

        public static class CurrentMapPixel
        {
            public const string Name = "qae_getcurrentpixel";
            public const string Description = "Output pixel coordinates for a current location.";
            public const string Usage = "qae_getcurrentpixel";

            public static string Execute(params string[] args)
            {
                var playerPosition = GameManager.Instance.PlayerGPS.CurrentMapPixel;

                return $"Current Pixel Coordinates:\nx: {playerPosition.X} y: {playerPosition.Y}";
            }
        }

        public static class CurrentRegionIndex
        {
            public const string Name = "qae_getcurrentregionindex";
            public const string Description = "Output integer index for a current region.";
            public const string Usage = "qae_getcurrentregionindex";

            public static string Execute(params string[] args)
            {
                var cri = GameManager.Instance.PlayerGPS.CurrentRegionIndex;

                return $"Current Region Index is: {cri}";
            }
        }

        public static class EnumerateInventory
        {
            private const string _error = "Incorrect arguments";
            public const string Name = "qae_player_possesses";
            public const string Description = "Output list of items that player possesses in the inventory or in the wagon.";
            public const string Usage = "qae_player_possesses (inventory|wagon)";

            public static string Execute(params string[] args)
            {
                if (args == null || args.Length < 1) return _error;

                ItemCollection container = null;

                if (args[0] == "inventory")
                {
                    container = GameManager.Instance.PlayerEntity.Items;
                }

                if (args[0] == "wagon")
                {
                    container = GameManager.Instance.PlayerEntity.WagonItems;
                }

                if (container == null) return Usage;

                List<string> itemText = new List<string>();

                var items = container.CloneAll();

                foreach (var item in items)
                {
                    if (!item.IsQuestItem)
                    {
                        itemText.Add("Name: " + item.LongName + " | Count: " + item.stackCount + " | ItemClass: " + (int)item.ItemGroup + " | TemplateIndex: " + item.TemplateIndex);
                    }
                }

                return "List of possessed items:\n" + string.Join("\n", itemText);
            }
        }
    }
}