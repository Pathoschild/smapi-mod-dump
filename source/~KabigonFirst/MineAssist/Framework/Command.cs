/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KabigonFirst/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineAssist.Framework {
    abstract class Command {
        public enum Paramter { };

        public static Command create(string name) {
            if (name == null) {
                return null;
            }
            if (name.Equals(CommandSwitchMode.name)) {
                return new CommandSwitchMode();
            } else if (name.Equals(CommandCraft.name)) {
                return new CommandCraft();
            } else if (name.Equals(ConnamdUseItem.name)) {
                return new ConnamdUseItem();
            } else if (name.Equals(CommandOpenMenu.name)) {
                return new CommandOpenMenu();
            } else if (name.Equals(CommandPause.name)) {
                return new CommandPause();
            }
            return null;
        }

        public bool isFinish { get; set; } = false;
        public bool isContinuous = false;

        public abstract void exec(Dictionary<string, string> par);
        public virtual void update() { }
        public virtual void updateGraphic() { }
        public virtual void end() { isFinish = true; }
    }
}
