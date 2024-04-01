/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/siweipancc/StardewMods
**
*************************************************/

using GenericModConfigMenu;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Tools;

namespace AutoMeleeAttack;

public class ModEntry : Mod


{
    // key: name, value : i18n
    private static readonly SortedDictionary<string, string> LocalizedMonsterNames = new();

    private readonly HashSet<string> _ignores = new();

    private ModConfig? _config;

    private bool _enable = true;
    private Monster? _lastSkipMonster;

    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);
        _config = helper.ReadConfig<ModConfig>();

        _config.SkipAlso ??= new SortedDictionary<string, bool>();

        if (!_config.SkipRockCrab)
        {
            Monitor.Log(
                "skipRockCrab is disable, this may lead to some weird actions, but if might work in such conditions.",
                LogLevel.Info);
        }

        helper.Events.Content.LocaleChanged += OnLocaleChange;
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.Input.ButtonReleased += ToggleOnButtonReleased;
        helper.Events.GameLoop.UpdateTicking += OnUpdateTicking;
    }

    private void ToggleOnButtonReleased(object? sender, ButtonReleasedEventArgs buttonReleasedEventArgs)
    {
        if (!Context.IsWorldReady)
        {
            return;
        }

        if (buttonReleasedEventArgs.Button != _config!.Toggle)
        {
            return;
        }

        // reverse
        _enable = !_enable;
        if (_enable)
        {
            Game1.addHUDMessage(new HUDMessage(I18n.Message_Toggle_Enable(), HUDMessage.newQuest_type));
            return;
        }

        Game1.addHUDMessage(new HUDMessage(I18n.Message_Toggle_Disabled(), HUDMessage.newQuest_type));
    }

    private void InitIgnores()
    {
        _ignores.Clear();
        foreach (var (monster, value) in _config!.SkipAlso!)
        {
            if (!value)
            {
                continue;
            }

            _ignores.Add(monster);
        }

        if (_ignores.Count <= 0)
        {
            return;
        }

        string join = string.Join(", ", _ignores);
        Monitor.Log($"ignore these monsters: {join}", LogLevel.Info);
    }


    private void OnUpdateTicking(object? sender, UpdateTickingEventArgs e)
    {
        if (!Context.IsWorldReady || !_enable)
        {
            return;
        }

        if (!e.IsMultipleOf(10))
        {
            return;
        }

        var player = Game1.player;

        // check for pre-act
        var playerCurrentTool = player.CurrentTool;
        if (!player.isActive() || playerCurrentTool is not MeleeWeapon || player.IsBusyDoingSomething())
        {
            return;
        }

        GameLocation currentLocation = player.currentLocation;
        IEnumerable<Monster?> enumerator = Utility
            .GetNpcsWithinDistance(player.Tile, _config!.DetectTiles, currentLocation)
            .OfType<Monster>();
        Monster? monster = enumerator.FirstOrDefault();
        if (monster == null)
        {
            return;
        }

        if (_ignores.Count > 0 && _ignores.Contains(monster.Name))
        {
            if (_lastSkipMonster != monster)
            {
                Game1.addHUDMessage(new HUDMessage(
                    $"{I18n.Message_Target_Missed()} : {LocalizedMonsterNames[monster.Name]}",
                    HUDMessage.newQuest_type));
            }

            _lastSkipMonster = monster;
            return;
        }

        if (monster is RockCrab rockCrab)
        {
            if (_config!.SkipRockCrab)
            {
                return;
            }

            if (Helper.Reflection.GetField<NetBool>(rockCrab, "isStickBug").GetValue().Value)
            {
                Monitor.Log("skipped attach RockCrab[inStickBug] at this frame");
                return;
            }
        }

        int towards = player.getGeneralDirectionTowards(monster.Position);
        if (player.FacingDirection != towards)
        {
            player.faceDirection(towards);
        }
        playerCurrentTool.beginUsing(currentLocation, (int)player.GetToolLocation(true).X,
            (int)player.GetToolLocation(true).Y, player);
    }

    private IGenericModConfigMenuApi? GetConfigApi()
    {
        return Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
    }

    private void RegisterGenericModConfigMenu()
    {
        IGenericModConfigMenuApi? configMenu = GetConfigApi();
        if (configMenu == null)
        {
            return;
        }

        configMenu.Unregister(ModManifest);
        // register mod
        configMenu.Register(
            mod: ModManifest,
            reset: () => _config = new ModConfig(),
            save: () =>
            {
                Helper.WriteConfig(_config!);
                InitIgnores();
            }
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.Config_SkipRockCrab_Label,
            tooltip: I18n.Config_SkipRockCrab_Tooltip,
            getValue: () => _config!.SkipRockCrab,
            setValue: value => { _config!.SkipRockCrab = value; }
        );

        configMenu.AddNumberOption(
            mod: ModManifest,
            name: I18n.Config_DetectTiles_Label,
            tooltip: I18n.Config_DetectTiles_Tooltip,
            getValue: () => _config!.DetectTiles,
            setValue: value => { _config!.DetectTiles = value; },
            min: 1,
            max: 4,
            interval: 1
        );


        // dynamic monsters load from DataLoader
        // i18n missed!
        configMenu.AddKeybind(
            mod: ModManifest,
            name: I18n.Config_Toggle_Label,
            tooltip: I18n.Config_Toggle_Tooltip,
            getValue: () => _config!.Toggle,
            setValue: value => { _config!.Toggle = value; }
        );
        configMenu.AddSectionTitle(mod: ModManifest, I18n.Config_Section_Also_Title);


        foreach (var (monsterName, _) in _config!.SkipAlso!)
        {
            configMenu.AddBoolOption(mod: ModManifest,
                name: () => LocalizedMonsterNames[monsterName],
                getValue: () => _config.SkipAlso[monsterName],
                setValue: value => { _config.SkipAlso[monsterName] = value; }
            );
        }
    }


    private void ForceReloadLocalizedMonsterNames()
    {
        Dictionary<string, string> dictionary = Helper.GameContent.Load<Dictionary<string, string>>("Data\\Monsters");
        foreach (var (key, value) in dictionary)
        {
            string localizedText = value.Split('/')[14];
            LocalizedMonsterNames[key] = localizedText;
        }
    }


    private void OnGameLaunched(object? sender, GameLaunchedEventArgs args)
    {
        ForceReloadLocalizedMonsterNames();
        // try add
        foreach (var name in LocalizedMonsterNames.Keys)
        {
            _config!.SkipAlso!.TryAdd(name, false);
        }

        InitIgnores();

        if (LocalizedMonsterNames.Count != _config!.SkipAlso!.Count)
        {
            Monitor.Log("monsters count not consists, delete invalid(possible) opts");
            foreach (var key in _config!.SkipAlso.Keys.Where(key => !LocalizedMonsterNames.ContainsKey(key)))
            {
                _config!.SkipAlso.Remove(key);
            }
        }

        RegisterGenericModConfigMenu();
    }

    private void OnLocaleChange(object? sender, LocaleChangedEventArgs args)
    {
        if (args.OldLanguage != args.NewLanguage)
        {
            ForceReloadLocalizedMonsterNames();
        }
    }
}