/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.animals;
using AnimalHusbandryMod.common;
using AnimalHusbandryMod.farmer;
using MailFrameworkMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;
using DataLoader = AnimalHusbandryMod.common.DataLoader;

namespace AnimalHusbandryMod.tools
{
    public class ToolsLoader : IAssetEditor
    {
        private readonly Texture2D _toolsSpriteSheet;
        private readonly Texture2D _menuTilesSpriteSheet;
        private readonly Texture2D _customLetterBG;


        public ToolsLoader(Texture2D toolsSpriteSheet, Texture2D menuTilesSpriteSheet, Texture2D customLetterBG)
        {
            _toolsSpriteSheet = toolsSpriteSheet;
            _menuTilesSpriteSheet = menuTilesSpriteSheet;
            _customLetterBG = customLetterBG;

            if (DataLoader.ModConfig.Softmode)
            {
                MeatCleaverOverrides.Suffix = ".Soft";
            }
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("TileSheets\\tools") || asset.AssetNameEquals("Maps\\MenuTiles");
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("TileSheets\\tools"))
            {
                Texture2D toolSpriteSheet = asset.AsImage().Data;
                int originalWidth = toolSpriteSheet.Width;
                int originalHeight = toolSpriteSheet.Height;
                Color[] data1 = new Color[originalWidth * originalHeight];
                toolSpriteSheet.GetData<Color>(data1);
                Texture2D customToolsSpriteSheet = _toolsSpriteSheet;
                int meatCleaverWidth = customToolsSpriteSheet.Width;
                int meatCleaverlHeight = customToolsSpriteSheet.Height;
                Color[] data2 = new Color[meatCleaverWidth * meatCleaverlHeight];
                customToolsSpriteSheet.GetData<Color>(data2);
                Texture2D newSpriteSheet = new Texture2D(Game1.game1.GraphicsDevice, originalWidth, originalHeight + meatCleaverlHeight, false, SurfaceFormat.Color);

                var data3 = new Color[data1.Length + data2.Length];
                data1.CopyTo(data3, 0);
                data2.CopyTo(data3, data1.Length);

                newSpriteSheet.SetData(0, new Rectangle(0, 0, originalWidth, originalHeight + meatCleaverlHeight), data3, 0, data3.Length);

                asset.ReplaceWith(newSpriteSheet);

                var newToolInitialParentIndex = (originalWidth / 16) * (originalHeight / 16);

                int offset = 0;
                if (DataLoader.ModConfig.Softmode)
                {
                    offset = 7;
                }

                MeatCleaverOverrides.InitialParentTileIndex = newToolInitialParentIndex + offset;
                MeatCleaverOverrides.IndexOfMenuItemView = newToolInitialParentIndex + 26 + offset;
                InseminationSyringeOverrides.InitialParentTileIndex = newToolInitialParentIndex + 14;
                InseminationSyringeOverrides.IndexOfMenuItemView = newToolInitialParentIndex + 14;
                FeedingBasketOverrides.InitialParentTileIndex = newToolInitialParentIndex + 15;
                FeedingBasketOverrides.IndexOfMenuItemView = newToolInitialParentIndex + 15;
                ParticipantRibbonOverrides.InitialParentTileIndex = newToolInitialParentIndex + 16;
                ParticipantRibbonOverrides.IndexOfMenuItemView = newToolInitialParentIndex + 16;
                LoadMail();
            } else if (asset.AssetNameEquals("Maps\\MenuTiles"))
            {
                Texture2D menuTilesSpriteSheet = asset.AsImage().Data;
                int originalWidth = menuTilesSpriteSheet.Width;
                int originalHeight = menuTilesSpriteSheet.Height;
                Color[] data1 = new Color[originalWidth * originalHeight];
                menuTilesSpriteSheet.GetData<Color>(data1);
                Texture2D customMenuTilesSpriteSheet = _menuTilesSpriteSheet;
                int customMenuTilesWidth = customMenuTilesSpriteSheet.Width;
                int customMenuTileslHeight = customMenuTilesSpriteSheet.Height;
                Color[] data2 = new Color[customMenuTilesWidth * customMenuTileslHeight];
                customMenuTilesSpriteSheet.GetData<Color>(data2);
                Texture2D newSpriteSheet = new Texture2D(Game1.game1.GraphicsDevice, originalWidth, originalHeight + customMenuTileslHeight, false, SurfaceFormat.Color);

                var data3 = new Color[data1.Length + data2.Length];
                data1.CopyTo(data3, 0);
                data2.CopyTo(data3, data1.Length);

                newSpriteSheet.SetData(data3);

                asset.ReplaceWith(newSpriteSheet);

                var newMenuTitlesInitialParentIdex = (originalWidth / 64) * (originalHeight / 64);                

                InseminationSyringeOverrides.AttachmentMenuTile = newMenuTitlesInitialParentIdex;
                FeedingBasketOverrides.AttachmentMenuTile = newMenuTitlesInitialParentIdex + 1;
            }
        }

