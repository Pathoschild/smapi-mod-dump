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