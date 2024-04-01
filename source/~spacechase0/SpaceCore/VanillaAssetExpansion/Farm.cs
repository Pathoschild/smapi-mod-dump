/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.Buildings;

namespace SpaceCore.VanillaAssetExpansion
{
    public class FarmExtensionData
    {
        public class BuildingData
        {
            public string Id { get; set; }
            public string Type { get; set; }
            public Vector2 Position { get; set; }
            public Dictionary<string, string> Animals { get; set; }
        }

        public class FenceData
        {
            public string Id { get; set; }
            public string FenceId { get; set; }
            public Rectangle Area { get; set; }
            public bool IsGate { get; set; }
        }

        public List<BuildingData> Buildings { get; set; } = new();
        public List<FenceData> Fences { get; set; } = new();
    }

    [HarmonyPatch(typeof(Farm), nameof(Farm.onNewGame))]
    public static class FarmStartersPatch
    {
        public static void Postfix(Farm __instance)
        {
            var dict = Game1.content.Load<Dictionary<string, FarmExtensionData>>("spacechase0.SpaceCore/FarmExtensionData");
            if (!dict.ContainsKey(Game1.whichModFarm?.Id ?? ""))
                return;
            var fdata = dict[Game1.whichModFarm.Id];

            foreach (var bfdata in fdata.Buildings)
            {
                var building = Building.CreateInstanceFromId(bfdata.Type, bfdata.Position);
                if (building == null)
                    continue;
                building.FinishConstruction(true);
                building.LoadFromBuildingData(building.GetData(), false, true);

                foreach (var animalPair in bfdata.Animals)
                {
                    FarmAnimal animal = new FarmAnimal(animalPair.Value, Game1.Multiplayer.getNewID(), Game1.player.UniqueMultiplayerID);
                    animal.Name = animalPair.Key;
                    (building.GetIndoors() as AnimalHouse).adoptAnimal(animal);
                }

                __instance.buildings.Add(building);
            }

            foreach (var fence in fdata.Fences)
            {
                for (int ix = fence.Area.X; ix < fence.Area.X + fence.Area.Width; ++ix)
                {
                    for (int iy = fence.Area.X; iy < fence.Area.Y + fence.Area.Height; ++iy)
                    {
                        __instance.objects.Add(new Vector2(ix, iy), new Fence(new(ix, iy), fence.FenceId, fence.IsGate));
                    }
                }
            }
        }
    }
}
