/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Slingshots;

#region using directives

using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

/// <summary>The runtime state for Slingshot variables.</summary>
internal sealed class State
{
    internal int SlingshotCooldown { get; set; }

    internal Vector2 DriftVelocity { get; set; }

    internal Slingshot? AutoSelectableSlingshot { get; set; }
}
