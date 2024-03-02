/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/SAML
**
*************************************************/

namespace SAML
{
    /// <summary>
    /// The orientation to use for layout in any supported panel
    /// </summary>
    public enum Orientation
    {
        Horizontal = 0,
        LandScape = Horizontal,
        Vertical = 1,
        Portrait = Vertical
    }

    /// <summary>
    /// Simple definition for Item information rendering
    /// </summary>
    public enum StackDrawType
    {
        None,
        StackNumOnly,
        QualityOnly,
        Both
    }

    /// <summary>
    /// A set of recognized event ids
    /// </summary>
    /// <remarks>
    /// This is not a comprehensive list of all events, since it excludes events fired by the source, which aren't included in the event manager
    /// </remarks>
    public enum EventIds
    {
        Update,
        LeftClick,
        RightClick,
        GamePadButtonDown,
        Hover,
        KeyDown,
        TextInput,
        GotFocus,
        LostFocus,
        WindowSizeChanged,
        MouseEnter,
        MouseLeave,
        Scrolled
    }

    /// <summary>
    /// Controls the horizontal position (x) of the designated target
    /// </summary>
    public enum HorizontalAlignment
    {
        None,
        Left,
        Start = Left,
        Center,
        Right,
        End = Right
    }

    /// <summary>
    /// Controls the vertical position (y) of the designated target
    /// </summary>
    public enum VerticalAlignment
    {
        None,
        Top,
        Start = Top,
        Center,
        Bottom,
        End = Bottom
    }

    /// <summary>
    /// A readable value for a scroll bar's direction
    /// </summary>
    public enum ScrollDirection
    {
        Up,
        Down
    }

    /// <summary>
    /// What type of decoration to use for the <see cref="MenuBox"/>
    /// </summary>
    public enum DecorationStyle
    {
        None,
        Smooth,
        Bauble
    }

    /// <summary>
    /// On What sides to apply decoration for the <see cref="MenuBox"/>
    /// </summary>
    [Flags]
    public enum DecorationSides
    {
        All = 0,
        Up = 1,
        Down = 2,
        Right = 4,
        Left = 8
    }
}
