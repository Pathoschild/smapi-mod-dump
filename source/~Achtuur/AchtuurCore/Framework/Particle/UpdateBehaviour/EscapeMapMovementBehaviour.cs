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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchtuurCore.Framework.Particle.UpdateBehaviour;
public class EscapeMapMovementBehaviour : MovementBehaviour
{
    public EscapeMapMovementBehaviour() : base()
    {
    }

    public EscapeMapMovementBehaviour(Vector2 initialPosition, Vector2 targetPosition) : base(initialPosition, targetPosition)
    {
    }

    public override void Start()
    {
        SetTargetPosition(Vector2.Zero);
    }

    public override void SetTargetPosition(Vector2 _targetPosition)
    {
        Ellipse screen = Tiles.GetVisibleAreaEllipse(expand: 10);
        float angle = RandomUtil.GetFloat(-0.1, 0.1) - (float)Math.PI / 2f;
        //m_Kinematics.TargetPosition = RandomUtil.GetPointOnEllipse(screen);
        m_Kinematics.TargetPosition = screen.PointAt(angle);
    }

    public override Vector2 GetTargetPosition()
    {
        return m_Kinematics.TargetPosition;
    }
}
