/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KirbyLink/UnderdarkSewer
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

namespace UnderdarkSewer
{
    
    public class ModEntry : Mod
    {
        
        public override void Entry(IModHelper helper)
        {
            var harmony = HarmonyInstance.Create("com.github.kirbylink.underdarksewer");
            var locOriginal = typeof(GameLocation).GetMethod("drawWater");
            var drawPrefix = helper.Reflection.GetMethod(typeof(LocationFix), "Prefix").MethodInfo;
            harmony.Patch(locOriginal, new HarmonyMethod(drawPrefix), null);
        }
    }

    public static class LocationFix
    {
        static void Prefix(GameLocation __instance)
        {
            if (__instance is Sewer)
            {
                __instance.waterColor.Value = new Color(255, 150, 255);

                var steamColorField = AccessTools.Field(typeof(Sewer), "steamColor");
                steamColorField.SetValue(__instance, new Color(255, 200, 255));  
            }
        }
    }
}