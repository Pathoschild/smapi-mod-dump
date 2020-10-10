/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;
using StardewValley.GameData.Movies;
using System.Collections.Generic;

namespace CustomMovies
{
    public class TranslatableMovieReactions
    {
        public MovieCharacterReaction Reaction { get; set; }

        public IContentPack _pack { get; set; }

        public TranslatableMovieReactions(MovieCharacterReaction reaction, IContentPack pack)
        {
            Reaction = reaction;
            _pack = pack;
        }
    }
}
