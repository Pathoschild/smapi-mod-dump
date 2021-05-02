/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Revitalize.Framework.Minigame.SeasideScrambleMinigame.Interfaces;
using StardustCore.Animations;
using StardustCore.UIUtilities.SpriteFonts.Components;

namespace Revitalize.Framework.Minigame.SeasideScrambleMinigame.SSCEntities.Generic
{
    public class SignPost : ISSCLivingEntity
    {
        public float MovementSpeed { get; set; }
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }
        public Rectangle HitBox { get; set; }

        public AnimatedSprite sprite;
        Vector2 position;

        public TexturedString displayString;

        public Vector2 Position
        {
            get
            {
                return this.position;
            }
            set
            {
                this.position = value;
                Rectangle hitbox = this.HitBox;
                hitbox.X = (int)this.position.X;
                hitbox.Y = (int)this.position.Y;
            }
        }
        public Color color
        {
            get
            {
                return this.sprite.color;
            }
            set
            {
                this.sprite.color = value;
            }
        }

        public SignPost(Vector2 Position)
        {
            this.sprite = new AnimatedSprite("SignPost", Position, new AnimationManager(SeasideScramble.self.textureUtils.getExtendedTexture("Entities", "SignPost"), new Animation(0, 0, 16, 16)), Color.White);

        }

        public void update(GameTime time)
        {
            
        }
        public void draw(SpriteBatch b)
        {
            this.sprite.draw(b);
            if (this.displayString != null)
            {
                this.displayString.draw(b, new Rectangle(0, 0, 16, 16), 0f);
            }
        }
    }
}
