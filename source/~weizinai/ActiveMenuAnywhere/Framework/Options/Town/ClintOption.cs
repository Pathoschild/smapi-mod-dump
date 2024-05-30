/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

namespace ActiveMenuAnywhere.Framework.Options;

internal class ClintOption : BaseOption
{
    public ClintOption(Rectangle sourceRect) :
        base(I18n.Option_Clint(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        var options = new List<Response>
        {
            // 商店
            new("Shop", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Shop"))
        };

        // 工具升级
        options.Add(Game1.player.toolBeingUpgraded.Value == null
            ? new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Upgrade"))
            : new Response("Receive", I18n.ClintOption_Receive()));

        // 砸开晶球
        var hasGeode = Game1.player.Items.Any(item1 => Utility.IsGeode(item1));
        if (hasGeode)
            options.Add(new Response("Process", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Geodes")));

        // 离开
        options.Add(new Response("Leave", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Leave")));

        Game1.currentLocation.createQuestionDialogue("", options.ToArray(), AfterDialogueBehavior);
    }

    private void AfterDialogueBehavior(Farmer who, string whichAnswer)
    {
        switch (whichAnswer)
        {
            case "Shop":
                Utility.TryOpenShopMenu("Blacksmith", "Clint");
                break;
            case "Upgrade":
                Utility.TryOpenShopMenu("ClintUpgrade", "Clint");
                break;
            case "Receive":
                if (Game1.player.toolBeingUpgraded.Value != null &&
                    Game1.player.daysLeftForToolUpgrade.Value <= 0)
                {
                    if (Game1.player.freeSpotsInInventory() > 0 || Game1.player.toolBeingUpgraded.Value is GenericTool)
                    {
                        var tool = Game1.player.toolBeingUpgraded.Value;
                        Game1.player.toolBeingUpgraded.Value = null;
                        Game1.player.hasReceivedToolUpgradeMessageYet = false;
                        Game1.player.holdUpItemThenMessage(tool);
                        if (tool is GenericTool)
                            tool.actionWhenClaimed();
                        else
                            Game1.player.addItemToInventoryBool(tool);
                    }
                }
                else
                {
                    Game1.drawObjectDialogue(I18n.ClintOption_Unfinished());
                }

                break;
            case "Process":
                Game1.activeClickableMenu = new GeodeMenu();
                break;
            case "Leave":
                Game1.exitActiveMenu();
                Game1.player.forceCanMove();
                break;
        }
    }
}