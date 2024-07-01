/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enteligenz/StardewMods
**
*************************************************/

using System;

namespace TwitchChatIntegration
{
    public sealed class ModConfig
    {
        public string Username { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public string TargetChannel { get; set; } = String.Empty;
        public string[] IgnoredAccounts { get; set; } = Array.Empty<string>();
        public bool IgnoreCommands { get; set; } = false;
        public bool ShowSystemMessages { get; set; } = true;

        public bool IsValid()
        {
            Func<string, bool> FieldValid = (string field) =>
            {
                return !string.IsNullOrWhiteSpace(field);
            };

            return FieldValid.Invoke(this.Username) && 
                FieldValid.Invoke(this.Password) && 
                FieldValid.Invoke(this.TargetChannel) &&
                this.Password.Contains("oauth:");
        }
    }
}
