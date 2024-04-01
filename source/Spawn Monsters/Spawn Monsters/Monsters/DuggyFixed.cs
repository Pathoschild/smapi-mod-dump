/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Berisan/SpawnMonsters
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;

namespace Spawn_Monsters
{
    public class DuggyFixed : StardewValley.Monsters.Duggy
    {
        public DuggyFixed() : base() {

        }

        public DuggyFixed(Vector2 pos) : base(pos) {

        }

        public DuggyFixed(Vector2 pos, bool magmaDuggy) : base(pos, magmaDuggy) { 
            
        }

        public override void behaviorAtGameTick(GameTime time) {
            //base.behaviorAtGameTick(time); !I cant explain why but it works like this
            isEmoting = false;
            Sprite.loop = false;
            Rectangle boundingBox = GetBoundingBox();
            if (Sprite.currentFrame < 4) {
                boundingBox.Inflate(128, 128);
                if (!IsInvisible || boundingBox.Contains(Player.getStandingPosition().X, Player.getStandingPosition().Y)) {
                    if (IsInvisible) {
                        if (currentLocation.map.GetLayer("Back").Tiles[(int)Player.Tile.X, (int)Player.Tile.Y].Properties.ContainsKey("NPCBarrier") || !currentLocation.map.GetLayer("Back").Tiles[(int)Player.Tile.X, (int)Player.Tile.Y].TileIndexProperties.ContainsKey("Diggable") && currentLocation.map.GetLayer("Back").Tiles[(int)Player.Tile.X, (int)Player.Tile.Y].TileIndex != 0) {
                            return;
                        }

                        Position = new Vector2(Player.Position.X, Player.Position.Y + Player.Sprite.SpriteHeight - Sprite.SpriteHeight);
                        currentLocation.localSound(nameof(StardewValley.Monsters.Duggy));
                        Position = Player.Tile * 64f;
                    }
                    IsInvisible = false;
                    Sprite.interval = 100f;
                    Sprite.AnimateDown(time, 0, "");
                }
            }
            if (Sprite.currentFrame >= 4 && Sprite.currentFrame < 8) {
                boundingBox.Inflate(sbyte.MinValue, sbyte.MinValue);
                currentLocation.isCollidingPosition(boundingBox, Game1.viewport, false, 8, false, this);
                Sprite.AnimateRight(time, 0, "");
                Sprite.interval = 220f;
                DamageToFarmer = 8;
            }
            if (Sprite.currentFrame >= 8) {
                Sprite.AnimateUp(time, 0, "");
            }

            if (Sprite.currentFrame < 10) {
                return;
            }

            IsInvisible = true;
            Sprite.currentFrame = 0;
            Vector2 tileLocation = Tile;
            //this.currentLocation.map.GetLayer("Back").Tiles[(int)tileLocation.X, (int)tileLocation.Y].TileIndex = 0;
            //this.currentLocation.removeEverythingExceptCharactersFromThisTile((int)tileLocation.X, (int)tileLocation.Y);
            DamageToFarmer = 0;
        }
    }
}
