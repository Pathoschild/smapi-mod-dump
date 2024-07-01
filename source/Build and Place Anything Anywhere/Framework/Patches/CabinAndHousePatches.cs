/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using AnythingAnywhere.Framework.Helpers;
using AnythingAnywhere.Framework.Utilities;
using Common.Helpers;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using xTile.Dimensions;

namespace AnythingAnywhere.Framework.Patches;
internal sealed class CabinAndHousePatches : PatchHelper
{
    public static Vector2 FarmHouseRealPos { get; private set; }

    public void Apply()
    {
        Patch<FarmHouse>(PatchType.Prefix, "resetLocalState", nameof(ResetLocalStatePrefix));

        Patch<GameLocation>(PatchType.Prefix, "houseUpgradeOffer", nameof(HouseUpgradeOfferPrefix));
        Patch<GameLocation>(PatchType.Prefix, "houseUpgradeAccept", nameof(HouseUpgradeAcceptPrefix));
        Patch<GameLocation>(PatchType.Prefix, nameof(GameLocation.carpenters), nameof(CarpentersPrefix), [typeof(Location)]);

        Patch<Chest>(PatchType.Prefix, nameof(Chest.checkForAction), nameof(CheckForActionPrefix), [typeof(Farmer), typeof(bool)]);

        Patch<BedFurniture>(PatchType.Postfix, nameof(BedFurniture.CanModifyBed), nameof(CanModifyBedPostfix), [typeof(Farmer)]);
        Patch<BedFurniture>(PatchType.Transpiler, nameof(BedFurniture.placementAction), nameof(PlacementActionTranspiler));
    }

    // Remove Farmhouse Hardcoded warp
    private static void ResetLocalStatePrefix(FarmHouse __instance)
    {
        FarmHouseRealPos = Game1.player.Tile;
    }

    private static bool HouseUpgradeOfferPrefix(GameLocation __instance)
    {
        if (!ModEntry.Config.EnableFreeHouseUpgrade)
            return true;

        string msg;
        switch (Game1.player.HouseUpgradeLevel)
        {
            case 0:
                msg = Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_UpgradeHouse1").Replace("10,000", "0").Replace("10.000", "0").Replace("10 000", "0").Replace("450", "0");
                __instance.createQuestionDialogue(Game1.parseText(msg), __instance.createYesNoResponses(), "upgrade");
                break;
            case 1:
                msg = Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_UpgradeHouse2", "0", "0");
                __instance.createQuestionDialogue(Game1.parseText(msg), __instance.createYesNoResponses(), "upgrade");
                break;
            case 2:
                msg = Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_UpgradeHouse3").Replace("100,000", "0").Replace("100.000", "0").Replace("100 000", "0");
                __instance.createQuestionDialogue(Game1.parseText(msg), __instance.createYesNoResponses(), "upgrade");
                break;
        }

        return false;
    }

    private static bool HouseUpgradeAcceptPrefix(GameLocation __instance)
    {
        if (!ModEntry.Config.EnableFreeHouseUpgrade)
            return true;

        Game1.player.daysUntilHouseUpgrade.Value = ModEntry.Config.InstantHomeUpgrade ? 0 : 3;
        Game1.RequireCharacter("Robin").setNewDialogue("Data\\ExtraDialogue:Robin_HouseUpgrade_Accepted");
        Game1.drawDialogue(Game1.getCharacterFromName("Robin"));
        ModEntry.Multiplayer?.globalChatInfoMessage("HouseUpgrade", Game1.player.Name, Lexicon.getTokenizedPossessivePronoun(Game1.player.IsMale));
        UpgradeHelper.CompleteHouseUpgrade(Game1.player);

        return false;
    }

