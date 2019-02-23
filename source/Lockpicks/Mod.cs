using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using System.Collections.Generic;

namespace Lockpicks
{
    public class Mod : StardewModdingAPI.Mod
    {
        public static Mod instance;
#if DEBUG
        public static bool Debug = true;
#else
        public static bool Debug = false;
#endif
        public override void Entry(IModHelper helper)
        {
            instance = this;
            Monitor.Log("Lockpicks reporting in. " + (Debug ? "Debug" : "Release") + " build active.");

            Config.Load();
            if (Config.ready)
            {
                Item.Setup();
            }
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Debug)
            {
                if (e.Button == SButton.NumPad0)
                {
                    var f = Game1.getFarmer(0);
                    //give lockpick
                    Item ei = Item.items["lx.lockpick"];
                    StardewValley.Object i = (StardewValley.Object)StardewValley.Objects.ObjectFactory.getItemFromDescription(0, ei.internal_id, 1);
                    i.IsSpawnedObject = true;
                    i.ParentSheetIndex = ei.internal_id;
                    f.addItemByMenuIfNecessary(i);
                }
            }
            if (e.Button.IsActionButton())
            {
                var f = Game1.getFarmer(0);
                if (Context.IsPlayerFree)
                {
                    if (f.ActiveObject != null && f.ActiveObject.DisplayName == "Lockpick")
                    {
                        int target_x = (int)(f.getTileX());
                        int target_y = (int)(f.getTileY());
                        int d = f.FacingDirection;
                        switch (d)
                        {
                            case 0: target_y -= 1; break; //up
                            case 1: target_x += 1; break; //right
                            case 2: target_y += 1; break; //down
                            case 3: target_x -= 1; break; //left
                        }
                        if(Debug) Monitor.Log("Facing target: " + Game1.currentLocation.Name + ", " + target_x + ", " + target_y);
                        var cle = new ConfigLockEnd(Game1.currentLocation.Name, target_x, target_y);
                        var cle2 = Config.GetMatchingLockEnd(cle);
                        if (cle2 != null)
                        {
                            this.Helper.Input.Suppress(e.Button);//prevent the original click
                            Response[] responses = new[]
                            {
                                new Response(cle2.str(), "Yes"),
                                new Response("No", "No"),
                            };
                            Game1.currentLocation.lastQuestionKey = "custom_lockpick";
                            Game1.currentLocation.createQuestionDialogue("Use lockpick?", responses, handleLockpickMenuResponse);
                        }
                    }
                }
            }
        }

        public void handleLockpickMenuResponse(Farmer who, string answerKey)
        {
            if (answerKey != "No")
            {
                if(new System.Random().Next(100) < 30)
                {
                    //ohno. the pick broke.
                    Game1.playSound("axe");
                    Game1.showRedMessage("The lockpick broke!");
                    int count = who.ActiveObject.getStack();
                    if (count <= 1)
                    {
                        who.removeItemFromInventory(who.ActiveObject);
                    }
                    else who.ActiveObject.Stack = who.ActiveObject.Stack - 1;
                    return;
                }
                Game1.playSound("axchop");
                ConfigLockEnd cle2 = new ConfigLockEnd(answerKey);
                who.warpFarmer(new Warp(cle2.MapX, cle2.MapY, cle2.MapName, cle2.MapX, cle2.MapY, false));
            }
        }
    }
}