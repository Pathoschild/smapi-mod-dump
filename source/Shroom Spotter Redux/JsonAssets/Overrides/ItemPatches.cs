/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using JsonAssets.Data;
using SpaceShared;
using StardewValley;
using StardewValley.Tools;

namespace JsonAssets.Overrides
{
    public static class ItemPatches
    {
        public static void CanBeDropped_Postfix(Item __instance, ref bool __result)
        {
            try
            {
                if (__instance is StardewValley.Object obj)
                {
                    if (!obj.bigCraftable.Value && Mod.instance.objectIds.Values.Contains(obj.ParentSheetIndex))
                    {
                        var objData = new List<ObjectData>(Mod.instance.objects).Find(od => od.GetObjectId() == obj.ParentSheetIndex);
                        if (objData != null && !objData.CanTrash)
                            __result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.error($"Failed in {nameof(CanBeDropped_Postfix)} for {__instance} #{__instance?.ParentSheetIndex} {__instance?.Name}:\n{ex}");
            }
        }

        public static void CanBeTrashed_Postfix(Item __instance, ref bool __result)
        {
            try
            {
                if ( __instance is StardewValley.Object obj )
                {
                    if (!obj.bigCraftable.Value && Mod.instance.objectIds.Values.Contains(obj.ParentSheetIndex))
                    {
                        var objData = new List<ObjectData>(Mod.instance.objects).Find(od => od.GetObjectId() == obj.ParentSheetIndex);
                        if (objData != null && !objData.CanTrash)
                            __result = false;
                    }
                }
                else if ( __instance is MeleeWeapon weapon )
                {
                    if (Mod.instance.weaponIds.Values.Contains(weapon.ParentSheetIndex))
                    {
                        var weaponData = new List<WeaponData>(Mod.instance.weapons).Find(wd => wd.GetWeaponId() == weapon.ParentSheetIndex);
                        if (weaponData != null && !weaponData.CanTrash)
                            __result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.error($"Failed in {nameof(CanBeTrashed_Postfix)} for {__instance} #{__instance?.ParentSheetIndex} {__instance?.Name}:\n{ex}");
            }
        }
    }
}
