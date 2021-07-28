/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using xTile;

namespace PlatoTK.Content
{
    public interface IInjectionHelper
    {
        void InjectLoad<TAsset>(string assetName, TAsset asset, string conditions = "");
        void InjectDataInsert(string assetName, int key, string value, string conditions = "");
        void InjectDataPatch(string assetName, int key, string conditions = "", params string[] values);
        void InjectDataInsert(string assetName, string key, string value, string conditions = "");
        void InjectDataPatch(string assetName, string key, string conditions = "", params string[] values);
        void InjectPatch(string assetName, Texture2D asset, bool overlay = false, Rectangle? sourceArea = null, Rectangle? targetArea = null, string conditions = "");
        void InjectPatch(string assetName, Map asset, Rectangle? sourceArea = null, Rectangle? targetArea = null, string conditions = "", bool removeEmpty = false);

    }
}
