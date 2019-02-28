using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using Modworks = bwdyworks.Modworks;
using System;

namespace PersonalEffects
{
    public class Mod : StardewModdingAPI.Mod
    {
        internal static bool Debug = false;
        [System.Diagnostics.Conditional("DEBUG")]
        public void EntryDebug() { Debug = true; }
        internal static string Module;

        public static int tickUpdateLimiter = 0;
        public static bool EatingPrimed = false;
        public static StardewValley.Item EatingItem;
        public static int eatingQuantity = 0;

        public override void Entry(IModHelper helper)
        {
            Module = helper.ModRegistry.ModID;
            EntryDebug();
            if (!Modworks.InstallModule(Module, Debug)) return;

            Config.Load(Helper.DirectoryPath + System.IO.Path.DirectorySeparatorChar);
            if (Config.ready)
            {
                AddItems();
                Spot.Setup(helper);
            }
            if (Debug) helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
        }

        private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            tickUpdateLimiter++;
            if (tickUpdateLimiter < 10) return;
            tickUpdateLimiter = 0;
            if (EatingPrimed)
            {
                if (!Game1.player.isEating)
                {
                    
                    EatingPrimed = false;
                    //make sure we didn't say no
                    if(Game1.player.ActiveObject == null || Game1.player.ActiveObject.Stack < eatingQuantity) OnItemEaten(Game1.player, EatingItem);
                    EatingItem = null;
                }
            } else if (Game1.player.isEating)
            {
                EatingPrimed = true;
                EatingItem = Game1.player.itemToEat;
                eatingQuantity = Game1.player.ActiveObject.Stack;
            }
        }

        public static void OnItemEaten(StardewValley.Farmer who, StardewValley.Item what)
        {
            string dn = what.DisplayName;
            string npc;
            if (dn.EndsWith("'s Panties")) npc = dn.Substring(0, dn.IndexOf("'s Panties"));
            else if (dn.EndsWith("'s Underwear")) npc = dn.Substring(0, dn.IndexOf("'s Underwear"));
            else return; //not one of ours
            if (string.IsNullOrWhiteSpace(npc)) return;
            string npcName = Config.LookupNPC(npc);
            if (npcName == null) return;
            var npcConfig = Config.GetNPC(npcName);
            Game1.showGlobalMessage("You feel like this changes your relationship with " + npcConfig.Name + ".");
            int friendship = Modworks.Player.GetFriendshipPoints(npcName);
            int chance = 500 + (int)((StardewValley.Game1.dailyLuck * 10f) * 500f); //0-1000
            Random rng = new Random(DateTime.Now.Millisecond);
            int strikepoint = rng.Next(1250);
            if (Debug) Modworks.Log.Trace(chance + " > " + strikepoint);
            if (chance > strikepoint)
            {
                Modworks.Player.SetFriendshipPoints(npcName, Math.Min(2500, friendship + 125));
                if(Debug) Modworks.Log.Trace("Relationship increased by 125");
            }
            else
            {
                Modworks.Player.SetFriendshipPoints(npcName, Math.Max(friendship - 50, 0));
                if (Debug) Modworks.Log.Trace("Relationship decreased by 50");
            }
        }

