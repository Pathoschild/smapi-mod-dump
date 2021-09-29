/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TwinBuilderOne/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DynamicScriptLoader
{
    public class ModEntry : Mod
    {
        /*
         * TODO:
         * 
         * Replace macros with scripts
         * Scripting API
         * Script packs?
         */

        internal ModConfig Config { get; set; }

        public List<Macro> Macros { get; } = new List<Macro>();

        public List<SButton> ActiveToggles { get; } = new List<SButton>();

        public override void Entry(IModHelper helper)
        {
            Monitor.Log("Reading Config...", LogLevel.Debug);
            Config = helper.ReadConfig<ModConfig>();
            Macros.AddRange(Config.SavedMacros);

            helper.Events.Input.ButtonPressed += ButtonPressed;
            helper.Events.Input.ButtonReleased += ButtonReleased;
            helper.Events.GameLoop.UpdateTicked += Update;

            Monitor.Log("Registering Commands...", LogLevel.Debug);
            helper.ConsoleCommands.Add("macro",
                "Runs the specified macro. " +
                "To create a macro, create a text file in the macros folder with each command on its own line.\n\n" +
                "Usage: macro <name>\n" +
                "- name: the name of the macro file without the extension.",
                Macro);
            helper.ConsoleCommands.Add("addkeybind",
                "Binds a key to a macro. " +
                "To create a macro, create a text file in the macros folder with each command on its own line.\n\n" +
                "Usage: addkeybind <key> <macro> <keybindtype>\n" +
                "- key: the key that triggers the macro. To find out what keys are valid, run \"validkeys\".\n" +
                "- keybindtype: the type of key binding. To find out what the different types of key bindings are, run \"keybindtypes\".\n" +
                "- macro: the name of the macro file.",
                AddKeyBind);
            helper.ConsoleCommands.Add("viewkeybinds",
                "Prints out the current key binds.\n\n" +
                "Usage: viewkeybinds",
                ViewKeyBinds);
            helper.ConsoleCommands.Add("removekeybind",
                "Removes a key bind.\n\n" +
                "Usage: removekeybind <key> <keybindtype>\n" +
                "- key: the bound key. To find out what keys are valid, run \"validkeys\".\n" +
                "- keybindtype: the type of key binding that is being removed. " +
                "To find out what the different types of key bindings are, run \"keybindtypes\"",
                RemoveKeyBind);
            helper.ConsoleCommands.Add("savemacros",
                "Saves all the currently binded macros to the config file. " +
                "These will automatically be loaded on mod startup.\n\n" +
                "Usage: savemacros", 
                SaveMacros);
            helper.ConsoleCommands.Add("validkeys",
                "Prints a list of the valid bindable keys.\n\n" +
                "Usage: validkeys",
                ValidKeys);
            helper.ConsoleCommands.Add("keybindtypes",
                "Prints a list of the valid key binding types and how they function.\n\n" +
                "Usage: keybindtypes",
                KeyBindTypes);
        }

        private void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Macros.Where(macro => macro.Type == MacroType.Pressed).Select(macro => macro.Key).Contains(e.Button))
            {
                RunMacro(Macros.Find(macro => macro.Key == e.Button).Name);
            }

            if (ActiveToggles.Contains(e.Button))
            {
                ActiveToggles.Remove(e.Button);
            }
            else
            {
                ActiveToggles.Add(e.Button);
            }
        }

        private void ButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (Macros.Where(macro => macro.Type == MacroType.Released).Select(macro => macro.Key).Contains(e.Button))
            {
                RunMacro(Macros.Find(macro => macro.Key == e.Button).Name);
            }
        }

        private void Update(object sender, UpdateTickedEventArgs e)
        {
            foreach (Macro macro in Macros.Where(macro => macro.Type == MacroType.Held))
            {
                if (Helper.Input.IsDown(macro.Key))
                {
                    RunMacro(macro.Name);
                }
            }

            foreach (Macro macro in Macros.Where(macro => macro.Type == MacroType.Toggled))
            {
                if (ActiveToggles.Contains(macro.Key))
                {
                    RunMacro(macro.Name);
                }
            }
        }

        private void Macro(string command, string[] args)
        {
            if (args.Length == 0)
            {
                Monitor.Log("No argument <name> provided.", LogLevel.Error);
                return;
            }

            if (args.Length > 1)
            {
                Monitor.Log("Too many arguments.", LogLevel.Error);
                return;
            }

            RunMacro(args[0]);
        }

        private void AddKeyBind(string command, string[] args)
        {
            if (args.Length < 3)
            {
                Monitor.Log("No argument <keybindtype> provided.", LogLevel.Error);
                return;
            }

            if (args.Length > 3)
            {
                Monitor.Log("Too many arguments.", LogLevel.Error);
                return;
            }

            if (!Enum.TryParse(args[0], out SButton button))
            {
                Monitor.Log("Invalid key name. Run \"validkeys\" for a list of valid key names.", LogLevel.Error);
                return;
            }

            if (!Enum.TryParse(args[1], out MacroType type))
            {
                Monitor.Log("Invalid key binding type. Run \"keybindtypes\" for a list of valid macro types.", LogLevel.Error);
                return;
            }

            Macro bound = Macros.FindAll(macro => macro.Key == button).Find(macro => macro.Type == type);

            if (bound != null)
            {
                Monitor.Log($"This button is already bound to the \"{bound.Name}\" macro.", LogLevel.Warn);

                if (Config.OverwriteKeyBinds)
                {
                    Monitor.Log("According to the config file, key binds should be overwritten in this situation.", LogLevel.Warn);
                    Macros.Add(new Macro(args[2], button, type));
                    Macros.Remove(bound);
                    return;
                }
                else
                {
                    Monitor.Log("According to the config file, key binds should not be overwritten. " +
                        "To replace this bind, run \"removekeybind\" first.", LogLevel.Warn);
                    return;
                }
            }

            Macros.Add(new Macro(args[2], button, type));

            Monitor.Log("Done.", LogLevel.Info);
        }

        private void ViewKeyBinds(string command, string[] args)
        {
            if (args.Length > 0)
            {
                Monitor.Log("What are you even doing putting arguments here? Fine, I'll continue...", LogLevel.Warn);
            }

            if (!Macros.Any())
            {
                Monitor.Log("There aren't any current key binds.", LogLevel.Info);
            }

            foreach (Macro macro in Macros)
            {
                Monitor.Log(macro.ToString(), LogLevel.Info);
            }
        }

        private void RemoveKeyBind(string command, string[] args)
        {
            if (args.Length  < 2)
            {
                Monitor.Log("No argument <keybindtype> provided.", LogLevel.Error);
                return;
            }

            if (args.Length > 2)
            {
                Monitor.Log("Too many arguments.", LogLevel.Error);
                return;
            }

            if (!Enum.TryParse(args[0], out SButton button))
            {
                Monitor.Log("Invalid key name. Run \"validkeys\" for a list of valid key names.", LogLevel.Error);
                return;
            }

            if (!Enum.TryParse(args[1], out MacroType type))
            {
                Monitor.Log("Invalid key binding type. Run \"keybindtypes\" for a list of valid macro types.", LogLevel.Error);
                return;
            }

            if (Macros.Find(macro => macro.Key == button && macro.Type == type) == null)
            {
                Monitor.Log($"No macro with the {type} key binding type is bound to this key.", LogLevel.Error);
                return;
            }

            Macros.Remove(Macros.Find(macro => macro.Key == button && macro.Type == type));

            Monitor.Log("Done.", LogLevel.Info);
        }

        private void SaveMacros(string command, string[] args)
        {
            if (args.Length > 0)
            {
                Monitor.Log("What are you even doing putting arguments here? Fine, I'll continue...", LogLevel.Warn);
            }

            foreach (Macro macro in Macros)
            {
                if (Config.SavedMacros.Find(saved => saved.Key == macro.Key && saved.Type == macro.Type) == null)
                {
                    Config.SavedMacros.Add(macro);
                }
            }

            Helper.WriteConfig(Config);

            Monitor.Log("Done. To recap, the saved macros are:\n", LogLevel.Debug);

            if (!Config.SavedMacros.Any())
            {
                Monitor.Log("Oh. Nevermind, there aren't any. Why'd you even run this command?", LogLevel.Info);
            }

            foreach (Macro macro in Config.SavedMacros)
            {
                Monitor.Log(macro.ToString(), LogLevel.Info);
            }
        }

        private void ValidKeys(string command, string[] args)
        {
            if (args.Length > 0)
            {
                Monitor.Log("What are you even doing putting arguments here? Fine, I'll continue...", LogLevel.Warn);
            }

            foreach (string name in Enum.GetNames(typeof(SButton)))
            {
                Monitor.Log(name, LogLevel.Info);
            }
        }

        private void KeyBindTypes(string command, string[] args)
        {
            if (args.Length > 0)
            {
                Monitor.Log("What are you even doing putting arguments here? Fine, I'll continue...", LogLevel.Warn);
            }

            Monitor.Log("Pressed - Macro is triggered on the frame the key is pressed.", LogLevel.Info);
            Monitor.Log("Released - Macro is triggered on the frame the key is released.", LogLevel.Info);
            Monitor.Log("Held - Macro is triggered every frame that the key is held down.", LogLevel.Info);
            Monitor.Log("Toggled - Pressing the key toggles the macro on or off. " +
                "While it is toggled on, the macro triggers every frame.", LogLevel.Info);
        }

        public void RunMacro(string name)
        {
            string directoryPath = Path.Combine(Helper.DirectoryPath, "macros");

            if (!Directory.Exists(directoryPath))
            {
                Monitor.Log("It looks like the macros folder was deleted. No matter, creating it...", LogLevel.Warn);
                Directory.CreateDirectory(directoryPath);

                Monitor.Log("Ok, done. Try again when you have a macro set up.", LogLevel.Warn);
                return;
            }

            try
            {
                string[] commands = File.ReadAllLines(Path.Combine(directoryPath, name + ".txt"));

                foreach (string command in commands)
                {
                    string[] split = command.Split(' ');

                    Helper.ConsoleCommands.Trigger(split[0], split.Skip(1).ToArray());
                }
            }
            catch (ArgumentException e)
            {
                Monitor.Log($"It seems the macro file contained an invalid character.\nFull error:\n{e}", LogLevel.Error);
            }
            catch (DirectoryNotFoundException e)
            {
                Monitor.Log($"The macros folder doesn't exist (no clue how this happened).\nFull error:\n{e}", LogLevel.Error);
            }
            catch (FileNotFoundException)
            {
                Monitor.Log("That macro doesn't exist. Make sure you spelled the name correctly and the file ends in \".txt\".", LogLevel.Error);
            }
            catch (Exception e)
            {
                Monitor.Log($"An error occurred.\n{e}", LogLevel.Error);
            }
        }
    }
}
