/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.TooManyAnimals;

using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewMods.Common.Helpers;
using StardewMods.Common.Integrations.GenericModConfigMenu;
using StardewMods.CommonHarmony.Helpers;
using StardewValley;
using StardewValley.Menus;
using SObject = StardewValley.Object;

/// <inheritdoc />
public class TooManyAnimals : Mod
{
    private readonly PerScreen<int> _currentPage = new();
    private readonly PerScreen<ClickableTextureComponent?> _nextPage = new();
    private readonly PerScreen<ClickableTextureComponent?> _previousPage = new();
    private ModConfig? _config;

    private static TooManyAnimals? Instance { get; set; }

    private ModConfig Config
    {
        get
        {
            if (this._config is not null)
            {
                return this._config;
            }

            ModConfig? config = null;
            try
            {
                config = this.Helper.ReadConfig<ModConfig>();
            }
            catch (Exception)
            {
                // ignored
            }

            this._config = config ?? new ModConfig();
            Log.Trace(this._config.ToString());
            return this._config;
        }
    }

    private int CurrentPage
    {
        get => this._currentPage.Value;
        set
        {
            if (this._currentPage.Value == value)
            {
                return;
            }

            this._currentPage.Value = value;
            Game1.activeClickableMenu = new PurchaseAnimalsMenu(this.Stock);
        }
    }

    private ClickableTextureComponent NextPage
    {
        get => this._nextPage.Value ??= new(
            new(0, 0, 12 * Game1.pixelZoom, 11 * Game1.pixelZoom),
            Game1.mouseCursors,
            new(365, 495, 12, 11),
            Game1.pixelZoom)
        {
            myID = 69420,
        };
    }

    private ClickableTextureComponent PreviousPage
    {
        get => this._previousPage.Value ??= new(
            new(0, 0, 12 * Game1.pixelZoom, 11 * Game1.pixelZoom),
            Game1.mouseCursors,
            new(352, 495, 12, 11),
            Game1.pixelZoom)
        {
            myID = 69421,
        };
    }

    private List<SObject>? Stock { get; set; }

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        TooManyAnimals.Instance = this;
        Log.Monitor = this.Monitor;
        I18n.Init(this.Helper.Translation);

        if (this.Helper.ModRegistry.IsLoaded("furyx639.FuryCore"))
        {
            Log.Alert("Remove FuryCore, it is no longer needed by this mod!");
        }

        // Patches
        HarmonyHelper.AddPatch(
            this.ModManifest.UniqueID,
            AccessTools.Constructor(typeof(PurchaseAnimalsMenu), new[] { typeof(List<SObject>) }),
            typeof(TooManyAnimals),
            nameof(TooManyAnimals.PurchaseAnimalsMenu_constructor_prefix));
        HarmonyHelper.ApplyPatches(this.ModManifest.UniqueID);

