/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/DailyTaskReportPlus
**
*************************************************/

using System.Text;
using StardewValley.Buildings;
using DailyTasksReport.Tasks;
using StardewModdingAPI.Events;
using StardewValley.Menus;
using DailyTasksReport.UI;
using StardewValley.TokenizableStrings;



namespace DailyTasksReport.TaskEngines
{
    class AnimalTaskEngine : TaskEngine
    {
        public static readonly string[] CollectableAnimalProducts = { "107", "174", "176", "180", "182", "440", "442", "444", "446" };

        private static readonly List<TaskItem<SDObject>> AnimalProductsToCollect = new List<TaskItem<SDObject>>();
        private static readonly List<TaskItem<SDObject>> TrufflesToCollect = new List<TaskItem<SDObject>>();
        private static readonly List<TaskItem<FarmAnimal>> AnimalProductsToHarvest = new List<TaskItem<FarmAnimal>>();
        private static readonly List<TaskItem<FarmAnimal>> UnpettedAnimals = new List<TaskItem<FarmAnimal>>();
        private static readonly List<Tuple<Building, int>> MissingHay = new List<Tuple<Building, int>>();
        private static readonly List<Tuple<string, FishPond>> PondsNeedingAttention = new List<Tuple<string, FishPond>>();
        private static readonly List<Tuple<string, FishPond>> PondsWithItem = new List<Tuple<string, FishPond>>();

        public readonly AnimalsTaskId _id;
        public static AnimalsTaskId _who = AnimalsTaskId.None;

        private static List<GameLocation> _gameFarms = new();

        internal AnimalTaskEngine(TaskReportConfig config, AnimalsTaskId id)
        {
            _config = config;
            _id = id;
            TaskId = (int)id;
            TaskSubClass = id.ToString();
            TaskClass = "Animal";

            if (id == AnimalsTaskId.UnpettedAnimals)
            {
                DailyTaskHelper.Helper.Events.Display.MenuChanged += Display_MenuChanged;
            }
            SetEnabled();
        }
        public static void ReScanUnpettedAnimals()
        {
            UnpettedAnimals.Clear();

            foreach (GameLocation farm in _gameFarms)
            {
                foreach (KeyValuePair<long, FarmAnimal> animal in farm.animals.Pairs)
                    if (!animal.Value.wasPet.Value)
                        UnpettedAnimals.Add(new TaskItem<FarmAnimal>(farm, animal.Value.Position, animal.Value));

                foreach (Building? building in farm.buildings)
                {
                    if (building.isUnderConstruction() || !(building.indoors.Value is AnimalHouse animalHouse))
                        continue;

                    foreach (KeyValuePair<long, FarmAnimal> animal in animalHouse.animals.Pairs)
                        if (!animal.Value.wasPet.Value)
                            UnpettedAnimals.Add(new TaskItem<FarmAnimal>(animalHouse, animal.Value.Position, animal.Value));
                }
            }
        }

