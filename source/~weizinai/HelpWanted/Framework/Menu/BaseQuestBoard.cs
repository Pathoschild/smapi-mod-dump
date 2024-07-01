/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using weizinai.StardewValleyMod.Common.UI;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Quests;

namespace weizinai.StardewValleyMod.HelpWanted.Framework.Menu;

internal abstract class BaseQuestBoard : IClickableMenu
{
    protected readonly ClickableComponent AcceptQuestButton;
    protected string HoverTitle = "";
    protected string HoverText = "";


    protected int ShowingQuestID;
    protected Quest? ShowingQuest;
    protected readonly ModConfig Config;
    
    protected Rectangle BoardRect = new(78 * 4, 52 * 4, 184 * 4, 102 * 4);
    protected const int OptionIndex = -4200;

    protected BaseQuestBoard(ModConfig config) : base(0,0,0,0,true)
    {
        // 位置和大小逻辑
        this.width = 338 * 4;
        this.height = 198 * 4;
        var center = Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height);
        this.xPositionOnScreen = (int)center.X;
        this.yPositionOnScreen = (int)center.Y;
        
        // 接受任务按钮逻辑
        var stringSize = Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:AcceptQuest"));
        this.AcceptQuestButton = new ClickableComponent(new Rectangle(this.xPositionOnScreen + this.width / 2 - 128, this.yPositionOnScreen + this.height - 128,
            (int)stringSize.X + 24, (int)stringSize.Y + 24), "");
        
        // 关闭按钮逻辑
        this.upperRightCloseButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 20, this.yPositionOnScreen, 48, 48),
            Game1.mouseCursors, CommonImage.CloseButton, 4f);
        
        // 初始化
        this.Config = config;
    }
}