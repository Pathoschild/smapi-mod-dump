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
    public class NPCFind : ModTool
    {
        public NPCFind(IModHelper helper, IMonitor monitor, ModConfig config) : base(helper, monitor, config)
        {
        }

        public override void WorldRendered(SpriteBatch b)
        {
            this.Characters.ForEach(delegate (NPC p)
            {
                bool flag = this.Config.FindCharacter[Utilities.SaveKey].ContainsKey(p.name);
                if (flag)
                {
                    this.drawLineNpc(b, this.Config.FindCharacter[Utilities.SaveKey][p.name], p);
                }
                else
                {
                    bool findAllNPC = this.Config.FindAllNPC;
                    if (findAllNPC)
                    {
                        this.drawLineNpc(b, this.Config.NPC, p);
                    }
                }
            });
        }

        public void drawLineNpc(SpriteBatch b, Color color, NPC n)
        {
            Vector2 x = Game1.GlobalToLocal(base.ViewPort(), Game1.player.Position);
            Vector2 x2 = Game1.GlobalToLocal(base.ViewPort(), n.Position);
            Utility.drawLineWithScreenCoordinates((int)x.X + 32, (int)x.Y + 32, (int)x2.X + 32, (int)x2.Y + 32, b, color, 1f);
            bool drawArea = base.Config.DrawArea;
            if (drawArea)
            {
                Utilities.DrawArea(n, b, color);
            }
        }

        private void NPCChanged_(object o, NpcListChangedEventArgs e)
        {
            this.updateAllNPCInLocation(null);
        }

        private void PlayerWarped_(object o, WarpedEventArgs e)
        {
            this.updateAllNPCInLocation(e.NewLocation);
        }

        private void Saved_(object o, SavedEventArgs e)
        {
            this.updateAllNPCInLocation(Game1.player.currentLocation);
        }

        public void updateAllNPCInLocation(GameLocation location = null)
        {
            this.Characters.Clear();
            this.Characters.AddRange(Utilities.GetNpcs(location));
        }

        public override void Initialization()
        {
            base.Helper.Events.Player.Warped += this.PlayerWarped_;
            base.Helper.Events.World.NpcListChanged += this.NPCChanged_;
            base.Helper.Events.GameLoop.Saved += this.Saved_;
            base.Initialization();
        }

        public override void Destroy()
        {
            base.Helper.Events.Player.Warped -= this.PlayerWarped_;
            base.Helper.Events.World.NpcListChanged -= this.NPCChanged_;
            base.Helper.Events.GameLoop.Saved -= this.Saved_;
            base.Destroy();
        }

        public List<NPC> Characters = new List<NPC>();
    }
}
