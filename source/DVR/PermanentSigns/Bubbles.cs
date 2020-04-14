using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermanentSigns
{
    public class Bubbles
    {
        public NPC npc;
        public int ttl;
        public int waitTicks;
        public int parentSheetIndex;

        public void draw(SpriteBatch sb)
        {
            var pos = npc.getTileLocation();
            var drawPos = new Vector2((float)(pos.X * 64 ), (float)(pos.Y * 64 - 170));
            var iconPos = new Vector2(drawPos.X + 40, drawPos.Y + 40);
            sb.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, drawPos), new Rectangle?(new Rectangle(141, 465, 20, 24)), Color.White * .8f, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)((pos.Y + 1) * 64) / 10000f + 1E-06f + pos.X / 10000f);
            sb.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, iconPos), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, parentSheetIndex, 16, 16)), Color.White * .8f, 0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, (float)((pos.Y + 1) * 64) / 10000f + 1E-05f + pos.X / 10000f);
        }

        static IList<Bubbles> All = new List<Bubbles>();

        public static void Add(NPC npc, int psi)
        {
            All.Add(new Bubbles { npc = npc, parentSheetIndex = psi, ttl = 75, waitTicks = 75 });
        }

        public static void Render(SpriteBatch sb)
        {
            foreach(var b in All)
            {
                if (b.waitTicks > 0) continue;
                b.draw(sb);
            }
        }

        public static void Tick()
        {
            var any = false;
            foreach (var b in All)
            {
                if (b.waitTicks > 0)
                {
                    b.waitTicks--;
                }
                else
                {
                    b.ttl--;
                    if (b.ttl == 0)
                    {
                        any = true;
                    }
                }
            }
            if (!any) return;
            All = All.Where(x => x.ttl > 0).ToList();
        }

    }
}
