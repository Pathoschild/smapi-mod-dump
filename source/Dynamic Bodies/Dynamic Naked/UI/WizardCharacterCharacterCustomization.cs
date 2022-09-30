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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using StardewValley.Menus;

using DynamicBodies.Data;
using DynamicBodies.Patches;

namespace DynamicBodies.UI
{
	public class WizardCharacterCharacterCustomization : IClickableMenu
	{

		public const int region_okbutton = 505, region_randomButton = 507, region_male = 508, region_female = 509;

		public const int region_dog = 510, region_cat = 511;

		public const int region_skinLeft = 518, region_skinRight = 519;
		public const int region_directionLeft = 520, region_directionRight = 521;

		public const int region_nameBox = 536, region_favThingBox = 538;

		public const int region_opendoctors = 701, region_openleahs = 702, region_openpams = 703, region_openhaley = 704;

		public ClickableTextureComponent doctorsButton;
		public ClickableTextureComponent leahsButton;
		public ClickableTextureComponent pamsButton;
		public ClickableTextureComponent haleyButton;

		private Texture2D UItexture;


		public List<ClickableComponent> labels = new List<ClickableComponent>();
		public List<ClickableComponent> leftSelectionButtons = new List<ClickableComponent>();
		public List<ClickableComponent> rightSelectionButtons = new List<ClickableComponent>();

		public List<ClickableComponent> genderButtons = new List<ClickableComponent>();
		public List<ClickableComponent> petButtons = new List<ClickableComponent>();

		public ClickableTextureComponent okButton;
		public ClickableTextureComponent randomButton;

		private TextBox nameBox;
		private TextBox favThingBox;

		public bool isModifyingExistingPet;

		private string hoverText;
		private string hoverTitle;

		public ClickableComponent nameBoxCC;
		public ClickableComponent favThingBoxCC;

		private ClickableComponent nameLabel;

		private ClickableComponent favoriteLabel;
		private ClickableComponent skinLabel;


		protected Farmer _displayFarmer;
		public Rectangle portraitBox;
		public Rectangle? petPortraitBox;

		public string oldName = "";

		private int timesRandom;

		List<string> randomSounds = new List<string>(){ "drumkit1", "dirtyHit", "axchop", "hoeHit", "fishSlap", "drumkit6", "drumkit5", "drumkit6", "junimoMeep1", "coin", "axe", "hammer", "drumkit2", "drumkit4", "drumkit3" };


		public WizardCharacterCharacterCustomization()
			: base(Game1.uiViewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2, Game1.uiViewport.Height / 2 - (528 + IClickableMenu.borderWidth * 2) / 2 - 64, 632 + IClickableMenu.borderWidth * 2, 528 + IClickableMenu.borderWidth * 2 + 64)
		{
			UItexture = Game1.content.Load<Texture2D>("Mods/ribeena.dynamicbodies/assets/Interface/ui.png");

			this.oldName = Game1.player.Name;
			this.setUpPositions();

			this._displayFarmer = Game1.player;
			_displayFarmer.faceDirection(2);//look down
		}

		public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
		{
			base.gameWindowSizeChanged(oldBounds, newBounds);

			base.xPositionOnScreen = Game1.uiViewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2;
			base.yPositionOnScreen = Game1.uiViewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - 64;

			this.setUpPositions();
		}

