/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using System.Collections.Generic;

namespace PyTK.Types
{
    public class InterceptorData
    {
        public List<string> Mods { get; set; } = new List<string>() { PyTKMod._instance.ModManifest.UniqueID };
    }
}
