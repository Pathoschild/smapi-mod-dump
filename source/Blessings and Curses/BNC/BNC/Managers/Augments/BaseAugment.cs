/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GenDeathrow/SDV_BlessingsAndCurses
**
*************************************************/

using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using System;


namespace BNC.Managers.Augments
{

    public abstract class BaseAugment
    {

        public String DisplayName { set; get; }
        public String id { set; get; }
        public String desc { set; get; }
        public bool isNegative { set; get; }
        public bool isMineOnly { set; get; }
        public bool hasMonsterTick { set; get; }

        private bool _marked = false;

        public BaseAugment()
        {
            this.DisplayName = "Null";
            this.desc = "Empty";
            this.isMineOnly = true;
            this.hasMonsterTick = true;
        }

        public BaseAugment setDisplayName(String name)
        {
            return this;
        }

        public BaseAugment setChatId(String id)
        {
            this.id = id;
            return this;
        }

        public BaseAugment setDescription(String desc)
        {
            this.desc = desc;
            return this;
        }

        public BaseAugment setIsNegative(bool isNegative)
        {
            this.isNegative = isNegative;
            return this;
        }

        public BaseAugment setMineOnly(bool isMineOnly)
        {
            this.isMineOnly = isMineOnly;
            return this;
        }

        public bool shouldRemove()
        {
            return this._marked;
        }

        public void markForRemoval()
        {
            this._marked = true;
        }


        public abstract void Init();

        public abstract TwitchApp.ActionResponse PlayerTickUpdate();

        public abstract TwitchApp.ActionResponse MonsterTickUpdate(Monster m);

        public abstract TwitchApp.ActionResponse UpdateMonster(WarpedEventArgs e, Monster npc);

        public abstract TwitchApp.ActionResponse WarpLocation(WarpedEventArgs e);

        public void OnRemove() { }
    }
}
