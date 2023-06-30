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

        public Vector2 ShopPosition;
        public string ShopTexture = null;
        public string ShopAnimation = null;
        public string ShopEvent = "";
        public ShopType ShopType = ShopType.Dialogue;

        public bool DrawQuestionMark;
        public Vector2 QuestionMarkOffset;
        public Vector2 SpeechBubbleOffset;
        public Rectangle ParrotTarget;
        public float TimeUntilChomp;

        public bool? InteractionShake = null;
        public string InteractionTexture = null;
        public string InteractionAnimation = null;
        public string InteractionSound = null;

        public Dictionary<string, int> Price = new Dictionary<string, int>();
        public Dictionary<string, int> AlreadyPaid = new Dictionary<string, int>();
        public Dictionary<string, int> AlreadyPaidIndex = new Dictionary<string, int>();

        public string UpdateMap = "NONE";
        public string UpdateType = "Overlay";
        public Vector2 UpdatePosition;

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

                ShopPosition = v.ShopPosition,
                ShopTexture = v.ShopTexture,
                ShopAnimation = v.ShopAnimation,
                ShopEvent = v.ShopEvent,
                ShopType = v.ShopType,

                DrawQuestionMark = v.DrawQuestionMark,
                QuestionMarkOffset = v.QuestionMarkOffset,
                SpeechBubbleOffset = v.SpeechBubbleOffset,
                ParrotTarget = v.ParrotTarget,
                TimeUntilChomp = v.TimeUntilChomp,

                //TODO: Implement these
                InteractionShake = v.InteractionShake,
                InteractionTexture = v.InteractionTexture,
                InteractionAnimation = v.InteractionAnimation,
                InteractionSound = v.InteractionSound,

                //TODO: Multiplayer celebration message. Need one for parrot perch
                //TODO: JunimoNoteTexture alternative, where my current alternative just has a default replacement

                Price = v.Price,
                AlreadyPaid = v.AlreadyPaid,
                AlreadyPaidIndex = v.AlreadyPaidIndex,

                UpdateMap = v.UpdateMap,
                UpdateType = v.UpdateType,
                UpdatePosition = v.UpdatePosition,
            };
        }
        public void applyDefaultValues()
        {
            applyDefaultShopType();

        }

        private void applyDefaultShopType()
        {
            BundleDescription = defaultBundleDescription();

            ShopTexture = defaultShopTexture();
            ShopAnimation = defaultShopAnimation();

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
                _ => ""
            };
        }

        private string defaultShopTexture()
        {
            if (ShopTexture != null)
                return ShopTexture;

            return ShopType switch {
                ShopType.Dialogue => "UnlockableBundles/ShopTextures/Sign",
                ShopType.CCBundle => "UnlockableBundles/ShopTextures/CCBundle",
                ShopType.AltCCBundle => "UnlockableBundles/ShopTextures/Scroll",
                ShopType.SpeechBubble or ShopType.YesNoSpeechBubble => "UnlockableBundles/ShopTextures/Blue_Junimo",
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
                ShopType.SpeechBubble or ShopType.YesNoSpeechBubble => "0-10@100,11@500",
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
                _ => ""
            };
        }

        private void defaultSpeechbubbleOffset()
        {
            if (ShopType == ShopType.SpeechBubble || ShopType == ShopType.YesNoSpeechBubble)
                SpeechBubbleOffset += new Vector2(0, 100);
        }

        private float defaultTimeUntilChomp()
        {
            if (TimeUntilChomp != 0f)
                return TimeUntilChomp;

            return ShopType switch {
                ShopType.ParrotPerch => 1f,
                ShopType.SpeechBubble or ShopType.YesNoSpeechBubble => 1.25f,
                _ => 0f
            };
        }
    }
}
