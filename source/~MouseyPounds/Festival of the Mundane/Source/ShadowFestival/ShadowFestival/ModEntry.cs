using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xTile;

namespace ShadowFestival
{
    public interface IJsonAssetsApi
    {
        int GetHatId(string name);
        void LoadAssets(string path);
    }

    public class ModEntry: Mod, IAssetLoader, IAssetEditor
    {
        internal static ModEntry Instance { get; private set; }
        internal static ModData Data;
        internal static Random Random = new Random();
        private static Dictionary<ISalable, int[]> VendorItems = new Dictionary<ISalable, int[]>();
        private static readonly List<Vector2> VendorTiles = new List<Vector2> { new Vector2(8, 19) };

        protected bool _gettingKickedOut = false;
        protected bool _mapChanged = false;

		protected Dictionary<string, int> _currentDialogueIndex = new Dictionary<string, int>();

		// Contains all of the light sources for those weird pulsing lights.
		protected List<LightSource> _weirdLights = new List<LightSource>();

		protected Texture2D _goblinNoseTexture;

		// Set to true if the mod has subscribed to the tick update. It then kind of blindly unsubscribes any time the player
		// warps again so that we're not running this sewer specific code when the sewer's not active. Probably not the best way to do this!
		protected bool _registeredUpdate = false;

		public static System.Action<Vector2> setGoblinNosePosition;

		protected GameLocation _goblinNoseSpriteLocation;
		protected TemporaryAnimatedSprite _goblinNoseSprite;

        public override void Entry(IModHelper helper)
        {
            // Setup fields
            Instance = this;
            Data = helper.Data.ReadJsonFile<ModData>("data.json");
            if (Data == null)
            {
                Data = new ModData();
                helper.Data.WriteJsonFile("data.json", Data);
            }
            _mapChanged = false;

			_goblinNoseTexture = helper.Content.Load<Texture2D>("assets/Goblin_Nose.png", ContentSource.ModFolder);

			// Setup Harmony patches
			HarmonyInstance harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
            HarmonyPatcher.Hook(harmony, Monitor);

            // Setup event hooks
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            helper.Events.Display.MenuChanged += Display_MenuChanged;
            helper.Events.Player.Warped += Player_Warped;
        }

		public void SetGoblinNosePosition(Vector2 position)
		{
			// Removes the existing Goblin nose.
			_RemoveGoblinNose();

			// Adds a temporary sprite of the goblin nose at the position the player placed it.
			if (Game1.currentLocation.Name.Equals("Sewer"))
			{
				_goblinNoseSpriteLocation = Game1.currentLocation;

				position.Y = 19 * Game1.tileSize;

				_goblinNoseSprite = new TemporaryAnimatedSprite();
				_goblinNoseSprite.texture = _goblinNoseTexture;
				_goblinNoseSprite.animationLength = 1;
				_goblinNoseSprite.position = position;
				_goblinNoseSprite.sourceRect = new Rectangle(0, 0, 16, 16);
				_goblinNoseSprite.sourceRectStartingPos = Vector2.Zero;
				_goblinNoseSprite.interval = 9999.0F;
				_goblinNoseSprite.totalNumberOfLoops = 9999;
				_goblinNoseSprite.scale = Game1.pixelZoom;
				_goblinNoseSpriteLocation.TemporarySprites.Add(_goblinNoseSprite);
			}
		}

		protected void _RemoveGoblinNose()
		{
			if (_goblinNoseSprite != null)
			{
				_goblinNoseSpriteLocation.TemporarySprites.Remove(_goblinNoseSprite);
				_goblinNoseSprite = null;
				_goblinNoseSpriteLocation = null;
			}
		}

