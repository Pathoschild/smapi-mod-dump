/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using Pos = MPInfo.Position;

namespace MPInfo
{
    public class PlayerInfoBox : IClickableMenu
    {
        public Farmer Who { get; }

        public static Texture2D? Crown { get; set; }

        private Texture2D Texture => Game1.mouseCursors;
        private Rectangle SourceRectIconBackground => new(293, 360, 24, 24);
        private Rectangle SourceRectIconBackgroundSelf => new(163, 399, 24, 24);

        private Rectangle[] SourceRectInfoDisplay => new[]
        {
            new Rectangle(317, 361, 3, 22), //Left
            new Rectangle(320, 361, 2, 22), //Middle (Expands based on given info)
            new Rectangle(322, 361, 7, 22)  //Right
        };

        private Rectangle[] SourceRectIconPassOut => new[]
        {
            new Rectangle(195, 408, 4, 8), //Vertical
            new Rectangle(193, 410, 8, 4), //Horizontal
            new Rectangle(194, 409, 1, 1)  //Corner
        };

        private Rectangle SourceRectIconEnergy => new(0, 428, 10, 10);
        private Rectangle SourceRectIconHealth => new(0, 438, 10, 10);
        private Rectangle SourceRectIconSkull => new(140, 428, 10, 10);
        private Rectangle SourceRectIconCrown => new(0, 0, 9, 7);

        private Config Config => ModEntry.Config;
        private string hoverText = "";

        private string cachedData;
        private PlayerInfo? info;

        public PlayerInfoBox(Farmer who)
        {
            Who = who;
            width = 96 + 12 + 112 + 28;
            height = 96;
            if (who.modData.TryGetValue(ModEntry.Instance.ModManifest.UniqueID, out cachedData))
                info = PlayerInfo.Deserialize(cachedData);
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            if (new Rectangle(xPositionOnScreen + Config.XOffset, yPositionOnScreen + Config.YOffset, 96, 96).Contains(x, y))
                hoverText = $"{Who.Name}{(Game1.player.UniqueMultiplayerID == Who.UniqueMultiplayerID ? $" {ModEntry.Instance.Helper.Translation.Get("PlayerInfoBox.Me")}" : (Game1.serverHost.Value.UniqueMultiplayerID == Who.UniqueMultiplayerID ? $" {ModEntry.Instance.Helper.Translation.Get("PlayerInfoBox.Host")}" : ""))}";
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            if (new Rectangle(xPositionOnScreen, yPositionOnScreen, 96, 96).Contains(x, y))
                ModEntry.Instance.ForceUpdate();
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            if (oldBounds != newBounds)
                RedrawAll();
        }

        public override void update(GameTime time)
        {
            base.update(time);
            if (Who.modData.TryGetValue(ModEntry.Instance.ModManifest.UniqueID, out string data) && data != cachedData)
            {
                cachedData = data;
                info = PlayerInfo.Deserialize(cachedData);
            }
        }

        public bool Visible() => ModEntry.Instance.IsEnabled && (Config.ShowSelf || Who.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID);

        public static void RedrawAll()
        {
            var index = 0;
            foreach (var pib in Game1.onScreenMenus.Where(x => (x as PlayerInfoBox)?.Visible() ?? false).OfType<PlayerInfoBox>())
            {
                var pos = pib.GetPosition(index);
                pib.xPositionOnScreen = (int)pos.X;
                pib.yPositionOnScreen = (int)pos.Y;
                index++;
            }
        }

        private Vector2 GetPosition(int index)
        {
            int x = 32, y = 0;
            switch (Config.Position)
            {
                case Pos.BottomLeft:
                    y = (Game1.graphics.GraphicsDevice.Viewport.Height - 32 - 96) - (Config.SpaceBetween * index);
                    break;
                case Pos.TopLeft:
                    y = 32 + (Config.SpaceBetween * index);
                    break;
                case Pos.BottomRight:
                    y = (Game1.graphics.GraphicsDevice.Viewport.Height - 32 - 96) - (Config.SpaceBetween * index);
                    x = Game1.graphics.GraphicsDevice.Viewport.Width - 112 - 16;
                    break;
                case Pos.CenterRight:
                    y = ((Game1.graphics.GraphicsDevice.Viewport.Height / 2) + 32 - 96 + 8) + (Config.SpaceBetween * index);
                    x = Game1.graphics.GraphicsDevice.Viewport.Width - 112 - 16;
                    break;
            }
            return new(x + Config.XOffset, y + Config.YOffset);
        }

