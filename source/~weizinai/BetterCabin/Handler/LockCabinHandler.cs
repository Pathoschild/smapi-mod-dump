/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.BetterCabin.Framework;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;

namespace weizinai.StardewValleyMod.BetterCabin.Handler;

internal class LockCabinHandler : BaseHandler
{
    private static string LockCabinKey => "weizinai.BetterCabin_LockCabin";
    
    public LockCabinHandler(ModConfig config, IModHelper helper) : base(config, helper)
    {
    }

    public override void Init()
    {
        this.Helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        this.Helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    public override void Clear()
    {
        this.Helper.Events.GameLoop.SaveLoaded -= this.OnSaveLoaded;
        this.Helper.Events.Input.ButtonsChanged -= this.OnButtonChanged;
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        InitLockCabinConfig(this.Config);
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!Game1.IsClient || !Context.IsPlayerFree) return;

        if (this.Config.LockCabinKeybind.JustPressed())
        {
            if (!CheckLockCabinEnable())
            {
                Game1.addHUDMessage(new HUDMessage(I18n.UI_LockCabin_Disable(), 3));
                return;
            }
            
            var cabin = Utility.getHomeOfFarmer(Game1.player) as Cabin;
            if (CheckCabinLock(cabin!))
            {
                cabin!.modData[LockCabinKey] = "false";
                Game1.addHUDMessage(new HUDMessage(I18n.UI_LockCabin_Unlock()) { noIcon = true });
            }
            else
            {
                cabin!.modData[LockCabinKey] = "true";
                Game1.addHUDMessage(new HUDMessage(I18n.UI_LockCabin_Lock()) { noIcon = true });
            }
        }
    }

    public static void InitLockCabinConfig(ModConfig config)
    {
        if (!Game1.IsServer) return;

        if (config.LockCabin)
        {
            if (Game1.MasterPlayer.modData.ContainsKey(LockCabinKey))
                Game1.MasterPlayer.modData[LockCabinKey] = "true";
            else
                Game1.MasterPlayer.modData.Add(LockCabinKey, "true");
        }
        else
        {
            if (Game1.MasterPlayer.modData.ContainsKey(LockCabinKey))
                Game1.MasterPlayer.modData[LockCabinKey] = "false";
            else
                Game1.MasterPlayer.modData.Add(LockCabinKey, "false");
        }
    }

    private static bool CheckLockCabinEnable()
    {
        if (!Game1.MasterPlayer.modData.ContainsKey(LockCabinKey)) return false;

        return Game1.MasterPlayer.modData[LockCabinKey] == "true";
    }

    public static bool CheckCabinLock(Cabin cabin)
    {
        if (!CheckLockCabinEnable()) return false;

        if (!cabin.modData.ContainsKey(LockCabinKey)) return false;

        return cabin.modData[LockCabinKey] == "true";
    }
}