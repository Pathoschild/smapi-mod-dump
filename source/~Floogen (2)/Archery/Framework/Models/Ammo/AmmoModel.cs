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
using Archery.Framework.Models.Ammo;
using Archery.Framework.Models.ContentPack;
using Archery.Framework.Models.Display;
using Archery.Framework.Models.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Archery.Framework.Models.Weapons
{
    public class AmmoModel : BaseModel
    {
        public AmmoType Type { get; set; }
        public Rectangle? CollisionBox { get; set; }

        public ItemSpriteModel ProjectileSprite { get; set; }
        public List<ItemSpriteModel> ConditionalProjectileSprites { get; set; } = new List<ItemSpriteModel>();

        public DebrisModel Debris { get; set; }
        public ArrowTailModel Trail { get; set; }
        public ExplosionModel Explosion { get; set; }

        public int Damage { get; set; }
        public float BreakChance { get; set; } = 1.0f;
        public int MaxTravelDistance { get; set; } = -1;
        public float RotationVelocity { get; set; } = 0f;

        public Sound ImpactSound { get; set; } = new Sound() { Name = "hammer" };
        public Light Light { get; set; }
        public RandomRange BounceCountRange { get; set; }

        public EnchantmentModel Enchantment { get; set; }

        internal ItemSpriteModel GetProjectileSprite(Farmer who)
        {
            foreach (var sprite in ConditionalProjectileSprites.Where(s => s is not null))
            {
                if (sprite.AreConditionsValid(who))
                {
                    return sprite;
                }
            }
            Archery.conditionManager.Reset(ConditionalProjectileSprites);

            return ProjectileSprite;
        }

        internal static bool CanBreak(float breakChance)
        {
            return breakChance > 0;
        }

        internal static bool ShouldAlwaysBreak(float breakChance)
        {
            return breakChance >= 1f;
        }

        internal override void SetId(IContentPack contentPack)
        {
            Id = String.Concat(contentPack.Manifest.UniqueID, "/", Type, "/", Name);

            base.SetId(contentPack);
        }
    }
}
