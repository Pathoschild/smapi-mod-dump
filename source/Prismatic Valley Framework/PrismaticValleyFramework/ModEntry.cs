/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Jolly-Alpaca/PrismaticValleyFramework
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using PrismaticValleyFramework.Patches;
using PrismaticValleyFramework.Models;
using Microsoft.Xna.Framework.Graphics;

namespace PrismaticValleyFramework
{
    internal sealed class ModEntry : Mod
    {
        internal static IMonitor ModMonitor { get; private set; } = null!;

        internal static IModHelper ModHelper { get; private set; } = null!;
        internal static Harmony Harmony { get; private set; } = null!;

        public override void Entry(IModHelper helper)
        {
            ModMonitor = Monitor;
            ModHelper = helper;
            
            // Apply Harmony patches
            var harmony = new Harmony(this.ModManifest.UniqueID);
            //Harmony.DEBUG = true;

            // Apply FarmAnimal patches
            FarmAnimalPatcher.Apply(ModMonitor, harmony);
            AnimalPagePatcher.Apply(ModMonitor, harmony);
            //CharacterPatcher.Apply(ModMonitor, harmony);

            // Apply object, big craftable, and boots patches
            ObjectPatcher.Apply(ModMonitor, harmony);
            FurniturePatcher.Apply(ModMonitor, harmony);
            CraftingPagePatcher.Apply(ModMonitor, harmony);
            CollectionsPagePatcher.Apply(ModMonitor, harmony);
            LibraryMuseumPatcher.Apply(ModMonitor, harmony);
            BootsPatcher.Apply(ModMonitor, harmony);
            FarmerRendererPatcher.Apply(ModMonitor, harmony);

            ModHelper.Events.Content.AssetRequested += OnAssetRequested;
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            // Load custom boot data (equivalent to CustomFields field for data structures that don't have a CustomFields field)
            if (e.NameWithoutLocale.IsEquivalentTo("JollyLlama.PrismaticValleyFramework"))
            {
                e.LoadFrom(() => new Dictionary<string, ModColorData>(), AssetLoadPriority.High);
            }

            // Load male shoe texture
            if (e.NameWithoutLocale.IsEquivalentTo("JollyLlama.PrismaticValleyFramework/farmer_shoes"))
            {
                e.LoadFromModFile<Texture2D>("Assets/farmer_shoes.png", AssetLoadPriority.Medium);
            }
            // Load female shoe texture
            if (e.NameWithoutLocale.IsEquivalentTo("JollyLlama.PrismaticValleyFramework/farmer_girl_shoes"))
            {
                e.LoadFromModFile<Texture2D>("Assets/farmer_girl_shoes.png", AssetLoadPriority.Medium);
            }
        }
    }
}
