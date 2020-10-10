/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using StardewValley.GameData.Movies;
using System.Collections.Generic;

namespace CustomMovies
{
    public class CustomMoviePack
    {
        public List<CustomMovieData> Movies { get; set; } = new List<CustomMovieData>();
        public List<MovieCharacterReaction> Reactions { get; set; } = new List<MovieCharacterReaction>();
    }
}
