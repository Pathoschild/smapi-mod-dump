/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Yariazen/BuildableGreenhouse
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;

namespace BuildableGreenhouse.Compatibility
{
    partial class ModCompatibility
    {
        public static void applyGMCMCompatibility(object sender, GameLaunchedEventArgs e)
        {
            if (Helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu"))
            {
                Monitor.Log("Applying GMCM Compatibility");
                gmcmCompatibility();
            }
        }

        private static void gmcmCompatibility()
        {
            IGenericModConfigMenuApi configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (configMenu is null)
                return;

            Texture2D springObjects = Game1.objectSpriteSheet;

            configMenu.Register(
                mod: Manifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config),
                titleScreenOnly: true
            );

            configMenu.AddParagraph(
                mod: Manifest,
                text: () => "This is a very rudimentary config menu. To change the build materials itselves rather than just the amount, the only way is to edit the config.json file. This may change in the future."
            );

            configMenu.AddBoolOption(
                mod: Manifest,
                getValue: () => Config.StartWithGreenhouse,
                setValue: value => Config.StartWithGreenhouse = value,
                name: () => "Start With Greenhouse",
                tooltip: () => "True to start with the buildable greenhouse; False to unlock the buildable greenhouse when you unlock the greenhouse"
            );

            configMenu.AddNumberOption(
                mod: Manifest,
                getValue: () => Config.BuildPrice,
                setValue: value => Config.BuildPrice = value,
                name: () => "Build Price",
                tooltip: () => "This is the price to build a greenhouse"
            );

            configMenu.AddSectionTitle(
                mod: Manifest,
                text: () => "Build Materials",
                tooltip: () => "These are the materials and amounts needed to build a greenhouse"
            );

            var buildMaterials = Config.BuildMaterals;

            foreach (var buildMaterial in buildMaterials)
            {
                int objectIndex = buildMaterial.Key;

                configMenu.AddImage(
                    mod: Manifest,
                    texture: () => springObjects,
                    texturePixelArea: Game1.getSourceRectForStandardTileSheet(springObjects, objectIndex, 16, 16),
                    scale: 4
                );

                configMenu.AddNumberOption(
                    mod: Manifest,
                    getValue: () => objectIndex,
                    setValue: value => { },
                    name: () => "Material Id"
                );

                configMenu.AddNumberOption(
                    mod: Manifest,
                    getValue: () => buildMaterials[objectIndex],
                    setValue: value => buildMaterials[objectIndex] = value,
                    name: () => "Material Count"
                );
            }
        }   
    }

    public interface IGenericModConfigMenuApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);
        void AddSectionTitle(IManifest mod, Func<string> text, Func<string> tooltip = null);
        void AddParagraph(IManifest mod, Func<string> text);
        void AddImage(IManifest mod, Func<Texture2D> texture, Rectangle? texturePixelArea = null, int scale = Game1.pixelZoom);
        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
        void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string> tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string> formatValue = null, string fieldId = null);
        void OnFieldChanged(IManifest mod, Action<string, object> onChange);
    }
}
