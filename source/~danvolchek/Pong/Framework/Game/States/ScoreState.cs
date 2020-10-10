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
    internal class ScoreState : IState<ScoreState>
    {
        public int PlayerOneScore { get; set; }
        public int PlayerTwoScore { get; set; }

        public ScoreState()
        {
            this.Reset();
        }

        public void Reset()
        {
            this.PlayerOneScore = this.PlayerTwoScore = 0;
        }

        public void SetState(ScoreState state)
        {
            this.PlayerTwoScore = state.PlayerTwoScore;
            this.PlayerTwoScore = state.PlayerTwoScore;
        }
    }
}
