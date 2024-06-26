/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MartyrPher/GetGlam
**
*************************************************/

using StardewModdingAPI;

namespace GetGlam.Framework
{
    /// <summary>The mods Config</summary>
    public class ModConfig
    {
        // Key to open the Glam Menu
        public SButton OpenGlamMenuKey { get; set; } = SButton.C;

        // Setting that sets new hats to ignore the hair
        public bool NewHatsIgnoreHair { get; set; } = false;

        // The dressers location x
        public int DresserTableLocationX { get; set; } = 0;

        // The dressers location y
        public int DresserTableLocationY { get; set; } = 0;

        // Whether to draw the dresser over top the player in the menu
        public bool DrawDresserInMenu { get; set; } = true;

        // Whether to patch the dresser in the FarmHouse
        public bool PatchDresserInFarmHouse { get; set; } = true;

        // Whether there is a stove of the corner added by another mod
        public bool StoveInCorner { get; set; } = true;

        // Whether to Patch Farmer Face And Nose
        public bool PatchFaceNose { get; set; } = true;

        // Whether to Patch Farmer Shoes
        public bool PatchShoes { get; set; } = true;
    }
}
