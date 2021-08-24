/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Deflaktor/KeySuppressor
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeySuppressor
{
    class ModConfig
    {
        public enum SuppressMode
        {
            DoNotSuppress,
            Suppress,
            SuppressOnlyInMenu,
            SuppressOnlyWhenPlayerFree,
            SuppressOnlyWhenPlayerCanMove
        }

        public Dictionary<SButton, SuppressMode> SuppressedKeys { get; set; } = GetDefaultSuppressedKeys();

        private static Dictionary<SButton, SuppressMode> GetDefaultSuppressedKeys()
        {
            var defaultSuppressedKeys = new Dictionary<SButton, SuppressMode>();
            defaultSuppressedKeys.Add(SButton.DPadDown, SuppressMode.Suppress);
            defaultSuppressedKeys.Add(SButton.DPadLeft, SuppressMode.Suppress);
            defaultSuppressedKeys.Add(SButton.DPadRight, SuppressMode.Suppress);
            defaultSuppressedKeys.Add(SButton.DPadUp, SuppressMode.Suppress);
            defaultSuppressedKeys.Add(SButton.LeftShift, SuppressMode.DoNotSuppress);
            defaultSuppressedKeys.Add(SButton.RightStick, SuppressMode.Suppress);
            defaultSuppressedKeys.Add(SButton.ControllerA, SuppressMode.DoNotSuppress);
            defaultSuppressedKeys.Add(SButton.ControllerB, SuppressMode.SuppressOnlyWhenPlayerFree);
            defaultSuppressedKeys.Add(SButton.ControllerX, SuppressMode.DoNotSuppress);
            defaultSuppressedKeys.Add(SButton.ControllerY, SuppressMode.DoNotSuppress);
            defaultSuppressedKeys.Add(SButton.ControllerBack, SuppressMode.DoNotSuppress);
            defaultSuppressedKeys.Add(SButton.ControllerStart, SuppressMode.DoNotSuppress);
            defaultSuppressedKeys.Add(SButton.BigButton, SuppressMode.DoNotSuppress);
            defaultSuppressedKeys.Add(SButton.LeftShoulder, SuppressMode.DoNotSuppress);
            defaultSuppressedKeys.Add(SButton.RightShoulder, SuppressMode.DoNotSuppress);
            defaultSuppressedKeys.Add(SButton.LeftTrigger, SuppressMode.DoNotSuppress);
            defaultSuppressedKeys.Add(SButton.RightTrigger, SuppressMode.DoNotSuppress);
            defaultSuppressedKeys.Add(SButton.LeftThumbstickDown, SuppressMode.DoNotSuppress);
            defaultSuppressedKeys.Add(SButton.LeftThumbstickLeft, SuppressMode.DoNotSuppress);
            defaultSuppressedKeys.Add(SButton.LeftThumbstickRight, SuppressMode.DoNotSuppress);
            defaultSuppressedKeys.Add(SButton.LeftThumbstickUp, SuppressMode.DoNotSuppress);
            defaultSuppressedKeys.Add(SButton.RightThumbstickDown, SuppressMode.DoNotSuppress);
            defaultSuppressedKeys.Add(SButton.RightThumbstickLeft, SuppressMode.DoNotSuppress);
            defaultSuppressedKeys.Add(SButton.RightThumbstickRight, SuppressMode.DoNotSuppress);
            defaultSuppressedKeys.Add(SButton.RightThumbstickUp, SuppressMode.DoNotSuppress);
            return defaultSuppressedKeys;
        }
    }
}
