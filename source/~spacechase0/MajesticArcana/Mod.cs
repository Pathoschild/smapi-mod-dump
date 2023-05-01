/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using SpaceShared;
using StardewModdingAPI;
using StardewValley;

namespace MajesticArcana
{
    public class Mod : StardewModdingAPI.Mod
    {
        public static Mod instance;
        internal const string SpellStashKey = "spacechase0.MajesticArcana/SpellStash";

        internal static Dictionary<string, Texture2D> SpellIcons = new();

        public override void Entry(IModHelper helper)
        {
            instance = this;
            Log.Monitor = Monitor;
            I18n.Init(Helper.Translation);

            // TODO: Resize these smaller into a tilesheet
            foreach (string png in Directory.EnumerateFiles(Path.Combine(this.Helper.DirectoryPath, "assets", "spell-icons")))
            {
                string filename = Path.GetFileName(png);
                if (!filename.EndsWith(".png"))
                    continue;

                SpellIcons.Add(filename, Helper.ModContent.Load<Texture2D>(Path.Combine("assets", "spell-icons", filename)));
            }

            AlchemyRecipes.Init();

            Helper.ConsoleCommands.Add("magik_alchemy", "...", OnAlchemyCommand);
            Helper.ConsoleCommands.Add("magik_spellcrafting", "...", OnSpellcraftingCommand);
        }

        private void OnAlchemyCommand(string arg1, string[] arg2)
        {
            if (!Context.IsPlayerFree)
                return;

            Game1.activeClickableMenu = new AlchemyMenu();
        }

        private void OnSpellcraftingCommand(string arg1, string[] arg2)
        {
            if (!Context.IsPlayerFree)
                return;

            Game1.activeClickableMenu = new SpellcraftingMenu();
        }
    }
}
