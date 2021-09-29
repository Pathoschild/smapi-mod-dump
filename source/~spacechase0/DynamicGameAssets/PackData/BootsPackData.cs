/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.ComponentModel;
using DynamicGameAssets.Game;
using Newtonsoft.Json;
using SpaceShared;
using StardewValley;

namespace DynamicGameAssets.PackData
{
    public class BootsPackData : CommonPackData
    {
        [JsonIgnore]
        public string Name => this.pack.smapiPack.Translation.Get($"boots.{this.ID}.name");

        [JsonIgnore]
        public string Description => this.pack.smapiPack.Translation.Get($"boots.{this.ID}.description");

        public string Texture { get; set; }
        public string FarmerColors { get; set; }

        [DefaultValue(0)]
        public int Defense { get; set; }

        [DefaultValue(0)]
        public int Immunity { get; set; }

        [DefaultValue(0)]
        public int SellPrice { get; set; }


        public override TexturedRect GetTexture()
        {
            return this.pack.GetTexture(this.Texture, 16, 16);
        }

        public override void OnDisabled()
        {
            SpaceUtility.iterateAllItems((item) =>
           {
               if (item is CustomBoots cboots)
               {
                   if (cboots.SourcePack == this.pack.smapiPack.Manifest.UniqueID && cboots.Id == this.ID)
                       return null;
               }
               return item;
           });
        }

        public override Item ToItem()
        {
            return new CustomBoots(this);
        }
    }
}
