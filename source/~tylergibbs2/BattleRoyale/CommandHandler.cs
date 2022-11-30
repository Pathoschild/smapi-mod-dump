/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using BattleRoyale.Utils;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleRoyale
{
    class CommandHandler
    {
        private static readonly Dictionary<string, Action<string[]>> commands = new()
        {
            { "kill", Kill },
            { "spectate", Spectate },
            { "play", Play },
            { "start", Start },
            { "lobby", Lobby },
            { "name", Name },
            { "character", Character },
            { "specialround", SpecialRound }
        };

        public static bool Handle(string command)
        {
            command = command.Trim();
            string[] args = command.Split(' ');

            if (commands.ContainsKey(args[0]))
            {
                commands[args[0]](args.Skip(1).ToArray());
                return false;
            }
            else if (args[0] == "pause")
            {
                Game1.chatBox.addErrorMessage("Cannot pause with Battle Royalley mod.");
                return false;
            }

            return true;
        }

        public static void Kill(string[] args)
        {
            Round round = ModEntry.BRGame.GetActiveRound();
            if (!ModEntry.BRGame.InProgress)
            {
                Game1.chatBox.addErrorMessage("Game is not active.");
                return;
            }
            else if (!(bool)round?.AlivePlayers.Contains(Game1.player))
            {
                Game1.chatBox.addErrorMessage("You are already dead.");
                return;
            }
            FarmerUtils.TakeDamage(Game1.player, DamageSource.PLAYER, 1000, Game1.player.UniqueMultiplayerID);
        }

        public static void Spectate(string[] args)
        {
            if (ModEntry.BRGame.isSpectating)
            {
                Game1.chatBox.addErrorMessage("You are already spectating. Type /play to stop spectating.");
                return;
            }

            NetworkMessage.Send(
                NetworkUtils.MessageTypes.TOGGLE_SPECTATE,
                NetworkMessageDestination.ALL,
                new List<object>() { Game1.player.UniqueMultiplayerID, true }
            );

            Game1.chatBox.addInfoMessage("You are now spectating. Type /play to stop spectating.");
        }

        public static void Play(string[] args)
        {
            if (!ModEntry.BRGame.isSpectating)
            {
                Game1.chatBox.addErrorMessage("You are already playing. Type /spectate to start spectating.");
                return;
            }

            NetworkMessage.Send(
                NetworkUtils.MessageTypes.TOGGLE_SPECTATE,
                NetworkMessageDestination.ALL,
                new List<object>() { Game1.player.UniqueMultiplayerID, false }
            );

            Game1.chatBox.addInfoMessage("You will begin playing next round.");
        }

        public static void Start(string[] args)
        {
            if (Game1.player != Game1.MasterPlayer)
            {
                Game1.chatBox.addErrorMessage("Only the host can start the game.");
                return;
            }

            ModEntry.BRGame.Play();
        }

        public static void Lobby(string[] args)
        {
            if (Game1.player != Game1.MasterPlayer)
            {
                Game1.chatBox.addErrorMessage("Only the host can return to lobby.");
                return;
            }
            else if (ModEntry.BRGame.inLobby)
            {
                Game1.chatBox.addErrorMessage("The game is already in the lobby.");
                return;
            }
            else if (ModEntry.BRGame.lastRound)
            {
                Game1.chatBox.addErrorMessage("Already returning to lobby at the end of the round.");
                return;
            }

            ModEntry.BRGame.lastRound = true;

            Round round = ModEntry.BRGame.GetActiveRound();
            if (round != null && round.AlivePlayers.Count <= 1)
                round.HandleWin(null, null);
            else
                Game1.chatBox.addInfoMessage("Players will return to the lobby at the end of the round.");
        }

        public static void Name(string[] args)
        {
            if (ModEntry.BRGame.InProgress)
            {
                Game1.chatBox.addErrorMessage("Cannot customize character in the middle of a round.");
                return;
            }

            string name = string.Join(" ", args);
            if (name.Length > 12)
            {
                Game1.chatBox.addErrorMessage("Name must be less than 13 characters.");
                return;
            }
            else if (name.Trim().Length == 0)
            {
                Game1.chatBox.addErrorMessage("Invalid name.");
                return;
            }

            Game1.player.Name = name;

            Game1.chatBox.addInfoMessage("Successfully changed username.");
        }

        public static void Character(string[] args)
        {
            if (ModEntry.BRGame.InProgress)
            {
                Game1.chatBox.addErrorMessage("Cannot customize character in the middle of a round.");
                return;
            }
            else if (Game1.activeClickableMenu != null)
            {
                Game1.chatBox.addErrorMessage("Close existing menu before customizing your character.");
                return;
            }

            Game1.activeClickableMenu = new CharacterCustomization(CharacterCustomization.Source.NewFarmhand);
        }

        public static void SpecialRound(string[] args)
        {
            if (!Game1.IsMasterGame)
            {
                Game1.chatBox.addErrorMessage("Only the host of the game can force a special round.");
                return;
            }
            if (ModEntry.BRGame.ForceSpecialRound)
            {
                ModEntry.BRGame.ForceSpecialRound = false;
                Game1.chatBox.addInfoMessage("No longer going to force a special round.");
            }
            else
            {
                ModEntry.BRGame.ForceSpecialRound = true;
                Game1.chatBox.addInfoMessage("Next round will be a special round.");
            }
        }
    }
}
