/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

namespace Pong.Framework.Game.States
{
    internal class VelocityState : IState<VelocityState>
    {
        public int XVelocity { get; set; } = -4;
        public int YVelocity { get; set; } = -8;

        public VelocityState()
        {
            this.Reset();
        }

        public void Reset()
        {
            this.XVelocity = -4;
            this.YVelocity = 8;
        }

        public void SetState(VelocityState state)
        {
            this.XVelocity = state.XVelocity;
            this.YVelocity = state.YVelocity;
        }

        public void Invert()
        {
            this.XVelocity *= -1;
            this.YVelocity *= -1;
        }
    }
}
