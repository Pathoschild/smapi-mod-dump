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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchtuurCore.Framework.Particle;
public struct Kinematics
{
    public static Vector2 c_FarmerPositionCorrection = new Vector2(Game1.tileSize * 0.5f, 0f);
    /// <summary>
    /// Maximum speed of particle (in in-game coordinates)
    /// </summary>
    public static Vector2 c_MaxVelocity = new Vector2(15f, 13f);

    /// <summary>
    /// Maximum Acceleration of particle (in in-game coordinates)
    /// </summary>
    public static Vector2 c_MaxAcceleration = new Vector2(4f, 3.2f);

    public const float c_AccelPerTick = 0.05f;

    public Vector2 Position = Vector2.Zero;
    public Vector2 Velocity = Vector2.Zero;
    public Vector2 Acceleration = Vector2.Zero;

    public Vector2 InitialPosition = Vector2.Zero;
    public Vector2 TargetPosition = Vector2.Zero;
    public Farmer TargetFarmer = null;

    public Func<float> AccelerationMultiplier = () => 1f;

    private float GetAccelerationMultiplier() => AccelerationMultiplier is null ? 1f : AccelerationMultiplier();

    public Kinematics()
    {
    }

    public Kinematics(Vector2 position, Vector2 velocity, Vector2 acceleration)
    {
        Position = position;
        Velocity = velocity;
        Acceleration = acceleration;
    }

    public void Reset()
    {
        Position = InitialPosition;
        Velocity = new Vector2();
        Acceleration = new Vector2();
    }

    public Vector2 GetNextPosition(Vector2 direction)
    {
        direction.Normalize();
        // increase acceleration towards target direction
        float tick_multiplier = GetAccelerationMultiplier();
        Vector2 accel = tick_multiplier * c_AccelPerTick * direction;
        Vector2 random_offset = RandomUtil.GetUnitVector() * accel.Length() * .25f;
        accel += random_offset;
        SetAcceleration(accel);
        SetVelocity(Velocity + Acceleration);

        // return new position based on speed
        return Position + Velocity;
    }

    public void SetAcceleration(Vector2 accel)
    {
        //if (accel.LengthSquared() > 1)
        accel.Normalize();

        accel *= c_MaxAcceleration;
        Acceleration = accel;
    }

    public void SetVelocity(Vector2 speed)
    {
        //if (speed.LengthSquared() > 1)
        speed.Normalize();

        speed *= c_MaxVelocity;
        Velocity = speed;
    }
}
