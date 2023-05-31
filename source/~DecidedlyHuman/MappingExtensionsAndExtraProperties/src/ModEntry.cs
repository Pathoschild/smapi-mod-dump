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
using DecidedlyShared.Ui;
using DecidedlyShared.Utilities;
using HarmonyLib;
using MappingExtensionsAndExtraProperties.Api;
using MappingExtensionsAndExtraProperties.Models.TileProperties;
using MappingExtensionsAndExtraProperties.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using xTile.ObjectModel;
using xTile.Tiles;

namespace MappingExtensionsAndExtraProperties;

public class ModEntry : Mod
{
    private Logger logger;
    private TilePropertyHandler tileProperties;
    private MeepApi api;
    private List<FakeNpc> allNpcs;
    private ISaveAnywhereApi saveAnywhereApi;

    public override void Entry(IModHelper helper)
    {
        var harmony = new Harmony(this.ModManifest.UniqueID);
        this.logger = new Logger(this.Monitor);
        this.tileProperties = new TilePropertyHandler(this.logger);
        this.allNpcs = new List<FakeNpc>();
        Patches.InitialisePatches(this.logger, this.tileProperties);
        Parsers.InitialiseParsers(this.logger, helper);

        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

        helper.Events.Player.Warped += (sender, args) =>
        {
            // We need to ensure we can kill relevant UIs if the player is warping.
            if (Game1.activeClickableMenu is MenuBase menu)
            {
                // If it's one of our menus, we close it.
                // This should be refactored use an owned-menu system at some point.
                if (menu.MenuName.Equals(CloseupInteractionImage.PropertyKey))
                    Game1.exitActiveMenu();
            }

            // Then remove our fake NPCs from the previous map.
            // Utils.Locations.RemoveFakeNpcs(args.OldLocation);
        };

        // This is where we kill all of our "fake" NPCs so they don't get serialised.
        helper.Events.GameLoop.DayEnding += this.OnDayEnding;

        // Our asset loading.
        helper.Events.Content.AssetRequested += (sender, args) =>
        {
            if (args.NameWithoutLocale.IsDirectlyUnderPath("MEEP/FakeNPC/Dialogue"))
            {
                args.LoadFrom(() => { return new Dictionary<string, string>(); }, AssetLoadPriority.Low);
            }
        };

        // Our patch for handling interactions.
        harmony.Patch(
            AccessTools.Method(typeof(GameLocation), nameof(GameLocation.checkAction)),
            postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.GameLocation_CheckAction_Postfix)));

        // harmony.Patch(
        //     AccessTools.Method(typeof(Event), nameof(Event.checkAction)),
        //     postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.Event_CheckAction_Postfix)));
        //
        // harmony.Patch(
        //     AccessTools.Method(typeof(Event), nameof(Event.receiveActionPress)),
        //     postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.Event_ReceiveActionPress_Postfix)));

        // Our cursor draw patch for interaction highlights.
        harmony.Patch(
            AccessTools.Method(typeof(Game1), nameof(Game1.drawMouseCursor)),
            prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.Game1_drawMouseCursor_Prefix)));

        // We need this to handle items with integrated closeup interactions. Disabled for now.
        // harmony.Patch(
        //     AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.performUseAction)),
        //     prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.SObject_PerformUseAction)));

        helper.Events.Player.Warped += this.PlayerOnWarped;

#if DEBUG
        helper.Events.Display.RenderingWorld += (sender, args) =>
        {
            // args.SpriteBatch.DrawString(Game1.dialogueFont, "AAAAAAAAAAAAAAAAAAA", Vector2.Zero,
            //     Color.Red, 0f, Vector2.Zero, new Vector2(10, 10), SpriteEffects.None, 0f);
        };

        helper.Events.Player.Warped += (sender, args) =>
        {
            // TODO: Finish MEEP background functionality.
            string image = args.NewLocation.getMapProperty("MEEP_Background_Image");
            string tile = args.NewLocation.getMapProperty("MEEP_Background_Tile_Size");
            string variation = args.NewLocation.getMapProperty("MEEP_Background_Tile_Variation");

            int mapWidth = args.NewLocation.map.DisplayWidth;
            int mapHeight = args.NewLocation.map.DisplayHeight;

            // I want all of these for this test.
            if (image is null || tile is null || variation is null)
                return;

            bool imageParsed = Parsers.TryParse(image, out MapBackgroundImage backgroundImage);
            bool sizeParsed = Parsers.TryParse(tile, out MapBackgroundTileSize tileSize);
            bool variationParsed = Parsers.TryParse(variation, out MapBackgroundTileVariation tileVariation);

            if (!imageParsed || !sizeParsed || !variationParsed)
                return;

            Texture2D texture = Game1.content.Load<Texture2D>(backgroundImage.ImagePath);
            int numTiles = (texture.Width / tileSize.Width) * (texture.Height / tileSize.Height);
            Game1.background = new Background(texture, 1, mapWidth / tileSize.Width, mapHeight / tileSize.Height,
                tileSize.Width, tileSize.Height, 4f, 1, numTiles, 1d, Color.White);
        };

        helper.Events.Input.ButtonPressed += (sender, args) =>
        {
            // int cursorX = (int)Game1.currentCursorTile.X;
            // int cursorY = (int)Game1.currentCursorTile.Y;
            // GameLocation here = Game1.currentLocation;
            //
            // if (args.IsDown(SButton.OemSemicolon))
            // {
            //     // Item furnace = ObjectFactory.getItemFromDescription(1, 13, 1);
            //     // Game1.currentLocation.Objects.Add(Game1.currentCursorTile, (SObject)furnace);
            //     FakeNpc character = new FakeNpc(
            //         new AnimatedSprite("Characters\\NotAbigail", 0, 16, 32),
            //         Game1.currentCursorTile * 64f,
            //         2,
            //         "NotAbigail",
            //         this.logger
            //     );
            //
            //     string sheet = character.GetDialogueSheetName();
            //
            //     // character.CurrentDialogue.Push(new Dialogue("Hello!", Game1.getCharacterFromName("Abigail")));
            //     // character.CurrentDialogue.Push(new Dialogue("Hello!", Game1.getCharacterFromName("Abigail")));
            //     // character.CurrentDialogue.Push(new Dialogue("How are you doing today?", Game1.getCharacterFromName("Abigail")));
            //     // character.CurrentDialogue.Push(new Dialogue("I'm glad to hear!", Game1.getCharacterFromName("Abigail")));
            //     // character.Dialogue.Add("Wed", "Thing");
            //     // character.Dialogue.Add();
            //
            //     here.characters.Add(character);
            // }
        };
