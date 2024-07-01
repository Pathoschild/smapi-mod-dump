/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Network;
using System.Reflection;
using Unlockable_Bundles.NetLib;
using StardewModdingAPI;
using Unlockable_Bundles.Lib.Enums;
using Unlockable_Bundles.Lib.ShopTypes;
using StardewValley.Menus;
using Unlockable_Bundles.Lib.AdvancedPricing;
using StardewValley.GameData;
using StardewValley.Internal;
using static Unlockable_Bundles.ModEntry;
using Unlockable_Bundles.Lib.WalletCurrency;

namespace Unlockable_Bundles.Lib
{
    public sealed class Unlockable : INetObject<NetFields>
    {
        public NetFields NetFields { get; } = new NetFields("DLX.Bundles/Unlockable");

        private NetString _id = new();
        private NetString _location = new();
        private NetString _locationUnique = new();

        private NetString _bundleName = new();
        private NetString _bundleDescription = new();
        private NetEnum<BundleIconType> _bundleIcon = new();
        private NetString _bundleIconAsset = new();
        private NetInt _bundleSlots = new NetInt();
        private NetString _junimoNoteTexture = new();
        private NetString _bundleCompletedMail = new();

        private NetVector2 _shopPosition = new();
        public NetList<Vector2, NetVector2> _alternativeShopPositions = new();
        private NetString _shopTexture = new();
        private NetString _shopAnimation = new();
        private NetInt _shopTextureWidth = new();
        private NetInt _shopTextureHeight = new();
        private NetVector2 _shopDrawDimensions = new();
        private NetVector2 _shopDrawOffset = new();
        private NetString _shopEvent = new();
        private NetEnum<ShopType> _shopType = new();
        private NetBool _instantShopRemoval = new();
        private NetColor _shopColor = new();

        private NetBool _drawQuestionMark = new();
        private NetVector2 _questionMarkOffset = new();
        private NetVector2 _speechBubbleOffset = new();
        private NetRectangle _parrotTarget = new();
        private NetFloat _timeUntilChomp = new();
        private NetInt _parrotIndex = new();
        private NetString _parrotTexture = new();

        private NetBool _interactionShake = new();
        private NetString _interactionTexture = new();
        private NetString _interactionAnimation = new();
        private NetString _interactionSound = new();

        private NetString _overviewTexture = new();
        private NetString _overviewAnimation = new();
        private NetInt _overviewTextureWidth = new();
        private NetString _overviewDescription = new();
        private NetColor _overviewColor = new();

        public NetInt _randomPriceEntries = new();
        public NetInt _randomRewardEntries = new();
        public NetStringIntDictionary _price = new();
        public NetStringStringDictionary _priceMigration = new();
        public NetStringIntDictionary _alreadyPaid = new();
        public NetStringIntDictionary _alreadyPaidIndex = new();
        public NetStringIntDictionary _bundleReward = new();

        private NetString _editMap = new();
        private NetEnum<PatchMapMode> _editMapMode = new();
        private NetVector2 _editMapPosition = new();
        private NetString _editMapLocation = new();

        public NetBool _completed = new NetBool(false); //This value is currently only valid from the moment a bundle was purchased till end of day
        public string ID { get => _id.Value; set => _id.Value = value; }
        public string Location { get => _location.Value; set => _location.Value = value; }
        public string LocationUnique { get => _locationUnique.Value; set => _locationUnique.Value = value; }

        public string BundleName { get => _bundleName.Value; set => _bundleName.Value = value; }
        public string BundleDescription { get => _bundleDescription.Value; set => _bundleDescription.Value = value; }
        public BundleIconType BundleIcon { get => _bundleIcon.Value; set => _bundleIcon.Value = value; }
        public string BundleIconAsset { get => _bundleIconAsset.Value; set => _bundleIconAsset.Value = value; }
        public int BundleSlots { get => _bundleSlots.Value; set => _bundleSlots.Value = value; }
        public string JunimoNoteTexture { get => _junimoNoteTexture.Value; set => _junimoNoteTexture.Value = value; }
        public string BundleCompletedMail { get => _bundleCompletedMail.Value; set => _bundleCompletedMail.Value = value; }

