/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lisyce/SDV_Allergies_Mod
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace BZP_Allergies.HarmonyPatches.UI
{
    internal class PatchedSkillsPage : SkillsPage
    {
        private readonly ClickableTextureComponent AllergyTab;
        private bool OnAllergyTab = false;
        private AllergyOptionsMenu AllergyPage;
        private Traverse HoverTextTraverse;

        public PatchedSkillsPage(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            Texture2D sprites = Game1.content.Load<Texture2D>("BarleyZP.BzpAllergies/Sprites");
            
            AllergyTab = new(
                "BarleyZP.BzpAllergies",
                new Rectangle(xPositionOnScreen - 48, yPositionOnScreen + 64 * 2, 64, 64),
                "",
                ModEntry.Instance.Translation.Get("allergy-menu.title"),
                sprites,
                new Rectangle(64, 0, 16, 16),
                4f
                );

            AllergyPage = new(x, y, width, height);

            HoverTextTraverse = Traverse.Create(this).Field("hoverText");
        }

        
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (AllergyTab.containsPoint(x, y))
            {
                Game1.playSound("smallSelect");
                if (!OnAllergyTab)
                {
                    AllergyTab.bounds.X += CollectionsPage.widthToMoveActiveTab;
                    AllergyPage.currentItemIndex = 0;
                }
                else
                {
                    AllergyTab.bounds.X -= CollectionsPage.widthToMoveActiveTab;
                }
                OnAllergyTab = !OnAllergyTab;

                // re-populate the options to re-sort the checkboxes
                bool currentlyRandom = false;
                if (AllergenManager.ModDataGet(Game1.player, "BarleyZP.BzpAllergies_Random", out string val))
                {
                    currentlyRandom = val == "true";
                }

                AllergyPage.PopulateOptions(currentlyRandom);
            }
            else if (OnAllergyTab)
            {
                AllergyPage.receiveLeftClick(x, y, playSound);
            }
            else
            {
                base.receiveLeftClick(x, y, playSound);
            }
        }

        public override void performHoverAction(int x, int y)
        {
            HoverTextTraverse.SetValue("");
            if (AllergyTab.containsPoint(x, y))
            {
                HoverTextTraverse.SetValue(AllergyTab.hoverText);
            }
            else if (OnAllergyTab)
            {
                AllergyPage.performHoverAction(x, y);
                HoverTextTraverse.SetValue(AllergyPage.HoverTextTraverse.GetValue());
            }
            else
            {
                base.performHoverAction(x, y);
            }
        }

        public override void draw(SpriteBatch b)
        {
            AllergyTab.draw(b);
            if (OnAllergyTab)
            {
                AllergyPage.draw(b);
                if (HoverTextTraverse.GetValue<string>().Length > 0)
                {
                    IClickableMenu.drawHoverText(b, HoverTextTraverse.GetValue<string>(), Game1.smallFont, 0, 0, -1, null);
                }
            }
            else
            {
                base.draw(b);
            }
        }

        public override void snapToDefaultClickableComponent()
        {
            if (OnAllergyTab)
            {
                AllergyPage.snapToDefaultClickableComponent();
            }
            else
            {
                base.snapToDefaultClickableComponent();
            }
        }

        public override void applyMovementKey(int direction)
        {
            if (OnAllergyTab)
            {
                AllergyPage.applyMovementKey(direction);
            }
            else
            {
                base.applyMovementKey(direction);
            }
        }

        protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
        {
            if (OnAllergyTab)
            {
                Traverse.Create(AllergyPage).Method("customSnapBehavior").GetValue();
            }
            else
            {
                base.customSnapBehavior(direction, oldRegion, oldID);
            }
        }

        public override void snapCursorToCurrentSnappedComponent()
        {
            if (OnAllergyTab)
            {
                AllergyPage.snapCursorToCurrentSnappedComponent();
            }
            else
            {
                base.snapCursorToCurrentSnappedComponent();
            }
        }

        public override void leftClickHeld(int x, int y)
        {
            if (OnAllergyTab)
            {
                AllergyPage.leftClickHeld(x, y);
            }
            else
            {
                base.leftClickHeld(x, y);
            }
        }

        public override ClickableComponent getCurrentlySnappedComponent()
        {
            if (OnAllergyTab)
            {
                return AllergyPage.getCurrentlySnappedComponent();
            }
            else
            {
                return base.getCurrentlySnappedComponent();
            }
        }

        public override void setCurrentlySnappedComponentTo(int id)
        {
            if (OnAllergyTab)
            {
                AllergyPage.setCurrentlySnappedComponentTo(id);
            }
            else
            {
                base.setCurrentlySnappedComponentTo(id);
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            if (OnAllergyTab)
            {
                AllergyPage.receiveKeyPress(key);
            }
            else
            {
                base.receiveKeyPress(key);
            }
        }

        public override void receiveScrollWheelAction(int direction)
        {
            if (OnAllergyTab)
            {
                AllergyPage.receiveScrollWheelAction(direction);
            }
            else
            {
                base.receiveScrollWheelAction(direction);
            }
        }


        public override void releaseLeftClick(int x, int y)
        {
            if (OnAllergyTab)
            {
                AllergyPage.releaseLeftClick(x, y);
            }
            else
            {
                base.releaseLeftClick(x, y);
            }
        }

    }
}
