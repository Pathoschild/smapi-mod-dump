/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using AeroCore.API;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System;

namespace AeroCore
{
    internal class Config
    {
        [GMCMRange(0f, 1f)]
        [GMCMInterval(.1f)]
        [GMCMSection("CursorLight")]
        public float CursorLightIntensity { get; set; } = .5f;

        [GMCMSection("CursorLight")]
        public KeybindList CursorLightBind { get; set; } = new();

        [GMCMSection("CursorLight")]
        public bool CursorLightHold { get; set; } = false;
        public KeybindList PlaceBind { get; set; } = new();
        public KeybindList ReloadBind { get; set; } = new();
    }
}
