/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rokugin/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using xTile;
using xTile.Tiles;
using xTile.Layers;
using xTile.Dimensions;
using GenericModConfigMenu;
using StardewValley.Menus;

namespace CustomLocksUpdated {
    internal class ModEntry : Mod {

        ModConfig config = new();
        Dictionary<string, int[]> characterDict = new Dictionary<string, int[]>();

        public override void Entry(IModHelper helper) {
            config = helper.ReadConfig<ModConfig>();

            helper.Events.Content.AssetRequested += OnAssetRequested;
            helper.Events.Player.Warped += OnWarped;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.TimeChanged += OnTimeChanged;
        }

        private void OnTimeChanged(object? sender, TimeChangedEventArgs e) {
            Map map = Game1.currentLocation.Map;
            string locationName = Game1.currentLocation.Name;

            if (locationName == "Forest") {
                OnTimeChangedForest(map);
            }
            if (locationName == "Mountain") {
                OnTimeChangedMountain(map);
            }
            if (locationName == "Beach") {
                OnTimeChangedBeach(map);
            }
            if (locationName == "Town") {
                OnTimeChangedTown(map);
            }
            if (locationName == "Desert") {
                OnTimeChangedDesert(map);
            }
            if (Game1.activeClickableMenu is BobberBar bar) {
                int newFishID = Helper.Reflection.GetField<int>(bar, "whichFish").GetValue();
            }
        }

        void OnTimeChangedForest(Map map) {
            string[] actionProperties = new string[3];
            Tile[] tiles = new Tile[3];

            tiles[0] = GetTile(map, "Buildings", 90, 15);
            tiles[1] = GetTile(map, "Buildings", 104, 32);
            tiles[2] = GetTile(map, "Buildings", 5, 26);

            if (config.OutsideNormalHours) {
                actionProperties[0] = "Warp 13 19 AnimalShop";
                actionProperties[2] = "Warp 8 24 WizardHouse";
                if (config.StrangerRoomEntry) {
                    actionProperties[1] = "Warp 7 9 LeahHouse";
                }
            } else {
                actionProperties[0] = "LockedDoorWarp 13 19 AnimalShop 900 1800";
                if (config.StrangerRoomEntry) {
                    actionProperties[1] = "LockedDoorWarp 7 9 LeahHouse 1000 1800";
                } else {
                    actionProperties[1] = "LockedDoorWarp 7 9 LeahHouse 1000 1800 Leah 500";
                }
                actionProperties[2] = "LockedDoorWarp 8 24 WizardHouse 600 2300";
            }

            for (int i = 0; i < tiles.Length; i++) {
                if (tiles[i] == null) continue;
                tiles[i].Properties["Action"] = actionProperties[i];
            }
        }

        void OnTimeChangedMountain(Map map) {
            if (map == null) return;
            string[] actionProperties = new string[3];
            Tile[] tiles = new Tile[3];
            
            tiles[0] = GetTile(map, "Buildings", 12, 25);
            tiles[1] = GetTile(map, "Buildings", 8, 20);
            tiles[2] = GetTile(map, "Buildings", 76, 8);

            if (config.OutsideNormalHours) {
                actionProperties[0] = "Warp 6 24 ScienceHouse";
                actionProperties[2] = "Warp 6 19 AdventureGuild";
                if (config.StrangerRoomEntry) {
                    actionProperties[1] = "Warp 3 8 ScienceHouse";
                }
            } else {
                actionProperties[0] = "LockedDoorWarp 6 24 ScienceHouse 900 2000";
                if (config.StrangerRoomEntry) {
                    actionProperties[1] = "LockedDoorWarp 3 8 ScienceHouse 900 2000";
                } else {
                    actionProperties[1] = "LockedDoorWarp 3 8 ScienceHouse 900 2000 Maru 500";
                }
                actionProperties[2] = "LockedDoorWarp 6 19 AdventureGuild 1400 2600";
            }

            for (int i = 0; i < tiles.Length; i++) {
                if (tiles[i] == null) continue;
                tiles[i].Properties["Action"] = actionProperties[i];
            }

            bool playerIsGuildMember = Game1.player.mailReceived.Contains("guildMember");
            Tile tile = GetTile(map, "Back", 76, 9);

            if (config.OutsideNormalHours && config.EarlyAdventureGuild && !playerIsGuildMember) {
                tile.Properties["TouchAction"] = "Warp AdventureGuild 6 19";
            } else if (!config.OutsideNormalHours && config.EarlyAdventureGuild && !playerIsGuildMember) {
                if (Game1.timeOfDay > 1400 && Game1.timeOfDay < 2600) {
                    tile.Properties["TouchAction"] = "Warp AdventureGuild 6 19";
                }
            } else {
                tile.Properties.Remove("TouchAction");
            }
        }