    private static bool CarpentersPrefix(GameLocation __instance, Location tileLocation, ref bool __result)
    {
        if (!ModEntry.Config.UpgradeCabins && !ModEntry.Config.RenovateCabins)
            return true;

        foreach (var i in __instance.characters.Where(i => i.Name.Equals("Robin")))
        {
            if (Vector2.Distance(i.Tile, new Vector2(tileLocation.X, tileLocation.Y)) > 3f)
                return false;

            i.faceDirection(2);
            var options = new List<Response>();
            if (Game1.player.daysUntilHouseUpgrade.Value < 0 && !Game1.IsThereABuildingUnderConstruction())
            {
                options.Add(new Response("Shop", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Shop")));

                if (Game1.IsMasterGame)
                {
                    if (Game1.player.HouseUpgradeLevel < 3)
                    {
                        options.Add(new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeHouse")));
                    }
                    else if ((Game1.MasterPlayer.mailReceived.Contains("ccIsComplete") || Game1.MasterPlayer.mailReceived.Contains("JojaMember") || Game1.MasterPlayer.hasCompletedCommunityCenter()) && Game1.RequireLocation<Town>("Town").daysUntilCommunityUpgrade.Value <= 0)
                    {
                        if (!Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
                            options.Add(new Response("CommunityUpgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_CommunityUpgrade")));
                        else if (!Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts"))
                            options.Add(new Response("CommunityUpgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_CommunityUpgrade")));
                    }
                }
                else if (Game1.player.HouseUpgradeLevel < 3)
                {
                    options.Add(new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeCabin")));
                }

                if (Game1.IsMasterGame && CabinUtility.HasCabinsToUpgrade())
                {
                    options.Add(new Response("AA_Upgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeCabin")));
                }

                if (Game1.player.HouseUpgradeLevel >= 2)
                {
                    if (Game1.IsMasterGame)
                    {
                        options.Add(new Response("Renovate", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_RenovateHouse")));
                    }
                    else
                    {
                        options.Add(new Response("Renovate", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_RenovateCabin")));
                    }
                }

                if (Game1.IsMasterGame && CabinUtility.HasCabinsToUpgrade(true))
                {
                    options.Add(new Response("AA_Renovate", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_RenovateCabin")));
                }
                options.Add(new Response("Construct", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Construct")));
                options.Add(new Response("Leave", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Leave")));

                __instance.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu"), [.. options], (_, answer) =>
                {
                    switch (answer)
                    {
                        case "AA_Upgrade":
                            UpgradeHelper.UpgradeCabinsResponses();
                            break;
                        case "AA_Renovate":
                            RenovationHelper.RenovateCabinsResponses();
                            break;
                        default:
                            __instance.answerDialogueAction("carpenter_" + answer, null);
                            break;
                    }
                });
            }
            else
            {
                Utility.TryOpenShopMenu("Carpenter", "Robin");
            }

            __result = true;
            return false;
        }

        if (__instance.getCharacterFromName("Robin") == null && Game1.IsVisitingIslandToday("Robin"))
        {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_MoneyBox"));
            Game1.afterDialogues = delegate { Utility.TryOpenShopMenu("Carpenter", null, true); };
            return true;
        }

        if (Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).Equals("Tue"))
        {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_RobinAbsent").Replace('\n', '^'));
            return true;
        }

        return false;
    }

    // Remove starter gift
    private static bool CheckForActionPrefix(Chest __instance, Farmer who, ref bool __result, bool justCheckingForActivity = false)
    {
        if (!__instance.giftboxIsStarterGift.Value || !Game1.IsMasterGame || Game1.currentLocation.Equals(Game1.getLocationFromName(Game1.player.homeLocation.Value)) || justCheckingForActivity)
            return true;

        __instance.Location.removeObject(__instance.TileLocation, showDestroyedObject: false);
        return false;
    }

    // Enable modifying other players beds/placing inside of other players homes
    private static void CanModifyBedPostfix(BedFurniture __instance, Farmer who, ref bool __result)
    {
        if (!ModEntry.Config.EnablePlacing)
            return;

        __result = true;
    }

    // Enable all beds indoors
    private static IEnumerable<CodeInstruction> PlacementActionTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var placementActionTranspiler = instructions.ToList();
        try
        {
            var matcher = new CodeMatcher(placementActionTranspiler, generator);

            matcher.MatchEndForward(
                    new CodeMatch(OpCodes.Callvirt, AccessTools.Property(typeof(FarmHouse), nameof(FarmHouse.upgradeLevel)).GetGetMethod()),
                    new CodeMatch(OpCodes.Ldc_I4_2))
                .Set(OpCodes.Ldc_I4_0, null)
                .MatchEndForward(
                    new CodeMatch(OpCodes.Callvirt, AccessTools.Property(typeof(FarmHouse), nameof(FarmHouse.upgradeLevel)).GetGetMethod()),
                    new CodeMatch(OpCodes.Ldc_I4_1))
                .Set(OpCodes.Ldc_I4_0, null)
                .ThrowIfNotMatch("Could not find get_upgradeLevel()");

            return matcher.InstructionEnumeration();
        }
        catch (Exception e)
        {
            ModEntry.ModMonitor.Log($"There was an issue modifying the instructions for {typeof(BedFurniture)}.{original.Name}: {e}", LogLevel.Error);
            return placementActionTranspiler;
        }
    }
}