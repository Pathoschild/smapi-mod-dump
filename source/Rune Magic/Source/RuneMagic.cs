/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/facufierro/RuneMagic
**
*************************************************/

using JsonAssets.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using RuneMagic.Source.Interface;
using RuneMagic.Source.Items;
using SpaceCore;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using System.Xml.Linq;
using xTile.Dimensions;
using static SpaceCore.Skills;
using Object = StardewValley.Object;

namespace RuneMagic.Source
{
    public sealed class RuneMagic : Mod
    {
        public static RuneMagic Instance { get; private set; }
        public static readonly Dictionary<string, Texture2D> Textures = new();

        public static JsonAssets.IApi JsonAssetsApi { get; private set; }
        public static IApi SpaceCoreApi { get; private set; }
        public static IGenericModConfigMenuApi ConfigMenuApi { get; private set; }

        public static ModConfig Config { get; private set; }

        static RuneMagic()
        {
            LoadTextures();
        }

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Config = Helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.Saving += OnSaving;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.Player.Warped += OnWarped;
            helper.Events.Display.RenderedHud += OnRenderedHud;
            helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;
            SpaceCore.Events.SpaceEvents.OnEventFinished += OnEventFinished;
            SpaceCore.Events.SpaceEvents.OnBlankSave += OnBlankSave;
        }

        //Event Handlers
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            SpaceCoreApi = Helper.ModRegistry.GetApi<IApi>("spacechase0.SpaceCore");
            JsonAssetsApi = Helper.ModRegistry.GetApi<JsonAssets.IApi>("spacechase0.JsonAssets");
            ConfigMenuApi = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            SpaceCoreApi.RegisterSerializerType(typeof(SpellBook));
            SpaceCoreApi.RegisterSerializerType(typeof(Rune));
            SpaceCoreApi.RegisterSerializerType(typeof(Scroll));
            SpaceCoreApi.RegisterSerializerType(typeof(MagicWeapon));

            JsonAssetsApi.ItemsRegistered += OnItemsRegistered;

