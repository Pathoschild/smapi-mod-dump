/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using SkillPrestige.Framework;
using SkillPrestige.Logging;
using SpaceCore;

namespace SkillPrestige.Mods
{
    /// <summary>Handles registering skill mods for the prestige system.</summary>
    public static class ModHandler
    {
        /// <summary>Whether the mod is initialised and ready to register skill mods.</summary>
        private static bool IsInitialized;

        /// <summary>The mods to add once the handler is initialised.</summary>
        private static readonly List<ISkillMod> PendingMods = new();

        private static readonly List<ISpaceCoreSkillMod> SpaceCoreModsPendingLoad = new();

        /// <summary>The registered mods.</summary>
        private static readonly List<ISkillMod> Mods = new();

        public static void RegisterPendingSpaceCoreMods()
        {
            if (!SpaceCoreModsPendingLoad.Any()) return;
            var entriesToRemove = new List<ISpaceCoreSkillMod>();
            foreach (var entry in SpaceCoreModsPendingLoad.Where(entry => Skills.GetSkillList().Contains(entry.SpaceCoreSkillId)))
            {
                Logger.LogDisplay($"Attempting to register {entry.DisplayName}...");
                RegisterModImplementations(entry);
                entriesToRemove.Add(entry);
            }

            foreach (var entry in entriesToRemove)
            {
                SpaceCoreModsPendingLoad.Remove(entry);
            }
        }

        /// <summary>Register a skill mod for the prestige system.</summary>
        /// <param name="mod">The mod you wish to register. the mod and its profession Ids cannot already exist in the system,
        /// and the mod must implement ISkillMod. It is recommended to inherit from SkillPrestige's SkillMod class.</param>
        public static void RegisterMod(ISkillMod mod)
        {
            Logger.LogDisplay($"Registering skill mod {mod.DisplayName}");
            if (IsInitialized)
                RegisterModImplementations(mod);
            else
                PendingMods.Add(mod);
        }

        /// <summary>Initialize the mod handler and add any pending mods.</summary>
        internal static void Initialize()
        {
            IsInitialized = true;

            foreach (var mod in PendingMods)
                RegisterModImplementations(mod);
            PendingMods.Clear();
        }

        /// <summary>Register a skill mod for the prestige system.</summary>
        /// <param name="mod">The mod you wish to register. the mod and its profession Ids cannot already exist in the system,
        /// and the mod must implement ISkillMod. </param>
        private static void RegisterModImplementations(ISkillMod mod)
        {
            if (!IsInitialized)
                throw new InvalidOperationException($"The mod handler is not ready to register skill mods yet.");

            if (mod is ISpaceCoreSkillMod spaceCoreSkillMod)
            {
                Logger.LogInformation($"{mod.DisplayName} mod skill not yet loaded, adding to pending space core mods to load.");
                bool isLoaded = Skills.GetSkillList().Contains(spaceCoreSkillMod.SpaceCoreSkillId);
                if (!isLoaded )
                {
                    if(!SpaceCoreModsPendingLoad.Contains(spaceCoreSkillMod)) SpaceCoreModsPendingLoad.Add(spaceCoreSkillMod);
                    return;
                }
            }

            if (!mod.IsFound)
            {
                Logger.LogInformation($"{mod.DisplayName} Mod not found. Mod not registered.");
                return;
            }

            try
            {
                Logger.LogInformation($"Registering mod: {mod.DisplayName} ...");
                if (Mods.Any(x => x.GetType() == mod.GetType()))
                {
                    Logger.LogWarning($"Cannot load mod: {mod.DisplayName}, as it is already loaded.");
                    return;
                }
                var intersectingMods = GetIntersectingModProfessions(mod);
                if (intersectingMods.Any())
                {
                    Logger.LogWarning($"Cannot load skill mod: {mod.DisplayName}, as it collides with another mod's skills. Details:");
                    foreach (var intersectingMod in intersectingMods)
                        Logger.LogWarning($"Skill mod {mod.DisplayName} registration failed due to {intersectingMod.Key.DisplayName}, for profession ids: {string.Join(",", intersectingMod.Value)}");
                    return;
                }
                Mods.Add(mod);
                Skill.InvalidateSkillsList();

                Logger.LogInformation($"Registered mod: {mod.DisplayName}");
            }
            catch (Exception exception)
            {
                Logger.LogWarning($"Failed to register mod. please ensure mod implements the ISKillMod interface correctly and none of its members generate errors when called. {Environment.NewLine}{exception.Message}{Environment.NewLine}{exception.StackTrace}");
            }
        }

        /// <summary>Get the empty prestiges from mods for saving.</summary>
        public static IEnumerable<Prestige> GetAddedEmptyPrestiges()
        {
            return Mods.Where(x => x.AdditionalPrestiges != null).SelectMany(x => x.AdditionalPrestiges);
        }

        /// <summary>Get the skills added by other mods.</summary>
        public static IEnumerable<Skill> GetAddedSkills()
        {
            return Mods.Where(x => x.AdditionalSkills != null).SelectMany(x => x.AdditionalSkills);
        }

        /// <summary>Get the mods and profession IDs which collide with an already-registered professions ID.</summary>
        /// <param name="mod">The mod to check.</param>
        private static IDictionary<ISkillMod, IEnumerable<int>> GetIntersectingModProfessions(ISkillMod mod)
        {
            var intersectingMods = new Dictionary<ISkillMod, IEnumerable<int>>();
            Logger.LogInformation($"Loaded mods: {Mods.Count}");
            foreach (var loadedMod in Mods)
            {
                var loadedModProfessions = loadedMod.AdditionalSkills?.SelectMany(x => x.GetAllProfessionIds());
                if (loadedModProfessions == null)
                    continue;
                var modProfessions = mod.AdditionalSkills.SelectMany(x => x.GetAllProfessionIds());
                var intersectingProfessions = loadedModProfessions.Intersect(modProfessions).ToList();
                Logger.LogInformation($"intersecting profession for {loadedMod.DisplayName} and {mod.DisplayName}: {intersectingProfessions.Count}");
                if (intersectingProfessions.Any())
                    intersectingMods.Add(loadedMod, intersectingProfessions);
            }
            return intersectingMods;
        }
    }
}
