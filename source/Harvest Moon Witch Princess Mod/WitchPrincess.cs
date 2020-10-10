/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sasara2201/WitchPrincess
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using xTile;
using xTile.ObjectModel;
using xTile.Tiles;

namespace WitchPrincess
{
    /// <summary>The mod entry point.</summary>
    public class WitchPrincess : Mod, IAssetLoader, IAssetEditor
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.Warped += WarpedEventArgs;
            helper.Events.Display.MenuChanged += DisplayMenuChanged;
            helper.Events.GameLoop.DayStarted += WitchSpouse;
        }

        /// <summary>Get whether this instance can load the initial version of the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanLoad<T>(IAssetInfo asset)
        {
            
            if (asset.AssetNameEquals("Portraits/Wizard"))
                return true;

            if (asset.AssetNameEquals("Characters/Wizard"))
                return true;

            if (asset.AssetNameEquals("Maps/Beach-Luau"))
                return true;

            if (asset.AssetNameEquals("Maps/Forest-FlowerFestival"))
                return true;

            if (asset.AssetNameEquals("Maps/Forest-IceFestival"))
                return true;

            if (asset.AssetNameEquals("Maps/Town-Christmas"))
                return true;

            if (asset.AssetNameEquals("Maps/Town-EggFestival"))
                return true;

            if (asset.AssetNameEquals("Maps/Town-Halloween"))
                return true;

            if (Game1.hasLoadedGame)
                return asset.AssetNameEquals("Characters/Dialogue/MarriageDialogueWizard");

            return false;
        }

        /// <summary>Load a matched asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public T Load<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Portraits/Wizard"))
                return this.Helper.Content.Load<T>("assets/Portraits/Wizard.png", ContentSource.ModFolder);

            if (asset.AssetNameEquals("Characters/Wizard"))
                return this.Helper.Content.Load<T>("assets/Characters/wizard.png", ContentSource.ModFolder);

            if (asset.AssetNameEquals("Maps/Beach-Luau"))
                return this.Helper.Content.Load<T>("assets/Maps/Beach-Luau.tbin", ContentSource.ModFolder);

            if (asset.AssetNameEquals("Maps/Forest-FlowerFestival"))
                return this.Helper.Content.Load<T>("assets/Maps/Forest-FlowerFestival.tbin", ContentSource.ModFolder);

            if (asset.AssetNameEquals("Maps/Forest-IceFestival"))
                return this.Helper.Content.Load<T>("assets/Maps/Forest-IceFestival.tbin", ContentSource.ModFolder);

            if (asset.AssetNameEquals("Maps/Town-Christmas"))
                return this.Helper.Content.Load<T>("assets/Maps/Town-Christmas.tbin", ContentSource.ModFolder);

            if (asset.AssetNameEquals("Maps/Town-EggFestival"))
                return this.Helper.Content.Load<T>("assets/Maps/Town-EggFestival.tbin", ContentSource.ModFolder);

            if (asset.AssetNameEquals("Maps/Town-Halloween"))
                return this.Helper.Content.Load<T>("assets/Maps/Town-Halloween.tbin", ContentSource.ModFolder);

            if (asset.AssetNameEquals("Characters/Dialogue/MarriageDialogueWizard"))
                return (T)(object)this.LoadAssetFromFile<string, string>("WizardMarriageDialogue");


            throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.");
        }

        public bool CanEdit<T>(IAssetInfo info)
        {
            if (info.AssetNameEquals("Characters/Dialogue/Wizard"))
            {
                return true;
            }

            if (info.AssetNameEquals("Data/Quests"))
            {
                return true;
            }

            if (info.AssetNameEquals("Data/NPCDispositions"))
            {
                return true;
            }

            if (info.AssetNameEquals("Data/mail"))
            {
                return true;
            }

            if (info.AssetNameEquals("Data/EngagementDialogue"))
            {
                return true;
            }

            if (info.AssetNameEquals("Data/Festivals/spring13"))
            {
                return true;
            }

            if (info.AssetNameEquals("Data/Festivals/spring24"))
            {
                return true;
            }

            if (info.AssetNameEquals("Data/Festivals/summer28"))
            {
                return true;
            }

            if (info.AssetNameEquals("Data/Festivals/winter8"))
            {
                return true;
            }

            if (info.AssetNameEquals("Characters/Dialogue/schedules/Wizard"))
            {
                return true;
            }

            if (info.AssetNameEquals("Data/ExtraDialogue"))
            {
                return true;
            }

            if (info.AssetNameEquals("Data/Events/WizardHouse"))
            {
                return true;
            }

            if (info.AssetNameEquals("Data/Events/Railroad"))
            {
                return true;
            }
            if (info.AssetNameEquals("LooseSprites/Cursors"))
            {
                return true;
            }

            return false;
        }

        public IDictionary<TKey, TValue> LoadAssetFromFile<TKey, TValue>(string fileKey)
        {
            return
                this.Helper.Data.ReadJsonFile<Dictionary<TKey, TValue>>($"assets/{fileKey}.{this.Helper.Content.CurrentLocaleConstant}.json") // load translated file if it exists
                ?? this.Helper.Data.ReadJsonFile<Dictionary<TKey, TValue>>($"assets/{fileKey}.json"); // load English file

        }

        public void EditAssetFromFile<TKey, TValue>(IAssetData asset, string fileKey)
        {
            var assetData = asset.AsDictionary<TKey, TValue>().Data;
            var fromData = this.LoadAssetFromFile<TKey, TValue>(fileKey);

            foreach (var entry in fromData)
                assetData[entry.Key] = entry.Value;

        }

        public void Edit<T>(IAssetData asset)
        {

            if (asset.AssetNameEquals("Characters/Dialogue/Wizard"))
            {
                if (asset.AssetNameEquals("Characters/Dialogue/Wizard"))
                {
                    this.EditAssetFromFile<string, string>(asset, "WitchPrincess-Dialogue");
                }
                return;
            }
            if (asset.AssetNameEquals("Data/Quests"))
            {
                {
                    this.EditAssetFromFile<string, string>(asset, "WitchPrincess-Quests");
                }
                return;
            }

            if (asset.AssetNameEquals("Data/NPCDispositions"))
            {
                {
                    this.EditAssetFromFile<string, string>(asset, "WitchPrincess-NPCDisposition");
                }
                return;
            }

            if (asset.AssetNameEquals("Data/mail"))
            {
                {
                    this.EditAssetFromFile<string, string>(asset, "WitchPrincess-Mail");
                }
                return;
            }

            if (asset.AssetNameEquals("Data/EngagementDialogue"))
            {
                {
                    this.EditAssetFromFile<string, string>(asset, "WitchPrincess-EngagementDialogue");
                }
                return;
            }

            if (asset.AssetNameEquals("Data/Festivals/spring13"))
            {
                {
                    this.EditAssetFromFile<string, string>(asset, "WitchPrincess-Spring13");
                }
                return;
            }

            if (asset.AssetNameEquals("Data/Festivals/spring24"))
            {
                {
                    this.EditAssetFromFile<string, string>(asset, "WitchPrincess-Spring24");
                }
                return;
            }

            if (asset.AssetNameEquals("Data/Festivals/summer28"))
            {
                {
                    this.EditAssetFromFile<string, string>(asset, "WitchPrincess-Summer28");
                }
                return;
            }

            if (asset.AssetNameEquals("Data/Festivals/winter8"))
            {
                {
                    this.EditAssetFromFile<string, string>(asset, "WitchPrincess-Winter8");
                }
                return;
            }

            if (asset.AssetNameEquals("Characters/Dialogue/schedules/Wizard"))
            {
                {
                    this.EditAssetFromFile<string, string>(asset, "WitchPrincess-Schedules");
                }
                return;
            }

            if (asset.AssetNameEquals("Data/ExtraDialogue"))
            {
                {
                    this.EditAssetFromFile<string, string>(asset, "WitchPrincess-ExtraDialogue");
                }
                return;
            }

            if (asset.AssetNameEquals("Data/Events/WizardHouse"))
            {
                {
                    this.EditAssetFromFile<string, string>(asset, "WitchPrincess-Event-WizardHouse");
                }
                return;
            }

            if (asset.AssetNameEquals("Data/Events/Railroad"))
            {
                {
                    this.EditAssetFromFile<string, string>(asset, "WitchPrincess-Event-Railroad");
                }
                return;
            }

            if (asset.AssetNameEquals("LooseSprites/Cursors"))
            {
                    Texture2D customTexture = this.Helper.Content.Load<Texture2D>("assets/Images/WitchPrincess-Warp.png", ContentSource.ModFolder);
                    asset
                        .AsImage()
                        .PatchImage(customTexture, targetArea: new Rectangle(383, 1963, 24, 33));
                return;
            }

                throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.");
        }



        /*********
        ** Private methods
        *********/
        /// <summary>The method called after the player enters a new location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void WarpedEventArgs(object sender, WarpedEventArgs e)
        {
            if (!e.IsLocalPlayer)
                return;

            if (e.NewLocation is FarmHouse && Game1.player.isMarried() && Game1.player.spouse == "Wizard" && (Game1.player.HouseUpgradeLevel == 1 || Game1.player.HouseUpgradeLevel == 2))
                this.LoadSpouseRoom();
        }

        private void WitchSpouse(object sender, DayStartedEventArgs e)
        {

            if (Game1.player.spouse == "Wizard" && Game1.currentLocation.Name == "FarmHouse" && Game1.player.HouseUpgradeLevel == 1)
                this.LoadSpouseRoom();
            else if (Game1.player.spouse == "Wizard" && Game1.currentLocation.Name == "FarmHouse" && Game1.player.HouseUpgradeLevel == 2)
                this.LoadSpouseRoom();

        }


        private bool IsFestival(string season, int day)
        {
          return Game1.CurrentEvent.FestivalName != null && Game1.currentSeason == season && Game1.dayOfMonth == day;
        }

        /// <summary>The method called after the current menu changes.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void DisplayMenuChanged(object sender, MenuChangedEventArgs e)
        {
            {
                //check if it is Festival
                if (Game1.CurrentEvent?.isFestival == null)
                    return;

                // get Wizard dialogue
                if (Game1.currentSpeaker?.Name != "Wizard")
                    return;

                DialogueBox dialogue = e.NewMenu as DialogueBox;
                if (dialogue == null)
                    return;

                // check if player is married
                if (Game1.player.spouse != "Wizard")
                    return;

                //get dialogue text
                var DialogueStr = dialogue?.getCurrentString();

                //get the new text for each Festival, with translations
                int hearts = Game1.player.friendshipData["Wizard"].Points / NPC.friendshipPointsPerHeartLevel;
                string NewText = null;
                if (this.IsFestival("spring", 13) && (DialogueStr.StartsWith("Hmm...") || DialogueStr.StartsWith("唔……")))
                {
                    if (hearts > 6)
                        NewText = Helper.Translation.Get("dialogue.egg-festival");
                    else
                        NewText = Helper.Translation.Get("dialogue.egg-festival:low-friendship");
                }

                else if (this.IsFestival("spring", 24) && (DialogueStr.StartsWith("Tanzt du heute") || DialogueStr.StartsWith("Do you dance") || DialogueStr.StartsWith("你不该来")))
                {
                    if (hearts > 6)
                        NewText = Helper.Translation.Get("dialogue.flower-festival");
                    else
                        NewText = Helper.Translation.Get("dialogue.flower-festival.low-friendship");
                }

                if (this.IsFestival("summer", 11) && (DialogueStr.StartsWith("Die Meermenschen") || DialogueStr.StartsWith("The merpeople") || DialogueStr.StartsWith("鱼群们对")))
                {
                    if (hearts > 6)
                        NewText = Helper.Translation.Get("dialogue.luau-festival");
                    else
                        NewText = Helper.Translation.Get("dialogue.luau-festival.low-friendship");
                }

                else if (this.IsFestival("summer", 28) && (DialogueStr.StartsWith("Wie hast du") || DialogueStr.StartsWith("How did you") || DialogueStr.StartsWith("你是怎么找")))
                {
                    if (hearts > 11)
                        NewText = Helper.Translation.Get("dialogue.jelly-festival");
                    else if (hearts > 8)
                        NewText = Helper.Translation.Get("dialogue.jelly-festival.medium-friendship");
                    else
                        NewText = Helper.Translation.Get("dialogue.jelly-festival.low-friendship");
                }

                else if (this.IsFestival("fall", 16) && (DialogueStr.StartsWith("Welwick") || DialogueStr.StartsWith("我和维尔")))
                {
                    if (hearts > 6)
                        NewText = Helper.Translation.Get("dialogue.fair-festival");
                    else
                        NewText = Helper.Translation.Get("dialogue.fair-festival.low-friendship");
                }

                else if (this.IsFestival("fall", 27) && (DialogueStr.StartsWith("Die Angelegenheiten") || DialogueStr.StartsWith("The affairs of") || DialogueStr.StartsWith("尘世间的俗事我不")))
                {
                    if (hearts > 6)
                        NewText = Helper.Translation.Get("dialogue.haloween-festival");
                    else
                        NewText = Helper.Translation.Get("dialogue.haloween-festival.low-friendship");
                }

                else if (this.IsFestival("winter", 8) && (DialogueStr.StartsWith("Schleichst du dich") || DialogueStr.StartsWith("Sneaking off to") || DialogueStr.StartsWith("偷偷溜出")))
                {
                    if (hearts > 6)
                        NewText = Helper.Translation.Get("dialogue.ice-festival");
                    else
                        NewText = Helper.Translation.Get("dialogue.ice-festival.low-friendship");
                }

                else if (this.IsFestival("winter", 25) && (DialogueStr.StartsWith("Ah, der mysteriöse Winterstern") || DialogueStr.StartsWith("Ah, the mysterious Winter Star") || DialogueStr.StartsWith("啊，神秘的冬日星")))
                {
                    if (hearts > 6)
                        NewText = Helper.Translation.Get("dialogue.winter-festival");
                    else
                        NewText = Helper.Translation.Get("dialogue.winter-festival.low-friendship");
                }




                // replace dialogue1
                if (NewText != null)
                    Game1.activeClickableMenu = new DialogueBox(new Dialogue(NewText, Game1.getCharacterFromName("Wizard")));
            }

            {
                // check if player is married
                if (Game1.player.spouse != "Wizard")
                    return;

                // get Wizard dialogue
                DialogueBox dialogue = e.NewMenu as DialogueBox;
                if (dialogue == null)
                    return;
                if (Game1.currentSpeaker?.Name != "Wizard")
                    return;

                //get location
                if (!(Game1.currentLocation is Farm))
                    return;

                // get dialogue text
                var dialogueStr = dialogue?.getCurrentString();

                // get new text
                int hearts = Game1.player.friendshipData["Wizard"].Points / NPC.friendshipPointsPerHeartLevel;
                string NewText2 = null;
                if ((!(dialogueStr.StartsWith("今天天") || dialogueStr.StartsWith("The") || dialogueStr.StartsWith("Das Wetter") || dialogueStr.StartsWith("Die firsche") || dialogueStr.StartsWith("新鲜空气很好"))))
                {
                    if (hearts > 6)
                        NewText2 = Helper.Translation.Get("dialogue.outdoor");
                    else
                        NewText2 = Helper.Translation.Get("dialogue.outdoor.low-friendship");
                }

                // replace dialogue2
                if (NewText2 != null)
                    Game1.activeClickableMenu = new DialogueBox(new Dialogue(NewText2, Game1.getCharacterFromName("Wizard")));
            }

            {
                //get wizard dialogue
                DialogueBox dialogue = e.NewMenu as DialogueBox;
                if (dialogue == null)
                    return;

                //get Current Location, = Wizardhouse
                if (!(Game1.currentLocation is WizardHouse))
                    return;

                //get Wizard as dialoguepartner
                if (Game1.currentSpeaker?.Name != "Wizard")
                    return;

                // get dialogue text
                var dialogueStr = dialogue?.getCurrentString();

                
                //get new text
                string NewText3 = null;
                if (dialogueStr.StartsWith("I am the Witch Princess"))
                {
                    NewText3 = Helper.Translation.Get("OpeningDialogue");
                }

                //replace text3
                if (NewText3 != null)
                    Game1.activeClickableMenu = new DialogueBox(new Dialogue(NewText3, Game1.getCharacterFromName("Wizard")));
                return;
            }


        }


        /// <summary>Add the witch princess' spouse room to the farmhouse.</summary>
        public void LoadSpouseRoom()
        {
            if (Game1.player.isMarried())
            {
                // get farmhouse
                FarmHouse farmhouse = (FarmHouse)Game1.getLocationFromName("FarmHouse");

                // load custom map
                Map map = Helper.Content.Load<Map>(@"Content\WitchRoom.xnb");
                TileSheet room = new TileSheet(farmhouse.map, this.Helper.Content.GetActualAssetKey(@"Content\SRWitch.xnb"), map.TileSheets[0].SheetSize, map.TileSheets[0].TileSize) { Id = "ZZZ-WIZARD-SPOUSE-ROOM" };
                farmhouse.map.AddTileSheet(room);
                farmhouse.map.LoadTileSheets(Game1.mapDisplayDevice);

                // patch farmhouse
                farmhouse.map.Properties.Remove("DayTiles");
                farmhouse.map.Properties.Remove("NightTiles");
                TileSheet roomForFarmhouse = farmhouse.map.TileSheets[farmhouse.map.TileSheets.IndexOf(room)];

                int num = 0;
                Point point = new Point(num % 5 * 6, num / 5 * 9);
                Rectangle staticTile = farmhouse.upgradeLevel == 1 ? new Rectangle(29, 1, 6, 9) : new Rectangle(35, 10, 6, 9);
                for (int i = 0; i < staticTile.Width; i++)
                {
                    for (int j = 0; j < staticTile.Height; j++)
                    {
                        if (map.GetLayer("Back").Tiles[point.X + i, point.Y + j] != null)
                            farmhouse.map.GetLayer("Back").Tiles[staticTile.X + i, staticTile.Y + j] = new StaticTile(farmhouse.map.GetLayer("Back"), roomForFarmhouse, BlendMode.Alpha, map.GetLayer("Back").Tiles[point.X + i, point.Y + j].TileIndex);
                        if (map.GetLayer("Buildings").Tiles[point.X + i, point.Y + j] == null)
                            farmhouse.map.GetLayer("Buildings").Tiles[staticTile.X + i, staticTile.Y + j] = null;
                        else
                            farmhouse.map.GetLayer("Buildings").Tiles[staticTile.X + i, staticTile.Y + j] = new StaticTile(farmhouse.map.GetLayer("Buildings"), roomForFarmhouse, BlendMode.Alpha, map.GetLayer("Buildings").Tiles[point.X + i, point.Y + j].TileIndex);
                        if (j < staticTile.Height - 1 && map.GetLayer("Front").Tiles[point.X + i, point.Y + j] != null)
                            farmhouse.map.GetLayer("Front").Tiles[staticTile.X + i, staticTile.Y + j] = new StaticTile(farmhouse.map.GetLayer("Front"), roomForFarmhouse, BlendMode.Alpha, map.GetLayer("Front").Tiles[point.X + i, point.Y + j].TileIndex);
                        else if (j < staticTile.Height - 1)
                            farmhouse.map.GetLayer("Front").Tiles[staticTile.X + i, staticTile.Y + j] = null;
                        if (i == 4 && j == 4)
                        {
                            try
                            {
                                KeyValuePair<string, PropertyValue> prop = new KeyValuePair<string, PropertyValue>("NoFurniture", new PropertyValue("T"));

                                farmhouse.map.GetLayer("Back").Tiles[staticTile.X + i, staticTile.Y + j].Properties.Add(prop);
                            }


                            catch
                            {
                                // ignore errors
                            }
                        }
                    }
                }
            }
        }
    }
}

