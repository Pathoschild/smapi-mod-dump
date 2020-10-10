/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sakorona/SDVMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Reflection;

namespace SummitReborn
{
    public class SummitConfig
    {
        public bool Clouds = true;
    }

    public class SummitReborn : Mod, IAssetLoader, IAssetEditor
    {
        private float weatherX;
        private SummitConfig ModConfig;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModConfig = Helper.ReadConfig<SummitConfig>();
            helper.Events.GameLoop.GameLaunched += OnLaunched;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.Display.RenderingHud += OnRenderingHudEvent;
        }

        private void OnLaunched(object sender, GameLaunchedEventArgs e)
        {
            var api = Helper.ModRegistry.GetApi<Integrations.GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (api != null)
            {
                api.RegisterModConfig(ModManifest, () => ModConfig = new SummitConfig(), () => Helper.WriteConfig(ModConfig));
                api.RegisterSimpleOption(ModManifest, "Clouds", "Displays clouds in the summit", () => ModConfig.Clouds, (bool val) => ModConfig.Clouds = val);
            }
        }

        /// <summary>
        /// This is used to tell SMAPI what asset this mod will edit.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asset"></param>
        /// <returns></returns>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Data/Locations"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// This is used to tell SMAPI what is being edited in the asset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asset"></param>
        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data/Locations"))
            {
                asset.AsDictionary<string,string>().Data.Add("Summit","18 .9 20 .4 22 .7/396 .4 398 .4 402 .7/406 .6 408 .4 410 .6/414 .8 418 .8/-1/-1/-1/-1/580 .1 378 .15 102 .19 390 .25 330 1 114 .007 108 .0006 126 .0001 106 .003 103 .007 120 .01 105 .01");
            }
        }

        /// <summary>Get whether this instance can load the initial version of the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanLoad<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(@"Maps\Railroad") || asset.AssetNameEquals(@"Maps\Summit");
        }

        /// <summary>Load a matched asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public T Load<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals(@"Maps\Railroad"))
                return Helper.Content.Load<T>(@"assets\Railroad_alt.tbin");
            if (asset.AssetNameEquals(@"Maps\Summit"))
                return Helper.Content.Load<T>(@"assets\Summit_alt.tbin");
            throw new NotSupportedException($"Unexpected asset name: {asset.AssetName}");
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (Game1.currentGameTime != null)
            {
                weatherX += Game1.currentGameTime.ElapsedGameTime.Milliseconds * 0.03f;
            }
        }

        private static int GetPixelZoom()
        {
            FieldInfo field = typeof(Game1).GetField(nameof(Game1.pixelZoom), BindingFlags.Public | BindingFlags.Static);
            if (field == null)
                throw new InvalidOperationException($"The {nameof(Game1)}.{nameof(Game1.pixelZoom)} field could not be found.");
            return (int)field.GetValue(null);
        }

        /// <summary>Raised before drawing the HUD (item toolbar, clock, etc) to the screen. The vanilla HUD may be hidden at this point (e.g. because a menu is open).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnRenderingHudEvent(object sender, RenderingHudEventArgs e)
        {
            //draw weather in the summit map
            if (Game1.isRaining && Game1.currentLocation.IsOutdoors && (Game1.currentLocation is Summit) && (!Game1.eventUp || Game1.currentLocation.isTileOnMap(new Vector2(Game1.viewport.X / Game1.tileSize, Game1.viewport.Y / Game1.tileSize))))
            {
                for (int index = 0; index < Game1.rainDrops.Length; ++index)
                {
                    Game1.spriteBatch.Draw(Game1.rainTexture, Game1.rainDrops[index].position, new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.rainTexture, Game1.rainDrops[index].frame, -1, -1)), Color.White);
                }

                if (ModConfig.Clouds)
                {
                    int num2 = -61 * GetPixelZoom();
                    while (num2 < Game1.viewport.Width + 61 * GetPixelZoom())
                    {
                        Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(num2 + weatherX % (61 * GetPixelZoom()), -Game1.tileSize / 2), new Rectangle?(new Rectangle(643, 1142, 61, 53)), Color.DarkSlateGray * 1f, 0.0f, Vector2.Zero, GetPixelZoom(), SpriteEffects.None, 1f);
                        num2 += 61 * GetPixelZoom();
                    }
                }
            }
        }
    }
}
