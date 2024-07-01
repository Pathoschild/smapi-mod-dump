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
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

internal class AutoClickHandler : BaseHandler
{
    private int cooldown;

    public AutoClickHandler(IModHelper helper, ModConfig config) : base(helper, config)
    {
    }

    public override void Init()
    {
        this.Helper.Events.GameLoop.OneSecondUpdateTicked += this.OnOneSecondUpdateTicked;
        this.Helper.Events.GameLoop.Saving += this.OnSaving;
        this.Helper.Events.Display.MenuChanged += this.OnMenuChanged;
    }

    private void OnOneSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        if (Game1.activeClickableMenu is LevelUpMenu levelUpMenu)
        {
            this.cooldown++;
            if (this.cooldown > 2)
            {
                if (levelUpMenu.isProfessionChooser)
                {
                    var professionToChoose = this.Helper.Reflection.GetField<List<int>>(levelUpMenu, "professionsToChoose").GetValue()[0];
                    Game1.player.professions.Add(professionToChoose);
                    levelUpMenu.getImmediateProfessionPerk(professionToChoose);
                    levelUpMenu.isActive = false;
                    levelUpMenu.informationUp = false;
                    levelUpMenu.isProfessionChooser = false;
                    levelUpMenu.RemoveLevelFromLevelList();
                    Log.Info("你长时间没有选择职业，已自动为你选择左侧职业。");
                }
                else
                {
                    levelUpMenu.okButtonClicked();
                    Log.Info("你长时间没有确认，已自动点击确认按钮。");
                }
            }
        }
    }

    private void OnSaving(object? sender, SavingEventArgs e)
    {
        if (Game1.activeClickableMenu is ShippingMenu menu)
        {
            this.Helper.Reflection.GetMethod(menu, "okClicked").Invoke();
            Log.Info("已自动为你确认出货结算。");
        }
    }

    private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
    {
        if (e.NewMenu is LevelUpMenu) this.cooldown = 0;
    }
}