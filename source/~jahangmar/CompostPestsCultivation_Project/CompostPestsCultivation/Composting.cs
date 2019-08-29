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
using StardewValley.Objects;

using SavableItemList = System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<int, int>>;

namespace CompostPestsCultivation
{
    public class Composting : ModComponent
    {
        public static Dictionary<Vector2, int> CompostAppliedDays;
        public static Dictionary<Vector2, List<Item>> ComposterContents;
        public static Dictionary<Vector2, int> ComposterDaysLeft;
        public static Dictionary<Vector2, int> ComposterCompostLeft;

        public static Config config;

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

        public static void Init(Config conf)
        {
            config = conf;
        }

        private static Dictionary<Vector2, List<Item>> SavableItemsToItems(Dictionary<Vector2, SavableItemList> dic)
        {
            Dictionary<Vector2, List<Item>> result = new Dictionary<Vector2, List<Item>>();
            foreach (KeyValuePair<Vector2, SavableItemList> pair in dic)
            {
                result.Add(pair.Key, pair.Value.Select((arg) => arg.Key == -1 ? null : ObjectFactory.getItemFromDescription(ObjectFactory.regularObject, arg.Key, arg.Value)).ToList());
            }
            return result;
        }

        private static Dictionary<Vector2, SavableItemList> ItemsToSavableItems(Dictionary<Vector2, List<Item>> dic)
        {
            Dictionary<Vector2, SavableItemList> result = new Dictionary<Vector2, SavableItemList>();
            foreach (KeyValuePair<Vector2, List<Item>> pair in dic)
            {
                result.Add(pair.Key, pair.Value.Select((arg) => arg == null ? new KeyValuePair<int, int>(-1,-1) : new KeyValuePair<int, int>(arg.ParentSheetIndex, arg.Stack)).ToList());
            }
            return result;
        }

        public static void Load(SaveData data)
        {
            CompostAppliedDays = data.CompostAppliedDays;
            ComposterContents = SavableItemsToItems(data.ComposterContents);
            //ComposterContents = data.ComposterContents.Select((KeyValuePair<Vector2, Dictionary<int, int>> arg) => new KeyValuePair<Vector2, List<Item>>(arg.Key, IntsToItems(arg.Value))).ToDictionary((arg) => arg.Key);
            ComposterDaysLeft = data.ComposterDaysLeft;
            ComposterCompostLeft = data.ComposterCompostLeft;

            ModEntry.GetMonitor().Log("Composting.Load() executed", StardewModdingAPI.LogLevel.Trace);
        }

        public static void Save(SaveData data)
        {
            data.CompostAppliedDays = CompostAppliedDays;
            data.ComposterContents = ItemsToSavableItems(ComposterContents);
            data.ComposterDaysLeft = ComposterDaysLeft;
            data.ComposterCompostLeft = ComposterCompostLeft;

            ModEntry.GetMonitor().Log("Composting.Save() executed", StardewModdingAPI.LogLevel.Trace);
        }

        public static void ResetCompostingBins()
        {
            Game1.getFarm().buildings.Set(new List<Building>(Game1.getFarm().buildings).Select((Building building) => building is CompostingBin bin ? bin.ToShippingBin() : building).ToList());
        }

        public static void SetCompostingBins()
        {
            Game1.getFarm().buildings.Set(new List<Building>(Game1.getFarm().buildings).Select((Building building) => building is ShippingBin bin && Composting.IsComposter(bin) ? CompostingBin.FromShippingBin(bin) : building).ToList());
        }

        public static void OnNewDay()
        {
            //if (Game1.getFarm().buildings.ToList().Exists((Building obj) => Composting.IsComposter(obj)))
            //    ModEntry.GetMonitor().Log("Found Composter", StardewModdingAPI.LogLevel.Alert);

            SetCompostingBins();

            //if (Game1.getFarm().buildings.ToList().Exists((Building obj) => obj is CompostingBin))
            //    ModEntry.GetMonitor().Log("transformed composter", StardewModdingAPI.LogLevel.Alert);

            void DecreaseDays(Dictionary<Vector2, int> dic)
            {
                var copy = new Dictionary<Vector2, int>(dic);
                dic.Clear();
                foreach (KeyValuePair<Vector2, int> pair in copy)
                {
                    Vector2 key = pair.Key;
                    int value = pair.Value;
                    if (value > 0)
                        value--;
                    dic.Add(key, value);
                }
            }

            DecreaseDays(ComposterDaysLeft);
            ComposterDaysLeft.Keys.ToList().ForEach((Vector2 vec) =>
            {
                if (ComposterDaysLeft[vec] <= 0)
                {
                    ComposterDaysLeft.Remove(vec);
                    int left = ComposterContents[vec].Sum((Item arg) => arg == null ? 0 : arg.Stack);
                    if (ComposterCompostLeft.ContainsKey(vec))
                        ComposterCompostLeft[vec] = left;
                    else
                        ComposterCompostLeft.Add(vec, left);
                    ComposterContents.Remove(vec);
                }
            });

            DecreaseDays(CompostAppliedDays);
            CompostAppliedDays.Keys.ToList().ForEach((Vector2 vec) =>
            {
                if (CompostAppliedDays[vec] <= 0)
                    CompostAppliedDays.Remove(vec);
            });
        }

        public static bool AffectedByCompost(Vector2 tile)
        {
            return CompostAppliedDays.ContainsKey(tile) && CompostAppliedDays[tile] > 0 || CompostAppliedDays.Keys.Intersect(GetAdjacentTiles(tile)).ToList().Exists((Vector2 vec) => CompostAppliedDays[vec] > 0);
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
