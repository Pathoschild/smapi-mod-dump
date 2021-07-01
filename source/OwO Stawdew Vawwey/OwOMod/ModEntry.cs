/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CrunchyDuck/OwO-Stawdew-Vawwey
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley.BellsAndWhistles;
using StardewValley;
using Harmony;

namespace OwOMod {
    /// <summary>Most of the mod code.</summary>
    public class ModEntry : Mod {
        private ModConfig Config;
        private int seed = new Random().Next(1000);  // A random seed, used to offset the hashing function.
        private int poem_index = 0;
        private bool randomly_enabled = false;  // If randomly_enable is on, this will control if modifications should be made.
        private int randomly_enable_time_left = 0; // How long until a state change on randomly_enabled, in ticks. 60 ticks a second.

        public override void Entry(IModHelper helper) {
            ReadConfigValues();

            // Console commands.
            LoadOwOCommand();
            helper.ConsoleCommands.Add(name: "CSecret", callback: SecretCommand, documentation: "\nWho made this mod?");
            helper.ConsoleCommands.Add(name: "oworandom", callback: RandomlyEnableCommand, documentation:
                "\n==== Description ====\n" +
                "Cause OwO Stawdew Vawwey to be randomly enabled or disabled during gameplay!\n" +
                "If you're getting overexposed, want to mess with someone, or just like the unpredictable, this is for you!\n" +

                "\n==== Examples ====\n" +
                "oworandom\n" +
                "oworandom on\n" +
                "oworandom flip\n" +

                "\n==== Options ====\n" +
                "oworandom - Providing no arguments will give information about the current state.\n" +
                "on - Turn this modification on!\n" +
                "off - Turn this modification off!\n" +
                "flip - Immediately triggers the state change.\n");
			helper.ConsoleCommands.Add(name: "owoshuffle", callback: RandomizeSeedCommand, documentation:
				"\n==== Description ====\n" +
				"Reshuffles random elements such as stuttering, faces, punctuation...\n" +

				"\n==== Usage ====\n" +
				"owoshuffle\n" +
				"owoshuffle word\n" +

				"\n==== Options ====\n" +
				"What do trees grow from?\n" +
				"  What do birds eat?\n" +
				"    What rhymes with weed?\n" +
				"      Surely, it exists at the source of all life...\n");
            if (Config.hello_there_json_snooper) {
                helper.ConsoleCommands.Add(name: "owojson", callback: SecretJsonCommand, documentation:
                    "\nSnooper :)");
			}

			helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked; // Used by RandomlyEnable

            // Patches
            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
            System.Reflection.MethodInfo original;
            HarmonyMethod patch;
            PatchOwO.Init(this);

            // == Dialogue stuff ==
            patch = new HarmonyMethod(typeof(PatchOwO), "drawStringPrefix");

            original = AccessTools.Method(typeof(SpriteText), "drawString"); // Handles the dialogue and text boxes
			harmony.Patch(original, prefix: patch);

            // == Resize text boxes for bigger strings ==
			patch = new HarmonyMethod(typeof(PatchOwO), "textBoxPrefix");

			original = AccessTools.Method(typeof(SpriteText), "getWidthOfString"); // Resizes the textboxes to fit the next text.
			harmony.Patch(original, prefix: patch);
			original = AccessTools.Method(typeof(SpriteText), "getHeightOfString");
			harmony.Patch(original, prefix: patch);


            // == Hud/tooltip draw functions ==
            patch = new HarmonyMethod(typeof(PatchOwO), "microDrawStringPrefix");

            original = AccessTools.Method(typeof(SpriteBatch), "DrawString", new Type[] { typeof(SpriteFont), typeof(string), typeof(Vector2), typeof(Color) });
			harmony.Patch(original, prefix: patch);
			original = AccessTools.Method(typeof(SpriteBatch), "DrawString", new Type[] { typeof(SpriteFont), typeof(string), typeof(Vector2), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) });
			harmony.Patch(original, prefix: patch);

            // This is very similar to the Microsoft code, but Mr Ape made it himself.
            // Dear God, the number of types.
            Type[] lots_of_types = new Type[] { typeof(SpriteBatch), typeof(string), typeof(SpriteFont), typeof(int), typeof(int), typeof(int), typeof(string), typeof(int), typeof(string[]), typeof(Item), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(float), typeof(CraftingRecipe), typeof(System.Collections.Generic.IList<Item>) };
            original = AccessTools.Method(typeof(StardewValley.Menus.IClickableMenu), "drawHoverText", lots_of_types);
			harmony.Patch(original, prefix: patch);
        }