        public Vector2 ShopPosition { get => _shopPosition.Value; set => _shopPosition.Value = value; }
        public List<Vector2> PossibleShopPositions { get { var ret = new List<Vector2>() { ShopPosition }; ret.AddRange(_alternativeShopPositions.ToList()); return ret; } }
        public string ShopTexture { get => _shopTexture.Value; set => _shopTexture.Value = value; }
        public string ShopAnimation { get => _shopAnimation.Value; set => _shopAnimation.Value = value; }
        public int ShopTextureWidth { get => _shopTextureWidth.Value; set => _shopTextureWidth.Value = value; }
        public int ShopTextureHeight { get => _shopTextureHeight.Value; set => _shopTextureHeight.Value = value; }
        public Vector2 ShopDrawDimensions { get => _shopDrawDimensions.Value; set => _shopDrawDimensions.Value = value; }
        public Vector2 ShopDrawOffset { get => _shopDrawOffset.Value; set => _shopDrawOffset.Value = value; }
        public string ShopEvent { get => _shopEvent.Value; set => _shopEvent.Value = value; }
        public ShopType ShopType { get => _shopType.Value; set => _shopType.Value = value; }
        public bool InstantShopRemoval { get => _instantShopRemoval.Value; set => _instantShopRemoval.Value = value; }
        public Color ShopColor { get => _shopColor.Value; set => _shopColor.Value = value; }

        public bool DrawQuestionMark { get => _drawQuestionMark.Value; set => _drawQuestionMark.Value = value; }
        public Vector2 QuestionMarkOffset { get => _questionMarkOffset.Value; set => _questionMarkOffset.Value = value; }
        public Vector2 SpeechBubbleOffset { get => _speechBubbleOffset.Value; set => _speechBubbleOffset.Value = value; }
        public Rectangle ParrotTarget { get => _parrotTarget.Value; set => _parrotTarget.Value = value; }
        public float TimeUntilChomp { get => _timeUntilChomp.Value; set => _timeUntilChomp.Value = value; }
        public int ParrotIndex { get => _parrotIndex.Value; set => _parrotIndex.Value = value; }
        public string ParrotTexture { get => _parrotTexture.Value; set => _parrotTexture.Value = value; }

        public bool InteractionShake { get => _interactionShake.Value; set => _interactionShake.Value = value; }
        public string InteractionTexture { get => _interactionTexture.Value; set => _interactionTexture.Value = value; }
        public string InteractionAnimation { get => _interactionAnimation.Value; set => _interactionAnimation.Value = value; }
        public string InteractionSound { get => _interactionSound.Value; set => _interactionSound.Value = value; }

        public string OverviewTexture { get => _overviewTexture.Value; set => _overviewTexture.Value = value; }
        public string OverviewAnimation { get => _overviewAnimation.Value; set => _overviewAnimation.Value = value; }
        public int OverviewTextureWidth { get => _overviewTextureWidth.Value; set => _overviewTextureWidth.Value = value; }
        public string OverviewDescription { get => _overviewDescription.Value; set => _overviewDescription.Value = value; }
        public Color OverviewColor { get => _overviewColor.Value; set => _overviewColor.Value = value; }

        public int RandomPriceEntries { get => _randomPriceEntries.Value; set => _randomPriceEntries.Value = value; }
        public int RandomRewardEntries { get => _randomRewardEntries.Value; set => _randomRewardEntries.Value = value; }
        public string EditMap { get => _editMap.Value; set => _editMap.Value = value; }
        public PatchMapMode EditMapMode { get => _editMapMode.Value; set => _editMapMode.Value = value; }
        public Vector2 EditMapPosition { get => _editMapPosition.Value; set => _editMapPosition.Value = value; }
        public string EditMapLocation { get => _editMapLocation.Value; set => _editMapLocation.Value = value; }

        public List<PlacementRequirement> SpecialPlacementRequirements = new(); //Not a NetField, only relevant for host

