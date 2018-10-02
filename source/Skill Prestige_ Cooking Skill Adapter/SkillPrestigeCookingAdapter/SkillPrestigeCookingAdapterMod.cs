using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace SkillPrestigeCookingAdapter
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SkillPrestigeCookingAdapterMod : Mod
    {
        /// <summary>
        /// The cooking skill icon.
        /// </summary>
        internal static Texture2D IconTexture;

        /// <summary>
        /// Whether the Luck Skill mod is loaded.
        /// </summary>
        internal static bool IsLuckSkillModLoaded;

        public override void Entry(IModHelper helper)
        {
            IconTexture = helper.Content.Load<Texture2D>("icon.png");
            IsLuckSkillModLoaded = helper.ModRegistry.IsLoaded("spacechase0.LuckSkill");
        }
    }
}
