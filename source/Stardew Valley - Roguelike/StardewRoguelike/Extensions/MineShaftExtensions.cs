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
using StardewRoguelike.VirtualProperties;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using System.Collections.Generic;

namespace StardewRoguelike.Extensions
{
    public static class MineShaftExtensions
    {
        public static void SpawnMonsters(this MineShaft mine, int amount)
        {
            float spawnChance = 0.022f;

            while (amount > 0)
            {
                for (int i = 0; i < mine.map.GetLayer("Back").LayerWidth; i++)
                {
                    if (amount == 0)
                        break;
                    for (int j = 0; j < mine.map.GetLayer("Back").LayerHeight; j++)
                    {
                        if (amount == 0)
                            break;
                        else if (!mine.isTileClearForMineObjects(i, j))
                            continue;

                        if (Game1.random.NextDouble() <= spawnChance && mine.getDistanceFromStart(i, j) > 5f)
                        {
                            Monster monster = mine.BuffMonsterIfNecessary(mine.getMonsterForThisLevel(mine.mineLevel, i, j));
                            Roguelike.AdjustMonster(mine, ref monster);
                            mine.characters.Add(monster);
                            amount--;
                        }
                    }
                }
            }
        }

        public static void SetTile(this MineShaft mine, int x, int y, string layer, string tileSheetId, int tileId, int animationInterval = -1, int[] animationOrder = null)
        {
            var (tileSheetIndex, tileIndex) = Roguelike.GetTileIndexForMap(mine.map, tileSheetId, tileId);
            if (animationInterval != -1 && animationOrder is not null)
                mine.setAnimatedMapTile(x, y, animationOrder, animationInterval, layer, "", tileSheetIndex);
            else
                mine.setMapTileIndex(x, y, tileIndex, layer, whichTileSheet: tileSheetIndex);
        }

        public static DwarfGate CreateDwarfGate(this MineShaft mine, int index, Point gatePosition, Point switchPosition)
        {
            VolcanoDungeon fake = new(index);
            fake.possibleSwitchPositions[1] = new() { switchPosition };
            fake.mapPath.Value = mine.mapPath.Value;
            DwarfGate gate = new(fake, index, gatePosition.X, gatePosition.Y, 0);
            gate.locationRef.Value = mine;

            // fixes memory leak, do not remove
            fake = null;

            gate.ApplyTiles();
            mine.get_MineShaftDwarfGates().Add(gate);

            return gate;
        }

        public static void SpawnLocalChest(this MineShaft mine, Vector2 tileLocation)
        {
            var chests = mine.get_MineShaftNetChests();
            chests.Add(new(tileLocation));
        }

        public static void SpawnLocalChest(this MineShaft mine, Vector2 tileLocation, Item item)
        {
            List<Item> items = new()
            {
                item
            };
            SpawnLocalChest(mine, tileLocation, items);
        }

        public static void SpawnLocalChest(this MineShaft mine, Vector2 tileLocation, List<Item> items)
        {
            var chests = mine.get_MineShaftNetChests();
            chests.Add(new(tileLocation, items));
        }

        public static bool TryToAddMonster(this MineShaft mine, Monster m, int tileX, int tileY)
        {
            if (mine.isTileClearForMineObjects(tileX, tileY) && !mine.isTileOccupied(new Vector2(tileX, tileY)))
            {
                m.setTilePosition(tileX, tileY);
                mine.characters.Add(m);
                return true;
            }

            return false;
        }

        public static bool IsAreaClearForObjects(this MineShaft mine, Vector2 topLeftTile, int width, int height)
        {
            int topLeftX = (int)topLeftTile.X;
            int topLeftY = (int)topLeftTile.Y;

            for (int i = topLeftX; i < topLeftX + width; i++)
            {
                for (int j = topLeftY; j < topLeftY + height; j++)
                {
                    if (!mine.isTileClearForMineObjects(i, j))
                        return false;
                }
            }

            return true;
        }

        public static bool IsNormalFloor(this MineShaft mine)
        {
            return !ChallengeFloor.IsChallengeFloor(mine) && !ChestFloor.IsChestFloor(mine) && !Merchant.IsMerchantFloor(mine) && !BossFloor.IsBossFloor(mine) && !ForgeFloor.IsForgeFloor(mine);
        }
    }
}