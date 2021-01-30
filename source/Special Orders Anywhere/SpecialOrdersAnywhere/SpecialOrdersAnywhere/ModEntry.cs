/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AcidicNic/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;

namespace SpecialOrdersAnywhere
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private ModConfig config;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.config = helper.ReadConfig<ModConfig>();

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if the world isn't loaded
            if (!Context.IsWorldReady)
                return;

            // ActivateKey
            if (e.Button == this.config.ActivateKey)
            {
                if (Context.CanPlayerMove)
                {
                    if (this.config.enableCalendar)
                        Game1.activeClickableMenu = new Billboard();
                    else if (this.config.enableDailyQuests)
                        Game1.activeClickableMenu = new Billboard(true);
                    else if (this.config.SpecialOrdersBeforeUnlocked || SpecialOrder.IsSpecialOrdersBoardUnlocked())
                        Game1.activeClickableMenu = new SpecialOrdersBoard();
                    else if (this.config.QiBeforeUnlocked || Game1.netWorldState.Value.GoldenWalnutsFound.Value >= 100)
                        Game1.activeClickableMenu = new SpecialOrdersBoard("Qi");
                }
                else if (Game1.activeClickableMenu is SpecialOrdersBoard || Game1.activeClickableMenu is Billboard)
                    Game1.exitActiveMenu();
            }
            // CycleRight & LeftCycle
            else if ((e.Button == this.config.CycleRightKey || e.Button == this.config.CycleLeftKey) && !Context.IsPlayerFree)
            {
                string active = "";
                if (Game1.activeClickableMenu is Billboard)
                {
                    if ((Game1.activeClickableMenu as Billboard).calendarDays != null)
                        active = "cal";
                    else
                        active = "dq";
                }
                else if (Game1.activeClickableMenu is SpecialOrdersBoard)
                {
                    if ((Game1.activeClickableMenu as SpecialOrdersBoard).boardType == "Qi")
                        active = "qi";
                    else
                        active = "so";
                }

                if (active == "")
                    return;

                LinkedList<string> menuList = new LinkedList<string>();
                if (this.config.enableCalendar)
                    menuList.AddLast("cal");
                if (this.config.enableDailyQuests)
                    menuList.AddLast("dq");
                if (this.config.SpecialOrdersBeforeUnlocked || SpecialOrder.IsSpecialOrdersBoardUnlocked())
                    menuList.AddLast("so");
                if (this.config.QiBeforeUnlocked || Game1.netWorldState.Value.GoldenWalnutsFound.Value >= 100)
                    menuList.AddLast("qi");

                string nextMenu = "";

                if (menuList.Count == 1)
                    return;
                else if (menuList.Count == 2)
                {
                    if (menuList.First.Value == active)
                        nextMenu = menuList.Last.Value;
                    else
                        nextMenu = menuList.First.Value;
                }
                else
                {
                    if (e.Button == this.config.CycleRightKey)
                    {
                        var menuNode = menuList.First;
                        while (menuNode != null)
                        {
                            if (menuNode.Value == active)
                            {
                                if (menuNode.Next != null)
                                    nextMenu = menuNode.Next.Value;
                                else
                                    nextMenu = menuList.First.Value;
                                menuNode = null;
                            }
                            else
                                menuNode = menuNode.Next;
                        }
                    }
                    else
                    {
                        var menuNode = menuList.Last;
                        while (menuNode != null)
                        {
                            if (menuNode.Value == active)
                            {
                                if (menuNode.Previous != null)
                                    nextMenu = menuNode.Previous.Value;
                                else
                                    nextMenu = menuList.Last.Value;
                                menuNode = null;
                            }
                            else
                                menuNode = menuNode.Previous;
                        }
                    }
                }

                switch (nextMenu)
                {
                    case "cal":
                        Game1.activeClickableMenu = new Billboard();
                        break;
                    case "dq":
                        Game1.activeClickableMenu = new Billboard(true);
                        break;
                    case "so":
                        Game1.activeClickableMenu = new SpecialOrdersBoard();
                        break;
                    case "qi":
                        Game1.activeClickableMenu = new SpecialOrdersBoard("Qi");
                        break;
                    default:
                        return;
                }
            }
        }
    }
}
