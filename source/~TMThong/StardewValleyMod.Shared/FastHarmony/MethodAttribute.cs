/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace StardewValleyMod.Shared.FastHarmony
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class MethodPatchAttribute : System.Attribute
    {
        public Type TypePatch {  get; }

    }
}
