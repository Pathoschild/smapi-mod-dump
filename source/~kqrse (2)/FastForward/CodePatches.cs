/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using HarmonyLib;
using StardewValley;

namespace FastForward
{
    public partial class ModEntry
    {
        private static int count;
        private static void SGame_Update_Postfix(object __instance)
        {
            if (!Config.ModEnabled)
                return;
            if (count <= Config.SpeedMult && SHelper.Input.IsDown(Config.ModKey))
            {
                count++;
                Game1.currentGameTime = new Microsoft.Xna.Framework.GameTime(Game1.currentGameTime.TotalGameTime + Game1.currentGameTime.ElapsedGameTime, Game1.currentGameTime.ElapsedGameTime);
                //SMonitor.Log(Game1.currentGameTime.TotalGameTime + " " + Game1.currentGameTime.ElapsedGameTime);
                AccessTools.Method("StardewModdingAPI.Framework.SGame:Update").Invoke(__instance, new object[] { Game1.currentGameTime });
            }
            else
            {
                count = 0;
            }
        }
    }
}