/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.Common.Patcher;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.HelpWanted.Framework;
using weizinai.StardewValleyMod.HelpWanted.Patcher;

namespace weizinai.StardewValleyMod.HelpWanted;

internal class ModEntry : Mod
{
    private ModConfig config = null!;
    private QuestManager questManager = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        this.config = helper.ReadConfig<ModConfig>();
        this.questManager = new QuestManager(this.config, helper);
        Log.Init(this.Monitor);
        I18n.Init(helper.Translation);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        // 注册Harmony补丁
        var patches = new List<IPatcher>
        {
            new BillboardPatcher(this.config), new ItemDeliveryQuestPatcher(this.config), new SlayMonsterQuestPatcher(this.config), new ResourceCollectionQuestPatcher(this.config),
            new FishingQuestPatcher(this.config), new Game1Patcher(), new TownPatcher()
        };
        if (helper.ModRegistry.IsLoaded("Rafseazz.RidgesideVillage")) patches.Add(new RSVQuestBoardPatcher(this.config));
        HarmonyPatcher.Apply(this, patches.ToArray());
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        this.questManager.InitVanillaQuestList();
        if (this.Helper.ModRegistry.IsLoaded("Rafseazz.RidgesideVillage") && this.config.EnableRSVQuestBoard) this.questManager.InitRSVQuestList();
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        new GenericModConfigMenuIntegrationForHelpWanted(this.Helper, this.ModManifest,
            () => this.config,
            () => this.config = new ModConfig(),
            () => this.Helper.WriteConfig(this.config)
        ).Register();
    }
}