        private string CachedLocalizedShopDescription = null;
        private string CachedLocalizedOverviewDescription = null;
        public static Dictionary<string, string> CachedJsonAssetIDs = new Dictionary<string, string>();
        public static bool ShowDebugNames = false;

        private Dictionary<string, List<Item>> RequiredItems = new();
        private Dictionary<string, List<Item>> RequiredItemsWithExceptions = new();
        public Unlockable(UnlockableModel model)
        {
            this.ID = model.ID;
            this.Location = model.Location;
            this.LocationUnique = model.LocationUnique;

            this.BundleName = model.BundleName;
            this.BundleDescription = model.BundleDescription;
            this.BundleIcon = model.BundleIcon;
            this.BundleIconAsset = model.BundleIconAsset;
            this.BundleSlots = model.BundleSlots;
            this.JunimoNoteTexture = model.JunimoNoteTexture;
            this.BundleCompletedMail = model.BundleCompletedMail;

            this.ShopPosition = model.ShopPosition;
            this._alternativeShopPositions = new(model.AlternativeShopPositions);
            this.ShopTexture = model.ShopTexture;
            this.ShopAnimation = model.ShopAnimation;
            this.ShopTextureWidth = (int)model.ShopTextureWidth;
            this.ShopTextureHeight = (int)model.ShopTextureHeight;
            this.ShopDrawDimensions = model.ShopDrawDimensions;
            this.ShopDrawOffset = model.ShopDrawOffset;
            this.ShopEvent = model.ShopEvent;
            this.ShopType = model.ShopType;
            this.InstantShopRemoval = model.InstantShopRemoval == true;
            this.ShopColor = model.parseColor();

            this.DrawQuestionMark = model.DrawQuestionMark;
            this.QuestionMarkOffset = model.QuestionMarkOffset;
            this.SpeechBubbleOffset = model.SpeechBubbleOffset;
            this.ParrotTarget = model.ParrotTarget;
            this.TimeUntilChomp = model.TimeUntilChomp;
            this.ParrotIndex = model.ParrotIndex;
            this.ParrotTexture = model.ParrotTexture;

            this.InteractionShake = model.InteractionShake == true;
            this.InteractionTexture = model.InteractionTexture;
            this.InteractionAnimation = model.InteractionAnimation;
            this.InteractionSound = model.InteractionSound;

            this.OverviewTexture = model.OverviewTexture;
            this.OverviewAnimation = model.OverviewAnimation;
            this.OverviewTextureWidth = model.OverviewTextureWidth;
            this.OverviewDescription = model.OverviewDescription;
            this.OverviewColor = model.parseOverviewColor();

            this.RandomPriceEntries = model.RandomPriceEntries;
            this.RandomRewardEntries = model.RandomRewardEntries;
            this._price = new NetStringIntDictionary(model.Price);
            this._priceMigration = new NetStringStringDictionary(model.PriceMigration);
            this._alreadyPaid = new NetStringIntDictionary(model.AlreadyPaid);
            this._alreadyPaidIndex = new NetStringIntDictionary(model.AlreadyPaidIndex);
            this._bundleReward = new NetStringIntDictionary(model.BundleReward);

            this.EditMap = model.EditMap;
            this.EditMapMode = model.EditMapMode;
            this.EditMapPosition = model.EditMapPosition;
            this.EditMapLocation = model.EditMapLocation;
            addNetFields();

            this.SpecialPlacementRequirements = PlacementRequirement.CloneList(model.SpecialPlacementRequirements);
        }

        public Unlockable() => addNetFields();

        private void addNetFields() => NetFields.SetOwner(this)
            .AddField(_id, "_id")
            .AddField(_location, "_location")
            .AddField(_locationUnique, "_locationUnique")

            .AddField(_bundleName, "_bundleName")
            .AddField(_bundleDescription, "_bundleDescription")
            .AddField(_bundleIcon, "_bundleIcon")
            .AddField(_bundleIconAsset, "_bundleIconAsset")
            .AddField(_bundleSlots, "_bundleSlots")
            .AddField(_junimoNoteTexture, "_junimoNoteTexture")
            .AddField(_bundleCompletedMail, "_bundleCompletedMail")