        public void AddItems()
        {
            AddPersonalEffectItem("Haley", 1, "These look expensive.", 41);
            AddPersonalEffectItem("Haley", 2, "These have clearly been worn.", 29);
            AddPersonalEffectItem("Abigail", 1, "How exciting!", 29);
            AddPersonalEffectItem("Abigail", 2, "They smell just like " + Config.GetNPC("Abigail").GetPronoun(1) + ".", 33);
            AddPersonalEffectItem("Emily", 1, "I thought they'd be brighter.", 27);
            AddPersonalEffectItem("Emily", 2, "This is a nice material.", 35);
            AddPersonalEffectItem("Penny", 1, "How charming.", 29);
            AddPersonalEffectItem("Penny", 2, "They smell kind of sweet.", 32);
            AddPersonalEffectItem("Leah", 1, "I don't think " + Config.GetNPC("Leah").GetPronoun(0) + "'d mind.", 31);
            AddPersonalEffectItem("Leah", 2, "What a lucky find!", 34);
            AddPersonalEffectItem("Jodi", 1, "Huh.", 22);
            AddPersonalEffectItem("Jodi", 2, "A rare sight.", 22);
            AddPersonalEffectItem("Caroline", 1, "About what I'd expect.", 24);
            AddPersonalEffectItem("Caroline", 2, "Well how about that.", 32);
            AddPersonalEffectItem("Maru", 1, "A little less shy on the inside.", 32);
            AddPersonalEffectItem("Maru", 2, "Thoroughly lived in.", 33);
            AddPersonalEffectItem("Robin", 1, "A lucky find?", 33);
            AddPersonalEffectItem("Robin", 2, "They smell faintly of sawdust.", 36);
            AddPersonalEffectItem("Alex", 1, "They're all sweaty.", 19);
            AddPersonalEffectItem("Alex", 2, "Oh, these are nice.", 27);
            AddPersonalEffectItem("Elliott", 1, "Nothing but class.", 29);
            AddPersonalEffectItem("Elliott", 2, "Such graceful elegance.", 31);
            AddPersonalEffectItem("Harvey", 1, "They seem reliable.", 24);
            AddPersonalEffectItem("Harvey", 2, Config.GetNPC("Harvey").GetPronoun(0) + " kind of suprises me.", 28);
            AddPersonalEffectItem("Sam", 1, "So adventurous!", 33);
            AddPersonalEffectItem("Sam", 2, "They smell like " + Config.GetNPC("Sam").GetPronoun(1) + ".", 16);
            AddPersonalEffectItem("Sebastian", 1, UppercaseFirst(Config.GetNPC("Sebastian").GetPronoun(0)) + " never disappoints.", 19);
            AddPersonalEffectItem("Sebastian", 2, "They're kind of... damp inside.", 40);
            AddPersonalEffectItem("Shane", 1, "Whoa, they have a strong smell.", 14);
            AddPersonalEffectItem("Shane", 2, "They're nicer than you'd expect.", 21);
            AddPersonalEffectItem("Clint", 1, "To think of the tools " + Config.GetNPC("Clint").GetPronoun(0) + " uses.", 26);
            AddPersonalEffectItem("Clint", 2, "They have a strange, smoky quality.", 40);
            AddPersonalEffectItem("Demetrius", 1, "I probably shouldn't have these.", 28);
            AddPersonalEffectItem("Demetrius", 2, "They don't smell worn.", 24);
            AddPersonalEffectItem("Gus", 1, "These definitely belong to " + Config.GetNPC("Gus").Name + ".", 33);
            AddPersonalEffectItem("Gus", 2, "They're still kind of warm.", 36);
            AddPersonalEffectItem("Kent", 1, "A bit less brave on the inside.", 17);
            AddPersonalEffectItem("Kent", 2, "What parts of the world these have seen?", 22);
            AddPersonalEffectItem("Lewis", 1, "Another fine display for the fair.", 33);
            AddPersonalEffectItem("Lewis", 2, "Perfectly in character.", 19);
            AddPersonalEffectItem("Marnie", 1, "Soooo comfortable!", 25);
            AddPersonalEffectItem("Marnie", 2, "Smells like hay.", 16);
            AddPersonalEffectItem("Pam", 1, "They're... broken in.", 14);
            AddPersonalEffectItem("Pam", 2, "These smell like something, but what?", 19);
            AddPersonalEffectItem("Pierre", 1, "Clearly doing alright for " + Config.GetNPC("Pierre").GetPronoun(1) + "self.", 33);
            AddPersonalEffectItem("Pierre", 2, Config.GetNPC("Pierre").GetPronoun(0) + " should be more careful.", 36);
            AddPersonalEffectItem("Sandy", 1, "Absolutely fabulous.", 40);
            AddPersonalEffectItem("Sandy", 2, "Worth the trip.", 41);
            AddPersonalEffectItem("Willy", 1, "Salty.", 17);
            AddPersonalEffectItem("Willy", 2, "Smells vaguely like fish.", 21);
            AddPersonalEffectItem("Wizard", 1, "Almost ethereal - a fabric like you've never seen.", 49);
            AddPersonalEffectItem("Wizard", 2, "They are absolutely enchanting...", 41);
            AddPersonalEffectItem("Vincent", 1, "At least they're clean.", 24);
            AddPersonalEffectItem("Vincent", 2, "Ah, an air of adventure surrounds these.", 26);
            AddPersonalEffectItem("Jas", 1, "They give you the chills.", 18);
            AddPersonalEffectItem("Jas", 2, UppercaseFirst(Config.GetNPC("Jas").GetPronoun(0)) + " probably doesn't need these.", 16);
            AddPersonalEffectItem("Linus", 1, "Oh my. Oh.", 14);
            AddPersonalEffectItem("Linus", 2, "Hopefully " + Config.GetNPC("Linus").GetPronoun(0) + " got new ones.", 15);
            AddPersonalEffectItem("Evelyn", 1, "Aged and experienced.", 20);
            AddPersonalEffectItem("Evelyn", 2, ".....", 17);
            AddPersonalEffectItem("George", 1, "Very dependable.", 13);
            AddPersonalEffectItem("George", 2, "It's dangerous to go alone! Take this.", 15);
        }

