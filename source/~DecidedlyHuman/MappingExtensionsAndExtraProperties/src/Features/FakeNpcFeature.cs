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
using System.Diagnostics;
using DecidedlyShared.Logging;
using DecidedlyShared.Utilities;
using HarmonyLib;
using MappingExtensionsAndExtraProperties.Models.EventArgs;
using MappingExtensionsAndExtraProperties.Models.TileProperties;
using MappingExtensionsAndExtraProperties.Utils;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using xTile.Dimensions;
using xTile.ObjectModel;
using xTile.Tiles;

namespace MappingExtensionsAndExtraProperties.Features;

public class FakeNpcFeature : Feature
{
        public sealed override Harmony HarmonyPatcher { get; init; }
    public sealed override bool AffectsCursorIcon { get; init; }
    public sealed override int CursorId { get; init; }
    private string[] tilePropertiesControlled = [
        "MEEP_FakeNPC"];

    public sealed override bool Enabled
    {
        get => enabled;
        internal set => enabled = value;
    }
    private static bool enabled;

    public sealed override string FeatureId { get; init; }
    private static TilePropertyHandler tileProperties;
    private static Properties propertyUtils;
    private static Logger logger;
    private static IModHelper helper;

    private List<FakeNpc> allNpcs;

    public FakeNpcFeature(Harmony harmony, string id, Logger logger, TilePropertyHandler tileProperties, Properties propertyUtils, IModHelper helper)
    {
        this.Enabled = false;
        this.HarmonyPatcher = harmony;
        this.FeatureId = id;
        FakeNpcFeature.logger = logger;
        FakeNpcFeature.tileProperties = tileProperties;
        FakeNpcFeature.propertyUtils = propertyUtils;
        FakeNpcFeature.helper = helper;
        this.AffectsCursorIcon = false;
        this.allNpcs = new List<FakeNpc>();
    }

    public override void Enable()
    {
        try
        {
            this.HarmonyPatcher.Patch(
                AccessTools.Method(typeof(NPC), nameof(NPC.tryToReceiveActiveObject)),
                postfix: new HarmonyMethod(typeof(FakeNpcFeature),
                    nameof(FakeNpcFeature.Npc_TryToReceiveActiveObject_Postfix)));
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
        FeatureManager.OnLocationChangeCallback += this.ProcessNewLocation;
        FeatureManager.EarlyDayEndingCallback += this.OnEarlyDayEnding;
    }

    private void OnEarlyDayEnding(object? sender, EventArgs e)
    {
        foreach (FakeNpc npc in this.allNpcs)
        {
            npc.KillNpc();
        }
    }

    public override bool ShouldChangeCursor(GameLocation location, int tileX, int tileY, out int cursorId)
    {
        cursorId = default;

        return false;
    }

    public static void Npc_TryToReceiveActiveObject_Postfix(NPC __instance, Farmer who, bool probe)
    {
        if (__instance is not FakeNpc)
            return;

        HUDMessage message = new HUDMessage("This NPC can't receive gifts.", whatType: 3);

        if (!Game1.doesHUDMessageExist(message.message))
            Game1.addHUDMessage(message);
    }

    public void ProcessNewLocation(object _, OnLocationChangeEventArgs args)
    {
        GameLocation newLocation = args.NewLocation;

        int mapWidth = newLocation.Map.GetLayer("Back").Tiles.Array.GetLength(0);
        int mapHeight = newLocation.Map.GetLayer("Back").Tiles.Array.GetLength(1);

        if (mapWidth == 0 || mapHeight == 0)
            return;

#if DEBUG
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
#endif

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Tile tile;

                try
                {
                    tile = newLocation.Map.GetLayer("Back").Tiles.Array[x, y];
                }
                catch (Exception e)
                {
                    FakeNpcFeature.logger.Error(
                        $"Couldn't get tile {x}, {y} from map {newLocation.Name}. Exception follows.");
                    FakeNpcFeature.logger.Exception(e);

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
                            property.ToString(),
                            new AnimatedSprite($"Characters\\{fakeNpcProperty.NpcName}",
                                0,
                                fakeNpcProperty.HasSpriteSizes ? fakeNpcProperty.SpriteWidth : 16,
                                fakeNpcProperty.HasSpriteSizes ? fakeNpcProperty.SpriteHeight : 32),
                            new Vector2(x, y) * 64f,
                            2,
                            fakeNpcProperty.NpcName,
                            FakeNpcFeature.logger,
                            newLocation
                        );
                        if (fakeNpcProperty.HasSpriteSizes)
                        {
                            if (fakeNpcProperty.SpriteWidth > 16)
                                character.HideShadow = true;

                            character.Breather = false;
                        }

                        Dictionary<string, string> dialogue =
                            FakeNpcFeature.helper.GameContent.Load<Dictionary<string, string>>(
                                $"MEEP/FakeNPC/Dialogue/{fakeNpcProperty.NpcName}");

                        foreach (KeyValuePair<string, string> d in dialogue)
                        {
                            character.CurrentDialogue.Push(new Dialogue(character, $"{d.Key}:{d.Value}", d.Value));
                        }

                        // A safeguard for multiplayer.
                        if (newLocation.isTilePlaceable(new Vector2(x, y)))
                        {
                            foreach (Character npc in newLocation.characters)
                            {
                                if (npc is FakeNpc fake)
                                {
                                    // This means the FakeNPC was serialised over the network. Everything we care about
                                    // remains functional, however, so this is fine for now.
                                    if (fake.InternalId is null)
                                        return;

                                    // In this case, we've already added this NPC.
                                    if (fake.InternalId.Equals(character.InternalId))
                                        return;
                                }
                            }
                            newLocation.characters.Add(character);
                            this.allNpcs.Add(character);
                            FakeNpcFeature.logger.Log(
                                $"Fake NPC {character.Name} spawned in {newLocation.Name} at X:{x}, Y:{y}.",
                                LogLevel.Trace);
                        }
                    }
                    else
                    {
                        FakeNpcFeature.logger.Error($"Failed to parse property {property.ToString()}");
                    }
                }
            }
        }

#if DEBUG
        stopwatch.Stop();
        FakeNpcFeature.logger.Log($"Took {stopwatch.ElapsedMilliseconds}ms to process tiles for fake NPCs in {newLocation.NameOrUniqueName}.", LogLevel.Info);
#endif
    }
}
