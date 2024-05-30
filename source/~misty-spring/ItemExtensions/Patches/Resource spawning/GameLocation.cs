/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ItemExtensions.Additions;
using ItemExtensions.Additions.Clumps;
using ItemExtensions.Events;
using ItemExtensions.Models;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.GameData.Locations;
using StardewValley.Internal;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace ItemExtensions.Patches;

public class GameLocationPatches
{
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif
    
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);
    internal static void Apply(Harmony harmony)
    {
        Log($"Applying Harmony patch \"{nameof(GameLocationPatches)}\": postfixing SDV method \"GameLocation.explode\".");
        
        harmony.Patch(
            original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.damageMonster), new[]{ typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(Farmer), typeof(bool)}),
            postfix: new HarmonyMethod(typeof(GameLocationPatches), nameof(Post_damageMonster))
        );
        
        Log($"Applying Harmony patch \"{nameof(GameLocationPatches)}\": postfixing SDV method \"GameLocation.spawnObjects\".");
        
        harmony.Patch(
            original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.spawnObjects)),
            postfix: new HarmonyMethod(typeof(GameLocationPatches), nameof(Post_spawnObjects))
        );
        
        Log($"Applying Harmony patch \"{nameof(GameLocationPatches)}\": transpiling SDV method \"GameLocation.spawnObjects\".");
        
        harmony.Patch(
            original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.spawnObjects)),
            transpiler: new HarmonyMethod(typeof(GameLocationPatches), nameof(Transpiler))
        );
    }

    /// <summary>
    /// Sort-of hacky way of damaging clumps. Postfix this specific damageMonster as it's called by explosion
    /// </summary>
    /// <param name="__instance"></param>
    /// <param name="areaOfEffect"></param>
    /// <param name="minDamage"></param>
    /// <param name="maxDamage">Max possible damage.</param>
    /// <param name="isBomb">Whether the damage comes from a bomb.</param>
    /// <param name="who"></param>
    /// <param name="isProjectile"></param>
    private static void Post_damageMonster(GameLocation __instance, Rectangle areaOfEffect, int minDamage, int maxDamage, bool isBomb,
        Farmer who, bool isProjectile = false)
    {
        if (isBomb == false)
            return;

        var dmg = Game1.random.Next(minDamage, maxDamage);
        
        if (dmg > 8)
            dmg /= 4;
        
        #if DEBUG
        Log($"Calling clump damage code with dmg {dmg}. Rectangle bounds: {areaOfEffect.X}, {areaOfEffect.Y}, {areaOfEffect.Width}, {areaOfEffect.Height}");
        #endif

        var realRect = new Rectangle(areaOfEffect.X / 64, areaOfEffect.Y / 64, areaOfEffect.Width / 64,
            areaOfEffect.Height / 64);
        CheckClumpDamage(__instance, realRect, dmg);
    }
    
    private static void CheckClumpDamage(GameLocation gameLocation, Rectangle rectangle, int damage_amount = -1)
    {
        var alreadyChecked = new List<ResourceClump>();
        var toRemove = new List<ResourceClump>();
        
        foreach (var clump in gameLocation.resourceClumps)
        {
            if(alreadyChecked.Contains(clump))
                continue;
#if DEBUG
            Log($"Checking clump at {clump.Tile}");
#endif
            if (rectangle.Contains(clump.Tile) == false)
            {
#if DEBUG
                Log($"Clump not in range.");
#endif
                continue;
            }
            
            if(clump.modData.TryGetValue(ModKeys.ClumpId, out var id) == false)
            {
#if DEBUG
                Log($"Clump has no id.");
#endif
                continue;
            }
            
            if (ModEntry.BigClumps.TryGetValue(id, out var resource) == false)
            {
#if DEBUG
                Log($"Clump ID not found in files.");
#endif
                continue;
            }

            if (resource.ImmuneToBombs)
            {
#if DEBUG
                Log($"Clump is immune to bombs.");
#endif
                continue;
            }

            clump.performToolAction(null, damage_amount, clump.Tile);

            if (clump.health.Value <= 0)
                toRemove.Add(clump);
            
            alreadyChecked.Add(clump);
        }

        foreach (var removal in toRemove)
        {
            var location = removal.Location;
            location.resourceClumps.Remove(removal);
        }
    }

    /// <summary>
    /// Checks if any spawned object is a custom resource. If so, applies data.
    /// </summary>
    /// <param name="__instance"></param>
    private static void Post_spawnObjects(GameLocation __instance)
    {
        try
        {
            Log($"Checking spawns made at {__instance.DisplayName ?? __instance.NameOrUniqueName}");

            foreach (var item in __instance.Objects.Values)
            {
                if (!ModEntry.Ores.TryGetValue(item.ItemId, out var resource))
                    continue;

                if (resource is null || resource == new ResourceData())
                    continue;

                Log($"Setting spawn data for {item.DisplayName}");

                World.SetSpawnData(item, resource);
            }
        }
        catch (Exception e)
        {
            Log($"Error: {e}", LogLevel.Error);
        }
    }
    
    /// <summary>
    /// Edits <see cref="GameLocation.spawnObjects"/>:
    /// Before trying to create a forage, this checks if it's a clump. If so, spawns and breaks (sub)loop.
    /// </summary>
    /// <param name="instructions">Original code.</param>
    /// <param name="il"></param>
    /// <returns>Edited code.</returns>
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
    {
        //new one
        var codes = new List<CodeInstruction>(instructions);
        var instructionsToInsert = new List<CodeInstruction>();

        var index = codes.FindIndex(ci => ci.opcode == OpCodes.Callvirt && ci.operand is MethodInfo { Name: "get_Chance"});
#if DEBUG
        Log($"index: {index}", LogLevel.Info);
#endif
        
        var redirectTo = codes.Find(ci => codes.IndexOf(ci) == index + 3);
        var breakAt = codes.Find(ci => ci.opcode == OpCodes.Ldloc_S && ((LocalBuilder)ci.operand).LocalIndex == 11);
        
        CodeInstruction forage = null;
        for (var i = 0; i < codes.Count - 1; i++)
        {
            if(codes[i].opcode != OpCodes.Ldloc_S)
                continue;
            
            if(codes[i + 1].opcode != OpCodes.Ldfld)
                continue;

            forage = codes[i + 1];
            break;
        }

        if (forage is null)
        {
            Log("Forage variable wasn't found.");
            return codes.AsEnumerable();
        }
        
        //add label for brfalse
        var brfalseLabel = il.DefineLabel();
        redirectTo.labels ??= new List<Label>();
        redirectTo.labels.Add(brfalseLabel);
        
        //add label for br_S
        var brSLabel = il.DefineLabel();
        breakAt.labels ??= new List<Label>();
        breakAt.labels.Add(brSLabel);
        
        if (index <= -1) 
            return codes.AsEnumerable();
        
        /* if (TryCustomClump(forage, context, vector2))
         * {
         *      ++this.numberOfSpawnedObjectsOnMap;
         *      break;
         * }
         */
        
        //arguments
        instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldloc_S, 13)); //spawndata arg
        instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldfld, forage.operand)); //load forage
        instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldloc_S, 10)); //context arg
        instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldloc_S, 16)); //position arg
        
        //call my code w/ prev args
        instructionsToInsert.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GameLocationPatches), nameof(CheckIfCustomClump))));

        //tell where to go if false
        instructionsToInsert.Add(new CodeInstruction(OpCodes.Brfalse, brfalseLabel));
        
        //if true: +spawnobj
        instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldarg, 0)); 
        instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldarg, 0)); 
        instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldfld, typeof(GameLocation).GetRuntimeField("numberOfSpawnedObjectsOnMap")));
        instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldc_I4_1));
        instructionsToInsert.Add(new CodeInstruction(OpCodes.Add));
        instructionsToInsert.Add(new CodeInstruction(OpCodes.Stfld, typeof(GameLocation).GetRuntimeField("numberOfSpawnedObjectsOnMap")));
        
        //break
        instructionsToInsert.Add(new CodeInstruction(OpCodes.Br_S, brSLabel));

        Log($"codes count: {codes.Count}, insert count: {instructionsToInsert.Count}");
        Log("Inserting method");
        codes.InsertRange(index + 3, instructionsToInsert);
        
        /* print the IL code
         * courtesy of atravita
         *
        StringBuilder sb = new();
        sb.Append("ILHelper for: GameLocation.spawnObjects");
        for (int i = 0; i < codes.Count; i++)
        {
            sb.AppendLine().Append(codes[i]);
            if (index + 3 == i)
            {
                sb.Append("       <---- start of transpiler");
            }
            if (index + 3 + instructionsToInsert.Count == i)
            {
                sb.Append("       <----- end of transpiler");
            }
        }
        Log(sb.ToString(), LogLevel.Info);
        */
        return codes.AsEnumerable();
    }

    /// <summary>
    /// Checks if a spawn is a clump, and tries to place if so.
    /// </summary>
    /// <param name="forage">The spawn data.</param>
    /// <param name="context">Location context.</param>
    /// <param name="vector2">Position to spawn at.</param>
    /// <returns>Whether the spawn was a clump.</returns>
    public static bool CheckIfCustomClump(SpawnForageData forage, ItemQueryContext context, Vector2 vector2)
    {
        #if DEBUG
        Log($"Called transpiled code for {forage?.Id}");
        #endif
        
        if (forage is null)
        {
            Log("parameter forage can't be null.");
            return false;
        }
        
        if (context is null)
        {
            Log("Context can't be null. Skipping");
            return false;
        }
        
        var random = context.Random ?? Game1.random;
        var randomItemId = forage.RandomItemId;

        //check validity
        var isOurs = forage.Id.StartsWith("ItemExtension.Clump", StringComparison.OrdinalIgnoreCase);

        if (!isOurs)
        {
            #if DEBUG
            Log("Doesn't seem to be a clump spawn");
            #endif
            return false;
        }

        #if DEBUG
        var logged = "BigClumps added:\n";
        foreach (var pair in ModEntry.BigClumps)
        {
            logged += $"                  * {pair.Key}\n";
        }
        Log(logged, LogLevel.Info);
        #endif
        
        var validItemId = ModEntry.BigClumps.TryGetValue(forage.ItemId, out _);
        
        List<string> randomsThatAreItem = new();
        List<string> randomsThatAreClump = new();

        //if not ours and doesn't start w/ custom query
        if (!validItemId)
        {
            if (forage.ItemId.StartsWith(ModEntry.Id, StringComparison.OrdinalIgnoreCase))
            {
                var id = ClumpQueries.Resolve(forage.ItemId, forage.PerItemCondition, context);

                if (!string.IsNullOrWhiteSpace(id) && ModEntry.BigClumps.ContainsKey(id))
                {
                    randomsThatAreClump.Add(id);
                    forage.ItemId = id;
                    validItemId = true;
                    #if DEBUG
                    Log($"Changed current itemId to solved query: {id}");
                    #endif
                }
            }
            else
            {
                Log($"Couldn't find clump Id '{forage.ItemId}' in data. ({context.Location?.NameOrUniqueName} @ {forage.Id})", LogLevel.Warn);
                return false;
            }
        }
        else
            randomsThatAreClump.Add(forage.Id);
        
        var isAnyRandomAClump = false;

        //if a random Id exists, consider in
        if (randomItemId != null && randomItemId.Any())
        {
            foreach (var randomId in randomItemId)
            {
                //if parsed id NOT in big clumps, assume item
                if (ModEntry.BigClumps.ContainsKey(randomId) == false)
                {
                    randomsThatAreItem.Add(randomId);
                    continue;
                }

                randomsThatAreClump.Add(randomId);
                isAnyRandomAClump = true;
            }
            
            //if no random is clump
            if (!isAnyRandomAClump)
            {
                if (string.IsNullOrWhiteSpace(forage?.ItemId))
                {
                    return false;
                }
                
                //prioritize clump
                TryPlaceCustomClump(forage.ItemId, context, vector2);
                return true;
            }
        }
        else if (randomItemId is null && validItemId)
        {
            #if DEBUG
            Log("No random items listed. Attempting to spawn by ItemId");
            #endif
            TryPlaceCustomClump(forage.ItemId, context, vector2);
            return true;
        }
        

        //chose randomly from all
        var all = new List<string>();
        all.AddRange(randomsThatAreItem);
        all.AddRange(randomsThatAreClump);

        if (all.Any() == false)
        {
            Log($"Found no valid clumps. {forage.Id}", LogLevel.Info);
            return true;    //because by this point it's an id, we return true anyway
        }
        
        var chosen = random.ChooseFrom(all);
        var placeItem = randomsThatAreItem.Contains(chosen);

        if (placeItem)
        {
            //try to get item
            var firstOrDefault = ItemQueryResolver.TryResolve(
                chosen, 
                context, 
                perItemCondition: forage.PerItemCondition, 
                maxItems: forage.MaxItems, 
                // ReSharper disable once AccessToModifiedClosure
                logError: (query, error) => { Log($"Location '{context.Location.NameOrUniqueName}' failed parsing item query '{query}' for forage '{chosen}': {error}"); }
                ).FirstOrDefault();
            
            //spawn if so
            if (firstOrDefault is not null)
            {
                //create
                var asItem = ItemQueryResolver.ApplyItemFields(firstOrDefault.Item, forage, context) as Item;
                if (asItem is Object o)
                {
                    o.IsSpawnedObject = true;
                    if (context.Location.dropObject(o, vector2 * 64f, Game1.viewport, true))
                    {
                        #if DEBUG
                        Log($"Placed forage randomly chosen: item {o.DisplayName} ({o.QualifiedItemId}");
                        #endif
                        return true;
                    }
                }
            }
        }

        //if an item was meant to be placed but couldn't (for x reason), choose clump
        if (placeItem)
        {
            chosen = Game1.random.ChooseFrom(randomsThatAreClump);
        }
        
        TryPlaceCustomClump(chosen, context, vector2);
        return true;
    }

    /// <summary>
    /// Attempts to place a custom clump.
    /// </summary>
    /// <param name="clumpId">Id to get data from.</param>
    /// <param name="context">Query context.</param>
    /// <param name="position">Position to place at.</param>
    private static void TryPlaceCustomClump(string clumpId, ItemQueryContext context, Vector2 position)
    {
        #if DEBUG
        Log("Placing clump...");
        #endif

        var clump = ExtensionClump.Create(clumpId, position);
        
        if (clump is null)
            return;
        
        var cf = context.Location.GetData().CustomFields;

        if (cf is not null && cf.Any())
        {
            var hasRect = cf.TryGetValue(ModKeys.SpawnRect, out var rawRect);
                
            //default true, but can be set off, idk why
            var avoidOverlap = true;
            if (cf.TryGetValue(ModKeys.AvoidOverlap, out var overlap))
                avoidOverlap = bool.Parse(overlap);

            if (hasRect && !string.IsNullOrWhiteSpace(rawRect))
            {
                var newPosition = CheckPosition(context, position, rawRect, avoidOverlap);
                clump.Tile = newPosition;
            }
        }

        if (context.Location?.GetData()?.CustomFields != null && context.Location.GetData().CustomFields.TryGetValue(ModKeys.ClumpRemovalDays, out _))
        {
            clump.modData.Add(ModKeys.Days, "0");
        }
            
        context.Location?.resourceClumps?.Add(clump);
    }

    /// <summary>
    /// Fixes position for spawn.
    /// This might be heavy on resources, so it's recommended to just use FTM instead.
    /// </summary>
    /// <param name="context">Spawn context.</param>
    /// <param name="defaultPosition">Current position.</param>
    /// <param name="rawRect">Spawn zone, unparsed.</param>
    /// <param name="avoidOverlap">If to avoid placing on a tile with content.</param>
    /// <returns></returns>
    private static Vector2 CheckPosition(ItemQueryContext context, Vector2 defaultPosition, string rawRect, bool avoidOverlap)
    {
        if (string.IsNullOrWhiteSpace(rawRect))
            return defaultPosition;
        
        try
        {
            var result = defaultPosition;

            //can either be "x y w h" for single one, or for multiple "\"x y w h\" \"x y w h\""
            var split = ArgUtility.SplitBySpaceQuoteAware(rawRect);
            
            if (split is null || split.Length < 1)
                return defaultPosition;
            
            var rects = new List<Rectangle>();
            //if multiple, parse each. otherwise parse single one
            if (split[0].Contains(' ')) // it'd only have a space if there's quote split
            {
                foreach (var raw in split)
                {
                    if (string.IsNullOrWhiteSpace(raw))
                        return defaultPosition;
                    
                    var args = ArgUtility.SplitBySpace(raw);
            
                    if (args is null || args.Length < 4)
                        return defaultPosition;
                    
                    rects.Add(new Rectangle(int.Parse(args[0]), int.Parse(args[1]), int.Parse(args[2]), int.Parse(args[3])));
                }
            }
            else
            {
                if (split.Length < 4)
                    return defaultPosition;
                
                rects.Add(new Rectangle(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]), int.Parse(split[3])));
            }

            if (rects.Count <= 0 || rects.Any() == false)
                return defaultPosition;
            
            //if point isn't in allowed rect, set to a random point in any
            if (rects.Any(r => r.Contains(defaultPosition)))
                return result;

            var random = context.Random ?? Game1.random;
            var randomRect = random.ChooseFrom(rects);

            if (!avoidOverlap)
            {
                result = new Vector2(
                    random.Next(randomRect.X, randomRect.X + randomRect.Width),
                    random.Next(randomRect.Y, randomRect.Y + randomRect.Height));
            }
            else
            {
                for (var i = 0; i < 30; i++)
                {
                    result = new Vector2(
                        random.Next(randomRect.X, randomRect.X + randomRect.Width),
                        random.Next(randomRect.Y, randomRect.Y + randomRect.Height));

                    var cantSpawn = context.Location.IsTileOccupiedBy(result) ||
                                    context.Location.IsNoSpawnTile(result) ||
                                    !context.Location.CanItemBePlacedHere(result);

                    if (cantSpawn == false)
                        break;
                }
            }

            return result;
        }
        catch (Exception e)
        {
            Log($"Error: {e}.\n     Will use original position", LogLevel.Warn);
            return defaultPosition;
        }
    }
}