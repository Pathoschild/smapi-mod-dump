/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Effects
{
    public enum EffectIcon
    {
        None,
        Popularity,
        Health,
        MaxHealth,
        Energy,
        MaxEnergy,
        Attack,
        Defense,
        CriticalHitRate,
        Immunity,
        Speed,
        SaveFromDeath,
        SkillFarming,
        SkillFishing,
        SkillForaging,
        SkillMining,
        SkillCombat,
        SkillLuck,
        Yoba,
        TreasureChest,
        Animal_Chicken,
        Animal_Cow,
        Glow,
        Person_Lewis
    }

    static class EffectIconExtensions
    {
        public static void Draw(this EffectIcon icon, SpriteBatch spriteBatch, Vector2 location)
        {
            if (icon == EffectIcon.None) return;            

            Texture2D texture = Game1.mouseCursors;
            Rectangle? rect = null;

            switch (icon)
            {                
                case EffectIcon.Health: rect = new Rectangle(0, 438, 10, 10); break;                
                case EffectIcon.Energy: rect = new Rectangle(0, 428, 10, 10); break;
                case EffectIcon.MaxEnergy: rect = new Rectangle(80, 428, 10, 10); break;
                case EffectIcon.Defense: rect = new Rectangle(110, 428, 10, 10); break;
                case EffectIcon.SaveFromDeath: rect = new Rectangle(140, 428, 10, 10); break;
                case EffectIcon.Attack: rect = new Rectangle(120, 428, 10, 10); break;
                case EffectIcon.Speed: rect = new Rectangle(130, 428, 10, 10); break;
                case EffectIcon.Immunity: rect = new Rectangle(150, 428, 10, 10); break;
                case EffectIcon.CriticalHitRate: rect = new Rectangle(160, 428, 10, 10); break;
                case EffectIcon.SkillFarming: rect = new Rectangle(10, 428, 10, 10); break;
                case EffectIcon.SkillFishing: rect = new Rectangle(20, 428, 10, 10); break;
                case EffectIcon.SkillMining: rect = new Rectangle(30, 428, 10, 10); break;
                case EffectIcon.SkillCombat: rect = new Rectangle(40, 428, 10, 10); break;
                case EffectIcon.SkillLuck: rect = new Rectangle(50, 428, 10, 10); break;
                case EffectIcon.SkillForaging: rect = new Rectangle(60, 428, 10, 10); break;
                case EffectIcon.Popularity:
                    // draw smiley (which is actualy 13x13, so we use a smaller scaling factor)
                    Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new Vector2(location.X, location.Y + 2), new Rectangle(157, 515, 13, 13), Color.White, 0f, Vector2.Zero, 2f, flipped: false, 0.95f);
                    break;
                // icons from custom texture
                case EffectIcon.MaxHealth:
                    texture = EffectHelper.Textures.LooseSprites;
                    rect = new Rectangle(0, 0, 10, 10);
                    break;
                case EffectIcon.Yoba:
                    texture = EffectHelper.Textures.LooseSprites;
                    rect = new Rectangle(10, 0, 10, 10);
                    break;
                case EffectIcon.TreasureChest: rect = new Rectangle(137, 412, 10, 11); break;
                case EffectIcon.Animal_Chicken:                    
                    Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new Vector2(location.X, location.Y + 5), new Rectangle(0, 448, 32, 16), Color.White, 0f, Vector2.Zero, 1.2f, flipped: false, 0.95f);
                    break;
                case EffectIcon.Animal_Cow:                    
                    Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new Vector2(location.X, location.Y + 2), new Rectangle(40, 449, 17, 14), Color.White, 0f, Vector2.Zero, 1.8f, flipped: false, 0.95f);
                    break;
                case EffectIcon.Glow:
                    texture = EffectHelper.Textures.Emojis;
                    rect = new Rectangle(9, 63, 9, 9);                    
                    break;
                case EffectIcon.Person_Lewis:                    
                    Utility.drawWithShadow(spriteBatch, EffectHelper.Textures.LooseSprites, new Vector2(location.X, location.Y), new Rectangle(20, 0, 12, 13), Color.White, 0f, Vector2.Zero, 2f, flipped: false, 0.95f);                    
                    break;
            }

            if (rect.HasValue)
            {                              
                Utility.drawWithShadow(spriteBatch, texture, location, rect.Value, Color.White, 0f, Vector2.Zero, 3f, flipped: false, 0.95f);
            }            
        }
    }
}
