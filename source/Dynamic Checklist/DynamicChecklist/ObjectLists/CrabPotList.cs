namespace DynamicChecklist.ObjectLists
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using StardewValley;
    using StardewValley.Objects;

    public class CrabPotList : ObjectList
    {
        public CrabPotList(ModConfig config)
            : base(config)
        {
            this.ImageTexture = OverlayTextures.Crab;
            this.OptionMenuLabel = "Collect From And Bait Crab Pots";
            this.TaskDoneMessage = "All crab pots have been collected from and baited";
            this.Name = TaskName.CrabPot;
            this.ObjectInfoList = new List<StardewObjectInfo>();
        }

        public override string OptionMenuLabel { get; protected set; }

        public override string TaskDoneMessage { get; protected set; }

        protected override Texture2D ImageTexture { get; set; }

        public override void BeforeDraw()
        {
            this.UpdateObjectInfoList(Game1.currentLocation);
            this.TaskDone = !this.ObjectInfoList.Any(soi => soi.NeedAction);
        }

        public override void OnMenuOpen()
        {
        }

        protected override void UpdateObjectInfoList()
        {
            foreach (GameLocation loc in Game1.locations)
            {
                this.UpdateObjectInfoList(loc);
            }

            this.TaskDone = !this.ObjectInfoList.Any(soi => soi.NeedAction);
        }

        private void UpdateObjectInfoList(GameLocation loc)
        {
            this.ObjectInfoList.RemoveAll(soi => soi.Location == loc);
            foreach (KeyValuePair<Vector2, StardewValley.Object> o in loc.Objects.Pairs)
            {
                if (o.Value is CrabPot)
                {
                    CrabPot currentCrabPot = (CrabPot)o.Value;
                    var soi = new StardewObjectInfo();
                    soi.Coordinate = o.Key * Game1.tileSize + new Vector2(Game1.tileSize / 2, Game1.tileSize / 2);
                    soi.Location = loc;
                    if (currentCrabPot.readyForHarvest.Value)
                    {
                        soi.NeedAction = true;
                    }

                    // if player is luremaster, crab pots dont need bait
                    if (currentCrabPot.bait.Value == null && !Game1.player.professions.Contains(11))
                    {
                        soi.NeedAction = true;
                    }

                    this.ObjectInfoList.Add(soi);
                }
            }
        }
    }
}
