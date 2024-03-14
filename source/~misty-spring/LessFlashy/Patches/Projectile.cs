/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley.Projectiles;

namespace LessFlashy.Patches;

public static class ProjectilePatches
{
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif
    
    private static IModHelper Helper => ModEntry.Help;
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Log(msg, lv);

    internal static void Apply(Harmony harmony)
    {
        Log($"Applying Harmony patch \"{nameof(ProjectilePatches)}\": postfixing SDV method \"Projectile.InitNetFields\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(Projectile), "InitNetFields"),
            postfix: new HarmonyMethod(typeof(ProjectilePatches), nameof(Post_InitNetFields))
        );
    }

    private static void Post_InitNetFields(Projectile __instance)
    {
        try
        {
            __instance.rotationVelocity.Set(0f);
            __instance.startingRotation.Set(0f);
            var rotation = Helper.Reflection.GetField<float>(__instance, "rotation");
            rotation.SetValue(0f);
        }
        catch (Exception e)
        {
            Log($"Error: {e}", LogLevel.Error);
        }
    }
}