        public void AddPersonalEffectItem(string npc, int variant, string desc, int price)
        {
            Modworks.Items.AddItem(Module, new bwdyworks.BasicItemEntry(this, "px" + Config.GetNPC(npc).Abbreviate() + (Config.GetNPC(npc).HasMaleItems() ? "m" : "f") + variant, price, 10, "Personal Effects", StardewValley.Object.sellAtFishShopCategory, Config.GetNPC(npc).Name + "'s " + (Config.GetNPC(npc).HasMaleItems() ? "Underwear" : "Panties"), desc));
        }

        static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == SButton.OemTilde)
            {

                Monitor.Log("friendship power!");
                Modworks.Player.SetFriendshipPoints("Alex", 2500);
                Modworks.Player.SetFriendshipPoints("Elliott", 2500);
                Modworks.Player.SetFriendshipPoints("Harvey", 2500);
                Modworks.Player.SetFriendshipPoints("Sam", 2500);
                Modworks.Player.SetFriendshipPoints("Sebastian", 2500);
                Modworks.Player.SetFriendshipPoints("Shane", 2500);
                Modworks.Player.SetFriendshipPoints("Abigail", 2500);
                Modworks.Player.SetFriendshipPoints("Emily", 2500);
                Modworks.Player.SetFriendshipPoints("Haley", 2500);
                Modworks.Player.SetFriendshipPoints("Leah", 2500);
                Modworks.Player.SetFriendshipPoints("Maru", 2500);
                Modworks.Player.SetFriendshipPoints("Penny", 2500);
                Modworks.Player.SetFriendshipPoints("Caroline", 2500);
                Modworks.Player.SetFriendshipPoints("Clint", 2500);
                Modworks.Player.SetFriendshipPoints("Demetrius", 2500);
                Modworks.Player.SetFriendshipPoints("Gus", 2500);
                Modworks.Player.SetFriendshipPoints("Jodi", 2500);
                Modworks.Player.SetFriendshipPoints("Kent", 2500);
                Modworks.Player.SetFriendshipPoints("Lewis", 2500);
                Modworks.Player.SetFriendshipPoints("Marnie", 2500);
                Modworks.Player.SetFriendshipPoints("Pam", 2500);
                Modworks.Player.SetFriendshipPoints("Pierre", 2500);
                Modworks.Player.SetFriendshipPoints("Robin", 2500);
                Modworks.Player.SetFriendshipPoints("Sandy", 2500);
                Modworks.Player.SetFriendshipPoints("Willy", 2500);
                Modworks.Player.SetFriendshipPoints("Wizard", 2500);
                Modworks.Player.SetFriendshipPoints("Vincent", 2500);
                Modworks.Player.SetFriendshipPoints("Jas", 2500);
                Modworks.Player.SetFriendshipPoints("Linus", 2500);
                Modworks.Player.SetFriendshipPoints("Evelyn", 2500);
                Modworks.Player.SetFriendshipPoints("George", 2500);

                var pos = Modworks.Player.GetStandingTileCoordinate();
                this.Monitor.Log($"Loc: {Game1.currentLocation.Name} @ {pos[0]} x {pos[1]}", LogLevel.Alert);
            }
        }
    }
}