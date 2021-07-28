/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jn84/QualitySmash
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;

namespace QualitySmash
{
    class ModConfig
    {
        public bool EnableUISmashButtons { get; set; }

        public bool EnableUIColorSmashButton { get; set; }

        public bool EnableUIQualitySmashButton { get; set; }

        public bool EnableEggColorSmashing { get; set; }

        public bool EnableSingleItemSmashKeybinds { get; set; }

        public SButton ColorSmashKeybind { get; set; }

        public SButton QualitySmashKeybind { get; set; }

        public string ItemIDReference { get; set; }
        
        public List<string> IgnoreIridiumDescription { get; set; }
        public bool IgnoreIridium { get; set; }

        public List<string> IgnoreIridiumItemExceptionsDescription { get; set; }
        public List<int> IgnoreIridiumItemExceptions { get; set; }

        public List<string> IgnoreIridiumCategoryExceptionsDescription { get; set; }
        public List<int> IgnoreIridiumCategoryExceptions { get; set; }

        public List<string> IgnoreGoldDescription { get; set; }
        public bool IgnoreGold { get; set; }

        public List<string> IgnoreSilverDescription { get; set; }
        public bool IgnoreSilver { get; set; }

        public List<string> IgnoreItemsColorDescription { get; set; }
        public List<int> IgnoreItemsColor { get; set; }

        public List<string> IgnoreItemsQualityDescription { get; set; }
        public List<int> IgnoreItemsQuality { get; set; }

        public List<string> IgnoreItemsCategoryDescription { get; set; }
        public List<int> IgnoreItemsCategory { get; set; }

        public ModConfig()
        {
            this.EnableUISmashButtons = true;

            this.EnableUIColorSmashButton = true;

            this.EnableUIQualitySmashButton = true;

            this.EnableEggColorSmashing = true;
            
            this.EnableSingleItemSmashKeybinds = false;

            this.ColorSmashKeybind = SButton.C;

            this.QualitySmashKeybind = SButton.Q;

            this.ItemIDReference = "Item ID reference: https://stardewids.com";

            this.IgnoreIridiumDescription = new List<string>()
            {
                "If true, iridium quality items will",
                "not be affected by 'Smash Quality'"
            };
            this.IgnoreIridium = true;

            this.IgnoreIridiumItemExceptionsDescription = new List<string>()
            {
                "Items IDs listed here will still be",
                "converted to lowest present quality even if",
                "IgnoreIridium is set to true.",
                "Defaults:",
                "   296: Salmonberry",
            };
            this.IgnoreIridiumItemExceptions = new List<int>()
            {
                296,
            };

            this.IgnoreIridiumCategoryExceptionsDescription = new List<string>()
            {
                "Items IDs listed here will still be",
                "converted to lowest present quality even if",
                "IgnoreIridium is set to true.",
                "Defaults:",
                "   -4: Fish",
            };
            this.IgnoreIridiumCategoryExceptions = new List<int>()
            {
                -4,
            };

            this.IgnoreGoldDescription = new List<string>()
            {
                "If true, gold quality items will",
                "not be affected by 'Smash Quality'"
            };
            this.IgnoreGold = false;

            this.IgnoreSilverDescription = new List<string>()
            {
                "If true, silver quality items will",
                "not be affected by 'Smash Quality'"
            };
            this.IgnoreSilver = false;

            this.IgnoreItemsColorDescription = new List<string>()
            {
                "A list of item IDs which will not",
                "be affected by 'Smash Colors'",
                "Defaults:",
                "    591 = Tulip",
                "    593 = Summer Spangle",
                "",
                "Flowers not used in recipes"
            };
            this.IgnoreItemsColor = new List<int>()
            {
                591,
                593
            };

            this.IgnoreItemsQualityDescription = new List<string>()
            {
                "A list of item IDs which will not",
                "be affected by 'Smash Quality'",
                "Defaults:",
                "    348 = Wine",
                "    459 = Mead",
                "    303 = Pale Ale",
                "    346 = Beer",
                "    424 = Cheese",
                "    426 = Goat Cheese"
            };
            this.IgnoreItemsQuality = new List<int>()
            {
                348,
                459,
                303,
                346,
                424,
                426
            };

            this.IgnoreItemsCategoryDescription = new List<string>()
            {
                "A list of item categories which will not",
                "be affected by 'Smash Quality' or 'Smash Colors'",
                "Defaults:",
                "    -26 = Artisan Goods",
                "",
                "A list of categories (1.5) is included in the mod folder"
            };
            this.IgnoreItemsCategory = new List<int>()
            {
                -26 // Artisan Goods
            };
        }
    }
}