        private void Player_Warped(object sender, StardewModdingAPI.Events.WarpedEventArgs e)
        {
			_RemoveGoblinNose();
            _gettingKickedOut = false;

			// If we registered the weird light update, just remove it if we warp.
			if (e.IsLocalPlayer && _registeredUpdate)
			{
				Helper.Events.GameLoop.UpdateTicking -= UpdateWeirdLights;
				setGoblinNosePosition = null;
				_registeredUpdate = false;
			}

			if (e.IsLocalPlayer &&
                e.NewLocation != null &&
                e.NewLocation.Name.Equals("Sewer") &&
                IsShadowFestivalToday()
                )
            {
				setGoblinNosePosition += SetGoblinNosePosition;
				// Setup the Sewer Map for us
				if (e.Player.mailReceived.Contains("ShadowFestivalVisited"))
                {
                    e.Player.mailReceived.Add("ShadowFestivalVisited");
                }
                Helper.Reflection.GetField<Color>(e.NewLocation, "steamColor").SetValue(new Color(240, 180, 240));
                // Repatch the two tiles that Sewers.resetLocalState() changes on us.
                // Beware if that part of our map is edited, we need to manually change these here too.
                e.NewLocation.setMapTileIndex(31, 16, 77, "Front", 1);
                e.NewLocation.setMapTileIndex(31, 17, 85, "Buildings", 1);
                e.NewLocation.setTileProperty(31, 17, "Buildings", "Action", "FestivalDialogue BigShadow");
                // Check for Krobus and make him scarce
                if (e.NewLocation.characters != null)
                {
                    foreach (NPC npc in e.NewLocation.characters)
                    {
                        if (npc.Name == "Krobus")
                        {
                            npc.setTilePosition(3, 20);
                            break;
                        }
                    }
                }
                // Let's add critters. Start with a frog along the entry hall
                List<Critter> critters = Helper.Reflection.GetField<List<Critter>>(e.NewLocation, "critters").GetValue();
                if (critters == null)
                {
                    Helper.Reflection.GetField<List<Critter>>(e.NewLocation, "critters").SetValue(new List<Critter>());
                }
                e.NewLocation.addCritter(new Frog(new Vector2(4, ModEntry.Random.Next(26, 31)), true, false));
                e.NewLocation.addCritter(new Frog(new Vector2(15, 26), true, true));
                e.NewLocation.addCritter(new Frog(new Vector2(18, 27), true, false));

                // Add a bunch of fireflies.
                for (int i = 0; i < 60; i++)
				{
					Firefly firefly = new Firefly(new Vector2(ModEntry.Random.Next(3, 34), ModEntry.Random.Next(10, 44)));

					// These fireflies have weird colors.

					var light_source = Helper.Reflection.GetField<LightSource>(firefly, "light");

					int color = ModEntry.Random.Next(0, 3);

					if (color == 0)
					{
						light_source.GetValue().color.Value = new Color(0, 0, 255);
					}
					else if (color == 1)
					{
						light_source.GetValue().color.Value = new Color(0, 255, 0);
					}
					else if (color == 2)
					{
						light_source.GetValue().color.Value = new Color(255, 0, 0);
					}

					light_source.GetValue().radius.Value = 0.75F;

					e.NewLocation.addCritter(firefly);
				}

				_weirdLights.Clear();

				// Set up the lights in the scene.

				for (int x = 0; x < e.NewLocation.Map.Layers[0].LayerWidth; x++)
				{
					for (int y = 0; y < e.NewLocation.Map.Layers[0].LayerHeight; y++)
					{
						xTile.Tiles.Tile tile = e.NewLocation.Map.GetLayer("Front").Tiles[x, y];
						xTile.Tiles.Tile tile_2 = e.NewLocation.Map.GetLayer("AlwaysFront").Tiles[x, y];

						// Weird lamp things.

						if ((tile != null && tile.TileIndex == 122 && tile.TileSheet.Id == "Spirit_Sewer") ||
							(tile_2 != null && tile_2.TileIndex == 122 && tile_2.TileSheet.Id == "Spirit_Sewer"))
						{
							AddWeirdLight(e.NewLocation, x, y);
						}

						// Weird hanging lights (Left)

						if ((tile != null && tile.TileIndex == 65 && tile.TileSheet.Id == "Spirit_Sewer") ||
							(tile_2 != null && tile_2.TileIndex == 65 && tile_2.TileSheet.Id == "Spirit_Sewer"))
						{
							Game1.currentLightSources.Add(new LightSource(1, new Vector2((x + 0.5F) * Game1.tileSize, (y + 0.5F) * Game1.tileSize), 0.25F, new Color(255, 255, 255)));
						}

						// Middle

						if ((tile != null && tile.TileIndex == 66 && tile.TileSheet.Id == "Spirit_Sewer") ||
							(tile_2 != null && tile_2.TileIndex == 66 && tile_2.TileSheet.Id == "Spirit_Sewer"))
						{
							Game1.currentLightSources.Add(new LightSource(6, new Vector2((x + 0.5F) * Game1.tileSize, (y + 0.75F) * Game1.tileSize), 0.5F, new Color(255, 10, 255)));
						}

						// Right

						if ((tile != null && tile.TileIndex == 67 && tile.TileSheet.Id == "Spirit_Sewer") ||
							(tile_2 != null && tile_2.TileIndex == 67 && tile_2.TileSheet.Id == "Spirit_Sewer"))
						{
							Game1.currentLightSources.Add(new LightSource(1, new Vector2((x + 0.5F) * Game1.tileSize, (y + 0.5F) * Game1.tileSize), 0.25F, new Color(255, 10, 10)));
						}
					}
				}

				Game1.changeMusicTrack("WizardSong");

				// The Sewers class overrides the lighting, so override it again.
				Game1.ambientLight = new Color(190, 190, 150);

				// Register the weird light update. This causes those weird lights to kind of pulse.
				Helper.Events.GameLoop.UpdateTicking += UpdateWeirdLights;
				_registeredUpdate = true;
			}
        }