        void OnTimeChangedBeach(Map map) {
            string[] actionProperties = new string[2];
            Tile[] tiles = new Tile[2];

            tiles[0] = GetTile(map, "Buildings", 30, 33);
            tiles[1] = GetTile(map, "Buildings", 49, 10);

            if (config.OutsideNormalHours) {
                actionProperties[0] = "Warp 5 9 FishShop";
                if (config.StrangerRoomEntry) {
                    actionProperties[1] = "Warp 3 9 ElliottHouse";
                }
            } else {
                actionProperties[0] = "LockedDoorWarp 5 9 FishShop 900 1700";
                if (config.StrangerRoomEntry) {
                    actionProperties[1] = "LockedDoorWarp 3 9 ElliottHouse 1000 1800";
                } else {
                    actionProperties[1] = "LockedDoorWarp 3 9 ElliottHouse 1000 1800 Elliott 500";
                }
            }

            for (int i = 0; i < tiles.Length; i++) {
                if (tiles[i] == null) continue;
                tiles[i].Properties["Action"] = actionProperties[i];
            }
        }

        void OnTimeChangedTown(Map map) {
            string[] actionProperties = new string[14];
            Tile[] tiles = new Tile[14];

            tiles[0] = GetTile(map, "Buildings", 36, 55);
            tiles[1] = GetTile(map, "Buildings", 43, 56);
            tiles[2] = GetTile(map, "Buildings", 44, 56);
            tiles[3] = GetTile(map, "Buildings", 57, 63);
            tiles[4] = GetTile(map, "Buildings", 45, 70);
            tiles[5] = GetTile(map, "Buildings", 72, 68);
            tiles[6] = GetTile(map, "Buildings", 58, 85);
            tiles[7] = GetTile(map, "Buildings", 59, 85);
            tiles[8] = GetTile(map, "Buildings", 10, 85);
            tiles[9] = GetTile(map, "Buildings", 20, 88);
            tiles[10] = GetTile(map, "Buildings", 101, 89);
            tiles[11] = GetTile(map, "Buildings", 94, 81);
            tiles[12] = GetTile(map, "Buildings", 95, 50);
            tiles[13] = GetTile(map, "Buildings", 96, 50);

            if (config.OutsideNormalHours) {
                actionProperties[0] = "Warp 10 19 Hospital";
                actionProperties[1] = "Warp 6 29 SeedShop";
                actionProperties[2] = "Warp 6 29 SeedShop";
                actionProperties[3] = "Warp 9 24 JoshHouse";
                actionProperties[4] = "Warp 14 24 Saloon";
                actionProperties[5] = "Warp 12 9 Trailer";
                actionProperties[6] = "Warp 4 11 ManorHouse";
                actionProperties[7] = "Warp 5 11 ManorHouse";
                actionProperties[8] = "Warp 4 23 SamHouse";
                actionProperties[9] = "Warp 2 24 HaleyHouse";
                actionProperties[10] = "Warp 3 14 ArchaeologyHouse";
                actionProperties[11] = "Warp 5 19 Blacksmith";
                actionProperties[12] = "Warp 13 29 JojaMart";
                actionProperties[13] = "Warp 14 29 JojaMart";
            } else {
                actionProperties[0] = "LockedDoorWarp 10 19 Hospital 900 1500";
                actionProperties[1] = "LockedDoorWarp 6 29 SeedShop 900 2100";
                actionProperties[2] = "LockedDoorWarp 6 29 SeedShop 900 2100";
                actionProperties[3] = "LockedDoorWarp 9 24 JoshHouse 800 2000";
                actionProperties[4] = "LockedDoorWarp 14 24 Saloon 1200 2400";
                actionProperties[5] = "LockedDoorWarp 12 9 Trailer 900 2000";
                actionProperties[6] = "LockedDoorWarp 4 11 ManorHouse 830 2200";
                actionProperties[7] = "LockedDoorWarp 5 11 ManorHouse 830 2200";
                actionProperties[8] = "LockedDoorWarp 4 23 SamHouse 900 2000";
                actionProperties[9] = "LockedDoorWarp 2 24 HaleyHouse 900 2000";
                actionProperties[10] = "LockedDoorWarp 3 14 ArchaeologyHouse 800 1800";
                actionProperties[11] = "LockedDoorWarp 5 19 Blacksmith 900 1600";
                actionProperties[12] = "LockedDoorWarp 13 29 JojaMart 900 2300";
                actionProperties[13] = "LockedDoorWarp 14 29 JojaMart 900 2300";
            }

            for (int i = 0; i < tiles.Length; i++) {
                if (tiles[i] == null) continue;
                tiles[i].Properties["Action"] = actionProperties[i];
            }
        }

