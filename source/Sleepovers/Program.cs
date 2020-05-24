using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using StardewValley.Menus;

namespace Sleepovers
{
    public class Mod : StardewModdingAPI.Mod
    {
        public static List<string> NoSleep;
        public static List<string> Attempts = new List<string>();
        public static Random RNG = new Random(Guid.NewGuid().GetHashCode());
        public static NPC LastSleepoverNPC = null;
        public static bool SleepPrimed = false;
        

        public override void Entry(IModHelper helper)
        {
            NPCCheckAction += Events_NPCCheckAction;
            helper.Events.GameLoop.DayStarted +=
                new EventHandler<DayStartedEventArgs>(delegate (object o, DayStartedEventArgs a) { Attempts.Clear(); });
            try
            {
                string filecontents = File.ReadAllText(Helper.DirectoryPath + Path.DirectorySeparatorChar + "blacklist.json");
                NoSleep = JsonConvert.DeserializeObject<List<string>>(filecontents);
            }
            catch (Exception) { NoSleep = new List<string>(); }
            Helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            Helper.Events.GameLoop.OneSecondUpdateTicked += GameLoop_OneSecondUpdateTicked;
            
        }

        private void GameLoop_OneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            if (SleepPrimed)
            {
                SleepPrimed = false;
                LastSleepoverNPC.farmerPassesThrough = true;
            }
            if (LastSleepoverNPC == null) return;
            else if (LastSleepoverNPC.currentLocation != Game1.player.currentLocation)
            {
                LastSleepoverNPC.farmerPassesThrough = false;
                LastSleepoverNPC = null;
            }
            else if (Math.Max(Math.Abs(Game1.player.Position.X - LastSleepoverNPC.Position.X), Math.Abs(Game1.player.Position.Y - LastSleepoverNPC.Position.Y)) > 64)
            {
                LastSleepoverNPC.farmerPassesThrough = false;
                LastSleepoverNPC = null;
            }
        }

        private void Events_NPCCheckAction(object sender, NPCCheckActionEventArgs args)
        {
            if (args.Cancelled) return; //someone else already ate this one
            if (Game1.player.ActiveObject == null) //empty hands to sleep
            {
                if(!NoSleep.Contains(args.NPC.Name) && args.NPC.isSleeping)
                {
                    if (Attempts.Contains(args.NPC.Name)) return; //already tried. no means no.
                    args.Cancelled = true;
                    Game1.currentLocation.lastQuestionKey = "sleepover";
                    Game1.currentLocation.createQuestionDialogue("Sleepover with " + args.NPC.Name + "?", new[] { new Response(args.NPC.Name, "Yes"), new Response(".nope.", "No") }, QuestionCallback);
                }
            }
        }

        private void startSleep(NPC npc)
        {
            Game1.player.timeWentToBed.Value = Game1.timeOfDay;

            if (Game1.IsMultiplayer)
            {
                Game1.player.team.SetLocalReady("sleep", true);
                Game1.dialogueUp = false;
                Game1.activeClickableMenu = (IClickableMenu)new ReadyCheckDialog("sleep", true, (ConfirmationDialog.behavior)(who =>
                {
                    this.doSleep(npc);
                }), (ConfirmationDialog.behavior)(who =>
                {
                    if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ReadyCheckDialog)
                        (Game1.activeClickableMenu as ReadyCheckDialog).closeDialog(who);
                    who.timeWentToBed.Value = 0;
                }));
            }
            else
                this.doSleep(npc);

