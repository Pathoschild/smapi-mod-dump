/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/scayze/multiprairie
**
*************************************************/

using Microsoft.Xna.Framework;
using MultiPlayerPrairie;
using MultiplayerPrairieKing.Entities;
using MultiplayerPrairieKing.Utility;
using StardewValley;
using System;

namespace MultiplayerPrairieKing.Components
{
    public class Map
    {
        readonly GameMultiplayerPrairieKing gameInstance;

        private readonly MAP_TILE[,] _map = new MAP_TILE[16, 16];

        public Map(GameMultiplayerPrairieKing gameInstance)
        {
            this.gameInstance = gameInstance;
        }

        public MAP_TILE this[int x, int y]
        {
            get { return _map[x, y]; }
            set { _map[x, y] = value; }
        }

        static bool IsMapTilePassable(MAP_TILE tileType)
        {
            if ((uint)tileType <= 1u || (uint)(tileType - 5) <= 4u)
            {
                return false;
            }
            return true;
        }

        static bool IsMapTilePassableForMonsters(MAP_TILE tileType)
        {
            if (tileType == MAP_TILE.CACTUS || (uint)(tileType - 7) <= 2u)
            {
                return false;
            }
            return true;
        }

        public bool IsCollidingWithMonster(Rectangle r, Enemy subject)
        {
            foreach (Enemy c in gameInstance.monsters)
            {
                if ((subject == null || !subject.Equals(c)) && Math.Abs(c.position.X - r.X) < 48 && Math.Abs(c.position.Y - r.Y) < 48 && r.Intersects(new Rectangle(c.position.X, c.position.Y, 48, 48)))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsCollidingWithMapForMonsters(Rectangle positionToCheck)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 p = StardewValley.Utility.getCornersOfThisRectangle(ref positionToCheck, i);
                if (p.X < 0f || p.Y < 0f || p.X >= 768f || p.Y >= 768f || !IsMapTilePassableForMonsters(_map[(int)p.X / 16 / 3, (int)p.Y / 16 / 3]))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsCollidingWithMap(Rectangle positionToCheck)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 p = StardewValley.Utility.getCornersOfThisRectangle(ref positionToCheck, i);
                if (p.X < 0f || p.Y < 0f || p.X >= 768f || p.Y >= 768f || !IsMapTilePassable(_map[(int)p.X / 16 / 3, (int)p.Y / 16 / 3]))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsCollidingWithMap(Point position)
        {
            Rectangle positionToCheck = new(position.X, position.Y, 48, 48);
            for (int i = 0; i < 4; i++)
            {
                Vector2 p = StardewValley.Utility.getCornersOfThisRectangle(ref positionToCheck, i);
                if (p.X < 0f || p.Y < 0f || p.X >= 768f || p.Y >= 768f || !IsMapTilePassable(_map[(int)p.X / 16 / 3, (int)p.Y / 16 / 3]))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsCollidingWithMap(Vector2 position)
        {
            Rectangle positionToCheck = new((int)position.X, (int)position.Y, 48, 48);
            for (int i = 0; i < 4; i++)
            {
                Vector2 p = StardewValley.Utility.getCornersOfThisRectangle(ref positionToCheck, i);
                if (p.X < 0f || p.Y < 0f || p.X >= 768f || p.Y >= 768f || !IsMapTilePassable(_map[(int)p.X / 16 / 3, (int)p.Y / 16 / 3]))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
