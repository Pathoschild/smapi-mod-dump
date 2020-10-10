/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bwdymods/SDV-PersonalEffects
**
*************************************************/

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
                Modworks.Events.ItemEaten += Events_ItemEaten;
                helper.Events.Input.ButtonPressed += Input_ButtonPressed;
                AddTrashLoot("Sam", 0);
                AddTrashLoot("Jodi", 0);
                AddTrashLoot("Haley", 1);
                AddTrashLoot("Emily", 1);
                AddTrashLoot("Lewis", 2);
                AddTrashLoot("Clint", 4);
                //saloon is one catch-all
                AddTrashLoot("Gus", 5);
                AddTrashLoot("Pam", 5);
                AddTrashLoot("Emily", 5);
                AddTrashLoot("Abigail", 5);
                AddTrashLoot("Sebastian", 5);
                AddTrashLoot("Marnie", 5);
                //archeology is the other
                AddTrashLoot("Maru", 3);
                AddTrashLoot("Elliott", 3);
                AddTrashLoot("Harvey", 3);
                AddTrashLoot("Caroline", 3);
                AddTrashLoot("Pierre", 3);
                AddTrashLoot("Shane", 3);

            }
        }

        internal void AddTrashLoot(string NPC, int can)
        {
            if (Config.GetNPC(NPC) == null) return;
            if (!Config.GetNPC(NPC).Enabled) return;
            Modworks.Items.AddTrashLoot(Module, new bwdyworks.Structures.TrashLootEntry(Module, "px" + Config.GetNPC(NPC).Abbreviate() + (Config.GetNPC(NPC).HasMaleItems() ? "m" : "f") + 1, can));
            Modworks.Items.AddTrashLoot(Module, new bwdyworks.Structures.TrashLootEntry(Module, "px" + Config.GetNPC(NPC).Abbreviate() + (Config.GetNPC(NPC).HasMaleItems() ? "m" : "f") + 2, can));
        }

        private void Events_ItemEaten(object sender, bwdyworks.Events.ItemEatenEventArgs args)
        {
            string dn = args.Item.DisplayName;
            string npc;
            if (dn.EndsWith("'s Panties")) npc = dn.Substring(0, dn.IndexOf("'s Panties"));
            else if (dn.EndsWith("'s Underwear")) npc = dn.Substring(0, dn.IndexOf("'s Underwear"));
            else return; //not one of ours
            if (string.IsNullOrWhiteSpace(npc)) return;
            string npcName = Config.LookupNPC(npc);
            if (npcName == null) return;
            var npcConfig = Config.GetNPC(npcName);
            int friendship = Modworks.Player.GetFriendshipPoints(npcName);
            if (friendship == 0) return; //don't reveal identities of unknown NPCs
            Game1.showGlobalMessage("You feel like this changes your relationship with " + npcConfig.DisplayName + ".");

            if (Modworks.RNG.NextDouble() < 0.25f + (Modworks.Player.GetLuckFactorFloat() * 0.75f))
            {
                Modworks.Player.SetFriendshipPoints(npcName, Math.Min(2500, friendship + Modworks.RNG.Next(250)));
                Modworks.Log.Trace("Relationship increased");
            }
            else
            {
                Modworks.Player.SetFriendshipPoints(npcName, Math.Max(friendship - Modworks.RNG.Next(100), 0));
                Modworks.Log.Trace("Relationship decreased");
            }
        }

        public void AddItems()
        {
            AddPersonalEffectItem("Haley", 1, "These look expensive.", 41);
            AddPersonalEffectItem("Haley", 2, "These have been worn.", 29);
            AddPersonalEffectItem("Abigail", 1, "Adventureous!", 29);
            AddPersonalEffectItem("Abigail", 2, "They smell just like " + Config.GetNPC("Abigail").GetPronoun(1) + ".", 33);
            AddPersonalEffectItem("Emily", 1, "I thought they'd be brighter.", 27);
            AddPersonalEffectItem("Emily", 2, "This is a nice material.", 35);
            AddPersonalEffectItem("Penny", 1, "How charming.", 29);
            AddPersonalEffectItem("Penny", 2, "They smell kind of sweet.", 32);
            AddPersonalEffectItem("Leah", 1, "I don't think " + Config.GetNPC("Leah").GetPronoun(0) + "'d mind.", 31);
            AddPersonalEffectItem("Leah", 2, "What a lucky find!", 34);
            AddPersonalEffectItem("Jodi", 1, "Huh.", 22);
            AddPersonalEffectItem("Jodi", 2, "Well, how about that.", 22);
            AddPersonalEffectItem("Caroline", 1, "About what I'd expect.", 24);
            AddPersonalEffectItem("Caroline", 2, "Well how about that.", 32);
            AddPersonalEffectItem("Maru", 1, "Less shy than expected.", 32);
            AddPersonalEffectItem("Maru", 2, "Must have been laundry day.", 33);
            AddPersonalEffectItem("Robin", 1, "I wonder if " + Config.GetNPC("Robin").GetPronoun(0) + " made these.", 33);
            AddPersonalEffectItem("Robin", 2, "They smell faintly of sawdust.", 36);
            AddPersonalEffectItem("Alex", 1, "They're all sweaty.", 19);
            AddPersonalEffectItem("Alex", 2, "Oh, these are interesting.", 27);
            AddPersonalEffectItem("Elliott", 1, "Only the finest.", 29);
            AddPersonalEffectItem("Elliott", 2, "Elegant.", 31);
            AddPersonalEffectItem("Harvey", 1, "They seem pretty reliable.", 24);
            AddPersonalEffectItem("Harvey", 2, UppercaseFirst(Config.GetNPC("Harvey").GetPronoun(0)) + " probably wore these.", 28);
            AddPersonalEffectItem("Sam", 1, "What are these even made of?", 33);
            AddPersonalEffectItem("Sam", 2, "They smell like " + Config.GetNPC("Sam").GetPronoun(1) + ".", 16);
            AddPersonalEffectItem("Sebastian", 1, UppercaseFirst(Config.GetNPC("Sebastian").GetPronoun(0)) + " left these behind?", 19);
            AddPersonalEffectItem("Sebastian", 2, "They're kind of... dark.", 40);
            AddPersonalEffectItem("Shane", 1, "Whoa, intense.", 14);
            AddPersonalEffectItem("Shane", 2, "They're nicer than you'd expect.", 21);
            AddPersonalEffectItem("Clint", 1, "As nice as " + Config.GetNPC("Clint").GetPronoun(0) + " owns.", 26);
            AddPersonalEffectItem("Clint", 2, "They have a strange, smoky property.", 40);
            AddPersonalEffectItem("Demetrius", 1, "I probably shouldn't have these.", 28);
            AddPersonalEffectItem("Demetrius", 2, "They don't smell worn.", 24);
            AddPersonalEffectItem("Gus", 1, "These definitely belong to " + Config.GetNPC("Gus").DisplayName + ".", 33);
            AddPersonalEffectItem("Gus", 2, "They're still kind of warm.", 36);
            AddPersonalEffectItem("Kent", 1, "They seem well-travelled.", 17);
            AddPersonalEffectItem("Kent", 2, "I don't even know what to do with these.", 22);
            AddPersonalEffectItem("Lewis", 1, "Another fine display for the fair.", 33);
            AddPersonalEffectItem("Lewis", 2, "Perfectly in character.", 19);
            AddPersonalEffectItem("Marnie", 1, "Soooo comfortable!", 25);
            AddPersonalEffectItem("Marnie", 2, "Smells like hay.", 16);
            AddPersonalEffectItem("Pam", 1, "They're... broken in.", 14);
            AddPersonalEffectItem("Pam", 2, "These smell like something, but what?", 19);
            AddPersonalEffectItem("Pierre", 1, "Clearly doing alright for " + Config.GetNPC("Pierre").GetPronoun(1) + "self.", 33);
            AddPersonalEffectItem("Pierre", 2, UppercaseFirst(Config.GetNPC("Pierre").GetPronoun(0)) + " should be more careful.", 36);
            AddPersonalEffectItem("Sandy", 1, "Absolutely fabulous.", 40);
            AddPersonalEffectItem("Sandy", 2, "Worth the trip.", 41);
            AddPersonalEffectItem("Willy", 1, "Salty.", 17);
            AddPersonalEffectItem("Willy", 2, "Smells vaguely like fish.", 21);
            AddPersonalEffectItem("Wizard", 1, "Almost ethereal - a fabric like you've never seen.", 49);
            AddPersonalEffectItem("Wizard", 2, "They are absolutely enchanting...", 41);
            AddPersonalEffectItem("Linus", 1, "Oh my. Oh.", 14);
            AddPersonalEffectItem("Linus", 2, "Hopefully " + Config.GetNPC("Linus").GetPronoun(0) + " got new ones.", 15);
            AddPersonalEffectItem("Evelyn", 1, "Aged and experienced.", 20);
            AddPersonalEffectItem("Evelyn", 2, ".....", 17);
            AddPersonalEffectItem("George", 1, "Very dependable.", 13);
            AddPersonalEffectItem("George", 2, "It's dangerous to go alone! Take this.", 15);
        }

        public void AddPersonalEffectItem(string npc, int variant, string desc, int price)
        {
            Modworks.Items.AddItem(Module, new bwdyworks.BasicItemEntry(this, "px" + Config.GetNPC(npc).Abbreviate() + (Config.GetNPC(npc).HasMaleItems() ? "m" : "f") + variant, price, -300, "Underwear", StardewValley.Object.sellAtFishShopCategory, Config.GetNPC(npc).DisplayName + "'s " + (Config.GetNPC(npc).HasMaleItems() ? "Underwear" : "Panties"), desc));
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
            if (e.Button.IsActionButton())
            {
                if(Game1.player.ActiveObject != null)
                {
                    string name = Game1.player.ActiveObject.Name;
                    if(name.Contains("'s Underwear") || name.Contains("'s Panties"))
                    {
                        if(Game1.player.currentLocation.isCharacterAtTile(e.Cursor.GrabTile) == null)
                            Modworks.Player.ForceOfferEatInedibleHeldItem();
                    }
                }
            }
        }
    }
}