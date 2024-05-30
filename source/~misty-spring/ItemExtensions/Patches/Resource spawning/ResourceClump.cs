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
using ItemExtensions.Additions;
using ItemExtensions.Additions.Clumps;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace ItemExtensions.Patches;

public class ResourceClumpPatches
{
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif
    
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);

    internal static void Apply(Harmony harmony)
    {
        Log($"Applying Harmony patch \"{nameof(ResourceClumpPatches)}\": postfixing SDV method \"ResourceClump.OnAddedToLocation\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(TerrainFeature), nameof(TerrainFeature.OnAddedToLocation)),
            postfix: new HarmonyMethod(typeof(ResourceClumpPatches), nameof(Post_OnAddedToLocation))
        );
        
        Log($"Applying Harmony patch \"{nameof(ResourceClumpPatches)}\": prefixing SDV method \"ResourceClump.performToolAction\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(ResourceClump), nameof(ResourceClump.performToolAction)),
            prefix: new HarmonyMethod(typeof(ResourceClumpPatches), nameof(Pre_performToolAction))
        );
    }
    
    public static void Post_OnAddedToLocation(TerrainFeature __instance, GameLocation location, Vector2 tile)
    {
        try
        {
            if (__instance is not ResourceClump r)
                return;

            //if no custom id
            if (r.modData.TryGetValue(ModKeys.ClumpId, out var id) is false)
                return;

            //if it has a light Id, assume there's one placed
            if (r.modData.TryGetValue(ModKeys.LightId, out _))
                return;

            //try get light data
            if (r.modData.TryGetValue(ModKeys.LightSize, out var sizeRaw) == false ||
                r.modData.TryGetValue(ModKeys.LightColor, out var rgb) == false ||
                r.modData.TryGetValue(ModKeys.LightTransparency, out var transRaw) == false)
            {
#if DEBUG
                ModEntry.Mon.VerboseLog($"Data for {id} light not found. (onAddedToLocation)");
#endif
                return;
            }

            if (float.TryParse(sizeRaw, out var size) == false)
            {
                Log($"Couldn't parse light size for clump Id {id} ({sizeRaw})");
                return;
            }

            if (float.TryParse(transRaw, out var trans) == false)
            {
                Log($"Couldn't parse transparency for clump Id {id} ({sizeRaw})");
                return;
            }

            //parse
            Color color;
            if (rgb.Contains(' ') == false)
            {
                color = Utility.StringToColor(rgb) ?? Color.White;
            }
            else
            {
                var rgbs = ArgUtility.SplitBySpace(rgb);
                var parsed = rgbs.Select(int.Parse).ToList();
                color = new Color(parsed[0], parsed[1], parsed[2]);
            }

            color *= trans;

            //set
            var fixedPosition = new Vector2(tile.X + r.width.Value / 2, tile.Y * r.height.Value / 2);
            var lightSource = new LightSource(4, fixedPosition, size, color);

            r.modData.Add(ModKeys.LightId, $"{lightSource.Identifier}");
        }
        catch (Exception e)
        {
            Log($"Error: {e}",LogLevel.Error);
        }
    }
    
    //the transpiler would return anyway, so we make it a prefix
    public static bool Pre_performToolAction(ref ResourceClump __instance, Tool t, int damage, Vector2 tileLocation,
        ref bool __result)
    {
        try
        {
            if (ExtensionClump.IsCustom(__instance) == false)
                return true;

            __result = ExtensionClump.DoCustom(ref __instance, t, damage, tileLocation);
            return false;
        }
        catch (Exception e)
        {
            Log($"Error: {e}");
            return true;
        }
    }
}