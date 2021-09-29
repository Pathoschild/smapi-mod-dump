/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/EscasModdingPlugins
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using xTile;
using xTile.Tiles;
using xTile.ObjectModel;
using xTile.Format;
using xTile.Display;
using xTile.Dimensions;
using xTile.Layers;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace EscasModdingPlugins
{
    /// <summary>Draws exclamation point indicators above "Action" "CustomBoard" property tiles based on an optional parameter.</summary>
    public static class DisplayNewOrderExclamationPoint
    {
        /// <summary>True if these commands are currently enabled.</summary>
		public static bool Enabled { get; private set; } = false;
        /// <summary>The helper instance to use for API access.</summary>
        private static IModHelper Helper { get; set; } = null;
        /// <summary>The monitor instance to use for console/log messages.</summary>
        private static IMonitor Monitor { get; set; } = null;

        /// <summary>Initializes this class's SMAPI console commands.</summary>
        /// <param name="helper">The helper instance to use for API access.</param>
        /// <param name="monitor">The monitor instance to use for console/log messages.</param>
        public static void Enable(IModHelper helper, IMonitor monitor)
        {
            if (Enabled)
                return; //do nothing

            //store args
            Helper = helper;
            Monitor = monitor;

            //initialize events
            helper.Events.Player.Warped += Warped_UpdateTileList;
            helper.Events.GameLoop.DayStarted += DayStarted_UpdateTileList;
            helper.Events.Display.RenderedWorld += RenderedWorld_DrawExclamationPoints;
        }

        /// <summary>Updates the exclamation point tile list whenever a new day begins.</summary>
        private static void DayStarted_UpdateTileList(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            UpdateTileList(Game1.player.currentLocation); //update for the current player's location
        }

        /// <summary>Updates the exclamation point list whenever the local player warps to a new location.</summary>
        private static void Warped_UpdateTileList(object sender, StardewModdingAPI.Events.WarpedEventArgs e)
        {
            if (!e.IsLocalPlayer) //if the warping player is NOT the current local player
                return;

            UpdateTileList(e.NewLocation); //update for the current player's new location
        }

        /// <summary>Draws a hovering excalamation point above each entry in <see cref="ExclamationPointTiles"/> for the current player.</summary>
        private static void RenderedWorld_DrawExclamationPoints(object sender, StardewModdingAPI.Events.RenderedWorldEventArgs e)
        {
            if (!Context.IsPlayerFree) //if the plyer is NOT free (events, menus, etc)
                return; //do nothing

            foreach (ExclamationData point in ExclamationPointTiles.Value) //for each exclamation point to draw
            {
                if (!Game1.player.team.acceptedSpecialOrderTypes.Contains(point.OrderType)) //if the players do NOT already have an active order of this type
                {
                    //draw the exclamation point (imitate code from Town.draw(SpriteBatch))
                    float yOffset = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);
                    e.SpriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(point.PixelPosition.X, point.PixelPosition.Y + yOffset)), SourceRectangle, Color.White, 0f, new Vector2(1f, 4f), 4f + Math.Max(0f, 0.25f - yOffset / 16f), SpriteEffects.None, 1f);
                }
            }
        }

        /// <summary>The spritesheet rectangle for the draw event, cached here for minor optimization purposes.</summary>
        private static Microsoft.Xna.Framework.Rectangle SourceRectangle = new Microsoft.Xna.Framework.Rectangle(395, 497, 3, 8);

        /// <summary>A list of tiles where "available orders" indicators should be displayed if applicable. One instance per local player (e.g. splitscreen mode).</summary>
        private static PerScreen<List<ExclamationData>> ExclamationPointTiles { get; set; } = new PerScreen<List<ExclamationData>>(() => new List<ExclamationData>()); //initialize a blank list for each local player

        /// <summary>Information needed to draw a special orders board's exclamation point.</summary>
        private class ExclamationData
        {
            /// <summary>The default pixel position of the exclamation point in the world (not relative to the UI or player).</summary>
            public Vector2 PixelPosition;
            /// <summary>The board's custom order type. Used to check whether the exclamation point should be visible.</summary>
            public string OrderType;

            /// <param name="pixelPosition">The default pixel position of the exclamation point in the world (not relative to the UI or player).</param>
            /// <param name="orderType">The board's custom order type. Used to check whether the exclamation point should be visible.</param>
            public ExclamationData(Vector2 pixelPosition, string orderType)
            {
                PixelPosition = pixelPosition;
                OrderType = orderType;
            }
        }

        /// <summary>Updates <see cref="ExclamationPointTiles"/> based on the given location's tile properties.</summary>
        /// <param name="location">The current player's location.</param>
        private static void UpdateTileList(GameLocation location)
        {
            ExclamationPointTiles.Value.Clear(); //clear the local player's list

            try
            {
                if (location?.Map.GetLayer("Buildings") is Layer layer) //if this location exists and has a Buildings layer
                {
                    //search every tile for "Buildings" layer tile properties matching this pattern: "Action" "CustomBoard <boardname> true"
                    for (int x = 0; x < layer.LayerSize.Width; x++)
                    {
                        for (int y = 0; y < layer.LayerSize.Height; y++)
                        {
                            if (location.doesTileHaveProperty(x, y, "Action", "Buildings") is string actionProperty) //if this tile has an Action property
                            {
                                string[] args = actionProperty.Split(' ');
                                if (args.Length > 2 && args[0].Equals("CustomBoard", StringComparison.OrdinalIgnoreCase) && args[2].Equals("true"))
                                {
                                    string orderType = args[1]; //use the second param as the order type

                                    if (!orderType.StartsWith(ModEntry.PropertyPrefix, StringComparison.OrdinalIgnoreCase)) //if the order type does NOT start with "Esca.EMP/"
                                        orderType = ModEntry.PropertyPrefix + orderType; //add that prefix before using it

                                    Vector2 position = new Vector2((x * 64f) + 32f, (y * 64f) - 32f); //get pixel position for this tile, make cosmetic adjustments
                                    ExclamationPointTiles.Value.Add(new ExclamationData(position, orderType)); //add this tile + order type to the list

                                    if (Monitor.IsVerbose)
                                        Monitor.Log($"CustomBoard tile with '!' notifications found. Tile: {x},{y}. Location: {location.Name}.", LogLevel.Trace);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Monitor.LogOnce($"Something went wrong while trying to find custom order board exclamantion points. Full error message: {ex.ToString()}", LogLevel.Error);
            }
        }
    }
}
