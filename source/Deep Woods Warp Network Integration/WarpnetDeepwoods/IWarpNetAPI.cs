/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/WarpnetDeepwoods
**
*************************************************/

using System;

namespace WarpnetDeepwoods
{
    public interface IWarpNetAPI
    {
        void AddCustomDestinationHandler(string ID, Func<bool> getEnabled, Func<string> getLabel, Func<string> getIconName, Action warp);
        void RemoveCustomDestinationHandler(string ID);
    }
}
