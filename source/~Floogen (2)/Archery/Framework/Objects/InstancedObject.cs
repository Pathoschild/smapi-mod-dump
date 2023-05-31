/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using Archery.Framework.Models;
using Archery.Framework.Models.Weapons;
using Archery.Framework.Utilities;
using StardewValley;
using System;

namespace Archery.Framework.Objects
{
    internal class InstancedObject
    {
        public static string GetKey(Item item)
        {
            if (item is not null)
            {
                if (item.modData.ContainsKey(ModDataKeys.WEAPON_FLAG))
                {
                    return ModDataKeys.WEAPON_FLAG;
                }
                else if (item.modData.ContainsKey(ModDataKeys.AMMO_FLAG))
                {
                    return ModDataKeys.AMMO_FLAG;
                }
            }

            return String.Empty;
        }

        public static bool IsValid(Item item)
        {
            if (item is not null && item.modData.ContainsKey(GetKey(item)))
            {
                return true;
            }

            return false;
        }

        public static bool IsRecipe(Item item)
        {
            if (item is not null && item.modData.ContainsKey(ModDataKeys.RECIPE_FLAG))
            {
                return true;
            }

            return false;
        }

        public static string GetInternalId(Item item)
        {
            if (IsValid(item) is false || item.modData.TryGetValue(GetKey(item), out var id) is false)
            {
                return String.Empty;
            }

            return id;
        }

        internal static T GetModel<T>(Item item) where T : BaseModel
        {
            if (IsValid(item) is true)
            {
                var id = GetInternalId(item);
                if (Archery.modelManager.GetSpecificModel<BaseModel>(id) is BaseModel baseModel && baseModel is not null && baseModel is T)
                {
                    return (T)Archery.modelManager.GetSpecificModel<BaseModel>(id);
                }
            }

            return null;
        }

        internal static string GetName(Item item)
        {
            if (IsValid(item) is true)
            {
                var model = GetModel<BaseModel>(item);
                if (model is not null)
                {
                    return model.GetTranslation(model.DisplayName);
                }
            }

            return "DEFAULT NAME";
        }

        internal static string GetDescription(Item item)
        {
            string description = "DEFAULT DESCRIPTION";
            if (IsValid(item) is true)
            {
                var model = GetModel<BaseModel>(item);
                if (model is not null)
                {
                    description = model.GetTranslation(model.Description);

                    if (model is WeaponModel weaponModel && weaponModel.SpecialAttack is not null)
                    {
                        description = $"{description}\n\n{Archery.internalApi.GetSpecialAttackName(weaponModel.SpecialAttack.Id, weaponModel.SpecialAttack.Arguments)}\n{Archery.internalApi.GetSpecialAttackDescription(weaponModel.SpecialAttack.Id, weaponModel.SpecialAttack.Arguments)}";
                    }
                    else if (model is AmmoModel ammoModel)
                    {
                        if (ammoModel.Enchantment is not null)
                        {
                            description = $"{description}\n\n{Archery.internalApi.GetEnchantmentName(ammoModel.Enchantment.Id, ammoModel.Enchantment.Arguments)}\nTrigger Chance: {(ammoModel.Enchantment.TriggerChance >= 1f ? "Always" : $"{ammoModel.Enchantment.TriggerChance * 100}%")}\n\n{Archery.internalApi.GetEnchantmentDescription(ammoModel.Enchantment.Id, ammoModel.Enchantment.Arguments)}";
                        }

                        description = $"{description}\n\n+{ammoModel.Damage} Damage";
                    }
                }
            }

            return Game1.parseText(description, Game1.smallFont, System.Math.Max(272, (int)Game1.dialogueFont.MeasureString((description == null) ? "" : description).X));
        }
    }
}
