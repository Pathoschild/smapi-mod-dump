/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using Revitalize.Framework.Player.Managers;

namespace Revitalize.Framework.Player
{
    public class PlayerInfo
    {
        public SittingInfo sittingInfo;
        public MagicManager magicManager;

        public PlayerInfo()
        {
            this.sittingInfo = new SittingInfo();
            this.magicManager = new MagicManager();
        }

        public void update()
        {
            this.sittingInfo.update();
        }
    }
}
