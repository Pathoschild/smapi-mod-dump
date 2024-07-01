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
using weizinai.StardewValleyMod.BetterCabin.Framework.Menu;

namespace weizinai.StardewValleyMod.BetterCabin.Handler;

internal class CabinMenuHandler : BaseHandler
{
    public CabinMenuHandler(ModConfig config, IModHelper helper) : base(config, helper)
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
        if (!Context.IsPlayerFree) return;

        if (this.Config.CabinMenuKeybind.JustPressed())
        {
            if (Game1.IsServer)
            {
                Game1.activeClickableMenu = new ServerCabinMenu();
            }
            else
            {
                Utility.ForEachBuilding(building =>
                {
                    if (building.GetIndoors() is Cabin cabin && cabin.owner.Equals(Game1.player))
                    {
                        Game1.activeClickableMenu = new ClientCabinMenu(building);
                        return false;
                    }

                    return true;
                });
            }
        }
    }
}