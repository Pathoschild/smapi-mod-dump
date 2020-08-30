using Hello_SMAPI.Framework;
using StardewModdingAPI;
using System;

namespace Hello_SMAPI
{
    class ModEntry : Mod
    {
        private ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            Config = this.Helper.ReadConfig<ModConfig>();

            helper.ConsoleCommands.Add("hello", "Hello SMAPI \n\n Usage: hello", this.Hello);
            helper.ConsoleCommands.Add("hi", "Hi SMAPI \n\n Usage: hi", this.Hello);
            helper.ConsoleCommands.Add("hey", "Hey SMAPI \n\n Usage: hey", this.Hello);
        }

        private void Hello(string command, string[] args)
        {
            if (command == "hello" || command == "hey" || command == "hi")
            {
                var rand = new Random();

                int index = rand.Next(this.Config.Responses.Length);

                this.Monitor.Log(this.Config.Responses[index], LogLevel.Info);
            }
        }
    }
}
