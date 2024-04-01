/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ichortower/HatMouseLacey
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using xTile;
using xTile.Layers;
using xTile.Tiles;

namespace ichortower_HatMouseLacey
{

    internal sealed class ConsoleCommands
    {
        public static void Main(string command, string[] args)
        {
            if (args.Length == 0) {
                Help(command, args);
                return;
            }
            foreach (var entry in Commands) {
                if (string.Equals(entry.Key, args[0], StringComparison.OrdinalIgnoreCase)) {
                    entry.Value.Invoke(command, args[1..]);
                    return;
                }
            }
            Log.Warn($"Unknown command '{args[0]}'.");
        }

        public static Dictionary<string, Action<string, string[]>> Commands = new(){
            {"help", Help},
            {"map_repair", MapRepair},
            {"mousify_child", MousifyChild},
            {"change_clothes", ChangeClothes},
            {"hat_string", HatString},
        };

        public static Dictionary<string, string[]> HelpTexts = new(){
            {"help", new string[]{
                $"Usage: {HML.CommandWord} help [command]",
                "Prints help text for one or all commands.",
            }},
            {"map_repair", new string[]{
                $"Usage: {HML.CommandWord} map_repair",
                "Repairs the map near Lacey's house.",
                "This is called automatically when needed. You shouldn't need to run it.",
            }},
            {"mousify_child", new string[]{
                $"Usage: {HML.CommandWord} mousify_child <name> <variant>",
                "Sets or unsets mouse child status on one of your children.",
                "'variant' should be -1 (human), 0 (grey), or 1 (brown).",
            }},
            {"change_clothes", new string[]{
                $"Usage: {HML.CommandWord} change_clothes",
                "Resets Lacey's appearance.",
                "This is called automatically when needed. You shouldn't need to run it.",
            }},
            {"hat_string", new string[]{
                $"Usage: {HML.CommandWord} hat_string",
                "Prints your current hat string.",
                "This is for debug and development. You shouldn't use it.",
            }},
        };

        public static void Help(string command, string[] args)
        {
            Func<string, string[], string> dump = delegate(string command, string[] lines) {
                string rv = command + "\n";
                foreach (var l in lines) {
                    rv += $"   {l}\n";
                }
                return rv;
            };
            if (args.Length < 1) {
                string output = "\n";
                foreach (var entry in HelpTexts) {
                    output += dump(entry.Key, entry.Value) + "\n";
                }
                Log.Info(output);
                return;
            }
            string name = args[0];
            if (HelpTexts.ContainsKey(name)) {
                Log.Info($"\n{dump(name, HelpTexts[name])}");
            }
            else {
                Log.Warn($"Unknown command '{name}'.");
            }
        }

        /*
         * Reset terrain features (grass, trees, bushes) around Lacey's cabin
         * by reloading them from the (patched) map data.
         * This is to make sure the save file reflects the final map, even on
         * older saves.
         */
        public static void MapRepair(string command, string[] args)
        {
            Log.Trace($"Reloading terrain features near Lacey's house");
            /* This is the rectangle to reset. It should include every tile
             * that we hit with terrain-feature map patches. */
            var rect = new Microsoft.Xna.Framework.Rectangle(25, 89, 15, 11);
            GameLocation forest = Game1.getLocationFromName("Forest");
            if (forest is null || forest.map is null) {
                return;
            }
            Layer paths = forest.map.GetLayer("Paths");
            if (paths is null) {
                return;
            }
            // forest.largeTerrainFeatures is the bushes
            var largeToRemove = new List<LargeTerrainFeature>();
            foreach (var feature in forest.largeTerrainFeatures) {
                Vector2 pos = feature.Tile;
                if (pos.X >= rect.X && pos.X <= rect.X+rect.Width &&
                        pos.Y >= rect.Y && pos.Y <= rect.Y+rect.Height) {
                    largeToRemove.Add(feature);
                }
            }
            foreach (var doomed in largeToRemove) {
                forest.largeTerrainFeatures.Remove(doomed);
            }
            for (int x = rect.X; x < rect.X+rect.Width; ++x) {
                for (int y = rect.Y; y < rect.Y+rect.Height; ++y) {
                    Tile t = paths.Tiles[x, y];
                    if (t is null) {
                        continue;
                    }
                    if (t.TileIndex >= 24 && t.TileIndex <= 26) {
                        forest.largeTerrainFeatures.Add(
                                new StardewValley.TerrainFeatures.Bush(
                                new Vector2(x,y), 26 - t.TileIndex, forest));
                    }
                }
            }
            // forest.terrainFeatures includes grass and trees
            var smallToRemove = new List<Vector2>();
            foreach (var feature in forest.terrainFeatures.Pairs) {
                Vector2 pos = feature.Key;
                if ((feature.Value is Grass || feature.Value is Tree) &&
                        pos.X >= rect.X && pos.X <= rect.X+rect.Width &&
                        pos.Y >= rect.Y && pos.Y <= rect.Y+rect.Height) {
                    smallToRemove.Add(pos);
                }
            }
            foreach (var doomed in smallToRemove) {
                forest.terrainFeatures.Remove(doomed);
            }
            for (int x = rect.X; x < rect.X+rect.Width; ++x) {
                for (int y = rect.Y; y < rect.Y+rect.Height; ++y) {
                    Tile t = paths.Tiles[x, y];
                    if (t is null) {
                        continue;
                    }
                    if (t.TileIndex >= 9 && t.TileIndex <= 11) {
                        int treeType = t.TileIndex - 8 +
                                (Game1.currentSeason.Equals("winter") && t.TileIndex < 11 ? 3 : 0);
                        forest.terrainFeatures.Add(new Vector2(x,y),
                                new Tree($"{treeType}", 5));
                    }
                    else if (t.TileIndex == 12) {
                        forest.terrainFeatures.Add(new Vector2(x,y),
                                new Tree("6", 5));
                    }
                    else if (t.TileIndex == 31 || t.TileIndex == 32) {
                        forest.terrainFeatures.Add(new Vector2(x,y),
                                new Tree($"{40 - t.TileIndex}", 5));
                    }
                    else if (t.TileIndex == 22) {
                        forest.terrainFeatures.Add(new Vector2(x,y),
                                new Grass(1, 3));
                    }
                }
            }
        }

        public static void MousifyChild(string command, string[] args)
        {
            if (args.Length < 2) {
                Log.Warn($"Usage: mousify_child <name> <variant>");
                return;
            }
            if (Game1.player == null) {
                return;
            }
            Child child = null;
            try {
                foreach (var ch in Game1.player.getChildren()) {
                    if (string.Equals(ch.Name, args[0], StringComparison.OrdinalIgnoreCase)) {
                        child = ch;
                        break;
                    }
                }
            }
            catch {}
            if (child == null) {
                Log.Warn($"Could not find your child named '{args[0]}'.");
                return;
            }
            string variant = args[1];
            if (variant != "-1" && variant != "0" && variant != "1") {
                Log.Warn($"Unrecognized variant '{variant}'. Using 0 instead.");
                variant = "0";
            }
            Log.Trace($"Setting variant {variant} for child '{child.Name}'");
            child.modData[$"{HML.CPId}/ChildVariant"] = variant;
            child.reloadSprite();
        }

        public static void ChangeClothes(string command, string[] args)
        {
            Log.Trace("Reloading Lacey's clothes");
            Game1.getCharacterFromName(HML.LaceyInternalName)?.ChooseAppearance();
        }

        public static void HatString(string command, string[] args)
        {
            Log.Info($"'{LCHatString.GetCurrentHatString(Game1.player)}'");
        }

    }

}
