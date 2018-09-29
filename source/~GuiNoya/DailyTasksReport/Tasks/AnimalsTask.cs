using DailyTasksReport.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SObject = StardewValley.Object;

namespace DailyTasksReport.Tasks
{
    public class AnimalsTask : Task
    {
        private static readonly int[] CollectableAnimalProducts = { 107, 174, 176, 180, 182, 440, 442, 444, 446 };

        private static readonly List<TaskItem<SObject>> AnimalProductsToCollect = new List<TaskItem<SObject>>();
        private static readonly List<TaskItem<SObject>> TrufflesToCollect = new List<TaskItem<SObject>>();
        private static readonly List<TaskItem<FarmAnimal>> AnimalProductsToHarvest = new List<TaskItem<FarmAnimal>>();
        private static readonly List<TaskItem<FarmAnimal>> UnpettedAnimals = new List<TaskItem<FarmAnimal>>();
        private static readonly List<Tuple<Building, int>> MissingHay = new List<Tuple<Building, int>>();

        private readonly ModConfig _config;
        private readonly AnimalsTaskId _id;
        private static AnimalsTaskId _who = AnimalsTaskId.None;
        private static Farm _farm;

        internal AnimalsTask(ModConfig config, AnimalsTaskId id)
        {
            _config = config;
            _id = id;

            SettingsMenu.ReportConfigChanged += SettingsMenu_ReportConfigChanged;

            if (id == AnimalsTaskId.UnpettedAnimals)
            {
                MenuEvents.MenuChanged += MenuEvents_MenuChanged;
                MenuEvents.MenuClosed += MenuEvents_MenuClosed;
            }
        }

        private void SettingsMenu_ReportConfigChanged(object sender, EventArgs e)
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

