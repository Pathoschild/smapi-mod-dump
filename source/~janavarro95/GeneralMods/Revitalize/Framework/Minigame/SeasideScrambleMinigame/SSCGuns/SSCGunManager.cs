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
using Revitalize.Framework.Minigame.SeasideScrambleMinigame.SSCStatusEffects;
using StardustCore.Animations;

namespace Revitalize.Framework.Minigame.SeasideScrambleMinigame.SSCGuns
{
    public class SSCGunManager
    {

        public Dictionary<string, SSCGun> guns;

        public SSCGunManager()
        {
            this.guns = new Dictionary<string, SSCGun>();

            this.guns.Add("Default", new SSCGuns.SSCGun(new StardustCore.Animations.AnimatedSprite("MyFirstGun", Vector2.Zero, new AnimationManager(SeasideScramble.self.textureUtils.getExtendedTexture("Guns", "BasicGun"), new Animation(0, 0, 16, 16)), Color.White), SeasideScramble.self.entities.projectiles.getDefaultProjectile(this, Vector2.Zero, Vector2.Zero, 4f, new Rectangle(0, 0, 16, 16), Color.White, 4f, 300), 10, 1000, 3000));
            this.guns.Add("HeatWave", new SSCGuns.SSCGun_HeatWave(new StardustCore.Animations.AnimatedSprite("HeatWave", Vector2.Zero, new AnimationManager(SeasideScramble.self.textureUtils.getExtendedTexture("Guns", "BasicGun"), new Animation(0, 0, 18, 16)), Color.Red), SeasideScramble.self.entities.projectiles.getFireProjectile(this, Vector2.Zero, Vector2.Zero, 1.5f, new Rectangle(0, 0, 18, 16), Color.White, 3f, 3, SE_Burn.SpawnBurnEffect(Vector2.Zero, 3 * 1000, 1000, 1.00d, 2),150), 20, 200, 5000));
            this.guns.Add("Icicle", new SSCGuns.SSCGun(new StardustCore.Animations.AnimatedSprite("Icicle", Vector2.Zero, new AnimationManager(SeasideScramble.self.textureUtils.getExtendedTexture("Guns", "BasicGun"), new Animation(0, 0, 16, 16)), Color.SkyBlue), SeasideScramble.self.entities.projectiles.getIcicleProjectile(this, Vector2.Zero, Vector2.Zero, 5.5f, new Rectangle(0, 0, 16, 16), Color.White, 3f, 5, 200), 5, 1000, 5000));
        }

        /// <summary>
        /// Gets a gun from the list of made guns.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SSCGun getGun(string name)
        {
            if (this.guns.ContainsKey(name))
            {
                return this.guns[name].getCopy();
            }
            else
            {
                return null;
            }
        }

        public SSCGun getDefaultGun()
        {
            return this.getGun("Default");
        }

    }
}
