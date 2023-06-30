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

namespace Unlockable_Bundles.Lib
{
    public class Unlockable : INetObject<NetFields>
    {
        public NetFields NetFields { get; } = new NetFields();

        private NetString _id = new NetString();
        private NetString _location = new NetString();
        private NetString _locationUnique = new NetString();

        private NetString _bundleName = new NetString();
        private NetString _bundleDescription = new NetString();
        private NetEnum<BundleIconType> _bundleIcon = new NetEnum<BundleIconType>();
        private NetString _bundleIconAsset = new NetString();
        private NetInt _bundleSlots = new NetInt();

        private NetVector2 _shopPosition = new NetVector2();
        private NetString _shopTexture = new NetString();
        private NetString _shopAnimation = new NetString();
        private NetString _shopEvent = new NetString();
        private NetEnum<ShopType> _shopType = new NetEnum<ShopType>();

        private NetBool _drawQuestionMark = new NetBool();
        private NetVector2 _questionMarkOffset = new NetVector2();
        private NetVector2 _speechBubbleOffset = new NetVector2();
        private NetRectangle _parrotTarget = new NetRectangle();
        private NetFloat _timeUntilChomp = new NetFloat();

        private NetBool _interactionShake = new NetBool();
        private NetString _interactionTexture = new NetString();
        private NetString _interactionAnimation = new NetString();
        private NetString _interactionSound = new NetString();

        public NetStringIntDictionary _price = new NetStringIntDictionary();
        public NetStringIntDictionary _alreadyPaid = new NetStringIntDictionary();
        public NetStringIntDictionary _alreadyPaidIndex = new NetStringIntDictionary();

        private NetString _updateMap = new NetString();
        private NetString _updateType = new NetString();
        private NetVector2 _updatePosition = new NetVector2();

        public string ID { get => _id.Value; set => _id.Value = value; }
        public string Location { get => _location.Value; set => _location.Value = value; }
        public string LocationUnique { get => _locationUnique.Value; set => _locationUnique.Value = value; }

        public string BundleName { get => _bundleName.Value; set => _bundleName.Value = value; }
        public string BundleDescription { get => _bundleDescription.Value; set => _bundleDescription.Value = value; }
        public BundleIconType BundleIcon { get => _bundleIcon.Value; set => _bundleIcon.Value = value; }
        public string BundleIconAsset { get => _bundleIconAsset.Value; set => _bundleIconAsset.Value = value; }
        public int BundleSlots { get => _bundleSlots.Value; set => _bundleSlots.Value = value; }

        public Vector2 ShopPosition { get => _shopPosition.Value; set => _shopPosition.Value = value; }
        public string ShopTexture { get => _shopTexture.Value; set => _shopTexture.Value = value; }
        public string ShopAnimation { get => _shopAnimation.Value; set => _shopAnimation.Value = value; }
        public string ShopEvent { get => _shopEvent.Value; set => _shopEvent.Value = value; }
        public ShopType ShopType { get => _shopType.Value; set => _shopType.Value = value; }

        public bool DrawQuestionMark { get => _drawQuestionMark.Value; set => _drawQuestionMark.Value = value; }
        public Vector2 QuestionMarkOffset { get => _questionMarkOffset.Value; set => _questionMarkOffset.Value = value; }
        public Vector2 SpeechBubbleOffset { get => _speechBubbleOffset.Value; set => _speechBubbleOffset.Value = value; }
        public Rectangle ParrotTarget { get => _parrotTarget.Value; set => _parrotTarget.Value = value; }
        public float TimeUntilChomp { get => _timeUntilChomp.Value; set => _timeUntilChomp.Value = value; }

        public bool InteractionShake { get => _interactionShake.Value; set => _interactionShake.Value = value; }
        public string InteractionTexture { get => _interactionTexture.Value; set => _interactionTexture.Value = value; }
        public string InteractionAnimation { get => _interactionAnimation.Value; set => _interactionAnimation.Value = value; }
        public string InteractionSound { get => _interactionSound.Value; set => _interactionSound.Value = value; }

