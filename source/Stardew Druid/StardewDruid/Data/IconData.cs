/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewDruid.Cast;
using StardewDruid.Dialogue;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewDruid.Data
{
    public class IconData
    {

        public enum cursors
        {
            none,

            weald,
            mists,
            stars,
            fates,
            comet,
            shield,

            wealdCharge,
            mistsCharge,
            starsCharge,
            fatesCharge,
            chaosCharge,
            shadeCharge,

            redTarget,
            blueTarget,
            death,
            redArrow,
            blueArrow,

        }

        public Texture2D cursorTexture;

        public int cursorColumns;


        public enum displays
        {
            none,

            weald,
            mists,
            stars,
            fates,
            ether,
            chaos,

            effigy,
            jester,
            shadowtin,
            blank1,
            blank2,
            blank3,

            active,
            reverse,
            forward,
            end,
            exit,
            blank6,

            quest,
            effect,

        }

        public Texture2D displayTexture;

        public int displayColumns;


        public enum decorations
        {
            none,

            weald,
            mists,
            stars,
            fates,
            ether,
            fade,

        }

        public Dictionary<Rite.rites, decorations> riteDecorations = new()
        {

            [Rite.rites.weald] = decorations.weald,

            [Rite.rites.mists] = decorations.mists,

            [Rite.rites.stars] = decorations.stars,

            [Rite.rites.fates] = decorations.fates,

            [Rite.rites.ether] = decorations.ether,

        };

        public Texture2D decorationTexture;

        public int decorationColumns;


        public IconData()
        {

            cursorTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Cursors.png"));

            cursorColumns = 6;

            displayTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Displays.png"));

            displayColumns = 6;

            decorationTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Decorations.png"));

            decorationColumns = 4;

        }

        public Microsoft.Xna.Framework.Rectangle CursorRect(cursors id)
        {

            if (id == cursors.none) { return new(); }

            int slot = Convert.ToInt32(id) - 1;

            return new(slot % cursorColumns * 32, slot / cursorColumns * 32, 32, 32);


        }

        public TemporaryAnimatedSprite CursorIndicator(GameLocation location, Vector2 origin, cursors cursorId, float interval = 1200f, float scale = 3f, int loops = 1, int rotate = 60)
        {

            Vector2 originOffset = origin + new Vector2(32, 32) - new Vector2(16 * scale, 16 * scale);

            Microsoft.Xna.Framework.Rectangle cursorRect = Mod.instance.iconData.CursorRect(cursorId);

            float layer = origin.Y / 640000 + 0.0001f;

            TemporaryAnimatedSprite animation = new(0, interval, 1, loops, originOffset, false, false)
            {

                sourceRect = cursorRect,

                sourceRectStartingPos = new Vector2(cursorRect.X, cursorRect.Y),

                texture = cursorTexture,

                scale = scale,

                layerDepth = layer,

                timeBasedMotion = true,

                Parent = location,

                alpha = 0.65f,

            };

            if (rotate > 0)
            {

                animation.rotationChange = (float)(Math.PI / rotate);

            }

            location.temporarySprites.Add(animation);

            return animation;

        }

        public Microsoft.Xna.Framework.Rectangle DisplayRect(displays id)
        {

            if (id == displays.none) { return new(); }

            int slot = Convert.ToInt32(id) - 1;

            return new(slot % displayColumns * 16, slot / displayColumns * 16, 16, 16);

        }

        public Microsoft.Xna.Framework.Color schemeColour(Rite.rites scheme)
        {
            switch (scheme)
            {
                case Rite.rites.none:
                case Rite.rites.weald:

                    return Microsoft.Xna.Framework.Color.LightGreen;

            }

            return Microsoft.Xna.Framework.Color.White;

        }

        public Microsoft.Xna.Framework.Rectangle DecorativeRect(decorations id)
        {

            if (id == decorations.none) { return new(); }

            int slot = Convert.ToInt32(id) - 1;

            return new(slot % decorationColumns * 64, slot / decorationColumns * 64, 64, 64);

        }

        public TemporaryAnimatedSprite DecorativeIndicator(GameLocation location, Vector2 origin, decorations decorationId, float size = 1f, float interval = 1000f, float depth = 0.0001f)
        {

            Vector2 originOffset = origin + new Vector2(32, 32) - (new Vector2(32, 32) * (3f * size));

            Microsoft.Xna.Framework.Rectangle rect = DecorativeRect(decorationId);

            TemporaryAnimatedSprite animation = new(0, interval, 1, 1, originOffset, false, false)
            {

                sourceRect = rect,

                sourceRectStartingPos = new Vector2(rect.X, rect.Y),

                texture = decorationTexture,

                scale = 3f * size,

                timeBasedMotion = true,

                layerDepth = depth,

                rotationChange = (float)(Math.PI / 120),

                Parent = location,

                alpha = 0.65f,

            };

            location.temporarySprites.Add(animation);

            return animation;

        }

    }

}
