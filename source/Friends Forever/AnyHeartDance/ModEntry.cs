/*
 * MIT License
 *
 * Copyright (c) 2017-2019 Isaac S.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace AnyHeartDance {
    public class Config {
        /// <summary>How many hearts are required for the dance (0-13).</summary>
        public int HeartRequirement = 4;
    }

    public class ModEntry : StardewModdingAPI.Mod {
        internal Config Config;
    
        public override void Entry(IModHelper helper) {
            Config = helper.ReadConfig<Config>();
            
            if (Config.HeartRequirement < 0) {
                Config.HeartRequirement = 0;
                Monitor.Log("Config heart requirement is less than 0? Defaulting to 0.", LogLevel.Warn);
            } else if (Config.HeartRequirement > 13) {
                Config.HeartRequirement = 13;
                Monitor.Log("Config heart requirement is greater than 13? Defaulting to 13.", LogLevel.Warn);
            }
            
            helper.Events.Display.MenuChanged += OnUpdate;
        }

        private void OnUpdate(object sender, EventArgs args) {
            if (Game1.currentLocation == null || Game1.currentLocation.name != "Temp" || Game1.currentLocation.currentEvent == null)
                return;
            Event @event = Game1.currentLocation.currentEvent;
            //Dictionary<string, string> data = (Dictionary<string, string>)Util.GetInstanceField(typeof(Event), @event, "festivalData");

            if (!@event.FestivalName.Equals("Flower Dance"))
                return;

            foreach (NPC npc in @event.actors) {
                if ( !npc.datable || npc.HasPartnerForDance) continue;
                try {
                    if (npc.CurrentDialogue.Count() <= 0) continue;
                    Dialogue reject = new Dialogue(Game1.content.Load<Dictionary<string, string>>("Characters\\Dialogue\\" + npc.name)["danceRejection"], npc);
                    Dialogue curr = npc.CurrentDialogue.Peek();
                    if (reject == null || curr == null) continue;

                    //Log.Async("Dialogue " + curr.getCurrentDialogue() + " " + reject.getCurrentDialogue());
                    if (curr.getCurrentDialogue() == reject.getCurrentDialogue()) {
                        NPC who = npc;
                        // The original stuff, only the relationship point check is modified.
                        if (!who.HasPartnerForDance && Game1.player.getFriendshipLevelForNPC(who.name) >= 250 * Config.HeartRequirement) {
                            string s = "";
                            switch (who.gender) {
                            case 0:
                                s = "You want to be my partner for the flower dance?#$b#Okay. I look forward to it.$h";
                                break;
                            case 1:
                                s = "You want to be my partner for the flower dance?#$b#Okay! I'd love to.$h";
                                break;
                            }
                            
                            Game1.player.changeFriendship(250, Game1.getCharacterFromName(who.name));
                            
                            Game1.player.dancePartner.SetCharacter(who);
                            this.Monitor.Log(who.HasPartnerForDance.ToString());
                            
                            who.setNewDialogue(s, false, false);

                            // Okay, looks like I need to fix the current dialog box
                            Game1.activeClickableMenu = new DialogueBox(new Dialogue(s, who) { removeOnNextMove = false });
                        }
                    }
                } catch ( Exception e) {
                    this.Monitor.Log("Exception: " + e, LogLevel.Error);
                    continue;
                }
            }
        }
    }
}
