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
using StardewValley.Monsters;
using StardewValley.Projectiles;

namespace BattleRoyale.Patches
{
    class SlingshotPatch5 : Patch
    {
        public static Rectangle GetFarmerBounds(Farmer f)
        {
            var bounds = f.GetBoundingBox();
            int s = 32 * 2;
            bounds.Y -= s;
            bounds.Height += s;
            return bounds;
        }

        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(Projectile), "behaviorOnCollision");

        public static bool Prefix()
        {
            return false;
        }

        public static bool Postfix(bool __result, Projectile __instance, GameLocation location)
        {
            bool whatToReturn = false;

            foreach (Farmer farmer in location.farmers)
            {
                if (GetFarmerBounds(farmer).Intersects(__instance.getBoundingBox()))
                {
                    __instance.behaviorOnCollisionWithPlayer(location, farmer);
                    return whatToReturn;
                }
            }

            foreach (NPC character in location.characters)
            {
                if (character is not Monster)
                    continue;

                Monster monster = character as Monster;
                if (monster.GetBoundingBox().Intersects(__instance.getBoundingBox()))
                {
                    __instance.behaviorOnCollisionWithMonster(monster, location);
                    return true;
                }
            }

            foreach (Vector2 item in Utility.getListOfTileLocationsForBordersOfNonTileRectangle(__instance.getBoundingBox()))
            {
                //__result = true;
                whatToReturn = true;

                if (location.terrainFeatures.ContainsKey(item) && !location.terrainFeatures[item].isPassable(null))
                {
                    __instance.behaviorOnCollisionWithTerrainFeature(location.terrainFeatures[item], item, location);
                    return whatToReturn;
                }
            }
            __instance.behaviorOnCollisionWithOther(location);
            return whatToReturn;
        }
    }
}