		private void setUpPositions()
		{
			this.labels.Clear();
			this.petButtons.Clear();
			this.genderButtons.Clear();
			this.leftSelectionButtons.Clear();
			this.rightSelectionButtons.Clear();

			this.okButton = new ClickableTextureComponent("OK", new Rectangle(base.xPositionOnScreen + base.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 64, base.yPositionOnScreen + base.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + 16, 64, 64), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f)
			{
				myID = region_okbutton,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = -99998
			};

			this.portraitBox = new Rectangle(base.xPositionOnScreen + 64 + 42 - 2, base.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16, 128, 192);

			this.leftSelectionButtons.Add(new ClickableTextureComponent("Direction", new Rectangle(this.portraitBox.X - 32, this.portraitBox.Y + 144, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f)
			{
				myID = region_directionLeft,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				leftNeighborImmutable = true,
				rightNeighborID = -99998,
				downNeighborID = -99998
			});
			this.rightSelectionButtons.Add(new ClickableTextureComponent("Direction", new Rectangle(this.portraitBox.Right - 32, this.portraitBox.Y + 144, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f)
			{
				myID = region_directionRight,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = -99998
			});

			this.randomButton = new ClickableTextureComponent(new Rectangle(base.xPositionOnScreen + 48, base.yPositionOnScreen + 64 + 56, 40, 40), Game1.mouseCursors, new Rectangle(381, 361, 10, 10), 4f)
			{
				myID = region_randomButton,
				upNeighborID = -99998,
				leftNeighborImmutable = true,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = -99998
			};


			//Start along side portrait
			Point currentPosition = new Point(base.xPositionOnScreen + 64 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 256,
				base.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16);

			int textBoxLabelsXOffset = ((LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.pt) ? (-4) : 0);

			this.nameBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont, Game1.textColor)
			{
				X = currentPosition.X,
				Y = currentPosition.Y,
				Text = Game1.player.Name
			};

			this.nameBoxCC = new ClickableComponent(new Rectangle(currentPosition.X, currentPosition.Y, 192, 48), "")
			{
				myID = region_nameBox,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = -99998
			};

			this.labels.Add(this.nameLabel = new ClickableComponent(new Rectangle(currentPosition.X - 112 + textBoxLabelsXOffset, currentPosition.Y + 8, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Name")));

			//Next line
			currentPosition.Y += 64;

			int favThingBoxXoffset = ((LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko) ? 48 : 0);
			this.favThingBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont, Game1.textColor)
			{
				X = currentPosition.X + favThingBoxXoffset,
				Y = currentPosition.Y,
				Text = Game1.player.favoriteThing
			};

			this.favThingBoxCC = new ClickableComponent(new Rectangle(currentPosition.X, currentPosition.Y, 192, 48), "")
			{
				myID = region_favThingBox,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = -99998
			};

			this.labels.Add(this.favoriteLabel = new ClickableComponent(new Rectangle(currentPosition.X - 112 + textBoxLabelsXOffset, currentPosition.Y + 8, 1, 1), Game1.content.LoadString("Strings\\UI:Character_FavoriteThing")));

			//Next line
			currentPosition.Y += 72;

			this.genderButtons.Add(new ClickableTextureComponent("Male", new Rectangle(currentPosition.X + 8, currentPosition.Y, 64, 64), null, "Male", Game1.mouseCursors, new Rectangle(128, 192, 16, 16), 4f)
			{
				myID = region_male,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = -99998
			});
			this.genderButtons.Add(new ClickableTextureComponent("Female", new Rectangle(currentPosition.X + 8 + 80, currentPosition.Y, 64, 64), null, "Female", Game1.mouseCursors, new Rectangle(144, 192, 16, 16), 4f)
			{
				myID = region_female,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = -99998
			});

			//Below portrait
			int leftSelectionXOffset = ((LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.pt || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr) ? (-20) : 0);

			currentPosition.X = base.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 16;
			currentPosition.Y = portraitBox.Y + portraitBox.Height + 16;

			this.leftSelectionButtons.Add(new ClickableTextureComponent("Skin", new Rectangle(currentPosition.X + leftSelectionXOffset, currentPosition.Y, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f)
			{
				myID = region_skinLeft,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = -99998
			});
			this.labels.Add(this.skinLabel = new ClickableComponent(new Rectangle(currentPosition.X + 64 + 8 + leftSelectionXOffset / 2, currentPosition.Y + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Skin")));
			this.rightSelectionButtons.Add(new ClickableTextureComponent("Skin", new Rectangle(currentPosition.X + 128, currentPosition.Y, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f)
			{
				myID = region_skinRight,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = -99998
			});

			//Second column
			currentPosition.X += 224;

			this.isModifyingExistingPet = false;
			Pet pet = Game1.getCharacterFromName<Pet>(Game1.player.getPetName(), mustBeVillager: false);
			if (pet != null)
			{
				Game1.player.whichPetBreed = pet.whichBreed;
				Game1.player.catPerson = pet is Cat;
				this.isModifyingExistingPet = true;

				this.labels.Add(new ClickableComponent(new Rectangle(currentPosition.X + 300 / 2 - (int)(Game1.smallFont.MeasureString(pet.Name).X / 2f), currentPosition.Y, 1, 1), pet.Name));

				this.petPortraitBox = new Rectangle(currentPosition.X + 300 / 2 - 32, currentPosition.Y + 32, 64, 64);


				this.leftSelectionButtons.Add(new ClickableTextureComponent("Pet", new Rectangle(this.petPortraitBox.Value.Left - 64, this.petPortraitBox.Value.Top, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f)
				{
					myID = region_cat,
					upNeighborID = -99998,
					leftNeighborID = -99998,
					rightNeighborID = -99998,
					downNeighborID = -99998
				});
				this.rightSelectionButtons.Add(new ClickableTextureComponent("Pet", new Rectangle(this.petPortraitBox.Value.Left + 64, this.petPortraitBox.Value.Top, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f)
				{
					myID = region_dog,
					upNeighborID = -99998,
					leftNeighborID = -99998,
					rightNeighborID = -99998,
					downNeighborID = -99998
				});
			}

			//First column
			currentPosition.X -= 224;
			//Next line
			currentPosition.Y += 96;

			doctorsButton = new ClickableTextureComponent("Surgery", new Rectangle(currentPosition.X, currentPosition.Y, 128, 64), null, null, UItexture, new Rectangle(128, 32, 32, 16), 4f)
			{
				myID = region_opendoctors,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = CharacterCustomization.region_okbutton
			};
			leahsButton = new ClickableTextureComponent("Colorist", new Rectangle(currentPosition.X + (32 + 2) * 4, currentPosition.Y, 128, 64), null, null, UItexture, new Rectangle(128, 48, 32, 16), 4f)
			{
				myID = region_openleahs,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = -99998
			};
			pamsButton = new ClickableTextureComponent("Sink", new Rectangle(currentPosition.X + (32 + 2) * 4 * 2, currentPosition.Y, 128, 64), null, null, UItexture, new Rectangle(128, 64, 32, 16), 4f)
			{
				myID = region_openpams,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = -99998
			};
			haleyButton = new ClickableTextureComponent("Trinkets", new Rectangle(currentPosition.X + (32 + 2) * 4 * 3, currentPosition.Y, 128, 64), null, null, UItexture, new Rectangle(128, 16, 32, 16), 4f)
			{
				myID = region_openhaley,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = -99998
			};


			if (Game1.options.snappyMenus && Game1.options.gamepadControls)
			{
				base.populateClickableComponentList();
				this.snapToDefaultClickableComponent();
			}
		}


		public override void snapToDefaultClickableComponent()
		{
			base.currentlySnappedComponent = base.getComponentWithID(region_directionRight);
			this.snapCursorToCurrentSnappedComponent();
		}

		private void optionButtonClick(string name)
		{
			switch (name)
			{
				case "Male":
					ModEntry.debugmsg($"Changed player gender - Male", LogLevel.Debug);
					Game1.player.changeGender(male: true);
					Game1.player.changeHairStyle(0);
					break;
				case "Female":
					ModEntry.debugmsg($"Changed player gender - Female", LogLevel.Debug);
					Game1.player.changeGender(male: false);
					Game1.player.changeHairStyle(16);
					break;
				case "OK":
					{
						if (!this.canLeaveMenu())
						{
							return;
						}
						Game1.player.Name = this.nameBox.Text.Trim();
						Game1.player.displayName = Game1.player.Name;
						Game1.player.favoriteThing.Value = this.favThingBox.Text.Trim();
						Game1.player.isCustomized.Value = true;

						try
						{
							if (Game1.player.Name != this.oldName && Game1.player.Name.IndexOf("[") != -1 && Game1.player.Name.IndexOf("]") != -1)
							{
								int start = Game1.player.Name.IndexOf("[");
								int end = Game1.player.Name.IndexOf("]");
								if (end > start)
								{
									string s = Game1.player.Name.Substring(start + 1, end - start - 1);
									int item_index = -1;
									if (int.TryParse(s, out item_index))
									{
										string itemName = Game1.objectInformation[item_index].Split('/')[0];
										switch (Game1.random.Next(5))
										{
											case 0:
												Game1.chatBox.addMessage(Game1.content.LoadString("Strings\\UI:NameChange_EasterEgg1"), new Color(104, 214, 255));
												break;
											case 1:
												Game1.chatBox.addMessage(Game1.content.LoadString("Strings\\UI:NameChange_EasterEgg2", Lexicon.makePlural(itemName)), new Color(100, 50, 255));
												break;
											case 2:
												Game1.chatBox.addMessage(Game1.content.LoadString("Strings\\UI:NameChange_EasterEgg3", Lexicon.makePlural(itemName)), new Color(0, 220, 40));
												break;
											case 3:
												Game1.chatBox.addMessage(Game1.content.LoadString("Strings\\UI:NameChange_EasterEgg4"), new Color(0, 220, 40));
												DelayedAction.functionAfterDelay(delegate
												{
													Game1.chatBox.addMessage(Game1.content.LoadString("Strings\\UI:NameChange_EasterEgg5"), new Color(104, 214, 255));
												}, 12000);
												break;
											case 4:
												Game1.chatBox.addMessage(Game1.content.LoadString("Strings\\UI:NameChange_EasterEgg6", Lexicon.getProperArticleForWord(itemName), itemName), new Color(100, 120, 255));
												break;
										}
									}
								}
							}
						}
						catch (Exception)
						{
						}
						string changed_pet_name = null;
						if (this.petPortraitBox.HasValue && Game1.gameMode == 3 && Game1.locations != null)
						{
							Pet pet = Game1.getCharacterFromName<Pet>(Game1.player.getPetName(), mustBeVillager: false);
							if (pet != null && this.petHasChanges(pet))
							{
								pet.whichBreed.Value = Game1.player.whichPetBreed;
								changed_pet_name = pet.getName();
							}
						}
						Game1.exitActiveMenu();

						if (changed_pet_name != null)
						{
							ModEntry.multiplayer.globalChatInfoMessage("Makeover_Pet", Game1.player.Name, changed_pet_name);
						}
						else
						{
							ModEntry.multiplayer.globalChatInfoMessage("Makeover", Game1.player.Name);
						}
						Game1.flashAlpha = 1f;
						Game1.playSound("yoba");

						break;
					}
			}
			Game1.playSound("coin");
		}

		public bool petHasChanges(Pet pet)
		{
			if (Game1.player.catPerson && pet == null)
			{
				return true;
			}
			if (Game1.player.whichPetBreed != pet.whichBreed.Value)
			{
				return true;
			}
			return false;
		}

		private void selectionClick(string name, int change)
		{
			switch (name)
			{
				case "Skin":
					Game1.player.changeSkinColor((int)Game1.player.skin + change);
					PlayerBaseExtended pbe = PlayerBaseExtended.Get(Game1.player);
					pbe.nakedLower.texture = null;
					pbe.nakedUpper.texture = null;
					Game1.playSound("skeletonStep");
					break;
				case "Direction":
					this._displayFarmer.faceDirection((this._displayFarmer.FacingDirection - change + 4) % 4);
					this._displayFarmer.FarmerSprite.StopAnimation();
					this._displayFarmer.completelyStopAnimatingOrDoingAction();
					Game1.playSound("pickUpItem");
					break;
				case "Pet":
					Game1.player.whichPetBreed += change;
					if (Game1.player.whichPetBreed >= 3)
					{
						Game1.player.whichPetBreed = 0;
						if (!this.isModifyingExistingPet)
						{
							Game1.player.catPerson = !Game1.player.catPerson;
						}
					}
					else if (Game1.player.whichPetBreed < 0)
					{
						Game1.player.whichPetBreed = 2;
						if (!this.isModifyingExistingPet)
						{
							Game1.player.catPerson = !Game1.player.catPerson;
						}
					}
					Game1.playSound("coin");
					break;
			}
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{

			if (this.genderButtons.Count > 0)
			{
				foreach (ClickableComponent c6 in this.genderButtons)
				{
					if (c6.containsPoint(x, y))
					{

						PlayerBaseExtended.Get(Game1.player).DefaultOptions(Game1.player);
						this.optionButtonClick(c6.name);

						c6.scale -= 0.5f;
						c6.scale = Math.Max(3.5f, c6.scale);
					}
				}
			}
			if (this.petButtons.Count > 0)
			{
				foreach (ClickableComponent c4 in this.petButtons)
				{
					if (c4.containsPoint(x, y))
					{
						this.optionButtonClick(c4.name);
						c4.scale -= 0.5f;
						c4.scale = Math.Max(3.5f, c4.scale);
					}
				}
			}

			if (this.leftSelectionButtons.Count > 0)
			{
				foreach (ClickableComponent c2 in this.leftSelectionButtons)
				{
					if (c2.containsPoint(x, y))
					{
						this.selectionClick(c2.name, -1);
						if (c2.scale != 0f)
						{
							c2.scale -= 0.25f;
							c2.scale = Math.Max(0.75f, c2.scale);
						}
					}
				}
			}
			if (this.rightSelectionButtons.Count > 0)
			{
				foreach (ClickableComponent c in this.rightSelectionButtons)
				{
					if (c.containsPoint(x, y))
					{
						this.selectionClick(c.name, 1);
						if (c.scale != 0f)
						{
							c.scale -= 0.25f;
							c.scale = Math.Max(0.75f, c.scale);
						}
					}
				}
			}
			if (this.okButton.containsPoint(x, y) && this.canLeaveMenu())
			{
				this.optionButtonClick(this.okButton.name);
				this.okButton.scale -= 0.25f;
				this.okButton.scale = Math.Max(0.75f, this.okButton.scale);
			}

			this.nameBox.Update();
			this.favThingBox.Update();

			//Sub menu buttons
			if (doctorsButton.containsPoint(x, y))
			{
				Game1.activeClickableMenu = new DoctorModifier(true);
				doctorsButton.scale -= 0.25f;
				doctorsButton.scale = Math.Max(0.75f * doctorsButton.baseScale, doctorsButton.scale);
				Game1.playSound("shwip");
			}
			if (leahsButton.containsPoint(x, y))
			{
				Game1.activeClickableMenu = new FullColourModifier(true);
				leahsButton.scale -= 0.25f;
				leahsButton.scale = Math.Max(0.75f * leahsButton.baseScale, leahsButton.scale);
				Game1.playSound("shwip");
			}
			if (pamsButton.containsPoint(x, y))
			{
				Game1.activeClickableMenu = new SimpleColourModifier(true);
				pamsButton.scale -= 0.25f;
				pamsButton.scale = Math.Max(0.75f * pamsButton.baseScale, pamsButton.scale);
				Game1.playSound("shwip");
			}
			if (haleyButton.containsPoint(x, y))
			{
				Game1.activeClickableMenu = new AccessoryModifier(true);
				haleyButton.scale -= 0.25f;
				haleyButton.scale = Math.Max(0.75f * haleyButton.baseScale, haleyButton.scale);
				Game1.playSound("shwip");
			}

			if (this.randomButton.containsPoint(x, y))
			{
				string sound = "drumkit6";
				if (this.timesRandom > 0)
				{
					sound = randomSounds[Game1.random.Next(randomSounds.Count)];
				}
				Game1.playSound(sound);
				this.timesRandom++;
				this.randomButton.scale = 3.5f;

				RandomiseCharacter();
			}
		}

		public static void RandomiseCharacter()
		{
			Farmer who = Game1.player;

			PlayerBaseExtended pbe = PlayerBaseExtended.Get(who);
			//Skin colour
			Game1.player.changeSkinColor(Game1.random.Next(6));
			if (Game1.random.NextDouble() < 0.25)
			{
				Game1.player.changeSkinColor(Game1.random.Next(24));
			}

			//Hair Colour
			Color hairC = RandomColor();
			who.changeHairColor(hairC);
			pbe.SetModData(who, "DB.darkHair", FarmerRendererPatched.changeBrightness(hairC, Color.DarkGray, false).PackedValue.ToString());

			//Hair style
			List<string> all_hairs = ModEntry.getContentPackOptions(ModEntry.hairOptions).ToList();

			int selection = Game1.random.Next(all_hairs.Count);
			pbe.SetModData(who, "DB.hairStyle", all_hairs[selection]);
			if (pbe.hairStyle.option.StartsWith("Vanilla"))
			{
				who.changeHairStyle(int.Parse(pbe.hairStyle.file));
				//Update the base file recording as needed
				pbe.SetVanillaFile(who.getTexture());
			}
			else
			{
				ExtendedHair.ContentPackHairOption option = ModEntry.hairOptions[selection] as ExtendedHair.ContentPackHairOption;
				if (option.settings.isBaldStyle)
				{
					pbe.SetVanillaFile("farmer_base_bald");
				}
				else
				{
					pbe.SetVanillaFile("farmer_base");
				}
			}

			//Body
			List<string> all_bodys = ModEntry.getContentPackOptions(ModEntry.bodyOptions, who.IsMale).ToList();
			if (all_bodys.Count > 0)
			{
				pbe.SetModData(who, "DB.body", all_bodys[Game1.random.Next(all_bodys.Count)]);
			}

			//Face
			List<string> all_faces = ModEntry.getContentPackOptions(ModEntry.faceOptions, who.IsMale).ToList();
			if (all_faces.Count > 0)
			{
				pbe.SetModData(who, "DB.face", all_faces[Game1.random.Next(all_faces.Count)]);
			}

			//Eyes
			List<string> all_eyes = ModEntry.getContentPackOptions(ModEntry.eyesOptions, who.IsMale).ToList();
			if (all_eyes.Count > 0)
			{
				pbe.SetModData(who, "DB.eyes", all_eyes[Game1.random.Next(all_eyes.Count)]);
			}

			//Eyes Colour
			Color eyeC = RandomColor();
			who.changeEyeColor(eyeC);
			if (Game1.random.NextDouble() < 0.25)
			{
				pbe.SetModData(who, "DB.eyeColorR", RandomColor().PackedValue.ToString());
			}
			else
			{
				pbe.SetModData(who, "DB.eyeColorR", eyeC.PackedValue.ToString());
			}

			//Lash colour
			if (Game1.random.NextDouble() < 0.25)
			{
				pbe.SetModData(who, "DB.lash", RandomColor().PackedValue.ToString());
			}
			else
			{
				pbe.SetModData(who, "DB.lash", Color.Black.PackedValue.ToString());
			}

			//Nose
			List<string> all_noses = ModEntry.getContentPackOptions(ModEntry.noseOptions, who.IsMale).ToList();
			if (all_noses.Count > 0)
			{
				pbe.SetModData(who, "DB.nose", all_noses[Game1.random.Next(all_noses.Count)]);
			}

			//Ears
			List<string> all_ears = ModEntry.getContentPackOptions(ModEntry.earsOptions, who.IsMale).ToList();
			if (all_ears.Count > 0)
			{
				pbe.SetModData(who, "DB.ears", all_ears[Game1.random.Next(all_ears.Count)]);
			}
			//Arm
			List<string> all_arms = ModEntry.getContentPackOptions(ModEntry.armOptions, who.IsMale).ToList();
			if (all_arms.Count > 0)
			{
				pbe.SetModData(who, "DB.arm", all_arms[Game1.random.Next(all_arms.Count)]);
			}

			//Beard
			List<string> all_beards = ModEntry.getContentPackOptions(ModEntry.beardOptions, who.IsMale).ToList();
			if (all_beards.Count > 0)
			{
				pbe.SetModData(who, "DB.beard", all_beards[Game1.random.Next(all_beards.Count)]);
			}

			//BodyHair
			List<string> all_bh = ModEntry.getContentPackOptions(ModEntry.bodyHairOptions, who.IsMale).ToList();
			if (all_bh.Count > 0)
			{
				pbe.SetModData(who, "DB.bodyHair", all_bh[Game1.random.Next(all_bh.Count)]);
			}

			//Naked
			List<string> all_no = ModEntry.getContentPackOptions(ModEntry.nudeLOptions, who.IsMale).ToList();
			if (all_no.Count > 0)
			{
				pbe.SetModData(who, "DB.nakedLower", all_no[Game1.random.Next(all_no.Count)]);
			}

			//NakedU
			List<string> all_nou = ModEntry.getContentPackOptions(ModEntry.nudeUOptions, who.IsMale).ToList();
			if (all_nou.Count > 0)
			{
				pbe.SetModData(who, "DB.nakedUpper", all_nou[Game1.random.Next(all_nou.Count)]);
			}
		}

		public static Color RandomColor()
        {
			Color c = new Color(Game1.random.Next(25, 254), Game1.random.Next(25, 254), Game1.random.Next(25, 254));
			if (Game1.random.NextDouble() < 0.5)
			{
				c.R /= 2;
				c.G /= 2;
				c.B /= 2;
			}
			if (Game1.random.NextDouble() < 0.5)
			{
				c.R = (byte)Game1.random.Next(15, 50);
			}
			if (Game1.random.NextDouble() < 0.5)
			{
				c.G = (byte)Game1.random.Next(15, 50);
			}
			if (Game1.random.NextDouble() < 0.5)
			{
				c.B = (byte)Game1.random.Next(15, 50);
			}
			return c;
		}

		public override void leftClickHeld(int x, int y)
		{
		}

		public override void releaseLeftClick(int x, int y)
		{
		}

		public override void receiveRightClick(int x, int y, bool playSound = true)
		{
		}

		public override void receiveKeyPress(Keys key)
		{
			if (Game1.options.SnappyMenus && !Game1.options.doesInputListContain(Game1.options.menuButton, key) && Game1.GetKeyboardState().GetPressedKeys().Count() == 0)
			{
				base.receiveKeyPress(key);
			}
		}

		public override void performHoverAction(int x, int y)
		{
			this.hoverText = "";
			this.hoverTitle = "";

			foreach (ClickableTextureComponent c6 in this.leftSelectionButtons)
			{
				if (c6.containsPoint(x, y))
				{
					c6.scale = Math.Min(c6.scale + 0.02f, c6.baseScale + 0.1f);
				}
				else
				{
					c6.scale = Math.Max(c6.scale - 0.02f, c6.baseScale);
				}
			}

			foreach (ClickableTextureComponent c5 in this.rightSelectionButtons)
			{
				if (c5.containsPoint(x, y))
				{
					c5.scale = Math.Min(c5.scale + 0.02f, c5.baseScale + 0.1f);
				}
				else
				{
					c5.scale = Math.Max(c5.scale - 0.02f, c5.baseScale);
				}
			}

			foreach (ClickableTextureComponent c3 in this.genderButtons)
			{
				if (c3.containsPoint(x, y))
				{
					c3.scale = Math.Min(c3.scale + 0.05f, c3.baseScale + 0.5f);
				}
				else
				{
					c3.scale = Math.Max(c3.scale - 0.05f, c3.baseScale);
				}
			}

			if (this.okButton.containsPoint(x, y) && this.canLeaveMenu())
			{
				this.okButton.scale = Math.Min(this.okButton.scale + 0.02f, this.okButton.baseScale + 0.1f);
			}
			else
			{
				this.okButton.scale = Math.Max(this.okButton.scale - 0.02f, this.okButton.baseScale);
			}

			this.randomButton.tryHover(x, y, 0.25f);
			this.randomButton.tryHover(x, y, 0.25f);
			this.nameBox.Hover(x, y);
			this.favThingBox.Hover(x, y);

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

			if (haleyButton.containsPoint(x, y))
			{
				haleyButton.scale = Math.Min(haleyButton.scale + 0.02f, haleyButton.baseScale * 1.1f);
			}
			else
			{
				haleyButton.scale = Math.Max(haleyButton.scale - 0.02f, haleyButton.baseScale);
			}
		}

		public bool canLeaveMenu()
		{
			if (Game1.player.Name.Length > 0 && Game1.player.farmName.Length > 0)
			{
				return Game1.player.favoriteThing.Length > 0;
			}
			return false;
		}

		public override void draw(SpriteBatch b)
		{
			bool ignoreTitleSafe = false;
			ignoreTitleSafe = true;

			Game1.drawDialogueBox(base.xPositionOnScreen, base.yPositionOnScreen, base.width, base.height, speaker: false, drawOnlyBox: true, null, objectDialogueWithPortrait: false, ignoreTitleSafe);

			b.Draw(Game1.daybg, new Vector2(this.portraitBox.X, this.portraitBox.Y), Color.White);
			foreach (ClickableTextureComponent c2 in this.genderButtons)
			{
				if (c2.visible)
				{
					c2.draw(b);
					if ((c2.name.Equals("Male") && Game1.player.IsMale) || (c2.name.Equals("Female") && !Game1.player.IsMale))
					{
						b.Draw(Game1.mouseCursors, c2.bounds, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 34), Color.White);
					}
				}
			}

			Game1.player.Name = this.nameBox.Text;
			Game1.player.favoriteThing.Set(this.favThingBox.Text);

			foreach (ClickableTextureComponent leftSelectionButton in this.leftSelectionButtons)
			{
				leftSelectionButton.draw(b);
			}
			foreach (ClickableComponent c3 in this.labels)
			{
				if (!c3.visible)
				{
					continue;
				}
				string sub = "";
				float offset = 0f;
				float subYOffset = 0f;
				Color color = Game1.textColor;
				if (c3 == this.nameLabel)
				{
					color = ((Game1.player.Name != null && Game1.player.Name.Length < 1) ? Color.Red : Game1.textColor);
				}
				else if (c3 == this.favoriteLabel)
				{
					color = ((Game1.player.favoriteThing.Value != null && Game1.player.favoriteThing.Length < 1) ? Color.Red : Game1.textColor);
				}
				else if (c3 == this.skinLabel)
				{
					offset = 21f - Game1.smallFont.MeasureString(c3.name).X / 2f;
					sub = ((int)Game1.player.skin + 1).ToString() ?? "";
				}
				else
				{
					color = Game1.textColor;
				}
				Utility.drawTextWithShadow(b, c3.name, Game1.smallFont, new Vector2((float)c3.bounds.X + offset, c3.bounds.Y), color);
				if (sub.Length > 0)
				{
					Utility.drawTextWithShadow(b, sub, Game1.smallFont, new Vector2((float)(c3.bounds.X + 21) - Game1.smallFont.MeasureString(sub).X / 2f, (float)(c3.bounds.Y + 32) + subYOffset), color);
				}
			}
			foreach (ClickableTextureComponent rightSelectionButton in this.rightSelectionButtons)
			{
				rightSelectionButton.draw(b);
			}

			if (this.petPortraitBox.HasValue)
			{
				b.Draw(Game1.mouseCursors, this.petPortraitBox.Value, new Rectangle(160 + ((!Game1.player.catPerson) ? 48 : 0) + Game1.player.whichPetBreed * 16, 208, 16, 16), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.89f);
			}

			if (this.canLeaveMenu())
			{
				this.okButton.draw(b, Color.White, 0.75f);
			}
			else
			{
				this.okButton.draw(b, Color.White, 0.75f);
				this.okButton.draw(b, Color.Black * 0.5f, 0.751f);
			}

			this.nameBox.Draw(b);
			this.favThingBox.Draw(b);


			this.randomButton.draw(b);

			//Draw the new buttons
			doctorsButton.draw(b);
			leahsButton.draw(b);
			pamsButton.draw(b);
			haleyButton.draw(b);

			b.End();
			b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
			this._displayFarmer.FarmerRenderer.draw(b, this._displayFarmer.FarmerSprite.CurrentAnimationFrame, this._displayFarmer.FarmerSprite.CurrentFrame, this._displayFarmer.FarmerSprite.SourceRect, new Vector2(this.portraitBox.Center.X - 32, this.portraitBox.Bottom - 160), Vector2.Zero, 0.8f, Color.White, 0f, 1f, this._displayFarmer);
			b.End();
			b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
			if (this.hoverText != null && this.hoverText.Count() > 0)
			{
				IClickableMenu.drawHoverText(b, Game1.parseText(this.hoverText, Game1.smallFont, 256), Game1.smallFont, 0, 0, -1, this.hoverTitle);
			}
			base.drawMouse(b);
		}

		public override bool IsAutomaticSnapValid(int direction, ClickableComponent a, ClickableComponent b)
		{
			if (a.region != b.region)
			{
				return false;
			}

			if (a == this.favThingBoxCC && b.myID >= 522 && b.myID <= 530)
			{
				return false;
			}
			if (b == this.favThingBoxCC && a.myID >= 522 && a.myID <= 530)
			{
				return false;
			}

			if (a.name == "Direction" && b.name == "Pet")
			{
				return false;
			}
			if (b.name == "Direction" && a.name == "Pet")
			{
				return false;
			}

			if (this.randomButton != null)
			{
				switch (direction)
				{
					case 3:
						if (b == this.randomButton && a.name == "Direction")
						{
							return false;
						}
						break;
					default:
						if (a == this.randomButton && b.name != "Direction")
						{
							return false;
						}
						if (b == this.randomButton && a.name != "Direction")
						{
							return false;
						}
						break;
					case 0:
						break;
				}
				if (a.myID == 622 && direction == 1 && (b == this.nameBoxCC || b == this.favThingBoxCC))
				{
					return false;
				}
			}
			return base.IsAutomaticSnapValid(direction, a, b);
		}

		public override void update(GameTime time)
		{
			base.update(time);
		}

		protected override bool _ShouldAutoSnapPrioritizeAlignedElements()
		{
			return true;
		}
	}
}
