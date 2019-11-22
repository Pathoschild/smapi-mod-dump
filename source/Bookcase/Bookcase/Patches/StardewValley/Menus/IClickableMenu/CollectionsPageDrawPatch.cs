using Bookcase.Events;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bookcase.Patches {
    /// <summary>
    /// Patches the Menu.CollectionsPage - not cancellable, but will allow you to modify the elements being drawn.
    /// </summary>
    public class CollectionsPageDrawPatch : IGamePatch {
        public Type TargetType => typeof(CollectionsPage);

        public MethodBase TargetMethod => TargetType.GetMethod("draw");

        public static void Prefix(ref SpriteBatch b, ref CollectionsPage __instance){
            FieldInfo currentTab = __instance.GetType().GetField("currentTab", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo currentPage = __instance.GetType().GetField("currentPage", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo hoverText = __instance.GetType().GetField("hoverText", BindingFlags.NonPublic | BindingFlags.Instance);

            int currentTabVal = (int)currentTab.GetValue(__instance);
            int currentPageVal = (int)currentPage.GetValue(__instance);
            string hoverTextVal = (string)hoverText.GetValue(__instance);

            CollectionsPageDrawEvent evt = new CollectionsPageDrawEvent(__instance, currentTabVal, currentPageVal, hoverTextVal, __instance.collections);
            BookcaseEvents.CollectionsPageDrawEvent.Post(evt);

            currentTab.SetValue(__instance, evt.currentTab);
            currentPage.SetValue(__instance, evt.currentPage);
            hoverText.SetValue(__instance, evt.hoverText);
        }
    }
}
