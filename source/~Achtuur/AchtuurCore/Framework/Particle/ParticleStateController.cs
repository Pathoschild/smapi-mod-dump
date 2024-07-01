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
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchtuurCore.Framework.Particle;

public class ParticleStateController
{
    List<IParticleState> m_States = new();
    int current_state_index = 0;
    Kinematics m_InitialKinematics = new();
    Farmer m_TargetFarmer = null;

    IParticleState CurrentState => m_States[current_state_index];
    IParticleState NextState => (current_state_index < m_States.Count - 1) ? m_States[current_state_index + 1] : null;

    public bool Started = false;
    public bool Finished => current_state_index >= m_States.Count;

    public Kinematics Kinematics => current_state_index < m_States.Count ? CurrentState.GetKinematics() : m_InitialKinematics;

    public Vector2 Position => Kinematics.Position;
    public Vector2 Velocity => Kinematics.Velocity;
    public Vector2 Acceleration => Kinematics.Acceleration;
    public Vector2 InitialPosition => Kinematics.InitialPosition;
    public Vector2 TargetPosition => Kinematics.TargetPosition;

    public ParticleStateController()
    {
    }

    public void AddState<S>(S state)
        where S: IParticleState
    {
        m_States.Add(state);
    }

    public void AddState<S>()
        where S: IParticleState, new()
    {
        S state = new S();
        state.SetKinematics(m_InitialKinematics);
        state.SetTargetFarmer(m_TargetFarmer);
        m_States.Add(state);
    }

    public void SetInitialPosition(Vector2 pos)
    {
        m_InitialKinematics.InitialPosition = pos;
        m_InitialKinematics.Position = pos;
        foreach (IParticleState state in m_States)
            state.SetInitialPosition(pos);
    }

    public void SetTargetPosition(Vector2 target)
    {
        m_InitialKinematics.TargetPosition = target;
        foreach (IParticleState state in m_States)
            state.SetTargetPosition(target);
    }

    public void SetTargetFarmer(Farmer farmer)
    {
        m_TargetFarmer = farmer;
        foreach (IParticleState state in m_States)
            state.SetTargetFarmer(farmer);
    }

    public void Start()
    {
        if (m_States.Count == 0)
            AddState<MovementBehaviour>();
        Started = true;
        CurrentState.Start();
    }

    public void Reset()
    {
        foreach (IParticleState state in m_States)
            state.Reset();
    }

    public void Update()
    {
        if (m_States.Count == 0)
            return;

        CurrentState.Update();
        if (CurrentState.IsFinished())
        {
            SwitchToNextState();
            current_state_index++;
        }
    }

    private void SwitchToNextState()
    {
        if (NextState is null)
            return;
        Kinematics kinematics = CurrentState.GetKinematics();
        NextState.SetKinematics(kinematics);
        NextState.SetInitialPosition(kinematics.Position);
        NextState.SetTargetPosition(CurrentState.GetTargetPosition());
        NextState.Start();
    }
}
