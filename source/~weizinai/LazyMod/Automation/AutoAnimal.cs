/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Automation;

internal class AutoAnimal : Automate
{
    public AutoAnimal(ModConfig config, Func<int, List<Vector2>> getTileGrid) : base(config, getTileGrid)
    {
    }

    public override void AutoDoFunction(GameLocation location, Farmer player, Tool? tool, Item? item)
    {
        // 自动抚摸动物
        if (this.Config.AutoPetAnimal.IsEnable) this.AutoPetAnimal(location, player);
        // 自动抚摸宠物
        if (this.Config.AutoPetPet.IsEnable) this.AutoPetPet(location, player);
        // 自动挤奶
        if (this.Config.AutoMilkAnimal.IsEnable && (tool is MilkPail || this.Config.AutoMilkAnimal.FindToolFromInventory)) this.AutoMilkAnimal(location, player);
        // 自动剪毛
        if (this.Config.AutoShearsAnimal.IsEnable && (tool is Shears || this.Config.AutoShearsAnimal.FindToolFromInventory)) this.AutoShearsAnimal(location, player);
        // 自动喂食动物饼干
        if (this.Config.AutoFeedAnimalCracker.IsEnable && item?.QualifiedItemId is "(O)GoldenAnimalCracker") this.AutoFeedAnimalCracker(location, player);
        // 自动打开栅栏门
        if (this.Config.AutoOpenFenceGate.IsEnable) this.AutoOpenFenceGate(location, player);
    }

    // 自动抚摸动物
    private void AutoPetAnimal(GameLocation location, Farmer player)
    {
        var grid = this.GetTileGrid(this.Config.AutoPetAnimal.Range);

        var animals = location.animals.Values;
        foreach (var animal in animals)
            foreach (var tile in grid)
                if (this.CanPetAnimal(tile, animal))
                    this.PetAnimal(player, animal);
    }

    // 自动抚摸宠物
    private void AutoPetPet(GameLocation location, Farmer player)
    {
        var grid = this.GetTileGrid(this.Config.AutoPetAnimal.Range);

        var pets = location.characters.OfType<Pet>();
        foreach (var pet in pets)
        {
            foreach (var tile in grid)
            {
                if (pet.GetBoundingBox().Intersects(this.GetTileBoundingBox(tile)) &&
                    (!pet.lastPetDay.TryGetValue(player.UniqueMultiplayerID, out var lastPetDay) || lastPetDay != Game1.Date.TotalDays))
                    pet.checkAction(player, location);
            }
        }
    }

    // 自动挤奶
    private void AutoMilkAnimal(GameLocation location, Farmer player)
    {
        if (player.Stamina <= this.Config.AutoMilkAnimal.StopStamina) return;
        if (player.freeSpotsInInventory() < 1) return;

        var milkPail = this.FindToolFromInventory<MilkPail>();
        if (milkPail is null) return;

        var grid = this.GetTileGrid(this.Config.AutoMilkAnimal.Range);
        foreach (var tile in grid)
        {
            var animal = this.GetBestHarvestableFarmAnimal(location, milkPail, tile);
            if (animal is null) continue;
            milkPail.animal = animal;
            this.UseToolOnTile(location, player, milkPail, tile);
        }
    }

    // 自动剪毛
    private void AutoShearsAnimal(GameLocation location, Farmer player)
    {
        if (player.Stamina <= this.Config.AutoShearsAnimal.StopStamina) return;
        if (player.freeSpotsInInventory() < 1) return;

        var shears = this.FindToolFromInventory<Shears>();
        if (shears is null)
            return;

        var grid = this.GetTileGrid(this.Config.AutoShearsAnimal.Range);
        foreach (var tile in grid)
        {
            var animal = this.GetBestHarvestableFarmAnimal(location, shears, tile);
            if (animal is null) continue;
            shears.animal = animal;
            this.UseToolOnTile(location, player, shears, tile);
        }
    }

    // 自动喂食动物饼干
    private void AutoFeedAnimalCracker(GameLocation location, Farmer player)
    {
        var grid = this.GetTileGrid(this.Config.AutoFeedAnimalCracker.Range);
        var animals = location.animals.Values;
        foreach (var animal in animals)
            foreach (var tile in grid)
                if (this.CanFeedAnimalCracker(tile, animal))
                    this.FeedAnimalCracker(player, animal);
    }

