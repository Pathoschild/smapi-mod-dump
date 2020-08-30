using Harmony;
using PurrplingCore.Patching;
using StardewValley;
using System;
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
    internal class GetCharacterPatch : Patch<GetCharacterPatch>
    {
        private CompanionManager Manager { get; set; }
        public override string Name => nameof(GetCharacterPatch);

        public GetCharacterPatch(CompanionManager manager) 
        {
            this.Manager = manager;
            Instance = this;
        }

        private static void After_getCharacterFromName(ref NPC __result, string name)
        {
            try
            {
                if (__result == null && Instance.Manager.PossibleCompanions.TryGetValue(name, out var csm) && csm.Companion?.currentLocation != null)
                {
                    __result = csm.Companion;
                }
            } 
            catch (Exception ex)
            {
                Instance.LogFailure(ex, nameof(After_getCharacterFromName));
            }
        }

        private static bool MatchGetCharacterFromNameMethod(MethodInfo m)
        {
            return m.Name == nameof(Game1.getCharacterFromName) && m.ReturnType == typeof(NPC) && !m.IsGenericMethod;
        }

        protected override void Apply(HarmonyInstance harmony)
        {
            harmony.Patch(
                original: AccessTools.GetDeclaredMethods(typeof(Game1)).Find(MatchGetCharacterFromNameMethod),
                postfix: new HarmonyMethod(typeof(GetCharacterPatch), nameof(GetCharacterPatch.After_getCharacterFromName))
            );
        }
    }
}
