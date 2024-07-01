/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Extensions;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Enchantments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchtuurCore.Framework.Particle.UpdateBehaviour;
public class OrbitMovementBehaviour : MovementBehaviour
{
    float startAngle = 0f;
    float t = 0f;
    Circle moveCircle = new Circle();

    int MaxRotations = 1;

    private int RotationsMade => (int) (0.5*t / Math.PI);
    public OrbitMovementBehaviour()
    {
    }

    public override void Start()
    {
        base.Start();

        Vector2 r = m_Kinematics.Position - GetTargetPosition();
        if (r.Length() == 0)
            r = Vector2.One * Game1.tileSize;
        moveCircle = new Circle(GetTargetPosition(), r.Length());
        startAngle = r.Angle();
        m_Kinematics.Acceleration = Vector2.Zero;
        m_Kinematics.Velocity = Vector2.Zero;
    }

    public override bool IsFinished()
    {
        return base.IsFinished() && RotationsMade >= MaxRotations;
    }

    protected override Vector2 GetNextPosition()
    {
        if (RotationsMade >= MaxRotations)
            return base.GetNextPosition();

        moveCircle.Center = GetTargetPosition();
        // point should travel along circle with max speed
        // arc_length = r * theta -> theta = arc_length / r
        float dt = Kinematics.c_MaxVelocity.Length() / moveCircle.Radius;
        t += dt;
        Vector2 next_pos = moveCircle.PointAt(startAngle + t);

        return next_pos;

    }
}
