/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/TeleportPoint
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using TeleportPoint.Framework.Gui;

namespace TeleportPoint.Framework;

public class Config
{
    public KeybindList OpenTeleport { get; set; } = new(SButton.L);
    public List<TeleportData> TeleportData { get; set; } = new();
}