        void OnTimeChangedDesert(Map map) {
            string actionProperty;
            Tile tile = GetTile(map, "Buildings", 6, 51);

            if (config.OutsideNormalHours) {
                actionProperty = "Warp 4 9 SandyHouse";
            } else {
                actionProperty = "LockedDoorWarp 4 9 SandyHouse 900 2350";
            }

            if (tile == null) return;
            tile.Properties["Action"] = actionProperty;
        }

        private void OnWarped(object? sender, WarpedEventArgs e) {
            string name = e.NewLocation.Name;
            Map map = e.NewLocation.Map;

            if (e.NewLocation.IsActiveLocation()) {
                if (name == "Forest") {
                    OnTimeChangedForest(map);
                }
                if (name == "Beach") {
                    OnTimeChangedBeach(map);
                }
                if (name == "Mountain") {
                    OnTimeChangedMountain(map);
                }
                if (name == "Town") {
                    OnTimeChangedTown(map);
                }
                if (name == "Desert") {
                    OnTimeChangedDesert(map);
                }
                if (name == "AnimalShop") {
                    OnWarpInterior(name, "Jas", "Marnie", "Shane");
                }
                if (name == "ScienceHouse") {
                    OnWarpInterior(name, "Maru", "Robin", "Demetrius", true);
                }
                if (name == "SebastianRoom") {
                    OnWarpInterior(name, "Sebastian");
                }
                if (name == "Hospital") {
                    OnWarpHospital(name);
                }
                if (name == "SeedShop") {
                    OnWarpInterior(name, "Abigail", "Pierre", "Caroline", true);
                }
                if (name == "JoshHouse") {
                    OnWarpInterior(name, "Alex", "Evelyn", "George", true);
                }
                if (name == "Saloon") {
                    OnWarpInterior(name, "Gus");
                }
                if (name == "Trailer") {
                    OnWarpInterior(name, "Penny");
                }
                if (name == "Trailer_big") {
                    OnWarpInterior(name, "PennyBig", "Pam");
                }
                if (name == "ManorHouse") {
                    OnWarpInterior(name, "Lewis");
                }
                if (name == "SamHouse") {
                    OnWarpInterior(name, "Sam", "Vincent", "Jodi", "Kent");
                }
                if (name == "HaleyHouse") {
                    OnWarpInterior(name, "Haley", "Emily");
                }
                if (name == "Blacksmith") {
                    OnWarpInterior(name, "Clint");
                }
            }
        }

        void OnWarpHospital(string name) {
            GameLocation location = Game1.getLocationFromName(name);
            string characterActionProperty;

            if (Game1.player.getFriendshipHeartLevelForNPC("Harvey") >= 2 || config.StrangerRoomEntry) {
                characterActionProperty = "Door";
                location.removeTileProperty(9, 5, "Back", "TouchAction");
                location.removeTileProperty(10, 5, "Back", "TouchAction");
            } else {
                characterActionProperty = "Door Harvey";
                location.setTileProperty(9, 5, "Back", "TouchAction", characterActionProperty);
                location.setTileProperty(10, 5, "Back", "TouchAction", characterActionProperty);
            }

            location.setTileProperty(9, 5, "Buildings", "Action", characterActionProperty);
            location.setTileProperty(10, 5, "Buildings", "Action", characterActionProperty);
        }

        void OnWarpInterior(string name, string characterKey) {
            GameLocation location = Game1.getLocationFromName(name);
            string characterActionProperty;

            if (Game1.player.getFriendshipHeartLevelForNPC(characterKey) >= 2 || config.StrangerRoomEntry) {
                characterActionProperty = "Door";
                location.removeTileProperty(characterDict[characterKey][0], characterDict[characterKey][1], "Back", "TouchAction");
            } else {
                characterActionProperty = "Door " + characterKey;
                location.setTileProperty(characterDict[characterKey][0], characterDict[characterKey][1],
                                     "Back", "TouchAction", characterActionProperty);
            }

            location.setTileProperty(characterDict[characterKey][0], characterDict[characterKey][1],
                                     "Buildings", "Action", characterActionProperty);
        }