        // Events
        this.Helper.Events.Display.MenuChanged += this.OnMenuChanged;
        this.Helper.Events.Display.RenderedActiveMenu += this.OnRenderedActiveMenu;
        this.Helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        this.Helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
        this.Helper.Events.Input.ButtonPressed += this.OnButtonPressed;
    }

    private static void PurchaseAnimalsMenu_constructor_prefix(ref List<SObject> stock)
    {
        // Get actual stock
        TooManyAnimals.Instance!.Stock ??= stock;

        // Limit stock
        stock = TooManyAnimals.Instance.Stock
                              .Skip(TooManyAnimals.Instance.CurrentPage * TooManyAnimals.Instance.Config.AnimalShopLimit)
                              .Take(TooManyAnimals.Instance.Config.AnimalShopLimit)
                              .ToList();
    }

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (Game1.activeClickableMenu is not PurchaseAnimalsMenu || this.Stock is null || this.Stock.Count <= this.Config.AnimalShopLimit)
        {
            return;
        }

        if (e.Button is not SButton.MouseLeft or SButton.MouseRight && !(e.Button.IsActionButton() || e.Button.IsUseToolButton()))
        {
            return;
        }

        var (x, y) = Game1.getMousePosition(true);
        if (this.NextPage.containsPoint(x, y) && (this.CurrentPage + 1) * this.Config.AnimalShopLimit < this.Stock.Count)
        {
            this.CurrentPage++;
        }

        if (this.PreviousPage.containsPoint(x, y) && this.CurrentPage > 0)
        {
            this.CurrentPage--;
        }
    }

    private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (Game1.activeClickableMenu is not PurchaseAnimalsMenu || this.Stock is null || this.Stock.Count <= this.Config.AnimalShopLimit)
        {
            return;
        }

        if (this.Config.ControlScheme.NextPage.JustPressed() && (this.CurrentPage + 1) * this.Config.AnimalShopLimit < this.Stock.Count)
        {
            this.CurrentPage++;
            return;
        }

        if (this.Config.ControlScheme.PreviousPage.JustPressed() && this.CurrentPage > 0)
        {
            this.CurrentPage--;
        }
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        var gmcm = new GenericModConfigMenuIntegration(this.Helper.ModRegistry);
        if (!gmcm.IsLoaded)
        {
            return;
        }

        // Register mod configuration
        gmcm.Register(
            this.ModManifest,
            () => this._config = new(),
            () => this.Helper.WriteConfig(this.Config));

        gmcm.API!.AddSectionTitle(this.ModManifest, I18n.Section_General_Name, I18n.Section_General_Description);

        // Animal Shop Limit
        gmcm.API.AddNumberOption(
            this.ModManifest,
            () => this.Config.AnimalShopLimit,
            value => this.Config.AnimalShopLimit = value,
            I18n.Config_AnimalShopLimit_Name,
            I18n.Config_AnimalShopLimit_Tooltip,
            fieldId: nameof(ModConfig.AnimalShopLimit));

        gmcm.API.AddSectionTitle(this.ModManifest, I18n.Section_Controls_Name, I18n.Section_Controls_Description);

        // Next Page
        gmcm.API.AddKeybindList(
            this.ModManifest,
            () => this.Config.ControlScheme.NextPage,
            value => this.Config.ControlScheme.NextPage = value,
            I18n.Config_NextPage_Name,
            I18n.Config_NextPage_Tooltip,
            nameof(Controls.NextPage));

        // Previous Page
        gmcm.API.AddKeybindList(
            this.ModManifest,
            () => this.Config.ControlScheme.PreviousPage,
            value => this.Config.ControlScheme.PreviousPage = value,
            I18n.Config_PreviousPage_Name,
            I18n.Config_PreviousPage_Tooltip,
            nameof(Controls.PreviousPage));
    }

    private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
    {
        // Reset Stock/CurrentPage
        if (e.NewMenu is not PurchaseAnimalsMenu menu)
        {
            this.Stock = null;
            this._currentPage.Value = 0;
            return;
        }

        // Reposition Next/Previous Page Buttons
        this.NextPage.bounds.X = menu.xPositionOnScreen + menu.width - this.NextPage.bounds.Width;
        this.NextPage.bounds.Y = menu.yPositionOnScreen + menu.height;
        this.NextPage.leftNeighborID = this.PreviousPage.myID;
        this.PreviousPage.bounds.X = menu.xPositionOnScreen;
        this.PreviousPage.bounds.Y = menu.yPositionOnScreen + menu.height;
        this.PreviousPage.rightNeighborID = this.NextPage.myID;

        for (var index = 0; index < menu.animalsToPurchase.Count; index++)
        {
            var i = index + this.CurrentPage * this.Config.AnimalShopLimit;
            if (ReferenceEquals(menu.animalsToPurchase[index].texture, Game1.mouseCursors))
            {
                menu.animalsToPurchase[index].sourceRect.X = i % 3 * 16 * 2;
                menu.animalsToPurchase[index].sourceRect.Y = 448 + i / 3 * 16;
            }

            if (ReferenceEquals(menu.animalsToPurchase[index].texture, Game1.mouseCursors2))
            {
                menu.animalsToPurchase[index].sourceRect.X = 128 + i % 3 * 16 * 2;
                menu.animalsToPurchase[index].sourceRect.Y = i / 3 * 16;
            }
        }

        // Assign neighborId for controller
        var maxY = menu.animalsToPurchase.Max(component => component.bounds.Y);
        var bottomComponents = menu.animalsToPurchase.Where(component => component.bounds.Y == maxY).ToList();
        this.PreviousPage.upNeighborID = bottomComponents.OrderBy(component => Math.Abs(component.bounds.Center.X - this.PreviousPage.bounds.X)).First().myID;
        this.NextPage.upNeighborID = bottomComponents.OrderBy(component => Math.Abs(component.bounds.Center.X - this.NextPage.bounds.X)).First().myID;
        foreach (var component in bottomComponents)
        {
            component.downNeighborID = component.bounds.Center.X <= menu.xPositionOnScreen + menu.width / 2 ? this.PreviousPage.myID : this.NextPage.myID;
        }
    }

    private void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e)
    {
        if (Game1.activeClickableMenu is not PurchaseAnimalsMenu || this.Stock is null || this.Stock.Count <= this.Config.AnimalShopLimit)
        {
            return;
        }

        if ((this.CurrentPage + 1) * this.Config.AnimalShopLimit < this.Stock.Count)
        {
            this.NextPage.draw(e.SpriteBatch);
        }

        if (this.CurrentPage > 0)
        {
            this.PreviousPage.draw(e.SpriteBatch);
        }
    }
}