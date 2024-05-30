/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using BirbCore.Attributes;
using MoonShared.Config;

namespace ArchaeologySkill
{
    [SConfig]
    public class Config
    {

        [SConfig.Option(1, 2, 1)]
        public int AlternativeSkillPageIcon { get; set; } = 1;



        [SConfig.Option(0, 100, 1)]
        public int ExperienceFromArtifactSpots { get; set; } = 10;

        [SConfig.Option(0, 100, 1)]
        public int ExperienceFromPanSpots { get; set; } = 20;

        [SConfig.Option(0, 100, 1)]
        public int ExperienceFromMinesDigging { get; set; } = 5;

        [SConfig.Option(0, 100, 1)]
        public int ExperienceFromWaterShifter { get; set; } = 2;



        [SConfig.Option(0, 100, 1)]
        public int ExperienceFromResearchTable { get; set; } = 10;

        [SConfig.Option(0, 100, 1)]
        public int ExperienceFromAncientBattery { get; set; } = 10;


        [SConfig.Option(0, 100, 1)]
        public int ExperienceFromPreservationChamber { get; set; } = 10;


        [SConfig.Option(0, 100, 1)]
        public int ExperienceFromHPreservationChamber { get; set; } = 20;
    }
}