#endif
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
        // This is hacky and terrible, but this is also a hotfix. I'll live.
        this.logger.Log($"Save Anywhere fired its AfterLoad event. Processing our spawn map.", LogLevel.Info);

        this.ProcessNewLocation(Game1.currentLocation, Game1.currentLocation, Game1.player);
    }

    private void BeforeSaveAnywhereSave(object? sender, EventArgs e)
    {
        this.logger.Log($"Save Anywhere fired its BeforeSave event. Killing NPCs early.", LogLevel.Info);

        this.OnDayEnding(null, null);
    }

    private void PlayerOnWarped(object? sender, WarpedEventArgs args)
    {
        GameLocation oldLocation = args.OldLocation;
        GameLocation newLocation = args.NewLocation;
        Farmer player = args.Player;

        this.ProcessNewLocation(newLocation, oldLocation, player);
    }

    private void ProcessNewLocation(GameLocation newLocation, GameLocation oldLocation, Farmer player)
    {
        int mapWidth = newLocation.Map.GetLayer("Back").Tiles.Array.GetLength(0);
        int mapHeight = newLocation.Map.GetLayer("Back").Tiles.Array.GetLength(1);

        if (mapWidth == 0 || mapHeight == 0)
            return;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                // this.logger.Debug($"Processing tile {x}:{y} in map {args.NewLocation.Name}.");
                Tile tile;

                try
                {
                    tile = newLocation.Map.GetLayer("Back").Tiles.Array[x, y];
                }
                catch (Exception e)
                {
                    this.logger.Error(
                        $"Couldn't get tile {x}, {y} from map {newLocation.Name}. Exception follows.");
                    this.logger.Exception(e);

                    continue;
                }

                if (tile == null)
                    continue;

                if (tile.Properties.TryGetValue(DhFakeNpc.PropertyKey, out PropertyValue property))
                {
                    if (Parsers.TryParse(property.ToString(),
                            out DhFakeNpc fakeNpcProperty))
                    {
                        FakeNpc character = new FakeNpc(
                            new AnimatedSprite($"Characters\\{fakeNpcProperty.NpcName}",
                                0,
                                fakeNpcProperty.HasSpriteSizes ? fakeNpcProperty.SpriteWidth : 16,
                                fakeNpcProperty.HasSpriteSizes ? fakeNpcProperty.SpriteHeight : 32),
                            new Vector2(x, y) * 64f,
                            2,
                            fakeNpcProperty.NpcName,
                            this.logger,
                            newLocation
                        );
                        if (fakeNpcProperty.HasSpriteSizes)
                        {
                            if (fakeNpcProperty.SpriteWidth > 16)
                                character.HideShadow = true;

                            character.Breather = false;
                        }

                        Dictionary<string, string> dialogue =
                            this.Helper.GameContent.Load<Dictionary<string, string>>(
                                $"MEEP/FakeNPC/Dialogue/{fakeNpcProperty.NpcName}");

                        foreach (KeyValuePair<string, string> d in dialogue)
                        {
                            character.CurrentDialogue.Push(new Dialogue(d.Value, character));
                        }

                        // A safeguard for multiplayer.
                        if (!newLocation.isTileOccupied(new Vector2(x, y)))
                        {
                            newLocation.characters.Add(character);
                            this.allNpcs.Add(character);
                            this.logger.Log(
                                $"Fake NPC {character.Name} spawned in {newLocation.Name} at X:{x}, Y:{y}.",
                                LogLevel.Trace);
                        }
                    }
                    else
                    {
                        this.logger.Error($"Failed to parse property {property.ToString()}");
                    }
                }
            }
        }
    }

    // This is just to ensure we come before as many other DayEnding events as possible.
    [EventPriority((EventPriority)int.MaxValue)]
    private void OnDayEnding(object? sender, DayEndingEventArgs e)
    {
        foreach (FakeNpc npc in this.allNpcs)
        {
            npc.KillNpc();
        }

        // foreach (GameLocation location in Game1.locations)
        // {
        //     Utils.Locations.RemoveFakeNpcs(location, this.logger);
        // }
        //
        // foreach (Building building in Game1.getFarm().buildings)
        // {
        //     if (building.indoors.Value is GameLocation indoors)
        //         Utils.Locations.RemoveFakeNpcs(indoors, this.logger);
        //
        // }
    }

    public override object? GetApi()
    {
        return this.api;
    }
}