            if (Game1.player.team.announcedSleepingFarmers.Contains(Game1.player)) return;
            Game1.player.team.announcedSleepingFarmers.Add(Game1.player);
            if (!Game1.IsMultiplayer) return;
            string str = "GoneToBed";
            if (Game1.random.NextDouble() < 0.75)
            {
                if (Game1.timeOfDay < 1800) str += "Early";
                else if (Game1.timeOfDay > 2530) str += "Late";
            }
            int num = 0;
            for (int index = 0; index < 2; ++index)
            {
                if (Game1.random.NextDouble() < 0.25) ++num;
            }
            ((Multiplayer)typeof(Game1).GetField("multiplayer", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null)).globalChatInfoMessage(str + (object)num, Game1.player.displayName);
        }

        private void doSleep(NPC npc)
        {
            Game1.NewDay(1f);
            SleepPrimed = true;
            Game1.player.mostRecentBed = Game1.player.Position;
            Game1.player.doEmote(24);
            Game1.player.freezePause = 2000;
            LastSleepoverNPC = npc;
        }

        public void QuestionCallback(Farmer who, string npc)
        {
            if (npc != ".nope.")
            {
                Attempts.Add(npc);
                int friendship = Game1.player.getFriendshipLevelForNPC(npc);
                if (friendship < 500)
                {
                    //offensive to even ask - you shouldn't be in the room.
                    Game1.showRedMessage(npc + " is offended you would ask.");
                    Game1.player.changeFriendship(0 - RNG.Next(100), Game1.getCharacterFromName(npc));
                }
                else if (friendship < 750)
                {
                    Game1.showRedMessage(npc + " doesn't know you that well.");
                }
                else
                {
                    float chances = (float)friendship / 2600f; //1000 would be just shy of 0.4
                    Random rng = new Random(DateTime.Now.Millisecond);
                    if (rng.NextDouble() <= chances)
                    {
                        Game1.player.changeFriendship(RNG.Next(100), Game1.getCharacterFromName(npc));
                        Game1.player.isInBed.Value = true;
                        startSleep(Game1.getCharacterFromName(npc));
                    }
                    else
                    {
                        Game1.showRedMessage(npc + " doesn't want to.");
                    }
                }

            }
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.IsSuppressed()) return; //already eaten by someone else.
            //check for Character
            if (!Game1.eventUp)
            {
                if (e.Button.IsActionButton() || e.Button.IsUseToolButton())
                {
                    if (Context.IsPlayerFree)
                    {
                        //get the target tile
                        Vector2 vector = new Vector2(Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y) / 64f;
                        if (Game1.mouseCursorTransparency == 0f || !Game1.wasMouseVisibleThisFrame || (!Game1.lastCursorMotionWasMouse && (Game1.player.ActiveObject == null || (!Game1.player.ActiveObject.isPlaceable() && Game1.player.ActiveObject.Category != -74))))
                        {
                            vector = Game1.player.GetGrabTile();
                            if (vector.Equals(Game1.player.getTileLocation()))
                                vector = Utility.getTranslatedVector2(vector, Game1.player.FacingDirection, 1f);
                        }
                        if (!Utility.tileWithinRadiusOfPlayer((int)vector.X, (int)vector.Y, 1, Game1.player))
                        {
                            vector = Game1.player.GetGrabTile();
                            if (vector.Equals(Game1.player.getTileLocation()) && Game1.isAnyGamePadButtonBeingPressed())
                                vector = Utility.getTranslatedVector2(vector, Game1.player.FacingDirection, 1f);
                        }

                        //check characters
                        NPC character = Game1.currentLocation.isCharacterAtTile(vector);
                        if (character != null)
                        {
                            var argsResult = NPCCheckActionEvent(Game1.player, character);
                            if (argsResult.Cancelled) { Helper.Input.Suppress(e.Button); }
                        }
                        else
                        {
                            vector = Utility.getTranslatedVector2(vector, Game1.player.FacingDirection, 0f);
                            vector.Y += 1;
                            character = Game1.currentLocation.isCharacterAtTile(vector);
                            if (character != null)
                            {
                                var argsResult = NPCCheckActionEvent(Game1.player, character);
                                if (argsResult.Cancelled) { Helper.Input.Suppress(e.Button); }
                            }
                            else
                            {
                                vector = Game1.player.getTileLocation();
                                character = Game1.currentLocation.isCharacterAtTile(vector);
                                if (character != null)
                                {
                                    var argsResult = NPCCheckActionEvent(Game1.player, character);
                                    if (argsResult.Cancelled) { Helper.Input.Suppress(e.Button); }
                                }
                            }
                        }
                    }
                }
            }
        }
        public event NPCCheckActionHandler NPCCheckAction;
        public delegate void NPCCheckActionHandler(object sender, NPCCheckActionEventArgs args);
        internal NPCCheckActionEventArgs NPCCheckActionEvent(Farmer who, NPC npc)
        {
            var args = new NPCCheckActionEventArgs
            {
                NPC = npc,
                Farmer = who,
                Cancelled = false
            };
            if (NPCCheckAction != null) NPCCheckAction.Invoke(this, args);
            return args;
        }

        public class NPCCheckActionEventArgs
        {
            public NPC NPC { set; get; }
            public Farmer Farmer { set; get; }
            public bool Cancelled { get; set; }
        }
    }
}