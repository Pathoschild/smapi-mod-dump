/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using StardewValley;
using xTile.Dimensions;
using StardewValley.Objects;
using HarmonyLib;
using System;
using static StardewValley.GameLocation;
using StardewValley.Menus;

namespace ExtraGingerIslandMaps
{
  
    [HarmonyPatch(typeof(GameLocation))]
    internal static class GameLocationPatches
    {
        internal static void Apply(Harmony harmony)
        {
            ModEntry.Mon.Log($"Applying Harmony patch \"{nameof(GameLocationPatches)}\": postfixing SDV method \"GameLocation.performAction\".");
            
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.GameLocation), nameof(StardewValley.GameLocation.performAction)),
                postfix: new HarmonyMethod(typeof(GameLocationPatches), nameof(GameLocationPatches.PostFix_performAction))
                );
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameLocation.performAction))]
        internal static void PostFix_performAction(ref GameLocation __instance, string action, Farmer who, Location tileLocation)
        {
            if (action != null && who.IsLocalPlayer)
            { //warp 88 14 islandwest RopeToWest
                string[] actionParams = action.Split(' ');
                
                //check if custom
                switch (actionParams[0])
                {
                    case "GiEX.Elevator":
                        Game1.activeClickableMenu = new CustomElevatorMenu();
                        break;

                    case "GiEX.FireRing":
                        try
                        {
                            if (!Game1.player.mailReceived.Contains("gotFireRing"))
                            {
                                if (!Game1.player.isInventoryFull())
                                {
                                    Game1.playSound("parry");

                                    //tile X, tile Y, index, layer, whichTileSheet
                                    __instance.setMapTileIndex(4, 17, 10, "Front", 3);
                                    __instance.setMapTileIndex(5, 17, 11, "Front", 3);
                                    __instance.setMapTileIndex(4, 18, 26, "Buildings", 3);

                                    Game1.player.addItemByMenuIfNecessaryElseHoldUp(new Ring(ModEntry.FireRing));
                                    Game1.player.mailReceived.Add("gotFireRing");
                                }
                                else
                                {
                                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
                                }
                            }
                        }
                        catch(Exception ex)
                        {
                            ModEntry.Mon.Log("Error: " + ex, StardewModdingAPI.LogLevel.Error);
                        }
                        break;
                    case "GiEX.RopeToWest":
                        var res = Game1.player.currentLocation.createYesNoResponses();
                        Game1.player.currentLocation.createQuestionDialogue(ModEntry.RopeQuestion, res,new afterQuestionBehavior(RopeAfterQuestion),null);
                        break;
                    default:
                        break;
                }
                
                //30% to obtain some cash if checking the boxes.
                if(actionParams[0]=="Message" && actionParams[1]== "Custom_GiRiver.8")
                {
                    if (ModEntry.HasObtainedCashToday)
                    {
                        return; //only once per day
                    }

                    if (Game1.random.Next(10) < 3)
                    {
                        int G = Game1.random.Next(500, 2001); //randomly choose btwn 500 and 2k G

                        if (Game1.random.Next(20) > 18)
                        {
                            //if the random is 19
                            //this means 5% chance to receive 10k or 5k instead
                            G = Game1.random.Next(2) == 1 ? 10000 : 5000;
                        }
                            //format
                            string addedG = string.Format(ModEntry.FoundG, G.ToString());

                        //add to current dialogue
                        (Game1.activeClickableMenu as DialogueBox).dialogues.Add(addedG);

                        //add to player cash
                        Game1.player.Money += G;

                        var jackie = Game1.getCharacterFromName("Jackie", false, false);
                        //if jackie is present, reduce friendship by 10 and jump;
                        if (__instance.characters.Contains(jackie))
                        {
                            jackie.jump();
                            jackie.doEmote(12, false); //upset
                            jackie.setNewDialogue(ModEntry.UpsetJackie, true, true);

                            //take away 20 friendship for stealing
                            Game1.player.friendshipData["Jackie"].Points = -20;
                        }
                        
                        ModEntry.HasObtainedCashToday = true; //set to true
                    }
                }
            }
        }

        private static void RopeAfterQuestion(Farmer who, string whichAnswer)
        {
            if(whichAnswer == "Yes")
            {
                Game1.player.jump();
                Game1.playSound("fallDown");
                Game1.warpFarmer("IslandWest", 88, 14, false);
            }
        }
    }
}
    