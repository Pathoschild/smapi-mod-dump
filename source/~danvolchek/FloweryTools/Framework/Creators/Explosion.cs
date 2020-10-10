/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using FloweryTools.Framework.Flowerers;
using FloweryTools.ParticleCreator;
using StardewValley;

namespace FloweryTools.Framework.Creators
{
    // Used for explosions (e.g. charging a tool, bobber landing)
    internal class Explosion : BaseCreator, IParticleCreator
    {
        public Explosion(FlowerHelper helper) : base(helper)
        {

        }

        public void CreateParticles(GameLocation location, int frameIndex)
        {
            for (int i = 0; i < this.helper.rand.Next(1, 4); i++)
                this.helper.AddFlower(this.helper.ApplyJitter(this.helper.GetOffset(1), 10), this.helper.ApplyJitter(this.helper.GetExplosionMotion(), 0.1f), location);
        }
    }
}
