/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using System.Collections.Generic;
using SkillPrestige.Logging;
using SkillPrestige.Professions;

namespace SkillPrestige.Framework.Professions.Registration
{
    // ReSharper disable once UnusedMember.Global - created through reflection.
    internal sealed class FishingRegistration : ProfessionRegistration
    {
        /*********
        ** Public methods
        *********/
        public override void RegisterProfessions()
        {
            Logger.LogInformation("Registering Fishing professions...");
            Fisher = new TierOneProfession
            {
                Id = 6
            };
            Trapper = new TierOneProfession
            {
                Id = 7
            };
            Angler = new TierTwoProfession
            {
                Id = 8,
                TierOneProfession = Fisher
            };
            Pirate = new TierTwoProfession
            {
                Id = 9,
                TierOneProfession = Fisher
            };
            Mariner = new TierTwoProfession
            {
                Id = 10,
                TierOneProfession = Trapper
            };
            Luremaster = new TierTwoProfession
            {
                Id = 11,
                TierOneProfession = Trapper
            };
            Fisher.TierTwoProfessions = new List<TierTwoProfession>
            {
                Angler,
                Pirate
            };
            Trapper.TierTwoProfessions = new List<TierTwoProfession>
            {
                Mariner,
                Luremaster
            };
            Logger.LogInformation("Fishing professions registered.");
        }
    }
}
