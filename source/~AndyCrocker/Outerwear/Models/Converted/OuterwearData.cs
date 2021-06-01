/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;

namespace Outerwear.Models.Converted
{
    /// <summary>Represents an outerwear.</summary>
    public class OuterwearData
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The id of the outerwear object.</summary>
        public int ObjectId { get; }

        /// <summary>The type of outerwear.</summary>
        public OuterwearType Type { get; }

        /// <summary>The effects the outerwear has when equipped.</summary>
        public OuterwearEffects Effects { get; }

        /// <summary>The spritesheet of the outerwear on the farmer when it's equipped.</summary>
        public Texture2D EquippedTexture { get; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="objectId">The id of the outerwear object.</param>
        /// <param name="type">The type of outerwear.</param>
        /// <param name="effects">The effects the outerwear has when equipped.</param>
        /// <param name="equippedTexture">The spritesheet of the outerwear on the farmer when it's equipped.</param>
        public OuterwearData(int objectId, OuterwearType type, OuterwearEffects effects, Texture2D equippedTexture)
        {
            ObjectId = objectId;
            Type = type;
            Effects = effects ?? new OuterwearEffects();
            EquippedTexture = equippedTexture;
        }
    }
}