        void OnWarpInterior(string name, string firstCharacterKey, string secondCharacterKey) {
            GameLocation location = Game1.getLocationFromName(name);
            string firstCharacterActionProperty, secondCharacterActionProperty;
            bool firstAllowEntry, secondAllowEntry;

            if (Game1.player.getFriendshipHeartLevelForNPC(firstCharacterKey) >= 2 || config.StrangerRoomEntry) {
                firstCharacterActionProperty = "Door";
                firstAllowEntry = true;
            } else {
                firstCharacterActionProperty = "Door " + firstCharacterKey;
                firstAllowEntry = false;
            }
            if (Game1.player.getFriendshipHeartLevelForNPC(secondCharacterKey) >= 2 || config.StrangerRoomEntry) {
                secondCharacterActionProperty = "Door";
                secondAllowEntry = true;
            } else {
                secondCharacterActionProperty = "Door " + secondCharacterKey;
                secondAllowEntry = false;
            }

            location.setTileProperty(characterDict[firstCharacterKey][0], characterDict[firstCharacterKey][1],
                                     "Buildings", "Action", firstCharacterActionProperty);
            location.setTileProperty(characterDict[secondCharacterKey][0], characterDict[secondCharacterKey][1],
                                     "Buildings", "Action", secondCharacterActionProperty);

            if (firstAllowEntry) {
                location.removeTileProperty(characterDict[firstCharacterKey][0], characterDict[firstCharacterKey][1],
                                     "Back", "TouchAction");
            } else {
                location.setTileProperty(characterDict[firstCharacterKey][0], characterDict[firstCharacterKey][1],
                                     "Back", "TouchAction", firstCharacterActionProperty);
            }
            if (secondAllowEntry) {
                location.removeTileProperty(characterDict[secondCharacterKey][0], characterDict[secondCharacterKey][1],
                                     "Back", "TouchAction");
            } else {
                location.setTileProperty(characterDict[secondCharacterKey][0], characterDict[secondCharacterKey][1],
                                     "Back", "TouchAction", secondCharacterActionProperty);
            }
        }

        void OnWarpInterior(string name, string firstCharacterKey, string secondCharacterKey, string thirdCharacterKey) {
            GameLocation location = Game1.getLocationFromName(name);
            string firstCharacterActionProperty, secondCharacterActionProperty, thirdCharacterActionProperty;
            bool firstAllowEntry, secondAllowEntry, thirdAllowEntry;

            if (Game1.player.getFriendshipHeartLevelForNPC(firstCharacterKey) >= 2 || config.StrangerRoomEntry) {
                firstCharacterActionProperty = "Door";
                firstAllowEntry = true;
            } else {
                firstCharacterActionProperty = "Door " + firstCharacterKey;
                firstAllowEntry = false;
            }
            if (Game1.player.getFriendshipHeartLevelForNPC(secondCharacterKey) >= 2 || config.StrangerRoomEntry) {
                secondCharacterActionProperty = "Door";
                secondAllowEntry = true;
            } else {
                secondCharacterActionProperty = "Door " + secondCharacterKey;
                secondAllowEntry = false;
            }
            if (Game1.player.getFriendshipHeartLevelForNPC(thirdCharacterKey) >= 2 || config.StrangerRoomEntry) {
                thirdCharacterActionProperty = "Door";
                thirdAllowEntry = true;
            } else {
                thirdCharacterActionProperty = "Door " + thirdCharacterKey;
                thirdAllowEntry = false;
            }

            location.setTileProperty(characterDict[firstCharacterKey][0], characterDict[firstCharacterKey][1],
                                     "Buildings", "Action", firstCharacterActionProperty);
            location.setTileProperty(characterDict[secondCharacterKey][0], characterDict[secondCharacterKey][1],
                                     "Buildings", "Action", secondCharacterActionProperty);
            location.setTileProperty(characterDict[thirdCharacterKey][0], characterDict[thirdCharacterKey][1],
                                     "Buildings", "Action", thirdCharacterActionProperty);

            if (firstAllowEntry) {
                location.removeTileProperty(characterDict[firstCharacterKey][0], characterDict[firstCharacterKey][1],
                                     "Back", "TouchAction");
            } else {
                location.setTileProperty(characterDict[firstCharacterKey][0], characterDict[firstCharacterKey][1],
                                     "Back", "TouchAction", firstCharacterActionProperty);
            }
            if (secondAllowEntry) {
                location.removeTileProperty(characterDict[secondCharacterKey][0], characterDict[secondCharacterKey][1],
                                     "Back", "TouchAction");
            } else {
                location.setTileProperty(characterDict[secondCharacterKey][0], characterDict[secondCharacterKey][1],
                                     "Back", "TouchAction", secondCharacterActionProperty);
            }
            if (thirdAllowEntry) {
                location.removeTileProperty(characterDict[thirdCharacterKey][0], characterDict[thirdCharacterKey][1],
                                     "Back", "TouchAction");
            } else {
                location.setTileProperty(characterDict[thirdCharacterKey][0], characterDict[thirdCharacterKey][1],
                                     "Back", "TouchAction", thirdCharacterActionProperty);
            }
        }

