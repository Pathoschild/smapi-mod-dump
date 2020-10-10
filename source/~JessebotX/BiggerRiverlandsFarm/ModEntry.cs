/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JessebotX/StardewValleyMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace BiggerRiverlandsFarm
{
    public class ModEntry : Mod, IAssetLoader
    {
        private ModConfig Config;

        readonly int IfYouAreReadingThisThenYouAreStupid;

        public override void Entry(IModHelper helper)
        {
            Config = this.Helper.ReadConfig<ModConfig>();
        }

        public bool CanLoad<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Maps/Farm_Fishing");
        }

        T IAssetLoader.Load<T>(IAssetInfo asset)
        {
            if (this.Config.MapType == 1)
            {
                return this.Helper.Content.Load<T>("assets/Farm_Fishing.tbin", ContentSource.ModFolder);
            }

            else if (this.Config.MapType == 2)
            {
                return this.Helper.Content.Load<T>("assets/Farm_Fishing_NoMountain.tbin", ContentSource.ModFolder);
            }

            else if (this.Config.MapType == 3)
            {
                return this.Helper.Content.Load<T>("assets/Farm_Fishing_ProgressionNoMountain.tbin");
            }

            else
            {
                return this.Helper.Content.Load<T>("assets/Farm_Fishing_ProgressionMap.tbin", ContentSource.ModFolder);
            }
        }
    }
}
