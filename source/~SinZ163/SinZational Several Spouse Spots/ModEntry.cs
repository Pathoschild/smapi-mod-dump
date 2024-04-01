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
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Extensions;
using StardewValley.GameData.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace SeveralSpouseSpots;

public class ModEntry : Mod
{
    public const string SpousePatioBuildingId = "SinZ.SpousePatio";
    public const string SpousePatioAllocationKey = "SinZ.SpousePatio.Allocation";

    public static ModEntry Instance;

    public static Dictionary<string, Building> SpousePatioAllocations { get; } = new();
    public static Dictionary<string, Vector2> SpousePatioStandingSpots { get; } = new();

    public override void Entry(IModHelper helper)
    {
        Instance = this;
        var harmony = new Harmony(ModManifest.UniqueID);
        HarmonyPatches.Setup(Monitor, harmony);
        helper.Events.Content.AssetRequested += Content_AssetRequested;
        helper.Events.World.BuildingListChanged += World_BuildingListChanged;
    }

    public void RecalculateSpousePatioAllocations()
    {
        var spousePatioCount = 0;
        SpousePatioAllocations.Clear();
        SpousePatioStandingSpots.Clear();
        var spouses = new HashSet<string>();
        foreach (var farmer in Game1.getAllFarmers())
        {
            foreach (var npc in farmer.friendshipData.Keys)
            {
                if (farmer.friendshipData[npc].IsMarried())
                {
                    spouses.Add(npc);
                }
            }
        }
        // MasterPlayer spouse is handled via vanilla logic, let it stand.
        if (Game1.MasterPlayer.spouse != null)
        {
            spouses.Remove(Game1.MasterPlayer.spouse);
        }

        var unallocatedPatios = new List<Building>();
        var unallocatedSpouses = new List<string>(spouses);

        Utility.ForEachBuilding(b =>
        {
            if (b.buildingType.Value != SpousePatioBuildingId) return true;

            spousePatioCount++;
            if (b.modData.TryGetValue(SpousePatioAllocationKey, out string allocation))
            {
                SpousePatioAllocations[allocation] = b;
                unallocatedSpouses.Remove(allocation);
            }
            else
            {
                unallocatedPatios.Add(b);
            }
            if (SpousePatioAllocations.Count == spouses.Count()) return false;
            return true;
        });
        while (unallocatedPatios.Count > 0 && unallocatedSpouses.Count > 0)
        {
            var patio = unallocatedPatios.First();
            unallocatedPatios.RemoveAt(0);
            var spouse = unallocatedSpouses.First();
            unallocatedSpouses.RemoveAt(0);
            patio.modData[SpousePatioAllocationKey] = spouse;
            SpousePatioAllocations[spouse] = patio;
        }
        // Force Farm to be reloaded
        Monitor.Log("Invalidating Farm: " + Game1.getFarm().Map.assetPath);
        Helper.GameContent.InvalidateCache(Game1.getFarm().Map.assetPath);
    }

    public void ApplyMapModifications()
    {
        // Code is based on Farm.addSpouseOutdoorArea
        foreach (var (spouse, building) in SpousePatioAllocations)
        {
            var npc = Game1.getCharacterFromName(spouse, true);
            if (npc == null) continue;
            var patioData = npc.GetData()?.SpousePatio;
            if (patioData == null) continue;

            string assetName = patioData.MapAsset ?? "spousePatios";
            Rectangle sourceArea = patioData.MapSourceRect;
            int width = Math.Min(sourceArea.Width, 4);
            int height = Math.Min(sourceArea.Height, 4);
            Rectangle areaToRefurbish = new Rectangle(building.tileX.Value, building.tileY.Value - 2, width, height);
            var location = building.GetParentLocation();
            location.ApplyMapOverride(assetName, "SinZ.SpousePatios_" + spouse, new Rectangle(sourceArea.Location.X, sourceArea.Location.Y, areaToRefurbish.Width, areaToRefurbish.Height), areaToRefurbish);
            foreach (Point tile in areaToRefurbish.GetPoints())
            {
                if (location.getTileIndexAt(tile, "Paths") == 7)
                {
                    SpousePatioStandingSpots[spouse] = new Vector2(tile.X, tile.Y);
                    // Reload the patio activity to the new location
                    if (npc.shouldPlaySpousePatioAnimation.Value)
                    {
                        npc.setUpForOutdoorPatioActivity();
                    }
                    break;
                }
            }
        }
    }

