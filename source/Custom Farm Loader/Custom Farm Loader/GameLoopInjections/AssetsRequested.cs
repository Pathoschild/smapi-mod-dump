/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-custom-farm-loader
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using StardewValley;
using System.Xml;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using System.Reflection;
using Custom_Farm_Loader.Lib;
using StardewValley.Network;
using StardewValley.GameData;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Custom_Farm_Loader.GameLoopInjections
{
    public class AssetsRequested
    {
        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        public static void Initialize(Mod mod)
        {
            Mod = mod;
            Monitor = mod.Monitor;
            Helper = mod.Helper;

            Helper.Events.Content.AssetRequested += OnAssetRequested;
        }

        private static void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/AdditionalFarms"))
            {
                //List of all modded farms was requested.
                //We just append our own

                e.Edit(edit =>
                {
                    var data = edit.GetData<List<ModFarmType>>();
                    data.AddRange(CustomFarm.getAllAsModFarmType());
                });
                return;
            }

            if (e.NameWithoutLocale.BaseName.Contains("Strings/UI"))
            {
                e.Edit(edit =>
                {
                    var data = edit.AsDictionary<string, string>().Data;
                    List<CustomFarm> customFarms = CustomFarm.getAll();

                    foreach (CustomFarm customFarm in customFarms)
                    {
                        KeyValuePair<string, string> localization = customFarm.TranslatedDescriptions.Find(el => el.Key == e.Name.LocaleCode || el.Key == e.Name.LanguageCode.ToString());
                        string localizedDescription = localization.Key == null ? customFarm.Description : localization.Value;

                        data.Add(new KeyValuePair<string, string>($"CFL_Description/{customFarm.ID}", localizedDescription));
                    }
                });
            }

            if (e.NameWithoutLocale.BaseName.StartsWith("CFL_Icon/"))
            {
                string id = e.NameWithoutLocale.BaseName.Split("CFL_Icon/")[1];
                CustomFarm customFarm = CustomFarm.get(id);

                e.LoadFrom(delegate () {
                    if (customFarm.Icon != null)
                        return customFarm.Icon;
                    else
                        return customFarm.loadIconTexture();
                }, AssetLoadPriority.Low);

                return;
            }

            if (e.NameWithoutLocale.BaseName.StartsWith("CFL_WorldMap/"))
            {
                string id = e.NameWithoutLocale.BaseName.Split("CFL_WorldMap/")[1];
                CustomFarm customFarm = CustomFarm.get(id);

                e.LoadFrom(delegate () {
                    return customFarm.loadWorldMapTexture();
                }, AssetLoadPriority.Low);

                return;
            }

            if (e.NameWithoutLocale.BaseName.Contains("CFL_Map/"))
            {
                string id = e.NameWithoutLocale.BaseName.Split("CFL_Map/")[1];
                CustomFarm customFarm = CustomFarm.get(id);

                e.LoadFromModFile<xTile.Map>($"{customFarm.RelativeContentPackPath}{customFarm.RelativeMapDirectoryPath}{Path.DirectorySeparatorChar}{customFarm.MapFile}", AssetLoadPriority.Exclusive);

                return;
            }

            if (e.NameWithoutLocale.BaseName == Bridge.BridgesTilesheet)
                e.LoadFromModFile<Texture2D>("assets/CFL_Bridges", AssetLoadPriority.Medium);
        }
    }
}
