/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using System;
using System.Collections.Generic;

namespace PyTK.APIs
{
    public interface ISerializerAPI
    {
        void AddPreSerialization(IManifest manifest, Func<object, object> preserializer);
        void AddPostDeserialization(IManifest manifest, Func<object, object> postserializer);
        Dictionary<string, string> ParseDataString(object o);
    }
}