        public override void UpdateList()
        {
            switch ((AnimalsTaskId)TaskId)
            {
                case AnimalsTaskId.UnpettedAnimals:
                    UnpettedAnimals.RemoveAll(a => a.Object.wasPet.Value || a.Object.health.Value <= 0);

                    foreach (GameLocation farm in _gameFarms)
                    {
                        foreach (TaskItem<FarmAnimal> animal in UnpettedAnimals)
                            animal.Location = farm.animals.FieldDict.ContainsKey(animal.Object.myID.Value)
                                ? farm
                                : animal.Object.home?.indoors.Value ?? null;
                    }
                    break;
                case AnimalsTaskId.AnimalProducts:

                    AnimalProductsToCollect.RemoveAll(i => i.Location != null &&
                        !(i.Location.objects.TryGetValue(i.Position, out SDObject? obj) &&
                          obj.QualifiedItemId == i.Object.QualifiedItemId));

                    TrufflesToCollect.Clear();
                    AnimalProductsToHarvest.RemoveAll(animal =>
                      string.IsNullOrEmpty(animal.Object.currentProduce.Value) || animal.Object.health.Value <= 0);

                    CheckForTruffles(_gameFarms);

                    foreach (GameLocation farm in _gameFarms)
                    {
                        foreach (TaskItem<FarmAnimal> animal in AnimalProductsToHarvest)
                            animal.Location = farm.animals.ContainsKey(animal.Object.myID.Value)
                                            ? farm
                                            : animal.Object?.home?.indoors.Value ?? null;
                    }
                    break;

                case AnimalsTaskId.MissingHay:
                    MissingHay.Clear();

                    foreach (GameLocation farm in _gameFarms)
                    {
                        foreach (Building? building in farm.buildings)
                        {
                            if (building.isUnderConstruction() || !(building.indoors.Value is AnimalHouse animalHouse)) continue;
                            int count = animalHouse.numberOfObjectsWithName("Hay");
                            int animalLimit = animalHouse.animalLimit.Value;
                            if (count < animalLimit)
                                MissingHay.Add(new Tuple<Building, int>(building, animalLimit - count));
                        }
                    }
                    break;
                case AnimalsTaskId.PondsNeedingAttention:
                    PondsNeedingAttention.Clear();

                    foreach (GameLocation farm in _gameFarms)
                    {
                        foreach (Building? building in farm.buildings)
                        {
                            if (building.isUnderConstruction()) continue;

                            if (building is FishPond fishpond)
                            {
                                if (fishpond.HasUnresolvedNeeds())
                                {
                                    PondsNeedingAttention.Add(Tuple.Create(farm.DisplayName, fishpond));
                                }
                            }
                        }
                    }
                    break;
                case AnimalsTaskId.PondsWithItems:
                    PondsWithItem.Clear();

                    foreach (GameLocation farm in _gameFarms)
                    {
                        foreach (Building? building in farm.buildings)
                        {
                            if (building.isUnderConstruction()) continue;

                            if (building is FishPond fishpond)
                            {
                                if (fishpond.output.Value != null)
                                {
                                    PondsWithItem.Add(Tuple.Create(farm.DisplayName, fishpond));
                                }
                            }
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown animal task");
            }
        }


        private static void CheckAnimals(List<GameLocation> farms)
        {
            foreach (GameLocation farm in _gameFarms)
            {
                foreach (KeyValuePair<long, FarmAnimal> animal in farm.animals.Pairs)
                {
                    if (animal.Value != null)
                    {
                        if (!animal.Value.wasPet.Value)
                            UnpettedAnimals.Add(new TaskItem<FarmAnimal>(farm, animal.Value.Position, animal.Value));

                        if (animal.Value.currentProduce.Value != null)
                        {
                            if (animal.Value.currentProduce.Value != "430")
                                AnimalProductsToHarvest.Add(new TaskItem<FarmAnimal>(farm, animal.Value.Position, animal.Value));
                        }
                    }
                }
            }
        }



        private static void CheckAnimals(AnimalHouse location)
        {
            foreach (KeyValuePair<long, FarmAnimal> animal in location.animals.Pairs)
            {
                if (!animal.Value.wasPet.Value)
                    UnpettedAnimals.Add(new TaskItem<FarmAnimal>(location, animal.Value.position.Value, animal.Value));


                if (animal.Value.currentProduce.Value != null)
                {
                    if (animal.Value.currentProduce.Value != "430")
                        AnimalProductsToHarvest.Add(new TaskItem<FarmAnimal>(location, animal.Value.position.Value,
                            animal.Value));
                }
            }
        }

        private static void CheckAnimalProductsInCoop(GameLocation coop)
        {
            foreach (KeyValuePair<Vector2, SDObject> pair in coop.objects.Pairs)
                if (Array.BinarySearch(CollectableAnimalProducts, pair.Value.ItemId) >= 0 &&
                    (int)pair.Key.X <= coop.map.DisplayWidth / Game1.tileSize &&
                    (int)pair.Key.Y <= coop.map.DisplayHeight / Game1.tileSize)
                    AnimalProductsToCollect.Add(new TaskItem<SDObject>(coop, pair.Key, pair.Value));
        }

        private static void CheckForTruffles(List<GameLocation> farms)
        {

            foreach (GameLocation farm in farms)
            {
                foreach (KeyValuePair<Vector2, SDObject> pair in farm.objects.Pairs)

                    if (pair.Value.QualifiedItemId == "(O)430")
                        TrufflesToCollect.Add(new TaskItem<SDObject>(farm, pair.Key, pair.Value));
            }
        }

        public override void SetEnabled()
        {
            switch (_id)
            {
                case AnimalsTaskId.UnpettedAnimals:
                    Enabled = _config.UnpettedAnimals;
                    break;

                case AnimalsTaskId.AnimalProducts:
                    Enabled = _config.AnimalProducts.ContainsValue(true);
                    break;

                case AnimalsTaskId.MissingHay:
                    Enabled = _config.MissingHay;
                    break;
                case AnimalsTaskId.PondsNeedingAttention:
                    Enabled = _config.PondsNeedingAttention;
                    break;
                case AnimalsTaskId.PondsWithItems:
                    Enabled = _config.PondsWithItems;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown animal task");
            }
        }
        private static void Display_MenuChanged(object? sender, MenuChangedEventArgs e)
        {
            if (e.OldMenu is PurchaseAnimalsMenu ||
                e.OldMenu is NamingMenu)
                ReScanUnpettedAnimals();
        }
        public override void Clear()
        {
            SetEnabled();

            if ((AnimalsTaskId)TaskId != _who) return;

            UnpettedAnimals.Clear();
            AnimalProductsToHarvest.Clear();
            AnimalProductsToCollect.Clear();
            TrufflesToCollect.Clear();
            MissingHay.Clear();
            PondsNeedingAttention.Clear();
            PondsWithItem.Clear();
        }

        public override List<ReportReturnItem> DetailedInfo()
        {
            List<ReportReturnItem> rpItem = new List<ReportReturnItem> { };

            if (!Enabled) return rpItem;

            StringBuilder stringBuilder = new StringBuilder();

            switch ((AnimalsTaskId)TaskId)
            {
                case AnimalsTaskId.UnpettedAnimals:
                    if (UnpettedAnimals.Count == 0) return rpItem;

                    foreach (TaskItem<FarmAnimal>? animal in UnpettedAnimals.OrderBy(p => p.Object.displayName))
                    {
                        try
                        {
                            string label;
                            if (_gameFarms.Count == 1)
                            {
                                label = $"{animal.Object.displayType} {animal.Object.displayName}{I18n.Tasks_At()}{FormatLocation(animal.Location?.Name ?? "????", null, animal.Object.Tile)}";
                            }
                            else
                            {
                                label = $"{animal.Object.displayType} {animal.Object.displayName}{I18n.Tasks_At()}{FormatLocation(animal.Location?.GetParentLocation().Name ?? "????", animal.Object.home?.GetData().Name ?? "????", animal.Object.Tile)}";
                            }
                            rpItem.Add(new ReportReturnItem
                            {
                                SortKey = animal.Object.displayName,
                                Label = label,
                                WarpTo = animal.Location == null ? null : new Tuple<string, int, int>(animal.Location.Name, (int)animal.Object.Tile.X, (int)animal.Object.Tile.Y)
                            });
                        }
                        catch { }

                    }
                    break;
                case AnimalsTaskId.PondsNeedingAttention:
                    if (PondsNeedingAttention.Count == 0) return rpItem;

                    foreach (Tuple<string, FishPond> fpond in PondsNeedingAttention)
                    {
                        rpItem.Add(new ReportReturnItem
                        {
                            Label = $"Fish Pond{I18n.Tasks_At()}{FormatLocation(fpond.Item1, null, fpond.Item2.tileX.Value, fpond.Item2.tileY.Value)} wants something.",
                            WarpTo = Tuple.Create(fpond.Item1, fpond.Item2.tileX.Value + fpond.Item2.tilesWide.Value - 1, fpond.Item2.tileY.Value + fpond.Item2.tilesHigh.Value)
                        });
                    }

                    break;
                case AnimalsTaskId.PondsWithItems:
                    if (PondsWithItem.Count == 0) return rpItem;

                    foreach (Tuple<string, FishPond> fpond in PondsWithItem)
                    {
                        rpItem.Add(new ReportReturnItem
                        {
                            Label = $"Fish Pond{I18n.Tasks_At()}{FormatLocation(fpond.Item1, null, fpond.Item2.tileX, fpond.Item2.tileY)} have items to collect.",
                            WarpTo = Tuple.Create(fpond.Item1, fpond.Item2.tileX.Value + fpond.Item2.tilesWide.Value - 1, fpond.Item2.tileY.Value + fpond.Item2.tilesHigh.Value)
                        });
                    }

                    break;
                case AnimalsTaskId.AnimalProducts:
                    if (AnimalProductsToCollect.Count + TrufflesToCollect.Count + AnimalProductsToHarvest.Count == 0)
                        return rpItem;


                    foreach (TaskItem<FarmAnimal> animal in AnimalProductsToHarvest)
                    {
                        string currentProduce = animal.Object.currentProduce.Value;
                        if (!_config.ProductFromAnimal(currentProduce)) continue;


                        rpItem.Add(new ReportReturnItem
                        {
                            SortKey = animal.Object.displayName,
                            Label = $"{animal.Object.displayType} {animal.Object.displayName}{I18n.Tasks_Animal_Has()}{ObjectsNames[currentProduce]}{I18n.Tasks_At()}{FormatLocation(animal.Location.GetParentLocation().Name, animal.Location?.Name ?? "????", animal.Object.Tile)}",
                            WarpTo = animal.Location == null ? null : Tuple.Create(animal.Location.NameOrUniqueName, (int)animal.Object.Tile.X, (int)animal.Object.Tile.Y)
                        });
                    }

                    foreach (TaskItem<SDObject> product in AnimalProductsToCollect)
                    {
                        if (!_config.ProductToCollect(product.Object.ItemId)) continue;
                        Tuple<GameLocation?, GameLocation?> locationDetails = GetLocationAndBuilding(product.Location);
                        GameLocation? productLocation = locationDetails.Item1;
                        GameLocation? productBuilding = locationDetails.Item2;
                        
                        rpItem.Add(new ReportReturnItem
                        {
                            Label = $"{product.Object.DisplayName}{I18n.Tasks_At()}{FormatLocation(productLocation?.Name ?? "????", productBuilding?.Name, product.Position)}",
                            WarpTo = product.Location == null ? null : Tuple.Create(product.Location.NameOrUniqueName, (int)product.Position.X, (int)product.Position.Y)
                        });
                    }

                    if (!_config.AnimalProducts["Truffle"]) break;
                    foreach (TaskItem<SDObject> product in TrufflesToCollect)
                    {
                        Tuple<GameLocation?, GameLocation?> locationDetails = GetLocationAndBuilding(product.Location);
                        GameLocation? productLocation = locationDetails.Item1;
                        GameLocation? productBuilding = locationDetails.Item2;
                        rpItem.Add(new ReportReturnItem
                        {
                            Label = $"{product.Object.DisplayName}{I18n.Tasks_At()}{FormatLocation(productLocation?.Name ?? "???", productBuilding?.Name, product.Position)}",
                            WarpTo = product.Location == null ? null : Tuple.Create(product.Location.NameOrUniqueName, (int)product.Position.X, (int)product.Position.Y)
                        });
                    }
                    break;

                case AnimalsTaskId.MissingHay:
                    if (MissingHay.Count == 0) return rpItem;

                    foreach (Tuple<Building, int> tuple in MissingHay)
                    {
                        string s = tuple.Item2 == 1 ? I18n.Tasks_Animal_MissingHay() : I18n.Tasks_Animal_MissingHays();

                        rpItem.Add(new ReportReturnItem
                        {
                            Label = $"{tuple.Item2}{s}{FormatLocation(tuple.Item1.GetParentLocation().Name, tuple.Item1.indoors.Value.Name, tuple.Item1.tileX.Value, tuple.Item1.tileY.Value)}",
                            WarpTo = Tuple.Create(
                              tuple.Item1.indoors.Value.NameOrUniqueName,
                               tuple.Item1.tileX.Value,
                               tuple.Item1.tileY.Value
                            )
                        });
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException($"Animal task not implemented");
            }

            return rpItem;
        }
        private Tuple<GameLocation?, GameLocation?> GetLocationAndBuilding(GameLocation? gamelocation)
        {
            if( gamelocation == null )
            {
                return new Tuple<GameLocation?, GameLocation?>(null, null);
            }
            if (gamelocation?.GetParentLocation() != null)
            {
                return new Tuple<GameLocation?, GameLocation?>(gamelocation.GetParentLocation(), gamelocation);
            }
            else
            {
                return new Tuple<GameLocation?, GameLocation?>(gamelocation, null);
            }
        }
        public override List<ReportReturnItem> GeneralInfo()
        {
            List<ReportReturnItem> prItem = new List<ReportReturnItem> { };

            if (!Enabled) return prItem;

            int count = 0;

            UpdateList();

            switch ((AnimalsTaskId)TaskId)
            {
                case AnimalsTaskId.UnpettedAnimals:
                    if (UnpettedAnimals.Count > 0)
                    {
                        prItem.Add(new ReportReturnItem() { Label = I18n.Tasks_Animal_NotPetted(), Details = UnpettedAnimals.Count.ToString() });
                    }
                    break;
                case AnimalsTaskId.PondsNeedingAttention:
                    if (PondsNeedingAttention.Count > 0)
                    {
                        prItem.Add(new ReportReturnItem() { Label = I18n.Tasks_Ponds_Attention(), Details = PondsNeedingAttention.Count.ToString() });
                    }
                    break;
                case AnimalsTaskId.PondsWithItems:
                    if (PondsWithItem.Count > 0)
                    {
                        prItem.Add(new ReportReturnItem() { Label = I18n.Tasks_Ponds_Collect(), Details = PondsWithItem.Count.ToString() });
                    }
                    break;
                case AnimalsTaskId.AnimalProducts:
                    if (_config.AnimalProducts["Truffle"])
                        count = TrufflesToCollect.Count;
                    count += AnimalProductsToCollect.Count(p => _config.ProductToCollect(p.Object.ItemId));
                    count += AnimalProductsToHarvest.Count(p => _config.ProductFromAnimal(p.Object.currentProduce.Value));
                    if (count > 0)
                    {
                        prItem.Add(new ReportReturnItem() { Label = I18n.Tasks_Animal_Uncollected(), Details = count.ToString() });
                    }
                    break;

                case AnimalsTaskId.MissingHay:
                    count = MissingHay.Sum(t => t.Item2);
                    if (count > 0)
                    {
                        prItem.Add(new ReportReturnItem() { Label = I18n.Tasks_Animal_EmptyHay(), Details = count.ToString() });
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown animal task");
            }

            return prItem;
        }

        internal override void FirstScan()
        {
            if (_who == AnimalsTaskId.None)
                _who = (AnimalsTaskId)TaskId;
            else if (_who != (AnimalsTaskId)TaskId)
                return;

            if (ObjectsNames.Count == 0)
                PopulateObjectsNames();

            _gameFarms = Game1.locations.Where(l => l.IsBuildableLocation()).ToList();

            // Checking animals left outside
            CheckAnimals(_gameFarms);


            foreach (GameLocation farm in _gameFarms)
            {
                foreach (Building? building in farm.buildings)
                {
                    if (building.isUnderConstruction()) continue;

                    switch (building.indoors.Value)
                    {
                        case AnimalHouse animalHouse:
                            // Check animals
                            CheckAnimals(animalHouse);

                            // Check for object in Coop
                            if (building.GetData()?.ValidOccupantTypes?.Contains("Coop") ?? false)
                                CheckAnimalProductsInCoop(animalHouse);

                            // Check for hay
                            int count = animalHouse.numberOfObjectsWithName("Hay");
                            int animalLimit = animalHouse.animalLimit.Value;
                            if (count < animalLimit)
                                MissingHay.Add(new Tuple<Building, int>(building, animalLimit - count));
                            break;

                        case SlimeHutch slimeHutch:
                            // Check slime balls
                            foreach (KeyValuePair<Vector2, SDObject> pair in building.indoors.Value.objects.Pairs)

                                if (new List<string> { "(O)56", "(O)57", "(O)58", "(O)59", "(O)60", "(O)61" }.Contains(pair.Value.QualifiedItemId))
                                    AnimalProductsToCollect.Add(new TaskItem<SDObject>(slimeHutch, pair.Key, pair.Value));
                            break;
                        default:
                            break;
                    }
                    if (building is FishPond fishpond)
                    {
                        if (fishpond.HasUnresolvedNeeds())
                        {
                            PondsNeedingAttention.Add(Tuple.Create(farm.Name, fishpond));
                        }
                        if (fishpond.output.Value != null)
                        {
                            PondsWithItem.Add(Tuple.Create(farm.Name, fishpond));
                        }
                    }
                }
            }
            CheckForTruffles(_gameFarms);
        }
    }
}
