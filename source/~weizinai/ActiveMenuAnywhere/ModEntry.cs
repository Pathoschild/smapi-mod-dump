/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using ActiveMenuAnywhere.Framework;
using Common.Integrations;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ActiveMenuAnywhere;

internal class ModEntry : Mod
{
    private ModConfig config = null!;
    public static readonly Dictionary<MenuTabID, Texture2D> Textures = new();

    public override void Entry(IModHelper helper)
    {
        // 初始化
        config = helper.ReadConfig<ModConfig>();
        LoadTexture();
        I18n.Init(helper.Translation);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.Input.ButtonsChanged += OnButtonChanged;
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (config.MenuKey.JustPressed())
        {
            if (Game1.activeClickableMenu is AMAMenu)
                Game1.exitActiveMenu();
            else if (Context.IsPlayerFree)
                Game1.activeClickableMenu = new AMAMenu(config.DefaultMeanTabID, Helper);
        }
    }

    private void LoadTexture()
    {
        Textures.Add(MenuTabID.Farm, Helper.ModContent.Load<Texture2D>("Assets/Farm.png"));
        Textures.Add(MenuTabID.Town, Helper.ModContent.Load<Texture2D>("Assets/Town.png"));
        Textures.Add(MenuTabID.Mountain, Helper.ModContent.Load<Texture2D>("Assets/Mountain.png"));
        Textures.Add(MenuTabID.Forest, Helper.ModContent.Load<Texture2D>("Assets/Forest.png"));
        Textures.Add(MenuTabID.Beach, Helper.ModContent.Load<Texture2D>("Assets/Beach.png"));
        Textures.Add(MenuTabID.Desert, Helper.ModContent.Load<Texture2D>("Assets/Desert"));
        Textures.Add(MenuTabID.GingerIsland, Helper.ModContent.Load<Texture2D>("Assets/GingerIsland.png"));
        Textures.Add(MenuTabID.RSV, Helper.ModContent.Load<Texture2D>("Assets/RSV.png"));
        Textures.Add(MenuTabID.SVE, Helper.ModContent.Load<Texture2D>("Assets/SVE.png"));
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

        if (configMenu is null) return;

        configMenu.Register(
            ModManifest,
            () => config = new ModConfig(),
            () => Helper.WriteConfig(config)
        );

        configMenu.AddKeybindList(
            ModManifest,
            () => config.MenuKey,
            value => config.MenuKey = value,
            I18n.Config_MenuKeyName
        );

        configMenu.AddTextOption(
            ModManifest,
            () => config.DefaultMeanTabID.ToString(),
            value =>
            {
                if (!Enum.TryParse(value, out MenuTabID tabID)) throw new InvalidOperationException($"Couldn't parse tab name '{value}'.");
                config.DefaultMeanTabID = tabID;
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