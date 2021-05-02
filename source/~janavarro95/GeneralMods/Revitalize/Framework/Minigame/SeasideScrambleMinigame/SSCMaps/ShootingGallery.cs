/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardustCore.UIUtilities.SpriteFonts.Components;
using xTile;

namespace Revitalize.Framework.Minigame.SeasideScrambleMinigame.SSCMaps
{
    public class ShootingGallery: SeasideScrambleMap
    {

        public Dictionary<SSCEnums.PlayerID, int> score;

        public static int highScore;

        public TexturedString timeRemaining;

        public ShootingGallery():base()
        {

        }

        public ShootingGallery(Map m) : base(m)
        {
            this.score = new Dictionary<SSCEnums.PlayerID, int>();
            this.score.Add(SSCEnums.PlayerID.One, 0);
            this.score.Add(SSCEnums.PlayerID.Two, 0);
            this.score.Add(SSCEnums.PlayerID.Three, 0);
            this.score.Add(SSCEnums.PlayerID.Four, 0);
        }

        public override void spawnPlayersAtPositions()
        {
            foreach(SSCPlayer p in SeasideScramble.self.players.Values)
            {
                p.position = SeasideScramble.self.currentMap.Center;
            }
        }

        public override void update(GameTime time)
        {
            base.update(time);
        }
        public override void draw(SpriteBatch b)
        {
            base.draw(b);
        }

        public void addScore(SSCEnums.PlayerID player, int amount)
        {
            this.score[player] += amount;
        }

        public void startTimer(int amount)
        {

        }
    }
}
