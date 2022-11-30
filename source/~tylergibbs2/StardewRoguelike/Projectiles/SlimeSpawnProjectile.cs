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
using StardewValley.Projectiles;
using System;

namespace StardewRoguelike.Projectiles
{
    public class SlimeSpawnProjectile : BasicProjectile
    {
        public SlimeSpawnProjectile() : base() { }

        public SlimeSpawnProjectile(float scale, int damageToFarmer, float xVelocity, float yVelocity, Vector2 startingPosition, string firingSound, GameLocation location, Character firer)
            : base(damageToFarmer, 13, 5, 0, (float)Math.PI / 16f, xVelocity, yVelocity, startingPosition, "", firingSound, true, false, location, firer, false, null)
        {
            startingScale.Value = scale;
        }

        public override bool update(GameTime time, GameLocation location)
        {
            bool collided = base.update(time, location);

            if (collided)
            {
                location.playSound("slimeHit");

                // spawn slimes
                int spread = 3;
                int slimesToSpawn = 5;
                if (Curse.AnyFarmerHasCurse(CurseType.MoreEnemiesLessHealth))
                    slimesToSpawn += 2;

                Vector2 tilePosition = new((int)position.X / 64, (int)position.Y / 64);
                int attempts = 0;

                while (slimesToSpawn > 0 && attempts < 500)
                {
                    attempts++;

                    Vector2 randomTile = new(
                        tilePosition.X + Game1.random.Next(-spread, spread + 1),
                        tilePosition.Y + Game1.random.Next(-spread, spread + 1)
                    );
                    if (!location.isTileLocationTotallyClearAndPlaceable(randomTile))
                        continue;

                    MineShaft mine = (MineShaft)location;
                    Monster slime = mine.BuffMonsterIfNecessary(new GreenSlime(randomTile * 64f, 80));
                    slime.isHardModeMonster.Value = true;
                    slime.moveTowardPlayerThreshold.Value = 25;
                    if (!slime.Sprite.textureName.Value.EndsWith("_dangerous"))
                        slime.Sprite.LoadTexture(slime.Sprite.textureName.Value + "_dangerous");
                    Roguelike.AdjustMonster((MineShaft)location, ref slime);
                    location.characters.Add(slime);

                    slimesToSpawn--;
                }
            }

            return collided;
        }
    }
}
