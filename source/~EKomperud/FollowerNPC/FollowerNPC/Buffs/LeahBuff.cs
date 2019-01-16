using System;
using System.Reflection;
using StardewValley;
using StardewValley.Menus;
using FollowerNPC.CompanionStates;

namespace FollowerNPC.Buffs
{
    class LeahBuff : CompanionBuff
    {
        protected StardewValley.Object foragedObject;
        protected Random r;
        protected string[] forageFoundStrings;
        protected Dialogue forageFoundDialogue;
        protected int[] springForage = new int[] {16, 18, 20, 22, 296, 399};
        protected int[] summerForage = new int[] {396, 398, 402};
        protected int[] fallForage = new int[] {404, 406, 408, 410};
        protected int[] winterForage = new int[] {283, 412, 414, 416, 418};
        protected int[] caveForage = new int[] {78, 420, 422};
        protected int[] desertForage = new int[] {88, 90};
        protected int[] beachForage = new int[] {372, 392, 393, 394, 397, 718, 719, 723};
        protected int[] woodsSpringForage = new int[] {257, 404};
        protected int[] woodsSummerForage = new int[] {259, 420};
        protected int[] woodsFallForage = new int[] {281, 420};

        protected System.Collections.Generic.Stack<Dialogue> combatWithheldDialogue;

        public LeahBuff(Farmer farmer, NPC npc, CompanionsManager manager) : base(farmer, npc, manager)
        {
            buff = new Buff(0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 30, "", "");
            buff.description = "Leah always seems to smell like chopped wood and fall mushrooms."+
                               Environment.NewLine+
                               "You gain +2 Foraging while hanging out with her."+
                               Environment.NewLine+
                               "She's also keen to share quality forage every once in a while!";

            statBuffs = new Buff[1];
            statBuffs[0] = new Buff(0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 30, "", "");
            statBuffs[0].description = "+2 Foraging" +
                                       Environment.NewLine +
                                       "Source: Leah";

            ModEntry.modHelper.Events.GameLoop.TimeChanged += GameLoop_TimeChanged;
            ModEntry.modHelper.Events.Display.MenuChanged += Display_MenuChanged;
            r = new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed);
            combatWithheldDialogue = ((RecruitedState)manager.possibleCompanions["Leah"].currentState).combatWithheldDialogue;
        }

        public void SetForageFoundDialogue(string[] s)
        {
            forageFoundStrings = s;
        }

        private void GameLoop_TimeChanged(object sender, StardewModdingAPI.Events.TimeChangedEventArgs e)
        {
            if (r.Next(6) == 1 && foragedObject == null)
            {
                GameLocation location = buffOwner.currentLocation;
                StardewValley.Locations.MineShaft ms = location as StardewValley.Locations.MineShaft;
                if (location.IsOutdoors)
                {
                    string locationName = buffOwner.currentLocation.Name;
                    if (locationName.Equals("Woods"))
                    {
                        string season = Game1.currentSeason;
                        switch (season)
                        {
                            case "spring":
                                foragedObject = new StardewValley.Object(woodsSpringForage[r.Next(2)], 1, false, -1, 4); break;
                            case "summer":
                                foragedObject = new StardewValley.Object(woodsSummerForage[r.Next(2)], 1, false, -1, 4); break;
                            case "fall":
                                foragedObject = new StardewValley.Object(woodsFallForage[r.Next(2)], 1, false, -1, 4); break;
                            default:
                                foragedObject = new StardewValley.Object(winterForage[r.Next(5)], 1, false, -1, 4); break;
                        }
                    }
                    else if (locationName.Equals("Beach"))
                    {
                        foragedObject = new StardewValley.Object(beachForage[r.Next(8)], 1, false, -1, 4);
                    }
                    else if (locationName.Equals("Desert"))
                    {
                        foragedObject = new StardewValley.Object(desertForage[r.Next(2)], 1, false, -1, 4);
                    }
                    else
                    {
                        string season = Game1.currentSeason;
                        switch (season)
                        {
                            case "spring":
                                foragedObject = new StardewValley.Object(springForage[r.Next(6)], 1, false, -1, 4); break;
                            case "summer":
                                foragedObject = new StardewValley.Object(summerForage[r.Next(3)], 1, false, -1, 4); break;
                            case "fall":
                                foragedObject = new StardewValley.Object(fallForage[r.Next(4)], 1, false, -1, 4); break;
                            case "winter":
                                foragedObject = new StardewValley.Object(winterForage[r.Next(5)], 1, false, -1, 4); break;
                        }
                    }

                    buffGranter.showTextAboveHead("!", -1, 2, 3000, 0);
                    string dialogueString = r.Next(2) == 0
                        ? forageFoundStrings[0] + foragedObject.Name + "!"
                        : forageFoundStrings[1] + foragedObject.Name + forageFoundStrings[2];
                    Dialogue d = new Dialogue(dialogueString, buffGranter);
                    buffGranter.CurrentDialogue.Push(d);
                    forageFoundDialogue = d;
                }
                else if (ms != null && combatWithheldDialogue.Count == 0)
                {
                    foragedObject = new StardewValley.Object(caveForage[r.Next(3)], 1, false, -1, 4);

                    buffGranter.showTextAboveHead("!", -1, 2, 3000, 0);
                    string dialogueString = r.Next(2) == 0
                        ? forageFoundStrings[0] + foragedObject.Name + "!"
                        : forageFoundStrings[1] + foragedObject.Name + forageFoundStrings[2];
                    Dialogue d = new Dialogue(dialogueString, buffGranter);
                    buffGranter.CurrentDialogue.Push(d);
                    forageFoundDialogue = d;
                }
            }

            if (foragedObject != null &&
                !buffGranter.CurrentDialogue.Contains(forageFoundDialogue) &&
                combatWithheldDialogue.Count == 0)
            {
                buffGranter.CurrentDialogue.Push(forageFoundDialogue);
            }
        }

        private void Display_MenuChanged(object sender, StardewModdingAPI.Events.MenuChangedEventArgs e)
        {
            if (e.OldMenu != null && forageFoundDialogue != null)
            {
                DialogueBox db = (e.OldMenu as DialogueBox);
                if (db != null)
                {
                    Dialogue d = (Dialogue)typeof(DialogueBox).GetField("characterDialogue", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
                    if (d != null && d.Equals(forageFoundDialogue))
                    {
                        buffOwner.addItemToInventory(foragedObject);
                        forageFoundDialogue = null;
                        foragedObject = null;
                        buffGranter.faceTowardFarmerTimer = 0;
                        buffGranter.movementPause = 0;
                    }
                }
            }
        }

        public override void RemoveAndDisposeCompanionBuff()
        {
            base.RemoveAndDisposeCompanionBuff();
            ModEntry.modHelper.Events.GameLoop.TimeChanged -= GameLoop_TimeChanged;
            ModEntry.modHelper.Events.Display.MenuChanged -= Display_MenuChanged;
        }
    }
}