            .AddField(_shopPosition, "_shopPosition")
            .AddField(_alternativeShopPositions, "alternativeShopPositions")
            .AddField(_shopTexture, "_shopTexture")
            .AddField(_shopAnimation, "_shopAnimation")
            .AddField(_shopTextureWidth, "_shopTextureWidth")
            .AddField(_shopTextureHeight, "_shopTextureHeight")
            .AddField(_shopDrawDimensions, "_shopDrawDimensions")
            .AddField(_shopDrawOffset, "_shopDrawOffset")
            .AddField(_shopEvent, "_shopEvent")
            .AddField(_shopType, "_shopType")
            .AddField(_instantShopRemoval, "_instantShopRemoval")
            .AddField(_shopColor, "_shopColor")

            .AddField(_drawQuestionMark, "_drawQuestionMark")
            .AddField(_questionMarkOffset, "_questionMarkOffset")
            .AddField(_speechBubbleOffset, "_speechBubbleOffset")
            .AddField(_parrotTarget, "_parrotTarget")
            .AddField(_timeUntilChomp, "_timeUntilChomp")
            .AddField(_parrotIndex, "_parrotIndex")
            .AddField(_parrotTexture, "_parrotTexture")

            .AddField(_interactionShake, "_interactionShake")
            .AddField(_interactionTexture, "_interactionTexture")
            .AddField(_interactionAnimation, "_interactionAnimation")
            .AddField(_interactionSound, "_interactionSound")

            .AddField(_overviewTexture, "_overviewTexture")
            .AddField(_overviewAnimation, "_overviewAnimation")
            .AddField(_overviewTextureWidth, "_overviewTextureWidth")
            .AddField(_overviewDescription, "overviewDescription")
            .AddField(_overviewColor, "overviewColor")

            .AddField(_randomPriceEntries, "_randomPriceEntries")
            .AddField(_randomRewardEntries, "_randomRewardEntries")
            .AddField(_price, "_price")
            .AddField(_priceMigration, "_priceMigration")
            .AddField(_alreadyPaid, "_alreadyPaid")
            .AddField(_alreadyPaidIndex, "_alreadyPaidIndex")
            .AddField(_bundleReward, "_bundleReward")

            .AddField(_editMap, "_editMap")
            .AddField(_editMapMode, "_editMapMode")
            .AddField(_editMapPosition, "_editMapPosition")
            .AddField(_editMapLocation, "_editMapLocation");

        public static Dictionary<string, Unlockable> convertModelDicToEntity(Dictionary<string, UnlockableModel> modelDic)
        {
            var entityDic = new Dictionary<string, Unlockable>();
            foreach (var entry in modelDic) {
                entry.Value.ID = entry.Key;
                entityDic.Add(entry.Key, new Unlockable(entry.Value));
            }

            return entityDic;
        }

        public GameLocation getGameLocation()
        {
            var isStruct = Location != LocationUnique;

            if (Location == "FarmHouse" || Location == "Cellar")
                isStruct = true;

            return Game1.getLocationFromName(LocationUnique, isStruct);
        }

        public string getTranslatedShopDescription()
        {
            if (Context.IsOnHostComputer)
                return BundleDescription;

            if (CachedLocalizedShopDescription != null)
                return CachedLocalizedShopDescription;

            var unlockables = Helper.GameContent.Load<Dictionary<string, UnlockableModel>>("UnlockableBundles/Bundles");
            if (unlockables == null)
                return BundleDescription;

            if (unlockables.TryGetValue(ID, out var unlockable)) {
                unlockable.applyDefaultValues();
                CachedLocalizedShopDescription = unlockable.BundleDescription;

                return unlockable.BundleDescription;
            }

            CachedLocalizedShopDescription = BundleDescription;
            return BundleDescription;
        }

