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
    internal sealed class CombatRegistration : ProfessionRegistration
    {
        /*********
        ** Public methods
        *********/
        public override void RegisterProfessions()
        {
            Logger.LogInformation("Registering Combat professions...");
            Fighter = new TierOneProfession
            {
                Id = 24,
                SpecialHandling = new FighterSpecialHandling(),
            };
            Scout = new TierOneProfession
            {
                Id = 25,
            };
            Brute = new TierTwoProfession
            {
                Id = 26,
                TierOneProfession = Fighter
            };
            Defender = new TierTwoProfession
            {
                Id = 27,
                SpecialHandling = new DefenderSpecialHandling(),
                TierOneProfession = Fighter
            };
            Acrobat = new TierTwoProfession
            {
                Id = 28,
                TierOneProfession = Scout
            };
            Desperado = new TierTwoProfession
            {
                Id = 29,
                TierOneProfession = Scout
            };
            Scout.TierTwoProfessions = new List<TierTwoProfession>
            {
                Acrobat,
                Desperado
            };
            Fighter.TierTwoProfessions = new List<TierTwoProfession>
            {
                Brute,
                Defender
            };
            Logger.LogInformation("Combat professions registered.");
        }
    }
}
