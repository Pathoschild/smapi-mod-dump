/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using Archery.Framework.Interfaces.Internal;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Archery.Framework.Utilities.Enchantments
{
    public class Seeker
    {
        internal static bool HandleEnchantment(IEnchantment enchantment)
        {
            var projectileData = Archery.internalApi.GetProjectileData(Archery.manifest, enchantment.Projectile);
            if (projectileData is null)
            {
                return false;
            }

            var distanceFromFarmer = Vector2.Distance(enchantment.Farmer.Position / 64f, projectileData.Position.Value / 64f);
            if (distanceFromFarmer < 2f)
            {
                return true;
            }

            // Get the closest monster
            Dictionary<Monster, float> monsterToDistance = new Dictionary<Monster, float>();
            foreach (NPC c in enchantment.Location.characters)
            {
                if (c is not Monster monster)
                {
                    continue;
                }

                int maxDetectDistance = 64 * 3;
                float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;
                var distance = Vector2.DistanceSquared(monster.Position, projectileData.Position.Value);

                if (distance <= sqrMaxDetectDistance)
                {
                    monsterToDistance[monster] = distance;
                }
            }

            // If monster found, apply velocity adjustment logic
            var selectedMonster = monsterToDistance.OrderBy(m => m.Value).FirstOrDefault();
            if (selectedMonster.Key is not null)
            {
                int speed = 30;
                if (enchantment.Arguments is not null && enchantment.Arguments.Count > 0 && int.TryParse(enchantment.Arguments[0].ToString(), out int actualSpeed))
                {
                    speed = actualSpeed;
                }

                Vector2 destination = Vector2.Normalize(selectedMonster.Key.Position - projectileData.Position.Value) * projectileData.InitialSpeed.Value;
                Vector2 acceleration = Vector2.Normalize(destination - projectileData.Velocity.Value) * speed;

                projectileData.Velocity += acceleration * (float)enchantment.Time.ElapsedGameTime.TotalSeconds;
                projectileData.Rotation = (float)Math.Atan2(projectileData.Velocity.Value.Y, projectileData.Velocity.Value.X);
            }

            // Set the internal projectile data
            Archery.internalApi.SetProjectileData(Archery.manifest, enchantment.Projectile, projectileData);

            return true;
        }
    }
}