    // 自动打开动物门
    public static void AutoToggleAnimalDoor(bool isOpen)
    {
        if (isOpen && (Game1.isRaining || Game1.IsWinter))
            return;

        var buildableLocations = GetBuildableLocation().ToList();
        foreach (var location in buildableLocations)
        {
            foreach (var building in location.buildings)
            {
                // 如果该建筑没有动物门，或者动物门已经是目标状态，则跳过
                if (building.animalDoor is null || building.animalDoorOpen.Value == isOpen) continue;
                // 遍历所有的动物,将不在家的动物传送回家
                foreach (var animal in location.Animals.Values.Where(animal => !animal.IsHome && animal.home == building)) animal.warpHome();
                // 切换动物门状态
                building.ToggleAnimalDoor(Game1.player);
            }
        }
    }

    // 自动打开栅栏门
    private void AutoOpenFenceGate(GameLocation location, Farmer player)
    {
        var grid = this.GetTileGrid(this.Config.AutoOpenFenceGate.Range + 2);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is not Fence fence || !fence.isGate.Value)
                continue;

            var distance = this.GetDistance(player.Tile, tile);
            if (distance <= this.Config.AutoOpenFenceGate.Range && fence.gatePosition.Value == 0)
            {
                fence.toggleGate(player, true);
            }
            else if (distance > this.Config.AutoOpenFenceGate.Range + 1 && fence.gatePosition.Value != 0)
            {
                fence.toggleGate(player, false);
            }
        }
    }

    private FarmAnimal? GetBestHarvestableFarmAnimal(GameLocation location, Tool tool, Vector2 tile)
    {
        var animal = Utility.GetBestHarvestableFarmAnimal(location.Animals.Values, tool, this.GetTileBoundingBox(tile));
        if (animal?.currentProduce.Value is null || animal.isBaby() || !animal.CanGetProduceWithTool(tool))
            return null;

        return animal;
    }

    private static IEnumerable<GameLocation> GetBuildableLocation()
    {
        return Game1.locations.Where(location => location.IsBuildableLocation());
    }

    private int GetDistance(Vector2 origin, Vector2 tile)
    {
        return Math.Max(Math.Abs((int)(origin.X - tile.X)), Math.Abs((int)(origin.Y - tile.Y)));
    }

    private bool CanPetAnimal(Vector2 tile, FarmAnimal animal)
    {
        return animal.GetBoundingBox().Intersects(this.GetTileBoundingBox(tile)) &&
               !animal.wasPet.Value &&
               (animal.isMoving() || Game1.timeOfDay < 1900) &&
               !animal.Name.StartsWith("DH.MEEP.SpawnedAnimal_");
    }


    private void PetAnimal(Farmer player, FarmAnimal animal)
    {
        animal.wasPet.Value = true;

        // 好感度和心情逻辑
        var data = animal.GetAnimalData();
        var happinessDrain = data?.HappinessDrain ?? 0;
        animal.friendshipTowardFarmer.Value = animal.wasAutoPet.Value
            ? Math.Min(1000, animal.friendshipTowardFarmer.Value + 7)
            : Math.Min(1000, animal.friendshipTowardFarmer.Value + 15);
        animal.happiness.Value = Math.Min(255, animal.happiness.Value + Math.Max(5, 30 + happinessDrain));
        if (data is { ProfessionForHappinessBoost: >= 0 } && player.professions.Contains(data.ProfessionForHappinessBoost))
        {
            animal.friendshipTowardFarmer.Value = Math.Min(1000, animal.friendshipTowardFarmer.Value + 15);
            animal.happiness.Value = Math.Min(255, animal.happiness.Value + Math.Max(5, 30 + happinessDrain));
        }

        // 标签逻辑
        var emoteIndex = animal.wasAutoPet.Value ? 20 : 32;
        animal.doEmote(animal.moodMessage.Value == 4 ? 12 : emoteIndex);

        // 声音逻辑
        animal.makeSound();

        // 经验逻辑
        player.gainExperience(0, 5);
    }

    private bool CanFeedAnimalCracker(Vector2 tile, FarmAnimal animal)
    {
        return animal.GetBoundingBox().Intersects(this.GetTileBoundingBox(tile)) &&
               !animal.hasEatenAnimalCracker.Value &&
               (animal.GetAnimalData()?.CanEatGoldenCrackers ?? false);
    }

    private void FeedAnimalCracker(Farmer player, FarmAnimal animal)
    {
        animal.hasEatenAnimalCracker.Value = true;
        Game1.playSound("give_gift");
        animal.doEmote(56);
        player.reduceActiveItemByOne();
    }
}