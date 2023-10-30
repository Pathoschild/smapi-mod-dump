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

namespace Unlockable_Bundles.Lib
{
    public sealed class Unlockable : INetObject<NetFields>
    {
        public NetFields NetFields { get; } = new NetFields("DLX.Bundles/Unlockable");

        private NetString _id = new NetString();
        private NetString _location = new NetString();
        private NetString _locationUnique = new NetString();

        private NetString _bundleName = new NetString();
        private NetString _bundleDescription = new NetString();
        private NetEnum<BundleIconType> _bundleIcon = new NetEnum<BundleIconType>();
        private NetString _bundleIconAsset = new NetString();
        private NetInt _bundleSlots = new NetInt();
        private NetString _junimoNoteTexture = new NetString();
        private NetString _bundleCompletedMail = new NetString();

        private NetVector2 _shopPosition = new NetVector2();
        private NetString _shopTexture = new NetString();
        private NetString _shopAnimation = new NetString();
        private NetString _shopEvent = new NetString();
        private NetEnum<ShopType> _shopType = new NetEnum<ShopType>();
        private NetBool _instantShopRemoval = new NetBool();

        private NetBool _drawQuestionMark = new NetBool();
        private NetVector2 _questionMarkOffset = new NetVector2();
        private NetVector2 _speechBubbleOffset = new NetVector2();
        private NetRectangle _parrotTarget = new NetRectangle();
        private NetFloat _timeUntilChomp = new NetFloat();
        private NetInt _parrotIndex = new NetInt();
        private NetString _parrotTexture = new NetString();

        private NetBool _interactionShake = new NetBool();
        private NetString _interactionTexture = new NetString();
        private NetString _interactionAnimation = new NetString();
        private NetString _interactionSound = new NetString();

        public NetInt _randomPriceEntries = new NetInt();
        public NetStringIntDictionary _price = new NetStringIntDictionary();
        public NetStringIntDictionary _alreadyPaid = new NetStringIntDictionary();
        public NetStringIntDictionary _alreadyPaidIndex = new NetStringIntDictionary();
        public NetStringIntDictionary _bundleReward = new NetStringIntDictionary();

        private NetString _editMap = new NetString();
        private NetEnum<EditMapMode> _editMapMode = new NetEnum<EditMapMode>();
        private NetVector2 _editMapPosition = new NetVector2();
        private NetString _editMapLocation = new NetString();

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
        public string ShopTexture { get => _shopTexture.Value; set => _shopTexture.Value = value; }
        public string ShopAnimation { get => _shopAnimation.Value; set => _shopAnimation.Value = value; }
        public string ShopEvent { get => _shopEvent.Value; set => _shopEvent.Value = value; }
        public ShopType ShopType { get => _shopType.Value; set => _shopType.Value = value; }
        public bool InstantShopRemoval { get => _instantShopRemoval.Value; set => _instantShopRemoval.Value = value; }

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

        public int RandomPriceEntries { get => _randomPriceEntries.Value; set => _randomPriceEntries.Value = value; }

        public string EditMap { get => _editMap.Value; set => _editMap.Value = value; }
        public EditMapMode EditMapMode { get => _editMapMode.Value; set => _editMapMode.Value = value; }
        public Vector2 EditMapPosition { get => _editMapPosition.Value; set => _editMapPosition.Value = value; }
        public string EditMapLocation { get => _editMapLocation.Value; set => _editMapLocation.Value = value; }

        private string CachedLocalizedShopDescription = null;
        public static Dictionary<string, string> CachedJsonAssetIDs = new Dictionary<string, string>();
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
            this.ShopTexture = model.ShopTexture;
            this.ShopAnimation = model.ShopAnimation;
            this.ShopEvent = model.ShopEvent;
            this.ShopType = model.ShopType;
            this.InstantShopRemoval = model.InstantShopRemoval == true;

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

            this.RandomPriceEntries = model.RandomPriceEntries;
            this._price = new NetStringIntDictionary(model.Price);
            this._alreadyPaid = new NetStringIntDictionary(model.AlreadyPaid);
            this._alreadyPaidIndex = new NetStringIntDictionary(model.AlreadyPaidIndex);
            this._bundleReward = new NetStringIntDictionary(model.BundleReward);

