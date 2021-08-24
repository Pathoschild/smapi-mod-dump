/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AdvizeGH/FarmExpansion
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using FarmExpansion.Framework;
using System;
using System.Collections.Generic;

namespace FarmExpansion
{

    public class ModEntry : Mod, IAssetEditor
    {
        private FEFramework framework;
        public static IModHelper ModHelper;

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(@"Data\Locations") || asset.AssetNameEquals(@"LooseSprites\map");
        }

        public void Edit<T>(IAssetData asset)
        {
            switch (asset.AssetName)
            {
                case @"Data\Locations":
                    asset.AsDictionary<string, string>().Data.Add(new KeyValuePair<string, string>("FarmExpansion", "-1/-1/-1/-1/-1/-1/-1/-1/382 .05 770 .1 390 .25 330 1"));
                    break;
                case @"LooseSprites\map":
                    Texture2D customTexture;
                    try
                    {
                        customTexture = this.Helper.Content.Load<Texture2D>(@"assets\WorldMap.png");

                        Rectangle defaultFarm = new Rectangle(32, 64, 35, 25);
                        Rectangle fishingFarm = new Rectangle(32, 203, 34, 23);
                        Rectangle foragingFarm = new Rectangle(163, 202, 35, 24);
                        Rectangle miningFarm = new Rectangle(32, 262, 34, 27);
                        Rectangle combatFarm = new Rectangle(163, 262, 34, 25);
                        
                        if (customTexture != null)
                        {
                            asset.AsImage().PatchImage(customTexture, defaultFarm, defaultFarm);
                            asset.AsImage().PatchImage(customTexture, fishingFarm, fishingFarm);
                            asset.AsImage().PatchImage(customTexture, foragingFarm, foragingFarm);
                            asset.AsImage().PatchImage(customTexture, miningFarm, miningFarm);
                            asset.AsImage().PatchImage(customTexture, combatFarm, combatFarm);
                        }
                    }
                    catch(Exception ex)
                    {
                        this.Monitor.Log($"Could not load WorldMap.png from the mod folder, world map will not be patched.\n{ex}", LogLevel.Error);
                    }
                    break;
            }
        }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            framework = new FEFramework(helper, Monitor);
            framework.IsTreeTransplantLoaded = helper.ModRegistry.IsLoaded("TreeTransplant");// && helper.ModRegistry.Get("TreeTransplant").Version.IsNewerThan("1.0.0");
            helper.Events.Display.MenuChanged += framework.OnMenuChanged;
            helper.Events.GameLoop.SaveLoaded += framework.OnSaveLoaded;
            helper.Events.GameLoop.Saving += framework.OnSaving;
            helper.Events.GameLoop.Saved += framework.OnSaved;
            helper.Events.GameLoop.ReturnedToTitle += framework.OnReturnedToTitle;
            helper.Events.GameLoop.DayStarted += framework.OnDayStarted;
        }

        /// <summary>Get an API that other mods can access. This is always called after <see cref="Entry" />.</summary>
        public override object GetApi()
        {
            return new ModApi(this.framework);
        }
    }
}