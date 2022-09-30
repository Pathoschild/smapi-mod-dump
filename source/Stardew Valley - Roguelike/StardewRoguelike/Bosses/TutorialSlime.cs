/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
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

        public string MapPath
        {
            get { return "boss-slime"; }
        }

        public string TextureName
        {
            get { return "Characters\\Monsters\\Big Slime"; }
        }

        public Vector2 SpawnLocation
        {
            get { return new(24, 24); }
        }

        public List<string> MusicTracks
        {
            get { return new() { "junimoKart_slimeMusic" }; }
        }

        public bool InitializeWithHealthbar
        {
            get { return true; }
        }

        private float _difficulty;

        public float Difficulty
        {
            get { return _difficulty; }
            set { _difficulty = value; }
        }

        private double splitChance = 0.25;

        private readonly int width;
        private readonly int height;

        public TutorialSlime() { }

        public TutorialSlime(float difficulty) : base(Vector2.Zero, 40)
        {
            if (Roguelike.HardMode)
                splitChance += 0.10;

            setTileLocation(SpawnLocation);
            Difficulty = difficulty;

            width = 32;
            height = 32;
            Sprite.SpriteWidth = width;
            Sprite.SpriteHeight = height;
            Sprite.LoadTexture(TextureName);
            Scale = 3f;

            Health = (int)Math.Round(475 * difficulty);
            MaxHealth = Health;
            DamageToFarmer = (int)Math.Round(6 * difficulty);
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
                SpriteWidth = width,
                SpriteHeight = height
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
            Game1.createRadialDebris(currentLocation, Sprite.textureName.Value, new Rectangle(0, height * 4, width, height), width / 2, GetBoundingBox().Center.X, GetBoundingBox().Center.Y, number, (int)getTileLocation().Y, Color.White, 4f);
        }

        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {
            int result = base.takeDamage(damage, 0, 0, isBomb, addedPrecision, who);
            if (Health <= 0)
                BossManager.Death(who.currentLocation, who, DisplayName, SpawnLocation);
            else
            {
                if (Game1.random.NextDouble() < splitChance && (Scale > 0.8f || Roguelike.HardMode))
                {
                    shedChunks(4, 1f);
                    Game1.playSound("slime");
                    Monster splitSlime = new GreenSlime(Position)
                    {
                        focusedOnFarmers = true
                    };
                    Roguelike.AdjustMonster(currentLocation as MineShaft, ref splitSlime);
                    who.currentLocation.characters.Add(splitSlime);
                    Scale = Math.Max(0.8f, Scale - 0.15f);
                }
            }

            return result;
        }
    }
}