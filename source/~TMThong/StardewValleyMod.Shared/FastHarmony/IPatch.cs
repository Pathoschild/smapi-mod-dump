/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace StardewValleyMod.Shared.FastHarmony
{
    public interface IPatch
    {
        public Type TypePatch { get; }
        void Apply(Harmony harmony);
    }
}
