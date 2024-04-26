/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cheesysteak/stardew-steak
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoreMultiplayerInfo.Helpers;
using MoreMultiplayerInfo.Models;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using StardewModdingAPI.Utilities;

namespace MoreMultiplayerInfo.EventHandlers
{
    public class PlayerIconMenu : IClickableMenu
    {
        private readonly ReadyCheckHandler _readyCheckHandler;

        private readonly IMonitor _monitor;
        
        private readonly IModHelper _helper;

        public PlayerIconMenu(ReadyCheckHandler readyCheckHandler, IMonitor monitor, IModHelper helper)
        {
            _readyCheckHandler = readyCheckHandler;
            _monitor = monitor;
            _helper = helper;
            Icons = new List<PlayerIcon>();

            _helper.Events.GameLoop.UpdateTicked += SetupIcons;

            _helper.Events.Display.WindowResized += SetupIcons;
        }

        private void SetupIcons(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady) return;

            _helper.Events.GameLoop.UpdateTicked -= SetupIcons;

            SetupIcons();
        }

        private float WaitingIconScale => 0.5f;

        private float OfflineIconScale => 0.5f;

        private float PlayerIconScale => 0.75f;

        private int HeadshotIconSize => Convert.ToInt32(16 * Game1.pixelZoom * PlayerIconScale);

        public List<PlayerIcon> Icons { get; set; }


        public void SetupIcons()
        {
            Icons = new List<PlayerIcon>();

            var players = PlayerHelpers.GetAllCreatedFarmers();

            this.allClickableComponents = new List<ClickableComponent>();

            this.xPositionOnScreen = Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Right - 90;
            this.yPositionOnScreen = 310;
            this.height = players.Count * HeadshotIconSize;
            this.width = HeadshotIconSize;

            for (var idx = 0; idx < players.Count; idx++)
            {
                var player = players[idx];

                var yOffset = (idx * 50);

                var yPos = this.yPositionOnScreen + yOffset;
                var xPos = this.xPositionOnScreen;

                var waitingIconSize = Convert.ToInt32(9 * Game1.pixelZoom * WaitingIconScale);
                var offlineIconSize = Convert.ToInt32(11 * Game1.pixelZoom * OfflineIconScale);


                var waitingIcon = new ClickableTextureComponent(
                    name: "",
                    bounds: new Rectangle(xPos, yPos, waitingIconSize, waitingIconSize),
                    label: "",
                    hoverText: "",
                    texture: Game1.mouseCursors,
                    sourceRect: new Rectangle(434, 475, 9, 9),
                    scale: Game1.pixelZoom * 0.5f,
                    drawShadow: false);

                var offlineIcon = new ClickableTextureComponent(
                    name: "",
                    bounds: new Rectangle(xPos + HeadshotIconSize - offlineIconSize, yPos + 8, offlineIconSize, offlineIconSize),
                    label: "",
                    hoverText: "",
                    texture: Game1.mouseCursors,
                    sourceRect: new Rectangle(322, 498, 11, 11),
                    scale: Game1.pixelZoom * 0.5f,
                    drawShadow: false);


                var mugshotPosition = new Rectangle(xPos, yPos, HeadshotIconSize - 8, HeadshotIconSize - 16);

                this.allClickableComponents.Add(waitingIcon);

                Icons.Add(new PlayerIcon
                {
                    PlayerId = player.UniqueMultiplayerID,
                    WaitingIcon = waitingIcon,
                    HeadshotPosition = mugshotPosition,
                    OfflineIcon = offlineIcon
                });
            }
            
        }

        public override void draw(SpriteBatch b)
        {
            if (Game1.eventUp || !Context.IsWorldReady) return; /* Don't draw during festivals or events */

            DrawPlayerIcons();

            base.draw(b);
        }

        private void DrawPlayerIcons()
        {
            foreach (var icon in Icons)
            {
                var player = PlayerHelpers.GetPlayerWithUniqueId(icon.PlayerId);

                player.FarmerRenderer.drawMiniPortrat(Game1.spriteBatch, new Vector2(icon.HeadshotPosition.X, icon.HeadshotPosition.Y), 0.5f, 0.75f * Game1.pixelZoom, 1, player);

                var miniPortraitBounds = new Rectangle(Convert.ToInt32(icon.HeadshotPosition.X) + 8, Convert.ToInt32(icon.HeadshotPosition.Y) + 8, icon.HeadshotPosition.Width, icon.HeadshotPosition.Height);

                if (_readyCheckHandler.IsPlayerWaiting(icon.PlayerId))
                {
                    DrawWaitingIcon(icon);
                }

                if (PlayerHelpers.IsPlayerOffline(icon.PlayerId))
                {
                    DrawOfflineIcon(icon);
                }


                if (miniPortraitBounds.Contains(Game1.getMouseX(), Game1.getMouseY()))
                {
                    DrawHoverTextForPlayer(player);
                    UpdateMouseTypeToCursor();
                }

            }
        }

        private void DrawOfflineIcon(PlayerIcon icon)
        {
            icon.OfflineIcon.draw(Game1.spriteBatch);
        }

        private void UpdateMouseTypeToCursor()
        {
            Game1.mouseCursor = 5;
        }

        private void DrawHoverTextForPlayer(Farmer player)
        {
            var text = player.Name;

            if (PlayerHelpers.IsPlayerOffline(player.UniqueMultiplayerID))
            {
                text += " (Offline)";
            }
            else if (_readyCheckHandler.IsPlayerWaiting(player.UniqueMultiplayerID))
                text += " (Awaiting Players)";
            IClickableMenu.drawHoverText(Game1.spriteBatch, text, Game1.dialogueFont);
        }

        private void DrawWaitingIcon(PlayerIcon icon)
        {
            icon.WaitingIcon.draw(Game1.spriteBatch);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            foreach (var icon in Icons)
            {
                if (icon.HeadshotPosition.Contains(x, y))
                {
                    OnPlayerIconClicked(icon.PlayerId);
                }
            }

            base.receiveLeftClick(x, y, playSound);
        }

        private void OnPlayerIconClicked(long playerId)
        {
            EventHandler<PlayerIconClickedArgs> handler = PlayerIconClicked;

            handler?.Invoke(this, new PlayerIconClickedArgs{ PlayerId = playerId });
        }

        public event EventHandler<PlayerIconClickedArgs> PlayerIconClicked;
    }

    public class PlayerIconClickedArgs
    {
        public long PlayerId { get; set; }
    }
}
