/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using StardewValley;
using StardewValley.GameData.Tools;

namespace NermNermNerm.Stardew.QuestableTractor
{
    public class BorrowHarpoonQuestController
        : BaseQuestController<BorrowHarpoonQuestState>
    {
        public const string HarpoonToolId = "NermNermNerm.QuestableTractor.Harpoon";

        private static float chanceOfHookingWaterer = 0;
        private static bool hasPatchBeenInstalled = false;
        private static BorrowHarpoonQuestController instance = null!;
        private const string TrashItemId = "(O)168";

        public BorrowHarpoonQuestController(ModEntry entry) : base(entry) { }

        protected override string ModDataKey => ModDataKeys.BorrowHarpoonQuestStatus;

        protected override BorrowHarpoonQuestState AdvanceStateForDayPassing(BorrowHarpoonQuestState oldState) => oldState;

        protected override BaseQuest CreateQuest() => new BorrowHarpoonQuest(this);

        public void StartQuest()
        {
            if (!this.IsStarted)
            {
                Game1.addHUDMessage(new HUDMessage("Whoah, I snagged onto something big down there, but this line's nowhere near strong enough to yank it up!", HUDMessage.newQuest_type));
                this.CreateQuestNew();
                Game1.playSound("questcomplete"); // Note documentation suggests its for quest complete and "journal update".  That's what we are using it for.
            }
        }

        protected override void OnStateChanged()
        {
            base.OnStateChanged();

            if (this.OverallQuestState == OverallQuestState.NotStarted)
            {
                chanceOfHookingWaterer = this.Mod.RestoreTractorQuestController.IsComplete
                    ? 0.01f + Game1.Date.TotalDays / 200f
                    : .01f;
            }
            else
            {
                chanceOfHookingWaterer = 0;
            }

            this.LogTrace(FormattableString.Invariant($"Chance of hooking the starter for Waterer: {chanceOfHookingWaterer:P0}"));
            if (!hasPatchBeenInstalled && chanceOfHookingWaterer > 0)
            {
                this.LogTrace("Applying harmony patch to Farm.getFish");
                var farmType = typeof(Farm);
                var getFishMethod = farmType.GetMethod("getFish");
                BorrowHarpoonQuestController.instance = this; // Harmony doesn't support creating prefixes with instance methods...  Faking it.
                this.Mod.Harmony.Patch(getFishMethod, prefix: new HarmonyMethod(typeof(BorrowHarpoonQuestController), nameof(Prefix_GetFish)));
                hasPatchBeenInstalled = true;
            }
            // Undoing a Harmony patch is sketchy, so we're going to leave it installed even if the quest is over.
        }

        private static bool Prefix_GetFish(ref Item __result)
        {
            bool isFishingWithHarpoon = Game1.player.CurrentTool?.ItemId == HarpoonToolId;
            var newFish = instance.ReplaceFish();
            if (newFish is null)
            {
                return true; // Go ahead and call the normal function.
            }
            else
            {
                __result = newFish;
                return false; // Skip calling the function and use this result.
            }
        }

        /// <summary>
        ///   If this returns null, the behavior of fishing should be left alone.  If it returns something,
        ///   then the something should be what the player gets from the cast.
        /// </summary>
        /// <remarks>
        ///   This method is sorta bad-design as it does two things - one is that it handles starting the
        ///   harpoon quest, and the other is starting the waterer quest.
        /// </remarks>
        private Item? ReplaceFish()
        {
            // attaching to Farm.getFish should ensure we only get called from this location, but just to be sure nothing
            // else uses the same type...
            if (Game1.currentLocation != Game1.getFarm())
            {
                return null;
            }

            var borrowHarpoonQuest = Game1.player.questLog.OfType<BorrowHarpoonQuest>().FirstOrDefault();
            if (Game1.player.CurrentTool?.ItemId == HarpoonToolId)
            {
                if (borrowHarpoonQuest is null)
                {
                    // Shouldn't be possible, but if it happens...
                    this.LogError("BorrowHarpoon quest was not open when player used harpoon");
                    return null;
                }

                if (Game1.random.NextDouble() < .3)
                {
                    Game1.playSound("submarine_landing");
                    borrowHarpoonQuest.LandedTheWaterer();
                    var waterer =  ItemRegistry.Create<StardewValley.Object>(ObjectIds.BustedWaterer);
                    waterer.questItem.Value = true;
                    return waterer;
                }
                else
                {
                    Game1.playSound("clank");
                    string message = new string[]
                    {
                        "Aaahhh! ! I had it!",
                        "Nope...  nothing",
                        "Ooohhh!  So close!"
                    }[Game1.random.Next(3)];
                    Game1.addHUDMessage(new HUDMessage(message) { noIcon = true });

                    return ItemRegistry.Create(TrashItemId);
                }
            }
            else // we're not fishing with the harpoon
            {
                if (borrowHarpoonQuest is null && Game1.random.NextDouble() < chanceOfHookingWaterer)
                {
                    this.Mod.BorrowHarpoonQuestController.StartQuest();
                    return ItemRegistry.Create(TrashItemId);
                }
                else if (borrowHarpoonQuest is not null && borrowHarpoonQuest.State == BorrowHarpoonQuestState.CatchTheBigOne && Game1.random.NextDouble() < .25) // prolly indicates the user goofed and is still using the regular rod
                {
                    Game1.addHUDMessage(new HUDMessage("Dang! Snapped a line on that waterer again!  Perhaps switching rods to Willy's Harpoon would help.") { noIcon = true });
                    return ItemRegistry.Create(TrashItemId);
                }
                else
                {
                    chanceOfHookingWaterer *= 1.15F; // Reward determination.  1.15^10 (10 casts) is 4x the original chance.
                    return null;
                }
            }
        }


        internal static void EditToolAssets(IDictionary<string, ToolData> data)
        {
            data[HarpoonToolId] = new ToolData
            {
                ClassName = "FishingRod",
                Name = "Harpoon",
                AttachmentSlots = 0,
                SalePrice = 0,
                DisplayName = "Great Grandpappy's Harpoon",
                Description = "Willy's Great Grandpappy used this to hunt whales back in the day.",
                Texture = ModEntry.SpritesPseudoPath,
                SpriteIndex = 19,
                MenuSpriteIndex = -1,
                UpgradeLevel = 0,
                ApplyUpgradeLevelToDisplayName = false,
                ConventionalUpgradeFrom = null,
                UpgradeFrom = null,
                CanBeLostOnDeath = false,
                SetProperties = null,
            };
        }
    }
}
