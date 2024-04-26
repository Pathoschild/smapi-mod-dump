/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.GarbageDay.Framework.Services;

using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewMods.Common.Helpers;
using StardewMods.Common.Interfaces;
using StardewMods.Common.Services;
using StardewMods.Common.Services.Integrations.FauxCore;
using StardewMods.Common.Services.Integrations.ToolbarIcons;
using StardewMods.GarbageDay.Framework.Interfaces;
using StardewMods.GarbageDay.Framework.Models;
using StardewValley.Characters;
using StardewValley.Menus;
using StardewValley.Objects;

/// <summary>Represents a manager for managing garbage cans in a game.</summary>
internal sealed class GarbageCanManager : BaseService<GarbageCanManager>
{
    private readonly AssetHandler assetHandler;
    private readonly PerScreen<NPC?> currentNpc = new();
    private readonly Dictionary<string, GameLocation?> foundLocations = [];
    private readonly PerScreen<GarbageCan?> garbageCanOpened = new();
    private readonly Dictionary<string, GarbageCan> garbageCans = [];
    private readonly IInputHelper inputHelper;
    private readonly IModConfig modConfig;
    private readonly IReflectedField<Multiplayer> multiplayer;
    private readonly ToolbarIconsIntegration toolbarIconsIntegration;

    /// <summary>Initializes a new instance of the <see cref="GarbageCanManager" /> class.</summary>
    /// <param name="assetHandler">Dependency used for handling assets.</param>
    /// <param name="eventSubscriber">Dependency used for subscribing to events.</param>
    /// <param name="inputHelper">Dependency used for checking and changing input state.</param>
    /// <param name="log">Dependency used for logging debug information to the console.</param>
    /// <param name="manifest">Dependency for accessing mod manifest.</param>
    /// <param name="modConfig">Dependency used for accessing config data.</param>
    /// <param name="reflectionHelper">Dependency used for reflecting into external code.</param>
    /// <param name="toolbarIconsIntegration">Dependency for Toolbar Icons integration.</param>
    public GarbageCanManager(
        AssetHandler assetHandler,
        IEventSubscriber eventSubscriber,
        IInputHelper inputHelper,
        ILog log,
        IManifest manifest,
        IModConfig modConfig,
        IReflectionHelper reflectionHelper,
        ToolbarIconsIntegration toolbarIconsIntegration)
        : base(log, manifest)
    {
        // Init
        this.assetHandler = assetHandler;
        this.inputHelper = inputHelper;
        this.modConfig = modConfig;
        this.toolbarIconsIntegration = toolbarIconsIntegration;
        this.multiplayer = reflectionHelper.GetField<Multiplayer>(typeof(Game1), "multiplayer");

        // Events
        eventSubscriber.Subscribe<GameLaunchedEventArgs>(this.OnGameLaunched);
        eventSubscriber.Subscribe<MenuChangedEventArgs>(this.OnMenuChanged);
        eventSubscriber.Subscribe<ButtonPressedEventArgs>(this.OnButtonPressed);
        eventSubscriber.Subscribe<DayEndingEventArgs>(this.OnDayEnding);
        eventSubscriber.Subscribe<DayStartedEventArgs>(this.OnDayStarted);
    }

    private void OnGameLaunched(GameLaunchedEventArgs e)
    {
        // Integrations
        if (!this.toolbarIconsIntegration.IsLoaded)
        {
            return;
        }

        this.toolbarIconsIntegration.Api.AddToolbarIcon(
            this.Id,
            this.assetHandler.IconTexturePath,
            new Rectangle(0, 0, 16, 16),
            I18n.Button_GarbageFill_Name());

        this.toolbarIconsIntegration.Api.Subscribe(this.OnIconPressed);
    }

    private void OnButtonPressed(ButtonPressedEventArgs e)
    {
        if (!Context.IsPlayerFree
            || !e.Button.IsActionButton()
            || !Game1.currentLocation.Objects.TryGetValue(e.Cursor.GrabTile, out var obj)
            || obj is not Chest chest
            || !chest.modData.TryGetValue(this.ModId + "/WhichCan", out var whichCan)
            || !this.garbageCans.TryGetValue(whichCan, out var garbageCan))
        {
            return;
        }

        this.inputHelper.Suppress(e.Button);
        this.garbageCanOpened.Value = garbageCan;
        var character = Utility.isThereAFarmerOrCharacterWithinDistance(garbageCan.Tile, 7, garbageCan.Location);

        if (character is not NPC npc || character is Horse)
        {
            garbageCan.CheckAction();
            return;
        }

        // Queue up NPC response
        this.currentNpc.Value = npc;
        this.multiplayer.GetValue().globalChatInfoMessage("TrashCan", Game1.player.Name, npc.GetTokenizedDisplayName());
        if (npc.Name.Equals("Linus", StringComparison.OrdinalIgnoreCase))
        {
            npc.doEmote(32);
            npc.setNewDialogue("Data\\ExtraDialogue:Town_DumpsterDiveComment_Linus", true, true);

            Game1.player.changeFriendship(5, npc);
            this.multiplayer.GetValue().globalChatInfoMessage("LinusTrashCan");
        }
        else
        {
            switch (npc.Age)
            {
                case 2:
                    npc.doEmote(28);
                    npc.setNewDialogue("Data\\ExtraDialogue:Town_DumpsterDiveComment_Child", true, true);

                    break;
                case 1:
                    npc.doEmote(8);
                    npc.setNewDialogue("Data\\ExtraDialogue:Town_DumpsterDiveComment_Teen", true, true);

                    break;
                default:
                    npc.doEmote(12);
                    npc.setNewDialogue("Data\\ExtraDialogue:Town_DumpsterDiveComment_Adult", true, true);

                    break;
            }

            Game1.player.changeFriendship(-25, npc);
        }

        garbageCan.CheckAction();
    }

