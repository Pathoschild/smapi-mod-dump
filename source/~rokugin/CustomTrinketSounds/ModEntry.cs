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
using HarmonyLib;
using StardewValley;
using System.Reflection.Emit;
using StardewValley.Companions;
using StardewValley.Objects;

namespace CustomTrinketSounds {
    internal class ModEntry : Mod {

        public static ModConfig? Config;
        public static IMonitor? SMonitor;

        public override void Entry(IModHelper helper) {
            Config = helper.ReadConfig<ModConfig>();
            SMonitor = Monitor;

            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(FlyingCompanion), nameof(FlyingCompanion.Update)),
                transpiler: new HarmonyMethod(typeof(ModEntry), nameof(FlyingCompanion_Update_Transpiler))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(IceOrbTrinketEffect), nameof(IceOrbTrinketEffect.Update)),
                transpiler: new HarmonyMethod(typeof(ModEntry), nameof(IceOrbTrinketEffect_Update_Transpiler))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(HungryFrogCompanion), nameof(HungryFrogCompanion.Update)),
                transpiler: new HarmonyMethod(typeof(ModEntry), nameof(HungryFrogCompanion_Update_Transpiler))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(HungryFrogCompanion), nameof(HungryFrogCompanion.tongueReachedMonster)),
                transpiler: new HarmonyMethod(typeof(ModEntry), nameof(HungryFrogCompanion_tongueReachedMonster_Transpiler))
            );
        }

        static string GetParrotAudioCueName() {
            string cueName = Game1.soundBank.Exists("parrotCompanion") ? "parrotCompanion" : "parrot_squawk";
            if (Config!.Debug) SMonitor!.Log($"Playing audio cue: {cueName}.", LogLevel.Info);
            return cueName;
        }

        static IEnumerable<CodeInstruction> FlyingCompanion_Update_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
            var method = AccessTools.Method(typeof(ModEntry), nameof(GetParrotAudioCueName));

            var matcher = new CodeMatcher(instructions, generator);

            matcher.MatchStartForward(new CodeMatch(OpCodes.Ldstr, "parrot_squawk")).ThrowIfNotMatch("Couldn't find match for parrot audio name");

            matcher.Set(OpCodes.Call, method);

            return matcher.InstructionEnumeration();
        }

        static string GetIceOrbAudioCueName() {
            string cueName = Game1.soundBank.Exists("iceball") ? "iceball" : "fireball";
            if (Config!.Debug) SMonitor!.Log($"Playing audio cue: {cueName}.", LogLevel.Info);
            return cueName;
        }

        static IEnumerable<CodeInstruction> IceOrbTrinketEffect_Update_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
            var method = AccessTools.Method(typeof(ModEntry), nameof(GetIceOrbAudioCueName));

            var matcher = new CodeMatcher(instructions, generator);

            matcher.MatchStartForward(new CodeMatch(OpCodes.Ldstr, "fireball")).ThrowIfNotMatch("Couldn't find match for ice orb audio name");

            matcher.Set(OpCodes.Call, method);

            return matcher.InstructionEnumeration();
        }

        static string GetFrogAudioCueName() {
            string cueName = Game1.soundBank.Exists("frogCompanion") ? "frogCompanion" : "croak";
            if (Config!.Debug) SMonitor!.Log($"Playing audio cue: {cueName}.", LogLevel.Info);
            return cueName;
        }

        static IEnumerable<CodeInstruction> HungryFrogCompanion_Update_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
            var method = AccessTools.Method(typeof(ModEntry), nameof(GetFrogAudioCueName));

            var matcher = new CodeMatcher(instructions, generator);

            matcher.MatchStartForward(new CodeMatch(OpCodes.Ldstr, "croak")).ThrowIfNotMatch("Couldn't find match for croak audio name");

            matcher.Set(OpCodes.Call, method);

            return matcher.InstructionEnumeration();
        }

        static string GetTongueAudioCueName() {
            string cueName = Game1.soundBank.Exists("tongueHit") ? "tongueHit" : "fishSlap";
            if (Config!.Debug) SMonitor!.Log($"Playing audio cue: {cueName}.", LogLevel.Info);
            return cueName;
        }

        static IEnumerable<CodeInstruction> HungryFrogCompanion_tongueReachedMonster_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
            var method = AccessTools.Method(typeof(ModEntry), nameof(GetTongueAudioCueName));

            var matcher = new CodeMatcher(instructions, generator);

            matcher.MatchStartForward(new CodeMatch(OpCodes.Ldstr, "fishSlap")).ThrowIfNotMatch("Couldn't find match for tongue audio name");

            matcher.Set(OpCodes.Call, method);

            return matcher.InstructionEnumeration();
        }

    }
}
