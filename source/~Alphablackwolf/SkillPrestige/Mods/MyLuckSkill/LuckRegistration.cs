using System.Collections.Generic;
using SkillPrestige.Logging;
using SkillPrestige.Professions;
using SkillPrestige.Professions.Registration;

namespace SkillPrestige.Mods.MyLuckSkill
{
    // ReSharper disable once UnusedMember.Global - referenced and created through reflection.
    public sealed class LuckRegistration : ProfessionRegistration
    {
        public override void RegisterProfessions()
        {
            Logger.LogInformation("Registering Luck Professions...");
            Lucky = new TierOneProfession
            {
                Id = 30,
                DisplayName = "Lucky",
                EffectText = new[] {"Better daily luck."}
            };
            Quester = new TierOneProfession
            {
                Id = 31,
                DisplayName = "Quester",
                EffectText = new[] {"Quests are more likely to appear each day."},
            };
            SpecialCharm = new TierTwoProfession
            {
                Id = 32,
                DisplayName = "Special Charm",
                EffectText = new[] {"Great daily luck most of the time."},
                SpecialHandling = new SpecialCharmSpecialHandling(),
                TierOneProfession = Lucky
            };
            LuckA2 = new TierTwoProfession
            {
                Id = 33,
                DisplayName = "Luck A2",
                EffectText = new[] {"Does...nothing, yet."},
                TierOneProfession = Lucky
            };
            NightOwl = new TierTwoProfession
            {
                Id = 34,
                DisplayName = "Night Owl",
                EffectText = new[] {"Nightly events occur twice as often."},
                TierOneProfession = Quester
            };
            LuckB2 = new TierTwoProfession
            {
                Id = 35,
                DisplayName = "Luck B2",
                EffectText = new[] {"Does...nothing, yet."},
                TierOneProfession = Quester
            };
            Quester.TierTwoProfessions = new List<TierTwoProfession>
            {
                NightOwl,
                LuckB2
            };
            Lucky.TierTwoProfessions = new List<TierTwoProfession>
            {
                SpecialCharm,
                LuckA2
            };
            Logger.LogInformation("Luck Professions Registered.");
        }
    }
}
