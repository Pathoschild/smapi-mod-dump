/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI.Events;

namespace CustomMovies
{
    static class CMVAssetEditor
    {
        public static CustomMovieData CurrentMovie { get; set; } = null;

        public static void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (CurrentMovie is not null && e.Name.IsEquivalentTo("LooseSprites/Movies"))
                e.Edit(asset => asset.ReplaceWith(CurrentMovie._texture));
        }
    }
}
