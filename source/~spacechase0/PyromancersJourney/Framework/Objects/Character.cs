/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;

namespace PyromancersJourney.Framework.Objects
{
    internal class Character : BaseObject
    {
        public virtual RectangleF BoundingBox { get; } = new(0, 0, 0.35f, 0.35f);

        public virtual bool Floats { get; } = false;
        public int Health { get; set; } = 1;

        public Character(World world)
            : base(world) { }

        public virtual void Hurt(int amt)
        {
            this.Health -= amt;
        }

        public virtual void DoMovement() { }

        public override void Update()
        {
            var oldPos = this.Position;

            this.DoMovement();

            // Lazy implementation - would use something better if using a real engine
            Func<float, float, bool> solidCheck = this.Floats ? this.World.Map.IsAirSolid : this.World.Map.IsSolid;
            if (solidCheck(this.Position.X, this.Position.Z))
            {
                if (!solidCheck(oldPos.X, this.Position.Z))
                    this.Position.X = oldPos.X;
                else if (!solidCheck(this.Position.X, oldPos.Z))
                    this.Position.Z = oldPos.Z;
                else
                    this.Position = oldPos;
            }
        }

        /// <inheritdoc />
        public override void Dispose() { }
    }
}
