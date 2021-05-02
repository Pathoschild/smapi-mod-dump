/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/heyseth/SDVMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Harmony;
using System.Collections.Generic;
using StardewValley.Tools;
using System.Reflection;
using StardewValley.Menus;
using System.Text;
using System.Linq;
using StardewModdingAPI.Utilities;

namespace heysethUmbrellas
{
    public class ModEntry : Mod, IAssetEditor
    {
        public static UmbrellaConfig Config { get; set; } = null;
        internal static IJsonAssetsApi JsonAssets;

        private StardewValley.Item lastItem;
        public static Boolean drawUmbrella = false;
        public static Boolean drawRegularFarmer = false;
        public static Boolean isMaleFarmer;
        public static Boolean isBaldFarmer;
        public static Boolean fullRedraw;
        public static Boolean justDrewFarmer = true;
        public static Boolean changingAppearance = false;

        public static Texture2D umbrellaTexture;
        public static Texture2D regularFarmerTexture;
        public static Texture2D umbrellaOverlayTextureBack;
        public static Texture2D umbrellaOverlayTextureSide;

        //add new umbrellas here
        public static List<string> umbrellaNames = new List<string> { "Tattered Umbrella", "Red Umbrella", "Orange Umbrella", "Yellow Umbrella", "Green Umbrella", "Blue Umbrella", "Purple Umbrella", "Black Umbrella" };
        public static List<Texture2D> umbrellaPlayerTextures = new List<Texture2D>();
        public static List<Texture2D> umbrellaTextureBack = new List<Texture2D>();
        public static List<Texture2D> umbrellaTextureSide = new List<Texture2D>();

        public static List<int> bestHats = new List<int> { 3, 40, 42 };
        public static List<int> goodHats = new List<int> { 66, 78, 17, 4, 28, 55, 71, 27, 75};

        public static List<string> bestHatNames = new List<string> { "Sombrero", "Living Hat", "Mushroom Cap" };
        public static List<string> goodHatNames = new List<string> { "Garbage Hat", "Frog Hat", "Sailor's Cap", "Straw Hat", "Sou'wester", "Fishing Hat", "Copper Pan", "Hard Hat", "Golden Helmet" };

        public static List<string> customBestHats = new List<string> { };
        public static List<string> customGoodHats = new List<string> {
            "Rain Hood", // IllogicalMoodSwing's Rainy Day Clothing
            "Yellow Rain Hood", "White Rain Hood", "Red Rain Hood", "Purple Rain Hood", "Pink Rain Hood", "Green Rain Hood", "Blue Rain Hood", "Black Rain Hood"  // Hope's Hats - Rain Hoods
        };

        public static int wetBuffIndex = 35;
        public static int ticksInRain = 0;
        public static float staminaDrainRatePerTick;
        public static float staminaDrainShield;

        //private const string cofModID = "KoihimeNakamura.ClimatesOfFerngill";

