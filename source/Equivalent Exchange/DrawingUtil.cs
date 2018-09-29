using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquivalentExchange
{
    class DrawingUtil
    {
        public struct Icons
        {
            public static string SkillIcon = $"alchemySkillIconDeeper.png";
            public static string SkillIconBordered = $"alchemySkillIconBorderedNewColorExtended.png";
            public static string ShaperIcon = $"shaperProfessionIcon_Hybrid.png";
            public static string TransmuterIcon = $"transmuterProfessionIconV2.png";
            public static string AdeptIcon = $"adeptProfessionIcon.png";
            public static string SageIcon = $"sageProfessionIcon.png";
            public static string AurumancerIcon = $"aurumancerProfessionIcon.png";
            public static string ConduitIcon = $"conduitProfessionIcon.png";
            public static string AlchemyBarSpriteBackground = $"alchemyBarBackground.png";
            public static string AlchemyBarSpriteForeground = $"alchemyBarForeground.png";
        }

        public static Texture2D alchemySkillIcon;
        public static Texture2D alchemySkillIconBordered;
        public static Texture2D alchemyShaperIcon;
        public static Texture2D alchemyTransmuterIcon;
        public static Texture2D alchemyAdeptIcon;
        public static Texture2D alchemySageIcon;
        public static Texture2D alchemyAurumancerIcon;
        public static Texture2D alchemyConduitIcon;
        public static Texture2D alchemyBarSprite;
        public static Texture2D alchemyBarFillSprite;

        public static string assetPrefix = "assets\\";

        //handle capturing icons/textures for the mod's texture needs.
        public static void HandleTextureCaching()
        {
            alchemySkillIcon = EquivalentExchange.instance.Helper.Content.Load<Texture2D>($"{assetPrefix}{Icons.SkillIcon}");
            alchemySkillIconBordered = EquivalentExchange.instance.Helper.Content.Load<Texture2D>($"{assetPrefix}{Icons.SkillIconBordered}");
            alchemyShaperIcon = EquivalentExchange.instance.Helper.Content.Load<Texture2D>($"{assetPrefix}{Icons.ShaperIcon}");
            alchemyTransmuterIcon = EquivalentExchange.instance.Helper.Content.Load<Texture2D>($"{assetPrefix}{Icons.TransmuterIcon}");
            alchemyAdeptIcon = EquivalentExchange.instance.Helper.Content.Load<Texture2D>($"{assetPrefix}{Icons.AdeptIcon}");
            alchemySageIcon = EquivalentExchange.instance.Helper.Content.Load<Texture2D>($"{assetPrefix}{Icons.SageIcon}");
            alchemyAurumancerIcon = EquivalentExchange.instance.Helper.Content.Load<Texture2D>($"{assetPrefix}{Icons.AurumancerIcon}");
            alchemyConduitIcon = EquivalentExchange.instance.Helper.Content.Load<Texture2D>($"{assetPrefix}{Icons.ConduitIcon}");
            alchemyBarSprite = EquivalentExchange.instance.Helper.Content.Load<Texture2D>($"{assetPrefix}{Icons.AlchemyBarSpriteBackground}");
            alchemyBarFillSprite = EquivalentExchange.instance.Helper.Content.Load<Texture2D>($"{assetPrefix}{Icons.AlchemyBarSpriteForeground}");
        }

        internal static Texture2D GetProfessionTexture(int profession)
        {
            switch (profession)
            {
                case (int)ProfessionHelper.OldShaperId:
                    return alchemyShaperIcon;
                case (int)ProfessionHelper.OldSageId:
                    return alchemySageIcon;
                case (int)ProfessionHelper.OldTransmuterId:
                    return alchemyTransmuterIcon;
                case (int)ProfessionHelper.OldAdeptId:
                    return alchemyAdeptIcon;
                case (int)ProfessionHelper.OldAurumancerId:
                    return alchemyAurumancerIcon;
                case (int)ProfessionHelper.OldConduitId:
                    return alchemyConduitIcon;
            }
            return alchemySkillIconBordered;
        }
    }
}
