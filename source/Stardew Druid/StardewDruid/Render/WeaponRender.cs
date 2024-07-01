/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace StardewDruid.Render
{
    public class WeaponRender
    {

        public enum weapons
        {
            none,
            sword,
            cutlass,
            axe,
            estoc,
            carnyx,
            scythe,

            bazooka,
        }

        public Texture2D weaponTexture;
        public Dictionary<int, List<Rectangle>> weaponFrames;
        public Dictionary<int, List<float>> weaponOffsets;

        public Texture2D swipeTexture;
        public Dictionary<int, List<Rectangle>> swipeFrames;
        public Dictionary<int, List<float>> swipeOffsets;

        public Texture2D firearmTexture;
        public Dictionary<int, List<Rectangle>> firearmFrames;

        public bool melee;
        public bool firearm;

        public WeaponRender()
        {

            melee = true;

            weaponTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "WeaponSword.png"));

            weaponFrames = new()
            {
                [256] = new()
                {
                    // 1
                    new Rectangle(0, 0, 64, 64),
                    new Rectangle(64, 0, 64, 64),
                    new Rectangle(128, 0, 64, 64),
                    new Rectangle(192, 0, 64, 64),
                    // 4
                    new Rectangle(0, 192, 64, 64),
                    new Rectangle(64, 192, 64, 64),
                    new Rectangle(128, 192, 64, 64),
                    new Rectangle(192, 192, 64, 64),
                },
                [288] = new()
                {
                    // 3
                    new Rectangle(0, 128, 64, 64),
                    new Rectangle(64, 128, 64, 64),
                    new Rectangle(128, 128, 64, 64),
                    new Rectangle(192, 128, 64, 64),
                    // 5
                    new Rectangle(0, 256, 64, 64),
                    new Rectangle(64, 256, 64, 64),
                    new Rectangle(128, 256, 64, 64),
                    new Rectangle(192, 256, 64, 64),
                },
                [320] = new()
                {
                    // 2
                    new Rectangle(0, 64, 64, 64),
                    new Rectangle(64, 64, 64, 64),
                    new Rectangle(128, 64, 64, 64),
                    new Rectangle(192, 64, 64, 64),
                    // 6
                    new Rectangle(0, 320, 64, 64),
                    new Rectangle(64, 320, 64, 64),
                    new Rectangle(128, 320, 64, 64),
                    new Rectangle(64, 320, 64, 64),
                },

            };

            firearmFrames = new()
            {
                [0] = new()
                {
                    new(),
                    new(),
                    new(),
                    new(),
                    new(0, 128, 64, 64),
                    new(64, 128, 64, 64),

                },
                [32] = new()
                {
                    new(),
                    new(),
                    new(),
                    new(),
                    new(0, 64, 64, 64),
                    new(64, 64, 64, 64),

                },
                [64] = new()
                {
                    new(),
                    new(),
                    new(),
                    new(),
                    new(0, 0, 64, 64),
                    new(64, 0, 64, 64),

                },
            };

            weaponOffsets = new()
            {
                [256] = new()
                {
                    // 1
                    -0.0005f,
                    0.0005f,
                    0.0005f,
                    0.0005f,
                    // 4
                    0.0005f,
                    0.0005f,
                    0.0005f,
                    -0.0005f,
                },
                [288] = new()
                {
                    // 3
                    -0.0005f,
                    -0.0005f,
                    -0.0005f,
                    -0.0005f,
                    // 5
                    -0.0005f,
                    0.0005f,
                    0.0005f,
                    -0.0005f,
                },
                [320] = new()
                {
                    // 2
                    0.0005f,
                    0.0005f,
                    0.0005f,
                    0.0005f,
                    // 6
                    0.0005f,
                    0.0005f,
                    -0.0005f,
                    0.0005f,
                },
            };

            swipeTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "WeaponSwipe.png"));

            swipeFrames = new()
            {
                [256] = new()
                {
                    new Rectangle(0, 0, 64, 64),
                    new Rectangle(64, 0, 64, 64),
                    new Rectangle(128, 0, 64, 64),
                    new Rectangle(192, 0, 64, 64),
                    new Rectangle(0, 192, 64, 64),
                    new Rectangle(64, 192, 64, 64),
                    new Rectangle(128, 192, 64, 64),
                    new Rectangle(192, 192, 64, 64),
                },
                [288] = new()
                {
                    new Rectangle(0, 128, 64, 64),
                    new Rectangle(64, 128, 64, 64),
                    new Rectangle(128, 128, 64, 64),
                    new Rectangle(192, 128, 64, 64),
                    new Rectangle(0, 256, 64, 64),
                    new Rectangle(64, 256, 64, 64),
                    new Rectangle(128, 256, 64, 64),
                    new Rectangle(192, 256, 64, 64),
                },
                [320] = new()
                {
                    new Rectangle(0, 64, 64, 64),
                    new Rectangle(64, 64, 64, 64),
                    new Rectangle(128, 64, 64, 64),
                    new Rectangle(192, 64, 64, 64),

                },
            };


            swipeOffsets = new()
            {
                [256] = new()
                {
                    // 1
                    -0.0005f,
                    -0.0005f,
                    -0.0005f,
                    0.0005f,
                    // 4
                    -0.0005f,
                    0.0005f,
                    0.0005f,
                    -0.0005f,
                },
                [288] = new()
                {
                    // 3
                    0.0005f,
                    -0.0005f,
                    -0.0005f,
                    -0.0005f,
                    // 5
                    -0.0005f,
                    0.0005f,
                    0.0005f,
                    -0.0005f,
                },
                [320] = new()
                {
                    // 2
                    0.0005f,
                    -0.0005f,
                    0.0005f,
                    0.0005f,
                },
            };

        }

        public virtual void LoadWeapon(weapons weapon)
        {

            switch (weapon)
            {
                default:
                case weapons.none:
                    melee = false;
                    break;

                case weapons.sword:
                    weaponTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "WeaponSword.png"));
                    melee = true;
                    break;

                case weapons.cutlass:
                    weaponTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "WeaponCutlass.png"));
                    melee = true;
                    break;

                case weapons.axe:
                    weaponTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "WeaponAxe.png"));
                    melee = true;
                    break;

                case weapons.estoc:
                    weaponTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "WeaponEstoc.png"));
                    melee = true;
                    break;

                case weapons.carnyx:
                    weaponTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "WeaponCarnyx.png"));
                    melee = true;
                    break;

                case weapons.scythe:
                    weaponTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "WeaponScythe.png"));
                    melee = true;
                    break;

                case weapons.bazooka:
                    firearmTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "FirearmBazooka.png"));
                    firearm = true;
                    break;

            }

        }

        public virtual void DrawWeapon(SpriteBatch b, Vector2 spriteVector, float drawLayer, WeaponAdditional additional)
        {

            if (!melee) { return; }

            b.Draw(
                 weaponTexture,
                 spriteVector - new Vector2(64, 64f),
                 weaponFrames[additional.source.Y][additional.source.X == 0 ? 0 : additional.source.X / 32],
                 Color.White,
                 0f,
                 Vector2.Zero,
                additional.scale,
                additional.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                drawLayer + weaponOffsets[additional.source.Y][additional.source.X == 0 ? 0 : additional.source.X / 32]
            );

        }

        public virtual void DrawSwipe(SpriteBatch b, Vector2 spriteVector, float drawLayer, WeaponAdditional additional)
        {

            if (!melee) { return; }

            int swipeIndex = additional.source.X == 0 ? 0 : additional.source.X / 32;

            if (swipeFrames[additional.source.Y].Count <= swipeIndex)
            {

                return;

            }

            b.Draw(
                 swipeTexture,
                 spriteVector - new Vector2(64, 64f),
                 swipeFrames[additional.source.Y][swipeIndex],
                 Color.White * 0.65f,
                 0f,
                 Vector2.Zero,
                additional.scale,
                additional.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                drawLayer + swipeOffsets[additional.source.Y][swipeIndex]
            );

        }
        public virtual void DrawFirearm(SpriteBatch b, Vector2 spriteVector, float drawLayer, WeaponAdditional additional)
        {

            if (!firearm) { return; }

            b.Draw(
                 firearmTexture,
                 spriteVector - new Vector2(64, 64f),
                 firearmFrames[additional.source.Y][additional.source.X == 0 ? 0 : additional.source.X / 32],
                 Color.White,
                 0f,
                 Vector2.Zero,
                additional.scale,
                additional.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                drawLayer + 0.002f
            );

        }

    }


    public class WeaponAdditional
    {

        public Microsoft.Xna.Framework.Rectangle source;

        public bool flipped;

        public float scale = 4f;

    }

}
