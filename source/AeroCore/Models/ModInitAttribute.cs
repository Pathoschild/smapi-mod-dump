/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using System;

namespace AeroCore
{
    /// <summary>Allows easily initializing static data from <see cref="API.InitAll()"/></summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
    public class ModInitAttribute : Attribute
    {
        public string Method { get; set; }
        public string WhenHasMod { get; set; }
        public ModInitAttribute()
        {
            Method = "Init";
            WhenHasMod = null;
        }
    }
}
