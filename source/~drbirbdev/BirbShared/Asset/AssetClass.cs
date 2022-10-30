/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using StardewModdingAPI.Events;

namespace BirbShared.Asset
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class AssetClass : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AssetProperty : Attribute
    {
        public string LocalPath;
        public AssetLoadPriority Priority;

        public AssetProperty(string localPath, AssetLoadPriority priority = AssetLoadPriority.Medium)
        {
            this.LocalPath = localPath;
            this.Priority = priority;
        }
    }
}
