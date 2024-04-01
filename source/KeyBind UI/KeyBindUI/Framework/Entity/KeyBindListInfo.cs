/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/KeyBindUI
**
*************************************************/

namespace KeyBindUI.Framework.Entity;

public class KeyBindListInfo
{
    public string Name { get; set; }
    public FileInfo ConfigFileInfo { get; set; }
    public Dictionary<string, string> KeyBindList { get; set; } = new();
}