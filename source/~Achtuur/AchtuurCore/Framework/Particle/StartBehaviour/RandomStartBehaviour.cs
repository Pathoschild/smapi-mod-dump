/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchtuurCore.Framework.Particle.StartBehaviour;
public class RandomStartBehaviour : ParticleStartBehaviour
{
    public RandomStartBehaviour()
    {
    }

    public override void Start()
    {
        base.Start();
        m_Kinematics.Velocity = RandomUtil.GetUnitVector() * Kinematics.c_MaxVelocity;
    }
}
