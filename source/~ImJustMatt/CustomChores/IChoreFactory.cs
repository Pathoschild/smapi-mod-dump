/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/SDVCustomChores
**
*************************************************/

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using LeFauxMatt.CustomChores.Models;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace LeFauxMatt.CustomChores
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public interface IChoreFactory
    {
        IChore GetChore(ChoreData choreData);
    }
}
