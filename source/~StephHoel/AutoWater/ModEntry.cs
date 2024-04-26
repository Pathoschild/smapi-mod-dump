/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/StephHoel/ModsStardewValley
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace AutoWater;

public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        RemoveObsoleteFiles(helper, ["Utils.pdb", "AutoWater.pdb"]);

        helper.Events.GameLoop.DayStarted += WaterEverything;
    }

    private async void WaterEverything(object sender, EventArgs e)
    {
        foreach (var location in Game1.locations)
        {
            //Monitor.Log($"==========> {location.Name} is {(location is null ? "null" : "not null")}", LogLevel.Debug);

            // Garden Pots
            var objects = location.objects.Values.OfType<IndoorPot>();
            foreach (var pot in objects)
            {
                //Monitor.Log($"{location.Name} with Garden Pot to add water", LogLevel.Debug);
                pot.Water();
            }

            // Hoe Dirts
            var terrains = location.terrainFeatures.Values.OfType<HoeDirt>();
            foreach (var terrain in terrains)
            {
                //Monitor.Log($"{location.Name} with HoeDirt to add water", LogLevel.Debug);
                terrain.state.Value = 1;
            }
        }

        // Water to pet bowl
        try
        {
            var farmPetBowl = Game1.getFarm().getBuildingByType("Pet Bowl");
            PetBowl pet = (PetBowl)farmPetBowl;
            pet.watered.Value = true;
            // Monitor.Log("PetBowl is full", LogLevel.Debug);
        }
        catch
        { // For eventually error
            Monitor.Log("PetBowl do not exist", LogLevel.Debug);
        }
    }

    private void RemoveObsoleteFiles(IModHelper helper, string[] files)
    {
        foreach (var file in files)
        {
            string fullPath = Path.Combine(helper.DirectoryPath, file);
            if (File.Exists(fullPath))
            {
                try
                {
                    File.Delete(fullPath);
                    Monitor.Log($"Removed obsolete file '{file}'.", LogLevel.Debug);
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Failed deleting obsolete file '{file}':\n{ex}");
                }
            }
        }
    }
}