        public string getTranslatedOverviewDescription()
        {
            if (OverviewDescription is null)
                return getTranslatedShopDescription();

            if (Context.IsOnHostComputer)
                return OverviewDescription;

            if (CachedLocalizedOverviewDescription != null)
                return CachedLocalizedOverviewDescription;

            var unlockables = Helper.GameContent.Load<Dictionary<string, UnlockableModel>>("UnlockableBundles/Bundles");
            if (unlockables == null)
                return OverviewDescription;

            if (unlockables.TryGetValue(ID, out var unlockable)) {
                CachedLocalizedOverviewDescription = unlockable.OverviewDescription;

                return CachedLocalizedOverviewDescription;
            }

            CachedLocalizedOverviewDescription = OverviewDescription;
            return OverviewDescription;
        }

        public static int getQualityFromReqSplit(string key)
        {
            if (!key.Contains(":"))
                return 0;

            return key.Split(":").Last().ToLower().Trim() switch {
                "iridium" => 4,
                "gold" => 2,
                "silver" => 1,
                "" => 0,
                _ => -1
            };
        }
        public static bool isExceptionItem(string id) => id.ToLower().Trim() == "money" || id == "(O)858" || id == "(O)73" || WalletCurrencyHandler.getCurrencyItemMatch(id, out _, out _, out _);
        public static string getIDFromReqSplit(string key)
        {
            var id = key.Split(":").First().Trim();

            if (id.ToLower() == "money")
                return "money";

            if (id.First() != '(')
                id = "(O)" + id;

            //(S)10 can become (P)10 at ItemRegistry.Create.. so yeah
            var item = parseItem(id);

            return item.QualifiedItemId;
        }

        public static string getFirstIDFromReqKey(string reqKey) => getIDFromReqSplit(reqKey.Split(",").First().Trim());
        public static int getFirstQualityFromReqKey(string reqKey) => getQualityFromReqSplit(reqKey.Split(",").First());

        public string getMailKey() => getMailKey(ID);
        public static string getMailKey(string id) => "DLX.Bundles." + id.Replace("/", ".").Replace(" ", "_");
        public void processPurchase()
        {
            _completed.Value = true;
            ModData.setPurchased(ID, LocationUnique);
            ModAPI.raiseShopPurchased(new API.BundlePurchasedEventArgs(Game1.player, Location, LocationUnique, ID, true));
            Helper.Multiplayer.SendMessage((UnlockableModel)this, "BundlePurchased", modIDs: new[] { ModManifest.UniqueID });
            Helper.Reflection.GetField<StardewValley.Multiplayer>(typeof(Game1), "multiplayer").GetValue().globalChatInfoMessage("Bundle");

            Game1.addMailForTomorrow(getMailKey(), noLetter: BundleCompletedMail == "", sendToEveryone: true);
        }

        public void processContribution(KeyValuePair<string, int> requirement, int index = -1)
        {
            _alreadyPaid.Add(requirement.Key, requirement.Value);
            _alreadyPaidIndex.Add(requirement.Key, index);
            ModData.setPartiallyPurchased(ID, LocationUnique, requirement.Key, requirement.Value, index);
            ModAPI.raiseShopContributed(new API.BundleContributedEventArgs(Game1.player, new KeyValuePair<string, int>(requirement.Key, requirement.Value), Location, LocationUnique, ID, true));
            Helper.Multiplayer.SendMessage((UnlockableModel)this, "BundleContributed", modIDs: new[] { ModManifest.UniqueID });
        }

