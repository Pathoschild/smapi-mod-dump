/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewValley.GameData.Objects;

namespace NermNermNerm.Stardew.QuestableTractor
{
    public static class ObjectIds
    {
        public const string BustedEngine = "NermNermNerm.QuestableTractor.BustedEngine";
        public const string WorkingEngine = "NermNermNerm.QuestableTractor.WorkingEngine";
        public const string BustedLoader = "NermNermNerm.QuestableTractor.BustedLoader";
        public const string WorkingLoader = "NermNermNerm.QuestableTractor.WorkingLoader";
        public const string BustedScythe = "NermNermNerm.QuestableTractor.BustedScythe";
        public const string WorkingScythe = "NermNermNerm.QuestableTractor.WorkingScythe";
        public const string ScythePart1 = "NermNermNerm.QuestableTractor.ScythePart1";
        public const string ScythePart2 = "NermNermNerm.QuestableTractor.ScythePart2";
        public const string AlexesOldShoe = "NermNermNerm.QuestableTractor.AlexesShoe";
        public const string DisguisedShoe = "NermNermNerm.QuestableTractor.DisguisedShoe";
        public const string BustedWaterer = "NermNermNerm.QuestableTractor.BustedWaterer";
        public const string WorkingWaterer = "NermNermNerm.QuestableTractor.WorkingWaterer";
        public const string BustedSeeder = "NermNermNerm.QuestableTractor.BustedSeeder";
        public const string WorkingSeeder = "NermNermNerm.QuestableTractor.WorkingSeeder";

        internal static void EditAssets(IDictionary<string, ObjectData> objects)
        {
            void addQuestItem(string id, string displayName, string description, int spriteIndex)
            {
                objects[id] = new()
                {
                    Name = id,
                    DisplayName = displayName,
                    Description = description,
                    Type = "Quest",
                    Category = -999,
                    Price = 0,
                    Texture = ModEntry.SpritesPseudoPath,
                    SpriteIndex = spriteIndex,
                    ContextTags = new() { "not_giftable", "not_placeable", "prevent_loss_on_death" },
                };
            };
            addQuestItem(
                BustedEngine,
                "funky looking engine that doesn't work", // TODO: 18n
                "Sebastian pulled this off of the rusty tractor.  I need to find someone to fix it.", // TODO: 18n
                10);
            addQuestItem(
                WorkingEngine,
                "Junimo-powered tractor engine", // TODO: 18n
                "The engine for the tractor!  I need to find someone to install it.", // TODO: 18n
                0);
            addQuestItem(
                BustedScythe,
                "core of the harvesting attachment for the tractor", // TODO: 18n
                "This looks like it was a tractor attachment for harvesting crops, but it doesn't seem to be all together.", // TODO: 18n
                12);
            addQuestItem(
                WorkingScythe,
                "harvesting attachment for the tractor", // TODO: 18n
                "Just need to bring this to the tractor garage to be able to use it with the tractor!", // TODO: 18n
                2);
            addQuestItem(
                ScythePart1,
                "crop shakerlooser", // TODO: 18n
                "One of the missing parts for the scythe attachment", // TODO: 18n
                6);
            addQuestItem(
                ScythePart2,
                "shiny sprocket", // TODO: 18n
                "One of the missing parts for the scythe attachment", // TODO: 18n
                7);
            addQuestItem(
                BustedWaterer,
                "broken watering attachment for the tractor", // TODO: 18n
                "This looks like it was a tractor attachment for watering crops.  Sure hope somebody can help me get it working again, watering can really be a drag.", // TODO: 18n
                20);
            addQuestItem(
                WorkingWaterer,
                "watering attachment for the tractor", // TODO: 18n
                "The watering attachment for the tractor - it needs to be brought back to the tractor garage.", // TODO: 18n
                1);
            addQuestItem(
                BustedLoader,
                "bent up and rusty front-end loader for the tractor", // TODO: 18n
                "This was the front-end loader attachment (for picking up rocks and sticks), but it's all bent up and rusted through in spots.  It needs to be fixed to be usable.", // TODO: 18n
                18);
            addQuestItem(
                WorkingLoader,
                "front-end loader attachment for my tractor", // TODO: 18n
                "This will allow me to clear rocks and sticks on my farm.  It needs to go into the tractor garage so I can use it.", // TODO: 18n
                9);
            addQuestItem(
                AlexesOldShoe,
                "Alex's old shoes", // TODO: 18n
                "14EEE, slightly smudged", // TODO: 18n
                4);
            addQuestItem(
                DisguisedShoe,
                "cleverly repackaged pair of shoes", // TODO: 18n
                "Alex's old shoes, cleverly dyed.  Nobody will ever know.", // TODO: 18n
                11);
            addQuestItem(
                BustedSeeder,
                "broken fertilizer and seeder", // TODO: 18n
                "The fertilizer and seed spread for the old tractor.  It needs a good bit of fiddling to make work.", // TODO: 18n
                8);
            addQuestItem(
                WorkingSeeder,
                "fertilizer and seed Seeder attachment for the tractor", // TODO: 18n
                "Just needs to be brought back to the garage to use it on the tractor.", // TODO: 18n
                14);
        }
    }
}
