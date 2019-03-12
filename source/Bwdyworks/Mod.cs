using StardewModdingAPI;
using System.Reflection;
using bwdyworks.Registry;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using bwdyworks.Structures;
using System.Collections.Generic;
using Netcode;

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
            if (Game1.player == null) return;
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
                if (Game1.player.itemToEat == null) return;
                EatingItem = Game1.player.itemToEat;
                if (Game1.player.ActiveObject == null) return;
                eatingQuantity = Game1.player.ActiveObject.Stack;
            }
        }

        private void Input_ButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            if (e.IsSuppressed()) return; //already eaten by someone else.
            //check for Character or Action activation
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

                        //check characters
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

                        //reset the target tile
                        vector = new Vector2(Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y) / 64f;
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

                        //check Actions in tiledata
                        string value = Game1.currentLocation.doesTileHaveProperty((int)vector.X, (int)vector.Y, "Action", "Buildings");
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            var argsResult = Modworks.Events.TileCheckActionEvent(Game1.player, Game1.currentLocation, vector, value);
                            if (argsResult.Cancelled) { Helper.Input.Suppress(e.Button); }
                            else if (value.Split(' ')[0] == "Garbage" && DoTrashLoot(value)) Helper.Input.Suppress(e.Button);
                        }
                        else
                        {
                            vector = Utility.getTranslatedVector2(vector, Game1.player.FacingDirection, 0f);
                            vector.Y += 1;
                            value = Game1.currentLocation.doesTileHaveProperty((int)vector.X, (int)vector.Y, "Action", "Buildings");
                            if (!string.IsNullOrWhiteSpace(value))
                            {
                                var argsResult = Modworks.Events.TileCheckActionEvent(Game1.player, Game1.currentLocation, vector, value);
                                if (argsResult.Cancelled) { Helper.Input.Suppress(e.Button); }
                                else if (value.Split(' ')[0] == "Garbage" && DoTrashLoot(value)) Helper.Input.Suppress(e.Button);
                            }
                            else
                            {
                                vector = Game1.player.getTileLocation();
                                value = Game1.currentLocation.doesTileHaveProperty((int)vector.X, (int)vector.Y, "Action", "Buildings");
                                if (!string.IsNullOrWhiteSpace(value))
                                {
                                    var argsResult = Modworks.Events.TileCheckActionEvent(Game1.player, Game1.currentLocation, vector, value);
                                    if (argsResult.Cancelled) { Helper.Input.Suppress(e.Button); }
                                    else if (value.Split(' ')[0] == "Garbage" && DoTrashLoot(value)) Helper.Input.Suppress(e.Button);
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool DoTrashLoot(string value)
        {
            var lootTable = new List<TrashLootEntry>();
            bool hasFilter = int.TryParse(value.Split(' ')[1], out int which);
            if (!hasFilter) return false;
            StardewValley.Locations.Town t = (StardewValley.Locations.Town) Game1.getLocationFromName("Town");

            //was this can already checked today?
            NetArray<bool, NetBool> wasCheckedArray = (NetArray<bool, NetBool>)typeof(StardewValley.Locations.Town).GetField("garbageChecked", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(t);
            if (wasCheckedArray[which]) return false;

            //build a loot table specific to this can, as provided by mods
            foreach (var l in API.Items.TrashLoot)
            {
                if (l.Filter == -1) lootTable.Add(l); //"any can" item
                else if (l.Filter == which) lootTable.Add(l);
            }

            if (lootTable.Count > 0 && Modworks.RNG.NextDouble() < (System.Math.Min(0.1d + Modworks.Player.GetLuckFactorFloat(),0.9d))) { //10% to 90% chance, influenced by luck
                TrashLootEntry tle = lootTable[Modworks.RNG.Next(lootTable.Count)];
                int? itemId = Modworks.Items.GetModItemId(tle.Module, tle.ItemID);
                if (!itemId.HasValue)
                {
                    Modworks.Log.Debug("Attempted to award trash item of invalid id: " + tle.ItemID);
                    return false;
                }
                Modworks.Player.GiveItem(itemId.Value);
                Game1.playSound("trashcan");
                wasCheckedArray[which] = true; //mark the can as checked
                return true;
            }
            return false;
        }


        private void GameLoop_Saving(object sender, StardewModdingAPI.Events.SavingEventArgs e)
        {
            ItemRegistry.Save();
        }
    }
}