		public virtual void UpdateWeirdLights(object sender, StardewModdingAPI.Events.UpdateTickingEventArgs e)
		{ 
			foreach (LightSource light in _weirdLights)
			{
				float time = (float)Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 1000.0F;

				float color_position = ((float)Math.Sin(time * 0.3F + light.position.X * 1.15F + light.position.Y * 0.33F) + 1) / 2.0F;

				Color light_color = new Color(0, 0, 0);

				if (color_position < 0.33F)
				{
					float lerp_position = color_position / 0.33F;

					lerp_position = MathHelper.Clamp(lerp_position, 0, 1);

					light_color.R = (byte)(255 * MathHelper.Lerp(1.0F, 0, lerp_position));
					light_color.G = (byte)(255 * MathHelper.Lerp(0, 1.0F, lerp_position));
				}
				else if (color_position < 0.66F)
				{
					color_position -= .33F;

					float lerp_position = color_position / 0.33F;

					lerp_position = MathHelper.Clamp(lerp_position, 0, 1);

					light_color.G = (byte)(255 * MathHelper.Lerp(1.0F, 0, lerp_position));
					light_color.B = (byte)(255 * MathHelper.Lerp(0, 1.0F, lerp_position));
				}
				else
				{
					color_position -= .66F;

					float lerp_position = color_position / 0.33F;

					lerp_position = MathHelper.Clamp(lerp_position, 0, 1);

					light_color.B = (byte)(255 * MathHelper.Lerp(1.0F, 0, lerp_position));
					light_color.R = (byte)(255 * MathHelper.Lerp(0.0F, 1.0F, lerp_position));
				}


				// TODO: Adjust these light color values.
				light.color.Value = light_color;

				light.radius.Value = 0.3F + ((float)(Math.Sin(time + light.position.X + light.position.Y) + 1) / 2.0F) * 0.3F;
			}
		}

