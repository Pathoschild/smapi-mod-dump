/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheThor59/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using HarmonyLib;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Thor.Stardew.Mods.HoeWaterDirection
{
    /// <summary>
    /// Main class of the mod
    /// </summary>
    public class ModEntry : Mod
    {
        //Store the aim of the action when moving mouse
        private static Vector2? aim;
        //Store reference to this mod for Stardew function override
        private static ModEntry mod;
        //Store if the user is using tool or not
        private bool isClicking = false;
        //Store rotation changes
        private static int previousDirection = -1;

        // When user move the cursor we save position of mouse if 
        public void OnCursorMoved(object sender, CursorMovedEventArgs e)
        {
            if (isClicking)
            {
                aim = Game1.currentCursorTile;
            }
        }

        // Activate mouse tracking when using tools
        public void onButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.IsUseToolButton())
            {
                previousDirection = -1;
                isClicking = true;
                aim = null;
            }
        }


        // Deactivate mouse tracking when using tools
        public void onButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (e.Button.IsUseToolButton())
            {
                isClicking = false;
            }
        }

        // When a game start we add handlers on mouse tracking
        public void onGameStart(object sender, GameLaunchedEventArgs e)
        {
            Helper.Events.Input.CursorMoved += OnCursorMoved;
            Helper.Events.Input.ButtonPressed += onButtonPressed;
            Helper.Events.Input.ButtonReleased += onButtonReleased;
        }

        // When a game stop we remove handlers on mouse tracking
        public void onGameStopped(object sender, ReturnedToTitleEventArgs e)
        {
            Helper.Events.Input.CursorMoved -= OnCursorMoved;
            Helper.Events.Input.ButtonPressed -= onButtonPressed;
            Helper.Events.Input.ButtonReleased -= onButtonReleased;
        }

        // Mod start, we subscrive every elements we need to make i work here
        public override void Entry(IModHelper helper)
        {
            mod = this;
            Helper.Events.GameLoop.GameLaunched += onGameStart;
            Helper.Events.GameLoop.ReturnedToTitle += onGameStopped;
            // In this part we have to override a behavior of Stardew that is not available thought the API
            var harmony = new Harmony(ModManifest.UniqueID);
            try
            {
                harmony.Patch(
                   original: AccessTools.Method(typeof(Tool), "tilesAffected"),
                   prefix: new HarmonyMethod(typeof(ModEntry), nameof(HandleChangeDirection))
                );
            }
            catch (Exception e)
            {
                Monitor.Log(string.Format("Error while trying to setup required patches: {0}", e.Message), LogLevel.Error);
            }
        }
        
        // Here we handle the use of tool by checking if it's a watering can or a hoe
        public static bool HandleChangeDirection(ref Tool __instance, ref List<Vector2> __result, ref Vector2 tileLocation, ref int power, ref Farmer who)
        {
            try
            {
                if (__instance is WateringCan || __instance is Hoe)
                {
                    __result = HandleChangeDirectoryImpl(ref tileLocation, ref power, ref who);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                mod.Monitor.Log(string.Format("There was an exception in a patch", e.Message), LogLevel.Error);
                return true;
            }
        }

        //Here we handle the direction change by reusing the implementation from stardew but adding the rotation change
        public static List<Vector2> HandleChangeDirectoryImpl(ref Vector2 tileLocation, ref int power, ref Farmer who)
        {
            who.CanMove = true;
            ++power;
            List<Vector2> tiles = new List<Vector2>();
            tiles.Add(Vector2.Zero);
            Vector2 vector2 = Vector2.Zero;
            if (power >= 6)
            {
                vector2 = new Vector2(0, 0 - 2f);
            }
            else
            {
                if (power >= 2)
                {
                    tiles.Add(new Vector2(0.0f, -1f));
                    tiles.Add( new Vector2(0.0f, -2f));
                }
                if (power >= 3)
                {
                    tiles.Add(new Vector2(0.0f, -3f));
                    tiles.Add(new Vector2(0.0f, -4f));
                }
                if (power >= 4)
                {
                    tiles.RemoveAt(tiles.Count - 1);
                    tiles.RemoveAt(tiles.Count - 1);
                    tiles.Add( new Vector2(1f, -2f));
                    tiles.Add(new Vector2(1f, -1f));
                    tiles.Add(new Vector2(1f, 0.0f));
                    tiles.Add(new Vector2(-1f, -2f));
                    tiles.Add(new Vector2(-1f, -1f));
                    tiles.Add(new Vector2(-1f, 0.0f));
                }
                if (power >= 5)
                {
                    for (int index = tiles.Count - 1; index >= 0; --index)
                        tiles.Add(tiles[index] + new Vector2(0.0f, -3f));
                }
            }
                
            if (power >= 6)
            {
                tiles.Clear();
                for (int index1 = (int)vector2.X - 2; index1 <= vector2.X + 2.0; ++index1)
                {
                    for (int index2 = (int)vector2.Y - 2; index2 <= vector2.Y + 2.0; ++index2)
                        tiles.Add(new Vector2(index1, index2));
                }
            }

            Vector2 newTileLocation = Game1.player.getTileLocation();
            int newFacingDirection = previousDirection != -1 ? previousDirection : who.facingDirection;
            if (!aim.HasValue)
            {
                aim = tileLocation;
            }
            Vector2 diff = newTileLocation - (aim.HasValue ? aim.Value : tileLocation);
            if (diff.X >= 0.5)
            {
                newTileLocation.X -= 1;
                if (diff.Y < 1 && diff.Y > -1)
                {
                    newFacingDirection = 3;
                }
            }
            else if (diff.X <= -0.5)
            {
                newTileLocation.X += 1;
                if (diff.Y < 1 && diff.Y > -1)
                {
                    newFacingDirection = 1;
                }
            }
            if (diff.Y >= 0.5)
            {
                newTileLocation.Y -= 1;
                if (diff.X < 1 && diff.X > -1)
                {
                    newFacingDirection = 0;
                }
            }
            else if (diff.Y <= -0.5)
            {
                newTileLocation.Y += 1;
                if (diff.X < 1 && diff.X > -1)
                {
                    newFacingDirection = 2;
                }
            }
            previousDirection = newFacingDirection;
            RotateIfNeeded(ref tiles, newFacingDirection);


            for (int i = 0; i < tiles.Count; i++)
            {
                tiles[i] += newTileLocation;
            }

            return tiles;
        }

        // This will rotate the affected tiles according to the direction
        private static void RotateIfNeeded(ref List<Vector2> tileLocations, int facingDirection)
        {
            switch (facingDirection)
            {
                case 1:
                    for (int i = 0; i < tileLocations.Count; i++)
                    {
                        tileLocations[i] = new Vector2(-tileLocations[i].Y, -tileLocations[i].X);
                    }
                    break;

                case 2:
                    for (int i = 0; i < tileLocations.Count; i++)
                    {
                        tileLocations[i] = -tileLocations[i];
                    }
                    break;

                case 3:
                    for (int i = 0; i < tileLocations.Count; i++)
                    {
                        tileLocations[i] = new Vector2(tileLocations[i].Y, tileLocations[i].X);
                    }
                    break;
            }
        }
    }
}
