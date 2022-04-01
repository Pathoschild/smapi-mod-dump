/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using Microsoft.Xna.Framework;
using DeluxeJournal.Tasks;

namespace DeluxeJournal.Framework.Tasks
{
    internal class TaskTypes
    {
        public static readonly string Basic = Register("basic", typeof(BasicTask), typeof(BasicFactory<BasicTask>), 0, 0);
        public static readonly string Collect = Register("collect", typeof(CollectTask), typeof(CollectTask.Factory), 1, 32);
        public static readonly string Craft = Register("craft", typeof(CraftTask), typeof(CraftTask.Factory), 2, 30);
        public static readonly string Build = Register("build", typeof(BuildTask), typeof(BuildTask.Factory), 3, 33);
        public static readonly string Blacksmith = Register("blacksmith", typeof(BlacksmithTask), typeof(BlacksmithTask.Factory), 4, 34);
        public static readonly string Gift = Register("gift", typeof(GiftTask), typeof(GiftTask.Factory), 5, 10);
        public static readonly string Buy = Register("buy", typeof(BuyTask), typeof(BuyTask.Factory), 6, 16);
        public static readonly string Sell = Register("sell", typeof(SellTask), typeof(SellTask.Factory), 7, 15);

        private static string Register(string id, Type taskType, Type factoryType, int iconTileSheetIndex, int priority)
        {
            TaskRegistry.Register(id, taskType, factoryType, new TaskIcon(null, new Rectangle(iconTileSheetIndex * 14, 96, 14, 14)), priority);
            return id;
        }
    }
}
