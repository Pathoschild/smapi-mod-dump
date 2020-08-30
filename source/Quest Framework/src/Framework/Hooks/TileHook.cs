using Microsoft.Xna.Framework;
using QuestFramework.Hooks;
using QuestFramework.Quests;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Linq;

namespace QuestFramework.Framework.Hooks
{
    class TileHook : HookObserver
    {
        public override string Name => "Tile";
        public string TouchAction { get; set; }
        public Vector2 Position { get; set; }
        public string CurrentLocation { get; set; }

        public TileHook(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += this.OnTick;
        }

        private void OnTick(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (!e.IsMultipleOf(30))
                return;

            this.Position = new Vector2(Game1.player.getStandingX() / 64, Game1.player.getStandingY() / 64);
            this.CurrentLocation = Game1.player.currentLocation.Name;
            this.TouchAction = null;
            this.Observe();
        }

        protected override bool CheckHookExecute(Hook hook, ICompletionArgs args)
        {
            bool flag = hook.Has.Any();

            foreach(var cond in hook.Has)
            {
                switch (cond.Key)
                {
                    case "Location":
                        flag &= this.CurrentLocation == cond.Value;
                        break;
                    case "Position":
                        int[] pos = Utility.parseStringToIntArray(cond.Value);
                        flag &= pos.Length >= 2
                            && this.Position.X == pos[0]
                            && this.Position.Y == pos[1];
                        break;
                    case "Area":
                        var bbox = Game1.player.GetBoundingBox();
                        int[] rectPts = Utility.parseStringToIntArray(cond.Value);
                        flag &= rectPts.Length >= 4 
                            && new Rectangle(
                                rectPts[0], 
                                rectPts[1], 
                                rectPts[2], 
                                rectPts[3]
                            ).Contains(bbox);
                        break;
                    case "TouchAction":
                        flag &= this.TouchAction == cond.Value;
                        break;
                    default:
                        flag &= this.owner.CheckCondition(cond.Key, cond.Value, hook.ManagedQuest);
                        break;
                }
            }

            return flag;
        }
    }
}
