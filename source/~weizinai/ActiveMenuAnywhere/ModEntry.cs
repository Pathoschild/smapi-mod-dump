/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;
using weizinai.StardewValleyMod.Common.Integration;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere;

internal class ModEntry : Mod
{
    private ModConfig config = null!;
    public static readonly Dictionary<MenuTabID, Texture2D> Textures = new();

    public override void Entry(IModHelper helper)
    {
        // 初始化
        this.config = helper.ReadConfig<ModConfig>();
        this.LoadTexture();
        I18n.Init(helper.Translation);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (this.config.MenuKey.JustPressed())
        {
            if (Game1.activeClickableMenu is AMAMenu)
                Game1.exitActiveMenu();
            else if (Context.IsPlayerFree)
                Game1.activeClickableMenu = new AMAMenu(this.config.DefaultMeanTabId, this.Helper);
        }
    }

    private void LoadTexture()
    {
        Textures.Add(MenuTabID.Farm, this.Helper.ModContent.Load<Texture2D>("Assets/Farm.png"));
        Textures.Add(MenuTabID.Town, this.Helper.ModContent.Load<Texture2D>("Assets/Town.png"));
        Textures.Add(MenuTabID.Mountain, this.Helper.ModContent.Load<Texture2D>("Assets/Mountain.png"));
        Textures.Add(MenuTabID.Forest, this.Helper.ModContent.Load<Texture2D>("Assets/Forest.png"));
        Textures.Add(MenuTabID.Beach, this.Helper.ModContent.Load<Texture2D>("Assets/Beach.png"));
        Textures.Add(MenuTabID.Desert, this.Helper.ModContent.Load<Texture2D>("Assets/Desert"));
        Textures.Add(MenuTabID.GingerIsland, this.Helper.ModContent.Load<Texture2D>("Assets/GingerIsland.png"));
        Textures.Add(MenuTabID.RSV, this.Helper.ModContent.Load<Texture2D>("Assets/RSV.png"));
        Textures.Add(MenuTabID.SVE, this.Helper.ModContent.Load<Texture2D>("Assets/SVE.png"));
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

        if (configMenu is null) return;

        configMenu.Register(this.ModManifest,
            () => this.config = new ModConfig(),
            () => this.Helper.WriteConfig(this.config)
        );

        configMenu.AddKeybindList(this.ModManifest,
            () => this.config.MenuKey,
            value => this.config.MenuKey = value,
            I18n.Config_MenuKeyName
        );

        configMenu.AddTextOption(this.ModManifest,
            () => this.config.DefaultMeanTabId.ToString(),
            value =>
            {
                if (!Enum.TryParse(value, out MenuTabID tabID)) throw new InvalidOperationException($"Couldn't parse tab name '{value}'.");
                this.config.DefaultMeanTabId = tabID;
            },
            I18n.Config_DefaultMenuTabID,
            null,
            new[] { "Farm", "Town", "Mountain", "Forest", "Beach", "Desert", "GingerIsland", "RSV", "SVE" },
            value =>
            {
                var formatValue = value switch
                {
                    "Farm" => I18n.Tab_Farm(),
                    "Town" => I18n.Tab_Town(),
                    "Mountain" => I18n.Tab_Mountain(),
                    "Forest" => I18n.Tab_Forest(),
                    "Beach" => I18n.Tab_Beach(),
                    "Desert" => I18n.Tab_Desert(),
                    "GingerIsland" => I18n.Tab_GingerIsland(),
                    "RSV" => I18n.Tab_RSV(),
                    "SVE" => I18n.Tab_SVE(),
                    _ => ""
                };
                return formatValue;
            }
        );
    }
}