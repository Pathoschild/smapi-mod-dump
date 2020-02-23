using Harmony;
using NpcAdventure.Internal;
using StardewValley;
using System.Reflection;

namespace NpcAdventure.Patches
{
    /// <summary>
    /// This Patch fixes broken vanilla game functionality, if we have recruited NPC, Game1.getCharacterFromName returns null
    /// This is a fix that repair getCharacterFromName returns a instance of our recruited companion NPC and keep vanilla functionality
    /// Recruited NPC has a flag `eventActor` set to true and original function can't return event actors, 
    /// but our companion is not event actor in real, just uses this flag for disable unattended functionality 
    /// like can't walk through invisible NPC barrier and etc. We want avoid this functionality but keep the vanilla functionality 
    /// of getCharacterFromName because companion is not real event actor it's fake event actor. They are still regular villager NPC.
    /// </summary>
    internal class GetCharacterPatch
    {
        private static readonly SetOnce<CompanionManager> manager = new SetOnce<CompanionManager>();
        private static CompanionManager Manager { get => manager.Value; set => manager.Value = value; }

        internal static void After_getCharacterFromName(ref NPC __result, string name)
        {
            if (__result == null && Manager.PossibleCompanions.TryGetValue(name, out var csm) && csm.Companion?.currentLocation != null)
            {
                __result = csm.Companion;
            }
        }

        internal static void Setup(HarmonyInstance harmony, CompanionManager manager)
        {
            Manager = manager;
            
            harmony.Patch(
                original: AccessTools.GetDeclaredMethods(typeof(Game1)).Find(MatchGetCharacterFromNameMethod),
                postfix: new HarmonyMethod(typeof(GetCharacterPatch), nameof(GetCharacterPatch.After_getCharacterFromName))
            );
        }

        private static bool MatchGetCharacterFromNameMethod(MethodInfo m)
        {
            return m.Name == nameof(Game1.getCharacterFromName) && m.ReturnType == typeof(NPC) && !m.IsGenericMethod;
        }
    }
}
