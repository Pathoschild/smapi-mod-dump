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
using System.Collections.Generic;

namespace StardewRoguelike.Bosses
{
    internal class BigCrab : RockCrab, IBossMonster
    {
        public string DisplayName => "Big Crab Boi";

        public string MapPath
        {
            get { return "boss-crab"; }
        }

        public string TextureName
        {
            get { return "Characters\\Monsters\\Rock Crab"; }
        }

        public Vector2 SpawnLocation
        {
            get { return new(26, 22); }
        }

        public List<string> MusicTracks
        {
            get { return new() { "junimoKart_whaleMusic" }; }
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

        public BigCrab() { }

        public BigCrab(float difficulty) : base(Vector2.Zero)
        {
            setTileLocation(SpawnLocation);
            Difficulty = difficulty;

            Scale = 3f;

            moveTowardPlayerThreshold.Value = 20;
        }

        public override void update(GameTime time, GameLocation location)
        {
            base.update(time, location);

            location.forceViewportPlayerFollow = false;
        }

        public override void behaviorAtGameTick(GameTime time)
        {
            base.behaviorAtGameTick(time);
        }

        public override Rectangle GetBoundingBox()
        {
            int boxWidth = (int)(Sprite.SpriteWidth * 4 * 7 / 8 * Scale);
            int boxHeight = (int)(Sprite.SpriteHeight * Scale * 1.75);
            return new Rectangle((int)Position.X - boxWidth / 3, (int)Position.Y + boxHeight / 8, boxWidth, boxHeight);
        }
    }
}
