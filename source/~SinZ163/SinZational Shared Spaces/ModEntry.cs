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
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.Linq;
using xTile.Dimensions;
using static StardewValley.Minigames.MineCart;

public static class Constants
{

    public const string ElevatorTileAction = "SinZ.SharedSpaces_ElevatorButton";
    public const string SelectedFloor = "SinZ.SharedSpaces_SelectedFloor";
}

public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        helper.Events.Content.AssetRequested += Content_AssetRequested;

        GameLocation.RegisterTileAction(Constants.ElevatorTileAction, onElevatorPress);

        var harmony = new Harmony(ModManifest.UniqueID);
        Patches.Init(harmony, helper, Monitor);
        /**
         * TODO:
         * 
         * Allow stable buildings up to player instance count
         * 
         * Fix spouse standing spot to be building like Several Spouse Spots
         * 
         * Prevent cabins spawning, preferably done in customization menu itself
         * 
         * Allow manual cabin placement to migrate interiors over
         * 
         * Test multiple farmhands joining for the first time at the same time
         */
    }

    private bool onElevatorPress(GameLocation location, string[] arg2, Farmer who, Point point)
    {
        Game1.currentLocation.afterQuestion = (who, whichAnswer) =>
        {
            if (whichAnswer == "Cancel") return;
            who.modData[Constants.SelectedFloor] = whichAnswer;
            Game1.warpFarmer(whichAnswer, 0, 0, 0);
        };
        List<Response> options = new();
        foreach (var farmer in Game1.getAllFarmers())
        {
            if (!farmer.isUnclaimedFarmhand)
            {
                options.Add(new(farmer.homeLocation.Value, farmer.Name));
            }
        }
        options.Add(new Response("Cancel", Game1.content.LoadString("Strings\\Locations:ManorHouse_LedgerBook_TransferCancel")));
        Game1.currentLocation.createQuestionDialogue("Which floor do you want to enter?", options.ToArray(), "SinZ.SharedSpaces_Elevator");
        return true;
    }

    private void Content_AssetRequested(object? sender, StardewModdingAPI.Events.AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo("Data/Buildings"))
        {
            e.Edit(asset =>
            {
                var data = asset.AsDictionary<string, BuildingData>().Data;
                var farmhouse = data["Farmhouse"];
                farmhouse.ActionTiles.Add(new()
                {
                    Id = "SinZ.SharedSpaces_ElevatorButton",
                    Tile = new()
                    {
                        X = 6,
                        Y = 2
                    },
                    Action = "SinZ.SharedSpaces_ElevatorButton"
                });
                farmhouse.DrawLayers.Add(new()
                {
                    Id = "SinZ.SharedSpaces_ElevatorButton",
                    Texture = Helper.ModContent.GetInternalAssetName("assets/elevatorButton.png").Name,
                    SourceRect = new()
                    {
                        Width = 16,
                        Height = 16
                    },
                    DrawInBackground = false,
                    DrawPosition = new()
                    {
                        X = 6 * 16,
                        Y = 5 * 16
                    },
                });
            }, AssetEditPriority.Default - 1);
        }
    }
}