        private HarmonyInstance harmony;
        public override void Entry(IModHelper helper)
        {
            try
            {
                Config = Helper.ReadConfig<UmbrellaConfig>();
            }
            catch (Exception ex)
            {
                Monitor.Log($"Encountered an error while loading the config.json file. Default settings will be used instead. Full error message:\n-----\n{ex.ToString()}", LogLevel.Error);
                Config = new UmbrellaConfig();
            }
            this.Helper.WriteConfig<UmbrellaConfig>(Config);

            FarmerRendererPatches.Initialize(this.Monitor);
            UmbrellaPatch.Initialize(this.Monitor);

            //add new umbrellas here
            umbrellaTextureBack.Add(this.Helper.Content.Load<Texture2D>("assets/tattered/umbrella_overlay_back.png", ContentSource.ModFolder));
            umbrellaTextureBack.Add(this.Helper.Content.Load<Texture2D>("assets/red/umbrella_overlay_back.png", ContentSource.ModFolder));
            umbrellaTextureBack.Add(this.Helper.Content.Load<Texture2D>("assets/orange/umbrella_overlay_back.png", ContentSource.ModFolder));
            umbrellaTextureBack.Add(this.Helper.Content.Load<Texture2D>("assets/yellow/umbrella_overlay_back.png", ContentSource.ModFolder));
            umbrellaTextureBack.Add(this.Helper.Content.Load<Texture2D>("assets/green/umbrella_overlay_back.png", ContentSource.ModFolder));
            umbrellaTextureBack.Add(this.Helper.Content.Load<Texture2D>("assets/blue/umbrella_overlay_back.png", ContentSource.ModFolder));
            umbrellaTextureBack.Add(this.Helper.Content.Load<Texture2D>("assets/purple/umbrella_overlay_back.png", ContentSource.ModFolder));
            umbrellaTextureBack.Add(this.Helper.Content.Load<Texture2D>("assets/black/umbrella_overlay_back.png", ContentSource.ModFolder));
            //umbrellaTextureBack.Add(this.Helper.Content.Load<Texture2D>("assets/pink/umbrella_overlay_back.png", ContentSource.ModFolder));

            //add new umbrellas here
            umbrellaTextureSide.Add(this.Helper.Content.Load<Texture2D>("assets/tattered/umbrella_overlay_side.png", ContentSource.ModFolder));
            umbrellaTextureSide.Add(this.Helper.Content.Load<Texture2D>("assets/red/umbrella_overlay_side.png", ContentSource.ModFolder));
            umbrellaTextureSide.Add(this.Helper.Content.Load<Texture2D>("assets/orange/umbrella_overlay_side.png", ContentSource.ModFolder));
            umbrellaTextureSide.Add(this.Helper.Content.Load<Texture2D>("assets/yellow/umbrella_overlay_side.png", ContentSource.ModFolder));
            umbrellaTextureSide.Add(this.Helper.Content.Load<Texture2D>("assets/green/umbrella_overlay_side.png", ContentSource.ModFolder));
            umbrellaTextureSide.Add(this.Helper.Content.Load<Texture2D>("assets/blue/umbrella_overlay_side.png", ContentSource.ModFolder));
            umbrellaTextureSide.Add(this.Helper.Content.Load<Texture2D>("assets/purple/umbrella_overlay_side.png", ContentSource.ModFolder));
            umbrellaTextureSide.Add(this.Helper.Content.Load<Texture2D>("assets/black/umbrella_overlay_side.png", ContentSource.ModFolder));
            //umbrellaTextureBack.Add(this.Helper.Content.Load<Texture2D>("assets/pink/umbrella_overlay_back.png", ContentSource.ModFolder));

            //Events
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.Display.MenuChanged += this.onMenuChanged;

            harmony = HarmonyInstance.Create(ModManifest.UniqueID);

            //Harmony Patches
            harmony.Patch(
            original: AccessTools.Method(typeof(StardewValley.FarmerRenderer), nameof(StardewValley.FarmerRenderer.draw),
            new Type[] { typeof(SpriteBatch), typeof(FarmerSprite.AnimationFrame), typeof(int), typeof(Rectangle), typeof(Vector2), typeof(Vector2), typeof(float), typeof(int), typeof(Color), typeof(float), typeof(float), typeof(Farmer) }),
            postfix: new HarmonyMethod(typeof(FarmerRendererPatches), nameof(FarmerRendererPatches.draw_Postfix)));

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Menus.IClickableMenu), nameof(StardewValley.Menus.IClickableMenu.drawHoverText),
                new System.Type[] { typeof(SpriteBatch), typeof(StringBuilder), typeof(SpriteFont), typeof(int), typeof(int), typeof(int), typeof(string), typeof(int), typeof(string[]), typeof(Item), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(float), typeof(CraftingRecipe), typeof(IList<Item>) }),
                prefix: new HarmonyMethod(typeof(UmbrellaPatch), nameof(UmbrellaPatch.drawHoverTextPrefix)),
                postfix: new HarmonyMethod(typeof(UmbrellaPatch), nameof(UmbrellaPatch.drawHoverTextPostfix))
            );
        }

        private void onMenuChanged(object sender, MenuChangedEventArgs e) // Handle when the player changes their appearance at the shrine of illusions
        {
            if (changingAppearance)
            {
                changingAppearance = false;
                OnSaveLoaded(null, null);
                lastItem = null;
            }
            if (Game1.activeClickableMenu is StardewValley.Menus.CharacterCustomization)
            {
                changingAppearance = true;
                drawUmbrella = false;
                drawRegularFarmer = true;
                fullRedraw = true;
                redrawFarmer();
            }

            if (e.NewMenu is ShopMenu menu && menu != null)
            {
                // Add umbrellas to the hat mouse shop
                if (menu.potraitPersonDialogue == Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11494"), Game1.dialogueFont, Game1.tileSize * 5 - Game1.pixelZoom * 4))
                {
                    foreach (string umbrellaName in umbrellaNames)
                    {
                        if (umbrellaName != "Tattered Umbrella")
                        {
                            var o = new MeleeWeapon(spriteIndex: JsonAssets.GetWeaponId(name: umbrellaName));
                            menu.itemPriceAndStock.Add(o, new[] { (int)1000, int.MaxValue });
                            menu.forSale.Insert(menu.forSale.Count, o);
                        }
                    }
                }
            }

            if (e.OldMenu is LetterViewerMenu letterClosed && letterClosed.isMail && e.NewMenu == null)
            {
                if (letterClosed.mailTitle == "lewis_umbrella") //thanks bbblueberry for the below snippet
                {
                    Game1.player.completelyStopAnimatingOrDoingAction();
                    DelayedAction.playSoundAfterDelay("getNewSpecialItem", 750);

                    Game1.player.faceDirection(2);
                    Game1.player.freezePause = 4000;
                    Game1.player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[3]
                    {
                        new FarmerSprite.AnimationFrame(57, 0),
                        new FarmerSprite.AnimationFrame(57, 2500, secondaryArm: false, flip: false, Farmer.showHoldingItem),
                        new FarmerSprite.AnimationFrame((short)Game1.player.FarmerSprite.CurrentFrame, 500, secondaryArm: false, flip: false)
                    });
                    Game1.player.mostRecentlyGrabbedItem = new MeleeWeapon(spriteIndex: JsonAssets.GetWeaponId(name: "Tattered Umbrella"));
                    Game1.player.canMove = false;

                    AddItem(new MeleeWeapon(spriteIndex: JsonAssets.GetWeaponId(name: "Tattered Umbrella")));
                }
            }
        }

        public static void AddItem(Item item)
        {
            if (Game1.player.couldInventoryAcceptThisItem(item))
                Game1.player.addItemToInventory(item);
            else
                Game1.player.addItemByMenuIfNecessaryElseHoldUp(item);
        }


        public void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            try
            {
                JsonAssets = Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
                this.Helper.Content.InvalidateCache("Data/mail");
            }
            catch (Exception ex)
            {
                Monitor.Log("Error loading JSON assets", LogLevel.Warn);
            }

            try
            {
                GenericModConfigMenuAPI api = this.Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

                if (api == null)
                    return;

                api.RegisterModConfig(ModManifest, () => Config = new UmbrellaConfig(), () => Helper.WriteConfig(Config));

                api.RegisterSimpleOption
                (
                    ModManifest,
                    "Enable wetness",
                    "",
                    () => Config.enableWetness,
                    (bool val) => Config.enableWetness = val
                );

                api.RegisterParagraph(ModManifest, "If this box is checked, the farmer will get wet when standing outside in the rain without an umbrella. When the farmer is wet, stamina will be slowly drained. More information about the wetness system can be found on the mod page.");

                api.RegisterSimpleOption(
                    ModManifest,
                    "Stamina drain rate",
                    "",
                    () => Config.staminaDrainRate,
                    (float val) => Config.staminaDrainRate = val
                );

                api.RegisterParagraph(ModManifest, "How much stamina to drain every 10 minutes (in game time) when the farmer is wet.");
            }
            catch (Exception ex)
            {
                Monitor.Log($"An error happened while loading this mod's GMCM options menu. Its menu might be missing or fail to work. The auto-generated error message has been added to the log.", LogLevel.Warn);
                Monitor.Log($"----------", LogLevel.Trace);
                Monitor.Log($"{ex.ToString()}", LogLevel.Trace);
            }
        }

        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e) //Prevent weird graphical bugs when the player returns to title screen with umbrella equipped
        {
            drawUmbrella = false;
            drawRegularFarmer = true;
            fullRedraw = true;
            redrawFarmer();
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Characters/Farmer/farmer_base") || asset.AssetNameEquals("Characters/Farmer/farmer_base_bald")
                || asset.AssetNameEquals("Characters/Farmer/farmer_girl_base") || asset.AssetNameEquals("Characters/Farmer/farmer_girl_base_bald")
                || asset.AssetNameEquals("TileSheets/BuffsIcons") || asset.AssetNameEquals("Data/hats") || asset.AssetNameEquals("Strings/StringsFromCSFiles")
                || asset.AssetNameEquals("Data/mail");
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Strings/StringsFromCSFiles"))
            {
                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                data["ShopMenu.cs.11494"] = "Hiyo, poke. Did you bring coins? Gud. Me sell hats and umbrellas.";
            }
            if (asset.AssetNameEquals("Data/mail"))
            {
                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                
                data["hatter"] = "Hi.^^Me sell hats and umbrellas. Okay, poke?^^Come to old old old haus, poke. Bring coines.^^-hat mouse[#]Hiyo, Poke";

                if (JsonAssets != null)
                {
                    data["lewis_umbrella"] = "@,^I found this old umbrella lying around, I figure it could be useful to you on rainy days.  ^   Sincerely, Mr. Lewis";
                }
            }

            if (asset.AssetNameEquals("TileSheets/BuffsIcons"))
            {
                var editor = asset.AsImage();

                Texture2D sourceImage = this.Helper.Content.Load<Texture2D>("assets/WetBuff.png", ContentSource.ModFolder);
                editor.PatchImage(sourceImage, targetArea: new Rectangle(176, 32, 16, 16));
            }
            /*if (Config.enableWetness)
            {
                if (asset.AssetNameEquals("Data/hats"))
                {
                    IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;

                    // 90% rain protection
                    data[3] = "Sombrero/A festively decorated hat made from woven straw.\n\nProvides major rain protection./false/true";
                    data[40] = "Living Hat/It absorbs moisture from your scalp. No watering needed!\n\nProvides major rain protection./false/true";
                    data[42] = "Mushroom Cap/It smells earthy.\n\nProvides major rain protection./false/true";

                    // 50% rain protection
                    data[66] = "Garbage Hat/It's a garbage can lid 'upcycled' into a hat...\n\nProvides moderate rain protection./false/true";
                    data[78] = "Frog Hat/A slimy friend that lives on your dome.\n\nProvides moderate rain protection./hide/true";
                    data[17] = "Sailor's Cap/It's fresh and starchy.\n\nProvides moderate rain protection./false/true";
                    data[4] = "Straw Hat/Light and cool, it's a farmer's delight.\n\nProvides moderate rain protection./false/true";
                    data[28] = "Sou'wester/The shape helps to keep sailors dry during storms.\n\nProvides moderate rain protection./false/true";
                    data[55] = "Fishing Hat/The wide brim keeps you shaded when you're fishing on the riverbank.\n\nProvides moderate rain protection./false/true";
                    data[71] = "Copper Pan/You place the copper pan on your head...\n\nProvides moderate rain protection./false/true";
                    data[27] = "Hard Hat/Keep your dome in one piece.\n\nProvides moderate rain protection./false/true";
                    data[75] = "Golden Helmet/It's half of a golden coconut.\n\nProvides moderate rain protection./false/true";
                }
            }*/
            if (!fullRedraw)
            {
                if (asset.AssetNameEquals("Characters/Farmer/farmer_girl_base"))
                {
                    if (drawUmbrella)
                    {
                        asset.AsImage().PatchImage(umbrellaTexture);
                    }
                    if (drawRegularFarmer)
                    {
                        asset.AsImage().PatchImage(regularFarmerTexture);
                    }
                }
                if (asset.AssetNameEquals("Characters/Farmer/farmer_base"))
                {
                    if (drawUmbrella)
                    {
                        asset.AsImage().PatchImage(umbrellaTexture);
                    }
                    if (drawRegularFarmer)
                    {
                        asset.AsImage().PatchImage(regularFarmerTexture);
                    }
                }
                if (asset.AssetNameEquals("Characters/Farmer/farmer_base_bald"))
                {
                    if (drawUmbrella)
                    {
                        asset.AsImage().PatchImage(umbrellaTexture);
                    }
                    if (drawRegularFarmer)
                    {
                        asset.AsImage().PatchImage(regularFarmerTexture);
                    }
                }
                if (asset.AssetNameEquals("Characters/Farmer/farmer_girl_base_bald"))
                {
                    if (drawUmbrella)
                    {
                        asset.AsImage().PatchImage(umbrellaTexture);
                    }
                    if (drawRegularFarmer)
                    {
                        asset.AsImage().PatchImage(regularFarmerTexture);
                    }
                }
            } else
            {
                if (asset.AssetNameEquals("Characters/Farmer/farmer_girl_base"))
                {
                    asset.AsImage().PatchImage(this.Helper.Content.Load<Texture2D>("assets/farmer_base_girl.png", ContentSource.ModFolder));
                }
                if (asset.AssetNameEquals("Characters/Farmer/farmer_base"))
                {
                    asset.AsImage().PatchImage(this.Helper.Content.Load<Texture2D>("assets/farmer_base_boy.png", ContentSource.ModFolder));
                }
                if (asset.AssetNameEquals("Characters/Farmer/farmer_base_bald"))
                {
                    asset.AsImage().PatchImage(this.Helper.Content.Load<Texture2D>("assets/farmer_base_boy_bald.png", ContentSource.ModFolder));
                }
                if (asset.AssetNameEquals("Characters/Farmer/farmer_girl_base_bald"))
                {
                    asset.AsImage().PatchImage(this.Helper.Content.Load<Texture2D>("assets/farmer_base_girl_bald.png", ContentSource.ModFolder));
                }
            }
            
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            /* future Climates of Ferngill Integration
            if (Helper.ModRegistry.IsLoaded(cofModID))
            {
                int currentRain = Helper.Reflection.GetMethod(Helper.ModRegistry.GetApi(cofModID), "GetCurrentRain").Invoke<int>();
                Monitor.Log(currentRain.ToString(), LogLevel.Debug);
            }
            */

            fullRedraw = false;
            isMaleFarmer = Game1.player.isMale.Value;
            isBaldFarmer = Game1.player.IsBaldHairStyle(Game1.player.getHair());
            customBestHats.AddRange(Config.bestRainHats.Split(',').ToList());
            customGoodHats.AddRange(Config.goodRainHats.Split(',').ToList());

            try
            {
                Config = Helper.ReadConfig<UmbrellaConfig>();
            }
            catch (Exception ex)
            {
                Monitor.Log($"Encountered an error while loading the config.json file. Default settings will be used instead. Full error message:\n-----\n{ex.ToString()}", LogLevel.Error);
                Config = new UmbrellaConfig();
            }

            staminaDrainRatePerTick = (float)(Config.staminaDrainRate / 420);
            this.Helper.Content.InvalidateCache("Data/hats");

            loadUmbrellaTextures();

            if (Config.enableWetness & !Game1.player.mailReceived.Contains("lewis_umbrella"))
            {
                Game1.addMailForTomorrow("lewis_umbrella");
            }
        }

        public void loadUmbrellaTextures()
        {
            umbrellaPlayerTextures.Clear();

            //add new umbrellas here

            if (isMaleFarmer)
            {
                if (isBaldFarmer)
                {
                    regularFarmerTexture = this.Helper.Content.Load<Texture2D>("assets/farmer_base_boy_bald.png", ContentSource.ModFolder);

                    umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/tattered/farmer_base_boy_bald.png", ContentSource.ModFolder));
                    umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/red/farmer_base_boy_bald.png", ContentSource.ModFolder));
                    umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/orange/farmer_base_boy_bald.png", ContentSource.ModFolder));
                    umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/yellow/farmer_base_boy_bald.png", ContentSource.ModFolder));
                    umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/green/farmer_base_boy_bald.png", ContentSource.ModFolder));
                    umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/blue/farmer_base_boy_bald.png", ContentSource.ModFolder));
                    umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/purple/farmer_base_boy_bald.png", ContentSource.ModFolder));
                    umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/black/farmer_base_boy_bald.png", ContentSource.ModFolder));
                    //umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/pink/farmer_base_boy_bald.png", ContentSource.ModFolder));
                }
                else
                {
                    regularFarmerTexture = this.Helper.Content.Load<Texture2D>("assets/farmer_base_boy.png", ContentSource.ModFolder);

                    umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/tattered/farmer_base_boy.png", ContentSource.ModFolder));
                    umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/red/farmer_base_boy.png", ContentSource.ModFolder));
                    umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/orange/farmer_base_boy.png", ContentSource.ModFolder));
                    umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/yellow/farmer_base_boy.png", ContentSource.ModFolder));
                    umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/green/farmer_base_boy.png", ContentSource.ModFolder));
                    umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/blue/farmer_base_boy.png", ContentSource.ModFolder));
                    umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/purple/farmer_base_boy.png", ContentSource.ModFolder));
                    umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/black/farmer_base_boy.png", ContentSource.ModFolder));
                    //umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/pink/farmer_base_boy.png", ContentSource.ModFolder));
                }
            }
            else
            {
                if (isBaldFarmer)
                {
                    regularFarmerTexture = this.Helper.Content.Load<Texture2D>("assets/farmer_base_girl_bald.png", ContentSource.ModFolder);

                    umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/tattered/farmer_base_girl_bald.png", ContentSource.ModFolder));
                    umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/red/farmer_base_girl_bald.png", ContentSource.ModFolder));
                    umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/orange/farmer_base_girl_bald.png", ContentSource.ModFolder));
                    umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/yellow/farmer_base_girl_bald.png", ContentSource.ModFolder));
                    umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/green/farmer_base_girl_bald.png", ContentSource.ModFolder));
                    umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/blue/farmer_base_girl_bald.png", ContentSource.ModFolder));
                    umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/purple/farmer_base_girl_bald.png", ContentSource.ModFolder));
                    umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/black/farmer_base_girl_bald.png", ContentSource.ModFolder));
                    //umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/pink/farmer_base_girl_bald.png", ContentSource.ModFolder));
                }
                else
                {
                    regularFarmerTexture = this.Helper.Content.Load<Texture2D>("assets/farmer_base_girl.png", ContentSource.ModFolder);

                    umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/tattered/farmer_base_girl.png", ContentSource.ModFolder));
                    umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/red/farmer_base_girl.png", ContentSource.ModFolder));
                    umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/orange/farmer_base_girl.png", ContentSource.ModFolder));
                    umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/yellow/farmer_base_girl.png", ContentSource.ModFolder));
                    umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/green/farmer_base_girl.png", ContentSource.ModFolder));
                    umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/blue/farmer_base_girl.png", ContentSource.ModFolder));
                    umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/purple/farmer_base_girl.png", ContentSource.ModFolder));
                    umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/black/farmer_base_girl.png", ContentSource.ModFolder));
                    //umbrellaPlayerTextures.Add(this.Helper.Content.Load<Texture2D>("assets/pink/farmer_base_girl.png", ContentSource.ModFolder));
                }
            }
        }

        public void redrawFarmer()
        {
            if (isMaleFarmer) //Redraw the farmer
            {
                if (isBaldFarmer)
                {
                    this.Helper.Content.InvalidateCache("Characters/Farmer/farmer_base_bald");
                }
                else
                {
                    this.Helper.Content.InvalidateCache("Characters/Farmer/farmer_base");
                }
            }
            else
            {
                if (isBaldFarmer)
                {
                    this.Helper.Content.InvalidateCache("Characters/Farmer/farmer_girl_base_bald");
                }
                else
                {
                    this.Helper.Content.InvalidateCache("Characters/Farmer/farmer_girl_base");
                }
            }
        }

        private void addWetBuff()
        {
            Buff wetnessBuff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == wetBuffIndex);

            Game1.buffsDisplay.addOtherBuff(wetnessBuff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "", ""));
            wetnessBuff.which = wetBuffIndex;
            wetnessBuff.sheetIndex = wetBuffIndex;
            wetnessBuff.millisecondsDuration = 10000;
            wetnessBuff.description = "You are drenched!\n\nDrains stamina.";
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            // Handle drawing umbrella
            if (lastItem != Game1.player.CurrentItem) // Selected item has changed
            {
                if (Game1.player.CurrentItem is object)
                {
                    if (umbrellaNames.Contains(Game1.player.CurrentItem.Name))
                    {
                        drawUmbrella = true;
                        drawRegularFarmer = false;
                        justDrewFarmer = false;
                        umbrellaTexture = umbrellaPlayerTextures[umbrellaNames.IndexOf(Game1.player.CurrentItem.Name)];
                        umbrellaOverlayTextureBack = umbrellaTextureBack[umbrellaNames.IndexOf(Game1.player.CurrentItem.Name)];
                        umbrellaOverlayTextureSide = umbrellaTextureSide[umbrellaNames.IndexOf(Game1.player.CurrentItem.Name)];
                        redrawFarmer();
                    }
                    else if (!justDrewFarmer)
                    {
                        drawRegularFarmer = true;
                        drawUmbrella = false;
                        redrawFarmer();
                        justDrewFarmer = true;
                    }
                }
                else if (!justDrewFarmer)
                {
                    drawRegularFarmer = true;
                    drawUmbrella = false;
                    redrawFarmer();
                    justDrewFarmer = true;
                }
            }

            // Handle wetness buff
            if (Game1.currentLocation.IsOutdoors & Game1.IsRainingHere(Game1.player.currentLocation) & Game1.player.currentLocation.Name != "Desert")
            {
                if (Game1.player.CurrentItem is object)
                {
                    if (!umbrellaNames.Contains(Game1.player.CurrentItem.Name))
                    {
                        ticksInRain += 1;
                        if (Game1.buffsDisplay.otherBuffs.FindIndex(p => p.which == wetBuffIndex) != -1)
                        {
                            Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == wetBuffIndex).millisecondsDuration = 10000;
                        }
                    } else
                    {
                        ticksInRain = 0;
                    }
                }
                else
                {
                    ticksInRain += 1;
                    if (Game1.buffsDisplay.otherBuffs.FindIndex(p => p.which == wetBuffIndex) != -1)
                    {
                        Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == wetBuffIndex).millisecondsDuration = 10000;
                    }
                }
                if (ticksInRain > 120 & Game1.buffsDisplay.otherBuffs.FindIndex(p => p.which == wetBuffIndex) == -1 & Config.enableWetness)
                {
                    addWetBuff();
                }
            } else
            {
                ticksInRain = 0;
            }

            // Handle stamina drain
            if (Game1.buffsDisplay.otherBuffs.FindIndex(p => p.which == wetBuffIndex) != -1 & Context.IsPlayerFree)
            {
                if (Game1.player.Stamina - staminaDrainRatePerTick > 1)
                {
                    staminaDrainShield = 0f;
                    if (Game1.player.hat.Value is object)
                    {
                        if (customBestHats.Contains(Game1.player.hat.Value.Name))
                        {
                            staminaDrainShield += 0.9f;
                        } else if (customGoodHats.Contains(Game1.player.hat.Value.Name))
                        {
                            staminaDrainShield += 0.5f;
                        }
                        else if (bestHats.Contains(Game1.player.hat.Value.which.Value))
                        {
                            staminaDrainShield += 0.9f;
                        }
                        else if (goodHats.Contains(Game1.player.hat.Value.which.Value))
                        {
                            staminaDrainShield += 0.5f;
                        }
                    }
                    if (Game1.player.shirtItem.Value is object)
                    {
                        if (Game1.player.shirtItem.Value.Name == "Rain Coat")
                        {
                            staminaDrainShield += 0.2f;
                        }
                    }
                    if (staminaDrainShield > 1)
                    {
                        staminaDrainShield = 1;
                    }
                    Game1.player.Stamina -= staminaDrainRatePerTick * (1-staminaDrainShield);
                }
            }

            lastItem = Game1.player.CurrentItem;
        }
    }

    public interface GenericModConfigMenuAPI
    {
        void RegisterModConfig(IManifest mod, Action revertToDefault, Action saveToFile);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<bool> optionGet, Action<bool> optionSet);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<float> optionGet, Action<float> optionSet);
        void RegisterParagraph(IManifest mod, string paragraph);
    }
}