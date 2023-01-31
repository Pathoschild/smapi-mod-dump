/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewModdingAPI.Utilities;

namespace Stackify.Framework;
public sealed class ModConfig
{
    public KeybindList ColorStackBind { get; set; } = KeybindList.Parse("LeftShift");

    public KeybindList QualityStackBind { get; set; } = KeybindList.Parse("LeftAlt");
}
