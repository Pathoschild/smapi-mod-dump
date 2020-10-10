/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/millerscout/StardewMillerMods
**
*************************************************/

namespace RetroactiveStardew
{
    public class ModConfig
    {
        public bool OnlyLog { get; set; } = false;
        public bool AchievementsCheck { get; set; } = true;
        public bool SpecialsCheck { get; set; } = true;
        public bool RetroMail { get; set; } = true;
        public bool ArtisanAchievementShouldUseRecipeCount { get; set; }

        public int RecipeCountForArtisanAchievement { get; set; } = 98;
    }
}
