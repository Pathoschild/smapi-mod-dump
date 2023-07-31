/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-bundles
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using Unlockable_Bundles.Lib.Enums;

namespace Unlockable_Bundles.Lib
{
    //The model is here because casting directly to Unlockable caused issues
    public class UnlockableModel
    {
        //Don't forget to implement changes to Unlockable constructor as well
        public string ID = "";
        public string Location = "";
        public string LocationUnique = "";

        public string BundleName = "";
        public string BundleDescription = null;
        public BundleIconType BundleIcon = BundleIconType.Spring_Foraging;
        public string BundleIconAsset = "";
        public int BundleSlots = 0;
        public string JunimoNoteTexture = null;
        public string BundleCompletedMail = "";

        public Vector2 ShopPosition;
        public string ShopTexture = null;
        public string ShopAnimation = null;
        public string ShopEvent = null;
        public ShopType ShopType = ShopType.Dialogue;
        public bool? InstantShopRemoval = null;

        public bool DrawQuestionMark;
        public Vector2 QuestionMarkOffset;
        public Vector2 SpeechBubbleOffset;
        public Rectangle ParrotTarget;
        public float TimeUntilChomp;
        public int ParrotIndex = 0;
        public string ParrotTexture = "";

        public bool? InteractionShake = null;
        public string InteractionTexture = null; //Currently not in use
        public string InteractionAnimation = null; //Currently not in use
        public string InteractionSound = null;

        public int RandomPriceEntries = 0;
        public Dictionary<string, int> Price = new Dictionary<string, int>();
        public Dictionary<string, int> AlreadyPaid = new Dictionary<string, int>();
        public Dictionary<string, int> AlreadyPaidIndex = new Dictionary<string, int>();
        public Dictionary<string, int> BundleReward = new Dictionary<string, int>();

        public string EditMap = "NONE";
        public EditMapMode EditMapMode = EditMapMode.Overlay;
        public Vector2 EditMapPosition;
        public string EditMapLocation = "";

        public static explicit operator UnlockableModel(Unlockable v)
        {
            return new UnlockableModel() {
                ID = v.ID,
                Location = v.Location,
                LocationUnique = v.LocationUnique,

                BundleName = v.BundleName,
                BundleDescription = v.BundleDescription,
                BundleIcon = v.BundleIcon,
                BundleIconAsset = v.BundleIconAsset,
                BundleSlots = v.BundleSlots,
                JunimoNoteTexture = v.JunimoNoteTexture,
                BundleCompletedMail = v.BundleCompletedMail,

                ShopPosition = v.ShopPosition,
                ShopTexture = v.ShopTexture,
                ShopAnimation = v.ShopAnimation,
                ShopEvent = v.ShopEvent,
                ShopType = v.ShopType,
                InstantShopRemoval = v.InstantShopRemoval,

                DrawQuestionMark = v.DrawQuestionMark,
                QuestionMarkOffset = v.QuestionMarkOffset,
                SpeechBubbleOffset = v.SpeechBubbleOffset,
                ParrotTarget = v.ParrotTarget,
                TimeUntilChomp = v.TimeUntilChomp,
                ParrotIndex = v.ParrotIndex,
                ParrotTexture = v.ParrotTexture,

                InteractionShake = v.InteractionShake,
                InteractionTexture = v.InteractionTexture,
                InteractionAnimation = v.InteractionAnimation,
                InteractionSound = v.InteractionSound,

                RandomPriceEntries = v.RandomPriceEntries,
                Price = v._price.Pairs.ToDictionary(x => x.Key, x => x.Value),
                AlreadyPaid = v._alreadyPaid.Pairs.ToDictionary(x => x.Key, x => x.Value),
                AlreadyPaidIndex = v._alreadyPaidIndex.Pairs.ToDictionary(x => x.Key, x => x.Value),
                BundleReward = v._bundleReward.Pairs.ToDictionary(x => x.Key, x => x.Value),

                EditMap = v.EditMap,
                EditMapMode = v.EditMapMode,
                EditMapPosition = v.EditMapPosition,
                EditMapLocation = v.EditMapLocation
            };
        }
        public void applyDefaultValues()
        {
            applyDefaultShopType();

        }