    private void OnDayEnding(DayEndingEventArgs e)
    {
        // Remove garbage cans
        foreach (var garbageCan in this.garbageCans.Values)
        {
            garbageCan.Location.Objects.Remove(garbageCan.Tile);
        }

        this.garbageCans.Clear();
    }

    private void OnDayStarted(DayStartedEventArgs e)
    {
        // Add garbage cans
        foreach (var (whichCan, foundGarbageCan) in this.assetHandler.FoundGarbageCans)
        {
            if (this.garbageCans.ContainsKey(whichCan))
            {
                continue;
            }

            if (!this.TryCreateGarbageCan(foundGarbageCan, out var garbageCan))
            {
                this.assetHandler.InvalidateGarbageCan(whichCan);
                continue;
            }

            this.garbageCans.Add(whichCan, garbageCan);
        }

        // Add loot
        foreach (var garbageCan in this.garbageCans.Values)
        {
            if (Game1.dayOfMonth % 7 == (int)this.modConfig.GarbageDay % 7)
            {
                garbageCan.EmptyTrash();
            }

            garbageCan.AddLoot(this.Log);
        }
    }

    private void OnIconPressed(IIconPressedEventArgs e)
    {
        if (e.Id != this.Id)
        {
            return;
        }

        foreach (var pos in Game1.player.Tile.Box(1))
        {
            if (Game1.currentLocation.Objects.TryGetValue(pos, out var obj)
                && obj is Chest chest
                && chest.modData.TryGetValue(this.ModId + "/WhichCan", out var whichCan)
                && this.garbageCans.TryGetValue(whichCan, out var garbageCan))
            {
                garbageCan.AddLoot(this.Log, ItemRegistry.Create("(H)66"));
            }
        }
    }

    private void OnMenuChanged(MenuChangedEventArgs e)
    {
        if (e.OldMenu is not ItemGrabMenu || this.garbageCanOpened.Value is null)
        {
            return;
        }

        // Close Can
        if (this.currentNpc.Value is not null)
        {
            Game1.drawDialogue(this.currentNpc.Value);
            this.currentNpc.Value = null;
        }

        this.garbageCanOpened.Value = null;
    }

    private bool TryCreateGarbageCan(FoundGarbageCan foundGarbageCan, [NotNullWhen(true)] out GarbageCan? garbageCan)
    {
        // Find location
        if (!this.foundLocations.TryGetValue(foundGarbageCan.AssetName.Name, out var location))
        {
            location = Game1.locations.FirstOrDefault(l => foundGarbageCan.AssetName.IsEquivalentTo(l.mapPath.Value));
            this.foundLocations[foundGarbageCan.AssetName.Name] = location;
        }

        if (location is null)
        {
            this.Log.Trace("Unable to find location for Garbage Can {0}", foundGarbageCan.WhichCan);
            garbageCan = null;
            return false;
        }

        // Remove existing garbage can
        if (location.Objects.TryGetValue(foundGarbageCan.TilePosition, out var obj)
            && obj.modData.ContainsKey(this.ModId + "/WhichCan"))
        {
            location.Objects.Remove(foundGarbageCan.TilePosition);
        }

        // Attempt to place item
        var item = this.assetHandler.GarbageCan;
        this.Log.Trace(
            "Placing Garbage Can {0} at {1} ({2})",
            foundGarbageCan.WhichCan,
            location.Name,
            foundGarbageCan.TilePosition);

        if (!item.placementAction(
                location,
                (int)foundGarbageCan.TilePosition.X * Game1.tileSize,
                (int)foundGarbageCan.TilePosition.Y * Game1.tileSize,
                Game1.player)
            || !location.Objects.TryGetValue(foundGarbageCan.TilePosition, out obj)
            || obj is not Chest chest)
        {
            this.Log.Trace("Unable to place Garbage Can");
            garbageCan = null;
            return false;
        }

        // Update chest
        chest.GlobalInventoryId = this.ModId + "-" + foundGarbageCan.WhichCan;
        chest.playerChoiceColor.Value = Color.DarkGray;
        chest.modData[this.ModId + "/WhichCan"] = foundGarbageCan.WhichCan;
        chest.modData["Pathoschild.ChestsAnywhere/IsIgnored"] = "true";
        chest.modData["furyx639.BetterChests/StorageName"] = $"{I18n.GarbageCan_Name()} {foundGarbageCan.WhichCan}";

        // Create garbage can
        garbageCan = new GarbageCan(chest);
        return true;
    }
}