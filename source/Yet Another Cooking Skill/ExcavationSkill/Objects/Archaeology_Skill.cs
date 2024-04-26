/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using MoonShared;
using Microsoft.Xna.Framework;
using SpaceCore;
using StardewValley;
using SpaceCore.Interface;
using StardewModdingAPI.Events;
using StardewModdingAPI;

namespace ArchaeologySkill
{
    public class Archaeology_Skill : SpaceCore.Skills.Skill
    {
        public static KeyedProfession Archaeology5a;
        public static KeyedProfession Archaeology5b;
        public static KeyedProfession Archaeology10a1;
        public static KeyedProfession Archaeology10a2;
        public static KeyedProfession Archaeology10b1;
        public static KeyedProfession Archaeology10b2;
        public readonly IModHelper _modHelper;

        public Archaeology_Skill() : base("moonslime.Archaeology")
        {
            this.Icon = ModEntry.Assets.IconA;
            if (ModEntry.Config.AlternativeSkillPageIcon == 1)
            {
                this.SkillsPageIcon = ModEntry.Assets.IconBalt;
            } else
            {
                this.SkillsPageIcon = ModEntry.Assets.IconB;
            }
            this.ExperienceBarColor = new Microsoft.Xna.Framework.Color(205, 127, 50);
            this.ExperienceCurve = new[] { 100, 380, 770, 1300, 2150, 3300, 4000, 6900, 10000, 15000 };
            this.AddProfessions(
                Archaeology5a = new KeyedProfession(this, "Archaeology5a", ModEntry.Assets.Archaeology5a, ModEntry.Instance.I18N),
                Archaeology5b = new KeyedProfession(this, "Archaeology5b", ModEntry.Assets.Archaeology5b, ModEntry.Instance.I18N),
                Archaeology10a1 = new KeyedProfession(this, "Archaeology10a1", ModEntry.Assets.Archaeology10a1, ModEntry.Instance.I18N),
                Archaeology10a2 = new KeyedProfession(this, "Archaeology10a2", ModEntry.Assets.Archaeology10a2, ModEntry.Instance.I18N),
                Archaeology10b1 = new KeyedProfession(this, "Archaeology10b1", ModEntry.Assets.Archaeology10b1, ModEntry.Instance.I18N),
                Archaeology10b2 = new KeyedProfession(this, "Archaeology10b2", ModEntry.Assets.Archaeology10b2, ModEntry.Instance.I18N)
            );


        }

        private void AddProfessions(KeyedProfession lvl5A, KeyedProfession lvl5B, KeyedProfession lvl10A1, KeyedProfession lvl10A2, KeyedProfession lvl10B1, KeyedProfession lvl10B2)
        {
            this.Professions.Add(lvl5A);
            this.Professions.Add(lvl5B);
            this.ProfessionsForLevels.Add(new ProfessionPair(5, lvl5A, lvl5B));

            this.Professions.Add(lvl10A1);
            this.Professions.Add(lvl10A2);
            this.ProfessionsForLevels.Add(new ProfessionPair(10, lvl10A1, lvl10A2, lvl5A));

            this.Professions.Add(lvl10B1);
            this.Professions.Add(lvl10B2);
            this.ProfessionsForLevels.Add(new ProfessionPair(10, lvl10B1, lvl10B2, lvl5B));
        }

        public override string GetName()
        {
            return ModEntry.Instance.I18N.Get("skill.name");
        }

        public override List<string> GetExtraLevelUpInfo(int level)
        {
            List<string> result = new()
            {
                ModEntry.Instance.I18N.Get("skill.perk", new { bonus = 5 })
            };            
            return result;
        }

        public override string GetSkillPageHoverText(int level)
        {
            return ModEntry.Instance.I18N.Get("skill.perk", new { bonus = 5 * level });
        }

        
        
    }
}