            this.EditMap = model.EditMap;
            this.EditMapMode = model.EditMapMode;
            this.EditMapPosition = model.EditMapPosition;
            this.EditMapLocation = model.EditMapLocation;
            addNetFields();
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
            .AddField(_shopTexture, "_shopTexture")
            .AddField(_shopAnimation, "_shopAnimation")
            .AddField(_shopEvent, "_shopEvent")
            .AddField(_shopType, "_shopType")
            .AddField(_instantShopRemoval, "_instantShopRemoval")

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

            .AddField(_randomPriceEntries, "_randomPriceEntries")
            .AddField(_price, "_price")
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

            var unlockables = ModEntry._Helper.GameContent.Load<Dictionary<string, UnlockableModel>>("UnlockableBundles/Bundles");
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
        public static bool isExceptionItem(string id) => id.ToLower() == "money" || id == "(O)858" || id == "(O)73";
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
            ModEntry._API.raiseShopPurchased(new API.BundlePurchasedEventArgs(Game1.player, Location, LocationUnique, ID, true));
            ModEntry._Helper.Multiplayer.SendMessage((UnlockableModel)this, "BundlePurchased", modIDs: new[] { ModEntry.Mod.ModManifest.UniqueID });
            ModEntry._Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue().globalChatInfoMessage("Bundle");

            if (BundleCompletedMail != "")
                Game1.addMailForTomorrow(getMailKey(), noLetter: false, sendToEveryone: true);
        }

        public void processContribution(KeyValuePair<string, int> requirement, int index = -1)
        {
            _alreadyPaid.Add(requirement.Key, requirement.Value);
            _alreadyPaidIndex.Add(requirement.Key, index);
            ModData.setPartiallyPurchased(ID, LocationUnique, requirement.Key, requirement.Value, index);
            ModEntry._API.raiseShopContributed(new API.BundlePurchasedEventArgs(Game1.player, Location, LocationUnique, ID, true));
            ModEntry._Helper.Multiplayer.SendMessage((UnlockableModel)this, "BundleContributed", modIDs: new[] { ModEntry.Mod.ModManifest.UniqueID });
        }

        public void processShopEvent()
        {
            if (InstantShopRemoval)
                Task.Delay(800).ContinueWith(t => ShopPlacement.removeShop(this));

            if (ShopEvent.ToLower() != "none" && Game1.activeClickableMenu != null) {
                Game1.dialogueUp = false;
                Game1.activeClickableMenu.exitThisMenu(false);
            }

            Game1.player.completelyStopAnimatingOrDoingAction();

            if (ShopEvent.ToLower() == "carpentry")
                Game1.globalFadeToBlack(playPurchasedEvent);
            else if (ShopEvent.ToLower() == "none") {
                UpdateHandler.applyUnlockable(this);
                openRewardsMenu();
                return;
            } else {
                var ev = new UBEvent(this, ShopEvent, Game1.player);
                ev.onEventFinished = openRewardsMenu;
                Game1.globalFadeToBlack(() => Game1.player.currentLocation.startEvent(ev));
            }
        }
        private void openRewardsMenu()
        {
            List<Item> rewards = new List<Item>();
            foreach (var entry in _bundleReward.Pairs) {
                var id = getIDFromReqSplit(entry.Key);
                var quality = getQualityFromReqSplit(entry.Key);

                if (Inventory.addExceptionItem(Game1.player, id, entry.Value))
                    continue;

                rewards.Add(parseItem(id, entry.Value, quality: quality));
            }

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
            UpdateHandler.applyUnlockable(this);
            Game1.globalFadeToClear();
            Game1.viewportFreeze = false;
            Game1.freezeControls = false;
            openRewardsMenu();
        }

        public bool allRequirementsPaid()
        {
            if (_completed.Value)
                return true;

            if (ShopType is ShopType.CCBundle or ShopType.AltCCBundle)
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
                    ModEntry._Monitor.LogOnce($"Unknown item name: {name}", LogLevel.Error);

                CachedJsonAssetIDs.Add(name, match.Key ?? name);

                id = match.Key ?? name;
            }

            var ret = ItemRegistry.Create(id, initialStack, quality: quality);

            return ret;
        }
    }
}
