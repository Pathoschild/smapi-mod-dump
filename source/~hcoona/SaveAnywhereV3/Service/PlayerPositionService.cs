using SaveAnywhereV3.DataContract;
using StardewValley;

namespace SaveAnywhereV3.Service
{
    public class PlayerPositionService : SaveLoadServiceBase<PlayerPostion>
    {
        public PlayerPositionService()
            : base(model => model.PlayerPosition)
        {
        }

        private global::StardewValley.Farmer Player => Game1.player;

        protected override void DoLoad(PlayerPostion model)
        {
            Game1.warpFarmer(model.Location, model.X, model.Y, model.Direction);
        }

        protected override PlayerPostion DumpSaveModel()
        {
            return new PlayerPostion()
            {
                X = Player.getTileX(),
                Y = Player.getTileY(),
                Location = Player.currentLocation.name,
                Direction = Player.facingDirection
            };
        }
    }
}
