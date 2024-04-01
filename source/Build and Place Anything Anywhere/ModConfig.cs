/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System;

namespace AnythingAnywhere
{
    internal class ModConfig
    {
        public ModConfigKeys Keys { get; set; } = new();
        
        // FURNITURE
        public bool EnablePlacing { get; set; } = true;
        //public bool EnableRugTweaks { get; set; } = true;
        public bool EnableWallFurnitureIndoors { get; set; } = false;
        public bool EnableFreePlace { get; set; } = false;


        // BUILDING
        public bool EnableBuilding { get; set; } = true;
        public KeybindList BuildMenu { get; set; } = new KeybindList(SButton.OemComma);
        public KeybindList WizardBuildMenu { get; set; } = new KeybindList(SButton.OemPeriod);
        public bool EnableFreeBuild { get; set; } = false;


        // OTHER
        //public KeybindList TableTweakBind { get; set; } = new KeybindList(SButton.LeftShift);
        //public bool EnableTableTweak {  get; set; } = true;
        public bool EnablePlanting { get; set; } = true;
        public bool EnableFruitTreeTweaks { get; set; } = true;
        public bool EnableWildTreeTweaks { get; set; } = false;
        public bool AllowMiniObelisksAnywhere { get; set; } = true;
        public bool EnableJukeboxFunctionality { get; set; } = true;
    }

    internal class ModConfigKeys
    {
        public KeybindList ReloadConfig { get; set; } = new();
    }
}
