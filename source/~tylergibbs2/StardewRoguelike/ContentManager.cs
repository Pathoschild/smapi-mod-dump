/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;

namespace StardewRoguelike
{
    internal static class ContentManager
    {

        /// <summary>
        /// All assets that have been modified.
        /// The dictionary key is the name of the asset as the game requests it,
        /// the value is the filepath of the asset to replace it with.
        /// </summary>
        private static readonly Dictionary<string, string> ModifiedAssets = new()
        {
            { "Maps/Mine", "assets/Maps/custom-lobby.tmx" },
            { "Maps/Festivals", "assets/Maps/Festivals.png" },
            { "TileSheets/Projectiles", "assets/TileSheets/Projectiles.png" },
            { "TileSheets/HatBoard", "assets/TileSheets/HatBoard.png" },
            { "LooseSprites/Cursors", "assets/TileSheets/Cursors.png" },
            { "LooseSprites/ForgeMenu", "assets/TileSheets/ForgeMenu.png" },
            { "TerrainFeatures/CleansingCauldron", "assets/TileSheets/Cauldron.png" },
            { "TerrainFeatures/CleansingCauldronEmpty", "assets/TileSheets/CauldronEmpty.png" },
            { "TerrainFeatures/SpeedPad", "assets/TileSheets/speedrun_pads.png" }
        };

        private static readonly Dictionary<string, string> ModifiedStrings = new()
        {
            { "OptionsPage.cs.11281", "Access Perks" }
        };

        private static readonly Dictionary<int, string> ModifiedObjectInformation = new()
        {
            { 896, "Galaxy Soul/2000/-300/Crafting -2/Galaxy Soul/Forge 3 of these into a Galaxy weapon to unleash its final form." },
            { 472, "Parsnip Seeds/50/-300/Seeds -74/Parsnip Seeds/Takes 6 floors to mature." },
            { 479, "Melon Seeds/100/-300/Seeds -74/Melon Seeds/Takes 12 floors to mature." },
            { 490, "Pumpkin Seeds/150/-300/Seeds -74/Pumpkin Seeds/Takes 18 floors to mature." },
            { 486, "Starfruit Seeds/200/-300/Seeds -74/Starfruit Seeds/Takes 24 floors to mature." },
            { 347, "Rare Seed/250/-300/Seeds -74/Rare Seed/Takes 36 floors to mature." },
            { 24,  "Parsnip/100/10/Basic -75/Parsnip/A spring tuber closely related to the carrot. It has an earthy taste and is full of nutrients." },
            { 254, "Melon/475/45/Basic -79/Melon/A cool, sweet summer treat." },
            { 276, "Pumpkin/600/-300/Basic -75/Pumpkin/A fall favorite, grown for its crunchy seeds and delicately flavored flesh." },
            { 268, "Starfruit/725/50/Basic -79/Starfruit/An extremely juicy fruit that grows in hot, humid weather. Slightly sweet with a sour undertone." },
            { 417, "Sweet Gem Berry/850/-300/Basic -17/Sweet Gem Berry/It's by far the sweetest thing you've ever smelled." }
        };

        /// <summary>
        /// Custom audio files that get loaded into the game.
        /// </summary>
        /// The tuples in this list are in the format (name, category, filename, loop).
        private static readonly List<(string, string, string, bool)> CustomAudio = new()
        {
            ("bee_boss", "Music", "bee_boss.ogg", true),
            ("hold_your_ground", "Music", "hold_your_ground.ogg", true),
            ("jelly_junktion", "Music", "jelly_junktion.ogg", true),
            ("photophobia", "Music", "photophobia.ogg", true),
            ("gelus_defensor", "Music", "gelus_defensor.ogg", false),
            ("gelus_defensor_no_intro", "Music", "gelus_defensor_no_intro.ogg", true),
            ("fallenstar", "Sound", "star.ogg", false),
            ("ceaseless_and_incessant", "Music", "ceaseless_and_incessant.ogg", true),
            ("circus_freak", "Music", "circus_freak.ogg", true),
            ("invoke_the_ancient", "Music", "invoke_the_ancient.ogg", true)
        };

        /// <summary>
        /// Parses and loads the custom audio files into new cues and
        /// adds them to the game's soundbank.
        /// </summary>
        public static void SetupAudio()
        {
            foreach (var (name, category, filename, loop) in CustomAudio)
            {
                string filePath = Path.Combine(ModEntry.Instance.Helper.DirectoryPath, "assets", "Audio", filename);
                string extension = Path.GetExtension(filePath);
                SoundEffect audio = extension switch
                {
                    ".wav" => SoundEffect.FromFile(filePath),
                    ".ogg" => OggLoader.OpenOggFile(filePath),
                    _ => throw new NotImplementedException(),
                };
                CueDefinition cue = new(name, audio, Game1.audioEngine.GetCategoryIndex(category), loop)
                {
                    instanceLimit = 1,
                    limitBehavior = CueDefinition.LimitBehavior.ReplaceOldest
                };

                Game1.soundBank.AddCue(cue);
            }
        }

        /// <summary>
        /// Event handler for when a game asset is requested to be loaded.
        /// This handles loading all custom assets as defined in <see cref="ModifiedAssets"/>.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="AssetRequestedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="NotImplementedException">If a file attempting to be loaded has no implemented way to be loaded.</exception>
        public static void AssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (ModifiedAssets.TryGetValue(e.Name.BaseName, out string? fromPath))
            {
                string extension = Path.GetExtension(fromPath);
                switch (extension)
                {
                    case ".tmx":
                        e.LoadFromModFile<xTile.Map>(fromPath, AssetLoadPriority.High);
                        return;
                    case ".png":
                        e.LoadFromModFile<Texture2D>(fromPath, AssetLoadPriority.High);
                        return;
                    default:
                        throw new NotImplementedException();
                }
            }

            // Fallback to load all custom maps
            if (e.Name.BaseName.StartsWith("Maps/"))
            {
                string map = e.Name.BaseName.Split("/").Last();
                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, $@"assets/Maps/{map}.tmx");
                if (File.Exists(path))
                {
                    e.LoadFromModFile<xTile.Map>($"assets/Maps/{map}.tmx", AssetLoadPriority.High);
                    return;
                }
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Strings\\StringsFromCSFiles"))
            {
                e.Edit(editor =>
                {
                    IDictionary<string, string> data = editor.AsDictionary<string, string>().Data;
                    foreach (var (key, value) in ModifiedStrings)
                        data[key] = value;
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data\\ObjectInformation"))
            {
                e.Edit(editor =>
                {
                    IDictionary<int, string> data = editor.AsDictionary<int, string>().Data;
                    foreach (var (key, value) in ModifiedObjectInformation)
                        data[key] = value;
                });
            }
        }
    }
}