		public void AddWeirdLight(GameLocation location, int x, int y)
		{
			LightSource weird_light = new LightSource(LightSource.sconceLight, new Vector2((x + 0.5F) * Game1.tileSize, (y + 0.5F) * Game1.tileSize), 1, new Color(255, 10, 10));

			_weirdLights.Add(weird_light);
			Game1.currentLightSources.Add(weird_light);
		}

        private void Display_MenuChanged(object sender, StardewModdingAPI.Events.MenuChangedEventArgs e)
        {
            // This hook is to remove our custom hats from the regular Hatmouse shop before the player has visited the festival
            // It is not terribly easy to identify the shop as belonging to Hatmouse so we will remove them from any shop in the Forest
            // To make hats available after visitng festival, could add condition: !Game1.player.mailReceived.Contains("ShadowFestivalVisited"
            if (e.NewMenu != null &&
                e.NewMenu is ShopMenu shop &&
                Game1.currentLocation.Name.Equals("Forest")
                )
            {
                Dictionary<ISalable, int[]> shopStock = Helper.Reflection.GetField<Dictionary<ISalable, int[]>>(shop, "itemPriceAndStock").GetValue();
                List<ISalable> shopForSale = Helper.Reflection.GetField<List<ISalable>>(shop, "forSale").GetValue();
                foreach (ISalable item in shopStock.Keys.ToArray())
                {
                    //Monitor.VerboseLog($"Checking item {item.Name}");
                    if (item is Hat hat && (Data.CalmingHats.Contains(hat.Name) || Data.OtherHats.Contains(hat.Name)))
                    {
                        Monitor.Log($"Removing {hat.Name} from forest shop.");
                        shopStock.Remove(item);
                        shopForSale.Remove(item);
                    }
                }
            }
        }

        private void GameLoop_DayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
			_currentDialogueIndex.Clear();
            PinGame.claimedPrizes.Clear();

			if (IsShadowFestivalToday())
            {
                Monitor.Log("It's Shadow Festival Day!");
                IJsonAssetsApi api = Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
                if (api == null)
                {
                    Monitor.Log("Can't load JSON Assets API, so custom hats will not be available", LogLevel.Warn);
                    return;
                }
                else
                {
                    // Setting up special vendor. First, 3 vanilla hats.
                    VendorItems.Clear();
                    VendorItems.Add(new Hat(37), new int[] { 5000, 2147483647 });
                    VendorItems.Add(new Hat(38), new int[] { 5000, 2147483647 });
                    VendorItems.Add(new Hat(36), new int[] { 10000, 2147483647 });
                    // Now for JA hats, whose names are all hardcoded.
                    int HatId;
                    Monitor.Log("Adding calming hats to Vendor stock");
                    foreach (string hatName in Data.CalmingHats)
                    {
                        HatId = api.GetHatId(hatName);
                        if (HatId != -1)
                        {
                            VendorItems.Add(new Hat(HatId), new int[] { 750, 2147483647 });
                        }
                        else
                        {
                            Monitor.Log($"Error adding {hatName} to Vendor stock", LogLevel.Warn);
                        }
                    }
                    Monitor.Log("Adding other hats to Vendor stock");
                    foreach (string hatName in Data.OtherHats)
                    {
                        HatId = api.GetHatId(hatName);
                        if (HatId != -1)
                        {
                            VendorItems.Add(new Hat(HatId), new int[] { 500, 2147483647 });
                        }
                        else
                        {
                            Monitor.Log($"Error adding {hatName} to Vendor stock", LogLevel.Warn);
                        }
                    }
                }

                // Make sure our asset changes happened
                Helper.Content.InvalidateCache("Data/mail");
                Helper.Content.InvalidateCache("Characters/Dialogue/Krobus");
                Helper.Content.InvalidateCache("Strings/StringsFromCSFiles");
                Helper.Content.InvalidateCache("Maps/Sewer");
            }
            else
            {
                // Trigger another invalidation to remove our changes
                if (_mapChanged)
                {
                    Helper.Content.InvalidateCache("Maps/Sewer");
                    Helper.Content.InvalidateCache("Characters/Dialogue/Krobus");
                    Helper.Content.InvalidateCache("Strings/StringsFromCSFiles");
                    _mapChanged = false;
                }
            }

