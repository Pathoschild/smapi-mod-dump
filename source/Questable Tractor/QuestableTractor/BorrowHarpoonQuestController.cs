/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Diagnostics;
using HarmonyLib;
using StardewValley;
using StardewValley.GameData.Tools;
using StardewValley.Tools;

using static NermNermNerm.Stardew.LocalizeFromSource.SdvLocalize;

namespace NermNermNerm.Stardew.QuestableTractor
{
    public class BorrowHarpoonQuestController
        : BaseQuestController<BorrowHarpoonQuestState>
    {
        public const string HarpoonToolId = "NermNermNerm.QuestableTractor.Harpoon";
        public const string HarpoonToolQiid = ItemRegistry.type_tool + HarpoonToolId;

        private static float chanceOfHookingWaterer = 0;
        private static bool hasPatchBeenInstalled = false;
        private static BorrowHarpoonQuestController instance = null!;
        private const string TrashItemQualifiedId = "(O)168";

        public BorrowHarpoonQuestController(ModEntry entry) : base(entry) { }

        protected override string ModDataKey => ModDataKeys.BorrowHarpoonQuestStatus;

        protected override BorrowHarpoonQuestState AdvanceStateForDayPassing(BorrowHarpoonQuestState oldState) => oldState;

        protected override BaseQuest CreateQuest() => new BorrowHarpoonQuest(this);

        public void StartQuest(Farmer player)
        {
            if (!this.IsStarted)
            {
                Game1.addHUDMessage(new HUDMessage(L("Whoah, I snagged onto something big down there, but this line's nowhere near strong enough to yank it up!"), HUDMessage.newQuest_type));
                this.CreateQuestNew(player);
                Game1.playSound("questcomplete"); // Note documentation suggests its for quest complete and "journal update".  That's what we are using it for.
            }
        }

        public override void Fix()
        {
            this.EnsureInventory(HarpoonToolQiid, this.OverallQuestState == OverallQuestState.InProgress && (this.State == BorrowHarpoonQuestState.CatchTheBigOne || this.State == BorrowHarpoonQuestState.ReturnThePole));
        }

        protected override void OnStateChanged()
        {
            base.OnStateChanged();

            if (Game1.IsMasterGame && this.Mod.WatererQuestController.OverallQuestState == OverallQuestState.NotStarted)
            {
                chanceOfHookingWaterer = this.Mod.RestoreTractorQuestController.IsComplete
                    ? 0.01f + Game1.Date.TotalDays / 200f
                    : .01f;
            }
            else
            {
                chanceOfHookingWaterer = 0;
            }

            this.LogTrace($"Chance of hooking the starter for Waterer: {chanceOfHookingWaterer:P0}");
            if (!hasPatchBeenInstalled && chanceOfHookingWaterer > 0)
            {
                this.LogTrace($"Applying harmony patch to Farm.getFish");
                var getFishMethod = typeof(Farm).GetMethod("getFish");
                BorrowHarpoonQuestController.instance = this; // Harmony doesn't support creating prefixes with instance methods...  Faking it.
                this.Mod.Harmony.Patch(getFishMethod, prefix: new HarmonyMethod(typeof(BorrowHarpoonQuestController), nameof(Prefix_GetFish)));
                hasPatchBeenInstalled = true;
            }
            // Undoing a Harmony patch is sketchy, so we're going to leave it installed even if the quest is over.
        }

        private static bool Prefix_GetFish(ref Item __result)
        {
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

        private bool IsActuallyFishing()
        {
            // When would you not be fishing?  In the stock game, the only thing that calls the method
            // we're Harmony-Patching is the inimically-named FishingRod.DoFunction, which does a ton
            // of stuff before calling GameLocation.getFish to see what got snagged.
            // However, VisibleFish also calls GameLocation.getFish to figure out what fish to draw,
            // and it calls it a whole bunch.  So we need to figure out if we're being called for
            // the fishing rod or something else.  The main technique we're going to use is to see if,
            // at a reasonable distance up the stack, we see that we're being called from the FishingRod
            // class.  It's not super-precise, and that's to defend against changes to the game code.
            //
            // System.Diagnostics.StackTrace is not built for speed, so performance is a concern, however
            // while it's not quick, it's not super-slow either.  So we mitigate it by the heuristic
            // of first just checking to see if the fishing rod is the active item (because if it's not,
            // we're obviously not fishing), then falling back to the stack walk if needed.  We also
            // track the performance of the method and issue warnings to make it easy for players to
            // discover if we're the source of lag.

            // High-performance test to see if we're fishing or not
            if (Game1.player.ActiveItem is not FishingRod)
            {
                return false;
            }

            var watch = Stopwatch.StartNew();

            // Note that we could use the variant of the constructor that allows you to skip the top few frames
            // (which will obviously be our code), but I don't feel like the perf win is enough to justify the risk
            // of something like compiler inlining or minor refactors causing mayhem.
            var stack = new StackTrace();
            for (int i = 0; i < 10; ++i) // <- 10 is just a spitball in a possibly-jinxed attempt to minimize false-positives.
            {
                var frame = stack.GetFrame(i);
                if (frame is not null)
                {
                    var method = frame.GetMethod();
                    if (method is not null)
                    {
                        if (method.DeclaringType == typeof(FishingRod) || method.Name.Contains("DoFunction_PatchedBy"))
                        {
                            return true;
                        }
                    }
                }
            }

            long ms = watch.ElapsedMilliseconds;
            if (ms > 10)
            {
                this.LogWarning($"IsActuallyFishing took {watch.Elapsed.TotalMilliseconds}.  If you're experiencing unpleasant lag while fishing on the farm, you can temporarily disable the VisibleFish mod until you complete the 'We need a bigger pole' quest to get past this problem.");
            }

            return false;
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
            // Ensure that the call is for fishing on the farm by the main player.
            if (Game1.currentLocation != Game1.getFarm() || !Game1.IsMasterGame || !this.IsActuallyFishing())
            {
                return null;
            }

            this.LogTrace($"Intercepted a call to Farm.GetFish");

            var borrowHarpoonQuest = FakeQuest.GetFakeQuestByType<BorrowHarpoonQuest>(Game1.player);
            if (Game1.player.CurrentTool?.QualifiedItemId == HarpoonToolQiid)
            {
                if (borrowHarpoonQuest is null)
                {
                    // Shouldn't be possible, but if it happens...
                    this.LogError($"BorrowHarpoon quest was not open when player used harpoon");
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
                        L("Aaahhh! ! I had it!"),
                        L("Nope...  nothing"),
                        L("Ooohhh!  So close!")
                    }[Game1.random.Next(3)];
                    Game1.addHUDMessage(new HUDMessage(message) { noIcon = true });

                    return ItemRegistry.Create(TrashItemQualifiedId);
                }
            }
            else // we're not fishing with the harpoon
            {
                if (borrowHarpoonQuest is null && Game1.random.NextDouble() < chanceOfHookingWaterer)
                {
                    this.Mod.BorrowHarpoonQuestController.StartQuest(Game1.player);
                    return ItemRegistry.Create(TrashItemQualifiedId);
                }
                else if (borrowHarpoonQuest is not null && borrowHarpoonQuest.State == BorrowHarpoonQuestState.CatchTheBigOne && Game1.random.NextDouble() < .25) // prolly indicates the user goofed and is still using the regular rod
                {
                    Game1.addHUDMessage(new HUDMessage(L("Dang! Snapped a line on that waterer again!  Perhaps switching rods to Willy's Harpoon would help.")) { noIcon = true });
                    return ItemRegistry.Create(TrashItemQualifiedId);
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
                Name = I("Harpoon"),
                AttachmentSlots = 0,
                SalePrice = 0,
                DisplayName = L("Great Grandpappy's Harpoon"),
                Description = L("Willy's Great Grandpappy used this to hunt whales back in the day."),
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