        internal void ReplaceOldTools()
        {
            IList<Item> inventory = Game1.player.Items;
            for (int i = 0; i < inventory.Count; i++)
            {
                ReplaceIfOldItem(inventory, i);

            }
            IList<GameLocation> locations = Game1.locations;
            for (int i = 0; i < locations.Count; i++)
            {
                var location = locations[i];
                ReplaceInLocationChests(location);

                if (location is BuildableGameLocation bgl)
                {
                    foreach(Building b in bgl.buildings)
                    {
                        if (b.indoors.Value is GameLocation gl)
                        {
                            ReplaceInLocationChests(gl);
                        }
                    };
                }
            }
        }

        private static void ReplaceInLocationChests(GameLocation location)
        {
            var objects = location.objects.Values;
            for (int j = 0; j < objects.Count(); j++)
            {
                var o = objects.ToList()[j];
                if (o is Chest chest)
                {
                    NetObjectList<Item> items = chest.items;
                    for (int k = 0; k < items.Count; k++)
                    {
                        ReplaceIfOldItem(items, k);
                    }
                }
                if (o.heldObject.Value is Chest autoGrabber)
                {
                    NetObjectList<Item> items = autoGrabber.items;
                    for (int k = 0; k < items.Count; k++)
                    {
                        ReplaceIfOldItem(items, k);
                    }
                }
            }
        }

        private static void ReplaceIfOldItem(IList<Item> items, int i)
        {
            Item item = items[i];
            if (item?.Name != null)
            {
                if (item.Name.Contains("ButcherMod.MeatCleaver") || item.Name.Contains("AnimalHusbandryMod.tools.MeatCleaver"))
                {
                    items[i] = ToolsFactory.GetMeatCleaver();
                    AnimalHusbandryModEntry.monitor.Log($"An older version of the MeatCleaver found. Replacing it with the new one.", LogLevel.Debug);
                }
                else if (item.Name.Contains("ButcherMod.tools.InseminationSyringe") || item.Name.Contains("AnimalHusbandryMod.tools.InseminationSyringe"))
                {
                    items[i] = ToolsFactory.GetInseminationSyringe();
                    AnimalHusbandryModEntry.monitor.Log($"An older version of the InseminationSyringe found. Replacing it with the new one.", LogLevel.Debug);
                }
                else if (item.Name.Contains("AnimalHusbandryMod.tools.FeedingBasket"))
                {
                    items[i] = ToolsFactory.GetFeedingBasket();
                    AnimalHusbandryModEntry.monitor.Log($"An older version of the FeedingBasket found. Replacing it with the new one.", LogLevel.Debug);
                }
                else if (item.Name.Contains("AnimalHusbandryMod.tools.ParticipantRibbon"))
                {
                    items[i] = ToolsFactory.GetParticipantRibbon();
                    AnimalHusbandryModEntry.monitor.Log($"An older version of the ParticipantRibbon found. Replacing it with the new one.", LogLevel.Debug);
                }
            }
        }

