using QuestFramework.Hooks;
using QuestFramework.Quests;
using StardewModdingAPI;
using System.Linq;

namespace QuestFramework.Framework.Hooks
{
    class LocationHook : HookObserver
    {
        private string newLocation;
        private string oldLocation;

        public LocationHook(IModHelper helper)
        {
            helper.Events.Player.Warped += this.OnLocationChanged;
        }

        private void OnLocationChanged(object sender, StardewModdingAPI.Events.WarpedEventArgs e)
        {
            if (!e.IsLocalPlayer)
                return;

            this.newLocation = e.NewLocation.Name;
            this.oldLocation = e.OldLocation.Name;

            this.Observe(new CompletionArgs(str: this.newLocation));
        }

        public override string Name => "Location";

        protected override bool CheckHookExecute(Hook hook, ICompletionArgs args)
        {
            bool flag = hook.Has.Any() && this.owner.CheckConditions(hook.Has, hook.ManagedQuest, new[] { "Enter", "Leave" });

            if (hook.Has.TryGetValue("Enter", out string newLocation))
            {
                flag &= this.newLocation == newLocation;
            }

            if (hook.Has.TryGetValue("Leave", out string oldLocation))
            {
                flag &= this.oldLocation == oldLocation;
            }

            return flag;
        }
    }
}
