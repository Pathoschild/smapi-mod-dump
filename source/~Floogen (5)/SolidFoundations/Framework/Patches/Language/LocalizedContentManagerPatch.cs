/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using HarmonyLib;
using SolidFoundations.Framework.Models.ContentPack;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Buildings;
using StardewValley.Menus;
using System;

namespace SolidFoundations.Framework.Patches.GameData
{
    internal class LocalizedContentManagerPatch : PatchTemplate
    {

        private readonly Type _object = typeof(LocalizedContentManager);

        internal LocalizedContentManagerPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, "set_CurrentLanguageCode", null), postfix: new HarmonyMethod(GetType(), nameof(HandleTranslation)));
        }

        internal static void HandleTranslation(LocalizedContentManager __instance)
        {
            foreach (var model in SolidFoundations.buildingManager.GetAllBuildingModels())
            {
                model.Name = model.GetTranslation(model.NameTranslationKey);
                model.Description = model.GetTranslation(model.DescriptionTranslationKey);

                SolidFoundations.buildingManager.UpdateModel(model);
            }
        }
    }
}
