/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using Netcode;
using System.Collections.Generic;

namespace PyTK.CustomElementHandler
{
    public interface ISyncableElement : ISaveElement
    {
        Dictionary<string, string> getSyncData();
        void sync(Dictionary<string, string> syncData);
        PySync syncObject { get; set; }
    }
}
