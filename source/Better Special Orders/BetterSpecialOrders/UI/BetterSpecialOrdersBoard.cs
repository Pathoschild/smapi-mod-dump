/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/xeru98/StardewMods
**
*************************************************/

using BetterSpecialOrders.Messages;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace BetterSpecialOrders.UI;

public class BetterSpecialOrdersBoard : SpecialOrdersBoard
{

    public ClickableComponent rerollButton;
    
    public BetterSpecialOrdersBoard(string orderType)
    : base(orderType)
    {
        
        //WILL UNCOMMENT AFTER REBUILDING NETWORKING
        //Vector2 stringSize = Game1.dialogueFont.MeasureString(RerollManager.Get().boardConfigs[GetOrderType()].infiniteRerolls.Get() ? "Rerolls" : "Reroll: 10");
        Vector2 stringSize = Game1.dialogueFont.MeasureString("Rerolls");
        Vector2 position = Utility.getTopLeftPositionForCenteringOnScreen((int)stringSize.X + 24, (int)stringSize.Y + 24);
        rerollButton = new ClickableComponent(new Rectangle((int)position.X, (int)base.yPositionOnScreen + base.height - 128, (int)stringSize.X + 24 , (int)stringSize.Y + 24), "")
            {
                myID = 2,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                upNeighborID = -99998,
                downNeighborID = -99998,
                visible = RerollManager.Get().boardConfigs[orderType].canReroll.Get()
            };
        
        // Run this just in case we created a new SpecialOrdersBoard and need to update defaults
        leftOrder.SetHardOrderDuration();
        rightOrder.SetHardOrderDuration();
        
        UpdateButtons();
        
    }

    public override void draw(SpriteBatch b)
    {
        leftOrder = Game1.player.team.GetAvailableSpecialOrder(type: GetOrderType());
        rightOrder = Game1.player.team.GetAvailableSpecialOrder(1, GetOrderType());
        
        base.draw(b);

        if (rerollButton.visible)
        {
            //WILL UNCOMMENT AFTER REBUILDING NETWORKING
            //string text =  RerollManager.Get().boardConfigs[GetOrderType()].infiniteRerolls.Get() ? "Reroll" : $"Rerolls: { RerollManager.Get().rerollsRemaining[GetOrderType()].ToString()}";
            string text = "Reroll";
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 373, 9, 9), rerollButton.bounds.X, rerollButton.bounds.Y, rerollButton.bounds.Width, rerollButton.bounds.Height, (double) rerollButton.scale > 1.0 ? GetHoverColor() : Color.White, 4f * rerollButton.scale);
            Utility.drawTextWithShadow(b, text, Game1.dialogueFont, new Vector2(rerollButton.bounds.X + 12, rerollButton.bounds.Y + 12), Game1.textColor);
        }
        
        drawMouse(b);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        base.receiveLeftClick(x, y, playSound);
        if (rerollButton.visible && rerollButton.containsPoint(x, y))
        {
            if (RerollManager.Get().CanReroll(GetOrderType()))
            {
                Game1.playSound("Ship");
                RerollManager.Get().Reroll(GetOrderType());
            }
            else
            {
                Game1.playSound("detector");
            }
            
            
        }   
    }

    public override void performHoverAction(int x, int y)
    {
        base.performHoverAction(x, y);
        float prevScale = rerollButton.scale;
        rerollButton.scale = rerollButton.bounds.Contains(x, y) ? 1.5f : 1f;
        if ((double) rerollButton.scale > (double) prevScale)
            Game1.playSound("Cowboy_gunshot");
    }

    public override void UpdateButtons()
    {
        base.UpdateButtons();
        if (rerollButton != null)
        {
            rerollButton.visible = RerollManager.Get().boardConfigs[GetOrderType()].canReroll.Value;
        }
    }

    private Color GetHoverColor()
    {
        return RerollManager.Get().CanReroll(GetOrderType()) ? Color.LightGreen : Color.Pink;
    }
}