/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using Magic.Schools;
using Microsoft.Xna.Framework;
using SpaceCore;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace Magic.Spells
{
    public class WaterSpell : Spell
    {
        public WaterSpell() : base( SchoolId.Toil, "water" )
        {
        }

        public override int getManaCost(Farmer player, int level)
        {
            return 0;
        }

        public override IActiveEffect onCast(Farmer player, int level, int targetX, int targetY)
        {
            level += 1;
            targetX /= Game1.tileSize;
            targetY /= Game1.tileSize;
            Vector2 target = new Vector2(targetX, targetY);

            int num = 0;

            GameLocation loc = player.currentLocation;
            for (int ix = targetX - level; ix <= targetX + level; ++ix)
            {
                for (int iy = targetY - level; iy <= targetY + level; ++iy)
                {
                    if (player.getCurrentMana() <= 3)
                        return null;

                    Vector2 pos = new Vector2(ix, iy);
                    if (!loc.terrainFeatures.ContainsKey(pos))
                        continue;

                    HoeDirt dirt = loc.terrainFeatures[pos] as HoeDirt;
                    if (dirt == null || dirt.state.Value != HoeDirt.dry)
                        continue;

                    dirt.state.Value = HoeDirt.watered;

                    loc.temporarySprites.Add(new TemporaryAnimatedSprite(13, new Vector2(ix * (float)Game1.tileSize, iy * (float)Game1.tileSize), Color.White, 10, Game1.random.NextDouble() < 0.5, 70f, 0, Game1.tileSize, (float)(((double)iy * (double)Game1.tileSize + (double)(Game1.tileSize / 2)) / 10000.0 - 0.00999999977648258), -1, 0)
                    {
                        delayBeforeAnimationStart = num * 10
                    });
                    num++;

                    player.addMana(-4);
                    player.AddCustomSkillExperience(Magic.Skill, 1);
                    Game1.playSound("wateringCan");
                }
            }

            return null;
        }
    }
}
