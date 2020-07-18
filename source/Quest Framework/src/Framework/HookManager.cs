using QuestFramework.Framework.Hooks;
using QuestFramework.Hooks;
using QuestFramework.Quests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuestFramework.Framework
{
    internal class HookManager
    {
        public List<Hook> Hooks { get; private set; }
        public Dictionary<string, Func<string, bool>> Conditions { get; private set; }
        public Dictionary<string, HookObserver> Observers { get; }

        public HookManager()
        {
            this.Hooks = new List<Hook>();
            this.Observers = new Dictionary<string, HookObserver>();
            this.Clean();
        }

        public void CollectHooks(List<CustomQuest> managedQuests)
        {
            var hooks = from quest in managedQuests
                        where quest.Hooks != null && quest.Hooks.Count > 0
                        from hook in quest.Hooks
                        select new { managedQuest = quest, hook };

            this.Hooks = hooks.Select(h => { 
                h.hook.managedQuest = h.managedQuest; return h.hook; 
            }).ToList();
        }

        public List<Hook> GetHooksByWhen(string whenName)
        {
            return this.Hooks
                .Where(hook => hook.When == whenName)
                .ToList();
        }

        public void AddHookObserver(HookObserver hookObserver)
        {
            if (QuestFrameworkMod.Instance.Status != State.STANDBY)
                throw new InvalidOperationException($"Unable to add hook observer in state `{QuestFrameworkMod.Instance.Status}`.");

            if (this.Observers.ContainsKey(hookObserver.Name))
                throw new InvalidOperationException($"Observer for hook `{hookObserver.Name}` already added.");

            hookObserver.owner = this;

            this.Observers.Add(hookObserver.Name, hookObserver);
        }

        public bool CheckConditions(Dictionary<string, string> conditions, IEnumerable<string> ignore = null)
        {
            bool flag = true;

            if (conditions == null)
                return true;

            foreach (var cond in conditions)
            {
                if (ignore != null && ignore.Any(ig => ig == cond.Key))
                    continue;

                flag &= this.CheckCondition(cond.Key, cond.Value);
            }

            return flag;
        }

        public bool CheckCondition(string condition, string value)
        {
            if (this.Conditions.TryGetValue(condition, out var conditionFunc))
                return conditionFunc(value);
            
            return false;
        }

        public void Clean()
        {
            this.Conditions = CommonConditions.GetConditions();
            this.Hooks.Clear();
        }
    }
}
