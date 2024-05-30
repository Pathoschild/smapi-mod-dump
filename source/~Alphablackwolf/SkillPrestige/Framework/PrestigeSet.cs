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
using SkillPrestige.Logging;
using SkillPrestige.Mods;
using SkillPrestige.SkillTypes;

namespace SkillPrestige.Framework
{
    /// <summary>Represents a set of prestiges that are tied to skills.</summary>
    [Serializable]
    internal class PrestigeSet
    {
        public static PrestigeSet Instance { get; set; }

        public static Action Save;
        public static Func<PrestigeSet> Read;

        public static void Load()
        {
            Instance ??= Read();
        }
        public static bool TryLoad()
        {
            try
            {
                Instance ??= Read();
                return Instance is not null;
            }
            catch(Exception exception)
            {
                Logger.LogInformation($"attempted data read for singular data, read failed: {Environment.NewLine} {exception.Message} {Environment.NewLine} {exception.StackTrace}");
                return false;
            }

        }

        public static bool TryRead()
        {
            try
            {
                Read();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static void UnLoad()
        {
            Instance = null;
        }
        public IEnumerable<Prestige> Prestiges { get; set; }

        /// <summary>the default prestige set that contains prestiges for each of the skills in the unmodded version Stardew Valley.</summary>
        private static List<Prestige> DefaultPrestiges =>
            new()
            {
                new Prestige
                {
                    SkillType = SkillType.Farming
                },
                new Prestige
                {
                    SkillType = SkillType.Mining
                },
                new Prestige
                {
                    SkillType = SkillType.Fishing
                },
                new Prestige
                {
                    SkillType = SkillType.Foraging
                },
                new Prestige
                {
                    SkillType = SkillType.Combat
                }
            };

        /// <summary>Returns all prestige set loaded and registered into this mod, default and mod.</summary>
        public static PrestigeSet CompleteEmptyPrestigeSet()
        {
                var prestiges = DefaultPrestiges;
                var addedPrestiges = ModHandler.GetAddedEmptyPrestiges().ToList();
                if (addedPrestiges.Any())
                    prestiges.AddRange(addedPrestiges);
                return new PrestigeSet
                {
                    Prestiges = prestiges
                };
        }
    }
}
