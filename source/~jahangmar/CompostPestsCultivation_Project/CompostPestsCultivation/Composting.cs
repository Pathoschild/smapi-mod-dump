//Copyright (c) 2019 Jahangmar

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU Lesser General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//GNU Lesser General Public License for more details.

//You should have received a copy of the GNU Lesser General Public License
//along with this program. If not, see <https://www.gnu.org/licenses/>.


using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;

namespace CompostPestsCultivation
{
    public class Composting : ModComponent
    {
        public static List<Vector2> CompostApplied;
        public static Dictionary<Vector2, List<Item>> ComposterContents;
        public static Dictionary<Vector2, bool> ComposterRunning;

        private static readonly int MaxOccupantsID = -8763921;

        class GreenBrown
        {
            public int Green = 0;
            public int Brown = 0;

            public GreenBrown(int green, int brown)
            {
                this.Green = green;
                this.Brown = brown;
            }
        }

        private static Dictionary<string, GreenBrown> GreenBrownDistributionsByName = new Dictionary<string, GreenBrown>(){
            {"Hay", new GreenBrown(1,9) },
            {"Weeds", new GreenBrown(5,5) }
            };
        private static Dictionary<int, GreenBrown> GreenBrownDistributionsByCategory = new Dictionary<int, GreenBrown>(){
            {Object.VegetableCategory, new GreenBrown(8,2) },
            {Object.FruitsCategory, new GreenBrown(8,2) },
            {Object.flowersCategory, new GreenBrown(8,2) }
            };

        private static GreenBrown GetGreenBrown(Item item)
        {
            if (GreenBrownDistributionsByName.ContainsKey(item.Name))
                return GreenBrownDistributionsByName[item.Name];
            else if (GreenBrownDistributionsByCategory.ContainsKey(item.Category))
                return GreenBrownDistributionsByCategory[item.Category];
            else
                return new GreenBrown(0,0);
        }

        public static int GetGreen(Item item) => item == null ? 0 : GetGreenBrown(item).Green;
        public static int GetBrown(Item item) => item == null ? 0 : GetGreenBrown(item).Brown;

        public static void Load()
        {
            CompostApplied = LoadField<List<Vector2>>(SaveData._CompostApplied);
            ComposterContents = LoadField<Dictionary<Vector2, List<Item>>>(SaveData._ComposterContents);
            ComposterRunning = LoadField<Dictionary<Vector2, bool>>(SaveData._ComposterRunning);

            if (CompostApplied == null)
                CompostApplied = new List<Vector2>();
            if (ComposterContents == null)
                ComposterContents = new Dictionary<Vector2, List<Item>>();
            if (ComposterRunning == null)
                ComposterRunning = new Dictionary<Vector2, bool>();
        }

        public static void Save()
        {
            Game1.getFarm().buildings.Set(new List<Building>(Game1.getFarm().buildings).Select((Building building) => building is CompostingBin bin ? bin.ToShippingBin() : building).ToList());

            SaveField(CompostApplied);
            SaveField(ComposterContents);
            SaveField(ComposterRunning);
        }

        public static void OnNewDay()
        {
            //if (Game1.getFarm().buildings.ToList().Exists((Building obj) => Composting.IsComposter(obj)))
            //    ModEntry.GetMonitor().Log("Found Composter", StardewModdingAPI.LogLevel.Alert);

            Game1.getFarm().buildings.Set(new List<Building>(Game1.getFarm().buildings).Select((Building building) => building is ShippingBin bin && Composting.IsComposter(bin) ? CompostingBin.FromShippingBin(bin) : building).ToList());

            //if (Game1.getFarm().buildings.ToList().Exists((Building obj) => obj is CompostingBin))
            //    ModEntry.GetMonitor().Log("transformed composter", StardewModdingAPI.LogLevel.Alert);
        }

        public static bool AffectedByCompost(Vector2 tile)
        {
            return CompostApplied.Contains(tile) || CompostApplied.Exists((Vector2 vec) => GetAdjacentTiles(vec).Contains(tile));
        }

        public static BluePrint GetComposterBlueprint()
        {
            return new BluePrint("Shipping Bin")
            {
                displayName = ModEntry.GetHelper().Translation.Get("composter.name"),
                description = ModEntry.GetHelper().Translation.Get("composter.description"),
                maxOccupants = MaxOccupantsID,
                moneyRequired = 500,
                tilesWidth = 2,
                tilesHeight = 1
                //sourceRectForMenuView = new Rectangle(0, 0, 64, 96),
                //itemsRequired = this.Config.BuildMaterials
            };
        }

        public static bool IsComposter(Building building)
        {
            return building != null && building.maxOccupants == MaxOccupantsID;
        }

        public static bool IsComposterBlueprint(BluePrint blueprint)
        {
            return blueprint != null && blueprint.maxOccupants == MaxOccupantsID;
        }

        public static void AddBlueprint(CarpenterMenu menu)
        {
            ModEntry.GetHelper().Reflection.GetField<List<BluePrint>>(menu, "blueprints").GetValue().Add(GetComposterBlueprint());
        }
    }
}