        private void applyDefaultShopType()
        {
            BundleDescription = defaultBundleDescription();
            BundleSlots = defaultBundleSlots();

            ShopTexture = defaultShopTexture();
            ShopEvent = defaultEventScript();
            ShopAnimation = defaultShopAnimation();
            InstantShopRemoval = defaultShopRemoval();
            JunimoNoteTexture = defaultJunimoNoteTexture();

            InteractionShake = defaultInteractionShake();
            InteractionTexture = defaultInteractionTexture();
            InteractionAnimation = defaultInteractionAnimation();
            InteractionSound = defaultInteractionSound();

            TimeUntilChomp = defaultTimeUntilChomp();
            defaultSpeechbubbleOffset();
        }

        private string defaultBundleDescription()
        {
            if (BundleDescription != null)
                return BundleDescription;

            return ShopType switch {
                ShopType.ParrotPerch => ModEntry._Helper.Translation.Get("ub_parrot_ask"),
                ShopType.SpeechBubble => ModEntry._Helper.Translation.Get("ub_speech_ask"),
                _ => ""
            };
        }

        private int defaultBundleSlots()
        {
            if (BundleSlots <= 0)
                return RandomPriceEntries <= 0 ? Price.Count : RandomPriceEntries;

            return BundleSlots;
        }

        private string defaultShopTexture()
        {
            if (ShopTexture != null)
                return ShopTexture;

            return ShopType switch {
                ShopType.Dialogue => "UnlockableBundles/ShopTextures/Sign",
                ShopType.CCBundle => "UnlockableBundles/ShopTextures/CCBundle",
                ShopType.AltCCBundle => "UnlockableBundles/ShopTextures/Scroll",
                ShopType.SpeechBubble => "UnlockableBundles/ShopTextures/Blue_Junimo",
                ShopType.ParrotPerch => "UnlockableBundles/ShopTextures/ParrotPerch",
                _ => ""
            };
        }


        private string defaultShopAnimation()
        {
            if (ShopAnimation != null)
                return ShopAnimation;

            return ShopType switch {
                ShopType.CCBundle or ShopType.AltCCBundle => "0-7@100,8@1000",
                ShopType.SpeechBubble => "0-10@100,11@500",
                _ => ""
            };
        }

        private bool? defaultShopRemoval()
        {
            if (InstantShopRemoval != null)
                return InstantShopRemoval;

            return ShopType switch {
                ShopType.Dialogue or ShopType.SpeechBubble => true,
                _ => false
            };
        }

        private string defaultJunimoNoteTexture()
        {
            if (JunimoNoteTexture != null)
                return JunimoNoteTexture;

            return ShopType switch {
                ShopType.AltCCBundle => "UnlockableBundles/ShopTextures/AlternativeJunimoNote",
                _ => ""
            };
        }

        private bool? defaultInteractionShake()
        {
            if (InteractionShake != null)
                return InteractionShake;

            return ShopType switch {
                ShopType.ParrotPerch => true,
                _ => false
            };
        }

        private string defaultInteractionTexture()
        {
            if (InteractionTexture != null)
                return InteractionTexture;

            return ShopType switch {
                _ => ""
            };
        }

        private string defaultInteractionAnimation()
        {
            if (InteractionAnimation != null)
                return InteractionAnimation;

            return ShopType switch {
                _ => ""
            };
        }

        private string defaultInteractionSound()
        {
            if (InteractionSound != null)
                return InteractionSound;

            return ShopType switch {
                ShopType.ParrotPerch => "parrot_squawk",
                ShopType.SpeechBubble => "junimoMeep1",
                _ => ""
            };
        }

        private void defaultSpeechbubbleOffset()
        {
            if (ShopType == ShopType.SpeechBubble)
                SpeechBubbleOffset += new Vector2(0, 100);
        }

        private float defaultTimeUntilChomp()
        {
            if (TimeUntilChomp != 0f)
                return TimeUntilChomp;

            return ShopType switch {
                ShopType.ParrotPerch => 1f,
                ShopType.SpeechBubble => 1.25f,
                _ => 0f
            };
        }

        private string defaultEventScript()
        {
            if (ShopEvent != null)
                return ShopEvent;

            return ShopType switch {
                ShopType.CCBundle or ShopType.AltCCBundle or ShopType.ParrotPerch => "none",
                _ => "carpentry"
            };
        }
    }
}
