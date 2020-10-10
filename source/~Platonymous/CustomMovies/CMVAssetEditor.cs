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
using System;

namespace CustomMovies
{
    class CMVAssetEditor : IAssetEditor
    {
        public static CustomMovieData CurrentMovie { get; set; } = null;
        private IModHelper helper;

        public CMVAssetEditor(IModHelper helper)
        {
            this.helper = helper;
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(@"LooseSprites\Movies");
        }

        public void Edit<T>(IAssetData asset)
        {
            if (CurrentMovie != null)
                asset.ReplaceWith(CurrentMovie._texture);
        }

    }
}
