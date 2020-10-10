/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

namespace UltimateTool.Framework.Configuration
{
   internal class ModConfig
    {
        //public bool ModEnabled { get; set; } = false;
        public string ActionKey { get; set; } = "Z";
        public int MagnetRadius { get; set; } = 10;
        public int ToolRadius { get; set; } = 1;
        public int ToolLevel { get; set; } = 1;
        public bool ShowGrid { get; set; } = true;
        public ToolConfig Tools { get; set; } = new ToolConfig();
    }
}