    private void World_BuildingListChanged(object sender, BuildingListChangedEventArgs e)
    {
        var addedSpousePatios = e.Added.Where(b => b.buildingType.Value == SpousePatioBuildingId);
        var removedSpousePatios = e.Removed.Where(b => b.buildingType.Value == SpousePatioBuildingId);
        if (addedSpousePatios.Any() || removedSpousePatios.Any())
        {
            RecalculateSpousePatioAllocations();
        }
    }

    private void Content_AssetRequested(object sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo("Data/Buildings"))
        {
            e.Edit(data =>
            {
                var dict = data.AsDictionary<string, BuildingData>();
                dict.Data[SpousePatioBuildingId] = new()
                {
                    Name = "Spouse Patio", // TODO: i18n?
                    Description = "Spouse Patio", // TODO: i18n?

                    Builder = "Robin",
                    BuildDays = 0,
                    // TODO: Test this actually works
                    BuildCondition = "LOCATION_NAME Target Farm",
                    BuildCost = 500,
                    BuildMaterials = new()
                    {
                        new()
                        {
                            ItemId = "(O)388",
                            Amount = 20
                        }
                    },

                    Texture = Helper.ModContent.GetInternalAssetName("assets/baseSpousePatio.png").Name,
                    Size = new()
                    {
                        X = 4,
                        Y = 2
                    },
                    FadeWhenBehind = false,
                    DrawShadow = false,
                };
            }, AssetEditPriority.Default - 1);
        }
    }
}