        void OnWarpInterior(string name, string firstCharacterKey, string secondCharacterKey, string thirdCharacterKey,
                            bool secondAndThirdTogether) {
            GameLocation location = Game1.getLocationFromName(name);
            string firstCharacterActionProperty, secondCharacterActionProperty;
            bool firstAllowEntry, secondAllowEntry;

            if (Game1.player.getFriendshipHeartLevelForNPC(firstCharacterKey) >= 2 || config.StrangerRoomEntry) {
                firstCharacterActionProperty = "Door";
                firstAllowEntry = true;
            } else {
                firstCharacterActionProperty = "Door " + firstCharacterKey;
                firstAllowEntry = false;
            }
            if (Game1.player.getFriendshipHeartLevelForNPC(secondCharacterKey) >= 2 || config.StrangerRoomEntry) {
                secondCharacterActionProperty = "Door";
                secondAllowEntry = true;
            } else {
                secondCharacterActionProperty = "Door " + secondCharacterKey + " " + thirdCharacterKey;
                secondAllowEntry = false;
            }

            location.setTileProperty(characterDict[firstCharacterKey][0], characterDict[firstCharacterKey][1],
                                     "Buildings", "Action", firstCharacterActionProperty);
            location.setTileProperty(characterDict[secondCharacterKey][0], characterDict[secondCharacterKey][1],
                                     "Buildings", "Action", secondCharacterActionProperty);

            if (firstAllowEntry) {
                location.removeTileProperty(characterDict[firstCharacterKey][0], characterDict[firstCharacterKey][1],
                                     "Back", "TouchAction");
            } else {
                location.setTileProperty(characterDict[firstCharacterKey][0], characterDict[firstCharacterKey][1],
                                     "Back", "TouchAction", firstCharacterActionProperty);
            }
            if (secondAllowEntry) {
                location.removeTileProperty(characterDict[secondCharacterKey][0], characterDict[secondCharacterKey][1],
                                     "Back", "TouchAction");
            } else {
                location.setTileProperty(characterDict[secondCharacterKey][0], characterDict[secondCharacterKey][1],
                                     "Back", "TouchAction", secondCharacterActionProperty);
            }
        }

        void OnWarpInterior(string name, string firstCharacterKey, string secondCharacterKey,
                            string thirdCharacterKey, string fourthCharacterKey) {
            GameLocation location = Game1.getLocationFromName(name);
            string firstCharacterActionProperty, secondCharacterActionProperty, thirdCharacterActionProperty;
            bool firstAllowEntry, secondAllowEntry, thirdAllowEntry;

            if (Game1.player.getFriendshipHeartLevelForNPC(firstCharacterKey) >= 2 || config.StrangerRoomEntry) {
                firstCharacterActionProperty = "Door";
                firstAllowEntry = true;
            } else {
                firstCharacterActionProperty = "Door " + firstCharacterKey;
                firstAllowEntry = false;
            }
            if (Game1.player.getFriendshipHeartLevelForNPC(secondCharacterKey) >= 2 || config.StrangerRoomEntry) {
                secondCharacterActionProperty = "Door";
                secondAllowEntry = true;
            } else {
                secondCharacterActionProperty = "Door " + secondCharacterKey;
                secondAllowEntry = false;
            }
            if ((Game1.player.getFriendshipHeartLevelForNPC(thirdCharacterKey) >= 2
                || Game1.player.getFriendshipHeartLevelForNPC(fourthCharacterKey) >= 2)
                || config.StrangerRoomEntry) {
                thirdCharacterActionProperty = "Door";
                thirdAllowEntry = true;
            } else {
                thirdCharacterActionProperty = "Door " + thirdCharacterKey + " " + fourthCharacterKey;
                thirdAllowEntry = false;
            }

            location.setTileProperty(characterDict[firstCharacterKey][0], characterDict[firstCharacterKey][1],
                                     "Buildings", "Action", firstCharacterActionProperty);
            location.setTileProperty(characterDict[secondCharacterKey][0], characterDict[secondCharacterKey][1],
                                     "Buildings", "Action", secondCharacterActionProperty);
            location.setTileProperty(characterDict[thirdCharacterKey][0], characterDict[thirdCharacterKey][1],
                                     "Buildings", "Action", thirdCharacterActionProperty);

            if (firstAllowEntry) {
                location.removeTileProperty(characterDict[firstCharacterKey][0], characterDict[firstCharacterKey][1],
                                     "Back", "TouchAction");
            } else {
                location.setTileProperty(characterDict[firstCharacterKey][0], characterDict[firstCharacterKey][1],
                                     "Back", "TouchAction", firstCharacterActionProperty);
            }
            if (secondAllowEntry) {
                location.removeTileProperty(characterDict[secondCharacterKey][0], characterDict[secondCharacterKey][1],
                                     "Back", "TouchAction");
            } else {
                location.setTileProperty(characterDict[secondCharacterKey][0], characterDict[secondCharacterKey][1],
                                     "Back", "TouchAction", secondCharacterActionProperty);
            }
            if (thirdAllowEntry) {
                location.removeTileProperty(characterDict[thirdCharacterKey][0], characterDict[thirdCharacterKey][1],
                                     "Back", "TouchAction");
            } else {
                location.setTileProperty(characterDict[thirdCharacterKey][0], characterDict[thirdCharacterKey][1],
                                     "Back", "TouchAction", thirdCharacterActionProperty);
            }
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e) {
            SetUpGMCM();
            SetUpCharacterDict();
        }

