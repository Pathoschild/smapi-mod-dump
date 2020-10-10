/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using Harmony;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.TerrainFeatures;

namespace BorderlessWoodFloorMod
{
    public class BorderlessWoodFloorModEntry : Mod
    {
        internal static IModHelper ModHelper;

        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            DataLoader dataLoader = new DataLoader(ModHelper);
            Helper.Content.AssetEditors.Add((IAssetEditor) dataLoader);

            var harmony = HarmonyInstance.Create("Digus.CustomFlooringMod");
            harmony.Patch(
                original: AccessTools.Method(typeof(Flooring), nameof(Flooring.loadSprite)),
                postfix: new HarmonyMethod(typeof(BorderlessWoodFloorModEntry), nameof(BorderlessWoodFloorModEntry.loadSprite))
            );

            harmony.Patch(
                original: AccessTools.Constructor(typeof(Flooring), new Type[] { typeof(int) }),
                postfix: new HarmonyMethod(typeof(BorderlessWoodFloorModEntry), nameof(BorderlessWoodFloorModEntry.FlooringConstructor))
            );
        }

        private static void FlooringConstructor(Flooring __instance, int which)
        {
            if (which == 0)
            {
                ModHelper.Reflection.GetField<NetBool>(__instance, "isPathway").GetValue().Value = true;
            }
        }

        private static void loadSprite(Flooring __instance)
        {
            if (__instance.whichFloor.Value == 0)
            {
                ModHelper.Reflection.GetField<NetBool>(__instance, "isPathway").GetValue().Value = true;
            }
        }
    }
}
