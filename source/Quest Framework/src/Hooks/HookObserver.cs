/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using QuestFramework.Framework;
using QuestFramework.Quests;
using System;
using System.Linq;

namespace QuestFramework.Hooks
{
    public abstract class HookObserver : IDisposable
    {
        public abstract string Name { get; }

        internal HookManager owner;

        protected abstract bool CheckHookExecute(Hook hook, ICompletionArgs args);

        public void Observe()
        {
            this.Observe(new CompletionArgs());
        }

        public void Observe(ICompletionArgs args)
        {
            var hooks = this.owner.GetHooksByWhen(this.Name);

            if (!hooks.Any())
                return;

            foreach (Hook hook in hooks)
            {
                if (this.CheckHookExecute(hook, args))
                    hook.Execute(args);
            }
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing) { }

        ~HookObserver()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}