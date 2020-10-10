/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System;

namespace Igorious.StardewValley.DynamicAPI.Menu
{
    [Flags]
    public enum Aligment
    {
        Top = 0,
        VerticalCenter = 1,
        Bottom = 2,

        Left = 0,
        HorizontalCenter = 4,
        Right = 8,

        Center = VerticalCenter | HorizontalCenter,
    }
}