                default:
                    throw new ArgumentOutOfRangeException($"Unknown animal task");
            }
        }

        private static void MenuEvents_MenuClosed(object sender, EventArgsClickableMenuClosed e)
        {
            if (e.PriorMenu is PurchaseAnimalsMenu ||
                e.PriorMenu is NamingMenu ||
                e.PriorMenu.GetType().FullName == "FarmExpansion.Menus.FEPurchaseAnimalsMenu")
                ReScanUnpettedAnimals();
        }

        private static void MenuEvents_MenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
            if (e.PriorMenu is PurchaseAnimalsMenu ||
                e.PriorMenu is NamingMenu ||
                e.PriorMenu?.GetType().FullName == "FarmExpansion.Menus.FEPurchaseAnimalsMenu")
                ReScanUnpettedAnimals();
        }

        private static void ReScanUnpettedAnimals()
        {
            UnpettedAnimals.Clear();

            foreach (var animal in _farm.animals.Pairs)
                if (!animal.Value.wasPet.Value)
                    UnpettedAnimals.Add(new TaskItem<FarmAnimal>(_farm, animal.Value.Position, animal.Value));

            foreach (var building in _farm.buildings)
            {
                if (building.isUnderConstruction() || !(building.indoors.Value is AnimalHouse animalHouse))
                    continue;

                foreach (var animal in animalHouse.animals.Pairs)
                    if (!animal.Value.wasPet.Value)
                        UnpettedAnimals.Add(new TaskItem<FarmAnimal>(animalHouse, animal.Value.Position, animal.Value));
            }
        }

        protected override void FirstScan()
        {
            if (_who == AnimalsTaskId.None)
                _who = _id;
            else if (_who != _id)
                return;

            if (ObjectsNames.Count == 0)
                PopulateObjectsNames();

            _farm = Game1.locations.First(l => l is Farm) as Farm;

            // Checking animals left outside
            CheckAnimals(_farm);

            // ReSharper disable once PossibleNullReferenceException
            foreach (var building in _farm.buildings)
            {
                if (building.isUnderConstruction()) continue;

                switch (building.indoors.Value)
                {
                    case AnimalHouse animalHouse:
                        // Check animals
                        CheckAnimals(animalHouse);

                        // Check for object in Coop
                        if (building is Coop)
                            CheckAnimalProductsInCoop(animalHouse);

                        // Check for hay
                        var count = animalHouse.numberOfObjectsWithName("Hay");
                        var animalLimit = animalHouse.animalLimit.Value;
                        if (count < animalLimit)
                            MissingHay.Add(new Tuple<Building, int>(building, animalLimit - count));
                        break;

                    case SlimeHutch slimeHutch:
                        // Check slime balls
                        foreach (var pair in building.indoors.Value.objects.Pairs)
                            if (pair.Value.ParentSheetIndex >= 56 && pair.Value.ParentSheetIndex <= 61)
                                AnimalProductsToCollect.Add(new TaskItem<SObject>(slimeHutch, pair.Key, pair.Value));
                        break;

                    default:
                        break;
                }
            }

            CheckForTruffles(_farm);
        }

        private void UpdateList()
        {
            switch (_id)
            {
                case AnimalsTaskId.UnpettedAnimals:
                    UnpettedAnimals.RemoveAll(a => a.Object.wasPet.Value || a.Object.health.Value <= 0);
                    foreach (var animal in UnpettedAnimals)
                        animal.Location = _farm.animals.FieldDict.ContainsKey(animal.Object.myID.Value)
                            ? _farm
                            : animal.Object.home.indoors.Value;
                    break;

                case AnimalsTaskId.AnimalProducts:
                    AnimalProductsToCollect.RemoveAll(i =>
                        !(i.Location.objects.TryGetValue(i.Position, out var obj) &&
                          obj.ParentSheetIndex == i.Object.ParentSheetIndex));
                    TrufflesToCollect.Clear();
                    CheckForTruffles(_farm);
                    AnimalProductsToHarvest.RemoveAll(animal =>
                        animal.Object.currentProduce.Value <= 0 || animal.Object.health.Value <= 0);
                    foreach (var animal in AnimalProductsToHarvest)
                        animal.Location = _farm.animals.ContainsKey(animal.Object.myID.Value)
                            ? _farm
                            : animal.Object.home.indoors.Value;
                    break;

                case AnimalsTaskId.MissingHay:
                    MissingHay.Clear();
                    foreach (var building in _farm.buildings)
                    {
                        if (building.isUnderConstruction() || !(building.indoors.Value is AnimalHouse animalHouse)) continue;
                        var count = animalHouse.numberOfObjectsWithName("Hay");
                        var animalLimit = animalHouse.animalLimit.Value;
                        if (count < animalLimit)
                            MissingHay.Add(new Tuple<Building, int>(building, animalLimit - count));
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException($"Unknown animal task");
            }
        }

        private static void CheckAnimals(Farm farm)
        {
            foreach (var animal in farm.animals.Pairs)
            {
                if (!animal.Value.wasPet.Value)
                    UnpettedAnimals.Add(new TaskItem<FarmAnimal>(farm, animal.Value.Position, animal.Value));

                var currentProduce = animal.Value.currentProduce.Value;
                if (currentProduce > 0 && currentProduce != 430)
                    AnimalProductsToHarvest.Add(new TaskItem<FarmAnimal>(farm, animal.Value.Position, animal.Value));
            }
        }

        private static void CheckAnimals(AnimalHouse location)
        {
            foreach (var animal in location.animals.Pairs)
            {
                if (!animal.Value.wasPet.Value)
                    UnpettedAnimals.Add(new TaskItem<FarmAnimal>(location, animal.Value.position, animal.Value));

                var currentProduce = animal.Value.currentProduce.Value;
                if (currentProduce > 0 && currentProduce != 430)
                    AnimalProductsToHarvest.Add(new TaskItem<FarmAnimal>(location, animal.Value.position,
                        animal.Value));
            }
        }

        private static void CheckAnimalProductsInCoop(GameLocation coop)
        {
            foreach (var pair in coop.objects.Pairs)
                if (Array.BinarySearch(CollectableAnimalProducts, pair.Value.ParentSheetIndex) >= 0 &&
                    (int)pair.Key.X <= coop.map.DisplayWidth / Game1.tileSize &&
                    (int)pair.Key.Y <= coop.map.DisplayHeight / Game1.tileSize)
                    AnimalProductsToCollect.Add(new TaskItem<SObject>(coop, pair.Key, pair.Value));
        }

        private static void CheckForTruffles(GameLocation farm)
        {
            foreach (var pair in farm.objects.Pairs)
                if (pair.Value.ParentSheetIndex == 430)
                    TrufflesToCollect.Add(new TaskItem<SObject>(farm, pair.Key, pair.Value));
        }

        public override string GeneralInfo(out int usedLines)
        {
            usedLines = 0;

            if (!Enabled) return "";

            var count = 0;
            usedLines = 1;

            UpdateList();

            switch (_id)
            {
                case AnimalsTaskId.UnpettedAnimals:
                    if (UnpettedAnimals.Count > 0)
                        return $"Not petted animals: {UnpettedAnimals.Count}^";
                    break;

                case AnimalsTaskId.AnimalProducts:
                    if (_config.AnimalProducts["Truffle"])
                        count = TrufflesToCollect.Count;
                    count += AnimalProductsToCollect.Count(p => _config.ProductToCollect(p.Object.ParentSheetIndex));
                    count += AnimalProductsToHarvest.Count(p => _config.ProductFromAnimal(p.Object.currentProduce.Value));
                    if (count > 0)
                        return $"Uncollected animal products: {count}^";
                    break;

                case AnimalsTaskId.MissingHay:
                    count = MissingHay.Sum(t => t.Item2);
                    if (count > 0)
                        return $"Empty hay spots on feeding benches: {count}^";
                    break;

                default:
                    throw new ArgumentOutOfRangeException($"Unknown animal task");
            }

            usedLines = 0;

            return "";
        }

        public override string DetailedInfo(out int usedLines, out bool skipNextPage)
        {
            usedLines = 0;
            skipNextPage = false;

            if (!Enabled) return "";

            var stringBuilder = new StringBuilder();

            switch (_id)
            {
                case AnimalsTaskId.UnpettedAnimals:
                    if (UnpettedAnimals.Count == 0) return "";

                    stringBuilder.Append("Not petted animals:^");
                    usedLines++;

                    foreach (var animal in UnpettedAnimals)
                    {
                        stringBuilder.Append(
                            $"{animal.Object.type} {animal.Object.displayName} at {animal.Location.Name} ({animal.Object.getTileX()}, {animal.Object.getTileY()})^");
                        usedLines++;
                    }
                    break;

                case AnimalsTaskId.AnimalProducts:
                    if (AnimalProductsToCollect.Count + TrufflesToCollect.Count + AnimalProductsToHarvest.Count == 0)
                        return "";

                    stringBuilder.Append("Animal products:^");
                    usedLines++;

                    foreach (var animal in AnimalProductsToHarvest)
                    {
                        var currentProduce = animal.Object.currentProduce.Value;
                        if (!_config.ProductFromAnimal(currentProduce)) continue;
                        stringBuilder.Append(
                            $"{animal.Object.type} {animal.Object.displayName} has {ObjectsNames[currentProduce]} at {animal.Location.Name} ({animal.Object.getTileX()}, {animal.Object.getTileY()})^");
                        usedLines++;
                    }

                    foreach (var product in AnimalProductsToCollect)
                    {
                        if (!_config.ProductToCollect(product.Object.ParentSheetIndex)) continue;
                        stringBuilder.Append(
                            $"{product.Object.Name} at {product.Location.Name} ({product.Position.X}, {product.Position.Y})^");
                        usedLines++;
                    }

                    if (!_config.AnimalProducts["Truffle"]) break;
                    foreach (var product in TrufflesToCollect)
                    {
                        stringBuilder.Append(
                            $"{product.Object.Name} at {product.Location.Name} ({product.Position.X}, {product.Position.Y})^");
                        usedLines++;
                    }
                    break;

                case AnimalsTaskId.MissingHay:
                    if (MissingHay.Count == 0) return "";

                    stringBuilder.Append("Feedbenches not full of hay:^");
                    usedLines++;

                    foreach (var tuple in MissingHay)
                    {
                        var s = tuple.Item2 == 1 ? string.Empty : "s";

                        stringBuilder.Append(
                            $"{tuple.Item2} hay{s} missing at {tuple.Item1.indoors.Value.Name} ({tuple.Item1.tileX}, {tuple.Item1.tileY})^");
                        usedLines++;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException($"Animal task not implemented");
            }

            return stringBuilder.ToString();
        }

        public override void Draw(SpriteBatch b)
        {
            if (_id != _who || !(Game1.currentLocation is Farm) && !(Game1.currentLocation is AnimalHouse)) return;

            // Truffles
            if (_config.DrawBubbleTruffles && Game1.currentLocation is Farm)
            {
                var x = Game1.viewport.X / Game1.tileSize;
                var xLimit = (Game1.viewport.X + Game1.viewport.Width) / Game1.tileSize;
                var yStart = Game1.viewport.Y / Game1.tileSize;
                var yLimit = (Game1.viewport.Y + Game1.viewport.Height) / Game1.tileSize + 1;
                for (; x <= xLimit; ++x)
                    for (var y = yStart; y <= yLimit; ++y)
                    {
                        if (!Game1.currentLocation.objects.TryGetValue(new Vector2(x, y), out var o)) continue;

                        var v = new Vector2(o.TileLocation.X * Game1.tileSize - Game1.viewport.X + Game1.tileSize / 8f,
                            o.TileLocation.Y * Game1.tileSize - Game1.viewport.Y - Game1.tileSize * 2 / 4f);
                        if (o.name == "Truffle")
                            DrawBubble(Game1.spriteBatch, Game1.objectSpriteSheet, new Rectangle(352, 273, 14, 14), v);
                    }
            }

            // Animals

            var animalDict = (Game1.currentLocation as Farm)?.animals ??
                             (Game1.currentLocation as AnimalHouse)?.animals;

            if (animalDict == null) return;

            foreach (var animal in animalDict.Pairs)
            {
                if (animal.Value.isEmoting) continue;

                var currentProduce = animal.Value.currentProduce.Value;

                var needsPet = _config.DrawBubbleUnpettedAnimals && !animal.Value.wasPet.Value;
                var hasProduct = currentProduce != 430 &&
                                 currentProduce > 0 &&
                                 _config.DrawBubbleAnimalsWithProduce;

                var v = new Vector2(animal.Value.getStandingX() - Game1.viewport.X,
                    animal.Value.getStandingY() - Game1.viewport.Y);
                if (animal.Value.home is Coop)
                {
                    v.X -= Game1.tileSize * 0.3f;
                    v.Y -= Game1.tileSize * 6 / 4f;
                }
                else
                {
                    v.X -= Game1.tileSize * 0.2f;
                    v.Y -= Game1.tileSize * 2f;
                }

                if (needsPet)
                {
                    if (hasProduct)
                    {
                        DrawBubble2Icons(b, Game1.mouseCursors, new Rectangle(117, 7, 9, 8),
                            Game1.objectSpriteSheet,
                            Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet,
                                currentProduce, 16, 16),
                            v);
                        continue;
                    }
                    DrawBubble(b, Game1.mouseCursors, new Rectangle(117, 7, 9, 8), v);
                }
                else if (hasProduct)
                {
                    DrawBubble(b, Game1.objectSpriteSheet,
                        Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, currentProduce,
                            16, 16),
                        v);
                }
            }

            // Animal Houses

            if (!(Game1.currentLocation is Farm farm)) return;

            foreach (var building in farm.buildings)
                if (building.indoors.Value is AnimalHouse animalHouse)
                {
                    var anyHayMissing = _config.DrawBubbleBuildingsMissingHay &&
                                        animalHouse.numberOfObjectsWithName("Hay") < animalHouse.animalLimit.Value;
                    var anyProduce = _config.DrawBubbleBuildingsWithProduce && building is Coop &&
                                     animalHouse.objects.Values.Any(o =>
                                         Array.BinarySearch(CollectableAnimalProducts, o.ParentSheetIndex) >= 0);

                    var v = new Vector2(building.tileX.Value * Game1.tileSize - Game1.viewport.X + Game1.tileSize * 1.1f,
                        building.tileY.Value * Game1.tileSize - Game1.viewport.Y + Game1.tileSize / 2);

                    if (building is Barn)
                        v.Y += Game1.tileSize / 2f;

                    if (anyHayMissing)
                    {
                        if (anyProduce)
                        {
                            DrawBubble2Icons(b, Game1.mouseCursors, new Rectangle(32, 0, 10, 10),
                                Game1.objectSpriteSheet, new Rectangle(160, 112, 16, 16), v);
                            continue;
                        }
                        DrawBubble(b, Game1.objectSpriteSheet, new Rectangle(160, 112, 16, 16), v);
                    }
                    else if (anyProduce)
                    {
                        DrawBubble(b, Game1.mouseCursors, new Rectangle(32, 0, 10, 10), v);
                    }
                }
        }

        public override void Clear()
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

                default:
                    throw new ArgumentOutOfRangeException($"Animal task not implemented");
            }

            if (_id != _who) return;

            UnpettedAnimals.Clear();
            AnimalProductsToHarvest.Clear();
            AnimalProductsToCollect.Clear();
            TrufflesToCollect.Clear();
            MissingHay.Clear();
        }
    }

    public enum AnimalsTaskId
    {
        None = -1,
        UnpettedAnimals = 0,
        AnimalProducts = 1,
        MissingHay = 2
    }
}