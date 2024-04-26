/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using HarmonyLib;
using stardew_access.Translation;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace stardew_access.Patches;

// TODO Maybe also include community center progress and season info
internal class SkillsPagePatch : IPatch
{
    public void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(SkillsPage), "draw"),
            postfix: new HarmonyMethod(typeof(SkillsPagePatch), nameof(SkillsPagePatch.DrawPatch))
        );
    }

    private static void DrawPatch(SkillsPage __instance, string ___hoverText, string ___hoverTitle)
    {
        try
        {
            int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

            if (MainClass.Config.PrimaryInfoKey.JustPressed())
            {
                int currentMasteryLevel = MasteryTrackerMenu.getCurrentMasteryLevel();
                int currentMasteryPoints = (int)Game1.stats.Get("MasteryExp") - MasteryTrackerMenu.getMasteryExpNeededForLevel(currentMasteryLevel);
                int requiredMasteryPoints = MasteryTrackerMenu.getMasteryExpNeededForLevel(currentMasteryLevel + 1) - MasteryTrackerMenu.getMasteryExpNeededForLevel(currentMasteryLevel);

                string toSpeak = Translator.Instance.Translate("menu-skills_page-player_info", tokens: new
                {
                    name = Game1.player.Name,
                    title = Game1.player.getTitle(),
                    golden_walnut_count = Game1.netWorldState.Value.GoldenWalnuts,
                    qi_gem_count = Game1.player.QiGems,
                    house_upgrade_level = Game1.player.HouseUpgradeLevel + 1,
                    lowest_mine_level = MineShaft.lowestLevelReached < 0 ? 0 : MineShaft.lowestLevelReached,
                    stardrop_count = Utility.numStardropsFound(Game1.player),
                    mastery_level = Game1.stats.Get("MasteryExp") != 0 ? currentMasteryLevel : -1,
                    current_mastery_points = currentMasteryPoints,
                    required_mastery_points = requiredMasteryPoints,
                }, translationCategory: TranslationCategory.Menu);
                MainClass.ScreenReader.Say(toSpeak, true);
                return;
            }

            foreach (var area in __instance.skillAreas)
            {
                if (!area.containsPoint(x, y)) continue;
                string skillName = Farmer.getSkillDisplayNameFromIndex(Convert.ToInt32(area.name));

                int skillLevel = area.label switch
                {
                    "0" => Game1.player.FarmingLevel,
                    "2" => Game1.player.ForagingLevel,
                    "1" => Game1.player.FishingLevel,
                    "3" => Game1.player.MiningLevel,
                    "4" => Game1.player.CombatLevel,
                    _ => 0,
                };

                MainClass.ScreenReader.TranslateAndSayWithMenuChecker("menu-skills_page-skill_info", true, translationTokens: new
                {
                    name = skillName,
                    level = skillLevel,
                    buffs = area.hoverText
                });
                return;
            }

            if (string.IsNullOrWhiteSpace(___hoverTitle) && string.IsNullOrWhiteSpace(___hoverText)) return;
            MainClass.ScreenReader.SayWithMenuChecker($"{___hoverTitle},\n{___hoverText}".Trim(), true);
            return;
        }
        catch (Exception e)
        {
            Log.Error($"An error occurred in skills page patch:\n{e.Message}\n{e.StackTrace}");
        }
    }
}
