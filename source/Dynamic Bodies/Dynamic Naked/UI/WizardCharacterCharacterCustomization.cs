/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ribeena/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;

using DynamicBodies.Data;

namespace DynamicBodies.UI
{
    internal class WizardCharacterCharacterCustomization : CharacterCustomization
    {
        public const int region_opendoctors = 701, region_openleahs = 702, region_openpams = 703;

        public ClickableTextureComponent doctorsButton;
        public ClickableTextureComponent leahsButton;
        public ClickableTextureComponent pamsButton;

        private Texture2D UItexture;
        public WizardCharacterCharacterCustomization() : base(CharacterCustomization.Source.Wizard)
        {
            
            UItexture = Game1.content.Load<Texture2D>("Mods/ribeena.dynamicbodies/assets/Interface/ui.png");
            //will setup positions
            setUpPositions();
        }

        private void setUpPositions()
        {

            //Move some displays
            if (this.petPortraitBox.HasValue)
            {
                Pet pet = Game1.getCharacterFromName<Pet>(Game1.player.getPetName(), mustBeVillager: false);

                Rectangle petPortraitBoxRect = this.petPortraitBox.Value;
                petPortraitBoxRect.Y -= 80;
                this.petPortraitBox = petPortraitBoxRect;
                int leftID = leftSelectionButtons.FindIndex(x => x.myID == 511);
                leftSelectionButtons[leftID].bounds.Y -= 80;
                int rightID = rightSelectionButtons.FindIndex(x => x.myID == 510);
                rightSelectionButtons[rightID].bounds.Y -= 80;
                int labelID = labels.FindIndex(x => x.name == pet.Name);
                labels[labelID].bounds.Y -= 80;
            }

            //Add new points
            Point basePoint = new Point(base.xPositionOnScreen + base.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 128, base.yPositionOnScreen + base.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + 16 - 76);

            doctorsButton = new ClickableTextureComponent("Surgery", new Rectangle(basePoint.X, basePoint.Y, 128, 64), null, null, UItexture, new Rectangle(128, 32, 32, 16), 4f)
            {
                myID = region_opendoctors,
                upNeighborID = -99998,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                downNeighborID = CharacterCustomization.region_okbutton
            };
            leahsButton = new ClickableTextureComponent("Colorist", new Rectangle(basePoint.X - (32 + 2) * 4, basePoint.Y, 128, 64), null, null, UItexture, new Rectangle(128, 48, 32, 16), 4f)
            {
                myID = region_openleahs,
                upNeighborID = -99998,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                downNeighborID = -99998
            };
            pamsButton = new ClickableTextureComponent("Sink", new Rectangle(basePoint.X - (32 + 2) * 4*2, basePoint.Y, 128, 64), null, null, UItexture, new Rectangle(128, 64, 32, 16), 4f)
            {
                myID = region_openleahs,
                upNeighborID = -99998,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                downNeighborID = -99998
            };

            if (Game1.options.snappyMenus && Game1.options.gamepadControls)
            {
                base.populateClickableComponentList();
                base.snapToDefaultClickableComponent();
            }
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            setUpPositions();
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (doctorsButton.containsPoint(x, y))
            {
                Game1.activeClickableMenu = new DoctorModifier(true);
                doctorsButton.scale -= 0.25f;
                doctorsButton.scale = Math.Max(0.75f*doctorsButton.baseScale, doctorsButton.scale);
                Game1.playSound("shwip");
            }
            if (leahsButton.containsPoint(x, y))
            {
                Game1.activeClickableMenu = new FullColourModifier(true);
                leahsButton.scale -= 0.25f;
                leahsButton.scale = Math.Max(0.75f * doctorsButton.baseScale, leahsButton.scale);
                Game1.playSound("shwip");
            }
            if (pamsButton.containsPoint(x, y))
            {
                Game1.activeClickableMenu = new SimpleColourModifier(true);
                pamsButton.scale -= 0.25f;
                pamsButton.scale = Math.Max(0.75f * doctorsButton.baseScale, pamsButton.scale);
                Game1.playSound("shwip");
            }

            //Fix rendering
            PlayerBaseExtended pbe = PlayerBaseExtended.Get(Game1.player);
            foreach (ClickableComponent lbutt in base.leftSelectionButtons)
            {
                switch (lbutt.name)
                {
                    case "Skin":
                        pbe.nakedLower.texture = null;
                        pbe.nakedUpper.texture = null;
                        break;
                    case "Hair":
                        pbe.dirty = true;
                        break;
                }
            }

            if (base.genderButtons.Count > 0)
            {
                foreach (ClickableComponent c6 in base.genderButtons)
                {
                    if (c6.containsPoint(x, y))
                    {
                        //reset the look
                        PlayerBaseExtended.Get(Game1.player).DefaultOptions(Game1.player);
                    }
                }
            }

            foreach (ClickableComponent rbutt in base.rightSelectionButtons)
            {
                switch (rbutt.name)
                {
                    case "Skin":
                        pbe.nakedLower.texture = null;
                        pbe.nakedUpper.texture = null;
                        break;
                    case "Hair":
                        pbe.dirty = true;
                        break;
                }
            }
            Color HairCur = Game1.player.hairstyleColor.Value;
            //run default
            base.receiveLeftClick(x,y, playSound);
            if (!HairCur.Equals(Game1.player.hairstyleColor.Value))
            {
                pbe.dirty = true;
            }
        }

        public override void performHoverAction(int x, int y)
        {
            if (doctorsButton.containsPoint(x, y))
            {
                doctorsButton.scale = Math.Min(doctorsButton.scale + 0.02f, doctorsButton.baseScale * 1.1f);
            }
            else
            {
                doctorsButton.scale = Math.Max(doctorsButton.scale - 0.02f, doctorsButton.baseScale);
            }

            if (leahsButton.containsPoint(x, y))
            {
                leahsButton.scale = Math.Min(leahsButton.scale + 0.02f, leahsButton.baseScale * 1.1f);
            }
            else
            {
                leahsButton.scale = Math.Max(leahsButton.scale - 0.02f, leahsButton.baseScale);
            }

            if (pamsButton.containsPoint(x, y))
            {
                pamsButton.scale = Math.Min(pamsButton.scale + 0.02f, pamsButton.baseScale * 1.1f);
            }
            else
            {
                pamsButton.scale = Math.Max(pamsButton.scale - 0.02f, pamsButton.baseScale);
            }

            base.performHoverAction(x, y);
        }





        public override void draw(SpriteBatch b)
        {
            base.draw(b);
            //Draw the new buttons
            doctorsButton.draw(b);
            leahsButton.draw(b);
            pamsButton.draw(b);
            //Redraw the mouse so it is over the buttons
            base.drawMouse(b);
        }
     }
}
