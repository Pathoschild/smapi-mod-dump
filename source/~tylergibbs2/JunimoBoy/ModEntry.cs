/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Minigames;
using System;
using System.IO;

namespace JunimoBoy
{
    internal class ModEntry : Mod
    {
        private static IApi JsonAssetsApi { get; set; } = null!;

        public override void Entry(IModHelper helper)
        {
            I18n.Init(helper.Translation);

            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.GameLaunched += (s, e) => InitJsonAssets(helper);

            helper.ConsoleCommands.Add("jb_giveitem", "Gives you a Junimo Boy", GiveJunimoBoy);
        }

        private static void InitJsonAssets(IModHelper helper)
        {
            IApi? api = helper.ModRegistry.GetApi<IApi>("spacechase0.JsonAssets");
            if (api is null)
                throw new Exception("missing json assets dep somehow");

            api.LoadAssets(Path.Combine(helper.DirectoryPath, "assets"), helper.Translation);
            JsonAssetsApi = api;
        }

        private static int GetItemId()
        {
            return JsonAssetsApi.GetObjectId("Junimo Boy");
        }

        private void OnQuestionAnswer(Farmer who, string answer)
        {
            switch (answer)
            {
                case "Kart":
                    Game1.currentMinigame = new MineCart(0, 3);
                    break;
                case "JOTPK":
                    Game1.currentMinigame = new AbigailGame();
                    break;
            }
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || !e.Button.IsActionButton() || Game1.player.CurrentItem?.ParentSheetIndex != GetItemId())
                return;

            GameLocation location = Game1.player.currentLocation;

            var responses = new Response[3]
            {
                new Response("Kart", "Junimo Kart"),
                new Response("JOTPK", "Journey of the Prairie King"),
                new Response("Cancel", "Turn off")
            };

            location.createQuestionDialogue(I18n.Item_JunimoBoy_Startup(), responses, OnQuestionAnswer);
        }

        private void GiveJunimoBoy(string command, string[] args)
        {
            if (!Context.IsWorldReady)
                return;

            Game1.player.addItemByMenuIfNecessary(new SObject(GetItemId(), 1));
        }
    }
}
