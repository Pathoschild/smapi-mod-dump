/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class GreenSlime_Step
{
    internal static ConditionalWeakTable<GreenSlime, Holder> Values { get; } = [];

    internal static Point? Get_CurrentStep(this GreenSlime slime)
    {
        return Values.GetValue(slime, Create).Step;
    }

    internal static void Set_CurrentStep(this GreenSlime slime, Point? value)
    {
        Values.GetOrCreateValue(slime).Step = value;
    }

    internal static Holder Create(GreenSlime slime)
    {
        return new Holder { Step = slime.Get_IncrementalPathfinder().Step(slime.TilePoint, slime.Player.TilePoint) };
    }

    internal class Holder
    {
        public Point? Step { get; internal set; }
    }
}
