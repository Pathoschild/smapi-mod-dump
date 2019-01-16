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
