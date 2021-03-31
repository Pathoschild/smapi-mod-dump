/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using QuestFramework.Framework.Hooks;
using QuestFramework.Hooks;
using QuestFramework.Quests;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuestFramework.Framework
{
    internal class ConditionManager
    {
        private readonly IMonitor monitor;

        public List<Hook> Hooks { get; private set; }
        public Dictionary<string, Func<string, object, bool>> Conditions { get; }
        
        [Obsolete("This hook API is deprecated. Will be replaced in future")]
        public Dictionary<string, HookObserver> Observers { get; }

        public ConditionManager(IMonitor monitor)
        {
            this.Hooks = new List<Hook>();
            this.Observers = new Dictionary<string, HookObserver>();
            this.Conditions = CommonConditions.GetConditions();
            this.Clean();
            this.monitor = monitor;
        }

        [Obsolete("This hook API is deprecated. Will be replaced in future")]
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

        [Obsolete("This hook API is deprecated. Will be replaced in future")]
        public List<Hook> GetHooksByWhen(string whenName)
        {
            return this.Hooks
                .Where(hook => hook.When == whenName)
                .ToList();
        }

        [Obsolete("This hook API is deprecated. Will be replaced in future")]
        public void AddHookObserver(HookObserver hookObserver)
        {
            if (QuestFrameworkMod.Instance.Status != State.STANDBY)
                throw new InvalidOperationException($"Unable to add hook observer in state `{QuestFrameworkMod.Instance.Status}`.");

            if (this.Observers.ContainsKey(hookObserver.Name))
                throw new InvalidOperationException($"Observer for hook `{hookObserver.Name}` already added.");

            hookObserver.owner = this;

            this.Observers.Add(hookObserver.Name, hookObserver);
        }

        public bool CheckConditions(Dictionary<string, string> conditions, object context, IEnumerable<string> ignore = null, bool ignoreUnknown = false)
        {
            bool flag = true;

            if (conditions == null)
                return true;

            foreach (var cond in conditions)
            {
                if (ignore != null && ignore.Any(ig => ig == cond.Key))
                    continue;

                flag &= this.CheckCondition(cond.Key, cond.Value, context, ignoreUnknown);
            }

            this.monitor.VerboseLog($"All checked conditions result is {flag}");

            return flag;
        }

        public bool CheckCondition(string condition, string value, object context, bool ignoreUnknown = false)
        {
            bool isNot = false;
            string realConditionName = condition;
            string contextName = context is CustomQuest managedQuest 
                ? $"quest:{managedQuest.GetFullName()}" 
                : context.ToString();

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
                        $"in context `{contextName}` " +
                        $"returns {(isNot ? !result : result)}");

                return isNot ? !result : result;
            } else if (ignoreUnknown)
            {
                // Always true when unknown conditions are ignored
                return true;
            }

            this.monitor.Log(
                $"Checked unknown condition `{condition}` in context `{contextName}`. Result for unknown conditions is always false.", LogLevel.Warn);
            
            return false;
        }

        public void Clean()
        {
            this.Hooks.Clear();
        }
    }
}
