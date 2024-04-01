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
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches;

internal class MasteryTrackerMenuPatch : IPatch
{
    public void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(MasteryTrackerMenu), "draw"),
            postfix: new HarmonyMethod(typeof(MasteryTrackerMenuPatch), nameof(MasteryTrackerMenuPatch.DrawPatch))
        );
    }

    private static void DrawPatch(MasteryTrackerMenu __instance, int ___which, bool ___canClaim, List<ClickableTextureComponent> ___rewards)
    {
        try
        {
            if (MainClass.Config.PrimaryInfoKey.JustPressed())
            {
                MainClass.ScreenReader.PrevMenuQueryText = "";
            }

            if (___which == -1)
            {
                int currentMasteryLevel = MasteryTrackerMenu.getCurrentMasteryLevel();
                int currentPoints = (int)Game1.stats.Get("MasteryExp") - MasteryTrackerMenu.getMasteryExpNeededForLevel(currentMasteryLevel);
                int requiredPoints = MasteryTrackerMenu.getMasteryExpNeededForLevel(currentMasteryLevel + 1) - MasteryTrackerMenu.getMasteryExpNeededForLevel(currentMasteryLevel);

                MainClass.ScreenReader.TranslateAndSayWithMenuChecker("menu-mastery-pedestial_info", true, translationTokens: new
                {
                    final_path_text = Game1.content.LoadString("Strings\\1_6_Strings:FinalPath"),
                    current_points = currentPoints,
                    required_points = requiredPoints,
                    stars = currentMasteryLevel
                });
                return;
            }

            int x = Game1.getMouseX(true), y = Game1.getMouseY(true);

            string rewards = string.Join(", ", ___rewards.Select(reward => string.IsNullOrWhiteSpace(reward.name) ? reward.label : $"{reward.name}, {reward.label}"));
            bool wasSpoken = MainClass.ScreenReader.TranslateAndSayWithMenuChecker("menu-mastery-walls-claim_button", true, translationTokens: new
            {
                name = Game1.content.LoadString("Strings\\1_6_Strings:" + ___which + "_Mastery"),
                rewards = rewards
            });

            if (wasSpoken && !___canClaim)
            {
                Game1.playSound("invalid-selection");
            }
        }
        catch (Exception e)
        {
            Log.Error($"An error occurred in mastery tracker menu patch:\n{e.Message}\n{e.StackTrace}");
        }
    }
}
