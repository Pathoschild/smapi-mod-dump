/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using Magic.Framework.Schools;
using Microsoft.Xna.Framework;
using SpaceCore;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace Magic.Framework.Spells
{
    internal class WaterSpell : Spell
    {
        public WaterSpell()
            : base(SchoolId.Toil, "water") { }

        public override int GetManaCost(Farmer player, int level)
        {
            return 0;
        }

        public override IActiveEffect OnCast(Farmer player, int level, int targetX, int targetY)
        {
            level += 1;
            targetX /= Game1.tileSize;
            targetY /= Game1.tileSize;

            int num = 0;

            GameLocation loc = player.currentLocation;
            for (int tileX = targetX - level; tileX <= targetX + level; ++tileX)
            {
                for (int tileY = targetY - level; tileY <= targetY + level; ++tileY)
                {
                    if (player.GetCurrentMana() <= 3)
                        return null;

                    Vector2 tile = new Vector2(tileX, tileY);

                    if (!loc.terrainFeatures.TryGetValue(tile, out TerrainFeature feature) || feature is not HoeDirt dirt)
                        continue;

                    if (dirt.state.Value != HoeDirt.dry)
                        continue;

                    dirt.state.Value = HoeDirt.watered;

                    loc.temporarySprites.Add(new TemporaryAnimatedSprite(13, new Vector2(tileX * (float)Game1.tileSize, tileY * (float)Game1.tileSize), Color.White, 10, Game1.random.NextDouble() < 0.5, 70f, 0, Game1.tileSize, (float)((tileY * (double)Game1.tileSize + Game1.tileSize / 2) / 10000.0 - 0.00999999977648258))
                    {
                        delayBeforeAnimationStart = num * 10
                    });
                    num++;

                    player.AddMana(-4);
                    player.AddCustomSkillExperience(Magic.Skill, 1);
                    loc.localSoundAt("wateringCan", tile);
                }
            }

            return null;
        }
    }
}