public static class Patches
{
    public static IMonitor monitor;
    public static void Init(Harmony harmony, IModHelper helper, IMonitor monitor)
    {
        Patches.monitor = monitor;

        var stardewLocationsName = AccessTools.Method("StardewLocations.StardewLocations:getLocationName");
        if (stardewLocationsName != null)
        {
            harmony.Patch(stardewLocationsName, prefix: new HarmonyMethod(typeof(Patches).GetMethod(nameof(StardewLocations__getLocationName__Prefix))));
        }

        harmony.Patch(AccessTools.Method(typeof(Farm), nameof(Farm.UnsetFarmhouseValues)), postfix: new HarmonyMethod(typeof(Patches).GetMethod(nameof(Farm__UnsetFarmhouseValues__Postfix))));
        harmony.Patch(AccessTools.Method(typeof(GameLocation), nameof(GameLocation.updateWarps)), postfix: new HarmonyMethod(typeof(Patches).GetMethod(nameof(GameLocation__updateWarps__Postfix))));
        harmony.Patch(AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction), new Type[] { typeof(string[]), typeof(Farmer), typeof(Location) }), transpiler: new HarmonyMethod(typeof(Patches).GetMethod(nameof(GameLocation__performAction__Transpiler))));
        harmony.Patch(AccessTools.Method(typeof(Building), nameof(Building.OnUseHumanDoor)), postfix: new HarmonyMethod(typeof(Patches).GetMethod(nameof(Building__OnUseHumanDoor__Postfix))));
        harmony.Patch(AccessTools.Method(typeof(FarmhandMenu.FarmhandSlot), nameof(FarmhandMenu.FarmhandSlot.Activate)), transpiler: new HarmonyMethod(typeof(Patches).GetMethod(nameof(FarmhandSlot__Activate__Transpiler))));
        harmony.Patch(AccessTools.Method(typeof(Game1), "_newDayAfterFade"), postfix: new HarmonyMethod(typeof(Patches).GetMethod(nameof(Game1___newDaysAfterFade__Postfix))));
        harmony.Patch(AccessTools.Method(typeof(GameServer), "unclaimedFarmhandsExist"), prefix: new HarmonyMethod(typeof(Patches).GetMethod(nameof(GameServer__unclaimedFarmhandsExist__Prefix))));
        harmony.Patch(AccessTools.Method(typeof(GameServer), nameof(GameServer.sendAvailableFarmhands)), transpiler: new HarmonyMethod(typeof(Patches).GetMethod(nameof(GameServer__sendAvailableFarmhands__Transpiler))));
        harmony.Patch(AccessTools.Method(typeof(NetWorldState), nameof(NetWorldState.TryAssignFarmhandHome)), prefix: new HarmonyMethod(typeof(Patches).GetMethod(nameof(NetWorldState__TryAssignFarmhandHome__Prefix))));
        harmony.Patch(AccessTools.Method(typeof(SaveGame), nameof(SaveGame.loadDataToLocations)), transpiler: new HarmonyMethod(typeof(Patches).GetMethod(nameof(SaveGame__loadDataToLocations__Transpiler))));
    }

    public static void StardewLocations__getLocationName__Prefix(ref string name, ref GameLocation loc)
    {
        if (loc is Cabin)
        {
            name = "Cabin";
        }
    }

    public static void Farm__UnsetFarmhouseValues__Postfix()
    {
        if (Game1.IsClient) return;
        foreach (var location in Game1.locations)
        {
            if (location is Cabin)
            {
                foreach (var warp in location.warps)
                {
                    if (warp.TargetName == "Farm")
                    {
                        var coords = Game1.getFarm().GetMainFarmHouseEntry();
                        warp.TargetX = coords.X;
                        warp.TargetY = coords.Y;
                    }
                }
            }
        }
    }

    public static void  GameLocation__updateWarps__Postfix(GameLocation __instance)
    {
        if (Game1.IsClient) return;
        // If the farm isn't loaded, then don't handle it. It is done anyway via Farm.UnsetFarmhouseValues in loading usecases
        if (Game1.getLocationFromNameInLocationsList("Farm") == null) return;
        if (__instance is Cabin)
        {
            foreach (var warp in __instance.warps)
            {
                if (warp.TargetName == "Farm")
                {
                    var coords = Game1.getFarm().GetMainFarmHouseEntry();
                    warp.TargetX = coords.X;
                    warp.TargetY = coords.Y;
                }
            }
        }
    }

    public static IEnumerable<CodeInstruction> GameLocation__performAction__Transpiler(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
    {
        var output = new List<CodeInstruction>();
        foreach (var instruction in instructions)
        {
            // for the purposes of mailbox, the farmhouse is owned by everyone
            if (instruction.opcode == OpCodes.Callvirt && (instruction.operand as MethodInfo)?.Name == "get_IsOwnedByCurrentPlayer")
            {
                /**
                 * Replace the following 
	             *  IL_4016: ldloc.s 102
	             *  IL_4018: callvirt instance bool StardewValley.Locations.FarmHouse::get_IsOwnedByCurrentPlayer()
                 * with
                 *           ldc.i4.1
                 * so its always "owned"
                 */
                // Remove the ldloc.s
                output.RemoveAt(output.Count - 1);
                // Replace the callvirt
                output.Add(new CodeInstruction(OpCodes.Ldc_I4_1));
                continue;
            }
            output.Add(instruction);
        }
        return output;
    }

    public static IEnumerable<CodeInstruction> SaveGame__loadDataToLocations__Transpiler(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
    {
        var instructionArray = instructions.ToArray();
        var newLabel = generator.DefineLabel();
        var output = new List<CodeInstruction>();

        bool found = false;

        CodeInstruction stloc8 = default;

        foreach (var instruction in instructionArray)
        {
            if (stloc8 == null && instruction.opcode == OpCodes.Stloc_S && (instruction.operand as LocalBuilder).LocalIndex == 8)
            {
                stloc8 = instruction;
            }
            output.Add(instruction);
            if (!found && instruction.opcode == OpCodes.Ldloc_S && (instruction.operand as LocalBuilder).LocalIndex == 9 && instruction.labels.Count == 2)
            {
                found = true;
                var brTrue = instructionArray[output.Count];
                var ldLoc8 = instructionArray[output.Count + 3];
                var stLoc9 = instructionArray[output.Count - 2];
                output.Add(brTrue);
                output.Add(ldLoc8);
                output.Add(new CodeInstruction(OpCodes.Call, typeof(Patches).GetMethod(nameof(SaveGame_loadDataToLocations__FixCabin))));
                output.Add(stLoc9);
                output.Add(instruction); //ldLoc9
            }
        }
        return output;
    }

    public static GameLocation? SaveGame_loadDataToLocations__FixCabin(GameLocation location)
    {
        if (location is Cabin oldCabin)
        {
            // Need to duplicate it due to NetCollection.Set clears the destination before iterating the argument.
            // so if it was setting with itself as the argument it is a clear
            var newCabin = new Cabin("Maps/FarmHouse");
            newCabin.name.Value = location.name.Value;
            newCabin.isFarm.Value = true;
            newCabin.isAlwaysActive.Value = true;
            newCabin.IsOutdoors = false;

            newCabin.fridge.Value = oldCabin.fridge.Value;
            newCabin.farmhandReference.Value = oldCabin.farmhandReference.Value;
            Game1.locations.Add(newCabin);
            return newCabin;
        }
        monitor.Log("Help!");
        return null;
    }

    public static void Building__OnUseHumanDoor__Postfix(Building __instance, Farmer who, ref bool __result)
    {
        if (__instance.buildingType.Value != "Farmhouse") return;
        if (!who.modData.TryGetValue(Constants.SelectedFloor, out var floor)) return;
        // Default behaviour
        if (floor == "FarmHouse") return;
        __result = false;

        who.currentLocation.playSound("doorClose", who.Tile);
        Game1.warpFarmer(floor, 0, 0, false);
    }

    public static IEnumerable<CodeInstruction> FarmhandSlot__Activate__Transpiler(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
    {
        var output = new List<CodeInstruction>();
        var skipCount = 0;
        foreach (var instruction in instructions)
        {
            if (skipCount-- > 0) continue;

            if (instruction.opcode == OpCodes.Ldnull)
            {
                output.Add(new CodeInstruction(OpCodes.Call, typeof(Patches).GetMethod(nameof(FarmhandSlot__Activate__AddCabins))));
                skipCount = 1;
                continue;
            }
            output.Add(instruction);
        }
        return output;
    }
    public static void FarmhandSlot__Activate__AddCabins(Client client)
    {
        // Inject the Cabin locations as they are dynamic
        foreach (var farmhand in client.availableFarmhands)
        {
            var cabin = new Cabin("Maps\\FarmHouse");
            cabin.name.Value = farmhand.homeLocation.Value;
            cabin.isFarm.Value = true;
            cabin.isAlwaysActive.Value = true;
            Game1.locations.Add(cabin);
        }

        // Preserve the code we replaced
        client.availableFarmhands = null;
    }

    public static void Game1___newDaysAfterFade__Postfix()
    {
        Game1.player.slotCanHost = true;
    }

    public static IEnumerable<CodeInstruction> GameServer__sendAvailableFarmhands__Transpiler(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
    {
        var output = new List<CodeInstruction>();
        foreach (var instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Newobj && ((ConstructorInfo)instruction.operand).DeclaringType.Name == "MemoryStream")
            {
                var firstInstruction = new CodeInstruction(OpCodes.Ldloc_1)
                {
                    labels = instruction.labels
                };
                instruction.labels = new List<Label>();
                output.Add(firstInstruction);
                output.Add(new CodeInstruction(OpCodes.Call, typeof(Patches).GetMethod(nameof(GameServer__sendAvailableFarmHands__AddEmptySlot))));
            }
            output.Add(instruction);
        }
        return output;
    }
    public static void GameServer__sendAvailableFarmHands__AddEmptySlot(List<NetRef<Farmer>> list)
    {
        foreach(var item in list)
        {
            // An existing empty slot already exists, use that!
            if (item.Value.isUnclaimedFarmhand) return;
        }
        // Need a new cabin/empty slot
        var cabin = new Cabin("Maps\\FarmHouse");
        cabin.name.Value = "Cabin" + Guid.NewGuid();
        cabin.isFarm.Value = true;
        cabin.isAlwaysActive.Value = true;
        Game1.locations.Add(cabin);
        cabin.CreateFarmhand();
        cabin.owner.modData[Constants.SelectedFloor] = cabin.Name;
        list.Add(Game1.netWorldState.Value.farmhandData.FieldDict.Values.First(f => f.Value.UniqueMultiplayerID == cabin.OwnerId));
    }

    /// <summary>
    /// NetWorldstate.TryAssignFarmhandHome is used to indicate if they already have a home, and if not find a cabin and allocate it
    /// We want them to always be allocated, as not being allocated kicks them.
    /// </summary>
    /// <param name="__result"></param>
    /// <returns></returns>
    public static bool NetWorldState__TryAssignFarmhandHome__Prefix(ref bool __result)
    {
        __result = true;
        return false;
    }

    /// <summary>
    /// GameServer.unclaimedFarmhandsExist is used to indicate if empty slots exist as part of lobby data.
    /// We want there to always be unclaimed farmhand slots available
    /// </summary>
    /// <param name="__result"></param>
    /// <returns></returns>
    public static bool GameServer__unclaimedFarmhandsExist__Prefix(ref bool __result)
    {
        __result = true;
        return false;
    }
}