        public void LoadMail()
        {
            string meatCleaverText = DataLoader.i18n.Get(DataLoader.ModConfig.Softmode ? "Tool.MeatCleaver.Letter.Soft" : "Tool.MeatCleaver.Letter");
            string meatCleaverTitle = DataLoader.i18n.Get(DataLoader.ModConfig.Softmode ? "Tool.MeatCleaver.Letter.Soft.Title" : "Tool.MeatCleaver.Letter.Title");

            bool HasAnimal()
            {
                return Game1.locations.Any((location) =>
                {
                    if (location is Farm farm)
                    {
                        return farm.buildings
                            .Any((b => (b.indoors.Value as AnimalHouse)?.animalsThatLiveHere.Count > 0));
                    }
                    return false;
                });
            }

            bool MeatCleaverCondition(Letter l)
            {
                return !DataLoader.ModConfig.DisableMeat && !DataLoader.ModConfig.DisableMeatToolLetter && HasAnimal() && (!ItemUtility.HasModdedItem(MeatCleaverOverrides.MeatCleaverKey) || !Game1.player.mailReceived.Contains(l.Id));
            }

            List<string> validBuildingsForInsemination = new List<string>(new string[] { "Deluxe Barn", "Big Barn", "Deluxe Coop" });
            bool InseminationSyringeCondition(Letter l)
            {
                if (DataLoader.ModConfig.DisablePregnancy) return false;
                bool hasAnimalInValidBuildings = Game1.locations.Any((location) =>
                {
                    if (location is Farm farm)
                    {
                        return farm.buildings
                        .Any((b) => (b.indoors.Value as AnimalHouse)?.animalsThatLiveHere.Count > 0 && validBuildingsForInsemination.Contains(((AnimalHouse) b.indoors.Value)?.Name));
                    }
                    return false;
                });
               
                return hasAnimalInValidBuildings && (!ItemUtility.HasModdedItem(InseminationSyringeOverrides.InseminationSyringeKey) || !Game1.player.mailReceived.Contains(l.Id));
            }

            bool FeedingBasketCondition(Letter l)
            {
                return !DataLoader.ModConfig.DisableTreats && !Game1.player.mailReceived.Contains(l.Id) && Game1.player.getFriendshipHeartLevelForNPC("Marnie") >= 2 && (Game1.player.hasPet() || HasAnimal());
            }

            bool FeedingBasketRedeliveryCondition(Letter l)
            {
                return !DataLoader.ModConfig.DisableTreats && Game1.player.mailReceived.Contains("feedingBasket") && !ItemUtility.HasModdedItem(FeedingBasketOverrides.FeedingBasketKey) && Game1.player.getFriendshipHeartLevelForNPC("Marnie") >= 6;
            }

            Letter meatCleaverLetter = new Letter("meatCleaver", meatCleaverText, new List<Item> { ToolsFactory.GetMeatCleaver() }, MeatCleaverCondition, (l) => { if (!Game1.player.mailReceived.Contains(l.Id)) Game1.player.mailReceived.Add(l.Id); })
            {
                GroupId = "AHM.InterdimentionalFriend",
                Title = meatCleaverTitle
            };
            meatCleaverLetter.LetterTexture = _customLetterBG;
            meatCleaverLetter.TextColor = 4;
            MailDao.SaveLetter(meatCleaverLetter);

            Letter inseminationSyringeLetter = new Letter("inseminationSyringe", DataLoader.i18n.Get("Tool.InseminationSyringe.Letter"), new List<Item> { ToolsFactory.GetInseminationSyringe() }, InseminationSyringeCondition, (l) => { if (!Game1.player.mailReceived.Contains(l.Id)) Game1.player.mailReceived.Add(l.Id); })
            {
                GroupId = "AHM.InterdimentionalFriend",
                Title = DataLoader.i18n.Get("Tool.InseminationSyringe.Letter.Title")
            };
            inseminationSyringeLetter.LetterTexture = _customLetterBG;
            inseminationSyringeLetter.TextColor = 4;
            MailDao.SaveLetter(inseminationSyringeLetter);

            MailDao.SaveLetter
            (
                new Letter
                (
                    "participantRibbon"
                    , DataLoader.i18n.Get("Tool.ParticipantRibbon.Letter")
                    , new List<Item> { ToolsFactory.GetParticipantRibbon() }
                    , (l) => !DataLoader.ModConfig.DisableAnimalContest && SDate.Now().AddDays(1).Equals(AnimalContestController.GetNextContestDate()) && AnimalContestController.GetContestCount() == 0 && !Game1.player.mailReceived.Contains(l.Id + AnimalContestController.GetNextContestDateKey())
                    , (l) =>
                    {
                        Game1.player.mailReceived.Add(l.Id + AnimalContestController.GetNextContestDateKey());
                        if (!Game1.player.mailReceived.Contains(l.Id)) Game1.player.mailReceived.Add(l.Id);
                    })
                {
                    Title = DataLoader.i18n.Get("Tool.ParticipantRibbon.Letter.Title")
                }
            );
            MailDao.SaveLetter
            (
                new Letter
                (
                    "participantRibbonRedelivery"
                    , DataLoader.i18n.Get("Tool.ParticipantRibbon.LetterRedelivery")
                    , new List<Item> { ToolsFactory.GetParticipantRibbon() }
                    , (l) => !DataLoader.ModConfig.DisableAnimalContest && SDate.Now().AddDays(1).Equals(AnimalContestController.GetNextContestDate()) && AnimalContestController.GetContestCount() > 0 && !Game1.player.mailReceived.Contains(l.Id + AnimalContestController.GetNextContestDateKey())
                    , (l) =>
                    {
                        Game1.player.mailReceived.Add(l.Id + AnimalContestController.GetNextContestDateKey());
                        if (!Game1.player.mailReceived.Contains(l.Id)) Game1.player.mailReceived.Add(l.Id);
                    })
                {
                    Title = DataLoader.i18n.Get("Tool.ParticipantRibbon.LetterRedelivery.Title")
                }
            );

            MailDao.SaveLetter
            (
                new Letter
                (
                    "feedingBasket",
                    DataLoader.i18n.Get("Tool.FeedingBasket.Letter"),
                    new List<Item> {ToolsFactory.GetFeedingBasket()},
                    FeedingBasketCondition,
                    (l) => { if (!Game1.player.mailReceived.Contains(l.Id)) Game1.player.mailReceived.Add(l.Id); }
                )
                {
                    Title = DataLoader.i18n.Get("Tool.FeedingBasket.Letter.Title")
                }
            );
            MailDao.SaveLetter
            (
                new Letter
                (
                    "feedingBasketRedelivery",
                    DataLoader.i18n.Get("Tool.FeedingBasket.LetterRedelivery"),
                    new List<Item> { ToolsFactory.GetFeedingBasket() },
                    FeedingBasketRedeliveryCondition,
                    (l) => { if (!Game1.player.mailReceived.Contains(l.Id)) Game1.player.mailReceived.Add(l.Id); }
                )
                {
                    Title = DataLoader.i18n.Get("Tool.FeedingBasket.LetterRedelivery.Title")
                }
            );
        }

        public void RemoveAllToolsCommand(string n, string[] d)
        {
            ItemUtility.RemoveModdedItemAnywhere(MeatCleaverOverrides.MeatCleaverKey);
            ItemUtility.RemoveModdedItemAnywhere(InseminationSyringeOverrides.InseminationSyringeKey);
            ItemUtility.RemoveModdedItemAnywhere(FeedingBasketOverrides.FeedingBasketKey);
            ItemUtility.RemoveModdedItemAnywhere(ParticipantRibbonOverrides.ParticipantRibbonKey);
        }
    }
}