		private void GameLoop_UpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e) {
			if (Config.randomly_enable) {
                randomly_enable_time_left--;
                if (randomly_enable_time_left <= 0) {
                    randomly_enabled = !randomly_enabled;
                    randomly_enable_time_left = RandomlyEnableTime();

                    string state_name = randomly_enabled ? "enabled" : "disabled";
                    Monitor.Log($"OwO-ifying now {state_name} for {StringTime(randomly_enable_time_left)}");
				}
			}
		}

		// ==================================
		//           SMAPI FUNCTIONS
		// ==================================
		private void LoadOwOCommand() {
            string cmd_name = "owomods";
            string documentation = "\n==== Description ====\n" +
                "Allows you to customize the intensity of the OwO-ing text.\n\n" +

                "==== Usage ====\n" +
                $"{cmd_name}" +
                $"{cmd_name} lisp chaos excited anxiety cat"+"{0} faces\n\n" +

                "==== Modifiers ====\n" +
                $"Providing no commands \"{cmd_name}\" will tell you the status of the modifiers.\n\n" +

                "Using these words in your command will toggle effects on or off:\n" +
                "  :) - Enables all effects. please reconsider.\n" +
                "  :( - Disables all effects. please reconsider?\n\n" +

                "lisp - replaces Rs and Ls with Ws\n" +
                "chaos - replaces O with OwO and U with UwU\n" +
                "excited - Replace some periods with exclamation marks\n" +
                "anxious - Add s-s-stuttering!\n" +
                "{1}" +
                "cat - nya'ing :)\n" +
                "faces - Add emotes to text!\n";

            if (Config.secret_found != -1)
                documentation = string.Format(documentation, " stoned", "stoned - Doubles up the text wiikkee whhiiss. Will cause more graphical bugs!\n");
            else
                documentation = string.Format(documentation, "", "");

            Helper.ConsoleCommands.Add(name: cmd_name, callback: OwOCommand, documentation:documentation);
        }

        private void OwOCommand(string command, string[] args) {
            if (args.Length == 0) {
                ReadConfigValues();
                var states = $"\n" +
                    $"lisp - {Config.replace_r_l}\n" +
                    $"chaos - {Config.replace_o_u}\n" +
                    $"excited - {Config.excited}\n" +
                    $"anxious - {Config.anxiety}\n" +
                    "{0}" +
                    $"cat - {Config.cat}\n" +
                    $"faces - {Config.faces}\n";
                if (Config.secret_found != -1)
                    states = String.Format(states, $"stoned - {Config.stoned}\n");
                else
                    states = String.Format(states, "", "");

                Monitor.Log(states, LogLevel.Info);
                return;
			}
            string change_message = "\n"; // Default val.

            foreach(string arg in args) {
				switch (arg) {
                    // Invert found value.
                    case ":(":
                        Config.replace_r_l = false;
                        Config.replace_o_u = false;
                        Config.excited = false;
                        Config.anxiety = false;
                        Config.cat = false;
                        Config.faces = false;
                        change_message += $"lisp = {Config.replace_r_l}\n";
                        change_message += $"chaos = {Config.replace_o_u}\n";
                        change_message += $"excited = {Config.excited}\n";
                        change_message += $"anxious = {Config.anxiety}\n";
                        change_message += $"cat = {Config.cat}\n";
                        change_message += $"faces = {Config.faces}\n";

                        if (Config.secret_found != -1) {
                            Config.stoned = false;
                            change_message += $"stoned = {Config.stoned}\n";
                        }
                        break;
                    case ":)":
                        Config.replace_r_l = true;
                        Config.replace_o_u = true;
                        Config.excited = true;
                        Config.anxiety = true;
                        Config.cat = true;
                        Config.faces = true;
                        change_message += $"lisp = {Config.replace_r_l}\n";
                        change_message += $"chaos = {Config.replace_o_u}\n";
                        change_message += $"excited = {Config.excited}\n";
                        change_message += $"anxious = {Config.anxiety}\n";
                        change_message += $"cat = {Config.cat}\n";
                        change_message += $"faces = {Config.faces}\n";

                        if (Config.secret_found != -1) {
                            Config.stoned = true;
                            change_message += $"stoned = {Config.stoned}\n";
                        }
                        break;
                    case "lisp":
						Config.replace_r_l = !Config.replace_r_l;
                        change_message += $"lisp = {Config.replace_r_l}\n";
                        break;
                    case "chaos":
                        Config.replace_o_u = !Config.replace_o_u;
                        change_message += $"chaos = {Config.replace_o_u}\n";
                        break;
                    case "excited":
                        Config.excited = !Config.excited;
                        change_message += $"excited = {Config.excited}\n";
                        break;
                    case "stoned":
                        Config.stoned = !Config.stoned;
                        change_message += $"stoned = {Config.stoned}\n";
                        break;
                    case "anxious":
                        Config.anxiety = !Config.anxiety;
                        change_message += $"anxious = {Config.anxiety}\n";
                        break;
                    case "cat":
                        Config.cat = !Config.cat;
                        change_message += $"cat = {Config.cat}\n";
                        break;
                    case "faces":
                        Config.faces = !Config.faces;
                        change_message += $"faces = {Config.faces}\n";
                        break;
                }
			}
            Helper.WriteConfig(Config);

            if (change_message == "\n")
                change_message = "\nNo changes made!\n";
            Monitor.Log(change_message, LogLevel.Info);
		}

        private void RandomlyEnableCommand(string command, string[] args) {
            if (args.Length > 0) {
                string reply = "\n";
                foreach (var arg in args) {
					switch (arg.ToLower()) {
                        case "flip":
                            if (Config.randomly_enable) {
                                randomly_enabled = !randomly_enabled;
                                randomly_enable_time_left = RandomlyEnableTime();
                                string state_name = randomly_enabled ? "enabled" : "disabled";
                                reply += $"OwO-ifying now {state_name} for {StringTime(randomly_enable_time_left)}\n";
                            }
                            else
                                reply += "Turn oworandom on first!\n";
                            break;
                        case "on":
                            Config.randomly_enable = true;
                            randomly_enabled = false;
							reply += "Modification turned on!\n";
                            randomly_enable_time_left = RandomlyEnableTime();
                            reply += $"Triggers in {StringTime(randomly_enable_time_left)}\n";
                            break;
                        case "off":
                            Config.randomly_enable = false;
                            reply += "Modification turned off!\n";
                            break;
                    }
				}
                if (reply != "\n")
                    Monitor.Log(reply, LogLevel.Info);
                else
                    Monitor.Log("No changes made!", LogLevel.Info);
                return;
			}
			else {
				string msg = "\n";
                string mod_state = Config.randomly_enable ? "enabled" : "disabled";
                msg += $"Modification currently {mod_state}\n";

                if (Config.randomly_enable) {
                    msg += "\n";

                    string state_name = randomly_enabled ? "enabled" : "disabled";
                    msg += $"OwO-ifying is currently {state_name}.\n";
                    
                    string state_change_name = randomly_enabled ? "disable" : "enable";
                    string enable_time = StringTime(randomly_enable_time_left);
                    msg += $"Time until {state_change_name}: {enable_time}\n";
                }

                Monitor.Log(msg, LogLevel.Info);
			}

			Config.randomly_enable = !Config.randomly_enable;
            if (Config.randomly_enable) {

			}
            // Display info.
            string state = Config.randomly_enable ? "on" : "off";
            Monitor.Log($"Randomly activate turned {state}.\n", LogLevel.Info);
		}

        private void RandomizeSeedCommand(string commands, string[] args) {
            if (!Config.riddle_answered) {
                foreach (string _arg in args) {
                    string arg = _arg.ToLower();
                    if (arg == "seed" || arg == "seeds") {
                        Config.riddle_answered = true;
                        Helper.WriteConfig(Config);
                    }
                }
                if (args.Length > 0 && Config.riddle_answered == false)
                    Monitor.Log("Wrong answer.\n", LogLevel.Warn);
            }
            if (Config.riddle_answered) {
                string[] poems = { "\nLo, hark, behold this beautiful land!\n" +
                    "Of colourful coasts, mysterious mountains and fascinating forests;\n" +
                    "Of beautiful flowers, of beautiful faces, of beautiful community;\n" +
                    "Of quaint quiet and complete calm;\n" +
                    "I wish for nothing but you: Stardew Valley.\n" +
                    "    -CrunchyDuck\n",

                    "\nYour windswept hair so frames your face,\n"+
                    "I wish for naught but your embrace.\n"+
                    "Honeyed words, your turns of speech;\n"+
                    "Kisses exchanged on Pelican Beach.\n"+
                    "something something salty shores;\n"+
                    "elliot you’re a goddamn whore\n" +
                    "    -Pidgezz\n",

                    "\nIt! Is a cold day.\n\n"+
                    "Frozen, chilly, frosty, icy.\n"+
                    "The ground won't take nicely;\n"+
                    "To the seeds I try to plant!\n"+
                    "And so my world lies at a slant...\n"+
                    "    -Anonymous farmer\n",

                    "\nCows! :)\n" +
                    "    -Marnie\n",

                    "\nbitofa drink ain't hurt noone\n"+
                    "helps me focus on the road\n"+
                    "stop bothering me\n"+
                    "    -Unsigned\n",

                    "\nYour art is poggy your gameplay too\n" +
                    "I wish I wasn't an impatient fuck to enjoy you more\n" +
                    "Sadly I ruined it all with furry mods\n" +
                    "    -Oken\n"
                    };

                string poem = poems[poem_index];
                //poem = MakeOwO(poem);
                Monitor.Log(poem, LogLevel.Alert);
                poem_index++;
                if (poem_index >= poems.Length)
                    poem_index = 0;

                seed = new Random().Next(1000);
                Monitor.Log("Oh, and the seed was randomized.\n", LogLevel.Info);
                return;
            }

			seed = new Random().Next(1000);
            if (new Random().Next(8) == 1)
                Monitor.Log("Seed reset :)");
            else
                Monitor.Log("S-Seed m-messed up x3");
		}

        private void SecretCommand(string command, string[] args) {
            if (Config.secret_found > 0) {
                string message = "";
                bool end_reached = false;
                string[] sct_message = {
                    "\nBack again? You've already unlocked the secret stoned command!\n" +
                    "Sorry if it's a bit of a disappointment. It was orginally a mistake, actually.\n" +
                    "I made some goofs in my code, and didn't properly replace text after I modified it." +
                    "After that, it became a nice testing command. Easy to tell what text was changed with it!\n",

                    "Did you notice it now appears in the help menus? Probably. You're smart like that.\n" +
                    "I'll be impressed if anyone actually finds this mesasge in the first place!\n" +
                    "-CrunchyDuck <3\n",

                    "\nHi again.\n" +
                    "Come back to chat?\n" +
                    "It does get lonely sometimes working on this stuff.\n",

                    "\nI actually got my start in modding in RimWorld. The modding is strikingly similar to Stardew's!\n" +
                    "I wasn't very good at it, being my first foray into programming. But it's been years since then.\n" +
                    "When I came to Stardew and SMAPI was like \"Oh yeah, we use Harmony\" I was like :O\n" +
                    "Then when I learned a little more about decompilation for this project, I felt like a true hacker.\n" +
                    "I really want to try my hand at doing my own Harmony projects.\n",

                    "\nActually, something I was thinking of doing is a YouTube series on the code of Stardew.\n" +
                    "There's some interesting things in there.\n" +
                    "I figured I might start with explaining the ins and outs of fishing first.\n",

                    "\nThis project kind of helped me, personally. It's sort of the only reason I'm writing this easter egg.\n" +
                    "I guess I like \'marking\' parts of my history. I do it with most of my projects.\n" +
                    "I was having a bit of a crisis of *why* I do what I do. I made a video about it, kind of.\n" +
                    "I don't really know how to talk about it with a stranger,\nbut my upbringing really pushed me towards wanting fame, or glory.\n" +
                    "But I don't think that's what I want. I just want to marvel at the world, and help others do the same.\n",

                    "\nI actually tried to volunteer at a local farm, recently!\n" +
                    "I've always loved learning to do things on my own, from scratch.\n" +
                    "Like, if I had all the fancy tools taken away from me, I could still do it.\n" +
                    "They grow various vegetables, I love animals, and I figured it'd be some nice exercise.\n" +
                    "I haven't heard back from them, though.\n",

                    "\nI feel like I want to say a lot more here, but I can't really bring myself to do it.\n" +
                    "But I really do appreciate you reading my dumb ramblings. Or, well, just being someone who used my mod.\n" +
                    "I'll probably get back to work now. There's a lot of weird edge cases I need to fix.\n" +
                    "Thanks.\n",
                };
                int secret_index = Config.secret_found - 1;

                if (secret_index < sct_message.Length)
                    message = sct_message[secret_index];
                if (secret_index == sct_message.Length-1)
                    end_reached = true;

                Config.secret_found++;
                if (end_reached)
                    Config.secret_found = 0;
                Helper.WriteConfig(Config);
                Monitor.Log(message, LogLevel.Alert);
                return;
            }

            if (args.Length == 0) {
				Monitor.Log("Who made this mod?\n", LogLevel.Info);
                return;
            }

            string passed_text =
                "\nWell done! It is I, THE CrunchyDuck, coming at you live\n" +
                "from the past with this prerecorded message!\n\n" +
                "Thanks for using my dumb mod :) Here's a secert variable you can pass to the OwO command: stoned\n" +
                "Used like: owomods stoned\n" +
                "This doubles up much of the text to be lloonngg. Makes some graphical bugs a bit worse. Enjoy <3\n";
            bool passed = false;

            foreach(string arg in args) {
				switch (arg.ToLower()) {
					case "crunchy":
                    case "duck":
                    case "crunchyduck":
                    case "crunchyduck1337":
                        passed = true;
                        Config.secret_found = 1;
                        Helper.WriteConfig(Config);
                        break;
				}
                if (passed) break;
			}

            if (passed) Monitor.Log(passed_text, LogLevel.Alert);
            else Monitor.Log("Incorrect.", LogLevel.Warn);
		}

        private void SecretJsonCommand(string command, string[] args) {
            Monitor.Log("\n" +
                "If you found this without sifting through my source code, I'm impressed!\n" +
                "Truthfully, I don't expect anyone to find any of these easter eggs.\n" +
                "50% probably don't read mod descriptions, 25% probably don't use console.\n" +
                "Not sure about that last 25%. Probably don't care enough!\n" +
                "Or, I guess I'm wrong if you've unearthed this. Hm. Really setting myself up for failure here.\n" +
                "Be sure to let me know if you found this. It'll change my worldview ;)\n\n" +
                "Hm. That sounds flirty. It wasn't meant to be.\n", LogLevel.Alert);
		}

        private void ReadConfigValues() {
            Config = Helper.ReadConfig<ModConfig>();
        }

        // ==================================
        //            Functions!
        // ==================================
        /// <summary>
        /// Returns a random generator, which uses the hash of the provided string as a seed, plus an offset.
        /// This means an input that is done exactly the same will have the same result within one session.
        /// A note: In C#, hash values are randomized at points, but I add an additional random offset to help it along.
        /// </summary>
        public Random GetConsistentRandom(string _seed) {
            return new Random(_seed.GetHashCode() + seed);
		}

        /// <summary>
        /// Applies modifiers to the text.
        /// </summary>
        /// <param name="text">The text to modify</param>
		public string MakeOwO(string text) {
            if (Config.randomly_enable && randomly_enabled == false || text == null)
                return text;

            Random random = GetConsistentRandom(text);
            int len = text.Length;
            string new_text = "";

            for (int i = 0; i < len; ++i) {
                var c = text[i];
                var replaced = false;

                // hewwo
                if (Config.replace_r_l) {
                    switch (c) {
                        case 'r':
                        case 'l':
							new_text += 'w';
                            c = 'w';  // If a change is made, update the character to be the new, changed character.
                            replaced = true;
                            break;
                        case 'R':
                        case 'L':
							new_text += 'W';
                            c = 'W';
                            replaced = true;
                            break;
                    }
                }

                // S-S-S-Stutter >//<
                if (Config.anxiety) {
                    if (i == 0 || text[i-1] == ' ') {
                        if (char.IsLetter(c)) {
                            int roll = random.Next(20);  // Initial stutter chance
                            while (roll == 1) {
                                if (replaced)
                                    new_text += $"-{c}"; // No new character after stutter.
                                else
                                    new_text += $"{c}-"; // New character added after this stuttering
                                roll = random.Next(4);  // Higher chance for repeat stuttering
                            }
                        }
                    }
                }

                // Nya
                if (Config.cat) {
                    if (i + 1 < len) { // Not the last char
                        char next_char = text[i + 1];
                        if ("aeiouAEIOU".IndexOf(next_char) >= 0) {  // Next char is a vowel
                            bool catted = false;
                            if (c == 'n') {
                                new_text += "ny";
                                catted = true;
                            }
                            else if (c == 'N') {
                                new_text += "Ny";
                                catted = true;
                            }
                            if (catted) {  // Ny activated
                                int roll = random.Next(4);  // Initial vowel extend chance
                                while (roll == 1) {
                                    new_text += next_char;
                                    roll = random.Next(4);
                                }
                                continue;
                            }
                        }
                    }
                }

                // OwO UwU
                if (Config.replace_o_u) {
                    char last_char = i > 0 ? text[i - 1] : '|';
                    switch (c) {
                        case 'o':
                            if (last_char == 'o')
                                new_text += "wo";
                            else
                                new_text += "owo";
                            continue;
                        case 'u':
                            if (last_char == 'u')
                                new_text += "wu";
                            else
                                new_text += "uwu";
                            continue;
                        case 'O':
                            if (last_char == 'O')
                                new_text += "wO";
                            else
                                new_text += "OwO";
                            continue;
                        case 'U':
                            if (last_char == 'U')
                                new_text += "wU";
                            else
                                new_text += "UwU";
                            continue;
                    }
                }

                // woading!!.
                if (Config.excited && ".!?".IndexOf(c) >= 0) {
                    if (c == '?') { // make ????
                        new_text += c;
                        while(random.Next(3) == 1) {
                            new_text += c;
						}
                        continue;
					}

                    string replacements = "!~";
					int r = random.Next(replacements.Length*2); // 2/4 chance for a face to trigger
                    if (r < replacements.Length) {
                        new_text += replacements[r];
                        continue;
                    }
                }

                // >v>
                if (Config.faces) {
                    string face = TryFace(random);
                    if (face != null) {
                        if (c == ',') {  // On commas
                            new_text += $" {face}";
                            replaced = true;
                        }
                        else if (c == ' ' && i > 0 && ".?!".IndexOf(text[i-1]) >= 0) {  // After sentence
							new_text += $" {face} ";
                            replaced = true;
                        }
                    }
				}

                // No changes were made.
                if (!replaced) {
                    new_text += c;
                    if (Config.stoned) {
                        if (char.IsLetter(c)) {
                            new_text += c;
                        }
                    }
                }
            }
            if (Config.faces) {
                if (len > 10) {  // Stops it triggering on hud elements and the like.
                    string face = TryFace(random);
                    if (face != null) {
                        new_text += $" {face}";
                    }
                }
            }

            return new_text;
        }

        /// <summary>
        /// Rolls a chance to get a face to add. Returns an empty string if roll chance failed.
        /// </summary>
        /// <param name="random">A random generator to use.</param>
        public string TryFace(Random random) {
			string[] faces = new string[] { "<", ">//>", "x3", ">v>", "UvU", ":3", ":3c", ":P", "-w-", ";w;", ">w>" };
            int i = random.Next(faces.Length*6); // 1/6 chance to get a face.
            if (i < faces.Length)
                return faces[i];
            return null;
        }

        /// <summary>
        /// Converts an int ticks into a string, in minutes.
        /// </summary>
        public string StringTime(int ticks) {
            int seconds = ticks / 60;
            int minutes = seconds / 60;
            seconds = seconds % 60;
            return $"{minutes}:{seconds}";
		}

        /// <summary>
        /// Generates a time until randomly_enabled state change.
        /// </summary>
        /// <returns></returns>
        public int RandomlyEnableTime() {
			Random random = new Random();
            int minute = 60 * 60; // One minute in ticks.
            // Calculate enable time.
            if (randomly_enabled)
                return random.Next(minute * 5, minute * 10);
            else
                return random.Next(minute * 10, minute * 15);
		}
    }

    /// <summary>
    /// Settings for the mod.
    /// </summary>
    public class ModConfig {
        // hello there code snooper
        public bool hello_there_json_snooper { get; set; } = false;
        public bool replace_r_l { get; set; } = true;
        public bool replace_o_u { get; set; } = false;
        public bool faces { get; set; } = true;
        public bool stoned { get; set; } = false;
        public bool excited { get; set; } = true;
        public bool anxiety { get; set; } = true;
        public bool cat { get; set; } = true;

		public int secret_found { get; set; } = -1;
        public bool riddle_answered { get; set; } = false;

        public bool randomly_enable { get; set; } = false;
	}

    /// =================
    ///      Patches!
    /// =================
    class PatchOwO {
        private static ModEntry Mod;
        public static void Init(ModEntry mod) {
            Mod = mod;
		}

        public static void textBoxPrefix(ref string s) {
            s = Mod.MakeOwO(s);
        }

        public static void drawStringPrefix(ref string s, ref int characterPosition) {
            characterPosition = 999999; // m a x  v a l u e. Not fully sure what this variable does anyway, just cuts off my string if it's too small.
            s = Mod.MakeOwO(s);
		}

        public static void microDrawStringPrefix(ref string text) {
            text = Mod.MakeOwO(text);
		}
    }
}