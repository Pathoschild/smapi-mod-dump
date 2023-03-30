/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/facufierro/RuneMagic
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using xTile.Tiles;

namespace RuneMagic.Source.Effects
{
    public class ExcavationEffect : SpellEffect
    {
        //private TemporaryAnimatedSprite Sprite;
        //private Rectangle EffectArea;

        public ExcavationEffect(Spell spell) : base(spell, Duration.Medium)
        {
            Start();
        }

        public override void Start()
        {
            base.Start();
            ////Sprite = new()
            ////{
            ////    Position = new Vector2(Game1.currentCursorTile.X * Game1.tileSize, Game1.currentCursorTile.Y * Game1.tileSize),
            ////    texture = RuneMagic.Textures["excavation"],
            ////    sourceRect = new Rectangle(0, 0, 16, 16),
            ////    totalNumberOfLoops = 30,
            ////    animationLength = 1,
            ////    layerDepth = 0f,
            ////    scale = 4f,
            ////    color = Color.White,
            ////};
            ////EffectArea = new Rectangle(Sprite.Position.ToPoint(), new Point((int)(16 * Sprite.scale), (int)(16 * Sprite.scale)));
            ////RuneMagic.Instance.Monitor.Log($"Hole position {Sprite.Position}");
            ////Game1.currentLocation.temporarySprites.Add(Sprite);
        }

        public override void Update()
        {
            base.Update();
            //if the player touches the TemporaryAnimatedSprite
            //RuneMagic.Instance.Monitor.Log($"{Game1.player.GetBoundingBox().X},{Game1.player.GetBoundingBox().Y}");
            RuneMagic.Instance.Monitor.Log($"{Timer}");
            //if (EffectArea.Contains(Game1.player.GetBoundingBox()))
            //{
            //    //dig a hole at the cursor location
            //    RuneMagic.Instance.Monitor.Log($"Hole");
            //}
        }
    }
}