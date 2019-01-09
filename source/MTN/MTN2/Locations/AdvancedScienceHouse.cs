using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;
using xTile.ObjectModel;
using xTile.Tiles;

namespace MTN2.Locations
{
    public class AdvancedScienceHouse : GameLocation
    {
        public GameLocation BaseLocation { get; private set; }

        public AdvancedScienceHouse() { }

        public AdvancedScienceHouse(string mapPath, string mapName) : base(mapPath, mapName) { }

        public AdvancedScienceHouse(string mapPath, string mapName, GameLocation oldScienceHouse) : base(mapPath, mapName) {
            if (oldScienceHouse != null) Import(oldScienceHouse);
        }

        public void Import(GameLocation oldLocation) {
            BaseLocation = oldLocation;

            characters.Set(oldLocation.characters);
            projectiles.Set(oldLocation.projectiles);
            debris.Set(oldLocation.debris);
            Traverse.Create(this).Field("terrainFeatures").SetValue(oldLocation.terrainFeatures);
        }

        public GameLocation Export() {
            GameLocation results = BaseLocation;

            results.characters.Set(characters);
            results.projectiles.Set(projectiles);
            results.debris.Set(debris);
            Traverse.Create(results).Field("terrainFeatures").SetValue(terrainFeatures);
            return results;
        }

        public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who) {
            Vector2 vect = new Vector2(tileLocation.X, tileLocation.Y);
            PropertyValue action = null;
            Tile tile = map.GetLayer("Buildings").PickTile(new Location(tileLocation.X * 64, tileLocation.Y * 64), viewport.Size);
            if (tile != null) {
                tile.Properties.TryGetValue("Action", out action);
                if (action != null) {
                    return (currentEvent != null || isCharacterAtTile(vect + new Vector2(0f, 1f)) == null) && performAction(action, who, tileLocation);
                }
            }
            return base.checkAction(tileLocation, viewport, who);
        }

        public new bool performAction(string action, Farmer who, Location tileLocation) {
            if (action == null || !who.IsLocalPlayer) {
                return false;
            }

            string[] strArguments = action.Split(' ');
            string command = strArguments[0];

            switch (command) {
                case "Carpenter":
                    if (who.getTileY() > tileLocation.Y) {
                        carpenters(tileLocation);
                    }
                    return true;
                case "Door":
                    if (strArguments.Length > 1 && !Game1.eventUp) {
                        for (int index = 1; index < strArguments.Length; ++index) {
                            if (who.getFriendshipHeartLevelForNPC(strArguments[index]) >= 2 || Game1.player.mailReceived.Contains("doorUnlock" + strArguments[index])) {
                                Rumble.rumble(0.1f, 100f);
                                if (!Game1.player.mailReceived.Contains("doorUnlock" + strArguments[index]))
                                    Game1.player.mailReceived.Add("doorUnlock" + strArguments[index]);
                                openDoor(tileLocation, true);
                                return false;
                            }
                        }
                        if (strArguments.Length == 2 && Game1.getCharacterFromName(strArguments[1], false) != null) {
                            NPC characterFromName = Game1.getCharacterFromName(strArguments[1], false);
                            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:DoorUnlock_NotFriend_" + (characterFromName.Gender == 0 ? "Male" : "Female"), (object)characterFromName.displayName));
                            return true;
                        } else if (Game1.getCharacterFromName(strArguments[1], false) != null && Game1.getCharacterFromName(strArguments[2], false) != null) {
                            NPC characterFromName1 = Game1.getCharacterFromName(strArguments[1], false);
                            NPC characterFromName2 = Game1.getCharacterFromName(strArguments[2], false);
                            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:DoorUnlock_NotFriend_Couple", (object)characterFromName1.displayName, (object)characterFromName2.displayName));
                            return true;
                        } else if (Game1.getCharacterFromName(strArguments[1], false) != null) {
                            NPC characterFromName = Game1.getCharacterFromName(strArguments[1], false);
                            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:DoorUnlock_NotFriend_" + (characterFromName.Gender == 0 ? "Male" : "Female"), (object)characterFromName.displayName));
                            return true;
                        }
                    } else {
                        openDoor(tileLocation, true);
                        return false;
                    }
                    break;
                case "Message":
                    Game1.drawDialogueNoTyping(Game1.content.LoadString("Strings\\StringsFromMaps:" + strArguments[1].Replace("\"", "")));
                    return true;
            }

            return false;
        }

        public override bool answerDialogueAction(string questionAndAnswer, string[] questionParams) {
            if (questionAndAnswer == null) return false;

            switch (questionAndAnswer) {
                case "carpenter_Construct":
                    Game1.activeClickableMenu = new CarpenterMenu(false);
                    break;
                case "carpenter_HouseDesign":
                    Game1.drawDialogue(Game1.getCharacterFromName("Robin", false), "Oh, I'm sorry but the architect who lived in town has been away for quite sometimes. I believe he went to see his friend who lives far away. Strangely enough, his friend calls himself \"Pickles\", kind of a strange guy.");
                    break;
                default:
                    return base.answerDialogueAction(questionAndAnswer, questionParams);
            }
            return true;

        }

        private void carpenters(Location tileLocation) {
            if (Game1.player.currentUpgrade == null) {
                foreach (NPC i in characters) {
                    if (i.Name.Equals("Robin")) {
                        if (Vector2.Distance(i.getTileLocation(), new Vector2(tileLocation.X, tileLocation.Y)) > 3f) {
                            return;
                        }
                        i.faceDirection(2);
                        if (Game1.player.daysUntilHouseUpgrade < 0 && !Game1.getFarm().isThereABuildingUnderConstruction()) {
                            List<Response> options = new List<Response>();
                            options.Add(new Response("Shop", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Shop")));
                            if (Game1.IsMasterGame) {
                                if (Game1.player.houseUpgradeLevel < 3) {
                                    options.Add(new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeHouse")));
                                } else if ((Game1.MasterPlayer.mailReceived.Contains("ccIsComplete") || Game1.MasterPlayer.mailReceived.Contains("JojaMember") || Game1.MasterPlayer.hasCompletedCommunityCenter()) && (Game1.getLocationFromName("Town") as Town).daysUntilCommunityUpgrade <= 0 && !Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade")) {
                                    options.Add(new Response("CommunityUpgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_CommunityUpgrade")));
                                }
                            } else if (Game1.player.houseUpgradeLevel < 2) {
                                options.Add(new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeCabin")));
                            }
                            if (Game1.IsMasterGame) {
                                options.Add(new Response("HouseDesign", "Change Farm House Exterior"));
                            }
                            options.Add(new Response("Construct", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Construct")));
                            options.Add(new Response("Leave", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Leave")));
                            createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu"), options.ToArray(), "carpenter");
                            return;
                        }
                        Game1.activeClickableMenu = new ShopMenu(Utility.getCarpenterStock(), 0, "Robin");
                        return;
                    }
                }
                if (Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).Equals("Tue")) {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_RobinAbsent").Replace('\n', '^'));
                }
            }
        }
    }
}
