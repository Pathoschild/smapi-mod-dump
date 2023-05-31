/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.HUD;
using Omegasis.Revitalize.Framework.Managers;
using Omegasis.Revitalize.Framework.Menus.MenuComponents;
using Omegasis.Revitalize.Framework.Utilities.JsonContentLoading;
using Omegasis.Revitalize.Framework.World.Buildings;
using Omegasis.Revitalize.Framework.World.WorldUtilities;
using Omegasis.StardustCore.UIUtilities.MenuComponents.ComponentsV2.Buttons;
using StardewValley;
using StardewValley.Menus;

namespace Omegasis.Revitalize.Framework.Menus.Items
{
    public class DimensionalStorageUnitMenu : InventoryDisplayMenu
    {
        public ClickableTextureComponent _upgradeButton;
        public string upgradePromptUnformatted;
        public string upgradePrompt;
        public Item upgradeItem;
        public string missingItemPrompt;


        public DimensionalStorageUnitBuilding dimensionalStorageUnit;


        public DimensionalStorageUnitMenu(DimensionalStorageUnitBuilding dimensionalStorageUnitBuilding):base(new Color(Color.Purple.R/4,Color.Purple.G/4, Color.Purple.B/4),Color.White,dimensionalStorageUnitBuilding.items,dimensionalStorageUnitBuilding.DimensionalStorageUnitMaxItems)
        {
            this.upgradeItem = RevitalizeModCore.ModContentManager.objectManager.getItem(Enums.SDVObject.VoidEssence);
            this.upgradePromptUnformatted = JsonContentPackUtilities.LoadStringFromDictionaryFile(Path.Combine(Constants.PathConstants.StringsPaths.Menus, "DimensionalStorageUnit.json"), "UpgradePrompt");
            this.missingItemPrompt = JsonContentPackUtilities.LoadStringFromDictionaryFile(Path.Combine(Constants.PathConstants.StringsPaths.Menus, "DimensionalStorageUnit.json"), "MissingUpgradeItem");

            this.dimensionalStorageUnit= dimensionalStorageUnitBuilding;
            this.defaultTitle = JsonContentPackUtilities.LoadStringFromDictionaryFile(Path.Combine(Constants.PathConstants.StringsPaths.Menus, "DimensionalStorageUnit.json"), "Capacity");


            //We need to update the display texts here since the update for this class requires the dimensional storage unit to be set. However the texts are attempted to be updated in the base class before this happens, causing a null reference exception.
            this.updateDisplayTexts();
        }

        public override long getInventoryMaxCapacity()
        {
            return this.dimensionalStorageUnit.DimensionalStorageUnitMaxItems;
        }

        public override void setUpPositions()
        {
            base.setUpPositions();
            this._upgradeButton = new ClickableTextureComponent("UpgradeButton", new Rectangle(this.searchBox.X + this.searchBox.Width + 96, this.searchBox.Y, 192, 48), "", "", TextureManagers.Menus_DimensionalStorageMenu.getExtendedTexture("UpgradeButton").getTexture(), new Rectangle(0, 0, 192, 48), 1f);
        }

        public virtual void upgradeStorage()
        {
            long found = this.getUpgradeItemAmount();
            List<Item> dimensionalStorageVoidEssence = new List<Item>();
            foreach (Item voidEssence in this.dimensionalStorageUnit.items)
            {
                if (voidEssence.canStackWith(this.upgradeItem))
                {
                    dimensionalStorageVoidEssence.Add(voidEssence);
                }
            }
            bool updadeDimensionalStorageUnitDisplay = false;
            foreach (Item voidEssence in dimensionalStorageVoidEssence)
            {
                if (voidEssence.Stack == 1)
                {
                    //There's an odd edge case where the stack is truely never 0 so we need to have some special handling logic here.
                    this.dimensionalStorageUnit.items.Remove(voidEssence);
                    updadeDimensionalStorageUnitDisplay = true;
                    found--;
                }
                else
                {
                    long subTractionAmount = Math.Min((long)voidEssence.Stack, this.dimensionalStorageUnit.getUpgradeCost());
                    voidEssence.Stack -= (int)subTractionAmount; //Stacks should cap at about 999 anyways so no need to worry about conversion losses here.
                    if (voidEssence.Stack == 0)
                    {
                        this.dimensionalStorageUnit.items.Remove(voidEssence);
                        updadeDimensionalStorageUnitDisplay = true;
                    }
                    found -= subTractionAmount;
                }

                if (found == 0)
                {
                    break;
                }
            }
            foreach (Item item in Game1.player.Items)
            {
                if (item == null) continue;
                if (!item.canStackWith(this.upgradeItem))
                {
                    continue;
                }

                if (item.Stack == 1)
                {
                    //There's an odd edge case where the stack is truely never 0 so we need to have some special handling logic here.
                    Game1.player.Items.Remove(item);
                    found--;
                }
                else
                {
                    long subTractionAmount = Math.Min((long)item.Stack, this.dimensionalStorageUnit.getUpgradeCost());
                    item.Stack -= (int)subTractionAmount; //Stacks should cap at about 999 anyways so no need to worry about conversion losses here.
                    if (item.Stack == 0)
                    {
                        Game1.player.Items.Remove(item);
                    }
                    found -= subTractionAmount;
                }
                if (found == 0)
                {
                    break;
                }
            }
            this.dimensionalStorageUnit.DimensionalStorageUnitMaxItems++;
            if (updadeDimensionalStorageUnitDisplay)
            {
                this.populateItemsToDisplay();
            }
            this.updateDisplayTexts();
        }

