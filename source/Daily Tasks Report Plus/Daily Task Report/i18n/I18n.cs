/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/DailyTaskReportPlus
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewValley;

namespace DailyTasksReport.UI
{
    internal static class I18n
    {
        private static ITranslationHelper Translations;
        public static void Init(ITranslationHelper translations)
        {
            Translations = translations;
        }

        //
        //  translations
        //
        public static string Tasks_In()
        {
            return GetByKey("tasks.location.in");
        }
        //
        //  task reporting
        //
        public static string Tasks_LastDay()
        {
            return GetByKey("tasks.misc.lastday");
        }
        public static string Tasks_NoHay()
        {
            return GetByKey("tasks.misc.nohay");
        }
        public static string Tasks_Hay(int iCount)
        {
            return GetByKey("tasks.misc.hay", new { Count = iCount.ToString() });
        }
        public static string Tasks_NothingToDo()
        {
            return GetByKey("tasks.nothingtodo");
        }
        public static string Tasks_Object_Silver()
        {
            return GetByKey("tasks.object.silver");
        }
        public static string Tasks_Object_Gold()
        {
            return GetByKey("tasks.object.gold");
        }
        public static string Tasks_Object_Iridium()
        {
            return GetByKey("tasks.object.iridium");
        }
        public static string Tasks_Object_With()
        {
            return GetByKey("tasks.object.with");
        }
        public static string Tasks_At()
        {
            return GetByKey("tasks.at");
        }
        public static string Tasks_Pet_Pet()
        {
            return GetByKey("tasks.pet.pet");
        }
        public static string Tasks_Pet_Bowl()
        {
            return GetByKey("tasks.pet.bowl");
        }
        public static string Tasks_Terrain_Fence()
        {
            return GetByKey("tasks.terrain.fence");
        }
        public static string Tasks_Terrain_Damaged()
        {
            return GetByKey("tasks.terrain.damaged");
        }
        public static string Tasks_Complete()
        {
            return GetByKey("tasks.complete");
        }
        public static string Tasks_Object_Crab()
        {
            return GetByKey("tasks.object.crab");
        }
        public static string Tasks_Object_CrabPot()
        {
            return GetByKey("tasks.object.crabpot");
        }
        public static string Tasks_Object_Machine()
        {
            return GetByKey("tasks.object.machine");
        }
        public static string Tasks_Misc_Queen()
        {
            return GetByKey("tasks.misc.queen");
        }
        public static string Tasks_Misc_Birthday(string sNPCName)
        {
            return GetByKey("tasks.misc.birthday", new { NPCName = sNPCName });
        }
        public static string Tasks_Misc_Merchant()
        {
            return GetByKey("tasks.misc.merchant");
        }
        public static string Tasks_Cave_InCave()
        {
            return GetByKey("tasks.cave.incave");
        }
        public static string Tasks_Cave_ItemName_Fruits()
        {
            return GetByKey("tasks.cave.itemname.fruits");
        }
        public static string Tasks_Cave_ItemName_Mushrooms()
        {
            return GetByKey("tasks.cave.itemname.mushrooms");
        }
        public static string Tasks_Crop_NotWatered()
        {
            return GetByKey("tasks.crop.notwatered");
        }
        public static string Tasks_Crop_ReadyToHarvest()
        {
            return GetByKey("tasks.crop.readytoharvest");
        }
        public static string Tasks_Crop_Dead()
        {
            return GetByKey("tasks.crop.dead");
        }
        public static string Tasks_Crop_Fruit()
        {
            return GetByKey("tasks.crop.fruits");
        }
        public static string Tasks_Crop_Label_Unwatered()
        {
            return GetByKey("tasks.crop.label.unwatered");
        }
        public static string Tasks_Crop_Label_Harvest()
        {
            return GetByKey("tasks.crop.label.harvest");
        }
        public static string Tasks_Crop_Label_Crops()
        {
            return GetByKey("tasks.crop.label.crops");
        }
        public static string Tasks_Crop_Label_Fruit()
        {
            return GetByKey("tasks.crop.label.fruits");
        }
        public static string Tasks_Crop_TreeAt()
        {
            return GetByKey("tasks.crop.treeat");
        }
        public static string Tasks_Crop_With()
        {
            return GetByKey("tasks.crop.with");
        }
        public static string Tasks_Crop_WithFruit()
        {
            return GetByKey("tasks.crop.withfruit");
        }
        public static string Tasks_Crop_WithFruits()
        {
            return GetByKey("tasks.crop.withfruits");
        }
        public static string Tasks_Animal_NotPetted()
        {
            return GetByKey("tasks.animal.notpettedanimals");
        }
        public static string Tasks_Animal_Uncollected()
        {
            return GetByKey("tasks.animal.uncollectedanimalproduct");
        }
        public static string Tasks_Animal_EmptyHay()
        {
            return GetByKey("tasks.animal.emptyhay");
        }
        public static string Tasks_Animal_Has()
        {
            return GetByKey("tasks.animal.has");
        }
        public static string Tasks_Animal_MissingHay()
        {
            return GetByKey("tasks.animal.missinghay");
        }
        public static string Tasks_Animal_MissingHays()
        {
            return GetByKey("tasks.animal.missinghays");
        }
        public static string Tasks_Ponds_Attention()
        {
            return GetByKey("tasks.animal.ponds.attention");
        }
        public static string Tasks_Ponds_Collect()
        {
            return GetByKey("tasks.animal.ponds.collect");
        }
        public static string Prod_Egg_Brown()
        {
            return GetByKey("prod.egg.brown");
        }
        private static Translation GetByKey(string key, object? tokens = null)
        {
            if (Translations == null)
                throw new InvalidOperationException($"You must call {nameof(I18n)}.{nameof(Init)} from the mod's entry method before reading translations.");
            return Translations.Get(key, tokens);
        }
    }
}
