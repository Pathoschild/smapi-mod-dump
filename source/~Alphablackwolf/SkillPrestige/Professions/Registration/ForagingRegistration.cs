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

namespace SkillPrestige.Professions.Registration
{
    // ReSharper disable once UnusedMember.Global - created through reflection.
    public sealed class ForagingRegistration : ProfessionRegistration
    {
        public override void RegisterProfessions()
        {
            Logger.LogInformation("Registering Foraging professions...");
            Forester = new TierOneProfession
            {
                Id = 12
            };
            Gatherer = new TierOneProfession
            {
                Id = 13
            };
            Lumberjack = new TierTwoProfession
            {
                Id = 14,
                TierOneProfession = Forester
            };
            Tapper = new TierTwoProfession
            {
                Id = 15,
                TierOneProfession = Forester
            };
            Botanist = new TierTwoProfession
            {
                Id = 16,
                TierOneProfession = Gatherer
            };
            Tracker = new TierTwoProfession
            {
                Id = 17,
                TierOneProfession = Gatherer
            };
            Forester.TierTwoProfessions = new List<TierTwoProfession>
            {
                Lumberjack,
                Tapper
            };
            Gatherer.TierTwoProfessions = new List<TierTwoProfession>
            {
                Botanist,
                Tracker
            };
            Logger.LogInformation("Foraging professions registered.");
        }
    }
}
