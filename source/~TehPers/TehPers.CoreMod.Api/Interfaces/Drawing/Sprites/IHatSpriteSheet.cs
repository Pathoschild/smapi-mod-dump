namespace TehPers.CoreMod.Api.Drawing.Sprites {
    public interface IHatSpriteSheet : ISpriteSheet {
        /// <summary>Tries to get a reference to a specific hat sprite on this sprite sheet, given the direction the wearer is facing.</summary>
        /// <param name="hatId">The ID for the hat.</param>
        /// <param name="direction">The direction the wearer is facing.</param>
        /// <param name="sprite">The sprite for the hat.</param>
        /// <returns>True if the hat was found, false otherwise.</returns>
        bool TryGetSprite(int hatId, FacingDirection direction, out ISprite sprite);
    }
}