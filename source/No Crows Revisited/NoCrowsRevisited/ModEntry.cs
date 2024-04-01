/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/siweipancc/StardewMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace NoCrowsRevisited;

public class ModEntry : Mod
{
    private static IMonitor? _modMonitor;

    public override void Entry(IModHelper helper)
    {
        _modMonitor = Monitor;
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        var harmony = new Harmony("Siweipancc.NoCrowsRevisited");
        Type type;
        try
        {
            type = GetRunTimeType("Farm");
        }
        catch (Exception exception)
        {
            Monitor.Log(exception.Message, LogLevel.Error);
            throw;
        }

        harmony.Patch(
            AccessTools.Method(type, "addCrows"),
            new HarmonyMethod(typeof(FarmPatch), nameof(FarmPatch.addCrows)));
        harmony.Patch(
            AccessTools.Method(type, "doSpawnCrow", new[] { typeof(Vector2) }),
            new HarmonyMethod(typeof(FarmPatch), nameof(FarmPatch.doSpawnCrow)));
    }


    private static Type GetRunTimeType(string subPath)
    {
        Type? type = Type.GetType($"StardewValley.{subPath}, Stardew Valley");
        if (type == null)
        {
            type = Type.GetType($"StardewValley.{subPath}, StardewValley");
        }

        return type ?? throw new SystemException($"cannot find type: {subPath}");
    }

    private static class FarmPatch
    {
        // ReSharper disable once InconsistentNaming
        public static bool addCrows()
        {
            _modMonitor?.Log("skip addCrows in farm");
            return false;
        }

        // ReSharper disable once InconsistentNaming
        public static bool doSpawnCrow()
        {
            _modMonitor?.Log("skip doSpawnCrow in farm");
            return false;
        }
    }
}