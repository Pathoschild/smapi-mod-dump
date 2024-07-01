/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Framework.Particle.UpdateBehaviour;
using AchtuurCore.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;

namespace AchtuurCore.Framework.Particle;
public class Particle : ICloneable
{
    /// <summary>
    /// Current speed of particle
    /// </summary>
    protected Vector2 speed;

    
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
    protected const int c_MaxLifeSpanTicks = 60 * 6;

    /// <summary>
    /// Number of ticks this particle has been alive
    /// </summary>
    protected int ticks = 0;

    protected Color color = Color.White;
    protected float particleColorOpacity = 0.7f;
    protected Vector2 size = new Vector2(10f, 10f);

    protected LightSource lightSource;
    protected float lightRadius = 0.0069f; // multiply this by size.magnitude squared to get light radius

    protected ParticleStateController m_StateController = new ParticleStateController();

    protected bool ShouldDraw => m_StateController.Started && !m_StateController.Finished;


    public Particle() : this(Color.White, Vector2.One) { }

    public Particle(Color color, Vector2 size)
    {
        this.color = color;
        this.size = size;

        long id = 0;
        if (Context.IsWorldReady)
            id = Game1.player.UniqueMultiplayerID;

        lightSource = new LightSource(10, Vector2.Zero, this.size.LengthSquared() * lightRadius, this.color, LightSource.LightContext.None, id);
    }

    public void AddState<S>(S state)
        where S : IParticleState
    {
        m_StateController.AddState(state);
    }

    public void AddState<S>()
        where S : IParticleState, new()
    {
        m_StateController.AddState<S>();
    }

    protected virtual void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        ticks++;
        m_StateController.Update();
        
        if (m_StateController.Finished || ticks > c_MaxLifeSpanTicks)
        {
            OnReachTargetPosition();
        }
    }
    protected void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
    {
        DrawToScreen(e.SpriteBatch);
    }

    private void OnPlayerWarp(object sender, WarpedEventArgs e)
    {
        Destroy();
    }

    public virtual void Start()
    {
        ModEntry.Instance.Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        ModEntry.Instance.Helper.Events.Display.RenderedWorld += OnRenderedWorld;
        ModEntry.Instance.Helper.Events.Player.Warped += OnPlayerWarp;

        m_StateController.Start();
    }    

    public virtual void Reset()
    {
        m_StateController.Reset();
        Start();
    }
    public object Clone()
    {
        return MemberwiseClone();
    }

    public void Destroy()
    {
        // Unsubscribe from events
        ModEntry.Instance.Helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
        ModEntry.Instance.Helper.Events.Display.RenderedWorld -= OnRenderedWorld;
        // pray to god nothing references it anymore and the garbage collector fixes my problems
    }

    public virtual void SetColor(Color color)
    {
        this.color = color;
        lightSource.color.Value = color;
    }

    public virtual void SetSize(Vector2 size)
    {
        this.size = size;
        lightSource.radius.Value = size.LengthSquared() * lightRadius;
    }

    public virtual void SetInitialPosition(Vector2 initialPosition)
    {
        m_StateController.SetInitialPosition(initialPosition);
    }

    /// <summary>
    /// Set target position
    /// </summary>
    /// <param name="position">Target position</param>
    public void SetTargetPosition(Vector2 position)
    {
        m_StateController.SetTargetPosition(position);
    }

    /// <summary>
    /// Set target Farmer, the particle will try and reach the farmer's position
    /// </summary>
    /// <param name="farmer">Farmer who's position will be targeted</param>
    public void SetTargetFarmer(Farmer farmer)
    {
        m_StateController.SetTargetFarmer(farmer);
    }

    protected void OnReachTargetPosition()
    {
        Destroy();
    }

    public virtual void DrawToScreen(SpriteBatch spriteBatch)
    {
        if (!ShouldDraw)
            return;

        Vector2 screenCoords = Drawing.GetPositionScreenCoords(m_StateController.Position);
        spriteBatch.DrawRect(screenCoords, size, color * particleColorOpacity);
        spriteBatch.DrawBorderNoCorners(screenCoords, size, color * (1 / particleColorOpacity), bordersize: 1);

        lightSource.position.Value = screenCoords;
        spriteBatch.DrawLightSource(lightSource);

        Debug.DebugOnlyExecute(() =>
        {
            DebugDrawCoords(spriteBatch);
            //DebugDrawDirection(spriteBatch);
        });
    }

    private void DebugDrawCoords(SpriteBatch spriteBatch)
    {
        Vector2 initialCoords = Drawing.GetPositionScreenCoords(m_StateController.InitialPosition);
        Vector2 targetCoords = Drawing.GetPositionScreenCoords(m_StateController.TargetPosition);
        spriteBatch.DrawRect(initialCoords, size, Color.Yellow);
        spriteBatch.DrawRect(targetCoords, size, Color.Magenta);
    }

    private void DebugDrawDirection(SpriteBatch spriteBatch)
    {
        Vector2 velocity = m_StateController.Position + m_StateController.Velocity;
        Vector2 accel = m_StateController.Position + m_StateController.Acceleration;

        // draw velocity
        Vector2 screenCoords = Drawing.GetPositionScreenCoords(m_StateController.Position);
        Vector2 targetCoords = Drawing.GetPositionScreenCoords(velocity);
        spriteBatch.DrawLine(screenCoords, targetCoords, Color.Red, 1);

        // draw accel at tail of velocity
        targetCoords = Drawing.GetPositionScreenCoords(accel);
        spriteBatch.DrawLine(screenCoords, targetCoords, Color.Cyan, 1);
    }
}
