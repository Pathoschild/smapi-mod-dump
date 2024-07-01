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

using static NermNermNerm.Stardew.LocalizeFromSource.SdvLocalize;

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
                    Type = I("Quest"),
                    Category = -999,
                    Price = 0,
                    Texture = ModEntry.SpritesPseudoPath,
                    SpriteIndex = spriteIndex,
                    ContextTags = new() { "not_giftable", "not_placeable", "prevent_loss_on_death" },
                };
            };
            addQuestItem(
                BustedEngine,
                L("funky looking engine that doesn't work"),
                L("Sebastian pulled this off of the rusty tractor.  I need to find someone to fix it."),
                10);
            addQuestItem(
                WorkingEngine,
                L("Junimo-powered tractor engine"),
                L("The engine for the tractor!  I need to find someone to install it."),
                0);
            addQuestItem(
                BustedScythe,
                L("core of the harvesting attachment for the tractor"),
                L("This looks like it was a tractor attachment for harvesting crops, but it doesn't seem to be all together."),
                12);
            addQuestItem(
                WorkingScythe,
                L("harvesting attachment for the tractor"),
                L("Just need to bring this to the tractor garage to be able to use it with the tractor!"),
                2);
            addQuestItem(
                ScythePart1,
                L("crop shakerlooser"),
                L("One of the missing parts for the scythe attachment"),
                7);
            addQuestItem(
                ScythePart2,
                L("shiny sprocket"),
                L("One of the missing parts for the scythe attachment"),
                6);
            addQuestItem(
                BustedWaterer,
                L("broken watering attachment for the tractor"),
                L("This looks like it was a tractor attachment for watering crops.  Sure hope somebody can help me get it working again, watering can really be a drag."),
                20);
            addQuestItem(
                WorkingWaterer,
                L("watering attachment for the tractor"),
                L("The watering attachment for the tractor - it needs to be brought back to the tractor garage."),
                1);
            addQuestItem(
                BustedLoader,
                L("bent up and rusty front-end loader for the tractor"),
                L("This was the front-end loader attachment (for picking up rocks and sticks), but it's all bent up and rusted through in spots.  It needs to be fixed to be usable."),
                18);
            addQuestItem(
                WorkingLoader,
                L("front-end loader attachment for my tractor"),
                L("This will allow me to clear rocks and sticks on my farm.  It needs to go into the tractor garage so I can use it."),
                9);
            addQuestItem(
                AlexesOldShoe,
                L("Alex's old shoes"),
                L("14EEE, slightly smudged"),
                4);
            addQuestItem(
                DisguisedShoe,
                L("cleverly repackaged pair of shoes"),
                L("Alex's old shoes, cleverly dyed.  Nobody will ever know."),
                11);
            addQuestItem(
                BustedSeeder,
                L("broken fertilizer and seeder"),
                L("The fertilizer and seed spread for the old tractor.  It needs a good bit of fiddling to make work."),
                8);
            addQuestItem(
                WorkingSeeder,
                L("fertilizer and seed Seeder attachment for the tractor"),
                L("Just needs to be brought back to the garage to use it on the tractor."),
                14);
        }
    }
}
