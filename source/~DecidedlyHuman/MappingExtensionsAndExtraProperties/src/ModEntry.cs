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
using DecidedlyShared.APIs;
using DecidedlyShared.Logging;
using DecidedlyShared.Utilities;
using HarmonyLib;
using MappingExtensionsAndExtraProperties.Api;
using MappingExtensionsAndExtraProperties.Features;
using MappingExtensionsAndExtraProperties.Functionality;
using MappingExtensionsAndExtraProperties.Models.FarmAnimals;
using MappingExtensionsAndExtraProperties.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace MappingExtensionsAndExtraProperties;

public class ModEntry : Mod
{
    private Logger logger;
    private TilePropertyHandler tileProperties;
    private Properties propertyUtils;
    private MeepApi api;
    private Harmony harmony;

    private ISaveAnywhereApi saveAnywhereApi;
    private ISpaceCoreApi spaceCoreApi;
    private EventCommands eventCommands;

    public override void Entry(IModHelper helper)
    {
        this.harmony = new Harmony(this.ModManifest.UniqueID);
        this.logger = new Logger(this.Monitor);
        this.tileProperties = new TilePropertyHandler(this.logger);
        this.propertyUtils = new Properties(this.logger);
        Parsers.InitialiseParsers(this.logger, helper);
        this.eventCommands = new EventCommands(this.Helper, this.logger);

        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

        this.LoadContentPacks();

        // This is where we kill all of our "fake" NPCs so they don't get serialised.
        helper.Events.GameLoop.DayEnding += this.OnDayEndingEarly;

        // Our asset loading.
        helper.Events.Content.AssetRequested += (sender, args) =>
        {
            if (args.NameWithoutLocale.IsDirectlyUnderPath("MEEP/FakeNPC/Dialogue"))
            {
                args.LoadFrom(() => new Dictionary<string, string>(), AssetLoadPriority.Low);
            }

            if (args.NameWithoutLocale.IsEquivalentTo("MEEP/FarmAnimals/SpawnData"))
            {
                args.LoadFrom(() => new Dictionary<string, Animal>(), AssetLoadPriority.Low);
            }
        };

        helper.Events.Player.Warped += this.PlayerOnWarped;
        helper.Events.GameLoop.DayStarted += this.OnDayStarted;
    }

    private void LoadContentPacks()
    {
        bool interactionsUsed = false;
        bool fakeNpcsUsed = false;
        bool vanillaLettersUsed = false;
        bool setMailFlagUsed = false;
        bool farmAnimalSpawningUsed = false;
        bool addConversationTopicUsed = false;


        foreach (var mod in this.Helper.ModRegistry.GetAll())
        {
            if (mod.Manifest.ExtraFields.ContainsKey("DH.MEEP"))
            {
                // This mod uses MEEP, so we want to check for the features.

                if (mod.Manifest.ExtraFields.ContainsKey("DH.MEEP.CloseupInteractions"))
                    interactionsUsed = true;
                if (mod.Manifest.ExtraFields.ContainsKey("DH.MEEP.FakeNPCs"))
                    fakeNpcsUsed = true;
                if (mod.Manifest.ExtraFields.ContainsKey("DH.MEEP.VanillaLetters"))
                    vanillaLettersUsed = true;
                if (mod.Manifest.ExtraFields.ContainsKey("DH.MEEP.SetMailFlag"))
                    setMailFlagUsed = true;
                if (mod.Manifest.ExtraFields.ContainsKey("DH.MEEP.FarmAnimalSpawns"))
                    farmAnimalSpawningUsed = true;
                if (mod.Manifest.ExtraFields.ContainsKey("DH.MEEP.AddConversationTopic"))
                    addConversationTopicUsed = true;
            }
        }

        // We've figured out all of our feature usage, so now we create and enable the appropriate features.
        // TODO: Make this more elegant in future. For now, this is 100% functionally fine.

        if (interactionsUsed)
        {
            CloseupInteractionFeature closeupInteractions = new CloseupInteractionFeature(
                this.harmony, "DH.CloseupInteractions", this.logger, this.tileProperties, this.propertyUtils);
            FeatureManager.AddFeature(closeupInteractions);
        }

        if (fakeNpcsUsed)
        {
            FakeNpcFeature fakeNpc = new FakeNpcFeature(this.harmony, "DH.FakeNPC", this.logger, this.tileProperties,
                this.propertyUtils, this.Helper);
            FeatureManager.AddFeature(fakeNpc);
        }

        if (vanillaLettersUsed)
        {
            LetterFeature letter = new LetterFeature(
                this.harmony, "DH.Letter", this.logger, this.tileProperties);
            FeatureManager.AddFeature(letter);
        }

        if (setMailFlagUsed)
        {
            SetMailFlagFeature setMailFlag =
                new SetMailFlagFeature(this.harmony, "DH.SetMailFlag", this.logger, this.tileProperties);
            FeatureManager.AddFeature(setMailFlag);
        }

        if (farmAnimalSpawningUsed)
        {
            FarmAnimalSpawnsFeature farmAnimals =
                new FarmAnimalSpawnsFeature(this.harmony, "DH.FarmAnimalSpawns", this.logger, this.Helper);
            FeatureManager.AddFeature(farmAnimals);
        }

        if (addConversationTopicUsed)
        {
            AddConversationTopicFeature conversationTopics =
                new AddConversationTopicFeature(this.harmony, "DH.AddConversationTopic", this.logger,
                    this.tileProperties);
            FeatureManager.AddFeature(conversationTopics);
        }

        if (FeatureManager.FeatureCount > 0)
        {
            CleanupFeature cleanup = new CleanupFeature("DH.Internal.CleanupFeature");
            FeatureManager.AddFeature(cleanup);
        }

        FeatureManager.EnableFeatures();
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs args)
    {
        if (this.Helper.ModRegistry.IsLoaded("Omegasis.SaveAnywhere"))
        {
            // Grab the Save Anywhere API so we can safely destroy our NPCs before it saves.
            try
            {
                this.saveAnywhereApi = this.Helper.ModRegistry.GetApi<ISaveAnywhereApi>("Omegasis.SaveAnywhere");
                this.saveAnywhereApi.BeforeSave += this.BeforeSaveAnywhereSave;
                this.saveAnywhereApi.AfterLoad += this.AfterSaveAnywhereLoad;
            }
            catch (Exception e)
            {
                this.logger.Exception(e);
            }
        }
    }

    private void AfterSaveAnywhereLoad(object? sender, EventArgs e)
    {
        this.logger.Log($"Save Anywhere fired its AfterLoad event. Processing our spawn map.", LogLevel.Info);

        FeatureManager.OnLocationChange(Game1.currentLocation, Game1.currentLocation, Game1.player);
    }

    private void BeforeSaveAnywhereSave(object? sender, EventArgs e)
    {
        this.logger.Log($"Save Anywhere fired its BeforeSave event. Killing NPCs early.", LogLevel.Info);

        FeatureManager.EarlyOnDayEnding();
    }

    private void PlayerOnWarped(object? sender, WarpedEventArgs args)
    {
        FeatureManager.OnLocationChange(args.OldLocation, args.NewLocation, args.Player);
    }

    // This is just to ensure we come before as many other DayEnding events as possible.
    [EventPriority((EventPriority)int.MaxValue)]
    private void OnDayEndingEarly(object? sender, DayEndingEventArgs e)
    {
        FeatureManager.EarlyOnDayEnding();
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        FeatureManager.OnDayStart();
    }

    public override object? GetApi()
    {
        return this.api;
    }
}
