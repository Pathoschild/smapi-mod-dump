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

internal class ResetCabinHandler : BaseHandler
{
    public ResetCabinHandler(ModConfig config, IModHelper helper) : base(config, helper)
    {
    }

    public override void Init()
    {
        this.Helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    public override void Clear()
    {
        this.Helper.Events.Input.ButtonsChanged -= this.OnButtonChanged;
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!Context.IsMainPlayer || !Context.IsPlayerFree) return;

        if (this.Config.ResetCabinPlayerKeybind.JustPressed())
        {
            var location = Game1.player.currentLocation;
            if (location is Cabin cabin)
            {
                if (Game1.player.team.playerIsOnline(cabin.owner.UniqueMultiplayerID))
                {
                    Game1.addHUDMessage(new HUDMessage(I18n.UI_ResetCabin_Online()){ noIcon = true});
                    return;
                }
                
                if (!cabin.owner.isUnclaimedFarmhand)
                    this.ResetCabin(cabin);
                else
                    Game1.addHUDMessage(new HUDMessage(I18n.UI_ResetCabin_NoOwner()) { noIcon = true });
            }
            else
            {
                var farmhands = Game1.getAllFarmhands()
                    .Where(farmer => !farmer.isUnclaimedFarmhand)
                    .Select(farmer => new KeyValuePair<string, string>(farmer.UniqueMultiplayerID.ToString(), farmer.displayName));
                location.ShowPagedResponses(I18n.UI_ResetCabin_ChooseFarmhand(), farmhands.ToList(), value =>
                {
                    var id = long.Parse(value);
                    if (Game1.player.team.playerIsOnline(id)) Game1.server.kick(id);
                    var farmer = Game1.getFarmer(id);
                    this.ResetCabin((Utility.getHomeOfFarmer(farmer) as Cabin)!);
                });
            }
        }
    }

    private void ResetCabin(Cabin cabin)
    {
        Game1.addHUDMessage(new HUDMessage(I18n.UI_ResetCabin_Success(cabin.owner.displayName)) { noIcon = true });
        cabin.DeleteFarmhand();
        cabin.CreateFarmhand();
    }
}