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
    internal sealed class FarmingRegistration : ProfessionRegistration
    {
        public override void RegisterProfessions()
        {
            Logger.LogInformation("Registering Farming professions...");
            Rancher = new TierOneProfession
            {
                Id = 0
            };
            Tiller = new TierOneProfession
            {
                Id = 1
            };
            Coopmaster = new TierTwoProfession
            {
                Id = 2,
                TierOneProfession = Rancher
            };
            Shepherd = new TierTwoProfession
            {
                Id = 3,
                TierOneProfession = Rancher
            };
            Artisan = new TierTwoProfession
            {
                Id = 4,
                TierOneProfession = Tiller
            };
            Agriculturist = new TierTwoProfession
            {
                Id = 5,
                TierOneProfession = Tiller
            };
            Rancher.TierTwoProfessions = new List<TierTwoProfession>
            {
                Coopmaster, Shepherd
            };
            Tiller.TierTwoProfessions = new List<TierTwoProfession>
            {
                Artisan, Agriculturist
            };
            Logger.LogInformation("Farming professions registered.");
        }
    }
}
