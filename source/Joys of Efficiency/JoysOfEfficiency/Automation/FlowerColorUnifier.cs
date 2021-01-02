/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pomepome/JoysOfEfficiency
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using JoysOfEfficiency.Core;
using JoysOfEfficiency.Menus;
using JoysOfEfficiency.Utils;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace JoysOfEfficiency.Automation
{
    using SVObject = Object;

    internal class FlowerIndex
    {
        public const int Poppy = 376;
        public const int Tulip = 591;
        public const int BlueJazz = 597;
        public const int SummerSpangle = 593;
        public const int FairyRose = 595;
    }
    internal class FlowerColorUnifier
    {
        private static Config Config => InstanceHolder.Config;
        private static ITranslationHelper Translation => InstanceHolder.Translation;

        private static readonly Logger Logger = new Logger("FlowerColorUnifier");

        public static void UnifyFlowerColors()
        {
            foreach (KeyValuePair<Vector2, TerrainFeature> featurePair in Game1.currentLocation.terrainFeatures.Pairs.Where(kv => kv.Value is HoeDirt))
            {
                Vector2 loc = featurePair.Key;
                HoeDirt dirt = (HoeDirt)featurePair.Value;
                Crop crop = dirt.crop;
                if (crop == null || dirt.crop.dead.Value || !dirt.crop.programColored.Value)
                {
                    continue;
                }
                Color oldColor = crop.tintColor.Value;
                switch (crop.indexOfHarvest.Value)
                {
                    case FlowerIndex.Poppy:
                        //Poppy
                        crop.tintColor.Value = Config.PoppyColor;
                        break;
                    case FlowerIndex.Tulip:
                        //Tulip
                        crop.tintColor.Value = Config.TulipColor;
                        break;
                    case FlowerIndex.BlueJazz:
                        //Blue Jazz
                        crop.tintColor.Value = Config.JazzColor;
                        break;
                    case FlowerIndex.SummerSpangle:
                        //Summer Spangle
                        crop.tintColor.Value = Config.SummerSpangleColor;
                        break;
                    case FlowerIndex.FairyRose:
                        //Fairy Rose
                        crop.tintColor.Value = Config.FairyRoseColor;
                        break;
                    default:
                        Color? color = GetCustomizedFlowerColor(crop.indexOfHarvest.Value);
                        if (color != null)
                        {
                            crop.tintColor.Value = color.Value;
                            break;
                        }
                        else
                        {
                            continue;   
                        }
                }

                if (oldColor.PackedValue == crop.tintColor.Value.PackedValue)
                {
                    continue;
                }

                SVObject obj = new SVObject(crop.indexOfHarvest.Value, 1);
                Logger.Log($"changed {obj.DisplayName} @[{loc.X},{loc.Y}] to color(R:{crop.tintColor.R},G:{crop.tintColor.G},B:{crop.tintColor.B},A:{crop.tintColor.A})");
            }
        }

        private static Color? GetCustomizedFlowerColor(int indexOfHarvest)
        {
            return Config.CustomizedFlowerColors.TryGetValue(indexOfHarvest, out Color color) ? (Color?)color : null;
        }

        private static bool IsVanillaFlower(int index)
        {
            switch (index)
            {
                case FlowerIndex.Poppy:
                case FlowerIndex.Tulip:
                case FlowerIndex.BlueJazz:
                case FlowerIndex.SummerSpangle:
                case FlowerIndex.FairyRose: break;
                default: return false;
            }
            return true;
        }

        public static void ToggleFlowerColorUnification()
        {
            GameLocation loc = Game1.currentLocation;
            Vector2 tileLoc = Game1.currentCursorTile;
            Dictionary<Vector2, HoeDirt> hoeDirts = 
                loc.terrainFeatures.Pairs
                    .Where(p => p.Value is HoeDirt)
                    .ToDictionary(p => p.Key, p => p.Value as HoeDirt);
            if (!hoeDirts.ContainsKey(tileLoc))
            {
                Logger.Log("The given tile is not a hoe dirt.");
                return;
            }
            HoeDirt dirt = hoeDirts[tileLoc];
            Crop crop = dirt.crop;
            if (crop == null)
            {
                Logger.Log("There is no crop.");
                return;
            }
            if (crop.dead.Value)
            {
                Logger.Log("The crop is dead.");
                return;
            }

            if (!crop.programColored.Value)
            {
                Logger.Log("That crop may not be a flower.");
                return;
            }

            int index = crop.indexOfHarvest.Value;

            if (IsVanillaFlower(index))
            {
                Util.ShowHudMessage(Translation.Get("flower.vanilla"));
                return;
            }

            if (GetCustomizedFlowerColor(index) != null)
            {
                // Unregister flower
                Config.CustomizedFlowerColors.Remove(crop.indexOfHarvest.Value);
                InstanceHolder.WriteConfig();
                Util.ShowHudMessage(string.Format(Translation.Get("flower.unregister"), Util.GetItemName(index)));
                return;
            }

            // Show flower registration menu
            Game1.playSound("bigSelect");
            Game1.activeClickableMenu = new RegisterFlowerMenu(800, 640, crop.tintColor.Value, index, RegisterFlowerColor);
        }

        private static void RegisterFlowerColor(int whichFlower, Color color)
        {
            Config.CustomizedFlowerColors.Add(whichFlower, color);
            Util.ShowHudMessage(string.Format(Translation.Get("flower.register"), Util.GetItemName(whichFlower)));
        }
    }
}