        private void SetUpCharacterDict() {
            characterDict.Add("Jas", new int[] { 6, 12 });
            characterDict.Add("Marnie", new int[] { 15, 12 });
            characterDict.Add("Shane", new int[] { 21, 13 });
            characterDict.Add("Maru", new int[] { 7, 10 });
            characterDict.Add("Robin", new int[] { 13, 10 });
            characterDict.Add("Demetrius", new int[] { 13, 10 });
            characterDict.Add("Sebastian", new int[] { 1, 3 });
            characterDict.Add("Abigail", new int[] { 13, 11 });
            characterDict.Add("Pierre", new int[] { 20, 11 });
            characterDict.Add("Caroline", new int[] { 20, 11 });
            characterDict.Add("Alex", new int[] { 5, 9 });
            characterDict.Add("Evelyn", new int[] { 10, 9 });
            characterDict.Add("George", new int[] { 10, 9 });
            characterDict.Add("Penny", new int[] { 6, 7 });
            characterDict.Add("PennyBig", new int[] { 4, 18 });
            characterDict.Add("Pam", new int[] { 20, 21 });
            characterDict.Add("Gus", new int[] { 20, 9 });
            characterDict.Add("Lewis", new int[] { 16, 9 });
            characterDict.Add("Jodi", new int[] { 17, 6 });
            characterDict.Add("Kent", new int[] { 17, 6 });
            characterDict.Add("Sam", new int[] { 12, 14 });
            characterDict.Add("Vincent", new int[] { 11, 18 });
            characterDict.Add("Haley", new int[] { 5, 13 });
            characterDict.Add("Emily", new int[] { 16, 12 });
            characterDict.Add("Clint", new int[] { 4, 9 });
        }

        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e) {
            ForestChanges(e);
            MountainChanges(e);
            TownChanges(e);
            OtherLocationChanges(e);
        }

        void ForestChanges(AssetRequestedEventArgs e) {
            if (e.Name.IsEquivalentTo("Maps/Forest")) {
                AnimalShop(e);
                LeahHouse(e);
                WizardHouse(e);
            }
        }

        void MountainChanges(AssetRequestedEventArgs e) {
            if (e.Name.IsEquivalentTo("Maps/Mountain")) {
                ScienceHouse(e);
                AdventureGuild(e);
            }
        }

        void TownChanges(AssetRequestedEventArgs e) {
            if (e.Name.IsEquivalentTo("Maps/Town")) {
                Hospital(e);
                SeedShop(e);
                JoshHouse(e);
                Saloon(e);
                Trailer(e);
                ManorHouse(e);
                SamHouse(e);
                HaleyHouse(e);
                ArchaeologyHouse(e);
                Blacksmith(e);
                JojaMart(e);
            }
        }

        void OtherLocationChanges(AssetRequestedEventArgs e) {
            if (e.Name.IsEquivalentTo("Maps/Beach")) {
                FishShop(e);
                ElliotHouse(e);
            }
            if (e.Name.IsEquivalentTo("Maps/Desert")) {
                SandyHouse(e);
            }
        }

        void AnimalShop(AssetRequestedEventArgs e) {
            e.Edit(asset => {
                IAssetDataForMap editor = asset.AsMap();
                Map map = editor.Data;
                string actionProperty;

                Tile tile = GetTile(map, "Buildings", 90, 15); // Animal shop
                if (tile == null) return;

                if (config.OutsideNormalHours) {
                    actionProperty = "Warp 13 19 AnimalShop";
                } else {
                    actionProperty = "LockedDoorWarp 13 19 AnimalShop 900 1800";
                }

                tile.Properties["Action"] = actionProperty;
            });
        }

        void LeahHouse(AssetRequestedEventArgs e) {
            e.Edit(asset => {
                IAssetDataForMap editor = asset.AsMap();
                Map map = editor.Data;

                Tile tile = GetTile(map, "Buildings", 104, 32); // Leah's house
                if (tile == null) return;

                // conditionals: ignore events, outside normal hours, stranger room entry
                tile.Properties["Action"] = "Warp 7 9 LeahHouse";
            });
        }

        void WizardHouse(AssetRequestedEventArgs e) {
            e.Edit(asset => {
                IAssetDataForMap editor = asset.AsMap();
                Map map = editor.Data;

                Tile tile = GetTile(map, "Buildings", 5, 26); // Wizard's tower
                if (tile == null) return;

                // conditionals: ignore events, outside normal hours
                tile.Properties["Action"] = "Warp 8 24 WizardHouse";
            });
        }

        void FishShop(AssetRequestedEventArgs e) {
            e.Edit(asset => {
                IAssetDataForMap editor = asset.AsMap();
                Map map = editor.Data;

                Tile tile = GetTile(map, "Buildings", 30, 33); // Fish shop
                if (tile == null) return;

                // conditionals: ignore events, outside normal hours
                tile.Properties["Action"] = "Warp 5 9 FishShop";
            });
        }