        public void processShopEvent()
        {
            Task.Delay(800).ContinueWith(t => {
                if (InstantShopRemoval)
                    ShopPlacement.removeShop(this);

                if (Context.IsMainPlayer)
                    PlacementRequirement.CheckShopPlacement(PlacementRequirementType.BundleCompletion);
                else
                    Helper.Multiplayer.SendMessage(new KeyValuePair<PlacementRequirementType, string>(PlacementRequirementType.BundleCompletion, ""), "SPRUpdated", modIDs: new[] { ModManifest.UniqueID }, playerIDs: new[] { Game1.MasterPlayer.UniqueMultiplayerID});
            });

            if (ShopEvent.ToLower() != "none" && Game1.activeClickableMenu != null) {
                Game1.dialogueUp = false;
                Game1.activeClickableMenu.exitThisMenu(false);
            }

            Game1.player.completelyStopAnimatingOrDoingAction();

            if (ShopEvent.ToLower() == "carpentry")
                Game1.globalFadeToBlack(playPurchasedEvent);
            else if (ShopEvent.ToLower() == "none") {
                MapPatches.applyUnlockable(this);
                openRewardsMenu();
                return;
            } else {
                if (!ShopEvent.ToLower().Contains(UBEvent.APPLYPATCH.ToLower()))
                    MapPatches.applyUnlockable(this);
                var ev = new UBEvent(this, ShopEvent, Game1.player);
                ev.onEventFinished = openRewardsMenu;
                Game1.globalFadeToBlack(() => Game1.player.currentLocation.startEvent(ev));
            }
        }
        private void openRewardsMenu()
        {
            List<Item> rewards = new List<Item>();

            foreach (var reward in _bundleReward.Pairs) {
                var id = getIDFromReqSplit(reward.Key);
                var quality = getQualityFromReqSplit(reward.Key);

                if (Inventory.addExceptionItem(Game1.player, id, reward.Value))
                    continue;

                var item = parseItem(id, reward.Value, quality: quality);
                if (item is AdvancedPricingItem apItem) {
                    if (apItem.UsesFlavoredSyntax) {
                        apItem.ItemCopy.Quality = quality;
                        apItem.ItemCopy.Stack = reward.Value;
                        rewards.Add(apItem.ItemCopy);

                    } else
                        Monitor.Log($"BundleReward does not accept advanced pricing syntax apart from auto generated flavored Items! Please fix the following itemID: {id}", LogLevel.Error);

                } else
                    rewards.Add(item);
            }

            rewards.AddRange(getRewardSpawnFieldItems());

            if (RandomRewardEntries > 0)
                rewards = rewards.OrderBy(x => Game1.random.Next()).Take(RandomRewardEntries).ToList();

            if (rewards.Count == 0)
                return;

            Game1.playSound("smallSelect");
            var grabMenu = new ItemGrabMenu(rewards, reverseGrab: false, showReceivingMenu: true, null, null, null, null, snapToBottom: false, canBeExitedWithKey: true, playRightClickSound: true, allowRightClick: true, showOrganizeButton: false, 0, null, -1, this);
            grabMenu.behaviorBeforeCleanup = (Action<IClickableMenu>)Delegate.Combine(grabMenu.behaviorBeforeCleanup, (Action<IClickableMenu>)delegate {
                grabMenu.DropRemainingItems();
                Game1.dialogueUp = false;
                Game1.player.CanMove = true;
            });
            if (Game1.activeClickableMenu != null)
                Game1.activeClickableMenu.exitThisMenu();
            Game1.activeClickableMenu = grabMenu;
        }

        public void playPurchasedEvent()
        {
            Game1.freezeControls = true;
            DelayedAction.playSoundAfterDelay("crafting", 1000);
            DelayedAction.playSoundAfterDelay("crafting", 1500);
            DelayedAction.playSoundAfterDelay("crafting", 2000);
            DelayedAction.playSoundAfterDelay("crafting", 2500);
            DelayedAction.playSoundAfterDelay("axchop", 3000);
            DelayedAction.playSoundAfterDelay("Ship", 3200);
            Game1.viewportFreeze = true;
            Game1.viewport.X = -10000;
            Game1.pauseThenDoFunction(4000, doneWithPurchasedEvent);
        }

        public void doneWithPurchasedEvent()
        {
            MapPatches.applyUnlockable(this);
            Game1.globalFadeToClear();
            Game1.viewportFreeze = false;
            Game1.freezeControls = false;
            openRewardsMenu();
        }

        public bool allRequirementsPaid()
        {
            if (_completed.Value)
                return true;

            if (ShopType is ShopType.CCBundle)
                return _alreadyPaid.Count() >= BundleSlots;

            return _price.Pairs.All(e => _alreadyPaid.ContainsKey(e.Key));
        }

