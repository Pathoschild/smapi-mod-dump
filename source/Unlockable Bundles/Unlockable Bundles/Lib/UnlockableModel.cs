/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/unlockable-bundles
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData;
using Unlockable_Bundles.Lib.Enums;
using static Unlockable_Bundles.ModEntry;

namespace Unlockable_Bundles.Lib
{
    //The model is here because casting directly to Unlockable caused issues.
    //At this point some logic depends on it iirc to clone Unlockable for non unique locations
    public class UnlockableModel
    {
        //Any fields added here should also be added to:
        //- Unlockable -> UnlockableModel explicit operator
        //- Unlockable as a field/property
        //- Unlockable constructor
        //- Unlockable > addNetFields
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
        public List<Vector2> AlternativeShopPositions = new();
        public string ShopTexture = null;
        public string ShopAnimation = null;
        public int? ShopTextureWidth = null; //The width of the image
        public int? ShopTextureHeight = null;
        public Vector2 ShopDrawDimensions;
        public Vector2 ShopDrawOffset;
        public string ShopEvent = null;
        public ShopType ShopType = ShopType.Dialogue;
        public bool? InstantShopRemoval = null;
        public string ShopColor = "";

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

        public string OverviewTexture = null;
        public string OverviewAnimation = null;
        public int OverviewTextureWidth = 32;
        public string OverviewDescription = null;
        public string OverviewColor = "";

        public int RandomPriceEntries = 0;
        public int RandomRewardEntries = 0;
        public Dictionary<string, int> Price = new();
        public Dictionary<string, string> PriceMigration = new();
        public Dictionary<string, int> AlreadyPaid = new();
        public Dictionary<string, int> AlreadyPaidIndex = new();
        public Dictionary<string, int> BundleReward = new();
        public List<GenericSpawnItemDataWithCondition> BundleRewardSpawnFields = new();

        public string EditMap = "NONE";
        public PatchMapMode EditMapMode = PatchMapMode.Overlay;
        public Vector2 EditMapPosition;
        public string EditMapLocation = "";

        public List<PlacementRequirement> SpecialPlacementRequirements = new(); //Not a NetField, only relevant for host

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
                AlternativeShopPositions = v._alternativeShopPositions.ToList(),
                ShopTexture = v.ShopTexture,
                ShopAnimation = v.ShopAnimation,
                ShopTextureWidth = v.ShopTextureWidth,
                ShopTextureHeight = v.ShopTextureHeight,
                ShopDrawDimensions = v.ShopDrawDimensions,
                ShopDrawOffset = v.ShopDrawOffset,
                ShopEvent = v.ShopEvent,
                ShopType = v.ShopType,
                InstantShopRemoval = v.InstantShopRemoval,
                ShopColor = v.ShopColor.asHexString(),

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

                OverviewTexture = v.OverviewTexture,
                OverviewAnimation = v.OverviewAnimation,
                OverviewTextureWidth = v.OverviewTextureWidth,
                OverviewDescription = v.OverviewDescription,
                OverviewColor = v.OverviewColor.asHexString(),

                RandomPriceEntries = v.RandomPriceEntries,
                RandomRewardEntries = v.RandomRewardEntries,
                Price = v._price.Pairs.ToDictionary(x => x.Key, x => x.Value),
                PriceMigration = v._priceMigration.Pairs.ToDictionary(x => x.Key, x => x.Value),
                AlreadyPaid = v._alreadyPaid.Pairs.ToDictionary(x => x.Key, x => x.Value),
                AlreadyPaidIndex = v._alreadyPaidIndex.Pairs.ToDictionary(x => x.Key, x => x.Value),
                BundleReward = v._bundleReward.Pairs.ToDictionary(x => x.Key, x => x.Value),

                EditMap = v.EditMap,
                EditMapMode = v.EditMapMode,
                EditMapPosition = v.EditMapPosition,
                EditMapLocation = v.EditMapLocation,

