/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/barteke22/StardewMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;

namespace DiagonalAim
{
    public class ModEntry : Mod
    {
        public static ModConfig config;

        public override void Entry(IModHelper helper)
        {
            Log.Monitor = Monitor;
            config = Helper.ReadConfig<ModConfig>();

            helper.Events.Input.ButtonPressed += Input_ButtonPressed;


            try
            {
                var harmony = new Harmony(ModManifest.UniqueID); //this might summon Cthulhu
                //harmony.Patch(
                //    original: AccessTools.Method(typeof(Character), "GetToolLocation", new Type[] { typeof(Vector2), typeof(bool) }),
                //    postfix: new HarmonyMethod(typeof(HarmonyPatches), "GetToolLocation_Diagonals")
                //);
                harmony.Patch(
                    original: AccessTools.Method(typeof(Character), nameof(Character.GetToolLocation), new Type[] { typeof(Vector2), typeof(bool) }),
                    postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.GetToolLocation_Diagonals2))
                );
            }
            catch (Exception e)
            {
                Log.Error("Wat: " + e.Message);
            }
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == SButton.F5)
            {
                config = Helper.ReadConfig<ModConfig>();
            }
            if (config.ModToggleKey.GetState().IsDown()) HarmonyPatches.toggleOff = !HarmonyPatches.toggleOff;
        }
    }
}