        public override void handleButtonClick(string name)
        {
            switch (name)
            {
                case "UpgradeButton":

                    if (this.hasEnoughToUpgrade())
                    {
                        this.upgradeStorage();
                        SoundUtilities.PlaySound(Enums.StardewSound.coin);
                        return;
                    }
                    else
                    {
                        SoundUtilities.PlaySound(Enums.StardewSound.Cowboy_gunshot);
                        return;
                    }
            }
            base.handleButtonClick(name);
        }


        /// <summary>
        /// Gets the total amount of items found in dimensional storage unti that can be used for upgrades. In this case, void essence.
        /// </summary>
        /// <returns></returns>
        public virtual long getUpgradeItemAmount()
        {
            List<Item> dimensionalStorageVoidEssence = new List<Item>();
            foreach (Item voidEssence in this.dimensionalStorageUnit.items)
            {
                if (voidEssence.canStackWith(this.upgradeItem))
                {
                    dimensionalStorageVoidEssence.Add(voidEssence);
                }
            }

            long found = 0;
            foreach (Item voidEssence in dimensionalStorageVoidEssence)
            {
                found += (long)voidEssence.Stack;
            }
            foreach (Item voidEssence in Game1.player.Items)
            {
                if (voidEssence == null) continue;
                if (voidEssence.canStackWith(this.upgradeItem))
                {
                    found += (long)voidEssence.Stack;
                }
            }
            return found;
        }

        /// <summary>
        /// Is there enough items to upgrade the dimensional storage unit?
        /// </summary>
        /// <returns></returns>
        public virtual bool hasEnoughToUpgrade()
        {
            return this.getUpgradeItemAmount() >= this.dimensionalStorageUnit.getUpgradeCost();
        }

        public override void updateDisplayTexts()
        {
            if (this.dimensionalStorageUnit != null)
            {
                base.updateDisplayTexts();
                this.upgradePrompt = string.Format(this.upgradePromptUnformatted, (this.dimensionalStorageUnit.DimensionalStorageUnitMaxItems + 1).ToString(), this.dimensionalStorageUnit.getUpgradeCost().ToString(), this.upgradeItem.DisplayName);
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this._upgradeButton.containsPoint(x, y))
            {
                this.handleButtonClick(this._upgradeButton.name);
                return;
            }
            base.receiveLeftClick(x, y, playSound);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (this._upgradeButton.containsPoint(x, y))
            {
                this.handleButtonClick(this._upgradeButton.name);
                return;
            }
            base.receiveRightClick(x, y, playSound);
        }

        public override void performHoverAction(int x, int y)
        {
            if (this._upgradeButton.containsPoint(x, y) && this.hasEnoughToUpgrade())
            {
                this.hoverText = this.upgradePrompt;
                this._upgradeButton.scale = Math.Min(this._upgradeButton.scale + 0.02f, this._upgradeButton.baseScale + 0.1f);
            }
            else if (this._upgradeButton.containsPoint(x, y) && !this.hasEnoughToUpgrade())
            {
                this.hoverText = this.upgradePrompt + "\n" + string.Format(this.missingItemPrompt, this.getUpgradeItemAmount().ToString(), this.upgradeItem.DisplayName, this.dimensionalStorageUnit.getUpgradeCost().ToString());
                this._upgradeButton.scale = Math.Min(this._upgradeButton.scale + 0.02f, this._upgradeButton.baseScale + 0.1f);
            }
            else
            {
                base.performHoverAction(x, y);
            }
        }

        public override void draw(SpriteBatch b)
        {
            this._upgradeButton.draw(b);
            base.draw(b);
        }


    }
}
