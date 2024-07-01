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
using StardewValley.Locations;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class RobinOption : BaseOption
{
    public RobinOption(Rectangle sourceRect) :
        base(I18n.Option_Robin(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.daysUntilHouseUpgrade.Value < 0 && !Game1.IsThereABuildingUnderConstruction())
        {
            var options = new List<Response>
                { new("Shop", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Shop")) };
            if (Game1.IsMasterGame)
            {
                // 房屋升级_主玩家
                if (Game1.player.HouseUpgradeLevel < 3)
                {
                    options.Add(new Response("Upgrade",
                        Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeHouse")));
                }
                // 社区升级
                else if (this.CommunityUpgrade())
                {
                    // 拖车
                    if (!Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
                        options.Add(new Response("CommunityUpgrade",
                            Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_CommunityUpgrade")));
                    // 捷径
                    else if (!Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts"))
                        options.Add(new Response("CommunityUpgrade",
                            Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_CommunityUpgrade")));
                }
            }
            else if (Game1.player.HouseUpgradeLevel < 3)
            {
                // 房屋升级
                options.Add(new Response("Upgrade",
                    Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeCabin")));
            }

            // 装修房屋
            if (Game1.player.HouseUpgradeLevel >= 2)
            {
                if (Game1.IsMasterGame)
                    options.Add(new Response("Renovate",
                        Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_RenovateHouse")));
                else
                    options.Add(new Response("Renovate",
                        Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_RenovateCabin")));
            }

            // 农场建筑
            options.Add(new Response("Construct", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Construct")));
            // 离开
            options.Add(new Response("Leave", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Leave")));

            Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu"),
                options.ToArray(), "carpenter");
        }
        else
        {
            Utility.TryOpenShopMenu("Carpenter", "Robin");
        }
    }

    // 判断是否可以进行社区升级
    private bool CommunityUpgrade()
    {
        var isCommunityCenterCompleted = Game1.MasterPlayer.mailReceived.Contains("ccIsComplete") ||
                                         Game1.MasterPlayer.hasCompletedCommunityCenter();
        var isJojaMember = Game1.MasterPlayer.mailReceived.Contains("JojaMember");
        var isCommunityUpgradeCompleted = Game1.RequireLocation<Town>("Town").daysUntilCommunityUpgrade.Value <= 0;
        return (isCommunityCenterCompleted || isJojaMember) && isCommunityUpgradeCompleted;
    }
}