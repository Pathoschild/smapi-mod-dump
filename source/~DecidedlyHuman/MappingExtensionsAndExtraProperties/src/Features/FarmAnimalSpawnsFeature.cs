/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using DecidedlyShared.Logging;
using DecidedlyShared.Utilities;
using HarmonyLib;
using MappingExtensionsAndExtraProperties.Models.FarmAnimals;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace MappingExtensionsAndExtraProperties.Features;

public class FarmAnimalSpawnsFeature : Feature
{
    public override string FeatureId { get; init; }
    public override Harmony HarmonyPatcher { get; init; }
    public override bool AffectsCursorIcon { get; init; }
    public override int CursorId { get; init; }
    private static bool enabled;
    public override bool Enabled
    {
        get => enabled;
        internal set { enabled = value; }
    }

    private static Logger logger;
    private static Harmony harmony;
    private static IModHelper helper;
    private static Dictionary<string, Animal> animalData;
    private static Dictionary<FarmAnimal, Animal> spawnedAnimals = new Dictionary<FarmAnimal, Animal>();

    public FarmAnimalSpawnsFeature(Harmony harmony, string id, Logger logger, IModHelper helper)
    {
        this.Enabled = false;
        this.FeatureId = id;
        FarmAnimalSpawnsFeature.logger = logger;
        FarmAnimalSpawnsFeature.helper = helper;
        FarmAnimalSpawnsFeature.harmony = harmony;
    }

    public override void Enable()
    {
        try
        {
            FarmAnimalSpawnsFeature.harmony.Patch(
                AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.pet)),
                prefix: new HarmonyMethod(typeof(FarmAnimalSpawnsFeature),
                    nameof(FarmAnimalSpawnsFeature.FarmAnimalPetPrefix)));
        }
        catch (Exception e)
        {
            logger.Exception(e);
        }

        this.Enabled = true;
    }

    public override void Disable()
    {
        this.Enabled = false;
    }

    public override void RegisterCallbacks()
    {
        FeatureManager.OnDayStartCallback += this.OnDayStart;
        FeatureManager.EarlyDayEndingCallback += this.OnEarlyDayEnding;
    }

    private void OnEarlyDayEnding(object? sender, EventArgs e)
    {
        foreach (var animal in spawnedAnimals)
        {
            logger.Log($"Removing {animal.Key.displayName} of id {animal.Key.type} in {animal.Key.currentLocation.Name}.", LogLevel.Trace);
            animal.Key.currentLocation.animals.Remove(animal.Key.myID.Value);
        }
    }

    private void OnDayStart(object? sender, EventArgs e)
    {
        if (!Context.IsWorldReady || !Context.IsMainPlayer || !this.Enabled)
            return;

        // We technically only need to run this once, but this will be a super fast operation because it's cached.
        animalData = helper.GameContent.Load<Dictionary<string, Animal>>("MEEP/FarmAnimals/SpawnData");

        spawnedAnimals.Clear();

        // We need access to Game1.multiplayer. This is critical.
        Multiplayer multiplayer = helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();

        if (multiplayer is null)
        {
            // This is a catastrophic failure.
            logger.Log("Reflecting to get Game1.Multiplayer failed. As a result, we can't spawn any animals. This should never happen.", LogLevel.Error);

            return;
        }

        foreach (Animal animal in animalData.Values)
        {
            try
            {
                if (!GameStateQuery.CheckConditions(animal.Condition))
                {
                    logger.Log($"Condition to spawn {animal.DisplayName} was false. Skipping!", LogLevel.Trace);

                    continue;
                }

                GameLocation targetLocation = Game1.getLocationFromName(animal.LocationId);

                if (targetLocation is null)
                {
                    logger.Log($"Couldn't parse location name \"{animal.LocationId}\". Animal not spawned.",
                        LogLevel.Error);
                    continue;
                }

                // Sanity check time.
                if (animal.SkinId is null)
                    animal.SkinId = "";

                FarmAnimal babbyAnimal = new FarmAnimal(animal.AnimalId, multiplayer.getNewID(), -1L)
                {
                    skinID = { animal.SkinId },
                    age = { animal.Age }
                };

                babbyAnimal.Position =
                    new Vector2(animal.HomeTileX * Game1.tileSize, animal.HomeTileY * Game1.tileSize);
                babbyAnimal.Name = animal.DisplayName is null ? "No Name Boi" : animal.DisplayName;

                // We got a location, so we're good to check our GameStateQuery condition.

                targetLocation.animals.Add(babbyAnimal.myID.Value, babbyAnimal);
                babbyAnimal.update(Game1.currentGameTime, targetLocation);
                babbyAnimal.ReloadTextureIfNeeded();
                spawnedAnimals.Add(babbyAnimal, animal);

                logger.Log($"Animal {animal.AnimalId} spawned in {targetLocation.Name}.", LogLevel.Info);
            }
            catch (Exception ex)
            {
                logger.Log($"Caught an exception spawning {animal.AnimalId} spawned in {animal.LocationId}. Skipping!");
            }
        }
    }

    public override bool ShouldChangeCursor(GameLocation location, int tileX, int tileY, out int cursorId)
    {
        cursorId = default;
        return false;
    }

    public static bool FarmAnimalPetPrefix(FarmAnimal __instance, Farmer who, bool is_auto_pet)
    {
        if (!enabled)
            return true;

        // If we're dealing with one of our spawned animals, we display a nice message.
        if (spawnedAnimals.ContainsKey(__instance))
        {
            Vector2 messageSize = Geometry.GetLargestString(spawnedAnimals[__instance].PetMessage, Game1.dialogueFont);
            DialogueBox dialogue = new DialogueBox(spawnedAnimals[__instance].PetMessage.ToList());
            Game1.activeClickableMenu = dialogue;

            return false;
        }

        return true;
    }
}