            if (IsShadowFestivalTomorrow())
            {
                Monitor.Log("It is the day before the shadow festival.");
                if (!Game1.player.mailReceived.Contains("Wizard_ShadowFestival"))
                {
                    Monitor.Log("Player does not have our mail. Queueing for tomorrow.");
                    Game1.player.mailForTomorrow.Add("Wizard_ShadowFestival");
                }
                else
                {
                    Monitor.Log("Player already has our mail.");
                }
            }

            
        }

        private void Input_ButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
			if (Game1.player.freezePause > 0 || _gettingKickedOut) // Ignore these interactions when the farmer is frozen. This also happens when the farmer is getting kicked out.
			{
				return;
			}

            if (Context.IsWorldReady &&
                Game1.currentLocation != null &&
                Game1.currentLocation.Name.Equals("Sewer") &&
                Game1.activeClickableMenu == null &&
                e.Button.IsActionButton()
                )
            {
                //Monitor.VerboseLog($"Right-click intercepted on the Festival map at {e.Cursor.GrabTile.X}, {e.Cursor.GrabTile.Y}");
                if (VendorTiles.Contains(e.Cursor.GrabTile))
                {
                    Helper.Input.Suppress(e.Button);
                    //Monitor.VerboseLog("It was a boat click, showing vendor menu");
                    ShopMenu shop = new ShopMenu(VendorItems, 0, "Hatmouse");
                    string dialogueKey = $"shop-menu.message{ModEntry.Random.Next(3)}";
                    shop.potraitPersonDialogue = Game1.parseText(Helper.Translation.Get(dialogueKey), Game1.dialogueFont, 304);
                    Game1.activeClickableMenu = shop;
                }
                else
                {
					string action_property = Game1.currentLocation.doesTileHaveProperty((int)e.Cursor.GrabTile.X, (int)e.Cursor.GrabTile.Y, "Action", "Buildings");
					if (action_property != null)
					{
						string[] split = action_property.Split(' ');
                        //Monitor.VerboseLog($"Clicked on a tile with an action property: {action_property}");

						if (split.Length >= 2 && split[0] == "FestivalDialogue")
						{
                            Helper.Input.Suppress(e.Button);
                            if (!split[1].StartsWith("BigShadow") && !split[1].StartsWith("Snack") && !split[1].StartsWith("Festival_AncientDoll") &&
                                (Game1.player.hat.Value == null || !Data.CalmingHats.Contains(Game1.player.hat.Value.Name)))
							{
                                _gettingKickedOut = true;
                                Game1.playSound("shadowpeep");
								DelayedAction.playSoundAfterDelay("clubSmash", 1200);
								Game1.globalFadeToBlack(new Game1.afterFadeFunction(AfterFade), .045f);
								Game1.player.CanMove = false;
								Game1.player.freezePause = 1000;

							}
                            else
							{
								string base_key = split[1];

								if (!_currentDialogueIndex.ContainsKey(base_key))
								{
									_currentDialogueIndex[base_key] = 0;
								}

								string key = base_key + "_" + _currentDialogueIndex[base_key];
								string dialogue_text = Helper.Translation.Get(key);

                                //Monitor.VerboseLog($"Base key: {base_key}, full key: {key}");
                                //Monitor.VerboseLog($"Dialogue: {dialogue_text}");
                                // HACK: If there's no key, the key name is returned, so we can use this to determine if no key was found.
                                if (dialogue_text.Contains(key))
								{
									_currentDialogueIndex[base_key] = 0;

									key = base_key + "_" + _currentDialogueIndex[base_key];
									dialogue_text = Helper.Translation.Get(key);
								}

								Game1.drawObjectDialogue(dialogue_text);

                                // Advance this dialogue to the next string.
                                _currentDialogueIndex[base_key]++;
                            }
                        }
                        else if (split.Length >= 1 && split[0] == "PinGame")
                        {
                            Helper.Input.Suppress(e.Button);
                            string key = "PinGame.Barker.0";
                            if (PinGame.claimedPrizes != null)
                            {
                                int index = Math.Min(5, PinGame.claimedPrizes.Count);
                                key = $"PinGame.Barker.{index}";
                            }
                            Game1.currentLocation.createQuestionDialogue(
                                ModEntry.Instance.Helper.Translation.Get(key),
                                new Response[2]
                                {
                                    new Response("Play", ModEntry.Instance.Helper.Translation.Get("PinGame.Answer.Play")),
                                    new Response("Leave", ModEntry.Instance.Helper.Translation.Get("PinGame.Answer.Leave"))
                                },
                                delegate (Farmer who, string answer)
                                {
                                    if (answer.Equals("Play"))
                                    {
                                        Game1.currentMinigame = new PinGame();
                                    }
                                    else if (answer.Equals("Leave"))
                                    {
                                        // Nothing
                                    }
                                    else
                                    {
                                        Monitor.Log($"Received invalid response to PinGame question: {answer}", LogLevel.Info);
                                    }
                                }
                                );
                        }
                    }
                }
            }
        }

        // Asset Editing and Loading
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return (asset.AssetNameEquals("Data/mail") || asset.AssetNameEquals("Characters/Dialogue/Krobus") || asset.AssetNameEquals("Strings/StringsFromCSFiles"));
        }
        public void Edit<T>(IAssetData asset)
        {
            IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
            if (asset.AssetNameEquals("Data/mail"))
            {
                Monitor.Log("Adding our mail to Data/mail");
                data["Wizard_ShadowFestival"] = Helper.Translation.Get("wizard-letter");
            }
            else if (IsShadowFestivalToday() && asset.AssetNameEquals("Characters/Dialogue/Krobus"))
            {
                Monitor.Log("Adding our changes to Krobus standard dialogue");
                data["fall_27"] = Helper.Translation.Get("krobus-dialogue-known");
                data["Sat"] = Helper.Translation.Get("krobus-dialogue-known");
            }
            else if (IsShadowFestivalToday() && asset.AssetNameEquals("Strings/StringsFromCSFiles"))
            {
                Monitor.Log("Adding our changes to Krobus intro dialogue");
                data["NPC.cs.3990"] = Helper.Translation.Get("krobus-dialogue-unknown");
            }
        }
        public bool CanLoad<T>(IAssetInfo asset)
        {
            return (asset.AssetNameEquals("Maps/Sewer") && IsShadowFestivalToday());
        }
        public T Load<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Maps/Sewer"))
            {
                Monitor.Log("Loading our replacement Sewer map");
                Map map = this.Helper.Content.Load<Map>("assets/Sewer.tbin");
                _mapChanged = true;
                return (T)(object)map;
            }

            throw new NotSupportedException($"Unexpected asset: {asset.AssetName}");
        }

        // Utility functions
        public bool IsShadowFestivalToday()
        {
            return (Game1.currentSeason.ToLower() == "fall" && Game1.dayOfMonth.Equals(27));
        }
        public bool IsShadowFestivalTomorrow()
        {
            return (Game1.currentSeason.ToLower() == "fall" && Game1.dayOfMonth.Equals(26));
        }

        public void AfterFade()
        {
            Game1.player.swimming.Value = false;
            Game1.player.changeOutOfSwimSuit();
            Game1.drawObjectDialogue(Helper.Translation.Get("interaction.nohat"));
            Game1.messagePause = true;
            Game1.warpFarmer("Forest", 94, 100, 2);
			Game1.fadeToBlackAlpha = 1.0F;
        }
    }
}
