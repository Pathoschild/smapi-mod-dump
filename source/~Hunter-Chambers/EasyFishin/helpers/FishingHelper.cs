/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hunter-Chambers/StardewValleyMods
**
*************************************************/

using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

namespace EasyFishin.helpers
{
    internal class FishingHelper
    {
        private static readonly string[] BAIT_IDS = { "685", "703", "774", "908", "DeluxeBait", "SpecificBait", "ChallengeBait" };
        private static readonly string[] TACKLE_IDS = { "686", "687", "694", "695", "692", "693", "691", "856", "877", "SonarBobber" };

        private static readonly PerScreen<BobberBar?> BobberBarMenus = new();

        private static bool CanHook(FishingRod fishingRod)
        {
            return fishingRod.isNibbling &&
                   !fishingRod.hit &&
                   !fishingRod.isReeling &&
                   !fishingRod.pullingOutOfWater &&
                   !fishingRod.fishCaught &&
                   !fishingRod.showingTreasure;
        }

        public static void AutoHook(FishingRod fishingRod, bool disableVibration)
        {
            if (CanHook(fishingRod))
            {
                fishingRod.timePerBobberBob = 1f;
                fishingRod.timeUntilFishingNibbleDone = FishingRod.maxTimeToNibble;
                fishingRod.DoFunction(Game1.player.currentLocation, (int)fishingRod.bobber.X, (int)fishingRod.bobber.Y, 1, Game1.player);

                if (!disableVibration)
                {
                    Rumble.rumble(0.95f, 200f);
                }
            }
        }

        public static void ApplyInfiniteBaitAndTackle(EasyFishinConfig config)
        {
            if (Game1.player.CurrentTool is FishingRod fishingRod)
            {
                foreach (StardewValley.Object attachment in fishingRod.attachments)
                {
                    if (attachment is not null)
                    {
                        if (config.InfiniteTackle && TACKLE_IDS.Contains(attachment.ItemId))
                        {
                            attachment.uses.Value = 0;
                        }
                        else if (config.InfiniteBait && BAIT_IDS.Contains(attachment.ItemId))
                        {
                            // minimum stack size of 2 to avoid 'out of bait' message, also makes it harder to make infinite money :P
                            attachment.Stack = (attachment.Stack < 2) ? 2 : attachment.Stack; 
                        }
                    }
                }
            }
        }

        public static void SetBobberToPerfect(int screenID)
        {
            BobberBar? bobberBar = BobberBarMenus.GetValueForScreen(screenID);
            if (bobberBar is not null)
            {
                bobberBar.perfect = true;
                bobberBar.fishQuality = 4;
            }
        }

        public static void ApplyFishingMiniGameAdjustments(EasyFishinConfig config, int screenID, BobberBar bobberBar)
        {
            BobberBarMenus.SetValueForScreen(screenID, bobberBar);

            if (config.AlwaysFindTreasure)
            {
                bobberBar.treasure = true;
            }

            if (config.InstantCatchFish)
            {
                bobberBar.distanceFromCatching = 100f;

                if (bobberBar.treasure)
                {
                    bobberBar.treasureCaught = true;
                }
            }
            else if (config.InstantCatchTreasure && bobberBar.treasure)
            {
                bobberBar.treasureCaught = true;
            }
        }

        public static void ClearBobberBar(int screenID)
        {
            BobberBarMenus.SetValueForScreen(screenID, null);
        }
    }
}
