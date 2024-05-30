/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using ArsVenefici.Framework.Spells.Effects;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.Interfaces
{
    public class ProjectileEntityWrapper : IEntity
    {
        private object _entity;
        public object entity { get { return _entity; } }

        public ProjectileEntityWrapper(SpellProjectile spellProjectile)
        {
            _entity = spellProjectile;
        }

        public GameLocation GetGameLocation()
        {
            return ((SpellProjectile)entity).GetGameLocation();
        }

        public Vector2 GetPosition()
        {
            return ((SpellProjectile)entity).position.Get();
        }

        public Rectangle GetBoundingBox()
        {
            return ((SpellProjectile)_entity).getBoundingBox();
        }

        public int GetHorizontalMovement()
        {
            return 1;
        }

        public int GetVerticalMovement()
        {
            return 1;
        }
    }
}