public static class HarmonyPatches
{
    private static IMonitor monitor;
    public static void Setup(IMonitor monitor, Harmony harmony)
    {
        HarmonyPatches.monitor = monitor;
        var marriageDuties = typeof(NPC).GetMethod(nameof(NPC.marriageDuties));
        if (marriageDuties != null)
        {
            harmony.Patch(marriageDuties, transpiler: new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(marriageDuties__Transpiler))));
        }
        var getSpousePatioPosition = typeof(NPC).GetMethod(nameof(NPC.GetSpousePatioPosition));
        if (getSpousePatioPosition != null)
        {
            harmony.Patch(getSpousePatioPosition, postfix: new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(GetSpousePatioPosition__Postfix))));
        }
        var performActionOnBuildingPlacement = AccessTools.Method(typeof(Building), nameof(Building.performActionOnBuildingPlacement));
        if (performActionOnBuildingPlacement != null)
        {
            harmony.Patch(performActionOnBuildingPlacement, postfix: new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(performActionOnBuildingPlacement__Postfix))));
        }
        harmony.Patch(AccessTools.Method(typeof(Building), nameof(Building.isTilePassable)), prefix: new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(isTilePassable__Prefix))));
        harmony.Patch(AccessTools.Method(typeof(Building), nameof(Building.draw)), prefix: new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(Draw__prefix))));
        harmony.Patch(AccessTools.Method(typeof(Farm), nameof(Farm.OnMapLoad)), postfix: new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(OnMapLoad__Postfix))));
        harmony.Patch(AccessTools.Method(typeof(Farmer), nameof(Farmer.doDivorce)), prefix: new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(doDivorce__Prefix))), postfix: new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(doDivorce__Postfix))));
        harmony.Patch(AccessTools.Method(typeof(Game1), nameof(Game1.OnDayStarted)), prefix: new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(OnDayStarted__Prefix))));

        var polyNPCPatch = AccessTools.Method("PolyamorySweetLove.NPCPatches:NPC_setUpForOutdoorPatioActivity_Prefix");
        if (polyNPCPatch != null)
        {
            harmony.Patch(polyNPCPatch, transpiler: new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(PolyNPC__NullifyNPCPatch))));
        }
    }

    public static IEnumerable<CodeInstruction> PolyNPC__NullifyNPCPatch(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
    {
        monitor.Log("Gutting PolyamorySweetLove.NPCPatches:NPC_setUpForOutdoorPatioActivity_Prefix");
        return new CodeInstruction[]
        {
            new CodeInstruction(OpCodes.Ldc_I4_1),
            new CodeInstruction(OpCodes.Ret)
        };
    }

    /// <summary>
    /// Harmony patch to make allocated patios passable, due to the lack of conditional CollisionMaps
    /// </summary>
    /// <param name="__instance"></param>
    /// <param name="__result"></param>
    /// <returns></returns>
    public static bool isTilePassable__Prefix(Building __instance, ref bool __result)
    {
        if (__instance.buildingType.Value == ModEntry.SpousePatioBuildingId && __instance.modData.ContainsKey(ModEntry.SpousePatioAllocationKey))
        {
            __result = true;
            return false;
        }
        return true;
    }

    public static bool Draw__prefix(Building __instance)
    {
        // Override drawing if its my building and allocated
        if (__instance.buildingType.Value == ModEntry.SpousePatioBuildingId && __instance.modData.ContainsKey(ModEntry.SpousePatioAllocationKey))
        {
            return false;
        }
        return true;
    }

    public static void doDivorce__Prefix(Farmer __instance, out string? __state)
    {
        // Set state if the farmer is married to an npc, not roommates
        __state = null;
        if (__instance.spouse != null && __instance.friendshipData.TryGetValue(__instance.spouse, out var friendship) && friendship.IsMarried() && !friendship.IsRoommate())
        {
            __state = __instance.spouse;
        }
    }
    public static void doDivorce__Postfix(string? __state)
    {
        if (__state != null)
        {
            if (ModEntry.SpousePatioAllocations.TryGetValue(__state, out var patio))
            {
                patio.modData.Remove(ModEntry.SpousePatioAllocationKey);
            }
            ModEntry.Instance.RecalculateSpousePatioAllocations();
        }
    }

    public static void OnMapLoad__Postfix()
    {
        ModEntry.Instance.ApplyMapModifications();
    }

    public static void performActionOnBuildingPlacement__Postfix(Building __instance)
    {
        if (__instance.buildingType.Value == ModEntry.SpousePatioBuildingId)
        {
            ModEntry.Instance.RecalculateSpousePatioAllocations();
        }
    }

    /// <summary>
    /// Marriage Duties run <b>before</b> game loop DayStarted, so prefix hooking into Game1.OnDayStarted which eventually calls marriage duties
    /// </summary>
    public static void OnDayStarted__Prefix()
    {
        ModEntry.Instance.RecalculateSpousePatioAllocations();
    }

    public static void GetSpousePatioPosition__Postfix(NPC __instance, ref Vector2 __result)
    {
        if (ModEntry.SpousePatioStandingSpots.TryGetValue(__instance.Name, out var spot))
        {
            __result = spot;
        }
    }

    public static IEnumerable<CodeInstruction> marriageDuties__Transpiler(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
    {
        var output = new List<CodeInstruction>();
        var huntBNE = false;
        foreach (var instruction in instructions)
        {
            /*
             * 	IL_03ea: ldloc.0
	         *  IL_03eb: call class StardewValley.Farmer StardewValley.Game1::get_MasterPlayer()
	         *  IL_03f0: bne.un.s IL_040b
             */
            if (instruction.opcode == OpCodes.Call && instruction.operand is MethodInfo operandMethod && operandMethod.Name == "get_MasterPlayer")
            {
                // remove ldloc.0 and replace it with Ldarg_0 (this)
                output.RemoveAt(output.Count - 1);
                output.Add(new CodeInstruction(OpCodes.Ldarg_0));
                output.Add(new CodeInstruction(OpCodes.Call, typeof(HarmonyPatches).GetMethod(nameof(IsValidSpousePatio))));
                huntBNE = true;
                continue;
            }
            // We are no longer bne.un.s, and doing brfalse.s instead
            if (huntBNE && instruction.opcode == OpCodes.Bne_Un_S)
            {
                huntBNE = false;
                output.Add(new CodeInstruction(OpCodes.Brfalse_S, instruction.operand));
                continue;
            }
            output.Add(instruction);
        }
        return output;
    }
    /// <summary>
    /// This returns the spouses name if its valid.
    /// This is a harmony swap out of a call to Game1.MasterPlayer from NPC.marriageDuties but returns 
    /// </summary>
    /// <param name="spouse"></param>
    /// <returns></returns>
    public static bool IsValidSpousePatio(NPC npc)
    {
        if (Game1.MasterPlayer.spouse == npc.Name) return true;
        if (ModEntry.SpousePatioStandingSpots.ContainsKey(npc.Name)) return true;
        return false;
    }
}