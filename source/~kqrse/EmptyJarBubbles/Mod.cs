/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Machines;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace EmptyJarBubbles;

internal class Mod: StardewModdingAPI.Mod {
    internal static Configuration Config;
    internal static int CurrentEmoteFrame;
    internal static int CurrentEmoteInterval;
    internal static List<Object> Machines;
    internal static Dictionary<string, MachineData> MachineData;
    internal static List<string> ModdedMachineQualifiedIds;

    public override void Entry(IModHelper helper) {
        Config = Helper.ReadConfig<Configuration>();
        I18n.Init(helper.Translation);

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += SaveLoaded;
        helper.Events.GameLoop.DayStarted += DayStarted;
        helper.Events.GameLoop.ReturnedToTitle += ReturnedToTitle;
        helper.Events.GameLoop.UpdateTicked += UpdateTicked;
        helper.Events.World.ObjectListChanged += ObjectListChanged;
        helper.Events.Player.Warped += Warped;
        helper.Events.Display.MenuChanged += MenuChanged;
    }

    private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is not null) RegisterConfig(configMenu);
    }
    
    private void UpdateTicked(object sender, UpdateTickedEventArgs e) {
        if (!Config.Enabled) return;
        AnimateEmote();
    }

    private static void AnimateEmote() {
        CurrentEmoteInterval += Game1.currentGameTime.ElapsedGameTime.Milliseconds;

        if (CurrentEmoteFrame is < 16 or > 19) CurrentEmoteFrame = 16;
        if (CurrentEmoteInterval > Config.EmoteInterval) {
            if (CurrentEmoteFrame < 19) CurrentEmoteFrame++;
            else CurrentEmoteFrame = 16;
            CurrentEmoteInterval = 0;
        }
    }

    private void SaveLoaded(object sender, SaveLoadedEventArgs e)
    {
        Helper.Events.Display.RenderedWorld += RenderBubbles;
        MachineData = DataLoader.Machines(Game1.content);
        ModdedMachineQualifiedIds = GetModdedMachinesFromMachineData();
        ApplyZoomLevel99();
    }

    private static List<string> GetModdedMachinesFromMachineData() {
        return MachineData
            .Where(kvp =>
                kvp.Value.OutputRules is not null &&
                kvp.Value.OutputRules
                    .Any(outputRule => outputRule.Triggers
                        .Any(triggerRule => triggerRule.Trigger == MachineOutputTrigger.ItemPlacedInMachine)))
            .Select(kvp => kvp.Key)
            .Where(x => !VanillaMachineQualifiedIds.AsList().Contains(x))
            .ToList();
    }

    private void ReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
    {
        Helper.Events.Display.RenderedWorld -= RenderBubbles;
    }
    
    private void Warped(object sender, WarpedEventArgs e) {
        BuildMachineList(); 
    }
    
    private void DayStarted(object sender, DayStartedEventArgs e) {
        BuildMachineList();
    }

    private void MenuChanged(object sender, MenuChangedEventArgs e) {
        BuildMachineList();
        ApplyZoomLevel99();
    }

    private void ApplyZoomLevel99() {
        if (!Config.Enabled) return;
        if (!Config.ZoomLevel99Enabled) return;

        Game1.options.desiredBaseZoomLevel = 0.99f;
    }

    private void ObjectListChanged(object sender, ObjectListChangedEventArgs e) {
        if (!Config.Enabled) return;

        var removedMachines = e.Removed
            .Where(kvp => IsValidMachine(kvp.Value))
            .Select(kvp => kvp.Value);

        var newMachines = e.Added
            .Where(kvp => IsValidMachine(kvp.Value))
            .Select(kvp => kvp.Value);
        
        Machines.RemoveAll(x => removedMachines.Contains(x));
        Machines.AddRange(newMachines);
    }
    
    private void BuildMachineList()
    {
        if (!Config.Enabled) return;
        if (Game1.currentLocation is null) return;
        if (ModdedMachineQualifiedIds is null) return;

        Machines = Game1.currentLocation.Objects.Values
            .Where(IsValidMachine)
            .ToList();
    }
    
    private bool IsValidMachine(Object o) {
        return IsObjectValidMachine(o, Config.JarsEnabled, VanillaMachineQualifiedIds.Jar) ||
               IsObjectValidMachine(o, Config.KegsEnabled, VanillaMachineQualifiedIds.Keg) ||
               IsObjectValidMachine(o, Config.CasksEnabled, VanillaMachineQualifiedIds.Cask) ||
               IsObjectValidMachine(o, Config.MayonnaiseMachinesEnabled, VanillaMachineQualifiedIds.MayonnaiseMachine) ||
               IsObjectValidMachine(o, Config.CheesePressesEnabled, VanillaMachineQualifiedIds.CheesePress) ||
               IsObjectValidMachine(o, Config.LoomsEnabled, VanillaMachineQualifiedIds.Loom) ||
               IsObjectValidMachine(o, Config.OilMakersEnabled, VanillaMachineQualifiedIds.OilMaker) ||
               IsObjectValidMachine(o, Config.DehydratorsEnabled, VanillaMachineQualifiedIds.Dehydrator) ||
               IsObjectValidMachine(o, Config.FishSmokersEnabled, VanillaMachineQualifiedIds.FishSmoker) ||
               IsObjectValidMachine(o, Config.BaitMakersEnabled, VanillaMachineQualifiedIds.BaitMaker) ||
               IsObjectValidMachine(o, Config.BoneMillsEnabled, VanillaMachineQualifiedIds.BoneMill) ||
               IsObjectValidMachine(o, Config.CharcoalKilnsEnabled, VanillaMachineQualifiedIds.CharcoalKiln) ||
               IsObjectValidMachine(o, Config.CrystalariumsEnabled, VanillaMachineQualifiedIds.Crystalarium) ||
               IsObjectValidMachine(o, Config.FurnacesEnabled, VanillaMachineQualifiedIds.Furnace) ||
               IsObjectValidMachine(o, Config.FurnacesEnabled, VanillaMachineQualifiedIds.HeavyFurnace) ||
               IsObjectValidMachine(o, Config.RecyclingMachinesEnabled, VanillaMachineQualifiedIds.RecyclingMachine) ||
               IsObjectValidMachine(o, Config.SeedMakersEnabled, VanillaMachineQualifiedIds.SeedMaker) ||
               IsObjectValidMachine(o, Config.SlimeEggPressesEnabled, VanillaMachineQualifiedIds.SlimeEggPress) ||
               IsObjectValidMachine(o, Config.CrabPotsEnabled, VanillaMachineQualifiedIds.CrabPot) ||
               IsObjectValidMachine(o, Config.DeconstructorsEnabled, VanillaMachineQualifiedIds.Deconstructor) ||
               IsObjectValidMachine(o, Config.GeodeCrushersEnabled, VanillaMachineQualifiedIds.GeodeCrusher) ||
               IsObjectValidMachine(o, Config.WoodChippersEnabled, VanillaMachineQualifiedIds.WoodChipper) ||
               (Config.ModdedMachinesEnabled && ModdedMachineQualifiedIds.Contains(o.QualifiedItemId));
    }
    
    private bool IsObjectValidMachine(Object o, bool enabled, string qualifiedId) {
        if (!enabled) return false;
        return o.QualifiedItemId == qualifiedId;
    }

    private void RenderBubbles(object sender, RenderedWorldEventArgs e) {
        if (!Config.Enabled) return;

        foreach (var machine in Machines.Where(IsMachineRenderReady))
            DrawBubbles(machine, e.SpriteBatch);
    }

    private bool IsMachineRenderReady(Object o) {
        return (o is not CrabPot && o.MinutesUntilReady <= 0 && !o.readyForHarvest.Value) ||
               (o is CrabPot pot && pot.bait.Value is null && pot.heldObject.Value is null);
    }
    
    private void DrawBubbles(Object o, SpriteBatch spriteBatch) {
        Vector2 tilePosition = o.TileLocation * 64;
        Vector2 emotePosition = Game1.GlobalToLocal(tilePosition);
        emotePosition += new Vector2((100 - Config.SizePercent) / 100f * 32 +Config.OffsetX, -Config.OffsetY);
        if (o is CrabPot pot) {
            emotePosition += pot.directionOffset.Value;
            emotePosition.Y += pot.yBob + 20;
        }
        
        spriteBatch.Draw(Game1.emoteSpriteSheet,
            emotePosition, 
            new Rectangle(CurrentEmoteFrame * 16 % Game1.emoteSpriteSheet.Width, CurrentEmoteFrame * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16),
            Color.White * (Config.OpacityPercent / 100f), 
            0f,
            Vector2.Zero, 
            4f * Config.SizePercent / 100f, 
            SpriteEffects.None, 
            (tilePosition.Y + 37) / 10000f);
    }

    private void RegisterConfig(IGenericModConfigMenuApi configMenu) {
        configMenu.Register(
            mod: ModManifest,
            reset: () => Config = new Configuration(),
            save: () => Helper.WriteConfig(Config)
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.Enabled,
            getValue: () => Config.Enabled,
            setValue: value => Config.Enabled = value
        );
        
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: I18n.BubbleYOffset,
            getValue: () => Config.OffsetY,
            setValue: value => Config.OffsetY = value,
            min: 0,
            max: 128
        );
        
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: I18n.BubbleXOffset,
            getValue: () => Config.OffsetX,
            setValue: value => Config.OffsetX = value,
            min: -128,
            max: 128
        );
        
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: I18n.EmoteInterval,
            getValue: () => Config.EmoteInterval,
            setValue: value => Config.EmoteInterval = value,
            min: 0,
            max: 1000
        );
        
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: I18n.Opacity,
            getValue: () => Config.OpacityPercent,
            setValue: value => Config.OpacityPercent = value,
            min: 1,
            max: 100
        );
        
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: I18n.BubbleSize,
            getValue: () => Config.SizePercent,
            setValue: value => Config.SizePercent = value,
            min: 1,
            max: 100
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.ZoomLevel99Enabled,
            tooltip: I18n.ZoomLevel99EnabledTooltip,
            getValue: () => Config.ZoomLevel99Enabled,
            setValue: value => Config.ZoomLevel99Enabled = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.ModdedMachinesEnabled,
            tooltip: I18n.ModdedMachinesEnabledTooltip,
            getValue: () => Config.ModdedMachinesEnabled,
            setValue: value => Config.ModdedMachinesEnabled = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.JarsEnabled, 
            getValue: () => Config.JarsEnabled,
            setValue: value => Config.JarsEnabled = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.KegsEnabled, 
            getValue: () => Config.KegsEnabled,
            setValue: value => Config.KegsEnabled = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.CasksEnabled, 
            getValue: () => Config.CasksEnabled,
            setValue: value => Config.CasksEnabled = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.MayonnaiseMachinesEnabled, 
            getValue: () => Config.MayonnaiseMachinesEnabled,
            setValue: value => Config.MayonnaiseMachinesEnabled = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.CheesePressesEnabled, 
            getValue: () => Config.CheesePressesEnabled,
            setValue: value => Config.CheesePressesEnabled = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.LoomsEnabled, 
            getValue: () => Config.LoomsEnabled,
            setValue: value => Config.LoomsEnabled = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.OilMakersEnabled, 
            getValue: () => Config.OilMakersEnabled,
            setValue: value => Config.OilMakersEnabled = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.DehydratorsEnabled, 
            getValue: () => Config.DehydratorsEnabled,
            setValue: value => Config.DehydratorsEnabled = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.FishSmokersEnabled, 
            getValue: () => Config.FishSmokersEnabled,
            setValue: value => Config.FishSmokersEnabled = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.BaitMakersEnabled, 
            getValue: () => Config.BaitMakersEnabled,
            setValue: value => Config.BaitMakersEnabled = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.BoneMillsEnabled, 
            getValue: () => Config.BoneMillsEnabled,
            setValue: value => Config.BoneMillsEnabled = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.CharcoalKilnsEnabled, 
            getValue: () => Config.CharcoalKilnsEnabled,
            setValue: value => Config.CharcoalKilnsEnabled = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.CrystalariumsEnabled, 
            getValue: () => Config.CrystalariumsEnabled,
            setValue: value => Config.CrystalariumsEnabled = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.FurnacesEnabled, 
            getValue: () => Config.FurnacesEnabled,
            setValue: value => Config.FurnacesEnabled = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.RecyclingMachinesEnabled, 
            getValue: () => Config.RecyclingMachinesEnabled,
            setValue: value => Config.RecyclingMachinesEnabled = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.SeedMakersEnabled, 
            getValue: () => Config.SeedMakersEnabled,
            setValue: value => Config.SeedMakersEnabled = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.SlimeEggPressesEnabled, 
            getValue: () => Config.SlimeEggPressesEnabled,
            setValue: value => Config.SlimeEggPressesEnabled = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.CrabPotsEnabled, 
            getValue: () => Config.CrabPotsEnabled,
            setValue: value => Config.CrabPotsEnabled = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.DeconstructorsEnabled, 
            getValue: () => Config.DeconstructorsEnabled,
            setValue: value => Config.DeconstructorsEnabled = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.GeodeCrushersEnabled, 
            getValue: () => Config.GeodeCrushersEnabled,
            setValue: value => Config.GeodeCrushersEnabled = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.WoodChippersEnabled, 
            getValue: () => Config.WoodChippersEnabled,
            setValue: value => Config.WoodChippersEnabled = value
        );
    }
}