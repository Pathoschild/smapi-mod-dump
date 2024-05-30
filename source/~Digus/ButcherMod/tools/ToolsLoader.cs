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
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.Tools;
using StardewValley.Inventories;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;
using DataLoader = AnimalHusbandryMod.common.DataLoader;

namespace AnimalHusbandryMod.tools
{
    public class ToolsLoader
    {
        private readonly string _toolsSpriteName = "Mods/DIGUS.ANIMALHUSBANDRYMOD/Tools";
        public readonly string MenuTilesSpriteName = "Mods/DIGUS.ANIMALHUSBANDRYMOD/MenuTiles";
        private readonly string _customLetterBGName = "Mods/DIGUS.ANIMALHUSBANDRYMOD/customLetterBG";
        private readonly Texture2D _customLetterBG;
        public readonly Texture2D MenuTilesSprites;


        public ToolsLoader(IModHelper helper)
        {

            if (DataLoader.ModConfig.Softmode)
            {
                MeatCleaverOverrides.Suffix = ".Soft";
            }

            helper.Events.Content.AssetRequested += this.Edit;
            helper.GameContent.Load<Texture2D>(_toolsSpriteName);
            MenuTilesSprites = helper.GameContent.Load<Texture2D>(MenuTilesSpriteName);
            _customLetterBG = helper.GameContent.Load<Texture2D>(_customLetterBGName);
            DataLoader.ToolsSprites = DataLoader.Helper.GameContent.Load<Texture2D>(_toolsSpriteName);
            
        }

