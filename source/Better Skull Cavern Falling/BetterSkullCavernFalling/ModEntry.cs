/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/maxvollmer/BetterSkullCavernFalling
**
*************************************************/

using Harmony;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Reflection;

namespace BetterSkullCavernFalling
{
    public class ModEntry : Mod
    {
        private static HarmonyInstance harmonyInstance = null;

        private class MineShaftEnterMineShaftPatch
        {
            internal static bool enterMineShaft_Prefix(MineShaft __instance)
            {
                DelayedAction.playSoundAfterDelay("fallDown", 0, null, -1);
                DelayedAction.playSoundAfterDelay("clubSmash", 1000, null, -1);

                Random random = new Random(__instance.mineLevel + (int)Game1.uniqueIDForThisGame + Game1.Date.TotalDays);
                int levelsFallen = random.Next(3, 9);
                if (random.NextDouble() < 0.1)
                    levelsFallen = levelsFallen * 2 - 1;
                if (__instance.mineLevel < 220 && __instance.mineLevel + levelsFallen > 220)
                    levelsFallen = 220 - __instance.mineLevel;

                typeof(MineShaft).GetField("lastLevelsDownFallen", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, levelsFallen);

                Game1.enterMine(__instance.mineLevel + levelsFallen);

                string message = Game1.content.LoadString(levelsFallen > 7 ? "Strings\\Locations:Mines_FallenFar" : "Strings\\Locations:Mines_Fallen", levelsFallen);
                Game1.addHUDMessage(new HUDMessage(message) { noIcon = true });

                Game1.player.health = Math.Max(1, Game1.player.health - levelsFallen * 3);
                Game1.player.jump();
                Game1.player.faceDirection(2);
                Game1.player.showFrame(5, false);

                return false;
            }
        }

        public override void Entry(IModHelper helper)
        {
            harmonyInstance = HarmonyInstance.Create(this.ModManifest.UniqueID);
            harmonyInstance.Patch(
               original: AccessTools.Method(typeof(MineShaft), nameof(MineShaft.enterMineShaft)),
               prefix: new HarmonyMethod(typeof(MineShaftEnterMineShaftPatch), nameof(MineShaftEnterMineShaftPatch.enterMineShaft_Prefix))
            );
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && harmonyInstance != null)
            {
                harmonyInstance.UnpatchAll();
                harmonyInstance = null;
            }
        }
    }
}
