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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolidFoundations.Framework.Models.Backport;
using SolidFoundations.Framework.Models.ContentPack;
using SolidFoundations.Framework.Utilities;
using SolidFoundations.Framework.Utilities.Backport;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Events;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;
using Object = StardewValley.Object;

namespace SolidFoundations.Framework.Patches.Buildings
{
    // TODO: When updated to SDV v1.6, delete this patch
    internal class QuestionEventPatch : PatchTemplate
    {

        private readonly Type _object = typeof(QuestionEvent);

        internal QuestionEventPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(QuestionEvent.setUp), null), prefix: new HarmonyMethod(GetType(), nameof(SetUpPrefix)));
        }

        internal static bool SetUpPrefix(QuestionEvent __instance, ref bool __result, int ___whichQuestion, AnimalHouse ___animalHouse)
        {
            if (___whichQuestion == 2)
            {
                var targetLocation = FlexibleLocationFinder.GetBuildableLocationByName("Farm");

                FarmAnimal farmAnimal = null;
                foreach (GenericBuilding building in targetLocation.buildings.Where(b => b is GenericBuilding))
                {
                    if (building is null)
                    {
                        continue;
                    }
                    else if ((building.owner.Equals(Game1.player.UniqueMultiplayerID) || !Game1.IsMultiplayer) && building.Model != null && building.Model.AllowAnimalPregnancy && building.indoors.Value is AnimalHouse && !(building.indoors.Value as AnimalHouse).isFull() && Game1.random.NextDouble() < (double)(building.indoors.Value as AnimalHouse).animalsThatLiveHere.Count * 0.0055)
                    {
                        farmAnimal = Utility.getAnimal((building.indoors.Value as AnimalHouse).animalsThatLiveHere[Game1.random.Next((building.indoors.Value as AnimalHouse).animalsThatLiveHere.Count)]);
                        ___animalHouse = building.indoors.Value as AnimalHouse;
                        break;
                    }
                }

                if (farmAnimal != null && !farmAnimal.isBaby() && (bool)farmAnimal.allowReproduction && farmAnimal.CanHavePregnancy())
                {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Events:AnimalBirth", farmAnimal.displayName, farmAnimal.shortDisplayType()));
                    Game1.messagePause = true;
                    __instance.animal = farmAnimal;
                    return false;
                }

                __result = true;
            }

            return true;
        }
    }
}
