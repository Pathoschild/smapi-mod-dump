/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System;
using System.Linq;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

using Common.Extensions;
using Extensions;
using SuperMode;

#endregion using directives

[UsedImplicitly]
internal class StaticSaveLoadedEvent : SaveLoadedEvent
{
    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object sender, SaveLoadedEventArgs e)
    {
        // enable events
        EventManager.EnableAllForLocalPlayer();

        // load or initialize Super Mode index
        var superModeIndex = Enum.Parse<SuperModeIndex>(ModData.Read(DataField.SuperModeIndex, defaultValue: "None"));

        // validate Super Mode index
        switch (superModeIndex)
        {
            case <= SuperModeIndex.None when Game1.player.professions.Any(p => p is >= 26 and < 30):
                Log.W("Player eligible for Super Mode but not currently registered to any. Setting to a default value.");
                superModeIndex = (SuperModeIndex) Game1.player.professions.First(p => p is >= 26 and < 30);
                ModData.Write(DataField.SuperModeIndex, superModeIndex.ToString());

                break;

            case > SuperModeIndex.None when !Game1.player.professions.Contains((int) superModeIndex):
                Log.W($"Missing corresponding profession for {superModeIndex} Super Mode. Resetting to a default value.");
                if (Game1.player.professions.Any(p => p is >= 26 and < 30))
                {
                    superModeIndex = (SuperModeIndex) Game1.player.professions.First(p => p is >= 26 and < 30);
                    ModData.Write(DataField.SuperModeIndex, superModeIndex.ToString());
                }
                else
                {
                    superModeIndex = SuperModeIndex.None;
                    ModData.Write(DataField.SuperModeIndex, null);
                }

                break;
        }

        // initialize Super Mode
        if (superModeIndex > SuperModeIndex.None) ModEntry.State.Value.SuperMode = new(superModeIndex);

        // check for prestige achievements
        if (!Game1.player.HasAllProfessions()) return;

        string name =
            ModEntry.ModHelper.Translation.Get("prestige.achievement.name." +
                                               (Game1.player.IsMale ? "male" : "female"));
        if (Game1.player.achievements.Contains(name.GetDeterministicHashCode())) return;

        EventManager.Enable(typeof(AchievementUnlockedDayStartedEvent));
    }
}