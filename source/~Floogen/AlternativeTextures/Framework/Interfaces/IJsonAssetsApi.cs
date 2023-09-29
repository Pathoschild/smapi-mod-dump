/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/AlternativeTextures
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using System;

namespace AlternativeTextures.Framework.Interfaces
{
    public interface IJsonAssetsApi
    {
        int GetBigCraftableId(string name);
        int GetObjectId(string name);
        bool TryGetGiantCropSprite(int productID, out Lazy<Texture2D> texture);


        event EventHandler IdsAssigned;
    }
}
