/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.SpritePatcher.Framework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewMods.SpritePatcher.Framework.Enums;
using StardewMods.SpritePatcher.Framework.Models;

/// <summary>Represents a conditional patch.</summary>
public interface ISpritePatch
{
    /// <summary>Gets the unique identifier for this mod.</summary>
    string Id { get; }

    /// <summary>Gets the content pack associated with this mod.</summary>
    IContentPack ContentPack { get; }

    /// <summary>Gets the content model associated with this mod.</summary>
    IContentModel ContentModel { get; }

    /// <summary>Gets the source rectangle of the sprite sheet being patched.</summary>
    Rectangle SourceArea { get; }

    /// <summary>Gets or sets the raw texture data for the patch.</summary>
    IRawTextureData Texture { get; set; }

    /// <summary>Gets or sets the layer of the patch.</summary>
    PatchLayer Layer { get; set; }

    /// <summary>Gets or sets the path of the texture.</summary>
    string Path { get; set; }

    /// <summary>Gets or sets the area of the texture.</summary>
    Rectangle Area { get; set; }

    /// <summary>Gets or sets the tint of the texture.</summary>
    Color Tint { get; set; }

    /// <summary>Gets or sets the alpha of the texture.</summary>
    float Alpha { get; set; }

    /// <summary>Gets or sets the scale of the texture.</summary>
    float Scale { get; set; }

    /// <summary>Gets or sets the number of animation frames.</summary>
    int Frames { get; set; }

    /// <summary>Gets or sets the animation rate.</summary>
    Animate Animate { get; set; }

    /// <summary>Gets or sets the offset for where the patch will be applied.</summary>
    Vector2 Offset { get; set; }

    /// <summary>Gets or sets the color of the draw method.</summary>
    Color Color { get; set; }

    /// <summary>Gets or sets the rotation of the draw method.</summary>
    float Rotation { get; set; }

    /// <summary>Gets or sets the effects of the draw method.</summary>
    SpriteEffects Effects { get; set; }

    /// <summary>Runs code necessary to update the texture.</summary>
    /// <param name="sprite">The managed object requesting the patch.</param>
    /// <param name="spriteSheet">The spriteSheet that the patch is being applied to..</param>
    /// <returns>True if the texture should be applied.</returns>
    bool Run(ISprite sprite, ISpriteSheet spriteSheet);
}