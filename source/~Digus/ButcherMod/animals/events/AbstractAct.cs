/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.animals.data;
using StardewValley;
using static AnimalHusbandryMod.common.DataLoader;

namespace AnimalHusbandryMod.animals.events
{
    public abstract class AbstractAct : IAnimalContestAct
    {
        public abstract string NpcName { get; }
        public virtual string RequiredEvent { get; }
        public abstract string GetAct(AnimalContestItem animalContestInfo, List<AnimalContestItem> history);

        public virtual bool Available(List<AnimalContestItem> history)
        {
            return RequiredEvent == null || Game1.player.eventsSeen.Contains(RequiredEvent);
        }

        protected string TranslationKey(string keyPostFix)
        {
            return $"AnimalContest.Dialog.{NpcName}Act.{keyPostFix}";
        }

        protected string GetDialog(string keyPostFix)
        {
            return i18n.Get(TranslationKey(keyPostFix));
        }

        protected string GetDialog(string keyPostFix, object tokens)
        {
            return i18n.Get(TranslationKey(keyPostFix),tokens);
        }
    }
}