        public Dictionary<string, int> Price
        {
            get => this._price.Pairs.AsEnumerable().ToDictionary(x => x.Key, x => x.Value);
            set => this._price.CopyFrom(value.AsEnumerable());
        }
        public Dictionary<string, int> AlreadyPaid
        {
            get => this._alreadyPaid.Pairs.AsEnumerable().ToDictionary(x => x.Key, x => x.Value);
            set => this._alreadyPaid.CopyFrom(value.AsEnumerable());
        }
        public Dictionary<string, int> AlreadyPaidIndex
        {
            get => this._alreadyPaidIndex.Pairs.AsEnumerable().ToDictionary(x => x.Key, x => x.Value);
            set => this._alreadyPaidIndex.CopyFrom(value.AsEnumerable());
        }
        public string UpdateMap { get => _updateMap.Value; set => _updateMap.Value = value; }
        public string UpdateType { get => _updateType.Value; set => _updateType.Value = value; }
        public Vector2 UpdatePosition { get => _updatePosition.Value; set => _updatePosition.Value = value; }

        private string CachedLocalizedShopDescription = null;
        public Unlockable(UnlockableModel model)
        {
            this.ID = model.ID;
            this.Location = model.Location;
            this.LocationUnique = model.LocationUnique;

            this.BundleName = model.BundleName;
            this.BundleIcon = model.BundleIcon;
            this.BundleIconAsset = model.BundleIconAsset;
            this.BundleSlots = model.BundleSlots;
            this.BundleDescription = model.BundleDescription;

            this.ShopPosition = model.ShopPosition;
            this.ShopTexture = model.ShopTexture;
            this.ShopAnimation = model.ShopAnimation;
            this.ShopEvent = model.ShopEvent;
            this.ShopType = model.ShopType;

            this.DrawQuestionMark = model.DrawQuestionMark;
            this.QuestionMarkOffset = model.QuestionMarkOffset;
            this.SpeechBubbleOffset = model.SpeechBubbleOffset;
            this.ParrotTarget = model.ParrotTarget;
            this.TimeUntilChomp = model.TimeUntilChomp;

            this.InteractionShake = model.InteractionShake == true;
            this.InteractionTexture = model.InteractionTexture;
            this.InteractionAnimation = model.InteractionAnimation;
            this.InteractionSound = model.InteractionSound;

            this.Price = model.Price;
            this.AlreadyPaid = model.AlreadyPaid;
            this.AlreadyPaidIndex = model.AlreadyPaidIndex;
            this.UpdateMap = model.UpdateMap;
            this.UpdateType = model.UpdateType;
            this.UpdatePosition = model.UpdatePosition;
            addNetFields();
        }

        public Unlockable() => addNetFields();

        private void addNetFields() => this.NetFields.AddFields(
            _id,
            _location,
            _locationUnique,

            _bundleName,
            _bundleIcon,
            _bundleIconAsset,
            _bundleSlots,
            _bundleDescription,

            _shopPosition,
            _shopTexture,
            _shopAnimation,
            _shopEvent,
            _shopType,

            _drawQuestionMark,
            _questionMarkOffset,
            _speechBubbleOffset,
            _parrotTarget,
            _timeUntilChomp,

            _interactionShake,
            _interactionTexture,
            _interactionAnimation,
            _interactionSound,

            _price,
            _alreadyPaid,
            _alreadyPaidIndex,

            _updateMap,
            _updateType,
            _updatePosition
            );

        public static Dictionary<string, Unlockable> convertModelDicToEntity(Dictionary<string, UnlockableModel> modelDic)
        {
            var entityDic = new Dictionary<string, Unlockable>();
            foreach (var entry in modelDic) {
                entry.Value.ID = entry.Key;
                entityDic.Add(entry.Key, new Unlockable(entry.Value));
            }

            return entityDic;
        }

        public GameLocation getGameLocation() => Game1.getLocationFromName(LocationUnique, Location != LocationUnique);

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

        public static string getIDFromReqSplit(string key) => key.Split(":").First();

        public static string getFirstIDFromReqKey(string reqKey) => getIDFromReqSplit(reqKey.Split(",").First().Trim().ToLower());
        public static int getFirstQualityFromReqKey(string reqKey) => getQualityFromReqSplit(reqKey.Split(",").First());
    }
}