        void ElliotHouse(AssetRequestedEventArgs e) {
            e.Edit(asset => {
                IAssetDataForMap editor = asset.AsMap();
                Map map = editor.Data;

                Tile tile = GetTile(map, "Buildings", 49, 10); // Elliot's house
                if (tile == null) return;

                // conditionals: ignore events, outside normal hours, stranger room entry
                tile.Properties["Action"] = "Warp 3 9 ElliottHouse";
            });
        }

        void ScienceHouse(AssetRequestedEventArgs e) {
            e.Edit(asset => {
                IAssetDataForMap editor = asset.AsMap();
                Map map = editor.Data;

                Tile tile = GetTile(map, "Buildings", 12, 25); // Science house front door
                if (tile == null) return;

                // conditionals: ignore events, outside normal hours
                tile.Properties["Action"] = "Warp 6 24 ScienceHouse";

                tile = GetTile(map, "Buildings", 8, 20); // Maru's exterior bedroom door
                if (tile == null) return;

                // conditionals: ignore events, outside normal hours, stranger room entry
                tile.Properties["Action"] = "Warp 3 8 ScienceHouse";
            });
        }

        void AdventureGuild(AssetRequestedEventArgs e) {
            e.Edit(asset => {
                IAssetDataForMap editor = asset.AsMap();
                Map map = editor.Data;
                string actionProperty;

                Tile tile = GetTile(map, "Buildings", 76, 8); // Adventurer's guild
                if (tile == null) return;

                // conditionals: ignore events, outside normal hours, early access
                if (config.OutsideNormalHours) {
                    actionProperty = "Warp 6 19 AdventureGuild";
                } else {
                    actionProperty = "LockedDoorWarp 6 19 AdventureGuild 1400 2600";
                }

                tile.Properties["Action"] = actionProperty;

                bool playerIsGuildMember = Game1.player.mailReceived.Contains("guildMember");
                tile = GetTile(map, "Back", 76, 9);

                if (config.OutsideNormalHours && config.EarlyAdventureGuild && !playerIsGuildMember) {
                    tile.Properties["TouchAction"] = "Warp AdventureGuild 6 19";
                } else if (!config.OutsideNormalHours && config.EarlyAdventureGuild && !playerIsGuildMember) {
                    if (Game1.timeOfDay > 1400 && Game1.timeOfDay < 2600) {
                        tile.Properties["TouchAction"] = "Warp AdventureGuild 6 19";
                    }
                } else {
                    tile.Properties.Remove("TouchAction");
                }
            });
        }

        void Hospital(AssetRequestedEventArgs e) {
            e.Edit(asset => {
                IAssetDataForMap editor = asset.AsMap();
                Map map = editor.Data;

                Tile tile = GetTile(map, "Buildings", 36, 55); // Hospital
                if (tile == null) return;

                // conditionals: ignore events, outside normal hours
                tile.Properties["Action"] = "Warp 10 19 Hospital";
            });
        }

        void SeedShop(AssetRequestedEventArgs e) {
            e.Edit(asset => {
                IAssetDataForMap editor = asset.AsMap();
                Map map = editor.Data;

                Tile tile = GetTile(map, "Buildings", 43, 56); // Seed shop left door
                if (tile == null) return;

                // conditionals: ignore events, outside normal hours, open on wednesday
                tile.Properties["Action"] = "Warp 6 29 SeedShop";

                tile = GetTile(map, "Buildings", 44, 56); // Seed shop right door
                if (tile == null) return;

                // conditionals: ignore events, outside normal hours, open on wednesday
                tile.Properties["Action"] = "Warp 6 29 SeedShop";
            });
        }

        void JoshHouse(AssetRequestedEventArgs e) {
            e.Edit(asset => {
                IAssetDataForMap editor = asset.AsMap();
                Map map = editor.Data;

                Tile tile = GetTile(map, "Buildings", 57, 63); // Alex/Evelyn/George's house
                if (tile == null) return;

                // conditionals: ignore events, outside normal hours
                tile.Properties["Action"] = "Warp 9 24 JoshHouse";
            });
        }

        void Saloon(AssetRequestedEventArgs e) {
            e.Edit(asset => {
                IAssetDataForMap editor = asset.AsMap();
                Map map = editor.Data;

                Tile tile = GetTile(map, "Buildings", 45, 70); // Saloon
                if (tile == null) return;

                // conditionals: ignore events, outside normal hours
                tile.Properties["Action"] = "Warp 14 24 Saloon";
            });
        }

