using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace MailFrameworkMod
{
    public class LetterViewerMenuExtended : LetterViewerMenu
    {
        public int? TextColor { get; set; }

        private int _moneyIncluded;
        public string LearnedRecipe { get; set; }
        public string CookingOrCrafting { get; set; }

        public LetterViewerMenuExtended(string text) : base(text)
        {
            initiPrivateProperties();
        }

        public LetterViewerMenuExtended(int secretNoteIndex) : base(secretNoteIndex)
        {
            initiPrivateProperties();
        }

        public LetterViewerMenuExtended(string mail, string mailTitle) : base(mail, mailTitle)
        {
            initiPrivateProperties();
        }

        private void initiPrivateProperties()
        {
            _moneyIncluded = MailFrameworkModEntry.ModHelper.Reflection.GetField<int>(this, "moneyIncluded").GetValue();
            LearnedRecipe = MailFrameworkModEntry.ModHelper.Reflection.GetField<string>(this, "learnedRecipe").GetValue();
            CookingOrCrafting = MailFrameworkModEntry.ModHelper.Reflection.GetField<string>(this, "cookingOrCrafting").GetValue();
        }

        public static bool GetTextColor(LetterViewerMenu __instance, ref int __result)
        {
            if (__instance is LetterViewerMenuExtended letterViewerMenuExtended && letterViewerMenuExtended.TextColor.HasValue)
            {
                __result = (int)letterViewerMenuExtended.TextColor;
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
            
            if ((double)MailFrameworkModEntry.ModHelper.Reflection.GetField<float>(this, "scale").GetValue() == 1.0 && this._moneyIncluded <= 0)
            {
                if (!string.IsNullOrEmpty(this.LearnedRecipe))
                {
                    int textColor = MailFrameworkModEntry.ModHelper.Reflection.GetMethod(this, "getTextColor").Invoke<int>();
                    string s = Game1.content.LoadString("Strings\\UI:LetterViewer_LearnedRecipe", (object)this.CookingOrCrafting);
                    SpriteText.drawStringHorizontallyCenteredAt(b, s, this.xPositionOnScreen + this.width / 2, this.yPositionOnScreen + this.height - 32 - SpriteText.getHeightOfString(s, 999999) * 2, 999999, this.width - 64, 9999, 0.65f, 0.865f, false, textColor);
                    SpriteText.drawStringHorizontallyCenteredAt(b, Game1.content.LoadString("Strings\\UI:LetterViewer_LearnedRecipeName", (object)this.LearnedRecipe), this.xPositionOnScreen + this.width / 2, this.yPositionOnScreen + this.height - 32 - SpriteText.getHeightOfString("t", 999999), 999999, this.width - 64, 9999, 0.9f, 0.865f, false, textColor);
                    if (Game1.options.hardwareCursor)
                        return;
                    b.Draw(Game1.mouseCursors, new Vector2((float)Game1.getMouseX(), (float)Game1.getMouseY()), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, 16, 16)), Color.White, 0.0f, Vector2.Zero, (float)(4.0 + (double)Game1.dialogueButtonScale / 150.0), SpriteEffects.None, 1f);
                }
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if ((double)MailFrameworkModEntry.ModHelper.Reflection.GetField<float>(this, "scale").GetValue() < 1.0)
                return;

            if (this.upperRightCloseButton == null || !this.readyToClose() ||
                !this.upperRightCloseButton.containsPoint(x, y))
            {
                if (Game1.activeClickableMenu != null || Game1.currentMinigame != null)
                {
                    if (this.itemsToGrab.Count > 0)
                    {
                        ClickableComponent clickableComponent = this.itemsToGrab.Last();
                        if (clickableComponent.containsPoint(x, y) && clickableComponent.item != null)
                        {
                            Game1.playSound("coin");
                            Game1.player.addItemByMenuIfNecessary(clickableComponent.item, null);
                            if (Game1.activeClickableMenu != null && Game1.activeClickableMenu != this)
                            {
                                Game1.activeClickableMenu.exitFunction = new IClickableMenu.onExit(() => Game1.activeClickableMenu = this);
                            }
                            clickableComponent.item = (Item)null;
                            if (this.itemsToGrab.Count > 1)
                            {
                                this.itemsToGrab.Remove(clickableComponent);
                            }
                            return;
                        }
                    }
                }
            }

            base.receiveLeftClick(x, y, playSound);
        }
    }
}
