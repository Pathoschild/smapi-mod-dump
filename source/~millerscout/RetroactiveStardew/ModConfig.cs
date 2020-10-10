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
