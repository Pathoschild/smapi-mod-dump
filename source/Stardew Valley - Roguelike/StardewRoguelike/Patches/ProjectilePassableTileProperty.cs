/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Network;
using StardewValley.Projectiles;
using System.Reflection;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(Projectile), "isColliding")]
    internal class ProjectilePassableTileProperty
    {
        public static bool Prefix(ref bool __result, Projectile __instance, GameLocation location)
        {
			NetPosition position = (NetPosition)__instance.GetType().GetField("position", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);
			NetBool damagesMonsters = (NetBool)__instance.GetType().GetField("damagesMonsters", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);

			Vector2 tilePosition = new(position.Value.X / 64f, position.Value.Y / 64f);
			if (location.isTileOnMap(position.Value / 64f) && location.doesTileHaveProperty((int)tilePosition.X, (int)tilePosition.Y, "ProjectilePassable", "Buildings") is not null)
			{
				if (damagesMonsters.Value && location.doesPositionCollideWithCharacter(__instance.getBoundingBox()) != null)
                {
                    __result = true;
                    return false;
                }
                __result = false;
				return false;
			}

			return true;
        }
    }
}
