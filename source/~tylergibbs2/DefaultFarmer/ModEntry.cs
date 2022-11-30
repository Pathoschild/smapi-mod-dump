/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Reflection;

namespace DefaultFarmer
{
    public class ModEntry : Mod
    {
        public static IDataHelper Data;

        private IClickableMenu oldSubMenu;

        public override void Entry(IModHelper helper)
        {
            Data = helper.Data;

            helper.Events.Display.MenuChanged += MenuChanged;

            helper.Events.GameLoop.UpdateTicked += UpdateTicked;
            helper.Events.GameLoop.GameLaunched += Migrate;
        }

        public void Migrate(object sender, GameLaunchedEventArgs e)
        {
            FarmerCustomizationData data = Data.ReadGlobalData<FarmerCustomizationData>("farmer-defaults");

            if (data is null)
                return;

            bool migrated = false;

            // Have to migrate from the old format
            for (int i = 0; i < CharacterCustomizationDefaults.presetCount; i++)
            {
                FarmerCustomizationData newData = Data.ReadGlobalData<FarmerCustomizationData>($"farmer-defaults-{i}");

                // Only migrate if there is an empty slot
                if (newData is null)
                {
                    Data.WriteGlobalData($"farmer-defaults-{i}", data);
                    Data.WriteGlobalData<FarmerCustomizationData>("farmer-defaults", null);
                    migrated = true;
                    break;
                }
            }

            if (!migrated)
                throw new Exception("Failed to perform migration. All save slots are full! How?");

            Monitor.Log("Successfully migrated DefaultFarmer to new index-based format.");
        }

        public static void LoadDefaults(CharacterCustomizationDefaults menu, int which)
        {
            FarmerCustomizationData data = Data.ReadGlobalData<FarmerCustomizationData>($"farmer-defaults-{which}");
            if (data is null)
            {
                Game1.resetPlayer();
                menu.GetType().BaseType.GetField("_displayFarmer", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(menu, Game1.player);
                data = new();
            }

            TextBox nameBox = (TextBox)menu.GetType().BaseType.GetField("nameBox", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(menu);
            TextBox farmnameBox = (TextBox)menu.GetType().BaseType.GetField("farmnameBox", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(menu);
            TextBox favThingBox = (TextBox)menu.GetType().BaseType.GetField("favThingBox", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(menu);

            nameBox.Text = data.Name;

            if (menu.source != CharacterCustomization.Source.NewFarmhand)
            {
                farmnameBox.Text = data.FarmName;
                menu.GetType().BaseType.GetField("skipIntro", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(menu, data.SkipIntro);
                menu.skipIntroButton.sourceRect.X = data.SkipIntro ? 236 : 227;
                Game1.player.catPerson = data.CatPerson;
                Game1.player.whichPetBreed = data.Pet;
            }

            favThingBox.Text = data.FavThing;

            Game1.player.changeGender(male: data.Gender);
            Game1.player.changeEyeColor(data.EyeColor);
            Game1.player.changeHairColor(data.HairColor);
            Game1.player.changePants(data.PantsColor);
            Game1.player.changeSkinColor(data.Skin);
            Game1.player.changeHairStyle(data.Hair);
            Game1.player.changeShirt(data.Shirt);
            Game1.player.changePantStyle(data.Pants, is_customization_screen: true);
            Game1.player.changeAccessory(data.Accessory);

            menu.eyeColorPicker.setColor(data.EyeColor);
            menu.hairColorPicker.setColor(data.HairColor);
            menu.pantsColorPicker.setColor(data.PantsColor);
        }

        public static void SaveDefaults(CharacterCustomizationDefaults menu, int which)
        {
            bool skipIntro = (bool)menu.GetType().BaseType.GetField("skipIntro", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(menu);

            TextBox nameBox = (TextBox)menu.GetType().BaseType.GetField("nameBox", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(menu);
            TextBox farmnameBox = (TextBox)menu.GetType().BaseType.GetField("farmnameBox", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(menu);
            TextBox favThingBox = (TextBox)menu.GetType().BaseType.GetField("favThingBox", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(menu);

            FarmerCustomizationData data = new()
            {
                Name = nameBox.Text,
                FarmName = farmnameBox.Text,
                FavThing = favThingBox.Text,
                Gender = Game1.player.IsMale,
                EyeColor = menu.eyeColorPicker.getSelectedColor(),
                HairColor = menu.hairColorPicker.getSelectedColor(),
                PantsColor = menu.pantsColorPicker.getSelectedColor(),
                Skin = Game1.player.skin.Value,
                Hair = Game1.player.hair.Value,
                Shirt = Game1.player.shirt.Value,
                Pants = Game1.player.pants.Value,
                Accessory = Game1.player.accessory.Value,
                CatPerson = Game1.player.catPerson,
                Pet = Game1.player.whichPetBreed,
                SkipIntro = skipIntro
            };

            Data.WriteGlobalData($"farmer-defaults-{which}", data);
        }

        public void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (Game1.activeClickableMenu is not TitleMenu)
                return;

            if (oldSubMenu != TitleMenu.subMenu)
                oldSubMenu = TitleMenu.subMenu;

            if (TitleMenu.subMenu is CharacterCustomization && TitleMenu.subMenu is not CharacterCustomizationDefaults)
            {
                CharacterCustomization.Source source = (TitleMenu.subMenu as CharacterCustomization).source;
                if (source is CharacterCustomization.Source.NewGame || source is CharacterCustomization.Source.NewFarmhand || source is CharacterCustomization.Source.HostNewFarm)
                    TitleMenu.subMenu = new CharacterCustomizationDefaults(source);
            }
        }

        public void MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is CharacterCustomization && e.NewMenu is not CharacterCustomizationDefaults)
            {
                CharacterCustomization.Source source = (e.NewMenu as CharacterCustomization).source;
                if (source is CharacterCustomization.Source.NewFarmhand)
                    Game1.activeClickableMenu = new CharacterCustomizationDefaults(source);
            }
        }
    }
}
