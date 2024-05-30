/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rokugin/StardewMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.TerrainFeatures;
using System.Reflection.Emit;
using System.Threading;

namespace SafeLightningUpdated {
    internal class ModEntry : Mod {

        static ModConfig Config;
        static IMonitor StaticMonitor;

        public override void Entry(IModHelper helper) {
            Config = helper.ReadConfig<ModConfig>();
            StaticMonitor = Monitor;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(Utility), nameof(Utility.performLightningUpdate)),
                transpiler: new HarmonyMethod(typeof(ModEntry), nameof(UtilityPerformLightningUpdate_Transpiler))
            );
        }

        static bool GetStrikeStatus() {
            bool allow = Config.ModEnabled ? Config.DisableStrikes : false;
            return allow;
        }

        static bool GetModStatus() {
            return Config.ModEnabled;
        }

        static IEnumerable<CodeInstruction> UtilityPerformLightningUpdate_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
            var strikeStatusMethod = AccessTools.Method(typeof(ModEntry), nameof(GetStrikeStatus));
            var modStatusMethod = AccessTools.Method(typeof (ModEntry), nameof(GetModStatus));

            var matcher = new CodeMatcher(instructions, generator);

            matcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldloc_1),
                new CodeMatch(OpCodes.Ldfld),
                new CodeMatch(OpCodes.Ldloc_S),
                new CodeMatch(OpCodes.Callvirt),
                new CodeMatch(OpCodes.Ldfld),
                new CodeMatch(OpCodes.Ldstr, "(O)787")
            ).ThrowIfNotMatch("Couldn't find match for lightning rod loop start");

            matcher.MatchStartForward(new CodeMatch(OpCodes.Ret)
            ).ThrowIfNotMatch("Couldn't find match for lightning rod loop return");

            matcher.CreateLabel(out Label rodReturnLabel);

            matcher.MatchStartBackwards(new CodeMatch(OpCodes.Ldloc_1)
            ).ThrowIfNotMatch("Couldn't find match for lightning rod strike event fire");

            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Call, strikeStatusMethod),
                new CodeInstruction(OpCodes.Brtrue, rodReturnLabel)
            );

            matcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldloc_S),
                new CodeMatch(OpCodes.Ldloc_S),
                new CodeMatch(OpCodes.Ldc_I4_1),
                new CodeMatch(OpCodes.Callvirt),
                new CodeMatch(OpCodes.Ldloc_2)
            ).ThrowIfNotMatch("Couldn't find match for skip fruit tree struck countdown location");

            matcher.CreateLabel(out Label skipStruckLabel);

            matcher.MatchStartBackwards(
                new CodeMatch(OpCodes.Ldloc_S),
                new CodeMatch(OpCodes.Ldfld),
                new CodeMatch(OpCodes.Ldc_I4_4),
                new CodeMatch(OpCodes.Callvirt)
            ).ThrowIfNotMatch("Couldn't find match for fruit tree struck countdown");

            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Call, modStatusMethod),
                new CodeInstruction(OpCodes.Brtrue, skipStruckLabel)
            );

            matcher.MatchEndForward(
                new CodeMatch(OpCodes.Ldloc_S),
                new CodeMatch(OpCodes.Ldnull),
                new CodeMatch(OpCodes.Ldc_I4_S),
                new CodeMatch(OpCodes.Ldloc_S)
            ).ThrowIfNotMatch("Couldn't find match for after terrain damage location");

            matcher.CreateLabel(out Label afterDamageLabel);

            matcher.MatchStartBackwards(new CodeMatch(OpCodes.Ldc_I4_S)
            ).ThrowIfNotMatch("Couldn't find match for terrain damage amount");

            matcher.InsertAndAdvance(
                new CodeMatch(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Call, modStatusMethod),
                new CodeInstruction(OpCodes.Brtrue, afterDamageLabel),
                new CodeMatch(OpCodes.Pop)
            );

            matcher.MatchEndForward(
                new CodeMatch(OpCodes.Ldloc_S),
                new CodeMatch(OpCodes.Callvirt),
                new CodeMatch(OpCodes.Pop),
                new CodeMatch(OpCodes.Ldloc_2)
            ).ThrowIfNotMatch("Couldn't find match for remove terrain feature skip location");

            matcher.CreateLabel(out Label terrainSkipLabel);

            matcher.MatchStartBackwards(new CodeMatch(OpCodes.Ldloc_1)
            ).ThrowIfNotMatch("Couldn't find match for remove terrain feature");

            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Call, modStatusMethod),
                new CodeInstruction(OpCodes.Brtrue, terrainSkipLabel)
            );

            matcher.MatchEndForward(
                new CodeMatch(OpCodes.Ldloc_1),
                new CodeMatch(OpCodes.Ldfld),
                new CodeMatch(OpCodes.Ldloc_2),
                new CodeMatch(OpCodes.Callvirt),
                new CodeMatch(OpCodes.Ret)
            ).ThrowIfNotMatch("Couldn't find match for skip terrain feature strike event fire location");

            matcher.CreateLabel(out Label featureReturnLabel);

            matcher.MatchStartBackwards(new CodeMatch(OpCodes.Ldloc_1)
            ).ThrowIfNotMatch("Couldn't find match for terrain feature strike event start");

            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Call, strikeStatusMethod).MoveLabelsFrom(matcher.Instruction),
                new CodeInstruction(OpCodes.Brtrue, featureReturnLabel)
            );

            matcher.MatchEndForward(
                new CodeMatch(OpCodes.Ldloc_1),
                new CodeMatch(OpCodes.Ldfld),
                new CodeMatch(OpCodes.Ldloc_S),
                new CodeMatch(OpCodes.Callvirt),
                new CodeMatch(OpCodes.Ret)
            ).ThrowIfNotMatch("Couldn't find match for skip small flash strike event location");

            matcher.CreateLabel(out Label smallReturnLabel);

            matcher.MatchStartBackwards(new CodeMatch(OpCodes.Ldloc_1)
            ).ThrowIfNotMatch("Couldn't find match for small flash strike event");

            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Call, strikeStatusMethod),
                new CodeInstruction(OpCodes.Brtrue, smallReturnLabel)
            );

            return matcher.InstructionEnumeration();
        }

        private void OnGameLaunched(object? sender, StardewModdingAPI.Events.GameLaunchedEventArgs e) {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;

            configMenu.Register(
                mod: this.ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(Config)
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => Helper.Translation.Get("enable-mod.label"),
                tooltip: () => Helper.Translation.Get("enable-mod.tooltip"),
                getValue: () => Config.ModEnabled,
                setValue: value => Config.ModEnabled = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => Helper.Translation.Get("disable-strikes.label"),
                tooltip: () => Helper.Translation.Get("disable-strikes.tooltip"),
                getValue: () => Config.DisableStrikes,
                setValue: value => Config.DisableStrikes = value
            );
        }

    }
}
