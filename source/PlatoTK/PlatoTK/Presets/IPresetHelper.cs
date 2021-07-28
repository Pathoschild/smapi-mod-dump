/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using System;

namespace PlatoTK.Presets
{
    public interface IPresetHelper
    {
        void RegisterArcade(string id, string name, string objectName, Action start, string sprite, string iconForMobilePhone);
    }
}