        void Trailer(AssetRequestedEventArgs e) {
            e.Edit(asset => {
                IAssetDataForMap editor = asset.AsMap();
                Map map = editor.Data;

                Tile tile = GetTile(map, "Buildings", 72, 68); // Trailer
                if (tile == null) return;

                // conditionals: ignore events, outside normal hours
                tile.Properties["Action"] = "Warp 12 9 Trailer";
            });
        }

        void ManorHouse(AssetRequestedEventArgs e) {
            e.Edit(asset => {
                IAssetDataForMap editor = asset.AsMap();
                Map map = editor.Data;

                Tile tile = GetTile(map, "Buildings", 58, 85); // Mayor's manor left door
                if (tile == null) return;

                // conditionals: ignore events, outside normal hours
                tile.Properties["Action"] = "Warp 4 11 ManorHouse";

                tile = GetTile(map, "Buildings", 59, 85); // Mayor's manor right door
                if (tile == null) return;

                // conditionals: ignore events, outside normal hours
                tile.Properties["Action"] = "Warp 5 11 ManorHouse";
            });
        }

        void SamHouse(AssetRequestedEventArgs e) {
            e.Edit(asset => {
                IAssetDataForMap editor = asset.AsMap();
                Map map = editor.Data;

                Tile tile = GetTile(map, "Buildings", 10, 85); // Sam/Vincent/Jodi/Kent's house
                if (tile == null) return;

                // conditionals: ignore events, outside normal hours
                tile.Properties["Action"] = "Warp 4 23 SamHouse";
            });
        }

        void HaleyHouse(AssetRequestedEventArgs e) {
            e.Edit(asset => {
                IAssetDataForMap editor = asset.AsMap();
                Map map = editor.Data;

                Tile tile = GetTile(map, "Buildings", 20, 88); // Haley/Emily's house
                if (tile == null) return;

                // conditionals: ignore events, outside normal hours
                tile.Properties["Action"] = "Warp 2 24 HaleyHouse";
            });
        }

        void ArchaeologyHouse(AssetRequestedEventArgs e) {
            e.Edit(asset => {
                IAssetDataForMap editor = asset.AsMap();
                Map map = editor.Data;

                Tile tile = GetTile(map, "Buildings", 101, 89); // Museum
                if (tile == null) return;

                // conditionals: ignore events, outside normal hours
                tile.Properties["Action"] = "Warp 3 14 ArchaeologyHouse";
            });
        }

        void Blacksmith(AssetRequestedEventArgs e) {
            e.Edit(asset => {
                IAssetDataForMap editor = asset.AsMap();
                Map map = editor.Data;

                Tile tile = GetTile(map, "Buildings", 94, 81); // Blacksmith
                if (tile == null) return;

                // conditionals: ignore events, outside normal hours
                tile.Properties["Action"] = "Warp 5 19 Blacksmith";
            });
        }

        void JojaMart(AssetRequestedEventArgs e) {
            e.Edit(asset => {
                IAssetDataForMap editor = asset.AsMap();
                Map map = editor.Data;

                Tile tile = GetTile(map, "Buildings", 95, 50); // Joja Mart left door
                if (tile == null) return;

                // conditionals: ignore events, outside normal hours
                tile.Properties["Action"] = "Warp 13 29 JojaMart";

                tile = GetTile(map, "Buildings", 96, 50); // Joja Mart right door
                if (tile == null) return;

                // conditionals: ignore events, outside normal hours
                tile.Properties["Action"] = "Warp 14 29 JojaMart";
            });
        }

        void SandyHouse(AssetRequestedEventArgs e) {
            e.Edit(asset => {
                IAssetDataForMap editor = asset.AsMap();
                Map map = editor.Data;

                Tile tile = GetTile(map, "Buildings", 6, 51); // Oasis shop
                if (tile == null) return;

                // conditionals: ignore events, outside normal hours
                tile.Properties["Action"] = "Warp 4 9 SandyHouse";
            });
        }

        Tile GetTile(Map map, string layerName, int tileX, int tileY) {
            Layer layer = map.GetLayer(layerName);
            Location pixelPosition = new Location(tileX * Game1.tileSize, tileY * Game1.tileSize);

            return layer.PickTile(pixelPosition, Game1.viewport.Size);
        }

        void SetUpGMCM() {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => config = new ModConfig(),
                save: () => Helper.WriteConfig(config)
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("outside-hours.label"),
                tooltip: () => Helper.Translation.Get("outside-hours.tooltip"),
                getValue: () => config.OutsideNormalHours,
                setValue: value => config.OutsideNormalHours = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("stranger-entry.label"),
                tooltip: () => Helper.Translation.Get("stranger-entry.tooltip"),
                getValue: () => config.StrangerRoomEntry,
                setValue: value => config.StrangerRoomEntry = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("early-guild.label"),
                tooltip: () => Helper.Translation.Get("early-guild.tooltip"),
                getValue: () => config.EarlyAdventureGuild,
                setValue: value => config.EarlyAdventureGuild = value
            );
        }

    }
}
