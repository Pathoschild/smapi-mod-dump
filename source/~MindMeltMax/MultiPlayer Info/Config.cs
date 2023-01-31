/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;
using System;

namespace MPInfo 
{
    public class Config 
    {
        public bool Enabled { get; set; } = true;
        public bool ShowSelf { get; set; } = false;
        public bool ShowHostCrown { get; set; } = true;
        public bool HideHealthBars { get; set; } = false;
    }

    public interface IGenericModConfigMenuApi 
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string>? tooltip = null, string? fieldId = null);
    }
}
