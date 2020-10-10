/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/miketweaver/BashNinja_SDV_Mods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Buildings;
using System.Collections.Generic;

namespace Stardewponics
{
    /// <summary>The mod entry point.</summary>
    public class StardewponicsMod : Mod
    {
        public static IModHelper helper;

        /*********
		** Public methods
		*********/
        /// <summary>Initialise the mod.</summary>
        /// <param name="help">Provides methods for interacting with the mod directory, such as read/writing a config file or custom JSON files.</param>
        public override void Entry(IModHelper help)
        {
            helper = help;
            SaveEvents.AfterLoad += SaveEvents_AfterLoad;
            SaveEvents.AfterReturnToTitle += SaveEvents_AfterReturnToTitle;

        }

        private void SaveEvents_AfterReturnToTitle(object sender, EventArgs e)
        {
            MenuEvents.MenuChanged -= MenuEvents_MenuChanged;
        }

        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            MenuEvents.MenuChanged += MenuEvents_MenuChanged;
        }

        private void MenuEvents_MenuChanged(object sender, EventArgsClickableMenuChanged e)
        {

            if (e.NewMenu is CarpenterMenu robinMenu)
            {
                MenuEvents.MenuClosed += MenuEvents_MenuClosed;
                List<BluePrint> blueprints = this.Helper.Reflection.GetPrivateValue<List<BluePrint>>(robinMenu, "blueprints");
                blueprints.Add(CreateAquaponics());
                GameEvents.UpdateTick += GameEvents_UpdateTick;
            }
        }

        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            if (Game1.activeClickableMenu is CarpenterMenu carpenter)
            {
                if (carpenter.CurrentBlueprint.name == "Aquaponics")
                {
                    IPrivateField<Building> cBuilding = Helper.Reflection.GetPrivateField<Building>(carpenter, "currentBuilding");

                    if (!(cBuilding.GetValue() is Aquaponics))
                    {
                        cBuilding.SetValue(new Aquaponics(Vector2.Zero, Game1.getFarm()));
                    }
                }
            }
        }

        private void MenuEvents_MenuClosed(object sender, EventArgsClickableMenuClosed e)
        {
            GameEvents.UpdateTick -= GameEvents_UpdateTick;
            MenuEvents.MenuClosed -= MenuEvents_MenuClosed;
            Farm farm = Game1.getFarm();
            for (int i = 0; i < farm.buildings.Count; i++)
            {
                if (farm.buildings[i] is Building b && !(b is Aquaponics) && b.buildingType == "Aquaponics")
                {
                    farm.buildings[i] = new Aquaponics(new Vector2(b.tileX, b.tileY), farm);
                }
            }
        }

        private BluePrint CreateAquaponics()
        {
            BluePrint AquaBP = new BluePrint("Aquaponics");
            AquaBP.itemsRequired.Clear();

            string[] strArray2 = "390 200".Split(' ');//Cost
            int index = 0;
            while (index < strArray2.Length)
            {
                if (!strArray2[index].Equals(""))
                    AquaBP.itemsRequired.Add(Convert.ToInt32(strArray2[index]), Convert.ToInt32(strArray2[index + 1]));
                index += 2;
            }
            AquaBP.texture = this.Helper.Content.Load<Texture2D>(@"assets\greenhouse.xnb", ContentSource.ModFolder);
            AquaBP.humanDoor = new Point(-1, -1);
            AquaBP.animalDoor = new Point(-2, -1);
            AquaBP.mapToWarpTo = "null";
            AquaBP.displayName = "Aquaponics";
            AquaBP.description = "A place to grow plants using fertilized water from your Fish!";
            AquaBP.blueprintType = "Buildings";
            AquaBP.nameOfBuildingToUpgrade = "";
            AquaBP.actionBehavior = "null";
            AquaBP.maxOccupants = -1;
            AquaBP.moneyRequired = 100;
            AquaBP.tilesWidth = 14;
            AquaBP.tilesHeight = 7;
            AquaBP.getTileSheetIndexForStructurePlacementTile(0, 0);
            AquaBP.sourceRectForMenuView = new Microsoft.Xna.Framework.Rectangle(0, 0, 96, 96);
            AquaBP.namesOfOkayBuildingLocations.Clear();
            AquaBP.namesOfOkayBuildingLocations.Add("Farm");
            AquaBP.magical = false;

            return AquaBP;
        }

    }
}