/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using System;

namespace AchtuurCore.Framework;

public enum ParticleStartBehaviour
{
    /// <summary>
    /// No special behaviour, particle will start accelerating towards target on spawn
    /// </summary>
    None,
    /// <summary>
    /// Particle gets speed in random direction on start
    /// </summary>
    Random,
}

public abstract class Particle : ICloneable
{
    /// <summary>
    /// Maximum speed of particle (in in-game coordinates)
    /// </summary>
    protected Vector2 maxSpeed = new Vector2(15f, 13f);

    /// <summary>
    /// Current speed of particle
    /// </summary>
    protected Vector2 speed;

    protected Vector2 maxAccel = new Vector2(4f, 3.2f);
    /// <summary>
    /// Acceleration increase per tick
    /// </summary>
    protected float accel_per_tick = 0.05f;

    /// <summary>
    /// Current acceleration
    /// </summary>
    protected Vector2 accel;

    /// <summary>
    /// Maximum number of ticks this particle can stay alive for
    /// </summary>
    protected int maxLifeSpanTicks = 60 * 5;

    /// <summary>
    /// Number of ticks this particle has been alive
    /// </summary>
    protected int ticks = 0;

    protected Vector2 Position { get; set; }
    protected Vector2 intialPosition;
    protected Vector2 targetPosition;
    protected GameLocation spawnGameLocation;

    protected Farmer targetFarmer;

    protected Color color = Color.White;
    protected float particleColorOpacity = 0.7f;
    protected Vector2 size = new Vector2(10f, 10f);

    public ParticleStartBehaviour StartBehaviour { get; set; } = ParticleStartBehaviour.None;

    public bool Started { get; protected set; }
    public bool ReachedTarget { get; protected set; }

    public Particle(Vector2 position, Vector2 targetPosition, Color color, Vector2 size)
    {
        this.Position = position;
        this.intialPosition = position;
        this.targetPosition = targetPosition;
        spawnGameLocation = Game1.currentLocation;
        this.speed = new Vector2();
        this.accel = new Vector2();
        ReachedTarget = false;

        this.color = color;
        this.size = size;
    }
    protected virtual void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        this.ticks++;
        this.accel_per_tick = this.ticks / 10f;

        //this.maxSpeed.Normalize();
        //this.maxSpeed =  this.maxSpeed * (15f + this.ticks / 5f);

        this.Position = GetNextPosition();
        if (hasReachedTargetPosition() || this.spawnGameLocation != Game1.currentLocation)
        {
            OnReachTargetPosition();
        }
    }
    protected void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
    {
        this.DrawToScreen(e.SpriteBatch);
    }

    public virtual void Start()
    {
        this.Started = true;
        ModEntry.Instance.Helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        ModEntry.Instance.Helper.Events.Display.RenderedWorld += this.OnRenderedWorld;

        this.DoStartupBehaviour();
    }

    public virtual void Reset()
    {
        this.Position = intialPosition;
        this.speed = new Vector2();
        this.accel = new Vector2();
        this.ReachedTarget = false;
        this.Start();
    }
    public object Clone()
    {
        return MemberwiseClone();
    }

    public void Destroy()
    {
        // Unsubscribe from events
        ModEntry.Instance.Helper.Events.GameLoop.UpdateTicked -= this.OnUpdateTicked;
        ModEntry.Instance.Helper.Events.Display.RenderedWorld -= this.OnRenderedWorld;

        // pray to god nothing references it anymore and the garbage collector fixes my problems
    }

    public virtual void SetColor(Color color)
    {
        this.color = color;
    }

    public virtual void SetSize(Vector2 size)
    {
        this.size = size;
    }

    public virtual void SetInitialPosition(Vector2 initialPosition)
    {
        this.intialPosition = initialPosition;
        this.Position = initialPosition;
        spawnGameLocation = Game1.currentLocation;
    }

    /// <summary>
    /// Set target position
    /// </summary>
    /// <param name="position">Target position</param>
    public void SetTarget(Vector2 position)
    {
        this.targetPosition = position;
        spawnGameLocation = Game1.currentLocation;
    }

    /// <summary>
    /// Set target Farmer, the particle will try and reach the farmer's position
    /// </summary>
    /// <param name="farmer">Farmer who's position will be targeted</param>
    public void SetTarget(Farmer farmer)
    {
        this.targetFarmer = farmer;
        this.targetPosition = farmer.Position;
        this.spawnGameLocation = farmer.currentLocation;
    }

    protected Vector2 GetNextPosition()
    {
        // Update position if target farmer moves
        // Dont update if location changes, so the particles moves to the last position target was in before swapping locations
        if (this.targetFarmer is not null && this.spawnGameLocation == this.targetFarmer.currentLocation)
            this.targetPosition = this.targetFarmer.Position;

        Vector2 targetDirection = targetPosition - Position;
        targetDirection.Normalize();

        // increase acceleration towards target direction
        this.accel = accel_per_tick * targetDirection;
        this.accel = Vector2.Clamp(this.accel, -maxAccel, maxAccel);

        // increase speed based on acceleration
        this.speed += this.accel;
        this.speed = Vector2.Clamp(this.speed, -maxSpeed, maxSpeed);

        // return new position based on speed
        return this.Position + this.speed;
    }

    protected bool hasReachedTargetPosition()
    {
        // Distance scales from 0 to 64 based on percentage of max speed
        float d = 72f * this.accel.LengthSquared() / this.maxAccel.LengthSquared();
        return this.ticks > this.maxLifeSpanTicks ||
            (this.Position - this.targetPosition).LengthSquared() < d * d;
    }
    protected void OnReachTargetPosition()
    {
        this.ReachedTarget = true;

        // place out of bounds
        this.speed = new Vector2(0, 0);
        this.accel = new Vector2(0, 0);
        this.Position = new Vector2(3000, 3000);

        this.Destroy();
    }


    protected virtual void DoStartupBehaviour()
    {
        switch (this.StartBehaviour)
        {
            case ParticleStartBehaviour.None: break;
            case ParticleStartBehaviour.Random:
                // start with random speed to curve the particle a bit (cool effect)
                // generate random direction between 0.5 and -0.5 for x and y
                Random r = new Random();
                this.speed = new Vector2((float)r.NextDouble() - 0.5f, (float)r.NextDouble() - 0.5f);
                // Set speed to 0.1 max speed;
                this.speed.Normalize();
                this.speed *= maxSpeed * 1f;
                break;
        }
    }

    public virtual void DrawToScreen(SpriteBatch spriteBatch)
    {
        if (this.ReachedTarget || !this.Started)
            return;

        Vector2 screenCoords = Drawing.GetPositionScreenCoords(Position);

        Drawing.DrawLine(spriteBatch, screenCoords, this.size, this.color * particleColorOpacity);

        Drawing.DrawBorderNoCorners(spriteBatch, screenCoords, this.size, this.color * (1 / particleColorOpacity), bordersize: 1);

        Debug.DebugOnlyExecute(() =>
        {
            Vector2 initialCoords = Drawing.GetPositionScreenCoords(this.intialPosition);
            Vector2 targetCoords = Drawing.GetPositionScreenCoords(this.targetPosition);
            Drawing.DrawLine(spriteBatch, initialCoords, this.size, Color.Yellow);
            Drawing.DrawLine(spriteBatch, targetCoords, this.size, Color.Magenta);

        });
    }
}
