using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Enchantments;
using StardewValley.Tools;
using StardewValley.Monsters;
using System.Diagnostics;
using System.Xml.Serialization;
using Netcode;
using Microsoft.Xna.Framework.Graphics;

namespace CaveCultCode
{
    public class CustomWeapon : Tool
    {
        public int maxKills = 10;

        private readonly NetInt currentKills = new NetInt(10);

        public int CurrentKills
        {
            get
            {
                return currentKills;
            }
            set
            {
                currentKills.Value = value;
            }
        }

        protected override Item GetOneNew()
        {
            return new CustomWeapon();
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            base.drawInMenu(spriteBatch, location + (new Vector2(0f, -12f)), scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
            if (drawStackNumber != 0)
            {
                spriteBatch.Draw(Game1.mouseCursors, location + new Vector2(4f, 44f), new Rectangle(297, 420, 14, 5), Color.White * transparency, 0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth + 0.0001f);
                spriteBatch.Draw(Game1.staminaRect, new Rectangle((int)location.X + 8, (int)location.Y + 64 - 16, (int)((float)(int)currentKills / (float)maxKills * 48f), 8), (Color.DodgerBlue * 0.7f * transparency));
            }
        }

    }
}
