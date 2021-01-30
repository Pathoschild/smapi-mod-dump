/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Wellbott/StardewValleyMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buildings;

namespace FishWellSpace
{
    public class WellFishSilhouette : PondFishSilhouette
    {
        public float border_width = 0.8f;
        public new FishWell _pond;

        /// <summary>
        /// Adjusted initial shadow placement
        /// </summary>
        /// <param name="pond"></param>
        public WellFishSilhouette(FishWell pond, float bWidth)
            : base(pond)
        {
            _pond = pond;
            _fishObject = _pond.GetFishObject();
            border_width = bWidth;
            if (_fishObject.HasContextTag("fish_upright"))
            {
                _upRight = true;
            }
            position = new Vector2(
                _pond.tileX.Value + Utility.Lerp(border_width + 0.2f, (float)_pond.tilesWide - border_width - 0.2f, (float)Game1.random.NextDouble()),
                _pond.tileY.Value + Utility.Lerp(border_width + 0.2f, (float)_pond.tilesHigh - border_width - 0.2f, (float)Game1.random.NextDouble())
                ) * 64f;
            _age = 0f;
            _randomOffset = Utility.Lerp(0f, 500f, (float)Game1.random.NextDouble());
            ResetDartTime();
        }

        /// <summary>
        /// Normalzing the layer of the shadows so they go just under the main structure
        /// </summary>
        /// <param name="b"></param>
        public new void Draw(SpriteBatch b)
        {
            float angle = (float)Math.PI / 4f;
            if (_upRight)
            {
                angle = 0f;
            }
            SpriteEffects effect = SpriteEffects.None;
            angle += (float)Math.Sin(_wiggleTimer + _randomOffset) * 2f * (float)Math.PI / 180f;
            if (_velocity.Y < 0f)
            {
                angle -= (float)Math.PI / 18f;
            }
            if (_velocity.Y > 0f)
            {
                angle += (float)Math.PI / 18f;
            }
            if (_flipped)
            {
                effect = SpriteEffects.FlipHorizontally;
                angle *= -1f;
            }
            float draw_scale = Utility.Lerp(0.75f, 0.65f, Utility.Clamp(_sinkAmount, 0f, 1f));
            draw_scale *= Utility.Lerp(1f, 0.75f, (float)(int)_pond.currentOccupants / 10f);
            Vector2 draw_position = position;
            draw_position.Y += (float)Math.Sin(_age * 2f + _randomOffset) * 5f;
            draw_position.Y += (int)(_sinkAmount * 4f);
            float transparency = Utility.Lerp(0.25f, 0.15f, Utility.Clamp(_sinkAmount, 0f, 1f));
            b.Draw(origin: new Vector2(8f, 8f), texture: Game1.objectSpriteSheet, position: Game1.GlobalToLocal(Game1.viewport, draw_position), sourceRectangle: Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, _fishObject.ParentSheetIndex, 16, 16), color: Color.Black * transparency, rotation: angle, scale: 4f * draw_scale, effects: effect, layerDepth: (((float)(int)_pond.tileY.Value + 0.5f) * 64f - 0.5f) / 10000f);
        }

        /// <summary>
        /// Modifications to shadow movement, primarily allowed bounds
        /// </summary>
        /// <param name="time"></param>
        public new void Update(float time)
        {
            nextDart -= time;
            _age += time;
            _wiggleTimer += time;
            if (nextDart <= 0f || (nextDart <= 0.5f && Game1.random.NextDouble() < 0.10000000149011612))
            {
                ResetDartTime();
                int direction = Game1.random.Next(0, 2) * 2 - 1;
                if (direction < 0)
                {
                    _flipped = true;
                }
                else
                {
                    _flipped = false;
                }
                //increased Y variance a bit
                _velocity = new Vector2((float)direction * Utility.Lerp(50f, 100f, (float)Game1.random.NextDouble()), Utility.Lerp(-75f, 75f, (float)Game1.random.NextDouble()));
            }
            bool moving = false;
            if (_velocity.LengthSquared() > 0f)
            {
                moving = true;
                _wiggleTimer += time * 30f;
                _sinkAmount = Utility.MoveTowards(_sinkAmount, 0f, 2f * time);
            }
            else
            {
                _sinkAmount = Utility.MoveTowards(_sinkAmount, 1f, 1f * time);
            }
            position += _velocity * time;
            for (int i = 0; i < _pond.GetWellSilhouettes().Count; i++)
            {
                PondFishSilhouette other_silhouette = _pond.GetWellSilhouettes()[i];
                if (other_silhouette == this)
                {
                    continue;
                }
                //slightly more pushy
                float push_amount = 40f;
                float push_other_amount = 40f;
                if (IsMoving())
                {
                    push_amount = 0f;
                }
                if (other_silhouette.IsMoving())
                {
                    push_other_amount = 0f;
                }
                if (Math.Abs(other_silhouette.position.X - position.X) < 32f)
                {
                    if (other_silhouette.position.X > position.X)
                    {
                        other_silhouette.position.X += push_other_amount * time;
                        position.X += (0f - push_amount) * time;
                    }
                    else
                    {
                        other_silhouette.position.X -= push_other_amount * time;
                        position.X += push_amount * time;
                    }
                }
                if (Math.Abs(other_silhouette.position.Y - position.Y) < 32f)
                {
                    if (other_silhouette.position.Y > position.Y)
                    {
                        other_silhouette.position.Y += push_other_amount * time;
                        position.Y += -1f * time;
                    }
                    else
                    {
                        other_silhouette.position.Y -= push_other_amount * time;
                        position.Y += 1f * time;
                    }
                }
            }
            _velocity.X = Utility.MoveTowards(_velocity.X, 0f, 35f * time);
            _velocity.Y = Utility.MoveTowards(_velocity.Y, 0f, 15f * time);
            //corrected border width
            if (position.X > ((float)((int)_pond.tileX + (int)_pond.tilesWide) - border_width) * 64f)
            {
                position.X = ((float)((int)_pond.tileX + (int)_pond.tilesWide) - border_width) * 64f -1f;
                _velocity.X *= -1f;
                if (moving && (Game1.random.NextDouble() < 0.25 || Math.Abs(_velocity.X) > 30f))
                {
                    _flipped = !_flipped;
                }
            }
            if (position.X < ((float)(int)_pond.tileX + border_width) * 64f)
            {
                position.X = ((float)(int)_pond.tileX + border_width) * 64f + 1f;
                _velocity.X *= -1f;
                if (moving && (Game1.random.NextDouble() < 0.25 || Math.Abs(_velocity.X) > 30f))
                {
                    _flipped = !_flipped;
                }
            }
            if (position.Y > ((float)((int)_pond.tileY + (int)_pond.tilesHigh) - border_width) * 64f)
            {
                position.Y = ((float)((int)_pond.tileY + (int)_pond.tilesHigh) - border_width) * 64f - 1f;
                _velocity.Y *= -1f;
            }
            if (position.Y < ((float)(int)_pond.tileY + border_width) * 64f)
            {
                position.Y = ((float)(int)_pond.tileY + border_width) * 64f + 1f;
                _velocity.Y *= -1f;
            }
        }
    }
}
