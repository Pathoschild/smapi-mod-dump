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
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.GameData.FloorsAndPaths;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchtuurCore.Framework.Particle.UpdateBehaviour;
public class MovementBehaviour : IParticleState
{
    protected Kinematics m_Kinematics;
    public MovementBehaviour()
    {
    }

    public MovementBehaviour(Vector2 initialPosition, Vector2 targetPosition)
    {
        SetInitialPosition(initialPosition);
        SetTargetPosition(targetPosition);
    }

    public override Kinematics GetKinematics()
    {
        return m_Kinematics;
    }

    public override void SetKinematics(Kinematics kinematics)
    {
        m_Kinematics = kinematics;
    }
    public override void Start()
    {
    }

    public override void Reset()
    {
        m_Kinematics.Reset();
    }

    public override void Update()
    {
        m_Kinematics.Position = GetNextPosition();
    }

    public override bool IsFinished()
    {
        // Distance allowed scales with acceleration
        float acc_perc = Math.Min(1.0f, m_Kinematics.Acceleration.LengthSquared() / Kinematics.c_MaxAcceleration.LengthSquared());
        float d = 2f * Game1.tileSize * acc_perc;
        float tile_distance = (m_Kinematics.Position- GetTargetPosition()).LengthSquared();
        return tile_distance < d * d;
    }
    protected virtual Vector2 GetNextPosition()
    {
        Vector2 targetDirection = GetTargetPosition() - m_Kinematics.Position;
        return m_Kinematics.GetNextPosition(targetDirection);
    }
}
