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
            original: AccessTools.Method(typeof(CharacterCustomization), "ResetComponents"),
            postfix: new HarmonyMethod(typeof(Patches), nameof(CharacterCustomization_ResetComponents__Postfix))
        );
    }

    private static void CharacterCustomization_ResetComponents__Postfix(CharacterCustomization __instance)
    {
        var farmnameBox = typeof(CharacterCustomization).GetField("farmnameBox", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(__instance) as TextBox;
        farmnameBox.limitWidth = false;
    }
}