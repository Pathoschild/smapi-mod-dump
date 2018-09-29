using System.IO;
using System.Linq;
using Igorious.StardewValley.DynamicApi2.Constants;
using Igorious.StardewValley.DynamicApi2.Data;
using Igorious.StardewValley.DynamicApi2.Extensions;
using Igorious.StardewValley.DynamicApi2.Services;
using Igorious.StardewValley.ShowcaseMod.Constants;
using Igorious.StardewValley.ShowcaseMod.Core;
using Igorious.StardewValley.ShowcaseMod.ModConfig;
using StardewModdingAPI;

namespace Igorious.StardewValley.ShowcaseMod
{
    public class ShowcaseMod : Mod
    {
        public static ShowcaseModConfig Config { get; } = new ShowcaseModConfig();
        public static TextureModule TextureModule { get; set; }
        private static readonly string ResourcesFolder = "Resources";

        public override void Entry(IModHelper helper)
        {
            Config.Load(Helper.DirectoryPath);
            Config.Showcases.ForEach(s => ClassMapper.Instance.Map.Furniture<Showcase>(s.ID));
            Config.Showcases.ForEach(s => DataService.Instance.RegisterFurniture(ConfigDataConverter.ToFurnitureData(s)));
            Config.Showcases.ForEach(s => ShopService.Instance.AddFurniture(Locations.CarpentersShop, new ShopItemInfo(s.ID)));

            TextureModule = TextureService.Instance.RegisterModule(Path.Combine(Helper.DirectoryPath, ResourcesFolder))
                .LoadTexture(TextureNames.Glow)
                .LoadTexture(TextureNames.Furniture);

            this.RegisterCommands();
        }

        public static ShowcaseConfig GetShowcaseConfig(int id)
        {
            return Config.Showcases.First(c => c.ID == id);
        }
    }
}