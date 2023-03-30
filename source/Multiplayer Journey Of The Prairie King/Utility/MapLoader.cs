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
using MultiplayerPrairieKing.Components;
using StardewValley;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MultiplayerPrairieKing.Utility
{
    public static class MapLoader
    {
		static GameMultiplayerPrairieKing gameInstance;
		public static void Init(GameMultiplayerPrairieKing instance)
		{
            gameInstance = instance;
		}

		public static Map GetDefaultMap()
		{
            Map map = new(gameInstance);
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    if ((x == 0 || x == 15 || y == 0 || y == 15) && (x <= 6 || x >= 10) && (y <= 6 || y >= 10))
                    {
                        map[x, y] = MAP_TILE.CACTUS;
                    }
                    else if (x == 0 || x == 15 || y == 0 || y == 15)
                    {
                        map[x, y] = (MAP_TILE)((Game1.random.NextDouble() < 0.15) ? 1 : 0);
                    }
                    else if (x == 1 || x == 14 || y == 1 || y == 14)
                    {
                        map[x, y] = MAP_TILE.GRAVEL;
                    }
                    else
                    {
                        map[x, y] = (MAP_TILE)((Game1.random.NextDouble() < 0.1) ? 4 : 3);
                    }
                }
            }
			return map;
        }
		public static Map GetMap(int wave)
		{
            Map map = new(gameInstance);
			for (int x = 0; x < 16; x++)
			{
				for (int y = 0; y < 16; y++)
				{
					if ((x == 0 || x == 15 || y == 0 || y == 15) && (x <= 6 || x >= 10) && (y <= 6 || y >= 10))
					{
						map[x, y] = MAP_TILE.CACTUS;
					}
					else if (x == 0 || x == 15 || y == 0 || y == 15)
					{
						map[x, y] = (MAP_TILE)((Game1.random.NextDouble() < 0.15) ? 1 : 0);
					}
					else if (x == 1 || x == 14 || y == 1 || y == 14)
					{
						map[x, y] = MAP_TILE.GRAVEL;
					}
					else
					{
						map[x, y] = (MAP_TILE)((Game1.random.NextDouble() < 0.1) ? 4 : 3);
					}
				}
			}
			switch (wave)
			{
				case -1:
					{
						for (int x = 0; x < 16; x++)
						{
							for (int y = 0; y < 16; y++)
							{
								if (map[x, y] == 0 || map[x, y] ==  MAP_TILE.BARRIER2 || map[x, y] ==  MAP_TILE.GRAVEL || map[x, y] ==  MAP_TILE.CACTUS)
								{
									map[x, y] = MAP_TILE.SAND;
								}
							}
						}
						map[3, 1] = MAP_TILE.CACTUS;
						map[8, 2] = MAP_TILE.CACTUS;
						map[13, 1] = MAP_TILE.CACTUS;
						map[5, 0] = MAP_TILE.BARRIER1;
						map[10, 2] = MAP_TILE.GRAVEL;
						map[15, 2] = MAP_TILE.BARRIER2;
						map[14, 12] = MAP_TILE.CACTUS;
						map[10, 6] = MAP_TILE.FENCE;
						map[11, 6] = MAP_TILE.FENCE;
						map[12, 6] = MAP_TILE.FENCE;
						map[13, 6] = MAP_TILE.FENCE;
						map[14, 6] = MAP_TILE.FENCE;
						map[14, 7] = MAP_TILE.FENCE;
						map[14, 8] = MAP_TILE.FENCE;
						map[14, 9] = MAP_TILE.FENCE;
						map[14, 10] = MAP_TILE.FENCE;
						map[14, 11] = MAP_TILE.FENCE;
						map[14, 12] = MAP_TILE.FENCE;
						map[14, 13] = MAP_TILE.FENCE;
						for (int x = 0; x < 16; x++)
						{
							map[x, 3] = (MAP_TILE)((x % 2 == 0) ? 9 : 8);
						}
						map[3, 3] = MAP_TILE.BRIDGE;
						map[7, 8] = MAP_TILE.GRAVEL;
						map[8, 8] = MAP_TILE.GRAVEL;
						map[4, 11] = MAP_TILE.GRAVEL;
						map[11, 12] = MAP_TILE.GRAVEL;
						map[9, 11] = MAP_TILE.GRAVEL;
						map[3, 9] = MAP_TILE.GRAVEL;
						map[2, 12] = MAP_TILE.CACTUS;
						map[8, 13] = MAP_TILE.CACTUS;
						map[12, 11] = MAP_TILE.CACTUS;
						map[7, 14] = 0;
						map[6, 14] = MAP_TILE.GRAVEL;
						map[8, 14] = MAP_TILE.GRAVEL;
						map[7, 13] = MAP_TILE.GRAVEL;
						map[7, 15] = MAP_TILE.GRAVEL;
						break;
					}
				case 1:
					map[4, 4] = MAP_TILE.FENCE;
					map[4, 5] = MAP_TILE.FENCE;
					map[5, 4] = MAP_TILE.FENCE;
					map[12, 4] = MAP_TILE.FENCE;
					map[11, 4] = MAP_TILE.FENCE;
					map[12, 5] = MAP_TILE.FENCE;
					map[4, 12] = MAP_TILE.FENCE;
					map[5, 12] = MAP_TILE.FENCE;
					map[4, 11] = MAP_TILE.FENCE;
					map[12, 12] = MAP_TILE.FENCE;
					map[11, 12] = MAP_TILE.FENCE;
					map[12, 11] = MAP_TILE.FENCE;
					break;
				case 2:
					map[8, 4] = MAP_TILE.FENCE;
					map[12, 8] = MAP_TILE.FENCE;
					map[8, 12] = MAP_TILE.FENCE;
					map[4, 8] = MAP_TILE.FENCE;
					map[1, 1] = MAP_TILE.CACTUS;
					map[14, 1] = MAP_TILE.CACTUS;
					map[14, 14] = MAP_TILE.CACTUS;
					map[1, 14] = MAP_TILE.CACTUS;
					map[2, 1] = MAP_TILE.CACTUS;
					map[13, 1] = MAP_TILE.CACTUS;
					map[13, 14] = MAP_TILE.CACTUS;
					map[2, 14] = MAP_TILE.CACTUS;
					map[1, 2] = MAP_TILE.CACTUS;
					map[14, 2] = MAP_TILE.CACTUS;
					map[14, 13] = MAP_TILE.CACTUS;
					map[1, 13] = MAP_TILE.CACTUS;
					break;
				case 3:
					map[5, 5] = MAP_TILE.FENCE;
					map[6, 5] = MAP_TILE.FENCE;
					map[7, 5] = MAP_TILE.FENCE;
					map[9, 5] = MAP_TILE.FENCE;
					map[10, 5] = MAP_TILE.FENCE;
					map[11, 5] = MAP_TILE.FENCE;
					map[5, 11] = MAP_TILE.FENCE;
					map[6, 11] = MAP_TILE.FENCE;
					map[7, 11] = MAP_TILE.FENCE;
					map[9, 11] = MAP_TILE.FENCE;
					map[10, 11] = MAP_TILE.FENCE;
					map[11, 11] = MAP_TILE.FENCE;
					map[5, 6] = MAP_TILE.FENCE;
					map[5, 7] = MAP_TILE.FENCE;
					map[5, 9] = MAP_TILE.FENCE;
					map[5, 10] = MAP_TILE.FENCE;
					map[11, 6] = MAP_TILE.FENCE;
					map[11, 7] = MAP_TILE.FENCE;
					map[11, 9] = MAP_TILE.FENCE;
					map[11, 10] = MAP_TILE.FENCE;
					break;
				case 4:
				case 8:
					{
						for (int x = 0; x < 16; x++)
						{
							for (int y = 0; y < 16; y++)
							{
								if (map[x, y] ==  MAP_TILE.CACTUS)
								{
									map[x, y] = (MAP_TILE)((Game1.random.NextDouble() >= 0.5) ? 1 : 0);
								}
							}
						}
						for (int x = 0; x < 16; x++)
						{
							map[x, 8] = (MAP_TILE)((Game1.random.NextDouble() < 0.5) ? 8 : 9);
						}
						map[8, 4] = MAP_TILE.FENCE;
						map[8, 12] = MAP_TILE.FENCE;
						map[9, 12] = MAP_TILE.FENCE;
						map[7, 12] = MAP_TILE.FENCE;
						map[5, 6] = MAP_TILE.CACTUS;
						map[10, 6] = MAP_TILE.CACTUS;
						break;
					}
				case 5:
					map[1, 1] = MAP_TILE.CACTUS;
					map[14, 1] = MAP_TILE.CACTUS;
					map[14, 14] = MAP_TILE.CACTUS;
					map[1, 14] = MAP_TILE.CACTUS;
					map[2, 1] = MAP_TILE.CACTUS;
					map[13, 1] = MAP_TILE.CACTUS;
					map[13, 14] = MAP_TILE.CACTUS;
					map[2, 14] = MAP_TILE.CACTUS;
					map[1, 2] = MAP_TILE.CACTUS;
					map[14, 2] = MAP_TILE.CACTUS;
					map[14, 13] = MAP_TILE.CACTUS;
					map[1, 13] = MAP_TILE.CACTUS;
					map[3, 1] = MAP_TILE.CACTUS;
					map[13, 1] = MAP_TILE.CACTUS;
					map[13, 13] = MAP_TILE.CACTUS;
					map[1, 13] = MAP_TILE.CACTUS;
					map[1, 3] = MAP_TILE.CACTUS;
					map[13, 3] = MAP_TILE.CACTUS;
					map[12, 13] = MAP_TILE.CACTUS;
					map[3, 14] = MAP_TILE.CACTUS;
					map[3, 3] = MAP_TILE.CACTUS;
					map[13, 12] = MAP_TILE.CACTUS;
					map[13, 12] = MAP_TILE.CACTUS;
					map[3, 12] = MAP_TILE.CACTUS;
					break;
				case 6:
					map[4, 5] = MAP_TILE.GRAVEL;
					map[12, 10] = MAP_TILE.CACTUS;
					map[10, 9] = MAP_TILE.CACTUS;
					map[5, 12] = MAP_TILE.GRAVEL;
					map[5, 9] = MAP_TILE.CACTUS;
					map[12, 12] = MAP_TILE.CACTUS;
					map[3, 4] = MAP_TILE.CACTUS;
					map[2, 3] = MAP_TILE.CACTUS;
					map[11, 3] = MAP_TILE.CACTUS;
					map[10, 6] = MAP_TILE.CACTUS;
					map[5, 9] = MAP_TILE.FENCE;
					map[10, 12] = MAP_TILE.FENCE;
					map[3, 12] = MAP_TILE.FENCE;
					map[10, 8] = MAP_TILE.FENCE;
					break;
				case 7:
					{
						for (int x = 0; x < 16; x++)
						{
							map[x, 5] = (MAP_TILE)((x % 2 == 0) ? 9 : 8);
							map[x, 10] = (MAP_TILE)((x % 2 == 0) ? 9 : 8);
						}
						map[4, 5] = MAP_TILE.BRIDGE;
						map[8, 5] = MAP_TILE.BRIDGE;
						map[12, 5] = MAP_TILE.BRIDGE;
						map[4, 10] = MAP_TILE.BRIDGE;
						map[8, 10] = MAP_TILE.BRIDGE;
						map[12, 10] = MAP_TILE.BRIDGE;
						break;
					}
				case 9:
					map[4, 4] = MAP_TILE.CACTUS;
					map[5, 4] = MAP_TILE.CACTUS;
					map[10, 4] = MAP_TILE.CACTUS;
					map[12, 4] = MAP_TILE.CACTUS;
					map[4, 5] = MAP_TILE.CACTUS;
					map[5, 5] = MAP_TILE.CACTUS;
					map[10, 5] = MAP_TILE.CACTUS;
					map[12, 5] = MAP_TILE.CACTUS;
					map[4, 10] = MAP_TILE.CACTUS;
					map[5, 10] = MAP_TILE.CACTUS;
					map[10, 10] = MAP_TILE.CACTUS;
					map[12, 10] = MAP_TILE.CACTUS;
					map[4, 12] = MAP_TILE.CACTUS;
					map[5, 12] = MAP_TILE.CACTUS;
					map[10, 12] = MAP_TILE.CACTUS;
					map[12, 12] = MAP_TILE.CACTUS;
					break;
				case 10:
					{
						for (int x = 0; x < 16; x++)
						{
							map[x, 1] = (MAP_TILE)((x % 2 == 0) ? 9 : 8);
							map[x, 14] = (MAP_TILE)((x % 2 == 0) ? 9 : 8);
						}
						map[8, 1] = MAP_TILE.BRIDGE;
						map[7, 1] = MAP_TILE.BRIDGE;
						map[9, 1] = MAP_TILE.BRIDGE;
						map[8, 14] = MAP_TILE.BRIDGE;
						map[7, 14] = MAP_TILE.BRIDGE;
						map[9, 14] = MAP_TILE.BRIDGE;
						map[6, 8] = MAP_TILE.CACTUS;
						map[10, 8] = MAP_TILE.CACTUS;
						map[8, 6] = MAP_TILE.CACTUS;
						map[8, 9] = MAP_TILE.CACTUS;
						break;
					}
				case 11:
					{
						for (int x = 0; x < 16; x++)
						{
							map[x, 0] = MAP_TILE.FENCE;
							map[x, 15] = MAP_TILE.FENCE;
							if (x % 2 == 0)
							{
								map[x, 1] = MAP_TILE.CACTUS;
								map[x, 14] = MAP_TILE.CACTUS;
							}
						}
						break;
					}
				case 12:
					{
						for (int x = 0; x < 16; x++)
						{
							for (int y = 0; y < 16; y++)
							{
								if (map[x, y] == 0 || map[x, y] ==  MAP_TILE.BARRIER2)
								{
									map[x, y] = MAP_TILE.CACTUS;
								}
							}
						}
						for (int x = 0; x < 16; x++)
						{
							map[x, 0] = (MAP_TILE)((x % 2 == 0) ? 9 : 8);
							map[x, 15] = (MAP_TILE)((x % 2 == 0) ? 9 : 8);
						}
						Rectangle r = new(1, 1, 14, 14);
						foreach (Vector2 v2 in StardewValley.Utility.getBorderOfThisRectangle(r))
						{
							map[(int)v2.X, (int)v2.Y] = MAP_TILE.BRIDGE;
						}
						r.Inflate(-1, -1);
						{
							foreach (Vector2 v in StardewValley.Utility.getBorderOfThisRectangle(r))
							{
								map[(int)v.X, (int)v.Y] = MAP_TILE.GRAVEL;
							}
							return map;
						}
					}
				default:
					map[4, 4] = MAP_TILE.CACTUS;
					map[12, 4] = MAP_TILE.CACTUS;
					map[4, 12] = MAP_TILE.CACTUS;
					map[12, 12] = MAP_TILE.CACTUS;
					break;
			}
			return map;
		}
	}
}
