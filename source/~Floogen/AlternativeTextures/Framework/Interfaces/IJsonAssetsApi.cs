/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/AlternativeTextures
**
*************************************************/

using System;

namespace AlternativeTextures.Framework.Interfaces
{
    public interface IJsonAssetsApi
    {
        int GetBigCraftableId(string name);
        int GetObjectId(string name);

        event EventHandler IdsAssigned;
    }
}
