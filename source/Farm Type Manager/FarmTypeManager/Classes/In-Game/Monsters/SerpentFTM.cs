/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Monsters;

namespace FarmTypeManager.Monsters
{
    /// <summary>A subclass of Stardew's Serpent class, adjusted for use by this mod.</summary>
    public class SerpentFTM : Serpent
    {
        /// <summary>Creates an instance of Stardew's Serpent class, but with adjustments made for this mod.</summary>
        public SerpentFTM()
            : base()
        {

        }

        /// <summary>Creates an instance of Stardew's Serpent class, but with adjustments made for this mod.</summary>
        /// <param name="position">The x,y coordinates of this monster's location.</param>
        public SerpentFTM(Vector2 position)
            : base(position)
        {

        }

        /// <summary>Creates an instance of Stardew's Serpent class, but with adjustments made for this mod.</summary>
        /// <param name="position">The x,y coordinates of this monster's location.</param>
        /// <param name="name">The "internal" name for this monster. Known subtype(s) for this class: "Royal Serpent"</param>
        public SerpentFTM(Vector2 position, string name)
            : base(position, name)
        {
            
        }

        //this override forces any instance of GameLocation to call drawAboveAllLayers, fixing a bug where flying monsters are invisible on some maps
        public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
        {
            base.drawAboveAlwaysFrontLayer(b); //call the base version of this, if one exists
            b.End();
            b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
            base.drawAboveAllLayers(b); //call the extra draw method used by flying monsters
            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
        }

        //this override prevents serpents overwriting their own sprite if it's already loaded, which allows this mod's custom "Sprite" setting to work
        public override void reloadSprite(bool onlyAppearance = false)
        {
            if (this.IsRoyalSerpent())
            {
                if (Sprite == null) //skip updating the sprite if it already exists
                    this.Sprite = new AnimatedSprite("Characters\\Monsters\\Royal Serpent");
                base.Scale = 1f;
            }
            else
            {
                if (Sprite == null) //skip updating the sprite if it already exists
                    this.Sprite = new AnimatedSprite("Characters\\Monsters\\Serpent");
                base.Scale = 0.75f;
            }
            this.Sprite.SpriteWidth = 32;
            this.Sprite.SpriteHeight = 32;
            base.HideShadow = true;
        }
    }
}