        public void Edit(object sender, AssetRequestedEventArgs args)
        {
            if (args.Name.IsEquivalentTo(_toolsSpriteName))
            {
                args.LoadFromModFile<Texture2D>("tools/Tools.png",AssetLoadPriority.High);
                    
            }
            else if (args.Name.IsEquivalentTo(MenuTilesSpriteName))
            {
                args.LoadFromModFile<Texture2D>("tools/MenuTiles.png", AssetLoadPriority.High);

            }
            else if (args.Name.IsEquivalentTo(_customLetterBGName))
            {
                args.LoadFromModFile<Texture2D>("common/CustomLetterBG.png", AssetLoadPriority.High);

            }
            else if (args.Name.IsEquivalentTo("Data/Tools"))
            {
                ToolData MeatCleaverData = new ToolData()
                {
                    ClassName = "GenericTool",
                    Name = "MeatCleaver",
                    DisplayName = DataLoader.i18n.Get("Tool.MeatCleaver.Name" + MeatCleaverOverrides.Suffix),
                    Description = DataLoader.i18n.Get("Tool.MeatCleaver.Description" + MeatCleaverOverrides.Suffix),
                    Texture = _toolsSpriteName,
                    MenuSpriteIndex = 26,
                    SpriteIndex = 0,
                    ModData = new Dictionary<string, string> { { MeatCleaverOverrides.MeatCleaverKey, Game1.random.Next().ToString() } }
                };

                ToolData InseminationSyringeData = new ToolData()
                {
                    ClassName = "GenericTool",
                    Name = "InseminationSyringe",
                    DisplayName = DataLoader.i18n.Get("Tool.InseminationSyringe.Name"),
                    Description = DataLoader.i18n.Get("Tool.InseminationSyringe.Description"),
                    Texture = _toolsSpriteName,
                    MenuSpriteIndex = 14,
                    SpriteIndex = 14,
                    AttachmentSlots = 1,
                    ModData = new Dictionary<string, string> { { InseminationSyringeOverrides.InseminationSyringeKey, Game1.random.Next().ToString() } }
                };

                ToolData FeedingBasketData = new ToolData()
                {
                    ClassName = "GenericTool",
                    Name = "FeedingBasket",
                    DisplayName = DataLoader.i18n.Get("Tool.FeedingBasket.Name"),
                    Description = DataLoader.i18n.Get("Tool.FeedingBasket.Description"),
                    Texture = _toolsSpriteName,
                    MenuSpriteIndex = 15,
                    SpriteIndex = 15,
                    AttachmentSlots = 1,
                    ModData = new Dictionary<string, string> { { FeedingBasketOverrides.FeedingBasketKey, Game1.random.Next().ToString() } }
                };

                ToolData ParticipantRibbonData = new ToolData()
                {
                    ClassName = "GenericTool",
                    Name = "ParticipantRibbon",
                    DisplayName = DataLoader.i18n.Get("Tool.ParticipantRibbon.Name"),
                    Description = DataLoader.i18n.Get("Tool.ParticipantRibbon.Description"),
                    Texture = _toolsSpriteName,
                    MenuSpriteIndex = 16,
                    SpriteIndex = 16,
                    ModData = new Dictionary<string, string> { { ParticipantRibbonOverrides.ParticipantRibbonKey, Game1.random.Next().ToString() } }
                };

                args.Edit(asset => {
                    var toolDatas = asset.AsDictionary<string, ToolData>().Data;
                    
                    toolDatas[MeatCleaverOverrides.MeatCleaverItemId] = MeatCleaverData;
                    toolDatas[InseminationSyringeOverrides.InseminationSyringeItemId] = InseminationSyringeData;
                    toolDatas[FeedingBasketOverrides.FeedingBasketItemId] = FeedingBasketData;
                    toolDatas[ParticipantRibbonOverrides.ParticipantRibbonItemId] = ParticipantRibbonData;
                });

                if (DataLoader.ModConfig.Softmode)
                {
                    MeatCleaverData.SpriteIndex += 7;
                    MeatCleaverData.MenuSpriteIndex += 7;
                }
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

                if (location.IsBuildableLocation())
                {
                    foreach(Building b in location.buildings)
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
                    Inventory items = chest.Items;
                    for (int k = 0; k < items.Count; k++)
                    {
                        ReplaceIfOldItem(items, k);
                    }
                }
                if (o.heldObject.Value is Chest autoGrabber)
                {
                    Inventory items = autoGrabber.Items;
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
                if (item.Name.Contains("ButcherMod.MeatCleaver") || item.Name.Contains("AnimalHusbandryMod.tools.MeatCleaver") || (item is Axe && item.Name.Equals("Meat Cleaver")))
                {
                    items[i] = ToolsFactory.GetMeatCleaver();
                    AnimalHusbandryModEntry.monitor.Log($"An older version of the MeatCleaver found. Replacing it with the new one.", LogLevel.Debug);
                }
                else if (item.Name.Contains("ButcherMod.tools.InseminationSyringe") || item.Name.Contains("AnimalHusbandryMod.tools.InseminationSyringe") || (item is MilkPail && item.Name.Equals("Insemination Syringe")))
                {
                    items[i] = ToolsFactory.GetInseminationSyringe();
                    AnimalHusbandryModEntry.monitor.Log($"An older version of the InseminationSyringe found. Replacing it with the new one.", LogLevel.Debug);
                }
                else if (item.Name.Contains("AnimalHusbandryMod.tools.FeedingBasket") || (item is MilkPail && item.Name.Equals("Feeding Basket")))
                {
                    items[i] = ToolsFactory.GetFeedingBasket();
                    AnimalHusbandryModEntry.monitor.Log($"An older version of the FeedingBasket found. Replacing it with the new one.", LogLevel.Debug);
                }
                else if (item.Name.Contains("AnimalHusbandryMod.tools.ParticipantRibbon") || (item is MilkPail && item.Name.Equals("Participant Ribbon")))
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
                if (DataLoader.ModConfig.DisablePregnancy || DataLoader.ModConfig.DisableInseminationSyringeLetter) return false;
                bool hasAnimalInValidBuildings = Game1.locations.Any((location) =>
                {
                    if (location is Farm farm)
                    {
                        return farm.buildings
                        .Any((b) => {
                            return (b.indoors.Value as AnimalHouse)?.animalsThatLiveHere.Count > 0 && validBuildingsForInsemination.Contains(((AnimalHouse)b.indoors.Value)?.Name);
                            });
                    }
                    return false;
                });
               
                return hasAnimalInValidBuildings && (!ItemUtility.HasModdedItem(InseminationSyringeOverrides.InseminationSyringeKey) || !Game1.player.mailReceived.Contains(l.Id));
            }

            bool FeedingBasketCondition(Letter l)
            {
                return !DataLoader.ModConfig.DisableTreats && !DataLoader.ModConfig.DisableFeedingBasketLetter && !Game1.player.mailReceived.Contains(l.Id) && Game1.player.getFriendshipHeartLevelForNPC("Marnie") >= 2 && (Game1.player.hasPet() || HasAnimal());
            }

            bool FeedingBasketRedeliveryCondition(Letter l)
            {
                return !DataLoader.ModConfig.DisableTreats && !DataLoader.ModConfig.DisableFeedingBasketLetter && Game1.player.mailReceived.Contains("feedingBasket") && !ItemUtility.HasModdedItem(FeedingBasketOverrides.FeedingBasketKey) && Game1.player.getFriendshipHeartLevelForNPC("Marnie") >= 6;
            }

            Letter meatCleaverLetter = new Letter("meatCleaver", meatCleaverText, MeatCleaverCondition, (l) => { if (!Game1.player.mailReceived.Contains(l.Id)) Game1.player.mailReceived.Add(l.Id); })
            {
                GroupId = "AHM.InterdimentionalFriend",
                Title = meatCleaverTitle,
                DynamicItems = (l) => new List<Item> { ToolsFactory.GetMeatCleaver() }
            };
            meatCleaverLetter.LetterTexture = _customLetterBG;
            meatCleaverLetter.TextColor = 4;
            MailRepository.SaveLetter(meatCleaverLetter);

            Letter inseminationSyringeLetter = new Letter("inseminationSyringe", DataLoader.i18n.Get("Tool.InseminationSyringe.Letter"), InseminationSyringeCondition, (l) => { if (!Game1.player.mailReceived.Contains(l.Id)) Game1.player.mailReceived.Add(l.Id); })
            {
                GroupId = "AHM.InterdimentionalFriend",
                Title = DataLoader.i18n.Get("Tool.InseminationSyringe.Letter.Title"),
                DynamicItems = (l) => new List<Item> { ToolsFactory.GetInseminationSyringe() }
            };
            inseminationSyringeLetter.LetterTexture = _customLetterBG;
            inseminationSyringeLetter.TextColor = 4;
            MailRepository.SaveLetter(inseminationSyringeLetter);

            MailRepository.SaveLetter
            (
                new Letter
                (
                    "participantRibbon"
                    , DataLoader.i18n.Get("Tool.ParticipantRibbon.Letter")
                    , (l) => !DataLoader.ModConfig.DisableAnimalContest && SDate.Now().AddDays(1).Equals(AnimalContestController.GetNextContestDate()) && AnimalContestController.GetContestCount() == 0 && !Game1.player.mailReceived.Contains(l.Id + AnimalContestController.GetNextContestDateKey())
                    , (l) =>
                    {
                        Game1.player.mailReceived.Add(l.Id + AnimalContestController.GetNextContestDateKey());
                        if (!Game1.player.mailReceived.Contains(l.Id)) Game1.player.mailReceived.Add(l.Id);
                    })
                {
                    Title = DataLoader.i18n.Get("Tool.ParticipantRibbon.Letter.Title"),
                    DynamicItems = (l) => new List<Item> { ToolsFactory.GetParticipantRibbon() }
                }
            );
            MailRepository.SaveLetter
            (
                new Letter
                (
                    "participantRibbonRedelivery"
                    , DataLoader.i18n.Get("Tool.ParticipantRibbon.LetterRedelivery")
                    , (l) => !DataLoader.ModConfig.DisableAnimalContest && SDate.Now().AddDays(1).Equals(AnimalContestController.GetNextContestDate()) && AnimalContestController.GetContestCount() > 0 && !Game1.player.mailReceived.Contains(l.Id + AnimalContestController.GetNextContestDateKey())
                    , (l) =>
                    {
                        Game1.player.mailReceived.Add(l.Id + AnimalContestController.GetNextContestDateKey());
                        if (!Game1.player.mailReceived.Contains(l.Id)) Game1.player.mailReceived.Add(l.Id);
                    })
                {
                    Title = DataLoader.i18n.Get("Tool.ParticipantRibbon.LetterRedelivery.Title"),
                    DynamicItems = (l) => new List<Item> { ToolsFactory.GetParticipantRibbon() }
                }
            );

            MailRepository.SaveLetter
            (
                new Letter
                (
                    "feedingBasket",
                    DataLoader.i18n.Get("Tool.FeedingBasket.Letter"),
                    FeedingBasketCondition,
                    (l) => { if (!Game1.player.mailReceived.Contains(l.Id)) Game1.player.mailReceived.Add(l.Id); }
                )
                {
                    Title = DataLoader.i18n.Get("Tool.FeedingBasket.Letter.Title"),
                    DynamicItems = (l) => new List<Item> { ToolsFactory.GetFeedingBasket() }
                }
            );
            MailRepository.SaveLetter
            (
                new Letter
                (
                    "feedingBasketRedelivery",
                    DataLoader.i18n.Get("Tool.FeedingBasket.LetterRedelivery"),
                    FeedingBasketRedeliveryCondition,
                    (l) => { if (!Game1.player.mailReceived.Contains(l.Id)) Game1.player.mailReceived.Add(l.Id); }
                )
                {
                    Title = DataLoader.i18n.Get("Tool.FeedingBasket.LetterRedelivery.Title"),
                    DynamicItems = (l) => new List<Item> { ToolsFactory.GetFeedingBasket() }
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
