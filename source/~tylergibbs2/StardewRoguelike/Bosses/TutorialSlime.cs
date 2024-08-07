/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;

namespace StardewRoguelike.Bosses
{
    public class TutorialSlime : BigSlime, IBossMonster
    {
        public string DisplayName => "Goobins the Gelatinous Sludge";

        public string MapPath => "boss-slime";

        public string TextureName => "Characters\\Monsters\\Big Slime";

        public Vector2 SpawnLocation => new(24, 24);

        public List<string> MusicTracks => new() { "jelly_junktion" };

        public bool InitializeWithHealthbar => true;

        public float Difficulty { get; set; }

        private readonly double SplitChance = 0.225;

        private readonly int Width;
        private readonly int Height;

        public TutorialSlime() { }

        public TutorialSlime(float difficulty) : base(Vector2.Zero, 40)
        {
            if (Roguelike.HardMode)
                SplitChance += 0.10;

            setTileLocation(SpawnLocation);
            Difficulty = difficulty;

            Width = 32;
            Height = 32;
            Sprite.SpriteWidth = Width;
            Sprite.SpriteHeight = Height;
            Sprite.LoadTexture(TextureName);
            Scale = 3f;

            moveTowardPlayerThreshold.Value = 20;
        }

        public override void MovePosition(GameTime time, xTile.Dimensions.Rectangle viewport, GameLocation currentLocation)
        {
            base.MovePosition(time, viewport, currentLocation);
            if (Health < MaxHealth / 2)
                base.MovePosition(time, viewport, currentLocation);
        }

        public override void reloadSprite()
        {
            Sprite = new(TextureName)
            {
                SpriteWidth = Width,
                SpriteHeight = Height
            };
            Sprite.LoadTexture(TextureName);
            HideShadow = true;
        }

        public override Rectangle GetBoundingBox()
        {
            int boxWidth = (int)(Sprite.SpriteWidth * 4 * 5 / 6 * Scale);
            int boxHeight = (int)(Sprite.SpriteHeight * Scale * 1.5);
            if (scale < 0.85)
                return new Rectangle((int)Position.X + boxWidth / 4, (int)Position.Y + boxHeight / 4, boxWidth, boxHeight);
            else if (scale < 1f)
                return new Rectangle((int)Position.X, (int)Position.Y + boxHeight / 6, boxWidth, boxHeight);
            else if (scale < 1.5f)
                return new Rectangle((int)Position.X - boxWidth / 5, (int)Position.Y + boxHeight / 6, boxWidth, boxHeight);
            else if (scale < 2f)
                return new Rectangle((int)Position.X - boxWidth / 4, (int)Position.Y + boxHeight / 6, boxWidth, boxHeight);

            return new Rectangle((int)Position.X - boxWidth / 3, (int)Position.Y + boxHeight / 6, boxWidth, boxHeight);
        }

        public override void shedChunks(int number, float scale)
        {
            Game1.createRadialDebris(currentLocation, Sprite.textureName.Value, new Rectangle(0, Height * 4, Width, Height), Width / 2, GetBoundingBox().Center.X, GetBoundingBox().Center.Y, number, (int)getTileLocation().Y, Color.White, 4f);
        }

        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {
            int result = base.takeDamage(damage, 0, 0, isBomb, addedPrecision, who);
            if (Health <= 0)
                BossManager.Death(who.currentLocation, who, DisplayName, SpawnLocation);
            else
            {
                if (Game1.random.NextDouble() < SplitChance && (Scale > 0.8f || Roguelike.HardMode))
                {
                    shedChunks(4, 1f);
                    Game1.playSound("slime");
                    Monster splitSlime = new GreenSlime(Position)
                    {
                        focusedOnFarmers = true
                    };
                    Roguelike.AdjustMonster((MineShaft)currentLocation, ref splitSlime);
                    who.currentLocation.characters.Add(splitSlime);
                    Scale = Math.Max(0.8f, Scale - 0.15f);
                }
            }

            return result;
        }
    }
}
