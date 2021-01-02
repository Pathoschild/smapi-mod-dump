/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MartyrPher/GetGlam
**
*************************************************/

using GetGlam.Framework.Patches;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Reflection;

namespace GetGlam.Framework
{
    public class HarmonyHelper
    {
        // Instance of Harmony
        private HarmonyInstance Harmony;

        // The mods entry
        private ModEntry Entry;

        /// <summary>
        /// Constructor - Used for all Harmony related patching.
        /// </summary>
        /// <param name="entry">The Mod's Entry class.</param>
        public HarmonyHelper(ModEntry entry)
        {
            Entry = entry;
        }

        /// <summary>
        /// Initializes the Harmony Instance and starts the patches.
        /// </summary>
        public void InitializeAndPatch()
        {
            Harmony = HarmonyInstance.Create(Entry.ModManifest.UniqueID);
            PatchWithHarmony();
        }

        /// <summary>
        /// Harmony patch for accessory length and skin color length.
        /// </summary>
        private void PatchWithHarmony()
        {
            // Patch accessory length
            PatchChangeAccessory();

            // Patch the skin color length
            PatchSkinColor();
        }

        /// <summary>
        /// Patches changeAccessory using a harmony transpiler.
        /// </summary>
        private void PatchChangeAccessory()
        {
            AccessoryPatch accessoryPatch = new AccessoryPatch(Entry);

            Entry.Monitor.Log("Patching changeAccessory()", LogLevel.Trace);
            Harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.changeAccessory)),
                transpiler: new HarmonyMethod(accessoryPatch.GetType(), nameof(AccessoryPatch.ChangeAccessoryTranspiler))
            );
        }

        /// <summary>
        /// Patches changeSkinColor using a harmony transpiler.
        /// </summary>
        private void PatchSkinColor()
        {
            SkinColorPatch skinColorPatch = new SkinColorPatch(Entry);

            Entry.Monitor.Log("Patching changeSkinColor()", LogLevel.Trace);
            Harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.changeSkinColor)),
                transpiler: new HarmonyMethod(skinColorPatch.GetType(), nameof(SkinColorPatch.ChangeSkinColorTranspiler))
            );
        }

        /// <summary>
        /// Patches SpaceCore for extended hairstyles.
        /// </summary>
        public void SpaceCorePatchHairStyles()
        {
            Assembly spaceCoreAssembly = GetSpaceCoreAssembly();
            MethodInfo registerTileSheet = spaceCoreAssembly.GetType("SpaceCore.TileSheetExtensions").GetMethod("RegisterExtendedTileSheet");
            registerTileSheet.Invoke(null, new object[] { "Characters\\Farmer\\hairstyles", 96 });
        }
        
        /// <summary>
        /// Patches SpaceCore extended tilesheets.
        /// </summary>
        /// <param name="asset"> The asset to change.</param>
        /// <param name="sourceTexture"> The source texture.</param>
        /// <param name="sourceRect"> Source Rectangle of source texture.</param>
        /// <param name="targetRect"> Source Rectangle of target texture.</param>
        public void SpaceCorePatchExtendedTileSheet(IAssetDataForImage asset, Texture2D sourceTexture, Rectangle sourceRect, Rectangle targetRect)
        {
            Assembly spaceCoreAssembly = GetSpaceCoreAssembly();
            MethodInfo patchExtendedTileSheet = spaceCoreAssembly.GetType("SpaceCore.TileSheetExtensions").GetMethod("PatchExtendedTileSheet");
            patchExtendedTileSheet.Invoke(null, new object[] { asset, sourceTexture, sourceRect, targetRect, PatchMode.Replace });
        }

        /// <summary>
        /// Gets the SpaceCore Assembly.
        /// </summary>
        /// <returns> The SpaceCore Assembly.</returns>
        private Assembly GetSpaceCoreAssembly()
        {
            IModInfo modData = Entry.Helper.ModRegistry.Get("spacechase0.SpaceCore");
            object spaceCoreInstance = modData.GetType().GetProperty("Mod", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public).GetValue(modData);
            return spaceCoreInstance.GetType().Assembly;
        }

        /// <summary>
        /// Creates an Instance of the CustomizeAnywhere Menu.
        /// </summary>
        public void CustomizeAnywhereClothingMenu()
        {
            var modData = Entry.Helper.ModRegistry.Get("Cherry.CustomizeAnywhere");
            var customizeInstance = modData.GetType().GetProperty("Mod", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public).GetValue(modData);
            var customizeAssembly = customizeInstance.GetType().Assembly;
            var dresserMenu = customizeAssembly.GetType("CustomizeAnywhere.DresserMenu");
            Game1.activeClickableMenu = (IClickableMenu)Activator.CreateInstance(dresserMenu);
        }
    }
}
