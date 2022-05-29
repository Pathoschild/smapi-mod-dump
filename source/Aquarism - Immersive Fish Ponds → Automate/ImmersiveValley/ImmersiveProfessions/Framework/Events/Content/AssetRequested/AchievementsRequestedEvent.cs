/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.Content;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

using Common.Extensions;

#endregion using directives

[UsedImplicitly]
internal class AchievementsRequestedEvent : AssetRequestedEvent
{
    /// <inheritdoc />
    protected override void OnAssetRequestedImpl(object sender, AssetRequestedEventArgs e)
    {
        if (!e.NameWithoutLocale.IsEquivalentTo("Data/achievements")) return;

        e.Edit(asset =>
        {
            // patch custom prestige achievements
            var data = asset.AsDictionary<int, string>().Data;

            string name =
                ModEntry.ModHelper.Translation.Get("prestige.achievement.name." +
                                                   (Game1.player.IsMale ? "male" : "female"));
            var desc = ModEntry.ModHelper.Translation.Get("prestige.achievement.desc");

            const string SHOULD_DISPLAY_BEFORE_EARNED_S = "false";
            const string PREREQUISITE_S = "-1";
            const string HAT_INDEX_S = "";

            var newEntry = string.Join("^", name, desc, SHOULD_DISPLAY_BEFORE_EARNED_S, PREREQUISITE_S, HAT_INDEX_S);
            data[name.GetDeterministicHashCode()] = newEntry;
        });
    }
}