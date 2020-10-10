/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System.Globalization;
using System.Threading;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Services;
using StardewModdingAPI;

namespace Igorious.StardewValley.MoreCropsLoader
{
    public sealed class MoreCropsLoaderMod : Mod
    {
        public static MoreCropsLoaderModConfig Config { get; } = new MoreCropsLoaderModConfig();

        public override void Entry(params object[] objects)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Config.Load(PathOnDisk);

            InitializeObjectInformation();
            OverrideTextures();
        }

        private static void InitializeObjectInformation()
        {
            Config.Items.ForEach(InformationService.Instance.Register);
            Config.Trees.ForEach(InformationService.Instance.Register);
            Config.Crops.ForEach(InformationService.Instance.Register);
        }

        private void OverrideTextures()
        {
            var textureService = new TexturesService(PathOnDisk);
            Config.Items.ForEach(i => textureService.Override(TextureType.Items, i));
            Config.Crops.ForEach(i => textureService.Override(TextureType.Crops, i));
            Config.Trees.ForEach(i => textureService.Override(TextureType.Trees, i));
        }
    }
}
