using System;
using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Questing;

namespace Game.Mods.QuestActionsExtension.Actions
{
    public class UpdateQuestItemMaterial : ActionTemplate
    {
        private Symbol _itemSymbol;
        private string _material;
        private bool _isWeaponMat;
        private bool _isArmorMat;
        private bool _isLeveled;

        public UpdateQuestItemMaterial(Quest parentQuest) : base(parentQuest)
        {
        }

        public override string Pattern => @"update-quest-item (?<anItem>[a-zA-Z0-9_.]+) set-material (?<material>\w+)";

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;


            if (string.IsNullOrEmpty(match.Groups["anItem"].Value))
            {
                SetComplete();
                return null;
            }


            if (string.IsNullOrEmpty(match.Groups["material"].Value))
            {
                SetComplete();
                return null;
            }

            var desiredMaterial = match.Groups["material"].Value;

            var weaponMat = Enum.IsDefined(typeof(WeaponMaterialTypes), desiredMaterial);
            var armorMat = Enum.IsDefined(typeof(ArmorMaterialTypes), desiredMaterial);
            var leveled = desiredMaterial == "Leveled";

            if (weaponMat || armorMat || leveled)
            {
                return new UpdateQuestItemMaterial(parentQuest)
                {
                    _itemSymbol = new Symbol(match.Groups["anItem"].Value),
                    _material = desiredMaterial,
                    _isWeaponMat = weaponMat,
                    _isArmorMat = armorMat,
                    _isLeveled = leveled,
                };
            }

            SetComplete();
            return null;
        }

        public override void Update(Task _)
        {
            // Get related Item resource
            Item item = ParentQuest.GetItem(_itemSymbol);
            if (item == null)
            {
                // Stop if Item does not exist
                SetComplete();
                return;
            }


            var playerEntity = GameManager.Instance.PlayerEntity;


            switch (item.DaggerfallUnityItem.ItemGroup)
            {
                case ItemGroups.Weapons when (_isWeaponMat || _isLeveled):
                {
                    var newMaterial = _isLeveled ? FormulaHelper.RandomMaterial(playerEntity.Level) : (WeaponMaterialTypes)Enum.Parse(typeof(WeaponMaterialTypes), _material);
                    ItemBuilder.ApplyWeaponMaterial(item.DaggerfallUnityItem, newMaterial);
                    SetComplete();
                    return;
                }
                case ItemGroups.Armor when (_isArmorMat || _isLeveled):
                {
                    var newMaterial = _isLeveled ? FormulaHelper.RandomArmorMaterial(playerEntity.Level) : (ArmorMaterialTypes)Enum.Parse(typeof(ArmorMaterialTypes), _material);
                    ItemBuilder.ApplyArmorSettings(item.DaggerfallUnityItem, playerEntity.Gender, playerEntity.Race, newMaterial);
                    SetComplete();
                    return;
                }
                default:
                    // Item is not a weapon and not an armor. Just close the task.
                    SetComplete();
                    return;
            }
        }

        public override object GetSaveData()
        {
            return new SaveDataV1
            {
                ItemSymbol = _itemSymbol,
                Material = _material,
                IsWeaponMat = _isWeaponMat,
                IsArmorMat = _isArmorMat,
                IsLeveled = _isLeveled,
            };
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null) return;

            var data = (SaveDataV1)dataIn;

            _itemSymbol = data.ItemSymbol;
            _material = data.Material;
            _isWeaponMat = data.IsWeaponMat;
            _isArmorMat = data.IsArmorMat;
            _isLeveled = data.IsLeveled;
        }

        [FullSerializer.fsObject("v1")]
        private struct SaveDataV1
        {
            public Symbol ItemSymbol;
            public string Material;
            public bool IsWeaponMat;
            public bool IsArmorMat;
            public bool IsLeveled;
        }
    }
}