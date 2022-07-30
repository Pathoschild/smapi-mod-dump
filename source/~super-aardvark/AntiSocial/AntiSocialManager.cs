/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/super-aardvark/AardvarkMods-SDV
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SuperAardvark.AntiSocial
{
    /// <summary>
    /// This class can be copied into any mod to provide ad-hoc AntiSocial functionality.  Just call AntiSocialManager.DoSetupIfNecessary in your mod's Entry method.
    /// </summary>
    public class AntiSocialManager
    {
        public const string AssetName = "Data/AntiSocialNPCs";
        public const string OriginModId = "SuperAardvark.AntiSocial";

        private static Mod modInstance;
        private static bool adHoc = false;

        private static IAssetName asset = null!;

        private static Lazy<HashSet<string>> antisocials = new(GetAntiSocials);

        public static AntiSocialManager Instance { get; private set; }

        /// <summary>
        /// Checks for the AntiSocial stand-alone mod before running setup.
        /// </summary>
        /// <param name="modInstance">A reference to your Mod class.</param>
        public static void DoSetupIfNecessary(Mod modInstance)
        {
            if (modInstance.ModManifest.UniqueID.Equals(OriginModId))
            {
                modInstance.Monitor.Log("AntiSocial Mod performing stand-alone setup.", LogLevel.Debug);
                adHoc = false;
                DoSetup(modInstance);
            }
            else if (modInstance.Helper.ModRegistry.IsLoaded(OriginModId))
            {
                modInstance.Monitor.Log("AntiSocial Mod loaded.  Skipping ad hoc setup.", LogLevel.Debug);
            }
            else if (AntiSocialManager.modInstance is not null)
            {
                modInstance.Monitor.Log("AntiSocial setup was already completed.", LogLevel.Debug);
            }
            else
            {
                modInstance.Monitor.Log($"AntiSocial Mod not loaded.  No problem; performing ad hoc setup for {modInstance.ModManifest.Name}.", LogLevel.Debug);
                adHoc = true;
                DoSetup(modInstance);
            }
        }

        private static HashSet<string> GetAntiSocials()
            => Game1.content.Load<Dictionary<string, string>>(AssetName).Select((kvp) => kvp.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Sets up AntiSocial.
        /// </summary>
        /// <param name="modInstance"></param>
        private static void DoSetup(Mod modInstance)
        {
            if (Instance != null)
            {
                modInstance.Monitor.Log($"AntiSocial setup already completed by {AntiSocialManager.modInstance.ModManifest.Name} ({AntiSocialManager.modInstance.ModManifest.UniqueID}).", LogLevel.Warn);
                return;
            }

            Instance = new AntiSocialManager();
            AntiSocialManager.modInstance = modInstance;

            asset = modInstance.Helper.GameContent.ParseAssetName(AssetName);

            modInstance.Helper.Events.Content.AssetRequested += OnAssetRequested;
            modInstance.Helper.Events.Content.AssetsInvalidated += OnAssetInvalidated;

            Harmony harmony = new(OriginModId);
            harmony.Patch(original: AccessTools.DeclaredPropertyGetter(typeof(NPC), "CanSocialize"), 
                                  postfix: new HarmonyMethod(typeof(AntiSocialManager), "get_CanSocialize_Postfix"));
            harmony.Patch(original: AccessTools.Method(typeof(Utility), "getRandomTownNPC", new Type[] { typeof(Random) }),
                                  transpiler: new HarmonyMethod(typeof(AntiSocialManager), "getRandomTownNPC_Transpiler"));
            harmony.Patch(original: AccessTools.Method(typeof(SocializeQuest), "loadQuestInfo"),
                                  transpiler: new HarmonyMethod(typeof(AntiSocialManager), "loadQuestInfo_Transpiler"));

        }

        /// <summary>
        /// Listen for asset invalidations, refresh the static cache of antisocial npcs.
        /// </summary>
        /// <param name="sender">SMAPI.</param>
        /// <param name="e">event args.</param>
        private static void OnAssetInvalidated(object sender, AssetsInvalidatedEventArgs e)
        {
            if (antisocials.IsValueCreated && e.NamesWithoutLocale.Contains(asset))
            {
                antisocials = new(GetAntiSocials);
            }
        }

        private static void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo(AssetName))
            {
                e.LoadFrom(() => new Dictionary<string, string>(), AssetLoadPriority.Low);
            }
        }

        private static void get_CanSocialize_Postfix(
            ref bool __result,
            NPC __instance)
        {
            try
            {
                if (__result && antisocials.Value.Contains(__instance.Name))
                {
                    // Log($"Overriding CanSocialize for {__instance.Name}");
                    __result = false;
                }
            }
            catch (Exception ex)
            {
                Log($"Error in get_CanSocialize postfix patch: {ex}", LogLevel.Error);
            }
        }

        private static IEnumerable<CodeInstruction>? getRandomTownNPC_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            try 
            { 
                Log("Patching getRandomTownNPC...", LogLevel.Trace);
                return PatchNPCDispositions(instructions);
            }
            catch (Exception ex)
            {
                Log($"Error in getRandomTownNPC transpiler patch: {ex}", LogLevel.Error);
                return null;
            }
        }

        private static IEnumerable<CodeInstruction>? loadQuestInfo_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                Log("Patching loadQuestInfo...", LogLevel.Trace);
                return PatchNPCDispositions(instructions);
            }
            catch (Exception ex)
            {
                Log($"Error in loadQuestInfo transpiler patch: {ex}", LogLevel.Error);
                return null;
            }
        }

        private static IEnumerable<CodeInstruction> PatchNPCDispositions(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();
            for (int i = 0; i < codes.Count; i++)
            {
                CodeInstruction instr = codes[i];
                //Log($"{instr.opcode} : {instr.operand}");
                if (instr.opcode == OpCodes.Callvirt && instr.operand is MethodInfo method && method.Name == "Load")
                {
                    CodeInstruction prevInstr = codes[i - 1];
                    if (prevInstr.opcode == OpCodes.Ldstr && prevInstr.operand.Equals("Data\\NPCDispositions"))
                    {
                        Log($"Adding call to RemoveAntiSocialNPCs at index {i + 1}");
                        codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AntiSocialManager), nameof(RemoveAntiSocialNPCs))));
                    }
                }
            }
            return codes;
        }

        private static Dictionary<string, string> RemoveAntiSocialNPCs(Dictionary<string, string> dict)
        {
            try
            {
                Dictionary<string, string> copy = dict.Where((kvp) => !antisocials.Value.Contains(kvp.Key)).ToDictionary((kvp) => kvp.Key, (kvp) => kvp.Value);

                Log($"Initially {dict.Count} NPCs, removed anti-social ones, returning {copy.Count}");
                if (copy.Count == 0)
                {
                    Log($"No social NPCs found", LogLevel.Warn);
                    return dict;
                }
                return copy;
            }
            catch (Exception ex)
            {
                Log($"Error in RemoveAntiSocialNPCs: {ex}", LogLevel.Error);
            }
            return dict;
        }

        private static void Log(String message, LogLevel level = LogLevel.Trace)
        {
            modInstance?.Monitor.Log((adHoc ? "[AntiSocial] " + message : message), level);
        }
    }
}
