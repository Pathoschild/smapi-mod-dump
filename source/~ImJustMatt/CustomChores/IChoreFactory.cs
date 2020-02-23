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