                SpecialPlacementRequirements = PlacementRequirement.CloneList(v.SpecialPlacementRequirements)
        };
        }
        public void applyDefaultValues()
        {
            BundleDescription = defaultBundleDescription();
            BundleSlots = defaultBundleSlots();

            ShopTexture = defaultShopTexture();
            ShopEvent = defaultEventScript();
            ShopAnimation = defaultShopAnimation();
            ShopTextureWidth = defaultShopTextureWidth();
            ShopTextureHeight = defaultShopTextureHeight();
            ShopDrawDimensions = defaultShopDrawDimensions();
            ShopDrawOffset = defaultShopDrawOffset();
            InstantShopRemoval = defaultShopRemoval();
            JunimoNoteTexture = defaultJunimoNoteTexture();

            InteractionShake = defaultInteractionShake();
            InteractionTexture = defaultInteractionTexture();
            InteractionAnimation = defaultInteractionAnimation();
            InteractionSound = defaultInteractionSound();

            TimeUntilChomp = defaultTimeUntilChomp();

            applySpeechbubbleOffset();

            ShopType = overwriteShopType();
        }

        private Vector2 defaultShopDrawOffset()
        {
            if (ShopDrawOffset != Vector2.Zero)
                return ShopDrawOffset;

            return ShopType switch {
                ShopType.CCMagicBook => new Vector2(-16, -32),
                _ => new Vector2(0, 0)
            };
        }

        private Vector2 defaultShopDrawDimensions()
        {
            if (ShopDrawDimensions != Vector2.Zero)
                return ShopDrawDimensions;

            return ShopType switch {
                ShopType.CCMagicBook => new Vector2(96, 192),
                _ => new Vector2(64, 128)
            };     
        }

        private int? defaultShopTextureWidth()
        {
            if(ShopTextureWidth is not null)
                return ShopTextureWidth;

            return ShopType switch {
                ShopType.CCMagicBook => 48,
                _ => 32
            };
        }
        private int? defaultShopTextureHeight()
        {
            if (ShopTextureHeight is not null)
                return ShopTextureHeight;

            return ShopType switch {
                _ => ShopTextureWidth * 2
            };
        }

        private ShopType overwriteShopType()
        {
            return ShopType switch {
                ShopType.AltCCBundle => ShopType.CCBundle,
                ShopType.CCMagicBook => ShopType.CCBundle,
                _ => ShopType
            };
        }

        private string defaultBundleDescription()
        {
            if (BundleDescription != null)
                return BundleDescription;

            return ShopType switch {
                ShopType.ParrotPerch => Helper.Translation.Get("ub_parrot_ask"),
                ShopType.SpeechBubble => Helper.Translation.Get("ub_speech_ask"),
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
                ShopType.CCMagicBook => "UnlockableBundles/ShopTextures/MagicBook",
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
                ShopType.CCMagicBook => "0-23@100",
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
                ShopType.AltCCBundle => "UnlockableBundles/UI/AlternativeJunimoNote",
                ShopType.CCMagicBook => "UnlockableBundles/UI/MagicBookJunimoNote",
                _ => ""
            };
        }

        private bool? defaultInteractionShake()
        {
            if (InteractionShake != null)
                return InteractionShake;

            return ShopType switch {
                ShopType.ParrotPerch => true,
                ShopType.CCMagicBook => true,
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
                ShopType.CCMagicBook => "qi_shop",
                ShopType.ParrotPerch => "parrot_squawk",
                ShopType.SpeechBubble => "junimoMeep1",
                _ => ""
            };
        }

        private void applySpeechbubbleOffset()
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
                ShopType.CCBundle or ShopType.AltCCBundle or ShopType.CCMagicBook or ShopType.ParrotPerch => "none",
                _ => "carpentry"
            };
        }

        public Color parseColor()
            => parseColor(ID, ShopColor);

        public Color parseOverviewColor()
            => parseColor(ID, OverviewColor);

        public static Color parseColor(string ID, string shopColor)
        {
            try {
                shopColor = shopColor.Trim();
                if (shopColor == "")
                    return Color.White;

                System.Drawing.Color color;
                if (shopColor.StartsWith("#")) {
                    color = System.Drawing.ColorTranslator.FromHtml(shopColor);

                    if (color != System.Drawing.Color.Empty)
                        return new Color(color.R, color.G, color.B, color.A);
                }

                if (shopColor.StartsWith("rgb(")) {
                    var rgb = shopColor.Substring(4, shopColor.Length - 5);
                    var split = rgb.Split(",");
                    return new Color(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]));
                }

                if (shopColor.StartsWith("rgba(")) {
                    var rgb = shopColor.Substring(5, shopColor.Length - 6);
                    var split = rgb.Split(",");
                    return new Color(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]), int.Parse(split[3]));
                }

                color = System.Drawing.Color.FromName(shopColor);
                if (color.IsKnownColor)
                    return new Color(color.R, color.G, color.B, color.A);
            } catch (Exception ex) {
                Monitor.LogOnce(ex.Message);
            }

            Monitor.LogOnce($"Could not parse '{shopColor}' to a Color: {ID}", LogLevel.Warn);
            return Color.White;
        }
    }
}
