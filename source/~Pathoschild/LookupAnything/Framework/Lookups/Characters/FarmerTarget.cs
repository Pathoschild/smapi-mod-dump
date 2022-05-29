/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewValley;

namespace Pathoschild.Stardew.LookupAnything.Framework.Lookups.Characters
{
    /// <summary>Positional metadata about a farmer (i.e. player).</summary>
    internal class FarmerTarget : GenericTarget<Farmer>
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="gameHelper">Provides utility methods for interacting with the game code.</param>
        /// <param name="value">The underlying in-game entity.</param>
        /// <param name="getSubject">Get the subject info about the target.</param>
        public FarmerTarget(GameHelper gameHelper, Farmer value, Func<ISubject> getSubject)
            : base(gameHelper, SubjectType.Farmer, value, value.getTileLocation(), getSubject) { }

        /// <summary>Get the sprite's source rectangle within its texture.</summary>
        public override Rectangle GetSpritesheetArea()
        {
            return this.Value.FarmerSprite.SourceRect;
        }

        /// <summary>Get a rectangle which roughly bounds the visible sprite relative the viewport.</summary>
        public override Rectangle GetWorldArea()
        {
            return this.GetSpriteArea(this.Value.GetBoundingBox(), this.GetSpritesheetArea());
        }

        /// <summary>Get whether the visible sprite intersects the specified coordinate. This can be an expensive test.</summary>
        /// <param name="tile">The tile to search.</param>
        /// <param name="position">The viewport-relative coordinates to search.</param>
        /// <param name="spriteArea">The approximate sprite area calculated by <see cref="GetWorldArea"/>.</param>
        public override bool SpriteIntersectsPixel(Vector2 tile, Vector2 position, Rectangle spriteArea)
        {
            return spriteArea.Contains((int)position.X, (int)position.Y);
        }
    }
}
