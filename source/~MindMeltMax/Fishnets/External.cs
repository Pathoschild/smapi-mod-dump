/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Fishnets
{
    public interface IQualityBaitApi
    {
        int GetQuality(int currentQuality, int baitQuality);
    }

    public interface IAlternativeTexturesApi
    {
        Texture2D GetTextureForObject(Object obj, out Rectangle sourceRect);
    }

    public interface IJsonAssetsApi
    {
        int GetObjectId(string name);
    }

    public interface IDynamicGameAssetsApi
    {
        string GetDGAItemId(object item);
        object SpawnDGAItem(string fullId);
    }

    public interface ISaveAnywhereApi
    {
        void addBeforeSaveEvent(string id, Action beforeSave);
        void addAfterLoadEvent(string id, Action afterLoad);
        void addAfterSaveEvent(string id, Action afterSave);
    }
}
