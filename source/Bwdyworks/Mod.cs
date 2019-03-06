using StardewModdingAPI;
using System.Reflection;

//a simple mod wrapper for logging purposes.
//the real magic is the bwdymod class.
using bwdyworks.Registry;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

namespace bwdyworks
{
    public class Mod : StardewModdingAPI.Mod
    {
#if DEBUG
        private static readonly bool DEBUG = true;
#else
        private static readonly bool DEBUG = false;
#endif

        public override void Entry(IModHelper helper)
        {
            Modworks.Startup(this);
            Monitor.Log("bwdy here! let's have some fun <3 " + Assembly.GetEntryAssembly().GetName().Version.ToString() + (DEBUG ? " (DEBUG MODE ACTIVE)":""));
            Helper.Events.GameLoop.Saving += GameLoop_Saving;
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
        }


        public static int tickUpdateLimiter = 0;
        public static bool EatingPrimed = false;
        public static StardewValley.Item EatingItem;
        public static int eatingQuantity = 0;
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
                    if (Game1.player.ActiveObject == null || Game1.player.ActiveObject.Stack < eatingQuantity) Modworks.Events.ItemEatenEvent(Game1.player, EatingItem);
                    EatingItem = null;
                }
            }
            else if (Game1.player.isEating)
            {
                EatingPrimed = true;
                EatingItem = Game1.player.itemToEat;
                eatingQuantity = Game1.player.ActiveObject.Stack;
            }
        }

        private void Input_ButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            //check for NPC Check Action event
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
                            {
                                vector = Utility.getTranslatedVector2(vector, Game1.player.FacingDirection, 1f);
                            }
                        }
                        if (!Utility.tileWithinRadiusOfPlayer((int)vector.X, (int)vector.Y, 1, Game1.player))
                        {
                            vector = Game1.player.GetGrabTile();
                            if (vector.Equals(Game1.player.getTileLocation()) && Game1.isAnyGamePadButtonBeingPressed())
                            {
                                vector = Utility.getTranslatedVector2(vector, Game1.player.FacingDirection, 1f);
                            }
                        }

                        NPC character = Game1.currentLocation.isCharacterAtTile(vector);
                        if (character != null)
                        {
                            var argsResult = Modworks.Events.NPCCheckActionEvent(Game1.player, character);
                            if (argsResult.Cancelled) { Helper.Input.Suppress(e.Button); }
                        }
                        else
                        {
                            vector = Utility.getTranslatedVector2(vector, Game1.player.FacingDirection, 0f);
                            vector.Y += 1;
                            character = Game1.currentLocation.isCharacterAtTile(vector);
                            if (character != null)
                            {
                                var argsResult = Modworks.Events.NPCCheckActionEvent(Game1.player, character);
                                if (argsResult.Cancelled) { Helper.Input.Suppress(e.Button); }
                            } else
                            {
                                vector = Game1.player.getTileLocation();
                                character = Game1.currentLocation.isCharacterAtTile(vector);
                                if (character != null)
                                {
                                    var argsResult = Modworks.Events.NPCCheckActionEvent(Game1.player, character);
                                    if (argsResult.Cancelled) { Helper.Input.Suppress(e.Button); }
                                }
                            }
                        }
                    }
                }
            }
        }


        private void GameLoop_Saving(object sender, StardewModdingAPI.Events.SavingEventArgs e)
        {
            ItemRegistry.Save();
        }
    }
}