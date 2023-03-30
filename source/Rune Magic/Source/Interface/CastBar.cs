/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/facufierro/RuneMagic
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuneMagic.Source.Interface
{
    public class CastBar
    {
        private Texture2D Frame;
        private Texture2D Background;
        private Texture2D Bar;
        private Color BarColor;

        private float Scale;

        public CastBar()
        {
            Frame = RuneMagic.Textures["castbar_frame"];
            Background = RuneMagic.Textures["castbar_background"];
            Bar = RuneMagic.Textures["castbar_bar"];

            //create a new rectangle at the center of the screen
        }

        public void Render(SpriteBatch spriteBatch, ISpellCastingItem spellCastingItem)
        {
            Scale = (float)RuneMagic.Config.CastbarScale + 1;
            BarColor = new Color(new Vector4(0, 0, 200, 0.8f));
            if (spellCastingItem.Spell == null)
                return;
            var castingTime = spellCastingItem.Spell.CastingTime;
            if (Player.MagicStats.CastingTime > 0)
            {
                var x = (int)(spriteBatch.GraphicsDevice.Viewport.Width / 2 - (64 * Scale) / 2);
                var y = (int)(spriteBatch.GraphicsDevice.Viewport.Height / 6 * 5 - Scale * 64);
                var width = (int)(64 * Scale);
                var height = (int)(64 * Scale);
                spriteBatch.Draw(Background, new Rectangle(x, y, width, height), Color.White);

                width = (int)(Player.MagicStats.CastingTime / (castingTime * 60) * 64);
                spriteBatch.Draw(Bar, new Rectangle(x, y, (int)(width * Scale), height), BarColor);

                width = (int)(64 * Scale);
                spriteBatch.Draw(Frame, new Rectangle(x, y, width, height), Color.White);
            }
        }
    }
}