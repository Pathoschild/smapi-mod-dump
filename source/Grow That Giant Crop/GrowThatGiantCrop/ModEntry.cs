/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MercuryVN/GrowThatGiantCrop
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.GiantCrops;
using StardewValley.ItemTypeDefinitions;
using StardewValley.TerrainFeatures;

namespace GrowThatGiantCrop;

internal sealed class ModEntry : Mod
{
    private IModHelper? helper;
    private IGenericModConfigMenuApi? configMenu;
    private ModConfig? config;
    private Farmer? player;

    public override void Entry(IModHelper helper)
    {
        this.helper = helper;
        config = helper.ReadConfig<ModConfig>();
        helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
        helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
        helper.Events.Input.ButtonPressed += Input_ButtonPressed; ;
    }

    private void GameLoop_GameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu == null) return;
        configMenu.Register(
            mod: ModManifest,
            reset: () => config = new ModConfig(),
            save: () => helper!.WriteConfig(config!)
        );
        configMenu.AddKeybind(
            mod: ModManifest,
            getValue: () => config!.GrowCropButton.Buttons[0],
            setValue: value => config!.GrowCropButton.Buttons[0] = value,
            name: () => "Grow crop button"
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            getValue: () => config!.EnableMod,
            setValue: value => config!.EnableMod = value,
            name: () => "Enable mod"
        );
    }

    private void GameLoop_SaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        _ = Task.Run(AssignPlayerAsync);
    }

    private void Input_ButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (helper == null || config == null || player == null || !Context.IsPlayerFree) return;
        if (e.Button == config.GrowCropButton.Buttons[0] && config.EnableMod)
        {
            string result = AttempGrowGiantCrop(player);
            Monitor.Log(result, LogLevel.Info);
        }
    }

    private string AttempGrowGiantCrop(Farmer player)
    {
        Vector2 playerTile = player.Tile;
        string result = "No eligible giant crop tiles";
        if (Game1.currentLocation == null)
        {
            Monitor.Log("Null location", LogLevel.Trace);
            return result;
        }
        if (!Game1.currentLocation.terrainFeatures.TryGetValue(playerTile, out TerrainFeature terrainFeature))
        {
            Monitor.Log("Null Terrain", LogLevel.Trace);
            return result;
        }
        if (terrainFeature is not HoeDirt dirt || dirt.crop == null)
        {
            if (terrainFeature is not HoeDirt) Monitor.Log("Not hoe dirt", LogLevel.Trace);
            else Monitor.Log("Null crop", LogLevel.Trace);
            return result;
        }
        if (!dirt.crop.TryGetGiantCrops(out IReadOnlyList<KeyValuePair<string, GiantCropData>>? giantCrops))
        {
            Monitor.Log($"{ItemRegistry.GetData(dirt.crop.indexOfHarvest.Value).InternalName} is not giant crop", LogLevel.Trace);
            return result;
        }
        if (!dirt.isWatered())
        {
            Monitor.Log("Not watered", LogLevel.Trace);
            return result;
        }
        if (!dirt.crop.RegrowsAfterHarvest() && dirt.crop.currentPhase.Value != dirt.crop.phaseDays.Count - 1)
        {
            Monitor.Log("Not fully grown", LogLevel.Trace);
            return result;
        }
        if (dirt.crop.RegrowsAfterHarvest() && !dirt.crop.fullyGrown.Value)
        {
            Monitor.Log("Not fully grown", LogLevel.Trace);
            return result;
        }
        ParsedItemData cropData = ItemRegistry.GetData(dirt.crop.indexOfHarvest.Value);
        if (!CanGrowGiantCrop(cropData.QualifiedItemId, playerTile))
        {
            Monitor.Log("Not top-left corner", LogLevel.Trace);
            return result;
        }
        GrowGiantCrop(playerTile, giantCrops);
        result = $"{cropData.InternalName} grows into giant crop at {playerTile}";
        return result;
    }

    private static bool CanGrowGiantCrop(string cropId, Vector2 tile)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Vector2 currentTile = new(tile.X + i, tile.Y + j);
                if (Game1.currentLocation == null) return false;
                if (!Game1.currentLocation.terrainFeatures.TryGetValue(currentTile, out TerrainFeature terrainFeature)) return false;
                if (terrainFeature is not HoeDirt dirt) return false;
                if (dirt.crop == null) return false;
                if (!dirt.crop.TryGetGiantCrops(out _)) return false;
                if (ItemRegistry.GetData(dirt.crop.indexOfHarvest.Value).QualifiedItemId != cropId) return false;
                if (!dirt.crop.RegrowsAfterHarvest() && dirt.crop.currentPhase.Value != dirt.crop.phaseDays.Count - 1) return false;
                if (dirt.crop.RegrowsAfterHarvest() && !dirt.crop.fullyGrown.Value) return false;
                if (!dirt.isWatered()) return false;
            }
        }
        return true;
    }

    private static void GrowGiantCrop(Vector2 tile, IReadOnlyList<KeyValuePair<string, GiantCropData>> giantCrops)
    {
        foreach (KeyValuePair<string, GiantCropData> item in giantCrops)
        {
            string key = item.Key;
            GiantCropData value = item.Value;
            GiantCrop giantCrop = new(key, tile);
            for (int i = 0; i < value.TileSize.X; i++)
            {
                for (int j = 0; j < value.TileSize.Y; j++)
                {
                    Vector2 cropTile = new(tile.X + i, tile.Y + j);
                    (Game1.currentLocation.terrainFeatures[cropTile] as HoeDirt)!.crop = null;
                }
            }
            Game1.currentLocation.resourceClumps.Add(giantCrop);
            break;
        }
    }

    private async Task AssignPlayerAsync()
    {
        while (Game1.player == null) await Task.Delay(500);
        player = Game1.player;
    }
}