/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DraconisLeonidas/PressToQuack
**
*************************************************/

using StardewModdingAPI;
using System;

namespace PressToQuack
{
    /* (Regular Config- No Generic Mod Config Menu support)
    //internal class ModConfig
    //{
    //    public SButton QuackButton { get; set; } = SButton.Q;
    } */

    public class ModConfig
    {
        // Add option for Quacking keybind
        public SButton QuackButton { get; set; }

        public ModConfig()
        {
            // Default button = Q
            this.QuackButton = SButton.Q;
        }
    }

}
