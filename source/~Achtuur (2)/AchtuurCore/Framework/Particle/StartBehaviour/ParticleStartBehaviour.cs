/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchtuurCore.Framework.Particle.StartBehaviour;
/// <summary>
/// Particle start behaviour only sets the particle's initial state somehow and then finishes.
/// </summary>
public abstract class ParticleStartBehaviour : IParticleState
{
    protected Kinematics m_Kinematics = new Kinematics();
    protected bool m_Finished = false;
    protected Farmer m_TargetFarmer = null;

    public ParticleStartBehaviour()
    {
    }

    public override void Start()
    {
        m_Finished = true;
    }

    public override void Update()
    {
    }
    public override bool IsFinished()
    {
        return m_Finished;
    }

    public override void Reset()
    {
        m_Finished = false;
    }

    public override Kinematics GetKinematics()
    {
        return m_Kinematics;
    }

    public override void SetKinematics(Kinematics kinematics)
    {
        m_Kinematics = kinematics;
    }
}
