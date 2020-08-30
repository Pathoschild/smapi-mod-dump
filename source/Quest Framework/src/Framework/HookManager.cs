using QuestFramework.Framework.Hooks;
using QuestFramework.Hooks;
using QuestFramework.Quests;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuestFramework.Framework
{
    internal class HookManager
    {
        private readonly IMonitor monitor;

        public List<Hook> Hooks { get; private set; }
        public Dictionary<string, Func<string, CustomQuest, bool>> Conditions { get; }
        public Dictionary<string, HookObserver> Observers { get; }

        public HookManager(IMonitor monitor)
        {
            this.Hooks = new List<Hook>();
            this.Observers = new Dictionary<string, HookObserver>();
            this.Conditions = CommonConditions.GetConditions();
            this.Clean();
            this.monitor = monitor;
        }

        public void CollectHooks(List<CustomQuest> managedQuests)
        {
            var hooks = from quest in managedQuests
                        where quest.Hooks != null && quest.Hooks.Count > 0
                        from hook in quest.Hooks
                        select new { managedQuest = quest, hook };

            this.Hooks = hooks.Select(h => { 
                h.hook.ManagedQuest = h.managedQuest; return h.hook; 
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

        public bool CheckConditions(Dictionary<string, string> conditions, CustomQuest context, IEnumerable<string> ignore = null)
        {
            bool flag = true;

            if (conditions == null)
                return true;

            foreach (var cond in conditions)
            {
                if (ignore != null && ignore.Any(ig => ig == cond.Key))
                    continue;

                flag &= this.CheckCondition(cond.Key, cond.Value, context);
            }

            this.monitor.VerboseLog($"All checked conditions result is {flag}");

            return flag;
        }

        public bool CheckCondition(string condition, string value, CustomQuest context)
        {
            bool isNot = false;
            string realConditionName = condition;

            if (condition == null || value == null)
                return true;

            if (condition.StartsWith("not:"))
            {
                condition = condition.Substring(4);
                isNot = true;
            }

            if (this.Conditions.TryGetValue(condition, out var conditionFunc))
            {
                bool result = false;

                foreach (string valuePart in value.Split('|'))
                    result |= conditionFunc(valuePart.Trim(), context);

                if (this.monitor.IsVerbose)
                    this.monitor.Log(
                        $"Checked condition `{realConditionName}` for `{value}` " +
                        $"in quest context `{context.GetFullName()}` " +
                        $"returns {(isNot ? !result : result)}");

                return isNot ? !result : result;
            }

            this.monitor.Log(
                $"Checked unknown condition `{condition}` in quest context `{context.GetFullName()}`. Result for unknown conditions is always false.", LogLevel.Warn);
            
            return false;
        }

        public void Clean()
        {
            this.Hooks.Clear();
        }
    }
}
