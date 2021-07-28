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
using PlatoTK.Reflection;

namespace PlatoTK.Events
{
    internal class TVChannelSelectedEventArgs : ITVChannelSelectedEventArgs
    {
        internal Action Callback { get; }

        public TV TVInstance { get; }

        public string ChannelName { get; }

        public Vector2 ScreenPosition => TVInstance.getScreenPosition();

        public float ScreenLayerDepth => (float)((TVInstance.boundingBox.Bottom - 1) / 10000.0 + 9.99999974737875E-06);

        public float OverlayLayerDepth => (float)((TVInstance.boundingBox.Bottom - 1) / 10000.0 + 1.99999994947575E-05);

        public float Scale => TVInstance.getScreenSizeModifier();

        public TVChannelSelectedEventArgs(string name, TV tvInstance, Action callback)
        {
            ChannelName = name;
            TVInstance = tvInstance;
            Callback = callback;
        }

        public void ShowScene(TemporaryAnimatedSprite screen, TemporaryAnimatedSprite screenOverlay, string dialogue, Action nextAction)
        {
            TVInstance.SetFieldValue("screen",screen);
            TVInstance.SetFieldValue("screenOverlay", screenOverlay);
            
            if(!string.IsNullOrEmpty(dialogue))
                Game1.drawObjectDialogue(Game1.parseText(dialogue));

            Game1.afterDialogues = new Game1.afterFadeFunction(nextAction);
        }

        public void TurnOffTV()
        {
            TVInstance.turnOffTV();
        }

        public void PreventDefault()
        {
            Callback();
        }
    }
}