        public override void draw(SpriteBatch b)
        {
            if (!Visible())
                return;

            base.draw(b);
            //float info.Stamina = netStamina.Value;

            //I'm going to clean this up, I promise \\Never mind, maybe some other time
            switch (Config.Position)
            {
                case Pos.TopLeft:
                case Pos.BottomLeft:
                    b.Draw(Texture, new(xPositionOnScreen, yPositionOnScreen), Who == Game1.player ? SourceRectIconBackgroundSelf : SourceRectIconBackground, Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
                    b.Draw(Texture, new(xPositionOnScreen + 96, yPositionOnScreen + 4), SourceRectInfoDisplay[0], Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
                    b.Draw(Texture, new(xPositionOnScreen + 96 + 12, yPositionOnScreen + 4), SourceRectInfoDisplay[1], Color.White, 0.0f, Vector2.Zero, new Vector2(56f, 4f), SpriteEffects.None, 0.88f);
                    b.Draw(Texture, new(xPositionOnScreen + 96 + 12 + 112, yPositionOnScreen + 4), SourceRectInfoDisplay[2], Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);

                    FarmerRenderer.isDrawingForUI = true;
                    Who.FarmerRenderer.drawMiniPortrat(b, new(xPositionOnScreen + 16, yPositionOnScreen + 15), 0.89f, 4f, 0, Who);
                    if (info is not null && info.Health <= 0) //Icon to display being knocked out
                    {
                        b.Draw(Game1.fadeToBlackRect, new(xPositionOnScreen + 12, yPositionOnScreen + 12, 72, 72), new Rectangle(0, 0, 1, 1), Color.Black * 0.6f, 0.0f, Vector2.Zero, SpriteEffects.None, 0.9f);
                        b.Draw(Texture, new(xPositionOnScreen + 28, yPositionOnScreen + 36), SourceRectIconSkull, Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
                    }
                    if (Who.passedOut || Who.FarmerSprite.isPassingOut()) //Icon to display passing out
                    {
                        b.Draw(Game1.fadeToBlackRect, new(xPositionOnScreen + 12, yPositionOnScreen + 12, 72, 72), new Rectangle(0, 0, 1, 1), Color.Black * 0.6f, 0.0f, Vector2.Zero, SpriteEffects.None, 0.9f);
                        b.Draw(Texture, new(xPositionOnScreen + 40, yPositionOnScreen + 39), SourceRectIconPassOut[0], Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
                        b.Draw(Texture, new(xPositionOnScreen + 32, yPositionOnScreen + 47), SourceRectIconPassOut[1], Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
                        for (int i = 0; i < 2; i++) //Yes the corners are necessary
                            for (int j = 0; j < 2; j++)
                                b.Draw(Texture, new(xPositionOnScreen + 36 + (20 * j), yPositionOnScreen + 43 + (20 * i)), SourceRectIconPassOut[2], Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
                    }
                    if (Config.ShowHostCrown && Crown is not null && Game1.MasterPlayer.UniqueMultiplayerID == Who.UniqueMultiplayerID)
                        b.Draw(Crown, new(xPositionOnScreen - 16, yPositionOnScreen + 16), SourceRectIconCrown, Color.White, -.8f, Vector2.Zero, 4f, SpriteEffects.None, 7.91f);
                    FarmerRenderer.isDrawingForUI = false;

                    b.Draw(Texture, new(xPositionOnScreen + 96 + 4, yPositionOnScreen + 4 + 26), SourceRectIconHealth, Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, 0.89f);
                    if (info is not null)
                        b.DrawString(Game1.smallFont, $"{info.Health}/{info.MaxHealth}", new(xPositionOnScreen + 96 + 8 + 24, yPositionOnScreen + 4 + 20), (info.Health <= (float)(info.MaxHealth / 10)) ? Color.Red : (info.Health <= (float)(info.MaxHealth / 5) ? Color.Yellow : Game1.textColor), 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.89f);
                    else
                        b.DrawString(Game1.smallFont, $"???/???", new(xPositionOnScreen + 96 + 8 + 24, yPositionOnScreen + 4 + 20), Game1.textColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.89f);
                    
                    b.Draw(Texture, new(xPositionOnScreen + 96 + 4, yPositionOnScreen + 4 + 50), SourceRectIconEnergy, Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, 0.89f);
                    if (info is not null)
                        b.DrawString(Game1.smallFont, $"{Math.Round(info.Stamina)}/{info.MaxStamina}", new(xPositionOnScreen + 96 + 8 + 24, yPositionOnScreen + 4 + 44), (info.Stamina <= info.MaxStamina / 10) ? Color.Red : (info.Stamina <= info.MaxStamina / 5 ? Color.Yellow : Game1.textColor), 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.89f);
                    else
                        b.DrawString(Game1.smallFont, $"???/{Who.MaxStamina}", new(xPositionOnScreen + 96 + 8 + 24, yPositionOnScreen + 4 + 44), Game1.textColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.89f);
                    break;
                case Pos.CenterRight:
                case Pos.BottomRight:
                    b.Draw(Texture, new(xPositionOnScreen + 12, yPositionOnScreen), Who == Game1.player ? SourceRectIconBackgroundSelf : SourceRectIconBackground, Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
                    b.Draw(Texture, new(xPositionOnScreen, yPositionOnScreen + 4), SourceRectInfoDisplay[0], Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
                    b.Draw(Texture, new(xPositionOnScreen - 112 + 4, yPositionOnScreen + 4), SourceRectInfoDisplay[1], Color.White, 0.0f, Vector2.Zero, new Vector2(56f, 4f), SpriteEffects.None, 0.88f);
                    b.Draw(Texture, new(xPositionOnScreen - 112 - 20, yPositionOnScreen + 4), SourceRectInfoDisplay[2], Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, 0.88f);

                    FarmerRenderer.isDrawingForUI = true;
                    Who.FarmerRenderer.drawMiniPortrat(b, new(xPositionOnScreen + 28, yPositionOnScreen + 15), 0.89f, 4f, 0, Who);
                    if (info is not null && info.Health <= 0) //Icon to display being knocked out
                    {
                        b.Draw(Game1.fadeToBlackRect, new(xPositionOnScreen + 24, yPositionOnScreen + 12, 72, 72), new Rectangle(0, 0, 1, 1), Color.Black * 0.6f, 0.0f, Vector2.Zero, SpriteEffects.None, 0.9f);
                        b.Draw(Texture, new(xPositionOnScreen + 44, yPositionOnScreen + 39), SourceRectIconSkull, Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
                    }
                    if (Who.passedOut || Who.FarmerSprite.isPassingOut()) //Icon to display passing out
                    {
                        b.Draw(Game1.fadeToBlackRect, new(xPositionOnScreen + 24, yPositionOnScreen + 12, 72, 72), new Rectangle(0, 0, 1, 1), Color.Black * 0.6f, 0.0f, Vector2.Zero, SpriteEffects.None, 0.9f);
                        b.Draw(Texture, new(xPositionOnScreen + 52, yPositionOnScreen + 39), SourceRectIconPassOut[0], Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
                        b.Draw(Texture, new(xPositionOnScreen + 44, yPositionOnScreen + 47), SourceRectIconPassOut[1], Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
                        for (int i = 0; i < 2; i++) //Yes the corners are necessary
                            for (int j = 0; j < 2; j++)
                                b.Draw(Texture, new(xPositionOnScreen + 48 + (20 * j), yPositionOnScreen + 43 + (20 * i)), SourceRectIconPassOut[2], Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
                    }
                    if (Config.ShowHostCrown && Crown is not null && Game1.MasterPlayer.UniqueMultiplayerID == Who.UniqueMultiplayerID)
                        b.Draw(Crown, new(xPositionOnScreen + 96, yPositionOnScreen - 14), SourceRectIconCrown, Color.White, .7f, Vector2.Zero, 4f, SpriteEffects.None, 7.91f);
                    FarmerRenderer.isDrawingForUI = false;

                    b.Draw(Texture, new(xPositionOnScreen - 96 - 10, yPositionOnScreen + 4 + 26), SourceRectIconHealth, Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, 0.89f);
                    if (info is not null)
                        b.DrawString(Game1.smallFont, $"{info.Health}/{info.MaxHealth}", new(xPositionOnScreen - 96 - 8 + 20, yPositionOnScreen + 4 + 20), (info.Health <= (float)(info.MaxHealth / 10)) ? Color.Red : (info.Health <= (float)(info.MaxHealth / 5) ? Color.DarkOrange : Game1.textColor), 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.89f);
                    else
                        b.DrawString(Game1.smallFont, $"???/???", new(xPositionOnScreen - 96 - 8 + 20, yPositionOnScreen + 4 + 20), Game1.textColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.89f);
                    
                    b.Draw(Texture, new(xPositionOnScreen - 96 - 10, yPositionOnScreen + 4 + 50), SourceRectIconEnergy, Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, 0.89f);
                    if (info is not null)
                        b.DrawString(Game1.smallFont, $"{Math.Round(info.Stamina)}/{info.MaxStamina}", new(xPositionOnScreen - 96 - 8 + 20, yPositionOnScreen + 4 + 44), (info.Stamina <= info.MaxStamina / 10) ? Color.Red : (info.Stamina <= info.MaxStamina / 5 ? Color.DarkOrange : Game1.textColor), 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.89f);
                    else
                        b.DrawString(Game1.smallFont, $"???/{Who.MaxStamina}", new(xPositionOnScreen - 96 - 8 + 20, yPositionOnScreen + 4 + 44), Game1.textColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.89f);
                    break;
            }

            if (!string.IsNullOrWhiteSpace(hoverText))
            {
                drawHoverText(b, hoverText, Game1.smallFont);
                hoverText = "";
            }
        }
    }
}
