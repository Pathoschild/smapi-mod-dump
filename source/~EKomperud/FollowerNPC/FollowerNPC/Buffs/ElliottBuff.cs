using System;
using System.Reflection;
using System.Collections.Generic;
using StardewValley;
using StardewValley.Menus;

namespace FollowerNPC.Buffs
{
    class ElliottBuff : CompanionBuff
    {
        protected List<StardewValley.Object> fishCaught;
        protected Random r;
        protected string[] fishCaughtStrings;
        protected Dialogue fishCaughtDialogue;

        public ElliottBuff(Farmer farmer, NPC npc, CompanionsManager manager) : base(farmer, npc, manager)
        {
            buff = new Buff(0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 30, "", "");
            buff.description =
                "Truly a descendant of Thoreau himself, there's nobody quite"+
                Environment.NewLine+
                "like Elliott to sit down and fish with next to a tranquil pond."+
                Environment.NewLine+
                "You gain +3 to your Fishing skill and Elliott will occasionally"+
                Environment.NewLine+
                "share fresh caught fish with you!";

            statBuffs = new Buff[1];
            statBuffs[0] = new Buff(0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 30, "", "");
            statBuffs[0].description = "+3 Fishing" +
                                       Environment.NewLine +
                                       "Source: Elliot";

            fishCaught = new List<StardewValley.Object>();
            ModEntry.modHelper.Events.Display.MenuChanged += Display_MenuChanged;
            r = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed);
        }

        public void SetFishCaughtStrings(string[] s)
        {
            fishCaughtStrings = s;
        }

        public void StartFishing()
        {
            ModEntry.modHelper.Events.GameLoop.TimeChanged += GameLoop_TimeChanged;
        }

        public void StopFishing()
        {
            ModEntry.modHelper.Events.GameLoop.TimeChanged -= GameLoop_TimeChanged;
        }

        private void GameLoop_TimeChanged(object sender, StardewModdingAPI.Events.TimeChangedEventArgs e)
        {
            if (r.Next(3) == 1)
            {
                StardewValley.Object fish = buffOwner.currentLocation.getFish(200, 1, buffOwner.FishingLevel / 2, buffOwner, 4.5d);
                if (fish == null || fish.ParentSheetIndex <= 0)
                    fish = new StardewValley.Object(Game1.random.Next(167, 173), 1, false, -1, 0);
                if (fish.Category == -20 || fish.ParentSheetIndex == 152 || fish.ParentSheetIndex == 153 || 
                    fish.ParentSheetIndex == 157 || fish.ParentSheetIndex == 797 || fish.ParentSheetIndex == 79)
                {
                    fish = new StardewValley.Object(Game1.random.Next(167, 173), 1, false, -1, 0);
                }
                if (fish.Category != -20 && fish.ParentSheetIndex != 152 && fish.ParentSheetIndex != 153 &&
                    fish.ParentSheetIndex != 157 && fish.ParentSheetIndex != 797 && fish.ParentSheetIndex != 79)
                {
                    if (buffOwner.FishingLevel - 3 <= 2)
                        fish.Quality = 1;
                    if (buffOwner.FishingLevel - 3 <= 6)
                        fish.Quality = 2;
                    if (buffOwner.FishingLevel - 3 <= 9)
                        fish.Quality = 3;
                    else
                        fish.Quality = 4;
                    fishCaught.Add(fish);
                    buffGranter.showTextAboveHead("!", -1, 2, 3000, 0);
                    if (fishCaughtDialogue == null)
                    {
                        fishCaughtDialogue = manager.GenerateDialogue("Perk", "Elliott", true);
                        buffGranter.CurrentDialogue.Push(fishCaughtDialogue);
                    }
                }
            }
        }

        private void Display_MenuChanged(object sender, StardewModdingAPI.Events.MenuChangedEventArgs e)
        {
            if (e.OldMenu != null && fishCaughtDialogue != null)
            {
                DialogueBox db = (e.OldMenu as DialogueBox);
                if (db != null)
                {
                    Dialogue d = (Dialogue)typeof(DialogueBox).GetField("characterDialogue", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
                    if (d != null && d.Equals(fishCaughtDialogue))
                    {
                        foreach (StardewValley.Object fish in fishCaught)
                        {
                            buffOwner.addItemToInventory(fish);
                        }
                        fishCaughtDialogue = null;
                        fishCaught.Clear();
                        //buffGranter.faceTowardFarmerTimer = 0;
                        //buffGranter.movementPause = 0;
                    }
                }
            }
        }

        public override void RemoveAndDisposeCompanionBuff()
        {
            base.RemoveAndDisposeCompanionBuff();
            ModEntry.modHelper.Events.Display.MenuChanged -= Display_MenuChanged;
        }
    }
}
