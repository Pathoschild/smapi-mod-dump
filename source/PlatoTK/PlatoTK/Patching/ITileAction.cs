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

namespace PlatoTK.Patching
{
    public interface ITileAction
    {
        string[] Trigger { get; }

        Action<ITileActionTrigger> Handler { get; }
    }
}
