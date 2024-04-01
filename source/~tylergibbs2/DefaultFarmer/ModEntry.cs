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
        private static IDataHelper Data = null!;

        private IClickableMenu? OldSubMenu;

        public override void Entry(IModHelper helper)
        {
            Data = helper.Data;

            helper.Events.Display.MenuChanged += MenuChanged;

            helper.Events.GameLoop.UpdateTicked += UpdateTicked;
            helper.Events.GameLoop.GameLaunched += Migrate;
        }

        private void MigrateToIndexed()
        {
            for (int i = 0; i < CharacterCustomizationDefaults.presetCount; i++)
            {
                FarmerCustomizationData1_6? data_1_6 = Data.ReadGlobalData<FarmerCustomizationData1_6>($"farmer-defaults-1_6-{i}");
                if (data_1_6 is not null)
                {
                    Monitor.Log("Detected 1.6 data, skipping index migration.");
                    return;
                }
            }

            FarmerCustomizationData1_5? data = Data.ReadGlobalData<FarmerCustomizationData1_5>("farmer-defaults");

            if (data is null)
                return;

            bool migrated = false;

            // Have to migrate from the old format
            for (int i = 0; i < CharacterCustomizationDefaults.presetCount; i++)
            {
                FarmerCustomizationData1_5? newData = Data.ReadGlobalData<FarmerCustomizationData1_5>($"farmer-defaults-{i}");

                // Only migrate if there is an empty slot
                if (newData is null)
                {
                    Data.WriteGlobalData($"farmer-defaults-{i}", data);
                    Data.WriteGlobalData<FarmerCustomizationData1_5>("farmer-defaults", null);
                    migrated = true;
                    break;
                }
            }

            if (!migrated)
                throw new Exception("Failed to perform migration. All save slots are full! How?");

            Monitor.Log("Successfully migrated DefaultFarmer to new index-based format.");
        }

        private FarmerCustomizationData1_6 Convert1_5DataTo1_6Data(FarmerCustomizationData1_5 data)
        {
            return new FarmerCustomizationData1_6
            {
                Name = data.Name,
                FarmName = data.FarmName,
                FavThing = data.FavThing,
                Gender = data.Gender,
                EyeColor = data.EyeColor,
                HairColor = data.HairColor,
                PantsColor = data.PantsColor,
                Skin = data.Skin,
                Hair = data.Hair,
                Shirt = ClothingHelper.GetNewShirtId(data.Shirt),
                Pants = ClothingHelper.GetNewPantsId(data.Pants),
                Accessory = data.Accessory,
                WhichPetType = data.CatPerson ? "Cat" : "Dog",
                WhichPetBreed = data.Pet.ToString(),
                SkipIntro = data.SkipIntro
            };
        }

        private void MigrateTo1_6()
        {
            for (int i = 0; i < CharacterCustomizationDefaults.presetCount; i++)
            {
                FarmerCustomizationData1_6? data_1_6 = Data.ReadGlobalData<FarmerCustomizationData1_6>($"farmer-defaults-1_6-{i}");
                if (data_1_6 is not null)
                {
                    Monitor.Log("Detected 1.6 data, skipping 1.5 -> 1.6 migration.");
                    return;
                }
            }

            for (int i = 0; i < CharacterCustomizationDefaults.presetCount; i++)
            {
                FarmerCustomizationData1_5? oldData = Data.ReadGlobalData<FarmerCustomizationData1_5>($"farmer-defaults-{i}");
                if (oldData is null)
                    continue;

                FarmerCustomizationData1_6 newData = Convert1_5DataTo1_6Data(oldData);
                Data.WriteGlobalData($"farmer-defaults-1_6-{i}", newData);
            }

            Monitor.Log("Successfully migrated DefaultFarmer data from 1.5 -> 1.6");
        }

        private void Migrate(object? sender, GameLaunchedEventArgs e)
        {
            MigrateToIndexed();
            MigrateTo1_6();
        }


        public static void LoadDefaults(CharacterCustomizationDefaults menu, int which)
        {
            FarmerCustomizationData1_6? data = Data.ReadGlobalData<FarmerCustomizationData1_6>($"farmer-defaults-1_6-{which}");
            if (data is null)
            {
                Game1.resetPlayer();
                menu.GetType().BaseType!.GetField("_displayFarmer", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(menu, Game1.player);
                data = new();
            }

            TextBox nameBox = (TextBox)menu.GetType().BaseType!.GetField("nameBox", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(menu)!;
            TextBox farmnameBox = (TextBox)menu.GetType().BaseType!.GetField("farmnameBox", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(menu)!;
            TextBox favThingBox = (TextBox)menu.GetType().BaseType!.GetField("favThingBox", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(menu)!;

            nameBox.Text = data.Name;

            if (menu.source != CharacterCustomization.Source.NewFarmhand)
            {
                farmnameBox.Text = data.FarmName;
                menu.GetType().BaseType!.GetField("skipIntro", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(menu, data.SkipIntro);
                menu.skipIntroButton.sourceRect.X = data.SkipIntro ? 236 : 227;
                Game1.player.whichPetBreed = data.WhichPetBreed;
                Game1.player.whichPetType = data.WhichPetType;
            }

            favThingBox.Text = data.FavThing;

            Game1.player.changeGender(male: data.Gender);
            Game1.player.changeEyeColor(data.EyeColor);
            Game1.player.changeHairColor(data.HairColor);
            Game1.player.changePantsColor(data.PantsColor);
            Game1.player.changePantStyle(data.Pants);
            Game1.player.changeSkinColor(data.Skin);
            Game1.player.changeHairStyle(data.Hair);
            Game1.player.changeShirt(data.Shirt);
            Game1.player.changeAccessory(data.Accessory);

            menu.eyeColorPicker.setColor(data.EyeColor);
            menu.hairColorPicker.setColor(data.HairColor);
            menu.pantsColorPicker.setColor(data.PantsColor);
        }

        public static void SaveDefaults(CharacterCustomizationDefaults menu, int which)
        {
            bool skipIntro = (bool)menu.GetType().BaseType!.GetField("skipIntro", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(menu)!;

            TextBox nameBox = (TextBox)menu.GetType().BaseType!.GetField("nameBox", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(menu)!;
            TextBox farmnameBox = (TextBox)menu.GetType().BaseType!.GetField("farmnameBox", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(menu)!;
            TextBox favThingBox = (TextBox)menu.GetType().BaseType!.GetField("favThingBox", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(menu)!;

            FarmerCustomizationData1_6 data = new()
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
                WhichPetBreed = Game1.player.whichPetBreed,
                WhichPetType = Game1.player.whichPetType,
                SkipIntro = skipIntro
            };

            Data.WriteGlobalData($"farmer-defaults-1_6-{which}", data);
        }

        public void UpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (Game1.activeClickableMenu is not TitleMenu)
                return;

            if (OldSubMenu != TitleMenu.subMenu)
                OldSubMenu = TitleMenu.subMenu;

            if (TitleMenu.subMenu is CharacterCustomization && TitleMenu.subMenu is not CharacterCustomizationDefaults)
            {
                CharacterCustomization.Source source = (TitleMenu.subMenu as CharacterCustomization)!.source;
                if (source is CharacterCustomization.Source.NewGame || source is CharacterCustomization.Source.NewFarmhand || source is CharacterCustomization.Source.HostNewFarm)
                    TitleMenu.subMenu = new CharacterCustomizationDefaults(source);
            }
        }

        public void MenuChanged(object? sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is CharacterCustomization && e.NewMenu is not CharacterCustomizationDefaults)
            {
                CharacterCustomization.Source source = (e.NewMenu as CharacterCustomization)!.source;
                if (source is CharacterCustomization.Source.NewFarmhand)
                    Game1.activeClickableMenu = new CharacterCustomizationDefaults(source);
            }
        }
    }
}
