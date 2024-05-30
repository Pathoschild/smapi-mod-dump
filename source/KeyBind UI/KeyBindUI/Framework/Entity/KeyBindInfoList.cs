/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/KeyBindUI
**
*************************************************/

using System.Reflection;
using StardewModdingAPI;

namespace KeyBindUI.Framework.Entity;

public record KeyBindInfoList(
    (string Name, IMod Instance) Mod,
    List<(PropertyInfo PropertyInfo, object config)> Infos);