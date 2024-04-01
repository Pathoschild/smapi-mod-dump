/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SinZ163/StardewMods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using System.Reflection;

public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        helper.Events.Input.ButtonReleased += (s, e) =>
        {
            if (!Context.IsWorldReady) return;
            if (Game1.activeClickableMenu != null) return;
            if (e.Button == SButton.F6)
            {
                var manor = Game1.getLocationFromName("ManorHouse") as ManorHouse;
                manor!.ChooseRecipient();
            }
        };
        var harmony = new Harmony(ModManifest.UniqueID);
        Patches.Initialize(Monitor, harmony);
    }
}
public static class Patches
{
    private static IMonitor Monitor;
    public static void Initialize(IMonitor monitor, Harmony harmony)
    {
        Monitor = monitor;
        harmony.Patch(
            original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.answerDialogue)),
            prefix: new HarmonyMethod(typeof(Patches), nameof(GameLocation_answerDialogue__Prefix))
        );
    }

    public static bool GameLocation_answerDialogue__Prefix(GameLocation __instance, Response answer)
    {
        if (__instance is ManorHouse)
        {
            return true;
        }
        var manor = Game1.getLocationFromName("ManorHouse") as ManorHouse;
        var sendMoneyMapping = typeof(ManorHouse).GetField("sendMoneyMapping", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(manor) as Dictionary<string, Farmer>;
        if (answer.responseKey.StartsWith("Transfer") && sendMoneyMapping!.Count > 0)
        {
            // This would be set on the current location, not the Manor instance, its benign being left on the old Location instance
            manor.lastQuestionKey = "chooseRecipient";
            manor.answerDialogue(answer);
            return false;
        }
        return true;
    }
}