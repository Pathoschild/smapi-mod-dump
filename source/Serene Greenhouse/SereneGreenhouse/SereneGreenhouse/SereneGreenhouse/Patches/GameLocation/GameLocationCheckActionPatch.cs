/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SereneGreenhouse
**
*************************************************/

using HarmonyLib;
using StardewValley;
using System.Reflection;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using System.Linq;
using xTile.Dimensions;
using xTile.Tiles;
using System.Collections.Generic;

namespace SereneGreenhouse.Patches.GameLocation
{
    [HarmonyPatch]
    public class GameLocationCheckActionPatch
    {
        private static IMonitor monitor = ModEntry.monitor;
        private static readonly string acceptedOfferingMessage = "For us? Thank you, thank you!#Come back tomorrow, forest will change!";
        private static List<Response> offeringResponses = new List<Response>()
        {
            new Response("Offering_Yes", "Yes"),
            new Response("Offering_No", "No")
        };

        internal static MethodInfo TargetMethod()
        {
            return AccessTools.Method(typeof(StardewValley.GameLocation), nameof(StardewValley.GameLocation.checkAction));
        }

        internal static bool Prefix(StardewValley.GameLocation __instance, ref bool __result, Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
        {
            if (__instance.Name != "Greenhouse")
            {
                return true;
            }

            Tile tile = __instance.map.GetLayer("Buildings").PickTile(new Location(tileLocation.X * 64, tileLocation.Y * 64), viewport.Size);
            if (tile != null && tile.Properties.ContainsKey("CustomAction"))
            {
                if (tile.Properties["CustomAction"] == "Treehouse")
                {
                    if (bool.Parse(tile.Properties["HasReceivedOfferingToday"]) is true)
                    {
                        Game1.drawObjectDialogue("Fruits, fruits! Come back tomorrow, forest will change!");
                    }
                    else if (who.ActiveObject is null)
                    {
                        if (!Game1.MasterPlayer.mailReceived.Contains("SG_Treehouse_Expansion_1"))
                        {
                            Game1.drawObjectDialogue("An odd tree that seems to have a door fused to it.#From behind the door you can hear a tiny voice...#Gibe 100 Starfruit, we shape forest for more plants!");
                        }
                        else if (!Game1.MasterPlayer.mailReceived.Contains("SG_Treehouse_Expansion_2"))
                        {
                            Game1.drawObjectDialogue("An odd tree that seems to have a door fused to it.#From behind the door you can hear a tiny voice...#Gibe 100 Sweet Gem Berries, we shape forest for more plants!");
                        }
                        else if (!Game1.MasterPlayer.mailReceived.Contains("SG_Treehouse_Expansion_3"))
                        {
                            Game1.drawObjectDialogue("An odd tree that seems to have a door fused to it.#From behind the door you can hear a tiny voice...#Gibe 100 Ancient Fruit, we shape forest for more plants!");
                        }
                        else
                        {
                            Game1.drawObjectDialogue("An odd tree that seems to have a door fused to it.#From behind the door you hear only silence.");
                        }
                    }
                    else
                    {
                        if (!Game1.MasterPlayer.mailReceived.Contains("SG_Treehouse_Expansion_1") && who.ActiveObject.ParentSheetIndex == 268 && who.ActiveObject.Stack >= 100)
                        {
                            ModEntry.AcceptOffering(who, acceptedOfferingMessage, 100, tile);
                            Game1.MasterPlayer.mailReceived.Add("SG_Treehouse_Expansion_1");
                        }
                        else if (!Game1.MasterPlayer.mailReceived.Contains("SG_Treehouse_Expansion_2") && who.ActiveObject.ParentSheetIndex == 417 && who.ActiveObject.Stack >= 100)
                        {
                            ModEntry.AcceptOffering(who, acceptedOfferingMessage, 100, tile);
                            Game1.MasterPlayer.mailReceived.Add("SG_Treehouse_Expansion_2");
                        }
                        else if (!Game1.MasterPlayer.mailReceived.Contains("SG_Treehouse_Expansion_3") && who.ActiveObject.ParentSheetIndex == 454 && who.ActiveObject.Stack >= 100)
                        {
                            ModEntry.AcceptOffering(who, acceptedOfferingMessage, 100, tile);
                            Game1.MasterPlayer.mailReceived.Add("SG_Treehouse_Expansion_3");
                        }
                        else
                        {
                            Game1.drawObjectDialogue("Nothing interesting happens.");
                        }
                    }

                    __result = true;
                    return false;
                }
                else if (tile.Properties["CustomAction"] == "Waterhut")
                {
                    if (!Game1.MasterPlayer.modData.ContainsKey(ModEntry.offeringsStoredInWaterHutKey))
                    {
                        Game1.MasterPlayer.modData[ModEntry.offeringsStoredInWaterHutKey] = "0";
                    }

                    if (who.ActiveObject is null)
                    {
                        int offeringsCount = 0;
                        if (!int.TryParse(Game1.MasterPlayer.modData[ModEntry.offeringsStoredInWaterHutKey], out offeringsCount))
                        {
                            monitor.Log($"Issue parsing ModData key [{ModEntry.offeringsStoredInWaterHutKey}]'s value to int", LogLevel.Trace);
                        }

                        if (offeringsCount == 0)
                        {
                            Game1.drawObjectDialogue($"A chorus of tiny voices echo from inside the hut...#Gibe us fruit! Gibe us vegetables! Feed the mighty Junimos!#If we have at least one offering in storage, we'll water the plants of this forest!");
                        }
                        else
                        {
                            Game1.drawObjectDialogue($"There are {offeringsCount} offering(s) stored inside. The Junimos will water the plants each morning for {offeringsCount} day(s).");
                        }
                    }
                    else
                    {
                        // If it is a vegetable or fruit, prompt to accept the offering
                        if (who.ActiveObject.Category == -75 || who.ActiveObject.Category == -79)
                        {
                            Game1.drawObjectQuestionDialogue($"Offer the {who.ActiveObject.Stack} {who.ActiveObject.DisplayName}?", offeringResponses.ToArray());

                        }
                        else
                        {
                            Game1.drawObjectDialogue("The Junimos aren't interested in that offering.");
                        }
                    }

                    __result = true;
                    return false;
                }
            }

            return true;
        }
    }
}
