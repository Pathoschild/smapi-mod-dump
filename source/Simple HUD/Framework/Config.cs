/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/SimpleHUD
**
*************************************************/

using EnaiumToolKit.Framework.Utils;
using StardewModdingAPI;

namespace SimpleHUD.Framework
{
    public class Config
    {
        public SButton OpenSetting { get; set; } = SButton.H;
        public bool Enable { get; set; } = true;
        public string Title { get; set; } = "Simple HUD";
        
        public ColorUtils.NameType TextColor = ColorUtils.NameType.Aqua;
    }
}