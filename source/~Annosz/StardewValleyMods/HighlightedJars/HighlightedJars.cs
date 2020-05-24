using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HighlightedJars
{
    public class HighlightedJars : Mod
    {
        private ModConfig Config;

        private const int exclamationEmote = 16;

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.SaveLoaded += (s, e) => GameLoop_SaveLoaded(s, e, helper);
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e, IModHelper helper)
        {
            helper.Events.Display.RenderedWorld += Display_RenderedWorld; ;
        }

        private void Display_RenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            var highlightableObjects = Game1.currentLocation.objects.Values.Where(o =>
                   (Config.HighlightJars && (o as StardewValley.Object).parentSheetIndex == 15)
                   || (Config.HighlightKegs && (o as StardewValley.Object).parentSheetIndex == 12)
                   ).ToList();

            foreach (var highlightableObject in highlightableObjects)
            {
                Vector2 vector2 = (highlightableObject.isOn ? new Vector2(Math.Abs(highlightableObject.TileLocation.X - 5f), Math.Abs(highlightableObject.TileLocation.Y - 5f)) : Vector2.Zero) * 4f;
                Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(highlightableObject.TileLocation.X * 64), (float)(Math.Abs(highlightableObject.TileLocation.Y * 64 - 64))));
                Rectangle destinationRectangle = new Rectangle((int)((double)local.X + 32.0 - 8.0 - (double)vector2.X / 2.0), (int)((double)local.Y + 64.0 + 8.0 - (double)vector2.Y / 2.0), (int)(16.0 + (double)vector2.X), (int)(16.0 + (double)vector2.Y / 2.0));
                e.SpriteBatch.Draw(Game1.emoteSpriteSheet, destinationRectangle, new Rectangle(exclamationEmote * 16 % Game1.emoteSpriteSheet.Width, exclamationEmote * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), Color.White * 0.95f, 0.0f, Vector2.Zero, SpriteEffects.None, (float)((highlightableObject.TileLocation.Y + 1) * 64) / 10000f);
            }
        }
    }
}