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
using StardewValley.Locations;
using StardewValley;
using HarmonyLib;
using xTile.Dimensions;
using Microsoft.Xna.Framework;
using Rectangle = xTile.Dimensions.Rectangle;
using System.Reflection.Emit;
using System.Reflection;

namespace CustomLocksUpdated {
    public class ModEntry : Mod {

        public static ModConfig Config;
        public static IMonitor monitor;

        static Harmony harmony;

        public override void Entry(IModHelper helper) {
            Config = helper.ReadConfig<ModConfig>();
            monitor = Monitor;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            harmony = new Harmony(ModManifest.UniqueID);
            Harmony.DEBUG = true;

            harmony.Patch(
                original: AccessTools.Method(typeof(Mountain), nameof(Mountain.checkAction), [typeof(Location), typeof(Rectangle), typeof(Farmer)]),
                //prefix: new HarmonyMethod(typeof(ModEntry), nameof(MountainCheckActionPrefix)),
                transpiler: new HarmonyMethod(typeof(ModEntry), nameof(MountainCheckAction_Transpiler))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performTouchAction), [typeof(string[]), typeof(Vector2)]),
               //prefix: new HarmonyMethod(typeof(ModEntry), nameof(GameLocationPerformTouchActionPrefix)),
               transpiler: new HarmonyMethod(typeof(ModEntry), nameof(GameLocationPerformTouchAction_Transpiler))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction), [typeof(string[]), typeof(Farmer), typeof(Location)]),
               //prefix: new HarmonyMethod(typeof(ModEntry), nameof(GameLocationPerformActionPrefix)),
               transpiler: new HarmonyMethod(typeof(ModEntry), nameof(GameLocationPerformAction_Transpiler))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.lockedDoorWarp)),
               //prefix: new HarmonyMethod(typeof(ModEntry), nameof(GameLocationLockedDoorWarp)),
               transpiler: new HarmonyMethod(typeof(ModEntry), nameof(GameLocationLockedDoorWarp_Transpiler))
            );
        }

        static bool GetAllowRoomEntry() {
            bool allow = Config.Enabled ? Config.AllowStrangerRoomEntry : false;
            return allow;
        }

        static bool GetAllowHomeEntry() {
            bool allow = Config.Enabled ? Config.AllowStrangerHomeEntry : false;
            return allow;
        }

        static bool GetAllowEarlyGuild() {
            bool allow = Config.Enabled ? Config.AllowAdventureGuildEntry : false;
            return allow;
        }

        static bool GetAllowOutsideTime() {
            bool allow = Config.Enabled ? Config.AllowOutsideTime : false;
            return allow;
        }

        static bool GetAllowSeedShopWed() {
            bool allow = Config.Enabled ? Config.AllowSeedShopWed : false;
            return allow;
        }

        static bool GetIgnoreEvents() {
            bool allow = Config.Enabled ? Config.IgnoreEvents : false;
            return allow;
        }

        static IEnumerable<CodeInstruction> MountainCheckAction_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
            var method = AccessTools.Method(typeof(ModEntry), nameof(GetAllowEarlyGuild));

            var matcher = new CodeMatcher(instructions, generator);

            matcher.MatchEndForward(
                new CodeMatch(OpCodes.Ldloc_0),
                new CodeMatch(OpCodes.Ldc_I4, 1136),
                new CodeMatch(OpCodes.Bne_Un_S)
            ).ThrowIfNotMatch("Couldn't find match for guild tile index");

            var switchBreakLabel = (Label)matcher.Instruction.operand;

            matcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldsfld),
                new CodeMatch(OpCodes.Ldstr, "Strings\\Locations:Mountain_AdventurersGuildNote"),
                new CodeMatch(i => i.opcode == OpCodes.Callvirt),
                new CodeMatch(OpCodes.Ldc_I4_S)
            ).ThrowIfNotMatch("Couldn't find match for guild note");

            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Call, method),
                new CodeInstruction(OpCodes.Brtrue, switchBreakLabel)
            );

            return matcher.InstructionEnumeration();
        }

        public static bool MountainCheckActionPrefix(Mountain __instance, Location tileLocation, Farmer who) {
            if (!Config.Enabled) return true;

            try {
                if (__instance.getTileIndexAt(tileLocation, "Buildings") == 1136) {
                    if (!Config.AllowAdventureGuildEntry) {
                        return true;
                    } else {
                        if (!Config.AllowOutsideTime) {
                            return true;
                        } else {
                            Rumble.rumble(0.15f, 200f);
                            Game1.player.completelyStopAnimatingOrDoingAction();
                            __instance.playSound("doorClose", Game1.player.Tile);
                            Game1.warpFarmer("AdventureGuild", 6, 19, false);
                            return false;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex) {
                monitor.Log($"Failed in {nameof(MountainCheckActionPrefix)}:\n{ex}", LogLevel.Error);
            }
            return true;
        }

        static IEnumerable<CodeInstruction> GameLocationPerformAction_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
            var timeMethod = AccessTools.Method(typeof(ModEntry), nameof(GetAllowOutsideTime));
            var roomMethod = AccessTools.Method(typeof(ModEntry), nameof(GetAllowRoomEntry));
            var festivalMethod = AccessTools.Method(typeof(ModEntry), nameof(GetIgnoreEvents));

            var matcher = new CodeMatcher(instructions, generator);

            matcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldstr, "doorClose"),
                new CodeMatch(OpCodes.Ldloc_0),
                new CodeMatch(OpCodes.Ldflda)
            ).ThrowIfNotMatch("Couldn't find match for sunroom warp");

            matcher.CreateLabel(out Label sunroomLabel);

            matcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldsfld),
                new CodeMatch(OpCodes.Ldstr, "Strings\\Locations:Caroline_Sunroom_Door"),
                new CodeMatch(OpCodes.Callvirt)
            ).ThrowIfNotMatch("Couldn't find match for sunroom door friends only");

            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Call, roomMethod).MoveLabelsFrom(matcher.Instruction),
                new CodeInstruction(OpCodes.Brtrue, sunroomLabel)
            );

            return matcher.InstructionEnumeration();
        }

        public static bool GameLocationPerformActionPrefix(ref GameLocation __instance, string[] action, Farmer who, Location tileLocation) {
            if (!Config.Enabled) return true;

            try {
                if (!ArgUtility.TryGet(action, 0, out var actionType, out var error)) {
                    return true;
                }

                switch (actionType) {
                    case "WizardHatch":
                        if ((!who.friendshipData.ContainsKey("Wizard") ||
                            who.friendshipData["Wizard"].Points < 1000) && Config.AllowStrangerRoomEntry) {
                            __instance.playSound("doorClose", new Vector2(tileLocation.X, tileLocation.Y));
                            Game1.warpFarmer("WizardHouseBasement", 4, 4, true);
                            return false;
                        }

                        return true;
                    case "Door":
                        if (action.Length > 1) {
                            if (Game1.eventUp && !Config.IgnoreEvents) {
                                return true;
                            } else {
                                bool unlocked = false;
                                for (int i = 1; i < action.Length; i++) {
                                    if (who.getFriendshipHeartLevelForNPC(action[i]) >= 2 ||
                                        Game1.player.mailReceived.Contains("doorUnlock" + action[i])) {
                                        unlocked = true;
                                    }
                                }

                                if (!unlocked) {
                                    if (!Config.AllowStrangerRoomEntry) {
                                        return true;
                                    } else {
                                        Rumble.rumble(0.1f, 100f);
                                        __instance.openDoor(tileLocation, true);
                                        return false;
                                    }
                                }
                            }
                        }

                        return true;
                    case "Warp_Sunroom_Door":
                        if (who.getFriendshipHeartLevelForNPC("Caroline") >= 2) {
                            return true;
                        } else if (Config.AllowStrangerRoomEntry) {
                            __instance.playSound("doorClose", new Vector2(tileLocation.X, tileLocation.Y));
                            Game1.warpFarmer("Sunroom", 5, 13, flip: false);
                            return false;
                        }

                        return true;
                    case "Theater_Entrance":
                        if (!Game1.MasterPlayer.hasOrWillReceiveMail("ccMovieTheater")) {
                            return true;
                        }
                        if (Game1.player.team.movieMutex.IsLocked()) {
                            return true;
                        }
                        if (Game1.isFestival()) {
                            return true;
                        }
                        if (Game1.timeOfDay > 2100 || Game1.timeOfDay < 900) {
                            if (!Config.AllowOutsideTime) return true;
                        }
                        if (Game1.player.lastSeenMovieWeek.Value >= Game1.Date.TotalWeeks) {
                            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Characters:MovieTheater_AlreadySeen"));
                            return false;
                        }
                        NPC invited_npc = null;
                        foreach (MovieInvitation invitation in Game1.player.team.movieInvitations) {
                            if (invitation.farmer == Game1.player && !invitation.fulfilled && MovieTheater.GetFirstInvitedPlayer(invitation.invitedNPC) == Game1.player) {
                                invited_npc = invitation.invitedNPC;
                                break;
                            }
                        }
                        if (Game1.player.Items.ContainsId("(O)809")) {
                            string question = ((invited_npc != null) ?
                                Game1.content.LoadString("Strings\\Characters:MovieTheater_WatchWithFriendPrompt", invited_npc.displayName) :
                                Game1.content.LoadString("Strings\\Characters:MovieTheater_WatchAlonePrompt"));
                            Game1.currentLocation.createQuestionDialogue(question, Game1.currentLocation.createYesNoResponses(), "EnterTheaterSpendTicket");
                        } else {
                            Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieTheater_NoTicket")));
                            return false;
                        }

                        return false;
                }

                return true;
            }
            catch (Exception ex) {
                monitor.Log($"Failed in {nameof(GameLocationPerformActionPrefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }

        static IEnumerable<CodeInstruction> GameLocationPerformTouchAction_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
            var roomMethod = AccessTools.Method(typeof(ModEntry), nameof(GetAllowRoomEntry));

            var matcher = new CodeMatcher(instructions, generator);
            matcher.MatchEndForward(
                new CodeMatch(OpCodes.Ldloc_1),
                new CodeMatch(OpCodes.Ldstr, "Door"),
                new CodeMatch(i => i.opcode == OpCodes.Call),
                new CodeMatch(i => i.opcode == OpCodes.Brtrue)
            ).ThrowIfNotMatch("Couldn't find match for door check.");
            // Cursor is now at the brtrue instruction, save its jump point to find the entry to the for loop.
            var forLoopStart = (Label)matcher.Instruction.operand;
            // Advance to br label and steal the label to escape the switch if we need it
            matcher.MatchStartForward(new CodeMatch(i => i.opcode == OpCodes.Br))
                .ThrowIfNotMatch("Couldn't find switch break");
            var escapeLabel = (Label)matcher.Instruction.operand;

            matcher.MatchStartForward(new CodeMatch(i => i.labels.Contains(forLoopStart)))
                .ThrowIfNotMatch("Couldn't find start of for loop");

            // Insert before that instruction, taking its labels so our code gets jumped to first
            matcher.InsertAndAdvance(
                // Steal labels from instruction, since we're the new start, before the for loop
                new CodeInstruction(OpCodes.Call, roomMethod).MoveLabelsFrom(matcher.Instruction),
                // If we aren't allowing entry, break out of the switch
                new CodeInstruction(OpCodes.Brtrue, escapeLabel)
            );

            return matcher.InstructionEnumeration();
        }

        public static bool GameLocationPerformTouchActionPrefix(GameLocation __instance, string[] action, Vector2 playerStandingPosition) {
            if (!Config.Enabled) return true;

            try {
                if (Game1.eventUp && !Config.IgnoreEvents) return true;

                if (action[0] == "Door" && Config.AllowStrangerRoomEntry) {
                    int i = 1;
                    while (i < action.Length) {
                        if (Game1.player.getFriendshipHeartLevelForNPC(action[i]) >= 2) {
                            return true;
                        }
                        i++;
                    }

                    return false;
                }
            }
            catch (Exception ex) {
                monitor.Log($"Failed in {nameof(GameLocationPerformTouchActionPrefix)}:\n{ex}", LogLevel.Error);
            }
            return true;
        }

        static IEnumerable<CodeInstruction> GameLocationLockedDoorWarp_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
            var timeMethod = AccessTools.Method(typeof(ModEntry), nameof(GetAllowOutsideTime));
            var homeMethod = AccessTools.Method(typeof(ModEntry), nameof(GetAllowHomeEntry));
            var festivalMethod = AccessTools.Method(typeof(ModEntry), nameof(GetIgnoreEvents));
            var wedMethod = AccessTools.Method(typeof(ModEntry), nameof(GetAllowSeedShopWed));

            var matcher = new CodeMatcher(instructions, generator);

            matcher.MatchEndForward(
                new CodeMatch(i => i.opcode == OpCodes.Call),
                new CodeMatch(i => i.opcode == OpCodes.Callvirt),
                new CodeMatch(OpCodes.Stloc_0),
                new CodeMatch(i => i.opcode == OpCodes.Call),
                new CodeMatch(OpCodes.Brfalse_S)
            ).ThrowIfNotMatch("Couldn't find festival check");

            var ignoreFestivalLabel = (Label)matcher.Instruction.operand;

            matcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldsfld),
                new CodeMatch(OpCodes.Ldstr, "Strings\\Locations:FestivalDay_DoorLocked"),
                new CodeMatch(i => i.opcode == OpCodes.Callvirt),
                new CodeMatch(i => i.opcode == OpCodes.Call)
            ).ThrowIfNotMatch("Couldn't find closed for festival sequence");

            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Call, festivalMethod),
                new CodeInstruction(OpCodes.Brtrue, ignoreFestivalLabel)
            );

            matcher.MatchEndForward(
                new CodeMatch(OpCodes.Ldarg_2),
                new CodeMatch(OpCodes.Ldstr, "SeedShop"),
                new CodeMatch(i => i.opcode == OpCodes.Call),
                new CodeMatch(OpCodes.Brfalse_S)
            ).ThrowIfNotMatch("Couldn't find seed shop check");

            var allowWedLabel = (Label)matcher.Instruction.operand;

            matcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldsfld),
                new CodeMatch(OpCodes.Ldstr, "Strings\\Locations:SeedShop_LockedWed"),
                new CodeMatch(i => i.opcode == OpCodes.Callvirt),
                new CodeMatch(i => i.opcode == OpCodes.Call)
            ).ThrowIfNotMatch("Couldn't find closed on wednesday sequence");

            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Call, wedMethod),
                new CodeInstruction(OpCodes.Brtrue, allowWedLabel)
            );

            matcher.MatchEndForward(
                new CodeMatch(OpCodes.Ldarg_2),
                new CodeMatch(OpCodes.Ldstr, "AdventureGuild"),
                new CodeMatch(i => i.opcode == OpCodes.Callvirt),
                new CodeMatch(OpCodes.Brtrue_S)
            ).ThrowIfNotMatch("Couldn't find can open door check");

            var openDoorLabel = (Label)matcher.Instruction.operand;

            matcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldarg_3),
                new CodeMatch(i => i.opcode == OpCodes.Call),
                new CodeMatch(OpCodes.Ldstr, " ")
            ).ThrowIfNotMatch("Couldn't find shop hours sequence");

            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Call, timeMethod),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Stloc_1),
                new CodeInstruction(OpCodes.Brtrue, openDoorLabel)
            );

            matcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldsfld),
                new CodeMatch(OpCodes.Ldstr, "Strings\\Locations:LockedDoor"),
                new CodeMatch(i => i.opcode == OpCodes.Callvirt)
            ).ThrowIfNotMatch("Couldn't find locked door sequence");

            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Call, timeMethod).MoveLabelsFrom(matcher.Instruction),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Stloc_1),
                new CodeInstruction(OpCodes.Brtrue, openDoorLabel)
            );

            matcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldarg_S),
                new CodeMatch(OpCodes.Ldc_I4_1),
                new CodeMatch(OpCodes.Ldc_I4_0),
                new CodeMatch(OpCodes.Call),
                new CodeMatch(OpCodes.Stloc_S),
                new CodeMatch(OpCodes.Ldsfld),
                new CodeMatch(OpCodes.Ldstr, "Strings\\Locations:LockedDoor_FriendsOnly")
            ).ThrowIfNotMatch("Couldn't find friends only locked door sequence");

            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Call, homeMethod).MoveLabelsFrom(matcher.Instruction),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Stloc_1),
                new CodeInstruction(OpCodes.Brtrue, openDoorLabel)
            );

            return matcher.InstructionEnumeration();
        }

        public static bool GameLocationLockedDoorWarp(GameLocation __instance, Point tile, string locationName, int openTime, int closeTime,
            string npcName, int minFriendship) {
            if (!Config.Enabled) return true;

            try {
                bool town_key_applies = Game1.player.HasTownKey;
                if (GameLocation.AreStoresClosedForFestival() && __instance.InValleyContext()) {
                    if (!Config.IgnoreEvents) {
                        return true;
                    }
                }
                if (locationName == "SeedShop" &&
                    Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).Equals("Wed") &&
                    !Utility.HasAnyPlayerSeenEvent("191393") && !town_key_applies) {
                    if (!Config.AllowSeedShopWed) {
                        return true;
                    }
                }
                if (locationName == "FishShop" && Game1.player.mailReceived.Contains("willyHours")) {
                    if (!Config.AllowOutsideTime) {
                        return true;
                    }
                }
                if (town_key_applies) {
                    if (town_key_applies && !__instance.InValleyContext()) {
                        town_key_applies = false;
                    }
                    if (town_key_applies && __instance is BeachNightMarket && locationName != "FishShop") {
                        town_key_applies = false;
                    }
                }
                Friendship friendship;
                bool canOpenDoor = (town_key_applies || (Game1.timeOfDay >= openTime && Game1.timeOfDay < closeTime) || Config.AllowOutsideTime) &&
                    (minFriendship <= 0 || __instance.IsWinterHere() || Config.AllowStrangerHomeEntry ||
                    (Game1.player.friendshipData.TryGetValue(npcName, out friendship) &&
                    friendship.Points >= minFriendship));
                if (__instance.IsGreenRainingHere() && Game1.year == 1 && !(__instance is Beach) &&
                    !(__instance is Forest) && !locationName.Equals("AdventureGuild")) {
                    canOpenDoor = true;
                }
                if (canOpenDoor) {
                    DoWarp(locationName, __instance, tile);
                    return false;
                } else if (minFriendship <= 0) {
                    if (!Config.AllowOutsideTime) {
                        string openTimeString = Game1.getTimeOfDayString(openTime).Replace(" ", "");
                        if (locationName == "FishShop" && Game1.player.mailReceived.Contains("willyHours")) {
                            openTimeString = Game1.getTimeOfDayString(800).Replace(" ", "");
                        }
                        string closeTimeString = Game1.getTimeOfDayString(closeTime).Replace(" ", "");
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:LockedDoor_OpenRange", openTimeString, closeTimeString));
                        return false;
                    } else {
                        DoWarp(locationName, __instance, tile);
                        return false;
                    }
                } else if (Game1.timeOfDay < openTime || Game1.timeOfDay >= closeTime) {
                    if (!Config.AllowOutsideTime) {
                        return true;
                    }
                } else {
                    if (!Config.AllowStrangerHomeEntry) {
                        NPC character = Game1.getCharacterFromName(npcName);
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:LockedDoor_FriendsOnly", character.displayName));
                        return false;
                    } else {
                        DoWarp(locationName, __instance, tile);
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex) {
                monitor.Log($"Failed in {nameof(GameLocationLockedDoorWarp)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }

        internal static void DoWarp(string locationName, GameLocation instance, Point tile) {
            Rumble.rumble(0.15f, 200f);
            Game1.player.completelyStopAnimatingOrDoingAction();
            instance.playSound("doorClose", Game1.player.Tile);
            Game1.warpFarmer(locationName, tile.X, tile.Y, false);
        }

        private void OnGameLaunched(object? sender, StardewModdingAPI.Events.GameLaunchedEventArgs e) {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("overall-enabled.label"),
                tooltip: () => Helper.Translation.Get("overall-enabled.tooltip"),
                getValue: () => Config.Enabled,
                setValue: value => Config.Enabled = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("general-store-wed.label"),
                tooltip: () => Helper.Translation.Get("general-store-wed.tooltip"),
                getValue: () => Config.AllowSeedShopWed,
                setValue: value => Config.AllowSeedShopWed = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("outside-hours.label"),
                tooltip: () => Helper.Translation.Get("outside-hours.tooltip"),
                getValue: () => Config.AllowOutsideTime,
                setValue: value => Config.AllowOutsideTime = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("stranger-home-entry.label"),
                tooltip: () => Helper.Translation.Get("stranger-home-entry.tooltip"),
                getValue: () => Config.AllowStrangerHomeEntry,
                setValue: value => Config.AllowStrangerHomeEntry = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("stranger-room-entry.label"),
                tooltip: () => Helper.Translation.Get("stranger-room-entry.tooltip"),
                getValue: () => Config.AllowStrangerRoomEntry,
                setValue: value => Config.AllowStrangerRoomEntry = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("adventure-guild.label"),
                tooltip: () => Helper.Translation.Get("adventure-guild.tooltip"),
                getValue: () => Config.AllowAdventureGuildEntry,
                setValue: value => Config.AllowAdventureGuildEntry = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("ignore-events.label"),
                tooltip: () => Helper.Translation.Get("ignore-events.tooltip"),
                getValue: () => Config.IgnoreEvents,
                setValue: value => Config.IgnoreEvents = value
            );
        }

    }
}