        public static Item parseItem(string id, int initialStack = 0, int quality = 0)
        {
            //Items prefixed with (JA) are item names
            if (id.StartsWith("(JA)", StringComparison.OrdinalIgnoreCase)) {
                var name = id[4..].Trim();
                if (CachedJsonAssetIDs.ContainsKey(name))
                    return new StardewValley.Object(CachedJsonAssetIDs[name], initialStack, quality: quality);

                var match = Game1.objectData.FirstOrDefault(el => el.Value.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase));

                if (match.Value is null)
                    Monitor.LogOnce($"Unknown item name: {name}", LogLevel.Error);

                CachedJsonAssetIDs.Add(name, match.Key ?? name);

                id = match.Key ?? name;
            } else if (id.TrimStart().StartsWith(AdvancedPricingItem.APTYPEDEFINITION, StringComparison.OrdinalIgnoreCase))
                return AdvancedPricingItem.parseItem(id, initialStack, quality);

            else if (id.TrimStart().StartsWith(AdvancedPricingItem.FLAVOREDTYPEDEFINITION, StringComparison.OrdinalIgnoreCase))
                return AdvancedPricingItem.parseFlavoredItem(id, initialStack, quality);

            var ret = ItemRegistry.Create(id, initialStack, quality: quality);

            if (initialStack == 0)
                ret.Stack = 0;

            return ret;
        }

        public string getDisplayName()
        {
            if (ShowDebugNames)
                return ID;

            if (BundleName != "")
                return BundleName;

            if (ShopType == ShopType.ParrotPerch)
                return "Unnamed Parrot";

            return "Unnamed Bundle";
        }

        //Returns all required items of a Price entry except exception items
        public List<Item> getRequiredItems(string reqKey)
        {
            if (RequiredItems.TryGetValue(reqKey, out List<Item> cachedItems))
                return cachedItems;

            var items = new List<Item>();

            foreach (var req in reqKey.Split(",")) {
                var id = getIDFromReqSplit(req);
                if (isExceptionItem(id))
                    continue;

                var quality = getQualityFromReqSplit(req);
                items.Add(parseItem(id, 0, quality));
            }

            RequiredItems.Add(reqKey, items);
            return items;
        }

        public List<Item> getRequiredItemsAllowExceptions(string reqKey)
        {
            if (RequiredItemsWithExceptions.TryGetValue(reqKey, out List<Item> cachedItems))
                return cachedItems;

            var items = new List<Item>();

            foreach (var req in reqKey.Split(",")) {
                var id = getIDFromReqSplit(req);
                if (id.ToLower().Trim() == "money")
                    continue;

                var quality = getQualityFromReqSplit(req);
                items.Add(parseItem(id, 0, quality));
            }

            RequiredItemsWithExceptions.Add(reqKey, items);
            return items;
        }

        public List<Item> getRewardSpawnFieldItems()
        {
            var ret = new List<Item>();
            var models = Helper.GameContent.Load<Dictionary<string, UnlockableModel>>("UnlockableBundles/Bundles");
            if (!models.TryGetValue(ID, out var model))
                return ret;

            ItemQueryContext itemQueryContext = new();
            foreach (GenericSpawnItemDataWithCondition entry in model.BundleRewardSpawnFields) {
                if (!GameStateQuery.CheckConditions(entry.Condition))
                    continue;

                var item = ItemQueryResolver.TryResolveRandomItem(entry, itemQueryContext, logError: (query, message) => Monitor.Log($"Failed parsing item query '{query}': {message}", LogLevel.Warn));
                if (item is not null)
                    ret.Add(item);
            }
            return ret;
        }

        public bool updatePlacementRequirements()
        {
            var allFulfilled = true;
            foreach (var placementRequirement in SpecialPlacementRequirements)
                if (!placementRequirement.UpdateFulfilled(this))
                    allFulfilled = false;

            return allFulfilled;
        }
    }
}
