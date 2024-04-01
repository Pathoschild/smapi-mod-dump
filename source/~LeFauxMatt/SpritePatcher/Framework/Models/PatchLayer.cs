/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.SpritePatcher.Framework.Models;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewMods.SpritePatcher.Framework.Enums;

/// <summary>Represents the changes for each layer in a sprite sheet.</summary>
public record struct PatchLayer(
    string Path,
    Rectangle Area,
    Vector2 Offset,
    Color Tint,
    Animate Animate,
    int Frames,
    float Scale,
    float Alpha,
    Color Color,
    float Rotation,
    SpriteEffects Effects);