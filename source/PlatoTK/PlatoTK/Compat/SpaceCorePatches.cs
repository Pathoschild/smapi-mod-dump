/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;

namespace PlatoTK.Compat
{
    internal static class SpaceCorePatches
    {

        public static void PatchSpaceCore(IModHelper helper, Harmony instance)
        {
            if (!helper.ModRegistry.IsLoaded("spacechase0.SpaceCore"))
                return;

            Type TileSheetExtensions = Type.GetType("SpaceCore.TileSheetExtensions, SpaceCore");

            if (TileSheetExtensions != null)
            {
                instance.Patch(
                    original: AccessTools.Method(TileSheetExtensions,nameof(GetTileSheet)),
                    postfix: new HarmonyMethod(AccessTools.Method(typeof(SpaceCorePatches), nameof(GetTileSheet))));
            }
        }

        public static void GetTileSheet(ref Texture2D __result, Texture2D tex, int index)
        {
            if (index != 0)
                __result.Name = "Extended:" + tex.Name + ":" + index;
        }
    }
}
