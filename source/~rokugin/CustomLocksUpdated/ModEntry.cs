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

        public override void Entry(IModHelper helper) {
            Config = helper.ReadConfig<ModConfig>();
            monitor = Monitor;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(Mountain), nameof(Mountain.checkAction), [typeof(Location), typeof(Rectangle), typeof(Farmer)]),
                transpiler: new HarmonyMethod(typeof(ModEntry), nameof(MountainCheckAction_Transpiler))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performTouchAction), [typeof(string[]), typeof(Vector2)]),
               transpiler: new HarmonyMethod(typeof(ModEntry), nameof(GameLocationPerformTouchAction_Transpiler))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction), [typeof(string[]), typeof(Farmer), typeof(Location)]),
               transpiler: new HarmonyMethod(typeof(ModEntry), nameof(GameLocationPerformAction_Transpiler))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.lockedDoorWarp)),
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

        static IEnumerable<CodeInstruction> GameLocationPerformAction_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
            var timeMethod = AccessTools.Method(typeof(ModEntry), nameof(GetAllowOutsideTime));
            var roomMethod = AccessTools.Method(typeof(ModEntry), nameof(GetAllowRoomEntry));

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

            matcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldloc_0),
                new CodeMatch(OpCodes.Ldfld),
                new CodeMatch(OpCodes.Ldc_I4_1),
                new CodeMatch(OpCodes.Call)
            ).ThrowIfNotMatch("Couldn't find match for open door");

            matcher.CreateLabel(out Label openDoorLabel);

            matcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldsfld),
                new CodeMatch(OpCodes.Ldstr, "Strings\\Locations:DoorUnlock_NotFriend_Couple"),
                new CodeMatch(OpCodes.Ldloc_S)
            ).ThrowIfNotMatch("Couldn't find match for not friend couple door unlock");

            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Call, roomMethod),
                new CodeInstruction(OpCodes.Brtrue, openDoorLabel)
            );

            matcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldsfld),
                new CodeMatch(OpCodes.Ldstr, "Strings\\Locations:DoorUnlock_NotFriend_"),
                new CodeMatch(OpCodes.Ldloc_S),
                new CodeMatch(OpCodes.Callvirt),
                new CodeMatch(OpCodes.Brfalse_S)
            ).ThrowIfNotMatch("Couldn't find match for not friend couple door unlock");

            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Call, roomMethod),
                new CodeInstruction(OpCodes.Brtrue, openDoorLabel)
            );

            matcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldstr, "doorClose"),
                new CodeMatch(OpCodes.Ldloc_0),
                new CodeMatch(OpCodes.Ldflda),
                new CodeMatch(OpCodes.Ldfld)
            ).ThrowIfNotMatch("Couldn't find match for wizard basement");

            matcher.CreateLabel(out Label wizardLabel);

            matcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldfld),
                new CodeMatch(OpCodes.Ldc_I4_0),
                new CodeMatch(OpCodes.Callvirt),
                new CodeMatch(OpCodes.Stloc_S)
            ).ThrowIfNotMatch("Couldn't find match for wizard hatch dialogue");

            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Call, roomMethod).MoveLabelsFrom(matcher.Instruction),
                new CodeInstruction(OpCodes.Brtrue, wizardLabel)
            );

            matcher.MatchEndForward(
                new CodeMatch(OpCodes.Ldsfld),
                new CodeMatch(OpCodes.Ldc_I4),
                new CodeMatch(OpCodes.Bge_S)
            ).ThrowIfNotMatch("Couldn't find match for open time check");

            var seenLabel = (Label)matcher.Instruction.operand;

            matcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldc_I4, 900),
                new CodeMatch(OpCodes.Call),
                new CodeMatch(OpCodes.Ldstr, " ")
            ).ThrowIfNotMatch("Couldn't find match for theater open range dialogue");

            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Call, timeMethod).MoveLabelsFrom(matcher.Instruction),
                new CodeInstruction(OpCodes.Brtrue, seenLabel)
            );

            return matcher.InstructionEnumeration();
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
