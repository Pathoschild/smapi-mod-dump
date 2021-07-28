/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using System;

namespace PlatoTK.Events
{
    public interface ITVChannelSelectedEventArgs
    {
        TV TVInstance { get; }
        string ChannelName { get; }
        Vector2 ScreenPosition { get; }
        float ScreenLayerDepth { get; }
        float OverlayLayerDepth { get; }
        float Scale { get; }

        void ShowScene(TemporaryAnimatedSprite screen, TemporaryAnimatedSprite overlay, string dialogue, Action nextAction);

        void TurnOffTV();

        void PreventDefault();
    }
}
