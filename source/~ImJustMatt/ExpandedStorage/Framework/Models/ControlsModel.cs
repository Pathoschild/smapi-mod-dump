/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using System.Collections.Generic;
using Helpers.ConfigData;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace ExpandedStorage.Framework.Models
{
    internal class ControlsModel
    {
        internal static readonly ConfigHelper ConfigHelper = new(new ControlsModel(), new List<KeyValuePair<string, string>>
        {
            new("OpenCrafting", "Open the crafting menu using inventory from a held storage"),
            new("ScrollUp", "Button for scrolling the item storage menu up one row"),
            new("ScrollDown", "Button for scrolling the item storage menu down one row"),
            new("PreviousTab", "Button for switching to the previous tab"),
            new("NextTab", "Button for switching to the next tab")
        });

        public KeybindList OpenCrafting { get; set; } = new(SButton.K);
        public KeybindList ScrollUp { get; set; } = new(SButton.DPadUp);
        public KeybindList ScrollDown { get; set; } = new(SButton.DPadDown);
        public KeybindList PreviousTab { get; set; } = new(SButton.DPadLeft);
        public KeybindList NextTab { get; set; } = new(SButton.DPadRight);
    }
}