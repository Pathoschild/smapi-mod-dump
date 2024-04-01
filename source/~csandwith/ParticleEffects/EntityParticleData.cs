/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ParticleEffects
{
    public class EntityParticleData
    {
        public Dictionary<string, List<ParticleData>> particleDict  = new Dictionary<string,List<ParticleData>>();
    }

    public class ParticleData
    {
        public Vector2 direction;
        public int age;
        public float rotation;
        public float rotationRate;
        public int lifespan;
        public float scale;
        public float alpha;
        public int option = -1;
        public Vector2 position;
    }
}