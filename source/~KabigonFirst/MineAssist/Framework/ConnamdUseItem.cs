using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using Microsoft.Xna.Framework;

namespace MineAssist.Framework {
    class ConnamdUseItem : Command {
        public static string name = "UseItem";
        public new enum Paramter {
            IsContinuous,
            Position,
            ItemName,
            Condition,
            Order,
            SpecialAction
        }
        private int m_position = -1;
        private string m_itemName = null;
        private string m_condition = null;
        private string m_order = null;
        private bool m_specialAction = false;
        DateTime gt;

        public override void exec(Dictionary<string, string> par) {
            //parse parameter
            if(par.ContainsKey(Paramter.IsContinuous.ToString())) {
                isContinuous = par[Paramter.IsContinuous.ToString()].Equals("true", StringComparison.OrdinalIgnoreCase);
            }
            if (par.ContainsKey(Paramter.SpecialAction.ToString())) {
                m_specialAction = par[Paramter.SpecialAction.ToString()].Equals("true", StringComparison.OrdinalIgnoreCase);
            }
            if (par.ContainsKey(Paramter.Position.ToString())) {
                m_position = Convert.ToInt32(par[Paramter.Position.ToString()]) - 1;
            } else if(par.ContainsKey(Paramter.ItemName.ToString())) {
                m_itemName = par[Paramter.ItemName.ToString()];
                if (par.ContainsKey(Paramter.Condition.ToString())) {
                    m_condition = par[Paramter.Condition.ToString()];
                }
                if (par.ContainsKey(Paramter.Order.ToString())) {
                    m_order = par[Paramter.Order.ToString()];
                }
            } else {
                m_position = Game1.player.CurrentToolIndex;
            }

            //execute
            if(m_itemName == null) {
                StardewWrap.fastUse(m_position, m_specialAction);
            } else {
                StardewWrap.fastUse(ref m_itemName, ref m_condition, ref m_order, m_specialAction);
            }
            //set chargeable start time
            if(StardewWrap.isCurrentToolChargable()) {
                gt = DateTime.Now;
            }
        }

        public override void update() {
            if (isFinish || (!isContinuous && !StardewWrap.isCurrentToolChargable())) {
                return;
            }
            int ms = (DateTime.Now - gt).Milliseconds;
            gt = DateTime.Now;
            if(m_itemName == null) {
                StardewWrap.updateUse(ms, m_specialAction);
            } else {
                StardewWrap.updateUse(ms, ref m_itemName, ref m_condition, ref m_order, m_specialAction);
            }
        }
        public override void updateGraphic() {
            StardewWrap.updateUseGraphic();
        }

        public override void end() {
            int ms = (DateTime.Now - gt).Milliseconds;
            gt = DateTime.Now;
            StardewWrap.endUse(ms, m_specialAction);
        }
    }
}
