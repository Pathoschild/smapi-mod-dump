/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Mini-Bars
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System;
using System.Threading;

namespace MiniBars.Framework.Rendering
{
    public class Textures
    {
        private static IMonitor Monitor = ModEntry.instance.Monitor;
        public static List<BarInformations> barInformations = new List<BarInformations>();
        public static Texture2D hpSprite;

        private static Lazy<Texture2D> _pixelLazy = new(() =>
        {
            Texture2D _pixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
            return _pixel;
        });

        public static Texture2D Pixel => _pixelLazy.Value;

        public static void LoadTextures()
        {
            barInformations.Clear();

            IModHelper _helper = ModEntry.instance.Helper;

            Database.GetTheme();

            DirectoryInfo _dir = new DirectoryInfo($"{_helper.DirectoryPath}/assets/{Database.bars_theme}/");
            foreach (FileInfo _file in _dir.GetFiles("*.png"))
            {
                string _fileName = (_file.Name).Replace(".png", "");

                BarInformations _informations;
                _informations = ModEntry.instance.Helper.Data.ReadJsonFile<BarInformations>($"assets/informations/{_fileName}.json") ?? new BarInformations();
                _informations.texture = ModEntry.instance.Helper.ModContent.Load<Texture2D>($"assets/{Database.bars_theme}/{_file.Name}");

                barInformations.Add(_informations);
                Monitor.Log($"Loaded informations from: {_fileName}", LogLevel.Trace);
            }
            hpSprite = ModEntry.instance.Helper.ModContent.Load<Texture2D>($"assets/hp_sprite.png");

            BarInformations _defaultTheme;
            _defaultTheme = ModEntry.instance.Helper.Data.ReadJsonFile<BarInformations>($"assets/informations/default_theme.json") ?? new BarInformations();
            barInformations.Add(_defaultTheme);
        }
    }
}
