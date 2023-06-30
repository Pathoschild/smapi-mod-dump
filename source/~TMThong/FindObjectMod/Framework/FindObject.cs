/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
namespace FindObjectMod.Framework
{
    public class FindObject : ModTool
    {
        public FindObject(IMonitor monitor, IModHelper modHelper, ModConfig config) : base(modHelper, monitor, config)
        {
        }

        private void PlayerWarped_(object o, WarpedEventArgs e)
        {
            bool isLocalPlayer = e.IsLocalPlayer;
            if (isLocalPlayer)
            {
                this.Objects = Utilities.GetObjects(e.NewLocation);
            }
        }

        private void ObjectChanged_(object o, ObjectListChangedEventArgs e)
        {
            this.Objects = Utilities.GetObjects(e.Location);
        }

        private void Saved_(object o, SavedEventArgs e)
        {
            this.Objects = Utilities.GetObjects(null);
        }

        private void saveCreated_(object o, SaveCreatedEventArgs e)
        {
            this.Objects = Utilities.GetObjects(null);
        }

        public override void WorldRendered(SpriteBatch batch)
        {
            bool flag = this.Objects == null;
            if (flag)
            {
                this.Objects = Utilities.GetObjects(null);
            }
            bool initFindQuestObject = false;
            bool initFindObjectFromConfig = false;
            bool flag2 = Utilities.HasQuestObject(this.Objects);
            if (flag2)
            {
                initFindQuestObject = true;
                this.drawObject(batch, (from p in this.Objects.ToList<StardewValley.Object>()
                                        where Utilities.isQuestObject(p)
                                        select p).ToArray<StardewValley.Object>(), base.Config.QuestObject);
            }
            bool flag3 = Utilities.HasObjectCanFind(base.Config, this.Objects);
            if (flag3)
            {
                initFindObjectFromConfig = true;
                this.drawObjectCanfind(batch, (from p in this.Objects.ToList<StardewValley.Object>()
                                               where !Utilities.isQuestObject(p) && Utilities.isObjectCanFind(base.Config, p)
                                               select p).ToArray<StardewValley.Object>());
            }
            bool findAllObject = base.Config.FindAllObject;
            if (findAllObject)
            {
                StardewValley.Object[] gameObjects = this.Objects;
                bool flag4 = initFindQuestObject;
                if (flag4)
                {
                    gameObjects = (from p in gameObjects.ToList<StardewValley.Object>()
                                   where !Utilities.isQuestObject(p)
                                   select p).ToArray<StardewValley.Object>();
                }
                bool flag5 = initFindObjectFromConfig;
                if (flag5)
                {
                    gameObjects = (from p in gameObjects.ToList<StardewValley.Object>()
                                   where !Utilities.isObjectCanFind(base.Config, p)
                                   select p).ToArray<StardewValley.Object>();
                }
                this.drawObject(batch, gameObjects, base.Config.Object);
            }
        }

        public void drawObject(SpriteBatch batch, StardewValley.Object[] objects_, Color color)
        {
            foreach (StardewValley.Object a in objects_)
            {
                StardewValley.Object o = a;
                Vector2 x2 = Game1.GlobalToLocal(base.ViewPort(), Game1.player.Position);
                Vector2 x3 = o.getLocalPosition(base.ViewPort());
                Utility.drawLineWithScreenCoordinates((int)x3.X + 32, (int)x3.Y + 32, (int)x2.X + 32, (int)x2.Y, batch, color, 1f);
            }
            bool drawArea = base.Config.DrawArea;
            if (drawArea)
            {
                Utilities.DrawArea(objects_.ToList<StardewValley.Object>(), batch, color);
            }
        }

        public void drawObjectCanfind(SpriteBatch batch, StardewValley.Object[] objects_)
        {
            foreach (StardewValley.Object a in objects_)
            {
                StardewValley.Object o = a;
                Vector2 x2 = Game1.GlobalToLocal(base.ViewPort(), Game1.player.Position);
                Vector2 x3 = o.getLocalPosition(base.ViewPort());
                Utility.drawLineWithScreenCoordinates((int)x3.X + 32, (int)x3.Y + 32, (int)x2.X + 32, (int)x2.Y, batch, base.Config.ObjectToFind[Utilities.SaveKey][o.name], 1f);
                bool drawArea = base.Config.DrawArea;
                if (drawArea)
                {
                    Utilities.DrawArea(a, objects_, batch, base.Config.ObjectToFind[Utilities.SaveKey][o.Name]);
                }
            }
        }

        public override void Initialization()
        {
            base.Helper.Events.Player.Warped += this.PlayerWarped_;
            base.Helper.Events.World.ObjectListChanged += this.ObjectChanged_;
            base.Helper.Events.GameLoop.Saved += this.Saved_;
            base.Helper.Events.GameLoop.SaveCreated += this.saveCreated_;
            base.Initialization();
        }

        public override void Destroy()
        {
            base.Helper.Events.Player.Warped -= this.PlayerWarped_;
            base.Helper.Events.World.ObjectListChanged -= this.ObjectChanged_;
            base.Helper.Events.GameLoop.Saved -= this.Saved_;
            base.Helper.Events.GameLoop.SaveCreated -= this.saveCreated_;
            base.Destroy();
        }

        public StardewValley.Object[] Objects;
    }
}