            RegisterConfigMenu();
        }

        private void OnItemsRegistered(object sender, EventArgs e)
        {
            var jsonAssetsInstance = JsonAssets.Mod.instance;

            //Register BigCraftables
            jsonAssetsInstance.RegisterBigCraftable(ModManifest, SetBigCraftableData("Runic Anvil", "An anvil marked with strange runes.", Textures["big_craftable"],
                new() { new BigCraftableIngredient() { Object = "Iron Bar", Count = 20 }, new BigCraftableIngredient() { Object = "Amethyst", Count = 25 }, }));
            jsonAssetsInstance.RegisterBigCraftable(ModManifest, SetBigCraftableData("Inscription Table", "A table marked with strange runes.", Textures["big_craftable"],
                new() { new BigCraftableIngredient() { Object = "Wood", Count = 40 }, new BigCraftableIngredient() { Object = "Amethyst", Count = 25 }, }));
            jsonAssetsInstance.RegisterBigCraftable(ModManifest, SetBigCraftableData("Magic Grinder", "It's used to produce magic dust for glyphs.", Textures["magic_grinder"],
                new() { new BigCraftableIngredient() { Object = "Stone", Count = 40 }, new BigCraftableIngredient() { Object = "Topaz", Count = 25 }, }));

            //Register Runes and Scrolls
            int textureIndex = 0;
            foreach (var spell in Spell.List)
            {
                jsonAssetsInstance.RegisterObject(ModManifest, SetScrollData(spell));
                jsonAssetsInstance.RegisterObject(ModManifest, SetRuneData(spell, textureIndex));
                if (textureIndex == 8)
                    textureIndex = 0;
                else
                    textureIndex++;
            }

            //Register other Objects
            jsonAssetsInstance.RegisterObject(ModManifest, SetObjectData("Blank Rune", "A stone carved and prepared to carve runes in it.", Textures[$"blank_rune"],
                new List<ObjectIngredient>() { new ObjectIngredient() { Object = "Stone", Count = 1 }, }));
            jsonAssetsInstance.RegisterObject(ModManifest, SetObjectData("Blank Parchment", "A peace of parchment ready for inscribing.", Textures[$"blank_parchment"],
                new List<ObjectIngredient>() { new ObjectIngredient() { Object = "Fiber", Count = 1 }, }));
            jsonAssetsInstance.RegisterObject(ModManifest, SetObjectData("Magic Dust", "Magically processed dust obtained from Gems", Textures[$"magic_dust"], null));
            jsonAssetsInstance.RegisterObject(ModManifest, SetObjectData("Spell Book", "A magic journal with intricate symbols on cover and pages detailing secrets of spellcasting.", Textures[$"spell_book"], null));
            //Register Weapons
            jsonAssetsInstance.RegisterWeapon(ModManifest, SetWeaponData("Apprentice Staff", "A stick with strange markings in it.", Textures[$"apprentice_staff"], WeaponType.Club, 10));
            jsonAssetsInstance.RegisterWeapon(ModManifest, SetWeaponData("Adept Staff", "A stick with strange markings in it.", Textures[$"adept_staff"], WeaponType.Club, 80));
            jsonAssetsInstance.RegisterWeapon(ModManifest, SetWeaponData("Master Staff", "A stick with strange markings in it.", Textures[$"master_staff"], WeaponType.Club, 120));
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
        }

        private void OnRenderedHud(object sender, RenderedHudEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            Player.MagicStats.LearnRecipes();

            if (Game1.player.CurrentItem is ISpellCastingItem spellItem)
            {
                if (Instance.Helper.Input.IsDown(Config.ActionBarKey))
                {
                    if (spellItem is SpellBook spellBook)
                        spellBook.ActionBar.Render(e.SpriteBatch);
                }
                if (Instance.Helper.Input.IsDown(Config.CastKey))
                {
                    Player.MagicStats.CastBar.Render(e.SpriteBatch, spellItem);
                }
            }
        }

        private void OnBlankSave(object sender, EventArgs e)
        {
            if (Config.DevMode)
            {
                Game1.player.addItemByMenuIfNecessary(new SpellBook(JsonAssetsApi.GetObjectId("Spell Book"), 1));
                Game1.player.addItemByMenuIfNecessary(new Object(Vector2.Zero, JsonAssetsApi.GetBigCraftableId("Runic Anvil")));
                Game1.player.addItemByMenuIfNecessary(new Object(Vector2.Zero, JsonAssetsApi.GetBigCraftableId("Inscription Table")));
                Game1.player.addItemByMenuIfNecessary(new Object(JsonAssetsApi.GetObjectId("Magic Dust"), 100));
                Game1.player.addItemByMenuIfNecessary(new Object(JsonAssetsApi.GetObjectId("Blank Rune"), 100));
                Game1.player.addItemByMenuIfNecessary(new Object(JsonAssetsApi.GetObjectId("Blank Parchment"), 100));

                Player.MagicStats.MagicLearned = true;
                Player.MagicStats.RuneCraftingLearned = true;
                Player.MagicStats.ScrollScribingLearned = true;
            }
        }

        private void OnSaving(object sender, SavingEventArgs e)
        {
            foreach (var item in Game1.player.Items)
            {
                if (item is ISpellCastingItem)
                    (item as ISpellCastingItem).Spell = null;
            }
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            Player.MagicStats.Update();
            if (Game1.player.CurrentItem is ISpellCastingItem)
            {
                //PlayerStats.Cast(Game1.player.CurrentItem as ISpellCastingItem);
                PlayerAnimation();
            }
        }

        private void OnOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
        }

        private void OnEventFinished(object sender, EventArgs e)
        {
            if (Game1.CurrentEvent.id == 15065001)
            {
                Game1.player.addItemToInventory(new SpellBook(JsonAssetsApi.GetObjectId("Spell Book"), 1));

                Player.MagicStats.MagicLearned = true;
                Player.MagicStats.LearnRecipes();
                List<Response> responses = new();
                foreach (var school in School.All)
                {
                    responses.Add(new Response(school.Name, School.Abjuration.Name));
                }

                Game1.currentLocation.createQuestionDialogue("Choose your Specialization:", responses.ToArray(), (Farmer f, string responseKey) =>
                {
                    Player.MagicStats.ActiveSchool = School.All.Where(x => x.Name == responseKey).FirstOrDefault();
                });
            }
            if (Game1.CurrentEvent.id == 15065002)
            {
                Player.MagicStats.ScrollScribingLearned = true;
                Player.MagicStats.LearnRecipes();
            }
            if (Game1.CurrentEvent.id == 15065003)
            {
                Player.MagicStats.RuneCraftingLearned = true;
                Player.MagicStats.LearnRecipes();
            }
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            foreach (Item item in Game1.player.Items)
            {
                if (item is ISpellCastingItem magicItem && magicItem.Spell == null)
                {
                    magicItem.InitializeSpell();
                }
            }

            //if the player has a SpellBook in the inventory set all its slots to active
            foreach (Item item in Game1.player.Items)
            {
                if (item is SpellBook spellBook)
                {
                    spellBook.UpdateSpellSlots();
                }
            }
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Context.IsWorldReady)
            {
                if (e.Button == SButton.MouseRight)
                    Player.MagicStats.MagicCraftingActions();
                if (e.Button == Config.SpellBookKey)
                    //if the player has a spellbook in inventory open spellmenu
                    foreach (Item item in Game1.player.Items)
                    {
                        if (item is SpellBook spellBook)
                        {
                            Game1.activeClickableMenu = new SpellBookMenu(spellBook);
                            break;
                        }
                    }

                if (Config.DevMode)
                {
                    switch (e.Button)
                    {
                        case SButton.F9:
                            Player.MagicStats.ActiveSchool.Experience += 13000;
                            break;

                        case SButton.F11:
                            Game1.nextMineLevel();
                            break;

                        case SButton.F12:

                            Game1.warpFarmer("UndergroundMine2", 10, 4, false);
                            break;
                    }
                }
            }
        }

        private void OnWarped(object sender, WarpedEventArgs e)
        {
            TriggerEvent(e.NewLocation);
            Monitor.Log(e.NewLocation.Name);
        }

        public BigCraftableData SetBigCraftableData(string name, string description, Texture2D texture, List<BigCraftableIngredient> ingredients)
        {
            BigCraftableRecipe recipe = null;
            if (ingredients is not null)
            {
                recipe = new BigCraftableRecipe()
                {
                    ResultCount = 1,
                    Ingredients = ingredients.ToList(),
                    IsDefault = false
                };
            }
            return new BigCraftableData()
            {
                Name = $"{name}",
                Description = $"{description}",
                Texture = texture,
                Price = 0,
                Recipe = recipe
            };
        }

        public ObjectData SetObjectData(string name, string description, Texture2D texture, List<ObjectIngredient> ingredients)
        {
            ObjectRecipe recipe = null;
            if (ingredients is not null)
            {
                recipe = new ObjectRecipe()
                {
                    ResultCount = 1,
                    Ingredients = ingredients.OfType<ObjectIngredient>().ToList(),
                    IsDefault = false
                };
            }
            return new ObjectData()
            {
                Name = $"{name}",
                Description = $"{description}",
                Texture = texture,
                Category = ObjectCategory.Crafting,
                Price = 0,
                HideFromShippingCollection = true,
                Recipe = recipe,
            };
        }

        public WeaponData SetWeaponData(string name, string description, Texture2D texture, WeaponType type, int mineDropVar)
        {
            return new WeaponData()
            {
                Name = $"{name}",
                Description = $"{description}",
                Texture = texture,
                Type = type,
                MinimumDamage = 6,
                MaximumDamage = 12,
                Knockback = 0,
                Speed = -20,
                Accuracy = 100,
                Defense = 1,
                CritChance = 0.04,
                CritMultiplier = 1.5,
                ExtraSwingArea = 0,
                CanPurchase = true,
                CanTrash = true,
                MineDropVar = mineDropVar,
                MineDropMinimumLevel = 10,
            };
        }

        public ObjectData SetRuneData(Spell spell, int textureIndex)
        {
            var texture = Textures[$"rune_{textureIndex}"];
            var data = new Color[texture.Width * texture.Height];
            texture.GetData(data);
            for (int i = 0; i < data.Length; ++i)
            {
                if (data[i] == Color.White)
                    data[i] = spell.School.Colors.Item1;
                if (data[i] == Color.Black)
                    data[i] = spell.School.Colors.Item2;
            }
            texture.SetData(data);
            return new ObjectData()
            {
                Name = $"Rune of {spell.Name}",
                Description = $"{spell.Description}",
                Texture = texture,
                Category = ObjectCategory.Crafting,
                CategoryTextOverride = $"{spell.School.Name}",
                CategoryColorOverride = spell.School.Colors.Item2,
                Price = 0,
                HideFromShippingCollection = true,
                Recipe = new ObjectRecipe()
                {
                    ResultCount = 1,
                    IsDefault = false,
                    Ingredients = new List<ObjectIngredient>()
                    {
                        new ObjectIngredient()
                        {
                            Object = "Blank Rune",
                            Count = 1,
                        },
                         new ObjectIngredient()
                        {
                            Object = "Magic Dust",
                            Count = 5 + spell.Level * 2,
                        },
                    }
                }
            };
        }

        public ObjectData SetScrollData(Spell spell)
        {
            var texture = Textures["scroll"];
            var data = new Color[texture.Width * texture.Height];
            texture.GetData(data);
            for (int i = 0; i < data.Length; ++i)
            {
                if (data[i] == Color.White)
                    data[i] = spell.School.Colors.Item1;
                if (data[i] == Color.Black)
                    data[i] = spell.School.Colors.Item2;
            }
            texture.SetData(data);
            return new ObjectData()
            {
                Name = $"{spell.Name} Scroll",
                Description = $"{spell.Description}",
                Texture = texture,
                Category = ObjectCategory.Crafting,
                CategoryTextOverride = $"{spell.School.Name}",
                CategoryColorOverride = spell.School.Colors.Item2,
                Price = 0,
                HideFromShippingCollection = true,
                Recipe = new ObjectRecipe()
                {
                    ResultCount = 1,
                    IsDefault = false,
                    Ingredients = new List<ObjectIngredient>()
                    {
                        new ObjectIngredient()
                        {
                            Object = "Blank Parchment",
                            Count = 1,
                        },
                         new ObjectIngredient()
                        {
                            Object = "Magic Dust",
                            Count = spell.Level,
                        },
                    }
                }
            };
        }

        public void TriggerEvent(GameLocation location)
        {
            //Instance.Monitor.Log(PlayerStats.MagicLearned.ToString());
            if (!Game1.player.eventsSeen.Contains(15065001))
                if (Player.MagicStats.MagicLearned == false && location.Name == "WizardHouse" && Game1.player.getFriendshipHeartLevelForNPC("Wizard") >= 3)
                {
                    var eventString = $"WizardSong/6 18/Wizard 10 15 2 farmer 8 24 0/skippable" +
                        $"/speak Wizard \"@! Come in my friend, come in...\"" +
                        $"/pause 400" +
                        $"/advancedMove Wizard false -2 0 3 100 0 2 2 3000" +
                        $"/move farmer 0 -6 0 true" +
                        $"/speak Wizard \"What do you think of this? beautiful isn't it?\"" +
                        $"/pause 500" +
                        $"/speak Wizard \"It's a spell book, a gift for you.\"" +
                        $"/pause 1000" +
                        $"/speak Wizard \"Now pay attention, young adept! I am going to teach you everything you need to know to use it!\"" +
                        $"/end";
                    location.startEvent(new Event(eventString, 15065001));
                }
            if (!Game1.player.eventsSeen.Contains(15065002))
                if (location.Name == "WizardHouse" && Game1.player.getFriendshipHeartLevelForNPC("Wizard") >= 5 && Player.MagicStats.ScrollScribingLearned == false && Player.MagicStats.MagicLearned == true)
                {
                    var eventString = $"WizardSong/6 18/Wizard 10 15 2 farmer 8 24 0/skippable" +
                           $"/speak Wizard \"@! Come in young adept, come in...\"" +
                           $"/pause 400" +
                           $"/advancedMove Wizard false -2 0 3 100 0 2 2 3000" +
                           $"/move farmer 0 -6 0 true" +
                           $"/pause 2000" +
                           $"/speak Wizard \"Today I am going to teach you a new form of magic.\"" +
                           $"/pause 500" +
                           $"/speak Wizard \"Scroll Scribing.\"" +
                           $"/pause 1000" +
                           $"/speak Wizard \"Now pay attention, this can be a bit tricky. And if not done properly even dangerous!\"" +
                           $"/end";
                    location.startEvent(new Event(eventString, 15065002));
                }
            if (location.Name == "WizardHouse" && Game1.player.getFriendshipHeartLevelForNPC("Wizard") >= 6 && Player.MagicStats.MagicLearned == true)
            {
                //var eventString = $"WizardSong/6 18/Wizard 10 15 2 farmer 8 24 0/skippable" +
                //       $"/speak Wizard \"@... I know why you are here my friend...\"" +
                //       $"/pause 400" +
                //       $"/move farmer 0 -6 0 true" +
                //       $"/pause 2000" +
                //       $"/speak Wizard \"You want to specialize in another magic school. I understand, but it's not possible. \"" +
                //       $"/pause 500" +
                //       $"/speak Wizard \"The only way is to reverse your current knowledge and select another. I'm sorry.\"" +
                //       $"/pause 1000" +
                //       $"/speak Wizard \"I'll give you the means to do it, but be warned, it's a costly process.\"" +
                //       $"/end";
                //location.startEvent(new Event(eventString, 15065004));
            }
            if (!Game1.player.eventsSeen.Contains(15065003))
                if (location.Name == "Mine" && Game1.player.getFriendshipHeartLevelForNPC("Dwarf") >= 5 && Player.MagicStats.RuneCraftingLearned == false && Game1.player.canUnderstandDwarves && Player.MagicStats.MagicLearned == true)
                {
                    var eventString = $"WizardSong/43 8/Dwarf 43 6 2 farmer 39 8 1/skippable" +
                           $"/speak Dwarf \"Hey!\"" +
                           $"/pause 400" +
                           $"/advancedMove farmer false 43 8 1 100 43 7 3 3000" +
                           $"/pause 2000" +
                           $"/speak Dwarf \"Did you know that dwarves know how to use magic?.\"" +
                           $"/pause 500" +
                           $"/speak Wizard \"We do it different though, like this!\"" +
                           $"/pause 1000" +
                           $"/speak Wizard \"Wanna try? It's not hard if you already know the basics.\"" +
                           $"/end";
                    location.startEvent(new Event(eventString, 15065003));
                }
        }

        public void PlayerAnimation()
        {
            var time = 1;
            if (!Game1.player.FarmerSprite.PauseForSingleAnimation && !Game1.player.UsingTool)
            {
                if (Game1.player.isRidingHorse() && !Game1.player.mount.dismounting.Value)
                {
                    Game1.player.showRiding();
                }
                else if (Game1.player.FacingDirection == 3 && Game1.player.isMoving() && Game1.player.running)
                {
                    Game1.player.FarmerSprite.animate(56, time);
                }
                else if (Game1.player.FacingDirection == 1 && Game1.player.isMoving() && Game1.player.running)
                {
                    Game1.player.FarmerSprite.animate(40, time);
                }
                else if (Game1.player.FacingDirection == 0 && Game1.player.isMoving() && Game1.player.running)
                {
                    Game1.player.FarmerSprite.animate(48, time);
                }
                else if (Game1.player.FacingDirection == 2 && Game1.player.isMoving() && Game1.player.running)
                {
                    Game1.player.FarmerSprite.animate(32, time);
                }
                else if (Game1.player.FacingDirection == 3 && Game1.player.isMoving())
                {
                    Game1.player.FarmerSprite.animate(24, time);
                }
                else if (Game1.player.FacingDirection == 1 && Game1.player.isMoving())
                {
                    Game1.player.FarmerSprite.animate(8, time);
                }
                else if (Game1.player.FacingDirection == 0 && Game1.player.isMoving())
                {
                    Game1.player.FarmerSprite.animate(16, time);
                }
                else if (Game1.player.FacingDirection == 2 && Game1.player.isMoving())
                {
                    Game1.player.FarmerSprite.animate(0, time);
                }
                else if (Player.MagicStats.CastingTime > 0)
                {
                    Game1.player.faceDirection(2);
                    Game1.player.showCarrying();
                }
                else
                {
                    Game1.player.showNotCarrying();
                }
            }
        }

        public void RegisterConfigMenu()
        {
            if (ConfigMenuApi is null)
                return;

            ConfigMenuApi.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config));
            ConfigMenuApi.AddKeybind(
                mod: ModManifest,
                name: () => "Casting Key",
                getValue: () => Config.CastKey,
                setValue: value => Config.CastKey = value);
            ConfigMenuApi.AddKeybind(
                mod: ModManifest,
                name: () => "Spell Selection Key",
                getValue: () => Config.ActionBarKey,
                setValue: value => Config.ActionBarKey = value);
            ConfigMenuApi.AddKeybind(
                mod: ModManifest,
                name: () => "Spell Book Menu Key",
                getValue: () => Config.SpellBookKey,
                setValue: value => Config.SpellBookKey = value);

            ConfigMenuApi.AddNumberOption(
                mod: ModManifest,
                name: () => "Castbar Scale",
                getValue: () => Config.CastbarScale,
                setValue: value => Config.CastbarScale = value,
                min: 1,
                max: 3);
            ConfigMenuApi.SetTitleScreenOnlyForNextOptions(ModManifest, true);
            ConfigMenuApi.AddBoolOption(
               mod: ModManifest,
               name: () => "Developer Mode",
               getValue: () => Config.DevMode,
               setValue: value => Config.DevMode = value);
        }

        public static void LoadTextures()
        {
            foreach (var file in Directory.GetFiles("Mods\\RuneMagic\\assets\\Spells"))
            {
                var texture = Texture2D.FromStream(Game1.graphics.GraphicsDevice, File.OpenRead($"{file}"));
                Textures.Add($"{Path.GetFileNameWithoutExtension(file)}", texture);
            }
            foreach (var file in Directory.GetFiles("Mods\\RuneMagic\\assets\\Glyphs"))
            {
                var texture = Texture2D.FromStream(Game1.graphics.GraphicsDevice, File.OpenRead($"{file}"));

                Textures.Add($"{Path.GetFileNameWithoutExtension(file)}", texture);
            }
            foreach (var file in Directory.GetFiles("Mods\\RuneMagic\\assets\\Runes"))
            {
                var texture = Texture2D.FromStream(Game1.graphics.GraphicsDevice, File.OpenRead($"{file}"));

                Textures.Add($"{Path.GetFileNameWithoutExtension(file)}", texture);
            }
            foreach (var file in Directory.GetFiles("Mods\\RuneMagic\\assets\\Items"))
            {
                var texture = Texture2D.FromStream(Game1.graphics.GraphicsDevice, File.OpenRead($"{file}"));
                Textures.Add($"{Path.GetFileNameWithoutExtension(file)}", texture);
            }
            foreach (var file in Directory.GetFiles("Mods\\RuneMagic\\assets\\Interface"))
            {
                var texture = Texture2D.FromStream(Game1.graphics.GraphicsDevice, File.OpenRead($"{file}"));
                Textures.Add($"{Path.GetFileNameWithoutExtension(file)}", texture);
            }
        }
    }
}