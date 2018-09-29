using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceCore;
using Microsoft.Xna.Framework.Graphics;

namespace EquivalentExchange
{
    public class AlchemySkill : Skills.Skill
    {

        public static SpaceCore.Skills.Skill.Profession ProfessionShaper = null;
        public static SpaceCore.Skills.Skill.Profession ProfessionSage = null;
        public static SpaceCore.Skills.Skill.Profession ProfessionTransmuter = null;
        public static SpaceCore.Skills.Skill.Profession ProfessionAdept = null;
        public static SpaceCore.Skills.Skill.Profession ProfessionAurumancer = null;
        public static SpaceCore.Skills.Skill.Profession ProfessionConduit = null;

        public AlchemySkill() : base("EquivalentExchange.Alchemy")
        {
            Name = "Alchemy";
            Icon = DrawingUtil.alchemySkillIconBordered;
            SkillsPageIcon = DrawingUtil.alchemySkillIcon;

            ExperienceCurve = Alchemy.alchemyExperienceNeededPerLevel;

            ExperienceBarColor = new Microsoft.Xna.Framework.Color(255, 35, 165);

            // Level 5
            ProfessionShaper = new Profession(this, ProfessionHelper.GetProfessionInternalName(ProfessionHelper.OldShaperId))
            {
                Icon = DrawingUtil.alchemyShaperIcon,
                Name = ProfessionHelper.GetProfessionTitleFromNumber(ProfessionHelper.OldShaperId),
                Description = ProfessionHelper.GetProfessionDescription(ProfessionHelper.OldShaperId)
            };
            Professions.Add(ProfessionShaper);

            ProfessionSage = new Profession(this, ProfessionHelper.GetProfessionInternalName(ProfessionHelper.OldSageId))
            {
                Icon = DrawingUtil.alchemySageIcon,
                Name = ProfessionHelper.GetProfessionTitleFromNumber(ProfessionHelper.OldSageId),
                Description = ProfessionHelper.GetProfessionDescription(ProfessionHelper.OldSageId)
            };
            Professions.Add(ProfessionSage);

            ProfessionsForLevels.Add(new ProfessionPair(5, ProfessionShaper, ProfessionSage));

            // Level 10 - track A
            ProfessionTransmuter = new Profession(this, ProfessionHelper.GetProfessionInternalName(ProfessionHelper.OldTransmuterId))
            {
                Icon = DrawingUtil.alchemyTransmuterIcon,
                Name = ProfessionHelper.GetProfessionTitleFromNumber(ProfessionHelper.OldTransmuterId),
                Description = ProfessionHelper.GetProfessionDescription(ProfessionHelper.OldTransmuterId)
            };
            Professions.Add(ProfessionTransmuter);

            ProfessionAdept = new Profession(this, ProfessionHelper.GetProfessionInternalName(ProfessionHelper.OldAdeptId))
            {
                Icon = DrawingUtil.alchemyAdeptIcon,
                Name = ProfessionHelper.GetProfessionTitleFromNumber(ProfessionHelper.OldAdeptId),
                Description = ProfessionHelper.GetProfessionDescription(ProfessionHelper.OldAdeptId)
            };
            Professions.Add(ProfessionAdept);

            ProfessionsForLevels.Add(new ProfessionPair(10, ProfessionTransmuter, ProfessionAdept, ProfessionShaper));

            // Level 10 - track B
            ProfessionAurumancer = new Profession(this, ProfessionHelper.GetProfessionInternalName(ProfessionHelper.OldAurumancerId))
            {
                Icon = DrawingUtil.alchemyAurumancerIcon,
                Name = ProfessionHelper.GetProfessionTitleFromNumber(ProfessionHelper.OldAurumancerId),
                Description = ProfessionHelper.GetProfessionDescription(ProfessionHelper.OldAurumancerId)
            };
            Professions.Add(ProfessionAurumancer);

            ProfessionConduit = new Profession(this, ProfessionHelper.GetProfessionInternalName(ProfessionHelper.OldConduitId))
            {
                Icon = DrawingUtil.alchemyConduitIcon,
                Name = ProfessionHelper.GetProfessionTitleFromNumber(ProfessionHelper.OldConduitId),
                Description = ProfessionHelper.GetProfessionDescription(ProfessionHelper.OldConduitId)
            };
            Professions.Add(ProfessionConduit);

            ProfessionsForLevels.Add(new ProfessionPair(10, ProfessionAurumancer, ProfessionConduit, ProfessionSage));
        }

        public override List<string> GetExtraLevelUpInfo(int level)
        {
            var infoList = new List<string>
            {
                EquivalentExchange.instance.Helper.Translation.Get(Reference.Localizations.LevelUp),
                EquivalentExchange.instance.Helper.Translation.Get(Reference.Localizations.LevelUp2)
            };
            if (level % 2 == 0)
            {
                infoList.Add(EquivalentExchange.instance.Helper.Translation.Get(Reference.Localizations.LevelUpEven));
            }

            return infoList;
        }

        public override string GetSkillPageHoverText(int level)
        {
            var hoverTextString1 = EquivalentExchange.instance.Helper.Translation.Get(Reference.Localizations.SkillHoverText1, new { energy = $"{(level * 10)}" });
            var hoverTextString2 = EquivalentExchange.instance.Helper.Translation.Get(Reference.Localizations.SkillHoverText2, new { efficiency = $"{(level / 2)}" });
            return hoverTextString1 + Environment.NewLine + hoverTextString2;
        }
    }
}
