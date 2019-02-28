using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using Modworks = bwdyworks.Modworks;

using System.Collections.Generic;

namespace Lockpicks
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
                Modworks.Items.AddItem(Module, new bwdyworks.BasicItemEntry(this, "lockpick", 30, -300, "Basic", Object.junkCategory, "Lockpick", "Used to bypass locked doors."));
                Modworks.Items.AddMonsterLoot(Module, new bwdyworks.MonsterLootEntry(this, "Green Slime", "lockpick", 0.1f));                 
            }
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Debug)
            {
                if (e.Button == SButton.NumPad0)
                {
                    var id = Modworks.Items.GetModItemId(Module, "lockpick");
                    if (Debug && id != null) Monitor.Log("lockpick id: " + id.Value);

                    if (id != null) Modworks.Player.GiveItem(id.Value, 1);
                }
            }
            if (e.Button.IsActionButton())
            {
                if (Context.IsPlayerFree)
                {
                    var ao = Game1.player.ActiveObject;
                    if (ao != null && ao.DisplayName == "Lockpick")
                    {
                        var targetOut = Modworks.Player.GetFacingTileCoordinate();
                        var keyOut = Game1.currentLocation.Name + "." + targetOut[0] + "." + targetOut[1];
                        if (Debug) Monitor.Log("Facing target: " + keyOut);

                        var targetIn = Modworks.Player.GetStandingTileCoordinate();
                        var keyIn = Game1.currentLocation.Name + "." + targetIn[0] + "." + targetIn[1];
                        if (Debug) Monitor.Log("Standing at: " + keyIn);

                        //check out lock
                        var cle2 = Config.GetMatchingOutLock(keyOut);
                        if (cle2 != null)
                        {
                            Helper.Input.Suppress(e.Button);
                            Modworks.Menus.AskQuestion("Use lockpick?", new[]{new Response(keyOut, "Yes"),new Response("No","No")}, QuestionCallbackOutLock);
                        }
                        //check in lock
                        cle2 = Config.GetMatchingInLock(keyIn);
                        if (cle2 != null)
                        {
                            Helper.Input.Suppress(e.Button);
                            Modworks.Menus.AskQuestion("Use lockpick?", new[] { new Response(keyIn, "Yes"), new Response("No", "No") }, QuestionCallbackInLock);
                        }
                    }
                }
            }
        }

        public void QuestionCallbackOutLock(Farmer who, string answerKey)
        {
            if (answerKey != "No")
            {
                ConfigLockEnd cle2 = Config.GetMatchingOutLock(answerKey);
                if (new System.Random().Next(100) < 10) //ohno. the pick broke.
                {
                    Game1.playSound("axe");
                    Game1.showRedMessage("The lockpick broke!");
                    Modworks.Player.RemoveItem(who.ActiveObject);
                    return;
                }
                Game1.playSound("axchop");
                who.warpFarmer(new Warp(cle2.MapX, cle2.MapY, cle2.MapName, cle2.MapX, cle2.MapY, false));
            }
        }

        public void QuestionCallbackInLock(Farmer who, string answerKey)
        {
            if (answerKey != "No")
            {
                ConfigLockEnd cle2 = Config.GetMatchingInLock(answerKey);
                Game1.playSound("axchop");
                who.warpFarmer(new Warp(cle2.MapX, cle2.MapY, cle2.MapName, cle2.MapX, cle2.MapY, false));
            }
        }
    }
}